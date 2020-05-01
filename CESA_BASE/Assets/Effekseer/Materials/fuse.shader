// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// シェーダーの情報
//Shader "Unlit/fuse"
//{
//	// Unity上でやり取りをするプロパティ情報
//	// マテリアルのInspectorウィンドウ上に表示され、スクリプト上からも設定できる
//	Properties
//	{
//		_Color("Main Color", Color) = (1,1,1,1) // Color プロパティー (デフォルトは白)   a____
//	}
//		// サブシェーダー
//		// シェーダーの主な処理はこの中に記述する
//		// サブシェーダーは複数書くことも可能が、基本は一つ
//		SubShader
//	{
//		// パス
//		// 1つのオブジェクトの1度の描画で行う処理をここに書く
//		// これも基本一つだが、複雑な描画をするときは複数書くことも可能
//		Pass
//		{
//			CGPROGRAM   // プログラムを書き始めるという宣言
//
//			// 関数宣言
//			#pragma vertex vert    // "vert" 関数を頂点シェーダー使用する宣言
//			#pragma fragment frag  // "frag" 関数をフラグメントシェーダーと使用する宣言
//
//			// 変数宣言
//			fixed4 _Color; // マテリアルからのカラー   a____
//
//			// 頂点シェーダー
//			float4 vert(float4 vertex : POSITION) : SV_POSITION
//			{
//				return UnityObjectToClipPos(vertex);
//			}
//
//			// フラグメントシェーダー
//			fixed4 frag() : SV_Target
//			{
//				return _Color;
//			}
//
//			ENDCG   // プログラムを書き終わるという宣言
//		}
//	}
//}



//Shader "Unlit/fuse"
//{
//	// プロパティ
//	Properties
//	{
//		_Color("Main Color", Color) = (1,1,1,1)	// 原色
//		_MainTex("Texture", 2D) = "white" {}	// テクスチャ(現状無し)
//		_AlphaTex("Mask Tex", 2D) = "white" {}	// マスク画像
//
//		_Border("Border", Float) = 0.0	// 境界値取得用
//	}
//
//	// 処理１
//	SubShader
//	{
//		// レンダリング情報
//		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
//		LOD 100
//		Blend SrcAlpha OneMinusSrcAlpha
//
//		Pass
//		{
//			// 描画処理開始
//			CGPROGRAM
//			// シェーダ宣言
//			#pragma vertex vert
//			#pragma fragment frag
//			#include "UnityCG.cginc"
//
//			// なんか必要な構造体
//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float2 uv : TEXCOORD0;
//			};
//			// なんか必要な構造体
//			struct v2f
//			{
//				half2 uv : TEXCOORD0;
//				float4 vertex : SV_POSITION;
//			};
//
//			// プロパティ用変数
//			float4		_Color;
//			//sampler2D	_MainTex;
//			sampler2D	_AlphaTex;
//			float4		_MainTex_ST;
//			float		_Border;
//
//			//float offset = 0.5;
//
//			// バーテックスシェーダ
//			v2f vert(appdata v)
//			{
//				v2f o;
//				o.vertex = UnityObjectToClipPos(v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//				return o;
//			}
//
//			// フラグメントシェーダ
//			fixed4 frag(v2f i) : SV_Target
//			{
//				// 画像のサンプリング
//				//fixed4 col = tex2D(_MainTex, i.uv);
//				fixed4 col2 = tex2D(_AlphaTex, i.uv);
//				// 境界の判定
//				if (col2.r > _Border)
//					return fixed4(_Color.r, _Color.g, _Color.b, 1);	// 通常色
//				else
//					return fixed4(0, 0, 0, 1);	// 黒
//			}
//			ENDCG // 描画処理終了
//		}
//	}
//}

Shader "Custom/fuse" 
{
	// プロパティ
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)	// 原色
		_MainTex("Texture", 2D) = "white" {}	// テクスチャ(現状無し)
		_MaskTex("Mask Tex", 2D) = "white" {}	// マスク画像

		_Border("Border", Float) = 0.0	// 境界値取得用
	}

	//処理１
	SubShader
	{
		Tags { "RenderType" = "Opaque" }	// レンダリング情報

		// 描画処理開始
		CGPROGRAM
		#pragma surface surf Lambert	// シェーダ宣言

		struct Input
		{
			float2 MainTex;
			float2 uv_MaskTex;
		};

		// プロパティ用変数
		float4		_Color;
		sampler2D _MainTex;
		sampler2D	_MaskTex;
		float		_Border;

		// 変数
		fixed4	maskSample;	// マスク画像のサンプリング値用

		// サーフェスシェーダ
		void surf(Input IN, inout SurfaceOutput o) 
		{
			// 画像のサンプリング
			fixed4 mainTex = tex2D(_MainTex, IN.MainTex);
			fixed4 maskSample = tex2D(_MaskTex, IN.uv_MaskTex);

			// 境界の判定
			if (maskSample.r > _Border)
				o.Albedo = mainTex;	// 普通色
			else
				o.Albedo = fixed4(0, 0, 0, 1);	// 黒
		}
		ENDCG	// 描画処理終了
		
	}
	FallBack "Diffuse"	// フォールバック処理の指定
}