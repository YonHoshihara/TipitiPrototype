Shader "Custom/CassavaPeelBlend"
{
    Properties
    {
        _BaseTex("Base (Peel On)", 2D) = "white" {}
        _PeeledTex("Peeled (Under Skin)", 2D) = "gray" {}
        _PeelMask("Peel Mask (R)", 2D) = "black" {}
        _PeelEdgeBoost("Edge Boost", Range(0,2)) = 0.4
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True"}
        Cull Off Lighting Off ZWrite Off
        Blend One OneMinusSrcAlpha
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BaseTex;
            sampler2D _PeeledTex;
            sampler2D _PeelMask;
            float4 _BaseTex_ST;
            float4 _PeeledTex_ST;
            float _PeelEdgeBoost;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                float m = tex2D(_PeelMask, uv).r; // 0=skin, 1=peeled
                fixed4 baseCol = tex2D(_BaseTex, TRANSFORM_TEX(uv, _BaseTex));
                fixed4 peelCol = tex2D(_PeeledTex, TRANSFORM_TEX(uv, _PeeledTex));

                // Slight edge contrast to make the peeled boundary pop
                // (fake edge: remap mask for contrast)
                float edge = saturate((m - 0.5) * (1.0 + _PeelEdgeBoost) + 0.5);

                fixed4 col = lerp(baseCol, peelCol, edge);
                return col;
            }
            ENDHLSL
        }
    }
}
