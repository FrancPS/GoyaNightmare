Shader "Custom/PostProcess"
{
    Properties
    {
        _MainTex ("Main Texture (Screen default)", 2D) = "white" {}
        
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DistortionFactor("Distortion", Range(0.0, 1.0)) = 0

        _DarknessFactor ("Darkness", Range(0.0, 1.0)) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Vertex variables
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

            // Vertex functions

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment variables

            sampler2D _MainTex;
    
            sampler2D _NoiseTex;
            float _DistortionFactor;

            float _DarknessFactor;

            // Fragment functions
            float2 GetDistortionUVs(float2 uv) {

                float2 uv1 = uv + _Time[1] * float2(0.03, 0.22);
                float2 uv2 = uv + _Time[1] * float2(0.1, 0.02);

                float2 uvs1 = tex2D(_NoiseTex, uv1).rg;
                float2 uvs2 = tex2D(_NoiseTex, uv2).rb;

                float2 finalUVs = uv + _DistortionFactor * 0.03 *(uvs1 + uvs2);

                return finalUVs;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 newUV = i.uv;
                if (_DistortionFactor > 0) {
                    newUV = GetDistortionUVs(i.uv);
                }

                fixed4 col = tex2D(_MainTex, newUV);
                
                col.rgb = col.rgb * (1 - _DarknessFactor);
                return col;
            }
            ENDCG
        }
    }
}
