Shader "Custom/CameraDistance"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Normal ("_Normal", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Radius ("_Radius", Range(0,50)) = 0.0
    }
    SubShader
    {
      Tags {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
            LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows Lambert alpha
        #pragma vertex vert

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Normal;

        struct appdata
        {
            float4 vertex : POSITION;
        };
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        half _Radius;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex); // ローカル座標系をワールド座標系に変換
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 normal = tex2D (_Normal, IN.uv_MainTex);
            float dist = distance(IN.worldPos, _WorldSpaceCameraPos);


            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Normal = UnpackNormal(normal);
            o.Alpha = saturate(dist / _Radius);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
