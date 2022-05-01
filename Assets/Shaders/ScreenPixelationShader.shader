Shader "Custom/ScreenPixelationShader"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Saturate ("Saturate", Range(1,2)) = 1
        _Contrast ("Contrast", Range(1,0)) = 1
        _Pixelation ("Pixelation", Range(1,1000)) = 100
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Saturate;
            float _Contrast;
            int _Pixelation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 pixelArt(float2 uv, const float pixelSample)
            {
                half pixel = 1 / pixelSample;
                half2 pixeledUV = half2((int)(uv.x / pixel) * pixel, (int)(uv.y / pixel) * pixel);
                return pixeledUV;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, pixelArt(i.uv, _Pixelation)) * _Saturate;
                fixed4 contrast = col * col;

                fixed4 render = lerp(contrast, col, _Contrast);
                return render;
            }
            ENDCG
        }
    }
}
