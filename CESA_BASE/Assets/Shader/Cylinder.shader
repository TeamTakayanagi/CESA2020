Shader "Custom/Cylinder"
{
    Properties
    {
        _Normal("_Normal"  , 2D) = "bump" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _mono("_mono", Range(0, 1)) = 0.0
        _Bump("_Bump", Range(0, 100)) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0


        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        sampler2D _Normal;
        half _Glossiness;
        half _mono;
        half _Bump;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 colorTex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 normalTex = tex2D(_Normal, IN.uv_MainTex);
            half mono = colorTex.r * 0.298912f + colorTex.g * 0.586611f + colorTex.b * 0.114478f;

            o.Albedo = float3(0, 0, 0);
            o.Emission = colorTex * _mono + mono * (1 - _mono);
            o.Smoothness = _Glossiness;
            o.Normal = UnpackScaleNormal(normalTex, _Bump);
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
