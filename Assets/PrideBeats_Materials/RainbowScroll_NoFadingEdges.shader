Shader "Custom/RainbowScroll_NoFadingEdges"
{
    Properties
    {
        _Speed ("Scroll Speed", Float) = 1.0
        _Randomness ("Randomness Strength", Float) = 0.5
        _Alpha ("Transparency (Alpha)", Range(0,1)) = 1.0
        _Metallic ("Fake Metallic Intensity", Range(0,1)) = 0.0
        _Smoothness ("Fake Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Lighting On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Speed;
            float _Randomness;
            float _Alpha;
            float _Metallic;
            float _Smoothness;

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            float3 HSVtoRGB(float h, float s, float v)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(h + K.xyz) * 6.0 - K.www);
                return v * lerp(K.xxx, saturate(p - K.xxx), s);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Rainbow color scroll with randomness
                float noise = rand(i.uv) * _Randomness;
                float hue = frac(i.uv.x + _Time.y * _Speed + noise);
                float3 baseColor = HSVtoRGB(hue, 1.0, 1.0);

                // Lighting simulation (specular)
                float3 normal = normalize(i.normalDir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(lightDir + viewDir);

                float NdotL = saturate(dot(normal, lightDir));
                float NdotH = saturate(dot(normal, halfDir));

                float spec = pow(NdotH, lerp(10.0, 200.0, _Smoothness)) * _Metallic;

                float3 litColor = baseColor * (0.5 + 0.5 * NdotL) + spec;

                return float4(litColor, _Alpha);
            }
            ENDCG
        }
    }
}
