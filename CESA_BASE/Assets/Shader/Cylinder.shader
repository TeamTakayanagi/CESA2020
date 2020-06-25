Shader "Custom/Cylinder"
{
    Properties
    {
        _Normal("_Normal"  , 2D) = "bump" {}
        _Occlusion("_Occlusion"  , 2D) = "white" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ClearTex ("_ClearTex (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _mono("_mono", Range(0, 1)) = 0.0
        _texType("_texType", Range(0, 1)) = 0.0
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
        sampler2D _ClearTex;
        sampler2D _Normal;
        sampler2D _Occlusion;
        half _Glossiness;
        half _mono;
        half _Bump;
        half _texType;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 colorTex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 occlusion = tex2D(_Occlusion, IN.uv_MainTex);
            fixed4 colorClear = tex2D(_ClearTex, IN.uv_MainTex);
            fixed4 normalTex = tex2D(_Normal, IN.uv_MainTex);

            fixed4 tex = colorTex * (1 - _texType) + colorClear * _texType;
            half monoColor = colorTex.r * 0.298912f + colorTex.g * 0.586611f + colorTex.b * 0.114478f;


            int uvY = floor(IN.uv_MainTex.y * 10) % 10;
            int time = floor(sin(_Time.w) * 10) % 10;
            float uvRation = step(IN.uv_MainTex.y, 0.6f) * step(0.15f, IN.uv_MainTex.y) * step(0.4f, IN.uv_MainTex.x);
            float value = abs(0.5f - frac(IN.uv_MainTex.y * 10) % 10);

            o.Albedo = monoColor * (1 - _mono) * _texType + tex * _mono;
            o.Emission = o.Albedo + (fixed4(1, 1, 1, 1) - sqrt(value)) * step(uvY, time) * step(time, uvY) * uvRation * _mono * step(sin(_Time.w), sin(_Time.w + 0.1f));
            o.Smoothness = _Glossiness;
            o.Normal = UnpackScaleNormal(normalTex, _Bump);
            o.Occlusion = occlusion;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
