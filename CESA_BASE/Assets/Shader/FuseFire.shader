Shader "Unlit/FuseFire"
{
	Properties
	{
		[HideInInspector] _Dsitance("Distance", float) = 1.0
		[HideInInspector]  _Target("_Target", Vector) = (0, 0, 0, 0)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Dsitance;
			float3 _Target;
			fixed4 _FireOutColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv: TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex); // ローカル座標系をワールド座標系に変換
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 colorTex = tex2D(_MainTex, i.uv);
				// カメラとオブジェクトの距離(長さ)を取得
				float dist = saturate(length(_Target - i.worldPos));
				fixed4 col = fixed4(colorTex * (1.1 - dist) + float3(0, 0, 0) * dist, 1);
				return col;
			}
			ENDCG
		}
	}
}
