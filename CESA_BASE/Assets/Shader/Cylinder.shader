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

            o.Albedo = float3(0.3f, 0.3f, 0.3f);
            o.Emission = monoColor * (1 - _mono) * _texType + tex * _mono;
            o.Smoothness = _Glossiness;
            o.Normal = UnpackScaleNormal(normalTex, _Bump);
            o.Occlusion = occlusion;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
