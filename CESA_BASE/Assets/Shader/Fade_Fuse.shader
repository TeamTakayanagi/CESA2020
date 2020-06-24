Shader "Unlit/Fade_Fuse"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Current("_Current", Range(0, 1.0)) = 0.0
        _Direct("_Direct", Range(0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _Current;
            half _Direct;

            float2 RandomOut2(float2 vec)
            {
                vec = float2(dot(vec, float2(127.1, 311.7)), dot(vec, float2(269.5, 183.3)));
                return frac(sin(vec) * 43758.5453) * 2 - 1;
            }
            // パーリンノイズ
            float PerlinNoise(float2 uv)
            {
                // 前提としtr升目でっ区切られていること
                float2 i_uv = floor(uv);
                float2 f_uv = frac(uv);

                // 最終的には1次元の値で補間して
                // ノイズに出力する
                // 四隅の２Dベクトルをそのまま補間するので2次元の値のままなので、
                // 各ピクセルから四隅に向かうベクトルと内積結果を
                // 1次元の値として利用する
                float2 p1 = float2(0, 0);
                float2 p2 = float2(1, 0);
                float2 p3 = float2(0, 1);
                float2 p4 = float2(1, 1);

                // 四隅のベクトル
                float2 v1 = RandomOut2(i_uv + p1);	// 左上
                float2 v2 = RandomOut2(i_uv + p2);	// 右上
                float2 v3 = RandomOut2(i_uv + p3);	// 左下
                float2 v4 = RandomOut2(i_uv + p4);	// 右下
                // 四隅のベクトルに向かうベクトル
                float2 t1 = p1 - f_uv;	// 左上
                float2 t2 = p2 - f_uv;	// 右上
                float2 t3 = p3 - f_uv;	// 左下
                float2 t4 = p4 - f_uv;	// 右下
                // 四隅の内積
                float2 a = dot(v1, t1);	// 左上
                float2 b = dot(v2, t2);	// 右上
                float2 c = dot(v3, t3);	// 左下
                float2 d = dot(v4, t4);	// 右下

                // 計算結果を得られる式
                // 発表時の論文そのまま仕様
                f_uv = f_uv * f_uv * (3 - 2 * f_uv);

                // 四隅を補間して、現在のUV地点のノイズ値を計算
                float2 u1 = lerp(a, b, f_uv.x);
                float2 u2 = lerp(c, d, f_uv.x);
                float2 v = lerp(u1, u2, f_uv.y);

                return v * 0.5f + 0.5f;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 uv = i.uv;
                float2 defUV = i.uv;

                // UVを分割
                uv.x *= 50;
                uv.y *= 10;

                float noise = PerlinNoise(uv);
                half stepFront = defUV.x * (1.0f - _Direct) + noise * _Direct;
                half stepBack = defUV.x * _Direct + noise * (1.0f - _Direct);
                float alphaN = step(stepFront, stepBack + _Current * 2 - 1);
                col.a = alphaN;

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
