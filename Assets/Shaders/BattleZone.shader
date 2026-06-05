Shader "Custom/BattleZone"
{
    Properties
    {
        _OutsideColor ("Outside Tint",  Color)  = (0.05, 0.05, 0.2, 0.5)
        _RingColor    ("Ring Color",    Color)  = (0.2,  0.7,  1.0, 1.0)
        _RingWidth    ("Ring Width",    Float)  = 1.5
        _RingGlow     ("Ring Glow",     Float)  = 3.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent+1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "BattleZone"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Per-material tweakables
            CBUFFER_START(UnityPerMaterial)
                half4 _OutsideColor;
                half4 _RingColor;
                float _RingWidth;
                float _RingGlow;
            CBUFFER_END

            // Global properties — set by ZoneManager via Shader.SetGlobalXxx.
            // Must be OUTSIDE UnityPerMaterial or the per-material value wins.
            float4 _ZoneCenter;
            float  _ZoneRadius;

            struct Attributes { float4 positionOS : POSITION; };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.worldPos   = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.worldPos);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 offset = IN.worldPos.xz - _ZoneCenter.xz;
                float  dist   = length(offset);

                // --- Outside tint ---
                float outside = step(_ZoneRadius, dist);
                half4 col;
                col.rgb = _OutsideColor.rgb;
                col.a   = _OutsideColor.a * outside;

                // --- Ring glow (straddles the boundary) ---
                float ringDist = abs(dist - _ZoneRadius);

                // Sharp inner ring
                float ring = 1.0 - saturate(ringDist / max(_RingWidth, 0.001));
                ring = pow(ring, 0.35);

                // Wide soft glow halo
                float glow = 1.0 - saturate(ringDist / max(_RingGlow, 0.001));
                glow = pow(glow, 2.5) * 0.5;

                float ringTotal = saturate(ring + glow);

                // Blend ring on top of tint
                col.rgb = lerp(col.rgb, _RingColor.rgb, ringTotal);
                col.a   = saturate(col.a + ringTotal * _RingColor.a);

                return col;
            }
            ENDHLSL
        }
    }
}
