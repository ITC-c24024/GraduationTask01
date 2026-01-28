Shader "Custom/LitTransparent2D"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 200

        // 半透明対応
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;               // 光を受ける部分
            o.Metallic = _Metallic;         // 光沢や反射をほぼオフ
            o.Smoothness = _Glossiness;     // 滑らかさを調整
            o.Alpha = c.a;                  // アルファ反映
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}