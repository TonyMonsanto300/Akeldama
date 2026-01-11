Shader "Custom/BodySkinShader"
{
    Properties
    {
        _MainTex    ("Texture", 2D)       = "white" {}
        _FillColor  ("Fill Color 1", Color) = (1,0.9,0.8,1)
        _FillColor2 ("Fill Color 2", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _FillColor;
            float4    _FillColor2;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float Dist2(int3 a, int3 b)
            {
                float3 d = (float3)a - (float3)b;
                return dot(d, d);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);

                float3 srgbCol;
                #if defined(UNITY_COLORSPACE_GAMMA)
                    srgbCol = texCol.rgb;
                #else
                    srgbCol = LinearToGammaSpace(texCol.rgb);
                #endif

                float3 c255 = srgbCol * 255.0;
                int3   ci   = int3(round(c255));

                // SKIN MARKER #1 = FF6A00
                int3 skinIdx1 = int3(255, 106, 0);
                // SKIN MARKER #2 = 89FF00
                int3 skinIdx2 = int3(137, 255, 0);

                float eps = 3.0;

                float d1 = Dist2(ci, skinIdx1);
                float d2 = Dist2(ci, skinIdx2);

                if (d1 < eps)
                    return fixed4(_FillColor.rgb, texCol.a);

                if (d2 < eps)
                    return fixed4(_FillColor2.rgb, texCol.a);

                return texCol;
            }
            ENDCG
        }
    }
}
