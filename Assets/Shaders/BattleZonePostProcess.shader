Shader "Custom/BattleZonePostProcess"
{
    Properties
    {
        _OutsideColor        ("Outside Tint",        Color)        = (0.05, 0.05, 0.2, 0.45)
        _RingColor           ("Ring Color",           Color)        = (0.2,  0.7,  1.0, 1.0)
        _RingWidth           ("Ring Width",           Float)        = 1.5
        _RingGlow            ("Ring Glow",            Float)        = 3.0
        _OutsideDesaturation ("Outside Desaturation", Range(0, 1))  = 0.6
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "BattleZonePostProcess"
            ZWrite Off
            Cull   Off
            ZTest  Always
            Blend  Off

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _OutsideColor;
                half4 _RingColor;
                float _RingWidth;
                float _RingGlow;
                float _OutsideDesaturation;
            CBUFFER_END

            // Set globally every frame by ZoneManager
            float4 _ZoneCenter;
            float  _ZoneRadius;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;

                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // Reconstruct world-space position from depth
                float depth = SampleSceneDepth(uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);

                float dist = length(worldPos.xz - _ZoneCenter.xz);

                // Outside: desaturate + tint
                float outside = step(_ZoneRadius, dist);
                float lum     = dot(color.rgb, half3(0.299, 0.587, 0.114));
                color.rgb     = lerp(color.rgb, lerp(half3(lum, lum, lum), _OutsideColor.rgb, _OutsideColor.a), outside * _OutsideDesaturation);

                // Ring glow straddling the boundary
                float ringDist  = abs(dist - _ZoneRadius);
                float ring      = pow(1.0 - saturate(ringDist / max(_RingWidth, 0.001)), 0.35);
                float halo      = pow(1.0 - saturate(ringDist / max(_RingGlow,  0.001)), 2.5) * 0.5;
                float ringTotal = saturate(ring + halo);

                color.rgb = lerp(color.rgb, _RingColor.rgb, ringTotal * _RingColor.a);

                return color;
            }
            ENDHLSL
        }
    }
}
