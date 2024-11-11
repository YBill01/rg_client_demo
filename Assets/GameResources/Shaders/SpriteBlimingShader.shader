Shader "Custom/UI/SpriteBliming"
{
    Properties
    {
        [PerRendererData] _MainTex  ("Sprite Texture", 2D) = "white" {}
        _HightTex   ("Sprite ", 2D) = "white" {}
        _HiColorize ("Color", Color) = (1,1,1,1)
        _Progress   ("Progress", Float) = 0
        _GlowAlpha   ("Glow Alpha", Range(0,1)) = 1
        _HighlightScale ("Scale", Range(0, 100)) = 0
        [Space]
        [Space]
        _Color      ("Tint", Color) = (1,1,1,1)
        [Space]
        _Fill       ("TextureFill",         Range(0, 1))= 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        _HighlightSpeed ("HighlightSpeed", Float) = 1
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass  {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma enable_d3d11_debug_symbols

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t {
                float4 vertex   : POSITION;
                fixed2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex           : SV_POSITION;
                fixed4 color            : COLOR;

                float2 worldPosition    : TEXCOORD0;
                fixed2 uv               : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _HightTex;
            half _HighlightSpeed;
            half _Progress;
            half _HighlightScale;
            half _GlowAlpha;
            half4 _HiColorize;

            v2f vert(appdata_t v) {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex          = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition   = v.texcoord * _HighlightScale  + _Progress;
                OUT.uv              = v.texcoord;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 color = tex2D(_MainTex, IN.uv);
                fixed4 hiColor = tex2D(_HightTex, IN.worldPosition);

                hiColor.rgb *= _HiColorize;
                color.rgb += hiColor.rgb * _GlowAlpha;
                return color;
            }
        ENDCG
        }
    }
}
