Shader "Unlit/GoalFire"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_OutTime("_OutTime", float) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag


				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float _Ration;
				float _OutTime;
				float _Dsitance;
				float3 _Target;
				float3 _Center;
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
					// テクスチャから対応する色を取得
					fixed4 colorTex = tex2D(_MainTex, i.uv);
					colorTex.rgb -= step(i.worldPos.y, _OutTime) * 0.8f;

					return fixed4(colorTex.rgb, 1.0f);
				}
				ENDCG
			}
		}
}
