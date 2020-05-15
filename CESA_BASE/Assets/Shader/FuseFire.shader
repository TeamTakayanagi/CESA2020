Shader "Unlit/FuseFire"
{
	Properties
	{
		[HideInInspector] _Dsitance("_Distance", float) = 1.0
		[HideInInspector] _Ration("_Ration", float) = 0.0
		[HideInInspector] _Target("_Target", Vector) = (0, 0, 0, 0)
		[HideInInspector] _Center("_Center", Vector) = (0, 0, 0, 0)
		[HideInInspector] _MainTex("Albedo (RGB)", 2D) = "white" {}
		_ScaleFactor("Scale Factor", Range(0, 10)) = 0.0

	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag


			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Ration;
			float _Dsitance;
			float _ScaleFactor;
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
				float4 vertex : POSITION;
				float2 uv: TEXCOORD0;
			};
			struct g2f
			{
				float4 vertex : SV_POSITION;
				float2 uv: TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			appdata vert(appdata v)
			{
				return v;
			}
			// ジオメトリシェーダー
			[maxvertexcount(3)]
			void geom(triangle appdata input[3], inout TriangleStream<g2f> stream)
			{
				// 法線を計算
				float3 vec1 = input[1].vertex - input[0].vertex;
				float3 vec2 = input[2].vertex - input[0].vertex;
				float3 normal = normalize(cross(vec1, vec2));

				[unroll]
				for (int i = 0; i < 3; i++)
				{
					v2f v = input[i];
					g2f o;
					// 法線ベクトルに沿って頂点を移動
					v.vertex.xyz += normal * _ScaleFactor;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.worldPos = mul(unity_ObjectToWorld, o.vertex); // ローカル座標系をワールド座標系に変換
					stream.Append(o);
				}
				stream.RestartStrip();
			}

			fixed4 frag(g2f i) : SV_Target
			{
				// テクスチャから対応する色を取得
				fixed4 colorTex = tex2D(_MainTex, i.uv);

				// カメラとオブジェクトの距離(長さ)を取得
				float3 nearTarget = _Target + (i.worldPos - _Center) * _Ration;
				float minLength = min(length(nearTarget - i.worldPos), length(_Target - i.worldPos));
				float dist = saturate(minLength);

				return fixed4(colorTex.rgb * dist, 1);
			}
			ENDCG
	}
	}
}
