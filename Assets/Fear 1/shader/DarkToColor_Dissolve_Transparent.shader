Shader "Custom/DarkToColor/MagicalTransparent"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        _DarkTint ("Dark Tint", Color) = (0.05, 0.05, 0.12, 1)

        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1

        _Dissolve ("Dissolve", Range(0,1)) = 0
        _EdgeWidth ("Edge Width", Range(0.001,0.2)) = 0.05
        _EdgeColor ("Magic Edge Color", Color) = (1,0.5,1,1)
        _MagicPower ("Magic Power", Range(1,6)) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        Pass
        {
            HLSLPROGRAM
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

            sampler2D _BaseMap;
            sampler2D _NoiseTex;

            float4 _BaseColor;
            float4 _DarkTint;
            float4 _EdgeColor;

            float _NoiseScale;
            float _Dissolve;
            float _EdgeWidth;
            float _MagicPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _NoiseScale;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 baseTex = tex2D(_BaseMap, i.uv) * _BaseColor;
                float noise = tex2D(_NoiseTex, i.uv).r;

                noise = pow(noise, _MagicPower);

                float mask = smoothstep(
                    _Dissolve - _EdgeWidth,
                    _Dissolve + _EdgeWidth,
                    noise
                );

                float edge = smoothstep(
                    _Dissolve,
                    _Dissolve + _EdgeWidth,
                    noise
                ) - mask;

                float3 darkColor = baseTex.rgb * _DarkTint.rgb;
                float3 finalRGB = lerp(darkColor, baseTex.rgb, mask);
                finalRGB += edge * _EdgeColor.rgb;

                // ✅ Alpha is CLEAN and independent
                float alpha = baseTex.a * mask;

                return float4(finalRGB, alpha);
            }
            ENDHLSL
        }
    }
}