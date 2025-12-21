Shader "Custom/AvatarHeadShader"
{
    Properties
    {
        _MainTex      ("Base Texture", 2D) = "white" {}

        _EyeColor     ("Eye Color", Color)     = (1,1,1,1)
        _SkinColor    ("Skin Color", Color)    = (1,0.9,0.8,1)
        _SkinColor2   ("Skin 2 Color", Color)  = (1,0.8,0.7,1)   // NEW
        _HairColor1   ("Hair 1 Color", Color)  = (0.1,0.1,0.1,1)
        _HairColor2   ("Hair 2 Color", Color)  = (0.2,0.2,0.2,1)
        _PaintColor   ("Paint Color", Color)   = (1,0,0,1)

        _Cutoff       ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "TransparentCutout"
            "Queue"      = "AlphaTest"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            float4 _EyeColor;
            float4 _SkinColor;
            float4 _SkinColor2;   // NEW
            float4 _HairColor1;
            float4 _HairColor2;
            float4 _PaintColor;

            float  _Cutoff;

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

            // squared distance between two integer RGB triplets
            float Dist2(int3 a, int3 b)
            {
                float3 d = (float3)a - (float3)b;
                return dot(d, d);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);

                // Respect alpha cutout
                clip(texCol.a - _Cutoff);

                // Convert to 0–255 integer color
                float3 c255 = texCol.rgb * 255.0;
                int3 ci = int3(round(c255));

                // --- EXEMPT COLORS -----------------------------------
                int3 exemptWhite = int3(253, 251, 252); // FDFBFC
                int3 exemptBlack = int3(0, 0, 0);       // 000000

                float epsExempt = 3.0; // tolerance for sRGB/rounding

                if (Dist2(ci, exemptWhite) < epsExempt ||
                    Dist2(ci, exemptBlack) < epsExempt)
                {
                    return texCol; // do NOT recolor
                }
                // ------------------------------------------------------

                // Marker colors (0–255) for bucket quantization
                int3 eyeIdx    = int3(182, 255,   0); // B6FF00
                int3 skinIdx   = int3(255, 106,   0); // FF6A00
                int3 skin2Idx  = int3(255,   0,   0); // FF0000  (NEW)
                int3 hair1Idx  = int3(255,   0, 220); // FF00DC
                int3 hair2Idx  = int3(  0, 148, 255); // 0094FF
                int3 paintIdx  = int3(  0, 198,  29); // 00C61D

                // Distances to each marker
                float dEye    = Dist2(ci, eyeIdx);
                float dSkin   = Dist2(ci, skinIdx);
                float dSkin2  = Dist2(ci, skin2Idx);
                float dHair1  = Dist2(ci, hair1Idx);
                float dHair2  = Dist2(ci, hair2Idx);
                float dPaint  = Dist2(ci, paintIdx);

                // Pick the nearest marker
                float best = dEye;
                int which = 0; // 0=eye,1=skin,2=skin2,3=hair1,4=hair2,5=paint

                if (dSkin  < best) { best = dSkin;  which = 1; }
                if (dSkin2 < best) { best = dSkin2; which = 2; }
                if (dHair1 < best) { best = dHair1; which = 3; }
                if (dHair2 < best) { best = dHair2; which = 4; }
                if (dPaint < best) { best = dPaint; which = 5; }

                // Force pixel into the correct bucket
                fixed3 outColor;
                if      (which == 0) outColor = _EyeColor.rgb;
                else if (which == 1) outColor = _SkinColor.rgb;
                else if (which == 2) outColor = _SkinColor2.rgb; // NEW
                else if (which == 3) outColor = _HairColor1.rgb;
                else if (which == 4) outColor = _HairColor2.rgb;
                else                 outColor = _PaintColor.rgb;

                return fixed4(outColor, texCol.a);
            }
            ENDCG
        }
    }
}
