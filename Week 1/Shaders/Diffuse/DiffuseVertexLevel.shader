// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DiffuseVertexLevel"
{  
    Properties
    {
        _Diffuse ("Diffuse",Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            Tags{ "LightMode"="ForwardBase" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"

            fixed4 _Diffuse;

            struct a2v
            {
                float4 vertex:POSITION;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                //为了将顶点着色器中计算得到的光照颜色传递给片元着色器，定义一个
                //color变量，并不一定要使用COLOR语义，还可以使用TEXCOORD0语义
                float3 color: COLOR;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex );
                fixed3  ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,
                worldLight));
                o.color = ambient + diffuse;
                return o;
            }

            fixed4 frag(v2f i):SV_TARGET
            {
                return fixed4(i.color,1.0);
            }
            ENDCG
        }
    }
    Fallback"Diffuse"
}
