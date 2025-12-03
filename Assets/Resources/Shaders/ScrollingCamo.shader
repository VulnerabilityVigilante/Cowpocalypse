Shader "Cowpocalypse/ScrollingCamo_StaticMatrix"
{
    Properties
    {
        _MainTex ("Camo Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 0.5
        _Scale ("Scale", Float) = 0.5

        // 4 rows of a static matrix
        _StaticRow0 ("StaticRow0", Vector) = (1,0,0,0)
        _StaticRow1 ("StaticRow1", Vector) = (0,1,0,0)
        _StaticRow2 ("StaticRow2", Vector) = (0,0,1,0)
        _StaticRow3 ("StaticRow3", Vector) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ScrollSpeed;
            float _Scale;

            float4 _StaticRow0;
            float4 _StaticRow1;
            float4 _StaticRow2;
            float4 _StaticRow3;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 staticPos : TEXCOORD0;
                float3 staticNormal : TEXCOORD1;
            };

            float4x4 GetStaticMatrix()
            {
                return float4x4(_StaticRow0, _StaticRow1, _StaticRow2, _StaticRow3);
            }

            v2f vert (appdata v)
            {
                v2f o;

                float4x4 M = GetStaticMatrix();

                // transform into STATIC coordinate space
                float4 staticPos4 = mul(M, v.vertex);
                float3 staticNorm = mul((float3x3)M, v.normal);

                o.staticPos = staticPos4.xyz;
                o.staticNormal = normalize(staticNorm);

                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 p = i.staticPos;

                // Cylinder around Z axis (blue arrow)
                float angle = atan2(p.y, p.x);
                float u = (angle / (2 * UNITY_PI)) + 0.5;

                // Scroll downward along Z
                float v = (p.z * _Scale) - (_ScrollSpeed * _Time.y);

                float2 uv = float2(u * _Scale, v);

                return tex2D(_MainTex, uv);
            }



            ENDCG
        }
    }
}
