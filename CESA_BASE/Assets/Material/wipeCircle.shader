Shader "Unlit/wipeCircle"
{
    Properties{
        _Radius("_Radius", Range(0,1)) = 1
        _Alpha("_Alpha", Range(0,1)) = 1
    }
    SubShader{
        Pass {
            Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert_img
            #pragma fragment frag

            float _Radius;
            float _Alpha;
            
            fixed4 frag(v2f_img i) : COLOR {
                i.uv -= fixed2(0.5, 0.5);
                i.uv.x *= 16.0 / 9.0;

                return fixed4(0.0f,0.0f,0.0f,min(step(_Radius, distance(i.uv, fixed2(0, 0))), _Alpha));
            }
            ENDCG
        }
    }
}
