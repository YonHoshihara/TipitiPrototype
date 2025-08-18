Shader "Hidden/PeelBrushStamp"
{
    Properties{
        _MainTex("Mask", 2D) = "black" {}
        _BrushPos("Brush Pos (UV)", Vector) = (0.5,0.5,0,0)
        _BrushRadius("Brush Radius (UV)", Float) = 0.05
        _BrushHardness("Brush Hardness", Range(0,1)) = 0.7
        _BrushStrength("Brush Strength", Range(0,1)) = 1.0
        _Erase("Erase (0 add / 1 erase)", Float) = 0
    }
    SubShader
    {
        Tags{ "RenderType"="Opaque" }
        Pass
        {
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BrushPos;      // xy = uv
            float _BrushRadius;    // in UV units
            float _BrushHardness;  // 0 soft, 1 hard
            float _BrushStrength;  // blending amount
            float _Erase;          // 0: add (peel), 1: erase (revert)

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
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
                float d = distance(uv, _BrushPos.xy);
                float r = _BrushRadius;
                float t = saturate(1.0 - smoothstep(r * _BrushHardness, r, d)); // soft circle
                float current = tex2D(_MainTex, uv).r;

                // Write logic: add (toward 1) or erase (toward 0)
                float target = (_Erase > 0.5) ? 0.0 : 1.0;
                float painted = lerp(current, target, t * _BrushStrength);

                return fixed4(painted, painted, painted, 1);
            }
            ENDHLSL
        }
    }
}
