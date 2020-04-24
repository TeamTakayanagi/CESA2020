Shader "Custom/texture"
{
    Properties
    {
        _texNum("texNum", Range(0, 2)) = 0
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex1 ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo (RGB)", 2D) = "white" {}
        _MainTex3 ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex1;
        sampler2D _MainTex2;
        sampler2D _MainTex3;
        int _texNum;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        int equal(int i, int j)
        {
            return step(i, j) * step(j, i);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c1 = tex2D (_MainTex1, IN.uv_MainTex);
            fixed4 c2 = tex2D (_MainTex2, IN.uv_MainTex);
            fixed4 c3 = tex2D (_MainTex3, IN.uv_MainTex);

            o.Albedo = c1.rgb * equal(0, _texNum) + c2.rgb * equal(1, _texNum) + c3.rgb * equal(2, _texNum);
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
