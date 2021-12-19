Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Space(10)]
        _OutColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutValue ("Outline Value", Range(0.0, 0.1)) = 0
    }
    SubShader
    {
        // ------- Outline Pass ------------
        Pass
        {
            Tags 
            {
                "Queue" = "Geometry"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off ZWrite Off ZTest Always

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

            float _OutValue;

            float4 Outline(float4 vertexPos, float outValue)
            {
                float4x4 scale = float4x4
                    (
                        1 + outValue, 0, 0, 0,
                        0, 1 + outValue, 0, 0,
                        0, 0, 1 + outValue, 0,
                        0, 0, 0, 1 + outValue
                    );

                return mul(scale, vertexPos);
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4 vertexPos = Outline(v.vertex, _OutValue);
                o.vertex = UnityObjectToClipPos(vertexPos);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _OutColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return float4(_OutColor.r, _OutColor.g, _OutColor.b, col.a);
            }
            ENDCG
        }

        // ------- Texture Pass ------------
        Pass
        {
            Tags
            {
                "Queue" = "Geometry+1"
            }

            Blend SrcAlpha OneMinusSrcAlpha

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
