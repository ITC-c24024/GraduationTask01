Shader "Custom/SpriteSheetCutoutShadow"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Columns ("Columns", Float) = 1
        _Rows ("Rows", Float) = 1
        _Frame ("Frame", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }

        // 見た目用
        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float _Columns, _Rows, _Frame, _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float f = floor(_Frame);
                float col = fmod(f, _Columns);
                float row = floor(f / _Columns);

                float2 scale = float2(1.0/_Columns, 1.0/_Rows);
                o.uv = v.uv * scale + float2(col*scale.x, 1.0-scale.y-row*scale.y);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = max(0, dot(i.worldNormal, lightDir));
                col.rgb *= ndotl * _LightColor0.rgb + 0.2;

                return col;
            }
            ENDCG
        }

        // ★影用（ここが本命）
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragShadow
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Columns, _Rows, _Frame, _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float f = floor(_Frame);
                float col = fmod(f, _Columns);
                float row = floor(f / _Columns);

                float2 scale = float2(1.0/_Columns, 1.0/_Rows);
                o.uv = v.uv * scale + float2(col*scale.x, 1.0-scale.y-row*scale.y);

                return o;
            }

            float4 fragShadow(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);
                return 0;
            }
            ENDCG
        }
    }
}
