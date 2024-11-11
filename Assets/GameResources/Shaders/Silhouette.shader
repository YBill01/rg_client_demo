// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Silhuette"
{
	Properties
	{
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_EffectAmount("Effect Amount", Range(0, 1)) = 1.0

		_StencilComp("Stencil Comparison", Float) = 8
		 _Stencil("Stencil ID", Float) = 0
		 _StencilOp("Stencil Operation", Float) = 0
		 _StencilWriteMask("Stencil Write Mask", Float) = 255
		 _StencilReadMask("Stencil Read Mask", Float) = 255
		 _ColorMask("Color Mask", Float) = 15
	}

		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite On
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

	Stencil
	 {
		 Ref[_Stencil]
		 Comp[_StencilComp]
		 Pass[_StencilOp]
		 ReadMask[_StencilReadMask]
		 WriteMask[_StencilWriteMask]
	 }
	  ColorMask[_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			uniform float _EffectAmount;
			uniform float _Opacity;

			fixed4 frag(v2f IN) : COLOR
			{
				half4 texcol = tex2D(_MainTex, IN.texcoord);				
				texcol.rgb = lerp(texcol.rgb, _Color, _EffectAmount);
				if (texcol.a > 0.1f) {
					texcol.a = _Opacity;
				}
				return texcol;
			}
		ENDCG
		}
	}
		Fallback "Sprites/Default"
}