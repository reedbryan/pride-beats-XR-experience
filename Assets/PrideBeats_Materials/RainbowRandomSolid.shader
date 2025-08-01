Shader "Custom/RainbowRandomSolid"
{
    Properties
    {
        _Saturation ("Saturation", Range(0,1)) = 1.0
        _Value ("Brightness", Range(0,1)) = 1.0
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

            float _Saturation;
            float _Value;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Simple hash based on position
            float hash(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898, 78.233, 37.719))) * 43758.5453);
            }

            float3 HSVtoRGB(float h, float s, float v)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(h + K.xyz) * 6.0 - K.www);
                return v * lerp(K.xxx, saturate(p - K.xxx), s);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Get a hue based on the world position of the object
                float hue = hash(floor(i.worldPos)); // consistent per object
                float3 rgb = HSVtoRGB(hue, _Saturation, _Value);
                return float4(rgb, 1.0);
            }
            ENDCG
        }
    }
}
