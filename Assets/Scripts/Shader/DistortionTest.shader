Shader "Custom/Distortion Test"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_NoiseTex ("Noise Texture", 2D) = "white" {}

		_ColorMask ("Color Mask", Float) = 15
		
		_Amplitude ("Amplitude", Float) = 50
		_Frequency ("Frequency", Float) = 50
		_Frameskip ("Frameskip", Float) = 50
		_Modulo ("Modulo", Float) = 50000
		_Lineskip ("Lineskip", Float) = 10

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0) * float2(-1,1) * OUT.vertex.w;
				#endif
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			float _Amplitude;
			float _Frequency;
			float _Frameskip;
			float _Modulo;
			float _Lineskip;
			uniform float4 _MainTex_TexelSize;

			fixed4 frag(v2f IN) : SV_Target
			{
				float amplitude = _Amplitude / 1000;

				float offset = amplitude * sin(IN.texcoord.y * _Frequency + _Time[0] * _Frameskip);
				//float2 pos = float2(IN.vertex.x, IN.vertex.y);
				
				//horizontal translation
				//float2 pos = float2(IN.vertex.x - offset, IN.vertex.y);

				float aux = sign(floor(fmod((IN.texcoord.y + 2) * 5000000, 2)) - 0.5);
				//horizontal interlaced translation
				float2 pos = float2(IN.texcoord.x + offset * aux,
									IN.texcoord.y);
				
				// float aux = IN.texcoord.x + 1 * _Time[0];
				// if (aux > 1)
				// 	aux = aux - 1;

				// float2 pos = float2(aux, IN.texcoord.y);

				half4 color = (tex2D(_MainTex, pos) + _TextureSampleAdd) * IN.color;

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}
