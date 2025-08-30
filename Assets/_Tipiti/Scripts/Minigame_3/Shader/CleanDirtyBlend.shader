Shader "Sprites/CleanDirtyBlend"
{
    Properties
    {
        // Standard Sprite properties
        [PerRendererData]_MainTex ("Clean Sprite (RGBA)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Second sprite to blend with
        _DirtyTex ("Dirty Sprite (RGBA)", 2D) = "white" {}

        // 0 = clean, 1 = dirty
        _BlendMask ("Blend Clean -> Dirty", Range(0,1)) = 0

        // Softness for alpha premul fix (keep as 0)
        _AlphaBoost ("Alpha Boost", Range(0,1)) = 0
    }

    SubShader
    {
        Tags{
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "FORWARD"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;   float4 _MainTex_ST;
            sampler2D _DirtyTex;
            fixed4 _Color;
            float _BlendMask;
            float _AlphaBoost;

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                fixed4 color: COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv    = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color; // sprite renderer tint * material tint
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample both sprites (assumes matching UVs/layout)
                fixed4 clean = tex2D(_MainTex,  i.uv);   // RGBA
                fixed4 dirty = tex2D(_DirtyTex, i.uv);   // RGBA

                // Lerp color and alpha
                fixed4 c = lerp(clean, dirty, saturate(_BlendMask));

                // Multiply by tint (keeps SpriteRenderer tint workflow)
                c.rgb *= i.color.rgb;
                c.a   *= i.color.a;

                // Optional tiny alpha boost (usually 0)
                c.a = saturate(c.a + _AlphaBoost * (1-c.a));

                // Premultiplied look-alike safety not required; using straight alpha blending above
                return c;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
