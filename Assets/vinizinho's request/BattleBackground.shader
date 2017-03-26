Shader "Custom/BattleBackground"
{
	Properties
	{
		//Layer 1
		_Layer1 ("Layer 1", 2D) = "black" {}
		_Palette1("Color Palette", 2D) = "white"
		_ColorSpeed1("Color Speed", Float) = 1.0
		_Amp1("Amplitude", Float) = 0.0
		_Freq1("Frequency", Float) = 0.0
		_Speed1("Speed", Float) = 0.0
		_HorzT1("Horizontal Translation", Float) = 0.0
		_HorzInterT1("Horizontal Interlaced Translation", Float) = 0.0
		_VertT1("Vertical Translation", Float) = 0.0

		//Layer 2
		_Layer2("Layer 2", 2D) = "black" {}
		_Palette2("Color Palette", 2D) = "white"
		_ColorSpeed2("Color Speed", Float) = 1.0
		_Amp2("Amplitude", Float) = 0.0
		_Freq2("Frequency", Float) = 0.0
		_Speed2("Speed", Float) = 0.0
		_HorzT2("Horizontal Translation", Float) = 0.0
		_HorzInterT2("Horizontal Interlaced Translation", Float) = 0.0
		_VertT2("Vertical Translation", Float) = 0.0
	}
	SubShader
	{
		Tags 
		{
			"Queue" = "Background"
			"RenderType" = "Opaque"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			int _TexH;

			sampler2D _Layer1;
			float4 _Layer1_TexelSize;
			sampler2D _Palette1;
			float _ColorSpeed1;
			float _Amp1;
			float _Freq1;
			float _Speed1;

			float _HorzT1;
			float _HorzInterT1;
			float _VertT1;

			sampler2D _Layer2;
			float4 _Layer2_TexelSize;
			sampler2D _Palette2;
			float _ColorSpeed2;
			float _Amp2;
			float _Freq2;
			float _Speed2;

			float _HorzT2;
			float _HorzInterT2;
			float _VertT2;

			float offset(float y, float amp, float freq, float speed)
			{
				return amp * sin(freq * y + speed * _Time);
			}

			float4 layer(sampler2D layer, float4 texelSize, float2 uv, float amp, float freq, float speed, float horzT, float horzInterT, float vertT)
			{
				float y = round((uv.y / texelSize.y) + 0.5);
				float off = offset(y, amp, freq, speed);
				// Horizontal translations
				uv.x += off * horzT;
				// Horizontal interlaced translations
				if (y % 2)
					uv.x += off * horzInterT;
				else
					uv.x -= off * horzInterT;
				// Vertical compression translations
				off = offset(uv.y, amp, freq, speed);
				uv.y += off * vertT;
				float4 col = tex2D(layer, uv);
				return col;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{			

				fixed4 layer1 = layer(_Layer1, _Layer1_TexelSize, i.uv1, _Amp1, _Freq1, _Speed1, _HorzT1, _HorzInterT1, _VertT1);
				fixed4 layer2 = layer(_Layer2, _Layer2_TexelSize, i.uv2, _Amp2, _Freq2, _Speed2, _HorzT2, _HorzInterT2, _VertT2);

				half index = layer1.r + _Time * _ColorSpeed1;
				fixed4 col = tex2D(_Palette1, float2(index, 0));

				index = layer2.r + _Time * _ColorSpeed2;
				col += tex2D(_Palette2, float2(index, 0));

				col /= 2;

				return col;
			}
			ENDCG
		}
	}
}
