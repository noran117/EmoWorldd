Shader "Custom/blend"
{
    Properties
    {
        _Tex1 ("Panorama 1", 2D) = "grey" {}
        _Tex2 ("Panorama 2", 2D) = "grey" {}
        _Blend ("Blend", Range(0,1)) = 0
        _Exposure ("Exposure", Range(0, 8)) = 1
        _Rotation ("Rotation", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Tex1;
            sampler2D _Tex2;
            float _Blend;
            float _Exposure;
            float _Rotation;

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            float2 DirectionToEquirect(float3 dir, float rotDeg)
            {
                float rotRad = rotDeg * (3.14159265 / 180.0);
                float cosR = cos(rotRad);
                float sinR = sin(rotRad);
                float x = dir.x * cosR - dir.z * sinR;
                float z = dir.x * sinR + dir.z * cosR;
                dir = float3(x, dir.y, z);

                float2 uv;
                uv.x = (atan2(dir.z, dir.x) / (2.0 * 3.14159265)) + 0.5;
                uv.y = asin(clamp(dir.y / length(dir), -1, 1)) / 3.14159265 + 0.5;
                return uv;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = DirectionToEquirect(normalize(i.texcoord), _Rotation);

                fixed4 col1 = tex2D(_Tex1, uv);
                fixed4 col2 = tex2D(_Tex2, uv);

                fixed4 col = lerp(col1, col2, _Blend);
                col.rgb *= _Exposure;
                return col;
            }
            ENDCG
        }
    }
}