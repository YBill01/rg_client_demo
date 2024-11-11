Shader "Custom/LootBoxes"
{
    Properties
    {
        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        _speed("Speed", Float) = 0
        LastFrame("LastFrame", Float) = 30
        [HDR]CubeColor("CubeColor", Color) = (1, 1, 1, 0)
        _atlas_x("atlas_x_size", Float) = 6
        _atlas_y("atlas_y_size", Float) = 5
        _disappear("Disappear", Float) = 0.96
        Vector3_62FDD92("Hue", Vector) = (0, 0, 0, 0)
        [HDR]_AlphaAppear("AlphaAppear", Color) = (1, 1, 1, 0)
        [NoScaleOffset]Alpha("Alpha", 2D) = "white" {}
        HUE_SecondLayer("HUE_SecondLayer", Vector) = (0, 0, 0, 0)
        ColorMixSpeed("ColorMixSpeed", Float) = 2
        MixMaskSize("MixMaskSize", Float) = 200
        MixMaskForm("MixMaskForm", Float) = 48

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
			 "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True"
        }

		Blend SrcAlpha One
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off

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
        
        Pass
        {
            Name "Pass"
            Tags 
            { 
                // LightMode: <None>
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite Off
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #pragma multi_compile_instancing
            #define SHADERPASS_UNLIT
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float _speed;
            float LastFrame;
            float4 CubeColor;
            float _atlas_x;
            float _atlas_y;
            float _disappear;
            float3 Vector3_62FDD92;
            float4 _AlphaAppear;
            float3 HUE_SecondLayer;
            float ColorMixSpeed;
            float MixMaskSize;
            float MixMaskForm;
            CBUFFER_END
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
            TEXTURE2D(Alpha); SAMPLER(samplerAlpha); float4 Alpha_TexelSize;
            float4 _Color;
            SAMPLER(_SampleTexture2D_B150FC9F_Sampler_3_Linear_Repeat);
            SAMPLER(_SampleTexture2D_FEF72D1_Sampler_3_Linear_Repeat);
        
            // Graph Functions
            
            void Unity_Hue_Degrees_float(float3 In, float Offset, out float3 Out)
            {
                // RGB to HSV
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
                float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-4;
                float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            
                float hue = hsv.x + Offset / 360;
                hsv.x = (hue < 0)
                        ? hue + 1
                        : (hue > 1)
                            ? hue - 1
                            : hue;
            
                // HSV to RGB
                float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
                Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_Modulo_float(float A, float B, out float Out)
            {
                Out = fmod(A, B);
            }
            
            void Unity_Floor_float(float In, out float Out)
            {
                Out = floor(In);
            }
            
            void Unity_Flipbook_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
            {
                Tile = fmod(Tile, Width*Height);
                float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
                float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
                float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
                Out = (UV + float2(tileX, tileY)) * tileCount;
            }
            
            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            
            inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
            }
            
            inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
            {
                return (1.0-t)*a + (t*b);
            }
            
            
            inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
            
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                float r3 = Unity_SimpleNoise_RandomValue_float(c3);
            
                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                return t;
            }
            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;
            
                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3-0));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3-1));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3-2));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                Out = t;
            }
            
            void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
            {
                Out = lerp(A, B, T);
            }
        
            // Graph Vertex
            // GraphVertex: <None>
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float3 _Property_6EEF1DF4_Out_0 = Vector3_62FDD92;
                float3 _Hue_693EE311_Out_2;
                Unity_Hue_Degrees_float(_Property_6EEF1DF4_Out_0, 0, _Hue_693EE311_Out_2);
                float4 _Property_1209D724_Out_0 = CubeColor;
                float _Property_60AE3AC_Out_0 = _atlas_x;
                float _Property_73B14213_Out_0 = _atlas_y;
                float _Property_CEBA4588_Out_0 = _speed;
                float _Multiply_5C245D1C_Out_2;
                Unity_Multiply_float(_Property_CEBA4588_Out_0, IN.TimeParameters.x, _Multiply_5C245D1C_Out_2);
                float _Property_F9682191_Out_0 = LastFrame;
                float _Modulo_7A1D9EE4_Out_2;
                Unity_Modulo_float(_Multiply_5C245D1C_Out_2, _Property_F9682191_Out_0, _Modulo_7A1D9EE4_Out_2);
                float _Floor_7B9603A7_Out_1;
                Unity_Floor_float(_Modulo_7A1D9EE4_Out_2, _Floor_7B9603A7_Out_1);
                float2 _Flipbook_4A5979A2_Out_4;
                float2 _Flipbook_4A5979A2_Invert = float2 (0, 1);
                Unity_Flipbook_float(IN.uv0.xy, _Property_60AE3AC_Out_0, _Property_73B14213_Out_0, _Floor_7B9603A7_Out_1, _Flipbook_4A5979A2_Invert, _Flipbook_4A5979A2_Out_4);
                float4 _SampleTexture2D_B150FC9F_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _Flipbook_4A5979A2_Out_4);
                float _SampleTexture2D_B150FC9F_R_4 = _SampleTexture2D_B150FC9F_RGBA_0.r;
                float _SampleTexture2D_B150FC9F_G_5 = _SampleTexture2D_B150FC9F_RGBA_0.g;
                float _SampleTexture2D_B150FC9F_B_6 = _SampleTexture2D_B150FC9F_RGBA_0.b;
                float _SampleTexture2D_B150FC9F_A_7 = _SampleTexture2D_B150FC9F_RGBA_0.a;
                float4 _Multiply_ECB76C2F_Out_2;
                Unity_Multiply_float(_Property_1209D724_Out_0, _SampleTexture2D_B150FC9F_RGBA_0, _Multiply_ECB76C2F_Out_2);
                float3 _Multiply_FCFD2672_Out_2;
                Unity_Multiply_float(_Hue_693EE311_Out_2, (_Multiply_ECB76C2F_Out_2.xyz), _Multiply_FCFD2672_Out_2);
                float3 _Property_B8BA0E24_Out_0 = HUE_SecondLayer;
                float3 _Hue_53D6E5C2_Out_2;
                Unity_Hue_Degrees_float(_Property_B8BA0E24_Out_0, 0, _Hue_53D6E5C2_Out_2);
                float3 _Multiply_38B64A16_Out_2;
                Unity_Multiply_float((_SampleTexture2D_B150FC9F_RGBA_0.xyz), _Hue_53D6E5C2_Out_2, _Multiply_38B64A16_Out_2);
                float _Property_79B0517F_Out_0 = MixMaskForm;
                float _Property_231B3666_Out_0 = ColorMixSpeed;
                float _Multiply_657B8303_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_231B3666_Out_0, _Multiply_657B8303_Out_2);
                float2 _Flipbook_F152B4F0_Out_4;
                float2 _Flipbook_F152B4F0_Invert = float2 (0, 1);
                Unity_Flipbook_float(IN.uv0.xy, 10, _Property_79B0517F_Out_0, _Multiply_657B8303_Out_2, _Flipbook_F152B4F0_Invert, _Flipbook_F152B4F0_Out_4);
                float _Property_13CE8FF3_Out_0 = MixMaskSize;
                float _SimpleNoise_727852FA_Out_2;
                Unity_SimpleNoise_float(_Flipbook_F152B4F0_Out_4, _Property_13CE8FF3_Out_0, _SimpleNoise_727852FA_Out_2);
                float3 _Lerp_4F5FA663_Out_3;
                Unity_Lerp_float3(_Multiply_FCFD2672_Out_2, _Multiply_38B64A16_Out_2, (_SimpleNoise_727852FA_Out_2.xxx), _Lerp_4F5FA663_Out_3);
                float4 _Property_680AEA0D_Out_0 = _AlphaAppear;
                float4 _SampleTexture2D_FEF72D1_RGBA_0 = SAMPLE_TEXTURE2D(Alpha, samplerAlpha, _Flipbook_4A5979A2_Out_4);
                float _SampleTexture2D_FEF72D1_R_4 = _SampleTexture2D_FEF72D1_RGBA_0.r;
                float _SampleTexture2D_FEF72D1_G_5 = _SampleTexture2D_FEF72D1_RGBA_0.g;
                float _SampleTexture2D_FEF72D1_B_6 = _SampleTexture2D_FEF72D1_RGBA_0.b;
                float _SampleTexture2D_FEF72D1_A_7 = _SampleTexture2D_FEF72D1_RGBA_0.a;
                float4 _Multiply_803F44F_Out_2;
                Unity_Multiply_float(_Property_680AEA0D_Out_0, (_SampleTexture2D_FEF72D1_A_7.xxxx), _Multiply_803F44F_Out_2);
                float _Property_40EDF24_Out_0 = _disappear;
                float _SimpleNoise_934CCDC6_Out_2;
                Unity_SimpleNoise_float(IN.uv0.xy, 93.6, _SimpleNoise_934CCDC6_Out_2);
                float _Multiply_8644D170_Out_2;
                Unity_Multiply_float(_Property_40EDF24_Out_0, _SimpleNoise_934CCDC6_Out_2, _Multiply_8644D170_Out_2);
                surface.Color = _Lerp_4F5FA663_Out_3;
                surface.Alpha = (_Multiply_803F44F_Out_2).x;
                surface.AlphaClipThreshold = _Multiply_8644D170_Out_2;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.uv0 =                         input.texCoord0;
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #pragma multi_compile_instancing
            #define SHADERPASS_SHADOWCASTER
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float _speed;
            float LastFrame;
            float4 CubeColor;
            float _atlas_x;
            float _atlas_y;
            float _disappear;
            float3 Vector3_62FDD92;
            float4 _AlphaAppear;
            float3 HUE_SecondLayer;
            float ColorMixSpeed;
            float MixMaskSize;
            float MixMaskForm;
            CBUFFER_END
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
            TEXTURE2D(Alpha); SAMPLER(samplerAlpha); float4 Alpha_TexelSize;
            float4 _Color;
            SAMPLER(_SampleTexture2D_FEF72D1_Sampler_3_Linear_Repeat);
        
            // Graph Functions
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_Modulo_float(float A, float B, out float Out)
            {
                Out = fmod(A, B);
            }
            
            void Unity_Floor_float(float In, out float Out)
            {
                Out = floor(In);
            }
            
            void Unity_Flipbook_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
            {
                Tile = fmod(Tile, Width*Height);
                float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
                float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
                float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
                Out = (UV + float2(tileX, tileY)) * tileCount;
            }
            
            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }
            
            
            inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
            }
            
            inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
            {
                return (1.0-t)*a + (t*b);
            }
            
            
            inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
            
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                float r3 = Unity_SimpleNoise_RandomValue_float(c3);
            
                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                return t;
            }
            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;
            
                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3-0));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3-1));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3-2));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                Out = t;
            }
        
            // Graph Vertex
            // GraphVertex: <None>
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_680AEA0D_Out_0 = _AlphaAppear;
                float _Property_60AE3AC_Out_0 = _atlas_x;
                float _Property_73B14213_Out_0 = _atlas_y;
                float _Property_CEBA4588_Out_0 = _speed;
                float _Multiply_5C245D1C_Out_2;
                Unity_Multiply_float(_Property_CEBA4588_Out_0, IN.TimeParameters.x, _Multiply_5C245D1C_Out_2);
                float _Property_F9682191_Out_0 = LastFrame;
                float _Modulo_7A1D9EE4_Out_2;
                Unity_Modulo_float(_Multiply_5C245D1C_Out_2, _Property_F9682191_Out_0, _Modulo_7A1D9EE4_Out_2);
                float _Floor_7B9603A7_Out_1;
                Unity_Floor_float(_Modulo_7A1D9EE4_Out_2, _Floor_7B9603A7_Out_1);
                float2 _Flipbook_4A5979A2_Out_4;
                float2 _Flipbook_4A5979A2_Invert = float2 (0, 1);
                Unity_Flipbook_float(IN.uv0.xy, _Property_60AE3AC_Out_0, _Property_73B14213_Out_0, _Floor_7B9603A7_Out_1, _Flipbook_4A5979A2_Invert, _Flipbook_4A5979A2_Out_4);
                float4 _SampleTexture2D_FEF72D1_RGBA_0 = SAMPLE_TEXTURE2D(Alpha, samplerAlpha, _Flipbook_4A5979A2_Out_4);
                float _SampleTexture2D_FEF72D1_R_4 = _SampleTexture2D_FEF72D1_RGBA_0.r;
                float _SampleTexture2D_FEF72D1_G_5 = _SampleTexture2D_FEF72D1_RGBA_0.g;
                float _SampleTexture2D_FEF72D1_B_6 = _SampleTexture2D_FEF72D1_RGBA_0.b;
                float _SampleTexture2D_FEF72D1_A_7 = _SampleTexture2D_FEF72D1_RGBA_0.a;
                float4 _Multiply_803F44F_Out_2;
                Unity_Multiply_float(_Property_680AEA0D_Out_0, (_SampleTexture2D_FEF72D1_A_7.xxxx), _Multiply_803F44F_Out_2);
                float _Property_40EDF24_Out_0 = _disappear;
                float _SimpleNoise_934CCDC6_Out_2;
                Unity_SimpleNoise_float(IN.uv0.xy, 93.6, _SimpleNoise_934CCDC6_Out_2);
                float _Multiply_8644D170_Out_2;
                Unity_Multiply_float(_Property_40EDF24_Out_0, _SimpleNoise_934CCDC6_Out_2, _Multiply_8644D170_Out_2);
                surface.Alpha = (_Multiply_803F44F_Out_2).x;
                surface.AlphaClipThreshold = _Multiply_8644D170_Out_2;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.uv0 =                         input.texCoord0;
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags 
            { 
                "LightMode" = "DepthOnly"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _AlphaClip 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #pragma multi_compile_instancing
            #define SHADERPASS_DEPTHONLY
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float _speed;
            float LastFrame;
            float4 CubeColor;
            float _atlas_x;
            float _atlas_y;
            float _disappear;
            float3 Vector3_62FDD92;
            float4 _AlphaAppear;
            float3 HUE_SecondLayer;
            float ColorMixSpeed;
            float MixMaskSize;
            float MixMaskForm;
            CBUFFER_END
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
            TEXTURE2D(Alpha); SAMPLER(samplerAlpha); float4 Alpha_TexelSize;
            float4 _Color;
            SAMPLER(_SampleTexture2D_FEF72D1_Sampler_3_Linear_Repeat);
        
            // Graph Functions
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_Modulo_float(float A, float B, out float Out)
            {
                Out = fmod(A, B);
            }
            
            void Unity_Floor_float(float In, out float Out)
            {
                Out = floor(In);
            }
            
            void Unity_Flipbook_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
            {
                Tile = fmod(Tile, Width*Height);
                float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
                float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
                float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
                Out = (UV + float2(tileX, tileY)) * tileCount;
            }
            
            void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }
            
            
            inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
            }
            
            inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
            {
                return (1.0-t)*a + (t*b);
            }
            
            
            inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
            
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                float r3 = Unity_SimpleNoise_RandomValue_float(c3);
            
                float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                return t;
            }
            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;
            
                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3-0));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3-1));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3-2));
                t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
            
                Out = t;
            }
        
            // Graph Vertex
            // GraphVertex: <None>
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_680AEA0D_Out_0 = _AlphaAppear;
                float _Property_60AE3AC_Out_0 = _atlas_x;
                float _Property_73B14213_Out_0 = _atlas_y;
                float _Property_CEBA4588_Out_0 = _speed;
                float _Multiply_5C245D1C_Out_2;
                Unity_Multiply_float(_Property_CEBA4588_Out_0, IN.TimeParameters.x, _Multiply_5C245D1C_Out_2);
                float _Property_F9682191_Out_0 = LastFrame;
                float _Modulo_7A1D9EE4_Out_2;
                Unity_Modulo_float(_Multiply_5C245D1C_Out_2, _Property_F9682191_Out_0, _Modulo_7A1D9EE4_Out_2);
                float _Floor_7B9603A7_Out_1;
                Unity_Floor_float(_Modulo_7A1D9EE4_Out_2, _Floor_7B9603A7_Out_1);
                float2 _Flipbook_4A5979A2_Out_4;
                float2 _Flipbook_4A5979A2_Invert = float2 (0, 1);
                Unity_Flipbook_float(IN.uv0.xy, _Property_60AE3AC_Out_0, _Property_73B14213_Out_0, _Floor_7B9603A7_Out_1, _Flipbook_4A5979A2_Invert, _Flipbook_4A5979A2_Out_4);
                float4 _SampleTexture2D_FEF72D1_RGBA_0 = SAMPLE_TEXTURE2D(Alpha, samplerAlpha, _Flipbook_4A5979A2_Out_4);
                float _SampleTexture2D_FEF72D1_R_4 = _SampleTexture2D_FEF72D1_RGBA_0.r;
                float _SampleTexture2D_FEF72D1_G_5 = _SampleTexture2D_FEF72D1_RGBA_0.g;
                float _SampleTexture2D_FEF72D1_B_6 = _SampleTexture2D_FEF72D1_RGBA_0.b;
                float _SampleTexture2D_FEF72D1_A_7 = _SampleTexture2D_FEF72D1_RGBA_0.a;
                float4 _Multiply_803F44F_Out_2;
                Unity_Multiply_float(_Property_680AEA0D_Out_0, (_SampleTexture2D_FEF72D1_A_7.xxxx), _Multiply_803F44F_Out_2);
                float _Property_40EDF24_Out_0 = _disappear;
                float _SimpleNoise_934CCDC6_Out_2;
                Unity_SimpleNoise_float(IN.uv0.xy, 93.6, _SimpleNoise_934CCDC6_Out_2);
                float _Multiply_8644D170_Out_2;
                Unity_Multiply_float(_Property_40EDF24_Out_0, _SimpleNoise_934CCDC6_Out_2, _Multiply_8644D170_Out_2);
                surface.Alpha = (_Multiply_803F44F_Out_2).x;
                surface.AlphaClipThreshold = _Multiply_8644D170_Out_2;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float4 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.interp00.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.uv0 =                         input.texCoord0;
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
            ENDHLSL
        }
        
    }
	Fallback "UI/Default"
}
