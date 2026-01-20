Shader "Custom/StandardClip"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _CutHeightMax ("Cut Height (World Ymax)", Float) = 1.0
        _CutHeightMin ("Cut Height (World Ymin)", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200

        CGPROGRAM
        // Standard Lighting を使う SurfaceShader
        #pragma surface surf Standard fullforwardshadows addshadow

        #include "UnityCG.cginc"

        sampler2D _MainTex;

        half _Metallic;
        half _Smoothness;
        fixed4 _Color;
        float _CutHeightMax;
        float _CutHeightMin;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 上側カット
            clip(_CutHeightMax - IN.worldPos.y);

            // 下側カット
            clip(IN.worldPos.y - _CutHeightMin);

            // テクスチャ取得
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Standard"
}
