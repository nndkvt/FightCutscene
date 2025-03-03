
Shader "Custom/URPTerrain" {
	Properties {

		[HideInInspector][MainTexture] _BaseMap("leave blanc", 2D) = "white" {}
		[HideInInspector][MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)
		[Space(20)]
		_fogHi("Fog_Hi", float) = 1.0
		_fogLow("Fog_Low", float) = 0.0
		[Space(20)]
		[Toggle(_ALPHATEST_ON)] _AlphaTestToggle ("Alpha Clipping", Float) = 0
		_Cutoff ("Alpha Cutoff", Float) = 0.5

		[Space(20)]
		[Toggle(_SPECULAR_SETUP)] _MetallicSpecToggle ("Workflow, Specular (if on), Metallic (if off)", Float) = 1
		[Toggle(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)] _SmoothnessSource ("Smoothness Source, Albedo Alpha (if on) vs Metallic (if off)", Float) = 0
		_Metallic("Metallic", Range(0.0, 1.0)) = 0
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0.5)
		[Toggle(_METALLICSPECGLOSSMAP)] _MetallicSpecGlossMapToggle ("Use Metallic/Specular Gloss Map", Float) = 0
//		_MetallicSpecGlossMap("Specular or Metallic Map", 2D) = "black" {}

		
		[Space(20)]
		[Toggle(_NORMALMAP)] _NormalMapToggle ("Use Normal Map", Float) = 0
//		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1
		
		
		[Space(20)]
		[Toggle(_OCCLUSIONMAP)] _OcclusionToggle ("Use Occlusion Map", Float) = 0
//		[NoScaleOffset] _OcclusionMap("Occlusion Map", 2D) = "bump" {}
		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0

		[Space(20)]
		[Toggle(_EMISSION)] _Emission ("Emission", Float) = 0
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
//		[NoScaleOffset]_EmissionMap("Emission Map", 2D) = "black" {}

		[Space(20)]
		[Toggle(_SPECULARHIGHLIGHTS_OFF)] _SpecularHighlights("Turn Specular Highlights Off", Float) = 0
		[Toggle(_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentalReflections("Turn Environmental Reflections Off", Float) = 0

		
		[Space(20)]
		_TilesMult ("TilesMult1", Vector) = (1,1,1,1)
		_TilesMult2 ("TilesMult2", Vector) = (1,1,1,1)
		[NoScaleOffset]_MainTex ("Terrain Texture Array", 2DArray) = "white" {}
		[NoScaleOffset]_AOTex ("AO Texture Array", 2DArray) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Specular("Specular", Color) = (0.2, 0.2, 0.2)
		_BackgroundColor("Background Color", Color) = (0,0,0)
		
		[Toggle(USE_AO_MAP)] _bAO_Texture ("Use AO Map", Float) = 1
		
		[Header(Fog of war)]
		[NoScaleOffset]_FogOfWarTex ("Fog of war noise", 2D) = "black" {}
		
		
	}
	
	SubShader {
		Tags {
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)

		float _fogHi;
		float _fogLow;
		
		float4 _BaseMap_ST;
		float4 _BaseColor;
		float4 _EmissionColor;
		float4 _SpecColor;
		float _Metallic;
		float _Smoothness;
		float _OcclusionStrength;
		float _Cutoff;
		float _BumpScale;
		

		half _Glossiness;
		float3 _Specular;
		float4 _Color;
		half3 _BackgroundColor;

		float4 _TilesMult;
		float4 _TilesMult2;
		
		float4 _HexCellData_TexelSize;
		CBUFFER_END
		ENDHLSL

		
		Pass {
			Name "ForwardLit"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment
			
			#pragma target 3.5

			
			// Texture2DArray
			TEXTURE2D_ARRAY(_MainTex);
			SAMPLER(sampler_MainTex);

			TEXTURE2D_ARRAY(_AOTex);
			SAMPLER(sampler_AOTex);

			TEXTURE2D(_FogOfWarTex);
			SAMPLER(sampler_FogOfWarTex);
			
			TEXTURE2D(_HexCellData);
			SAMPLER(sampler_HexCellData);

			
			// flip UVs horizontally to correct for back side projection
	        #define TRIPLANAR_CORRECT_PROJECTED_U
	        // offset UVs to prevent obvious mirroring
			#define TRIPLANAR_UV_OFFSET
				
			#include "Includes/HexMetrics.cginc"


			// ---------------------------------------------------------------------------
			// Keywords
			// ---------------------------------------------------------------------------

			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local USE_AO_MAP

			
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
			#pragma shader_feature_local_fragment _EMISSION
			#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature_local_fragment _OCCLUSIONMAP


			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma shader_feature_local_fragment _SPECULAR_SETUP
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF

			// URP Keywords
			// #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			// #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			// Note, v11 changes these to :
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION // v10+ only (for SSAO support)
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING // v10+ only, renamed from "_MIXED_LIGHTING_SUBTRACTIVE"
			#pragma multi_compile _ SHADOWS_SHADOWMASK // v10+ only

			// Unity Keywords
			//#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile_fog

			
			// GPU Instancing (not supported)
			//#pragma multi_compile_instancing

			// ---------------------------------------------------------------------------
			// Structs
			// ---------------------------------------------------------------------------

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct Attributes {
				float4 positionOS	: POSITION;
				#ifdef _NORMALMAP
					float4 tangentOS 	: TANGENT;
				#endif
				float4 normalOS		: NORMAL;
				float2 uv		    : TEXCOORD0;
				float2 lightmapUV	: TEXCOORD1;
				float4 texcoord2	: TEXCOORD2;
				float4 color		: COLOR;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS 					: SV_POSITION;
				float2 uv		    				: TEXCOORD0;
				float4 uv2		    				: TEXCOORD2;
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
				
				float3 positionWS					: TEXCOORD3;

				#ifdef _NORMALMAP
					half4 normalWS					: TEXCOORD4;    // xyz: normal, w: viewDir.x
					half4 tangentWS					: TEXCOORD5;    // xyz: tangent, w: viewDir.y
					half4 bitangentWS				: TEXCOORD6;    // xyz: bitangent, w: viewDir.z
				#else
					half3 normalWS					: TEXCOORD8;
				#endif
				
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					half4 fogFactorAndVertexLight	: TEXCOORD6; // x: fogFactor, yzw: vertex light
				#else
				// todo  fix TEXCOORD6 repeating  
					half  fogFactor					: TEXCOORD6;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord 				: TEXCOORD7;
				#endif

				float4 color						: COLOR;
				//UNITY_VERTEX_INPUT_INSTANCE_ID
				//UNITY_VERTEX_OUTPUT_STEREO


				float3 terrain : TEXCOORD9;
				float4 visibility : TEXCOORD10;

				// todo merge pair to release interpolator 
				float2 uvX : TEXCOORD11;
				float2 uvY : TEXCOORD12;
				float2 uvZ : TEXCOORD13;
				float3 triblend : TEXCOORD14;

				float4 TilesMult : TEXCOORD15;

			};

			
			#include "Includes/PBRSurface.hlsl"
			#include "Includes/PBRInput.hlsl"

			
			#include "Includes/HexCellData.hlsl"

			// ---------------------------------------------------------------------------
			// Vertex Shader
			// ---------------------------------------------------------------------------

			Varyings LitPassVertex(Attributes IN) {
				Varyings OUT;

				OUT = (Varyings)0;

				//UNITY_SETUP_INSTANCE_ID(IN);
				//UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				#ifdef _NORMALMAP
					VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS.xyz, IN.tangentOS);
				#else
					VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS.xyz);
				#endif

				OUT.positionCS = positionInputs.positionCS;
				OUT.positionWS = positionInputs.positionWS;

				half3 viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
				half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
				half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
				
				#ifdef _NORMALMAP
					OUT.normalWS = half4(normalInputs.normalWS, viewDirWS.x);
					OUT.tangentWS = half4(normalInputs.tangentWS, viewDirWS.y);
					OUT.bitangentWS = half4(normalInputs.bitangentWS, viewDirWS.z);
				#else
					OUT.normalWS = NormalizeNormalPerVertex(normalInputs.normalWS);
					//OUT.viewDirWS = viewDirWS;
				#endif

				OUTPUT_LIGHTMAP_UV(IN.lightmapUV, unity_LightmapST, OUT.lightmapUV);
				//OUTPUT_SH(OUT.normalWS.xyz, OUT.vertexSH);

				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				#else
					OUT.fogFactor = fogFactor;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					OUT.shadowCoord = GetShadowCoord(positionInputs);
				#endif

				OUT.uv = IN.uv;
				OUT.uv2 = IN.texcoord2;
				OUT.color = IN.color;
					
				float4 cell0 = GetCellData_URP(IN, 0);
				float4 cell1 = GetCellData_URP(IN, 1);
				float4 cell2 = GetCellData_URP(IN, 2);

				OUT.terrain.x = cell0.w;
				OUT.terrain.y = cell1.w;
				OUT.terrain.z = cell2.w;

				OUT.visibility.x = cell0.x;
				OUT.visibility.y = cell1.x;
				OUT.visibility.z = cell2.x;
				
				OUT.visibility.xyz = lerp(0.25f, 1.0f, OUT.visibility.xyz);
				OUT.visibility.w =
					cell0.y * IN.color.x + cell1.y * IN.color.y + cell2.y * IN.color.z;

				
				OUT.TilesMult = float4(1.0f, 1.0f ,1.0f ,1.0f );

				[unroll]
				for (int i=0; i < 8; i++)
				{
					if(((int)cell0.w) == i)
					{
						OUT.TilesMult.x = (i<4) ? _TilesMult[i] : _TilesMult2[i - 4];
					}
				}
				
				[unroll]
				for (int i=0; i < 4; i++)
				{
					if(((int)cell1.w) == i)
					{
						OUT.TilesMult.y = (i<4) ? _TilesMult[i] : _TilesMult2[i - 4];
					}
				}
				
				[unroll]
				for (int i=0; i < 4; i++)
				{
					if(((int)cell2.w) == i)
					{
						OUT.TilesMult.z = (i<4) ? _TilesMult[i] : _TilesMult2[i - 4];
					}
				}

					
					return OUT;
			}


			
		float4 GetTerrainColor(Varyings IN, int index, out float AO, float _tileMult = 1.0f) {

			AO = 1.0f;
			
			float3 uvwX = float3(
				IN.uvX * (2 * TILING_SCALE * _tileMult),
				IN.terrain[index]
				);
			
			float3 uvwY = float3(
				IN.uvY * (2 * TILING_SCALE * _tileMult),
				IN.terrain[index]
				);
			
			float3 uvwZ = float3(
				IN.uvZ * (2 * TILING_SCALE * _tileMult),
				IN.terrain[index]
				);

			#define TRIPLANAR_SIDE_MULT 1.0f

				
			#define UNITY_SAMPLE_TEX2DARRAY(tex,coord) tex.Sample (sampler##tex,coord)
			
			float4 colorTexX = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvwX)* TRIPLANAR_SIDE_MULT;
			float4 colorTexY = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvwY);
			float4 colorTexZ = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvwZ)* TRIPLANAR_SIDE_MULT;

			float AO_TexX = UNITY_SAMPLE_TEX2DARRAY(_AOTex, uvwX).x;
			float AO_TexY = UNITY_SAMPLE_TEX2DARRAY(_AOTex, uvwY).x;
			float AO_TexZ = UNITY_SAMPLE_TEX2DARRAY(_AOTex, uvwZ).x;

			AO = max ((AO_TexX*IN.triblend.x), max ((AO_TexY*IN.triblend.y), ( AO_TexZ*IN.triblend.z)))  *
					(IN.color[index] );

			return (colorTexX * IN.triblend.x +
				colorTexY * IN.triblend.y +
				colorTexZ * IN.triblend.z ) *
					(IN.color[index] );


		}


			
			// ---------------------------------------------------------------------------
			// Fragment Shader
			// ---------------------------------------------------------------------------
			
			half4 LitPassFragment(Varyings IN) : SV_Target {
				//UNITY_SETUP_INSTANCE_ID(IN);
				//UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
				
				// Setup SurfaceData
				SurfaceData surfaceData = (SurfaceData)0;
				//InitializeSurfaceData(IN, surfaceData);

				IN.triblend = saturate(pow(IN.normalWS, 4));
	            IN.triblend /= max(dot(IN.triblend, half3(1,1,1)), 0.0001);
				
				IN.uvX = IN.positionWS.zy;
				IN.uvY = IN.positionWS.xz;
				IN.uvZ = IN.positionWS.xy;

	        #if defined(TRIPLANAR_UV_OFFSET)
	            IN.uvY += 0.33;
	            IN.uvZ += 0.67;
	        #endif

	            // minor optimization of sign(). prevents return value of 0
	            half3 axisSign = IN.normalWS < 0 ? -1 : 1;
	            
	            // flip UVs horizontally to correct for back side projection
	        #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
	            IN.uvX.x *= axisSign.x;
	            IN.uvY.x *= axisSign.y;
	            IN.uvZ.x *= -axisSign.z;
	        #endif

			float AO_0;
			float AO_1;
			float AO_2;
				
			// sum of 3 samplers per fragment
			float4 colorTex =
				GetTerrainColor(IN, 0, AO_0, IN.TilesMult.x) +
				GetTerrainColor(IN, 1, AO_1, IN.TilesMult.y) +
				GetTerrainColor(IN, 2, AO_2, IN.TilesMult.z);

				float AO = saturate(AO_0 + AO_1 + AO_2);

				float vis = saturate(dot(IN.color.xyz, IN.visibility.xyz));

				float explored =  smoothstep(0, .811f, IN.visibility.w) * saturate(smoothstep(0, 1.5f,vis) + .5f);
				// float fogOfWar =
				// 	FogOfWar_URP(IN.positionWS.xz, _FogOfWarTex, sampler_FogOfWarTex) *
				// 	(1-explored);

				
				surfaceData.albedo = colorTex.rgb  *  explored;
				surfaceData.alpha = 1.0f;
				surfaceData.normalTS = float3(0.0f, 0.0f, 1.0f);

				surfaceData.specular = _Specular * explored;

			#if defined(USE_AO_MAP)
				surfaceData.smoothness = _Glossiness * (1.0f);
				surfaceData.occlusion = lerp(explored, AO,  explored);
			#else
				surfaceData.smoothness = _Glossiness;
				surfaceData.occlusion = explored;
			#endif
				
				surfaceData.emission = ((1 - saturate((IN.positionWS.y - _fogLow) /
					(_fogHi + _fogLow))) * (1 - explored) * _BackgroundColor);    
				//_BackgroundColor * (1 - explored) - fogOfWar * 0.2 ;
				
				surfaceData.alpha = 1.0f;
				
				// Setup InputData
				InputData inputData;
				InitializeInputData(IN, surfaceData.normalTS, inputData);

				// Simple Lighting (Lambert & BlinnPhong)
				half4 color = UniversalFragmentPBR(inputData, surfaceData);
				// See Lighting.hlsl to see how this is implemented.
				// https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl

				// Fog
				color.rgb = MixFog(color.rgb, inputData.fogCoord);
				//color.a = OutputAlpha(color.a, _Surface);
				return color;
			}
			ENDHLSL
		}

		// UsePass "Universal Render Pipeline/Lit/ShadowCaster"
		// UsePass "Universal Render Pipeline/Lit/DepthOnly"
		// Would be nice if we could just use the passes from existing shaders,
		// However this breaks SRP Batcher compatibility. Instead, we should define them :

		// ShadowCaster, for casting shadows
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			// GPU Instancing
			#pragma multi_compile_instancing
			//#pragma multi_compile _ DOTS_INSTANCING_ON

			// Universal Pipeline Keywords
			// (v11+) This is used during shadow map generation to differentiate between directional and punctual (point/spot) light shadows, as they use different formulas to apply Normal Bias
			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
			
			ENDHLSL
		}

		// DepthOnly, used for Camera Depth Texture (if cannot copy depth buffer instead, and the DepthNormals below isn't used)
		Pass {
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ColorMask 0
			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			// GPU Instancing
			#pragma multi_compile_instancing
			//#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
			
			
			ENDHLSL
		}

	}

}
