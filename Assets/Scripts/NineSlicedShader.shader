/*
*Author: 邵志恒
*Blog: http://blog.csdn.net/rickshaozhiheng/article/details/53608168
*Email: zhiheng.rick@gmail.com
*Shader修改自unity5.5.0内置shader
* set Image Import Setting ->MeshType to Full Rect
*/
Shader "Rickshao/NineSlicedShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector][MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
 
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"
 
            float top;
            float bottom;
            float left;
            float right;
            float sx;
            float sy;
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                //UNITY_VERTEX_INPUT_INSTANCE_ID //copied from 5.5, not work in 5.4
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                //UNITY_VERTEX_OUTPUT_STEREO
            };
 
            fixed4 _Color;
 
            float2 UVTransform(float2 origin)
            {
                float2 result = origin;
 
                if(origin.x * sx < left)
                {
                    result.x = origin.x * sx;
                }else
                {
                    if((1 - origin.x) * sx < right)
                    {
                        result.x = 1 - (1 - origin.x) * sx ;
                    }else
                    {
                        result.x = (origin.x * sx - left)/(sx -left - right)*(1 - left - right) + left;
                    }
                }
 
                if(origin.y * sy < top)
                {
                    result.y = origin.y * sy;
                }else
                {
                    if((1 - origin.y) * sy < bottom)
                    {
                        result.y = 1 - (1 - origin.y) * sy ;
                    }else
                    {
                        result.y = (origin.y * sy - top)/(sy - top - bottom)*(1 - top - bottom) + top;
                    }
                }
 
                return result;
            }
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
 
                return OUT;
            }
 
            sampler2D _MainTex;
            sampler2D _AlphaTex;
 
            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);
 
#if ETC1_EXTERNAL_ALPHA
                // get the color from an external texture (usecase: Alpha support for ETC1 on android)
                color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA
 
                return color;
            }
 
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (UVTransform(IN.texcoord)) * IN.color;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}