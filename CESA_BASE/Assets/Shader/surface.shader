Shader "Custom/surface"
{
	Properties
	{
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader{
		 Tags { "Queue" = "Transparent" } //追加

		 LOD 200

		 CGPROGRAM
		 #pragma surface surf Standard alpha:fade //追加
		 #pragma surface surf Lambert
		 #pragma target 3.0

		 struct Input {
			 float2 uv_MainTex;
			 float3 worldPos;
		 };

		fixed4 _Color;
		void surf(Input IN, inout SurfaceOutputStandard o) {
			 o.Albedo = _Color; // アルベド・・色
			 o.Alpha = _Color.a; // Alpha設定 0-1
			 //clip(IN.uv_MainTex.x);
			 clip(frac(IN.worldPos.y * 25) - 0.5);
		}
		 ENDCG
	}
	FallBack "Diffuse"
}
