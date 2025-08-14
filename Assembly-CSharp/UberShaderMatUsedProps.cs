using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000236 RID: 566
public struct UberShaderMatUsedProps
{
	// Token: 0x06000D3D RID: 3389 RVA: 0x000481C8 File Offset: 0x000463C8
	public UberShaderMatUsedProps(Material mat)
	{
		this.material = mat;
		this.kw = new GTUberShader_MaterialKeywordStates(mat);
		this._notAProp = 0;
		this._TransparencyMode = 1;
		this._Cutoff = 0;
		this._ColorSource = 0;
		this._BaseColor = 0;
		this._GChannelColor = 0;
		this._BChannelColor = 0;
		this._AChannelColor = 0;
		this._TexMipBias = 0;
		this._BaseMap = 0;
		this._BaseMap_ST = 0;
		this._BaseMap_WH = 0;
		this._TexelSnapToggle = 0;
		this._TexelSnap_Factor = 0;
		this._UVSource = 0;
		this._AlphaDetailToggle = 0;
		this._AlphaDetail_ST = 0;
		this._AlphaDetail_Opacity = 0;
		this._AlphaDetail_WorldSpace = 0;
		this._MaskMapToggle = 0;
		this._MaskMap = 0;
		this._MaskMap_ST = 0;
		this._MaskMap_WH = 0;
		this._LavaLampToggle = 0;
		this._GradientMapToggle = 0;
		this._GradientMap = 0;
		this._DoTextureRotation = 0;
		this._RotateAngle = 0;
		this._RotateAnim = 0;
		this._UseWaveWarp = 0;
		this._WaveAmplitude = 0;
		this._WaveFrequency = 0;
		this._WaveScale = 0;
		this._WaveTimeScale = 0;
		this._UseWeatherMap = 0;
		this._WeatherMap = 0;
		this._WeatherMapDissolveEdgeSize = 0;
		this._ReflectToggle = 0;
		this._ReflectBoxProjectToggle = 0;
		this._ReflectBoxCubePos = 0;
		this._ReflectBoxSize = 0;
		this._ReflectBoxRotation = 0;
		this._ReflectMatcapToggle = 0;
		this._ReflectMatcapPerspToggle = 0;
		this._ReflectNormalToggle = 0;
		this._ReflectTex = 0;
		this._ReflectNormalTex = 0;
		this._ReflectAlbedoTint = 0;
		this._ReflectTint = 0;
		this._ReflectOpacity = 0;
		this._ReflectExposure = 0;
		this._ReflectOffset = 0;
		this._ReflectScale = 0;
		this._ReflectRotate = 0;
		this._HalfLambertToggle = 0;
		this._ParallaxPlanarToggle = 0;
		this._ParallaxToggle = 0;
		this._ParallaxAAToggle = 0;
		this._ParallaxAABias = 0;
		this._DepthMap = 0;
		this._ParallaxAmplitude = 0;
		this._ParallaxSamplesMinMax = 0;
		this._UvShiftToggle = 0;
		this._UvShiftSteps = 0;
		this._UvShiftRate = 0;
		this._UvShiftOffset = 0;
		this._UseGridEffect = 0;
		this._UseCrystalEffect = 0;
		this._CrystalPower = 0;
		this._CrystalRimColor = 0;
		this._LiquidVolume = 0;
		this._LiquidFill = 0;
		this._LiquidFillNormal = 0;
		this._LiquidSurfaceColor = 0;
		this._LiquidSwayX = 0;
		this._LiquidSwayY = 0;
		this._LiquidContainer = 0;
		this._LiquidPlanePosition = 0;
		this._LiquidPlaneNormal = 0;
		this._VertexFlapToggle = 0;
		this._VertexFlapAxis = 0;
		this._VertexFlapDegreesMinMax = 0;
		this._VertexFlapSpeed = 0;
		this._VertexFlapPhaseOffset = 0;
		this._VertexWaveToggle = 0;
		this._VertexWaveDebug = 0;
		this._VertexWaveEnd = 0;
		this._VertexWaveParams = 0;
		this._VertexWaveFalloff = 0;
		this._VertexWaveSphereMask = 0;
		this._VertexWavePhaseOffset = 0;
		this._VertexWaveAxes = 0;
		this._VertexRotateToggle = 0;
		this._VertexRotateAngles = 0;
		this._VertexRotateAnim = 0;
		this._VertexLightToggle = 0;
		this._InnerGlowOn = 0;
		this._InnerGlowColor = 0;
		this._InnerGlowParams = 0;
		this._InnerGlowTap = 0;
		this._InnerGlowSine = 0;
		this._InnerGlowSinePeriod = 0;
		this._InnerGlowSinePhaseShift = 0;
		this._StealthEffectOn = 0;
		this._UseEyeTracking = 0;
		this._EyeTileOffsetUV = 0;
		this._EyeOverrideUV = 0;
		this._EyeOverrideUVTransform = 0;
		this._UseMouthFlap = 0;
		this._MouthMap = 0;
		this._MouthMap_ST = 0;
		this._UseVertexColor = 0;
		this._WaterEffect = 0;
		this._HeightBasedWaterEffect = 0;
		this._WaterCaustics = 0;
		this._UseDayNightLightmap = 0;
		this._UseSpecular = 0;
		this._UseSpecularAlphaChannel = 0;
		this._Smoothness = 0;
		this._UseSpecHighlight = 0;
		this._SpecularDir = 0;
		this._SpecularPowerIntensity = 0;
		this._SpecularColor = 0;
		this._SpecularUseDiffuseColor = 0;
		this._EmissionToggle = 0;
		this._EmissionColor = 0;
		this._EmissionMap = 0;
		this._EmissionMaskByBaseMapAlpha = 0;
		this._EmissionUVScrollSpeed = 0;
		this._EmissionDissolveProgress = 0;
		this._EmissionDissolveAnimation = 0;
		this._EmissionDissolveEdgeSize = 0;
		this._EmissionIntensityInDynamic = 0;
		this._EmissionUseUVWaveWarp = 0;
		this._GreyZoneException = 0;
		this._Cull = 1;
		this._StencilReference = 1;
		this._StencilComparison = 1;
		this._StencilPassFront = 1;
		this._USE_DEFORM_MAP = 0;
		this._DeformMap = 0;
		this._DeformMapIntensity = 0;
		this._DeformMapMaskByVertColorRAmount = 0;
		this._DeformMapScrollSpeed = 0;
		this._DeformMapUV0Influence = 0;
		this._DeformMapObjectSpaceOffsetsU = 0;
		this._DeformMapObjectSpaceOffsetsV = 0;
		this._DeformMapWorldSpaceOffsetsU = 0;
		this._DeformMapWorldSpaceOffsetsV = 0;
		this._RotateOnYAxisBySinTime = 0;
		this._USE_TEX_ARRAY_ATLAS = 0;
		this._BaseMap_Atlas = 0;
		this._BaseMap_AtlasSlice = 0;
		this._BaseMap_AtlasSliceSource = 0;
		this._EmissionMap_Atlas = 0;
		this._EmissionMap_AtlasSlice = 0;
		this._DeformMap_Atlas = 0;
		this._DeformMap_AtlasSlice = 0;
		this._WeatherMap_Atlas = 0;
		this._WeatherMap_AtlasSlice = 0;
		this._DEBUG_PAWN_DATA = 0;
		this._SrcBlend = 1;
		this._DstBlend = 1;
		this._SrcBlendAlpha = 1;
		this._DstBlendAlpha = 1;
		this._ZWrite = 1;
		this._AlphaToMask = 1;
		this._Color = 0;
		this._Surface = 0;
		this._Metallic = 0;
		this._SpecColor = 0;
		this._DayNightLightmapArray = 0;
		this._DayNightLightmapArray_ST = 0;
		this._DayNightLightmapArray_AtlasSlice = 0;
		if (!this.kw._USE_TEXTURE)
		{
			bool use_TEXTURE__AS_MASK = this.kw.USE_TEXTURE__AS_MASK;
		}
		int num = 1;
		UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._BaseMap_Atlas);
		if (this.kw._MASK_MAP_ON)
		{
			this._MaskMap++;
		}
		if (this.kw._GRADIENT_MAP_ON)
		{
			this._GradientMap++;
		}
		if (this.kw._USE_WEATHER_MAP)
		{
			UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._WeatherMap, ref this._WeatherMap_Atlas);
		}
		if (this.kw._EMISSION || this.kw._CRYSTAL_EFFECT)
		{
			UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._EmissionMap, ref this._EmissionMap_Atlas);
		}
		if (this.kw._USE_DEFORM_MAP)
		{
			UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._DeformMap, ref this._DeformMap_Atlas);
		}
		bool flag = this.kw._ALPHA_DETAIL_MAP && (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK);
		bool flag2 = this.kw._WATER_EFFECT || this.kw._STEALTH_EFFECT || this.kw._ALPHA_BLUE_LIVE_ON;
		bool flag3 = this.kw._LIQUID_VOLUME || this.kw._INNER_GLOW || this.kw._VERTEX_ANIM_WAVE_DEBUG;
		bool flag4 = this.kw._WATER_EFFECT || this.kw._STEALTH_EFFECT;
		if (this.kw._REFLECTIONS)
		{
			this._ReflectTex++;
			if (this.kw._REFLECTIONS_USE_NORMAL_TEX)
			{
				this._ReflectNormalTex++;
			}
		}
		if (this.kw._PARALLAX)
		{
			this._DepthMap++;
		}
		if (this.kw.LIGHTMAP_ON)
		{
			bool use_DAY_NIGHT_LIGHTMAP = this.kw._USE_DAY_NIGHT_LIGHTMAP;
		}
		if (this.kw.LIGHTMAP_ON)
		{
			bool dirlightmap_COMBINED = this.kw.DIRLIGHTMAP_COMBINED;
		}
		bool use_WEATHER_MAP = this.kw._USE_WEATHER_MAP;
		if (this.kw._WATER_EFFECT)
		{
			if (!this.kw._WATER_CAUSTICS)
			{
				bool global_ZONE_LIQUID_TYPE__LAVA = this.kw._GLOBAL_ZONE_LIQUID_TYPE__LAVA;
			}
			if (this.kw._HEIGHT_BASED_WATER_EFFECT)
			{
				bool zone_LIQUID_SHAPE__CYLINDER = this.kw._ZONE_LIQUID_SHAPE__CYLINDER;
			}
		}
		bool eyecomp = this.kw._EYECOMP;
		if (this.kw._MOUTHCOMP)
		{
			this._MouthMap++;
		}
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK || this.kw._USE_WEATHER_MAP || this.kw._EMISSION || this.kw._USE_DEFORM_MAP || this.kw._REFLECTIONS)
		{
			bool gt_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;
		}
		if (!this.kw._USE_VERTEX_COLOR && !this.kw._USE_DEFORM_MAP && !this.kw._VERTEX_ANIM_FLAP)
		{
			bool vertex_ANIM_WAVE = this.kw._VERTEX_ANIM_WAVE;
		}
		bool lightmap_ON = this.kw.LIGHTMAP_ON;
		if (num == 0 && !this.kw._PARALLAX)
		{
			bool parallax_PLANAR = this.kw._PARALLAX_PLANAR;
		}
		bool mouthcomp = this.kw._MOUTHCOMP;
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK || this.kw._EMISSION || this.kw._REFLECTIONS)
		{
			bool gt_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z2 = this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;
		}
		bool inner_GLOW = this.kw._INNER_GLOW;
		if (!this.kw._USE_VERTEX_COLOR && !this.kw._VERTEX_ANIM_FLAP)
		{
			bool vertex_ANIM_WAVE2 = this.kw._VERTEX_ANIM_WAVE;
		}
		bool lightmap_ON2 = this.kw.LIGHTMAP_ON;
		if (num == 0 && !this.kw._PARALLAX)
		{
			bool parallax_PLANAR2 = this.kw._PARALLAX_PLANAR;
		}
		if (!this.kw._PARALLAX)
		{
			bool parallax_PLANAR3 = this.kw._PARALLAX_PLANAR;
		}
		bool water_EFFECT = this.kw._WATER_EFFECT;
		if (!this.kw._EMISSION)
		{
			bool crystal_EFFECT = this.kw._CRYSTAL_EFFECT;
		}
		bool liquid_VOLUME = this.kw._LIQUID_VOLUME;
		if (this.kw._REFLECTIONS)
		{
			bool reflections_MATCAP = this.kw._REFLECTIONS_MATCAP;
		}
		bool mouthcomp2 = this.kw._MOUTHCOMP;
		bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		if (this.kw._VERTEX_ROTATE)
		{
			this._VertexRotateAngles++;
		}
		if (this.kw._USE_DEFORM_MAP)
		{
			this._DeformMapUV0Influence++;
			this._DeformMapObjectSpaceOffsetsU++;
			this._DeformMapObjectSpaceOffsetsV++;
			this._DeformMapScrollSpeed++;
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D_LOD(this.kw, ref this._DeformMap, ref this._DeformMap_Atlas);
			this._DeformMapIntensity++;
			this._DeformMapMaskByVertColorRAmount++;
			this._RotateOnYAxisBySinTime++;
		}
		if (this.kw._VERTEX_ANIM_FLAP)
		{
			this._VertexFlapSpeed++;
			this._VertexFlapPhaseOffset++;
			this._VertexFlapDegreesMinMax++;
			this._VertexFlapAxis++;
		}
		if (this.kw._VERTEX_ANIM_WAVE)
		{
			this._VertexWavePhaseOffset++;
			this._VertexWaveParams++;
			this._VertexWaveParams++;
			this._VertexWaveParams++;
			this._VertexWaveParams++;
			this._VertexWaveEnd += 2;
			this._VertexWaveFalloff += 2;
			this._VertexWaveSphereMask++;
			this._VertexWaveAxes++;
			this._VertexWaveAxes++;
			this._VertexWaveAxes++;
			this._VertexWaveAxes++;
		}
		if (this.kw._LIQUID_VOLUME)
		{
			this._LiquidFill++;
			this._LiquidFillNormal++;
			this._LiquidSwayX++;
			this._LiquidSwayY++;
			this._LiquidFill++;
		}
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK || this.kw._EMISSION)
		{
			bool uv_SOURCE__WORLD_PLANAR_Y = this.kw._UV_SOURCE__WORLD_PLANAR_Y;
			if (this.kw._MAINTEX_ROTATE)
			{
				this._RotateAngle++;
				this._RotateAnim++;
			}
			if (this.kw._UV_WAVE_WARP)
			{
				this._WaveAmplitude++;
				this._WaveFrequency++;
				this._WaveScale++;
			}
			if (this.kw._UV_SHIFT)
			{
				this._UvShiftRate++;
				this._UvShiftSteps++;
				this._UvShiftOffset++;
			}
			this._BaseMap++;
			bool gt_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z3 = this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;
			if (this.kw._EYECOMP)
			{
				this._BaseMap_ST++;
				this._EyeOverrideUVTransform++;
				this._EyeOverrideUV += 2;
			}
			if (this.kw._EMISSION)
			{
				this._EmissionUVScrollSpeed += 2;
				this._BaseMap_ST += 2;
				if (this.kw._EMISSION_USE_UV_WAVE_WARP)
				{
					this._WaveAmplitude++;
					this._WaveFrequency++;
					this._WaveScale++;
				}
			}
		}
		if (!this.kw._USE_VERTEX_COLOR && !this.kw._VERTEX_ANIM_FLAP)
		{
			bool vertex_ANIM_WAVE3 = this.kw._VERTEX_ANIM_WAVE;
		}
		bool lightmap_ON3 = this.kw.LIGHTMAP_ON;
		if (this.kw._WATER_EFFECT)
		{
			bool water_CAUSTICS = this.kw._WATER_CAUSTICS;
		}
		if (this.kw._REFLECTIONS && this.kw._REFLECTIONS_MATCAP)
		{
			bool reflections_MATCAP_PERSP_AWARE = this.kw._REFLECTIONS_MATCAP_PERSP_AWARE;
		}
		if (this.kw._MOUTHCOMP)
		{
			this._MouthMap++;
		}
		if (!this.kw._PARALLAX)
		{
			bool parallax_PLANAR4 = this.kw._PARALLAX_PLANAR;
		}
		if (this.kw._INNER_GLOW)
		{
			this._InnerGlowParams += 2;
			this._InnerGlowSinePeriod++;
			this._InnerGlowSinePhaseShift++;
			this._InnerGlowSinePeriod++;
			this._InnerGlowTap++;
		}
		if (this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX)
		{
			bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX2 = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		}
		this._BaseColor++;
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK)
		{
			if (this.kw._TEXEL_SNAP_UVS)
			{
				this._BaseMap_WH++;
				this._TexelSnap_Factor++;
				this._TexelSnap_Factor++;
			}
			if (!this.kw._PARALLAX)
			{
				bool parallax_PLANAR5 = this.kw._PARALLAX_PLANAR;
			}
			if (this.kw._PARALLAX)
			{
				this._ParallaxSamplesMinMax += 2;
				this._DepthMap++;
				this._ParallaxAmplitude++;
				if (this.kw._PARALLAX_AA)
				{
					this._BaseMap_WH++;
					this._ParallaxAABias++;
				}
			}
			else if (this.kw._PARALLAX_PLANAR)
			{
				this._ParallaxAmplitude++;
			}
			if (this.kw._USE_TEX_ARRAY_ATLAS && this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z)
			{
				this._BaseMap_AtlasSlice++;
			}
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			if (this.kw.USE_TEXTURE__AS_MASK)
			{
				this._BaseColor++;
				this._GChannelColor++;
				this._BChannelColor++;
				this._AChannelColor++;
			}
			if (this.kw._ALPHA_DETAIL_MAP)
			{
				this._AlphaDetail_ST += 2;
				this._BaseMap_WH++;
				UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
				this._AlphaDetail_Opacity++;
			}
		}
		if (this.kw._USE_WEATHER_MAP)
		{
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._WeatherMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			this._WeatherMapDissolveEdgeSize++;
		}
		if (this.kw._EYECOMP)
		{
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			this._EyeTileOffsetUV++;
			this._EyeTileOffsetUV++;
			this._EyeTileOffsetUV++;
			this._EyeTileOffsetUV++;
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
		}
		if (this.kw._MOUTHCOMP)
		{
			this._MouthMap++;
		}
		bool use_VERTEX_COLOR = this.kw._USE_VERTEX_COLOR;
		if (this.kw.LIGHTMAP_ON && this.kw._USE_DAY_NIGHT_LIGHTMAP && this.kw.DIRLIGHTMAP_COMBINED)
		{
			bool unity_EDIT_MODE = this.kw._UNITY_EDIT_MODE;
		}
		if (this.kw._CRYSTAL_EFFECT)
		{
			this._CrystalPower++;
			this._CrystalRimColor += 2;
		}
		if (this.kw._USE_TEXTURE && this.kw._MASK_MAP_ON && this.kw._FX_LAVA_LAMP && this.kw._GRADIENT_MAP_ON)
		{
			this._MaskMap_ST += 2;
			this._MaskMap++;
			this._GradientMap++;
		}
		if (this.kw._USE_TEXTURE && this.kw._GRID_EFFECT)
		{
			this._BaseColor++;
			this._BaseMap_WH++;
		}
		if (this.kw._REFLECTIONS)
		{
			if (!this.kw._REFLECTIONS_MATCAP)
			{
				if (this.kw._REFLECTIONS_BOX_PROJECT)
				{
					this._ReflectBoxSize++;
					this._ReflectBoxCubePos++;
					this._ReflectBoxCubePos++;
					this._ReflectBoxRotation++;
					this._ReflectBoxCubePos++;
				}
				this._ReflectRotate++;
				this._ReflectOffset++;
				this._ReflectScale++;
			}
			if (this.kw._REFLECTIONS_USE_NORMAL_TEX)
			{
				this._ReflectNormalTex++;
			}
			this._ReflectTex++;
			if (this.kw._REFLECTIONS_ALBEDO_TINT)
			{
				this._ReflectTint++;
			}
			else
			{
				this._ReflectTint++;
			}
			this._ReflectOpacity++;
			this._ReflectExposure++;
		}
		bool half_LAMBERT_TERM = this.kw._HALF_LAMBERT_TERM;
		if (this.kw._GT_RIM_LIGHT)
		{
			this._Smoothness++;
			if (this.kw._USE_TEXTURE)
			{
				bool gt_RIM_LIGHT_USE_ALPHA = this.kw._GT_RIM_LIGHT_USE_ALPHA;
			}
		}
		if (this.kw._SPECULAR_HIGHLIGHT)
		{
			this._SpecularPowerIntensity++;
			this._SpecularPowerIntensity++;
			this._SpecularDir++;
			this._SpecularColor++;
			this._SpecularColor++;
			if (this.kw._USE_TEXTURE)
			{
				this._SpecularUseDiffuseColor++;
				mat.GetInt("_SpecularUseDiffuseColor");
			}
		}
		if (this.kw._EMISSION || this.kw._CRYSTAL_EFFECT)
		{
			this._EmissionColor += 2;
			if (this.kw._ALPHA_DETAIL_MAP)
			{
				this._AlphaDetail_Opacity++;
			}
			if (this.kw._PARALLAX)
			{
				this._DepthMap++;
				this._ParallaxAmplitude++;
			}
			else if (this.kw._PARALLAX_PLANAR)
			{
				this._ParallaxAmplitude++;
			}
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._EmissionMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			this._EmissionDissolveProgress++;
			this._EmissionDissolveEdgeSize++;
			this._EmissionDissolveAnimation += 2;
			this._EmissionMaskByBaseMapAlpha++;
			bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX3 = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		}
		if (this.kw._INNER_GLOW)
		{
			this._InnerGlowColor++;
		}
		if (this.kw._WATER_EFFECT)
		{
			bool global_ZONE_LIQUID_TYPE__LAVA2 = this.kw._GLOBAL_ZONE_LIQUID_TYPE__LAVA;
			bool height_BASED_WATER_EFFECT = this.kw._HEIGHT_BASED_WATER_EFFECT;
			if (this.kw._WATER_CAUSTICS)
			{
				bool global_ZONE_LIQUID_TYPE__LAVA3 = this.kw._GLOBAL_ZONE_LIQUID_TYPE__LAVA;
			}
			bool use_TEXTURE = this.kw._USE_TEXTURE;
			if (this.kw._HEIGHT_BASED_WATER_EFFECT)
			{
				bool zone_LIQUID_SHAPE__CYLINDER2 = this.kw._ZONE_LIQUID_SHAPE__CYLINDER;
			}
		}
		bool flag5 = !this.kw._LIQUID_CONTAINER;
		if (this.kw._LIQUID_VOLUME && flag5)
		{
			this._LiquidSwayX++;
			this._LiquidSwayY++;
			if (this.kw._USE_TEXTURE)
			{
				this._LiquidSurfaceColor++;
			}
			else
			{
				this._LiquidSurfaceColor++;
			}
		}
		if (this.kw._VERTEX_ANIM_WAVE_DEBUG)
		{
			this._VertexWaveEnd += 2;
			this._VertexWaveFalloff += 2;
			this._VertexWaveSphereMask++;
		}
		bool debug_PAWN_DATA = this.kw._DEBUG_PAWN_DATA;
		if (!this.kw._COLOR_GRADE_PROTANOMALY && !this.kw._COLOR_GRADE_PROTANOPIA && !this.kw._COLOR_GRADE_DEUTERANOMALY && !this.kw._COLOR_GRADE_DEUTERANOPIA && !this.kw._COLOR_GRADE_TRITANOMALY && !this.kw._COLOR_GRADE_TRITANOPIA && !this.kw._COLOR_GRADE_ACHROMATOMALY)
		{
			bool color_GRADE_ACHROMATOPSIA = this.kw._COLOR_GRADE_ACHROMATOPSIA;
		}
		if (this.kw._ALPHATEST_ON)
		{
			this._Cutoff++;
		}
		else if (this.kw._ALPHA_BLUE_LIVE_ON)
		{
			this._Cutoff++;
		}
		if (this.kw._LIQUID_CONTAINER)
		{
			this._LiquidPlanePosition++;
			this._LiquidPlaneNormal++;
		}
		else
		{
			bool liquid_VOLUME2 = this.kw._LIQUID_VOLUME;
		}
		if (!this.kw._ALPHATEST_ON && !this.kw._ALPHA_BLUE_LIVE_ON && !this.kw._LIQUID_CONTAINER)
		{
			bool liquid_VOLUME3 = this.kw._LIQUID_VOLUME;
		}
		if (this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX && (this.kw._EMISSION || this.kw._CRYSTAL_EFFECT))
		{
			this._EmissionIntensityInDynamic++;
		}
		bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX4 = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		this.IsValid = true;
		this.fingerprint = default(MaterialFingerprint);
		this.fingerprint = new MaterialFingerprint(this);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x0004990C File Offset: 0x00047B0C
	public override string ToString()
	{
		string[] array = new string[176];
		array[0] = "---- MaterialFingerprint of ";
		int num = 1;
		Material material = this.material;
		array[num] = ((material != null) ? material.name : null);
		array[2] = " ----\n";
		array[3] = ((this._TransparencyMode > 0) ? ("_TransparencyMode = " + this.fingerprint._TransparencyMode.ToString() + "\n") : "");
		array[4] = ((this._Cutoff > 0) ? ("_Cutoff = " + this.fingerprint._Cutoff.ToString() + "\n") : "");
		array[5] = ((this._ColorSource > 0) ? ("_ColorSource = " + this.fingerprint._ColorSource.ToString() + "\n") : "");
		int num2 = 6;
		string text;
		if (this._BaseColor <= 0)
		{
			text = "";
		}
		else
		{
			string str = "_BaseColor = ";
			int4 @int = this.fingerprint._BaseColor;
			text = str + @int.ToString() + "\n";
		}
		array[num2] = text;
		int num3 = 7;
		string text2;
		if (this._GChannelColor <= 0)
		{
			text2 = "";
		}
		else
		{
			string str2 = "_GChannelColor = ";
			int4 @int = this.fingerprint._GChannelColor;
			text2 = str2 + @int.ToString() + "\n";
		}
		array[num3] = text2;
		int num4 = 8;
		string text3;
		if (this._BChannelColor <= 0)
		{
			text3 = "";
		}
		else
		{
			string str3 = "_BChannelColor = ";
			int4 @int = this.fingerprint._BChannelColor;
			text3 = str3 + @int.ToString() + "\n";
		}
		array[num4] = text3;
		int num5 = 9;
		string text4;
		if (this._AChannelColor <= 0)
		{
			text4 = "";
		}
		else
		{
			string str4 = "_AChannelColor = ";
			int4 @int = this.fingerprint._AChannelColor;
			text4 = str4 + @int.ToString() + "\n";
		}
		array[num5] = text4;
		array[10] = ((this._TexMipBias > 0) ? ("_TexMipBias = " + this.fingerprint._TexMipBias.ToString() + "\n") : "");
		array[11] = ((this._BaseMap > 0) ? ("_BaseMap = " + this.fingerprint._BaseMap + "\n") : "");
		int num6 = 12;
		string text5;
		if (this._BaseMap_ST <= 0)
		{
			text5 = "";
		}
		else
		{
			string str5 = "_BaseMap_ST = ";
			int4 @int = this.fingerprint._BaseMap_ST;
			text5 = str5 + @int.ToString() + "\n";
		}
		array[num6] = text5;
		int num7 = 13;
		string text6;
		if (this._BaseMap_WH <= 0)
		{
			text6 = "";
		}
		else
		{
			string str6 = "_BaseMap_WH = ";
			int4 @int = this.fingerprint._BaseMap_WH;
			text6 = str6 + @int.ToString() + "\n";
		}
		array[num7] = text6;
		array[14] = ((this._TexelSnapToggle > 0) ? ("_TexelSnapToggle = " + this.fingerprint._TexelSnapToggle.ToString() + "\n") : "");
		array[15] = ((this._TexelSnap_Factor > 0) ? ("_TexelSnap_Factor = " + this.fingerprint._TexelSnap_Factor.ToString() + "\n") : "");
		array[16] = ((this._UVSource > 0) ? ("_UVSource = " + this.fingerprint._UVSource.ToString() + "\n") : "");
		array[17] = ((this._AlphaDetailToggle > 0) ? ("_AlphaDetailToggle = " + this.fingerprint._AlphaDetailToggle.ToString() + "\n") : "");
		int num8 = 18;
		string text7;
		if (this._AlphaDetail_ST <= 0)
		{
			text7 = "";
		}
		else
		{
			string str7 = "_AlphaDetail_ST = ";
			int4 @int = this.fingerprint._AlphaDetail_ST;
			text7 = str7 + @int.ToString() + "\n";
		}
		array[num8] = text7;
		array[19] = ((this._AlphaDetail_Opacity > 0) ? ("_AlphaDetail_Opacity = " + this.fingerprint._AlphaDetail_Opacity.ToString() + "\n") : "");
		array[20] = ((this._AlphaDetail_WorldSpace > 0) ? ("_AlphaDetail_WorldSpace = " + this.fingerprint._AlphaDetail_WorldSpace.ToString() + "\n") : "");
		array[21] = ((this._MaskMapToggle > 0) ? ("_MaskMapToggle = " + this.fingerprint._MaskMapToggle.ToString() + "\n") : "");
		array[22] = ((this._MaskMap > 0) ? ("_MaskMap = " + this.fingerprint._MaskMap + "\n") : "");
		int num9 = 23;
		string text8;
		if (this._MaskMap_ST <= 0)
		{
			text8 = "";
		}
		else
		{
			string str8 = "_MaskMap_ST = ";
			int4 @int = this.fingerprint._MaskMap_ST;
			text8 = str8 + @int.ToString() + "\n";
		}
		array[num9] = text8;
		int num10 = 24;
		string text9;
		if (this._MaskMap_WH <= 0)
		{
			text9 = "";
		}
		else
		{
			string str9 = "_MaskMap_WH = ";
			int4 @int = this.fingerprint._MaskMap_WH;
			text9 = str9 + @int.ToString() + "\n";
		}
		array[num10] = text9;
		array[25] = ((this._LavaLampToggle > 0) ? ("_LavaLampToggle = " + this.fingerprint._LavaLampToggle.ToString() + "\n") : "");
		array[26] = ((this._GradientMapToggle > 0) ? ("_GradientMapToggle = " + this.fingerprint._GradientMapToggle.ToString() + "\n") : "");
		array[27] = ((this._GradientMap > 0) ? ("_GradientMap = " + this.fingerprint._GradientMap + "\n") : "");
		array[28] = ((this._DoTextureRotation > 0) ? ("_DoTextureRotation = " + this.fingerprint._DoTextureRotation.ToString() + "\n") : "");
		array[29] = ((this._RotateAngle > 0) ? ("_RotateAngle = " + this.fingerprint._RotateAngle.ToString() + "\n") : "");
		array[30] = ((this._RotateAnim > 0) ? ("_RotateAnim = " + this.fingerprint._RotateAnim.ToString() + "\n") : "");
		array[31] = ((this._UseWaveWarp > 0) ? ("_UseWaveWarp = " + this.fingerprint._UseWaveWarp.ToString() + "\n") : "");
		array[32] = ((this._WaveAmplitude > 0) ? ("_WaveAmplitude = " + this.fingerprint._WaveAmplitude.ToString() + "\n") : "");
		array[33] = ((this._WaveFrequency > 0) ? ("_WaveFrequency = " + this.fingerprint._WaveFrequency.ToString() + "\n") : "");
		array[34] = ((this._WaveScale > 0) ? ("_WaveScale = " + this.fingerprint._WaveScale.ToString() + "\n") : "");
		array[35] = ((this._WaveTimeScale > 0) ? ("_WaveTimeScale = " + this.fingerprint._WaveTimeScale.ToString() + "\n") : "");
		array[36] = ((this._UseWeatherMap > 0) ? ("_UseWeatherMap = " + this.fingerprint._UseWeatherMap.ToString() + "\n") : "");
		array[37] = ((this._WeatherMap > 0) ? ("_WeatherMap = " + this.fingerprint._WeatherMap + "\n") : "");
		array[38] = ((this._WeatherMapDissolveEdgeSize > 0) ? ("_WeatherMapDissolveEdgeSize = " + this.fingerprint._WeatherMapDissolveEdgeSize.ToString() + "\n") : "");
		array[39] = ((this._ReflectToggle > 0) ? ("_ReflectToggle = " + this.fingerprint._ReflectToggle.ToString() + "\n") : "");
		array[40] = ((this._ReflectBoxProjectToggle > 0) ? ("_ReflectBoxProjectToggle = " + this.fingerprint._ReflectBoxProjectToggle.ToString() + "\n") : "");
		int num11 = 41;
		string text10;
		if (this._ReflectBoxCubePos <= 0)
		{
			text10 = "";
		}
		else
		{
			string str10 = "_ReflectBoxCubePos = ";
			int4 @int = this.fingerprint._ReflectBoxCubePos;
			text10 = str10 + @int.ToString() + "\n";
		}
		array[num11] = text10;
		int num12 = 42;
		string text11;
		if (this._ReflectBoxSize <= 0)
		{
			text11 = "";
		}
		else
		{
			string str11 = "_ReflectBoxSize = ";
			int4 @int = this.fingerprint._ReflectBoxSize;
			text11 = str11 + @int.ToString() + "\n";
		}
		array[num12] = text11;
		int num13 = 43;
		string text12;
		if (this._ReflectBoxRotation <= 0)
		{
			text12 = "";
		}
		else
		{
			string str12 = "_ReflectBoxRotation = ";
			int4 @int = this.fingerprint._ReflectBoxRotation;
			text12 = str12 + @int.ToString() + "\n";
		}
		array[num13] = text12;
		array[44] = ((this._ReflectMatcapToggle > 0) ? ("_ReflectMatcapToggle = " + this.fingerprint._ReflectMatcapToggle.ToString() + "\n") : "");
		array[45] = ((this._ReflectMatcapPerspToggle > 0) ? ("_ReflectMatcapPerspToggle = " + this.fingerprint._ReflectMatcapPerspToggle.ToString() + "\n") : "");
		array[46] = ((this._ReflectNormalToggle > 0) ? ("_ReflectNormalToggle = " + this.fingerprint._ReflectNormalToggle.ToString() + "\n") : "");
		array[47] = ((this._ReflectTex > 0) ? ("_ReflectTex = " + this.fingerprint._ReflectTex + "\n") : "");
		array[48] = ((this._ReflectNormalTex > 0) ? ("_ReflectNormalTex = " + this.fingerprint._ReflectNormalTex + "\n") : "");
		array[49] = ((this._ReflectAlbedoTint > 0) ? ("_ReflectAlbedoTint = " + this.fingerprint._ReflectAlbedoTint.ToString() + "\n") : "");
		int num14 = 50;
		string text13;
		if (this._ReflectTint <= 0)
		{
			text13 = "";
		}
		else
		{
			string str13 = "_ReflectTint = ";
			int4 @int = this.fingerprint._ReflectTint;
			text13 = str13 + @int.ToString() + "\n";
		}
		array[num14] = text13;
		array[51] = ((this._ReflectOpacity > 0) ? ("_ReflectOpacity = " + this.fingerprint._ReflectOpacity.ToString() + "\n") : "");
		array[52] = ((this._ReflectExposure > 0) ? ("_ReflectExposure = " + this.fingerprint._ReflectExposure.ToString() + "\n") : "");
		int num15 = 53;
		string text14;
		if (this._ReflectOffset <= 0)
		{
			text14 = "";
		}
		else
		{
			string str14 = "_ReflectOffset = ";
			int4 @int = this.fingerprint._ReflectOffset;
			text14 = str14 + @int.ToString() + "\n";
		}
		array[num15] = text14;
		int num16 = 54;
		string text15;
		if (this._ReflectScale <= 0)
		{
			text15 = "";
		}
		else
		{
			string str15 = "_ReflectScale = ";
			int4 @int = this.fingerprint._ReflectScale;
			text15 = str15 + @int.ToString() + "\n";
		}
		array[num16] = text15;
		array[55] = ((this._ReflectRotate > 0) ? ("_ReflectRotate = " + this.fingerprint._ReflectRotate.ToString() + "\n") : "");
		array[56] = ((this._HalfLambertToggle > 0) ? ("_HalfLambertToggle = " + this.fingerprint._HalfLambertToggle.ToString() + "\n") : "");
		array[57] = ((this._ParallaxPlanarToggle > 0) ? ("_ParallaxPlanarToggle = " + this.fingerprint._ParallaxPlanarToggle.ToString() + "\n") : "");
		array[58] = ((this._ParallaxToggle > 0) ? ("_ParallaxToggle = " + this.fingerprint._ParallaxToggle.ToString() + "\n") : "");
		array[59] = ((this._ParallaxAAToggle > 0) ? ("_ParallaxAAToggle = " + this.fingerprint._ParallaxAAToggle.ToString() + "\n") : "");
		array[60] = ((this._ParallaxAABias > 0) ? ("_ParallaxAABias = " + this.fingerprint._ParallaxAABias.ToString() + "\n") : "");
		array[61] = ((this._DepthMap > 0) ? ("_DepthMap = " + this.fingerprint._DepthMap + "\n") : "");
		array[62] = ((this._ParallaxAmplitude > 0) ? ("_ParallaxAmplitude = " + this.fingerprint._ParallaxAmplitude.ToString() + "\n") : "");
		int num17 = 63;
		string text16;
		if (this._ParallaxSamplesMinMax <= 0)
		{
			text16 = "";
		}
		else
		{
			string str16 = "_ParallaxSamplesMinMax = ";
			int4 @int = this.fingerprint._ParallaxSamplesMinMax;
			text16 = str16 + @int.ToString() + "\n";
		}
		array[num17] = text16;
		array[64] = ((this._UvShiftToggle > 0) ? ("_UvShiftToggle = " + this.fingerprint._UvShiftToggle.ToString() + "\n") : "");
		int num18 = 65;
		string text17;
		if (this._UvShiftSteps <= 0)
		{
			text17 = "";
		}
		else
		{
			string str17 = "_UvShiftSteps = ";
			int4 @int = this.fingerprint._UvShiftSteps;
			text17 = str17 + @int.ToString() + "\n";
		}
		array[num18] = text17;
		int num19 = 66;
		string text18;
		if (this._UvShiftRate <= 0)
		{
			text18 = "";
		}
		else
		{
			string str18 = "_UvShiftRate = ";
			int4 @int = this.fingerprint._UvShiftRate;
			text18 = str18 + @int.ToString() + "\n";
		}
		array[num19] = text18;
		int num20 = 67;
		string text19;
		if (this._UvShiftOffset <= 0)
		{
			text19 = "";
		}
		else
		{
			string str19 = "_UvShiftOffset = ";
			int4 @int = this.fingerprint._UvShiftOffset;
			text19 = str19 + @int.ToString() + "\n";
		}
		array[num20] = text19;
		array[68] = ((this._UseGridEffect > 0) ? ("_UseGridEffect = " + this.fingerprint._UseGridEffect.ToString() + "\n") : "");
		array[69] = ((this._UseCrystalEffect > 0) ? ("_UseCrystalEffect = " + this.fingerprint._UseCrystalEffect.ToString() + "\n") : "");
		array[70] = ((this._CrystalPower > 0) ? ("_CrystalPower = " + this.fingerprint._CrystalPower.ToString() + "\n") : "");
		int num21 = 71;
		string text20;
		if (this._CrystalRimColor <= 0)
		{
			text20 = "";
		}
		else
		{
			string str20 = "_CrystalRimColor = ";
			int4 @int = this.fingerprint._CrystalRimColor;
			text20 = str20 + @int.ToString() + "\n";
		}
		array[num21] = text20;
		array[72] = ((this._LiquidVolume > 0) ? ("_LiquidVolume = " + this.fingerprint._LiquidVolume.ToString() + "\n") : "");
		array[73] = ((this._LiquidFill > 0) ? ("_LiquidFill = " + this.fingerprint._LiquidFill.ToString() + "\n") : "");
		int num22 = 74;
		string text21;
		if (this._LiquidFillNormal <= 0)
		{
			text21 = "";
		}
		else
		{
			string str21 = "_LiquidFillNormal = ";
			int4 @int = this.fingerprint._LiquidFillNormal;
			text21 = str21 + @int.ToString() + "\n";
		}
		array[num22] = text21;
		int num23 = 75;
		string text22;
		if (this._LiquidSurfaceColor <= 0)
		{
			text22 = "";
		}
		else
		{
			string str22 = "_LiquidSurfaceColor = ";
			int4 @int = this.fingerprint._LiquidSurfaceColor;
			text22 = str22 + @int.ToString() + "\n";
		}
		array[num23] = text22;
		array[76] = ((this._LiquidSwayX > 0) ? ("_LiquidSwayX = " + this.fingerprint._LiquidSwayX.ToString() + "\n") : "");
		array[77] = ((this._LiquidSwayY > 0) ? ("_LiquidSwayY = " + this.fingerprint._LiquidSwayY.ToString() + "\n") : "");
		array[78] = ((this._LiquidContainer > 0) ? ("_LiquidContainer = " + this.fingerprint._LiquidContainer.ToString() + "\n") : "");
		int num24 = 79;
		string text23;
		if (this._LiquidPlanePosition <= 0)
		{
			text23 = "";
		}
		else
		{
			string str23 = "_LiquidPlanePosition = ";
			int4 @int = this.fingerprint._LiquidPlanePosition;
			text23 = str23 + @int.ToString() + "\n";
		}
		array[num24] = text23;
		int num25 = 80;
		string text24;
		if (this._LiquidPlaneNormal <= 0)
		{
			text24 = "";
		}
		else
		{
			string str24 = "_LiquidPlaneNormal = ";
			int4 @int = this.fingerprint._LiquidPlaneNormal;
			text24 = str24 + @int.ToString() + "\n";
		}
		array[num25] = text24;
		array[81] = ((this._VertexFlapToggle > 0) ? ("_VertexFlapToggle = " + this.fingerprint._VertexFlapToggle.ToString() + "\n") : "");
		int num26 = 82;
		string text25;
		if (this._VertexFlapAxis <= 0)
		{
			text25 = "";
		}
		else
		{
			string str25 = "_VertexFlapAxis = ";
			int4 @int = this.fingerprint._VertexFlapAxis;
			text25 = str25 + @int.ToString() + "\n";
		}
		array[num26] = text25;
		int num27 = 83;
		string text26;
		if (this._VertexFlapDegreesMinMax <= 0)
		{
			text26 = "";
		}
		else
		{
			string str26 = "_VertexFlapDegreesMinMax = ";
			int4 @int = this.fingerprint._VertexFlapDegreesMinMax;
			text26 = str26 + @int.ToString() + "\n";
		}
		array[num27] = text26;
		array[84] = ((this._VertexFlapSpeed > 0) ? ("_VertexFlapSpeed = " + this.fingerprint._VertexFlapSpeed.ToString() + "\n") : "");
		array[85] = ((this._VertexFlapPhaseOffset > 0) ? ("_VertexFlapPhaseOffset = " + this.fingerprint._VertexFlapPhaseOffset.ToString() + "\n") : "");
		array[86] = ((this._VertexWaveToggle > 0) ? ("_VertexWaveToggle = " + this.fingerprint._VertexWaveToggle.ToString() + "\n") : "");
		array[87] = ((this._VertexWaveDebug > 0) ? ("_VertexWaveDebug = " + this.fingerprint._VertexWaveDebug.ToString() + "\n") : "");
		int num28 = 88;
		string text27;
		if (this._VertexWaveEnd <= 0)
		{
			text27 = "";
		}
		else
		{
			string str27 = "_VertexWaveEnd = ";
			int4 @int = this.fingerprint._VertexWaveEnd;
			text27 = str27 + @int.ToString() + "\n";
		}
		array[num28] = text27;
		int num29 = 89;
		string text28;
		if (this._VertexWaveParams <= 0)
		{
			text28 = "";
		}
		else
		{
			string str28 = "_VertexWaveParams = ";
			int4 @int = this.fingerprint._VertexWaveParams;
			text28 = str28 + @int.ToString() + "\n";
		}
		array[num29] = text28;
		int num30 = 90;
		string text29;
		if (this._VertexWaveFalloff <= 0)
		{
			text29 = "";
		}
		else
		{
			string str29 = "_VertexWaveFalloff = ";
			int4 @int = this.fingerprint._VertexWaveFalloff;
			text29 = str29 + @int.ToString() + "\n";
		}
		array[num30] = text29;
		int num31 = 91;
		string text30;
		if (this._VertexWaveSphereMask <= 0)
		{
			text30 = "";
		}
		else
		{
			string str30 = "_VertexWaveSphereMask = ";
			int4 @int = this.fingerprint._VertexWaveSphereMask;
			text30 = str30 + @int.ToString() + "\n";
		}
		array[num31] = text30;
		array[92] = ((this._VertexWavePhaseOffset > 0) ? ("_VertexWavePhaseOffset = " + this.fingerprint._VertexWavePhaseOffset.ToString() + "\n") : "");
		int num32 = 93;
		string text31;
		if (this._VertexWaveAxes <= 0)
		{
			text31 = "";
		}
		else
		{
			string str31 = "_VertexWaveAxes = ";
			int4 @int = this.fingerprint._VertexWaveAxes;
			text31 = str31 + @int.ToString() + "\n";
		}
		array[num32] = text31;
		array[94] = ((this._VertexRotateToggle > 0) ? ("_VertexRotateToggle = " + this.fingerprint._VertexRotateToggle.ToString() + "\n") : "");
		int num33 = 95;
		string text32;
		if (this._VertexRotateAngles <= 0)
		{
			text32 = "";
		}
		else
		{
			string str32 = "_VertexRotateAngles = ";
			int4 @int = this.fingerprint._VertexRotateAngles;
			text32 = str32 + @int.ToString() + "\n";
		}
		array[num33] = text32;
		array[96] = ((this._VertexRotateAnim > 0) ? ("_VertexRotateAnim = " + this.fingerprint._VertexRotateAnim.ToString() + "\n") : "");
		array[97] = ((this._VertexLightToggle > 0) ? ("_VertexLightToggle = " + this.fingerprint._VertexLightToggle.ToString() + "\n") : "");
		array[98] = ((this._InnerGlowOn > 0) ? ("_InnerGlowOn = " + this.fingerprint._InnerGlowOn.ToString() + "\n") : "");
		int num34 = 99;
		string text33;
		if (this._InnerGlowColor <= 0)
		{
			text33 = "";
		}
		else
		{
			string str33 = "_InnerGlowColor = ";
			int4 @int = this.fingerprint._InnerGlowColor;
			text33 = str33 + @int.ToString() + "\n";
		}
		array[num34] = text33;
		int num35 = 100;
		string text34;
		if (this._InnerGlowParams <= 0)
		{
			text34 = "";
		}
		else
		{
			string str34 = "_InnerGlowParams = ";
			int4 @int = this.fingerprint._InnerGlowParams;
			text34 = str34 + @int.ToString() + "\n";
		}
		array[num35] = text34;
		array[101] = ((this._InnerGlowTap > 0) ? ("_InnerGlowTap = " + this.fingerprint._InnerGlowTap.ToString() + "\n") : "");
		array[102] = ((this._InnerGlowSine > 0) ? ("_InnerGlowSine = " + this.fingerprint._InnerGlowSine.ToString() + "\n") : "");
		array[103] = ((this._InnerGlowSinePeriod > 0) ? ("_InnerGlowSinePeriod = " + this.fingerprint._InnerGlowSinePeriod.ToString() + "\n") : "");
		array[104] = ((this._InnerGlowSinePhaseShift > 0) ? ("_InnerGlowSinePhaseShift = " + this.fingerprint._InnerGlowSinePhaseShift.ToString() + "\n") : "");
		array[105] = ((this._StealthEffectOn > 0) ? ("_StealthEffectOn = " + this.fingerprint._StealthEffectOn.ToString() + "\n") : "");
		array[106] = ((this._UseEyeTracking > 0) ? ("_UseEyeTracking = " + this.fingerprint._UseEyeTracking.ToString() + "\n") : "");
		int num36 = 107;
		string text35;
		if (this._EyeTileOffsetUV <= 0)
		{
			text35 = "";
		}
		else
		{
			string str35 = "_EyeTileOffsetUV = ";
			int4 @int = this.fingerprint._EyeTileOffsetUV;
			text35 = str35 + @int.ToString() + "\n";
		}
		array[num36] = text35;
		array[108] = ((this._EyeOverrideUV > 0) ? ("_EyeOverrideUV = " + this.fingerprint._EyeOverrideUV.ToString() + "\n") : "");
		int num37 = 109;
		string text36;
		if (this._EyeOverrideUVTransform <= 0)
		{
			text36 = "";
		}
		else
		{
			string str36 = "_EyeOverrideUVTransform = ";
			int4 @int = this.fingerprint._EyeOverrideUVTransform;
			text36 = str36 + @int.ToString() + "\n";
		}
		array[num37] = text36;
		array[110] = ((this._UseMouthFlap > 0) ? ("_UseMouthFlap = " + this.fingerprint._UseMouthFlap.ToString() + "\n") : "");
		array[111] = ((this._MouthMap > 0) ? ("_MouthMap = " + this.fingerprint._MouthMap + "\n") : "");
		int num38 = 112;
		string text37;
		if (this._MouthMap_ST <= 0)
		{
			text37 = "";
		}
		else
		{
			string str37 = "_MouthMap_ST = ";
			int4 @int = this.fingerprint._MouthMap_ST;
			text37 = str37 + @int.ToString() + "\n";
		}
		array[num38] = text37;
		array[113] = ((this._UseVertexColor > 0) ? ("_UseVertexColor = " + this.fingerprint._UseVertexColor.ToString() + "\n") : "");
		array[114] = ((this._WaterEffect > 0) ? ("_WaterEffect = " + this.fingerprint._WaterEffect.ToString() + "\n") : "");
		array[115] = ((this._HeightBasedWaterEffect > 0) ? ("_HeightBasedWaterEffect = " + this.fingerprint._HeightBasedWaterEffect.ToString() + "\n") : "");
		array[116] = ((this._WaterCaustics > 0) ? ("_WaterCaustics = " + this.fingerprint._WaterCaustics.ToString() + "\n") : "");
		array[117] = ((this._UseDayNightLightmap > 0) ? ("_UseDayNightLightmap = " + this.fingerprint._UseDayNightLightmap.ToString() + "\n") : "");
		array[118] = ((this._UseSpecular > 0) ? ("_UseSpecular = " + this.fingerprint._UseSpecular.ToString() + "\n") : "");
		array[119] = ((this._UseSpecularAlphaChannel > 0) ? ("_UseSpecularAlphaChannel = " + this.fingerprint._UseSpecularAlphaChannel.ToString() + "\n") : "");
		array[120] = ((this._Smoothness > 0) ? ("_Smoothness = " + this.fingerprint._Smoothness.ToString() + "\n") : "");
		array[121] = ((this._UseSpecHighlight > 0) ? ("_UseSpecHighlight = " + this.fingerprint._UseSpecHighlight.ToString() + "\n") : "");
		int num39 = 122;
		string text38;
		if (this._SpecularDir <= 0)
		{
			text38 = "";
		}
		else
		{
			string str38 = "_SpecularDir = ";
			int4 @int = this.fingerprint._SpecularDir;
			text38 = str38 + @int.ToString() + "\n";
		}
		array[num39] = text38;
		int num40 = 123;
		string text39;
		if (this._SpecularPowerIntensity <= 0)
		{
			text39 = "";
		}
		else
		{
			string str39 = "_SpecularPowerIntensity = ";
			int4 @int = this.fingerprint._SpecularPowerIntensity;
			text39 = str39 + @int.ToString() + "\n";
		}
		array[num40] = text39;
		int num41 = 124;
		string text40;
		if (this._SpecularColor <= 0)
		{
			text40 = "";
		}
		else
		{
			string str40 = "_SpecularColor = ";
			int4 @int = this.fingerprint._SpecularColor;
			text40 = str40 + @int.ToString() + "\n";
		}
		array[num41] = text40;
		array[125] = ((this._SpecularUseDiffuseColor > 0) ? ("_SpecularUseDiffuseColor = " + this.fingerprint._SpecularUseDiffuseColor.ToString() + "\n") : "");
		array[126] = ((this._EmissionToggle > 0) ? ("_EmissionToggle = " + this.fingerprint._EmissionToggle.ToString() + "\n") : "");
		int num42 = 127;
		string text41;
		if (this._EmissionColor <= 0)
		{
			text41 = "";
		}
		else
		{
			string str41 = "_EmissionColor = ";
			int4 @int = this.fingerprint._EmissionColor;
			text41 = str41 + @int.ToString() + "\n";
		}
		array[num42] = text41;
		array[128] = ((this._EmissionMap > 0) ? ("_EmissionMap = " + this.fingerprint._EmissionMap + "\n") : "");
		array[129] = ((this._EmissionMaskByBaseMapAlpha > 0) ? ("_EmissionMaskByBaseMapAlpha = " + this.fingerprint._EmissionMaskByBaseMapAlpha.ToString() + "\n") : "");
		int num43 = 130;
		string text42;
		if (this._EmissionUVScrollSpeed <= 0)
		{
			text42 = "";
		}
		else
		{
			string str42 = "_EmissionUVScrollSpeed = ";
			int4 @int = this.fingerprint._EmissionUVScrollSpeed;
			text42 = str42 + @int.ToString() + "\n";
		}
		array[num43] = text42;
		array[131] = ((this._EmissionDissolveProgress > 0) ? ("_EmissionDissolveProgress = " + this.fingerprint._EmissionDissolveProgress.ToString() + "\n") : "");
		int num44 = 132;
		string text43;
		if (this._EmissionDissolveAnimation <= 0)
		{
			text43 = "";
		}
		else
		{
			string str43 = "_EmissionDissolveAnimation = ";
			int4 @int = this.fingerprint._EmissionDissolveAnimation;
			text43 = str43 + @int.ToString() + "\n";
		}
		array[num44] = text43;
		array[133] = ((this._EmissionDissolveEdgeSize > 0) ? ("_EmissionDissolveEdgeSize = " + this.fingerprint._EmissionDissolveEdgeSize.ToString() + "\n") : "");
		array[134] = ((this._EmissionIntensityInDynamic > 0) ? ("_EmissionIntensityInDynamic = " + this.fingerprint._EmissionIntensityInDynamic.ToString() + "\n") : "");
		array[135] = ((this._EmissionUseUVWaveWarp > 0) ? ("_EmissionUseUVWaveWarp = " + this.fingerprint._EmissionUseUVWaveWarp.ToString() + "\n") : "");
		array[136] = ((this._GreyZoneException > 0) ? ("_GreyZoneException = " + this.fingerprint._GreyZoneException.ToString() + "\n") : "");
		array[137] = ((this._Cull > 0) ? ("_Cull = " + this.fingerprint._Cull.ToString() + "\n") : "");
		array[138] = ((this._StencilReference > 0) ? ("_StencilReference = " + this.fingerprint._StencilReference.ToString() + "\n") : "");
		array[139] = ((this._StencilComparison > 0) ? ("_StencilComparison = " + this.fingerprint._StencilComparison.ToString() + "\n") : "");
		array[140] = ((this._StencilPassFront > 0) ? ("_StencilPassFront = " + this.fingerprint._StencilPassFront.ToString() + "\n") : "");
		array[141] = ((this._USE_DEFORM_MAP > 0) ? ("_USE_DEFORM_MAP = " + this.fingerprint._USE_DEFORM_MAP.ToString() + "\n") : "");
		array[142] = ((this._DeformMap > 0) ? ("_DeformMap = " + this.fingerprint._DeformMap + "\n") : "");
		array[143] = ((this._DeformMapIntensity > 0) ? ("_DeformMapIntensity = " + this.fingerprint._DeformMapIntensity.ToString() + "\n") : "");
		array[144] = ((this._DeformMapMaskByVertColorRAmount > 0) ? ("_DeformMapMaskByVertColorRAmount = " + this.fingerprint._DeformMapMaskByVertColorRAmount.ToString() + "\n") : "");
		int num45 = 145;
		string text44;
		if (this._DeformMapScrollSpeed <= 0)
		{
			text44 = "";
		}
		else
		{
			string str44 = "_DeformMapScrollSpeed = ";
			int4 @int = this.fingerprint._DeformMapScrollSpeed;
			text44 = str44 + @int.ToString() + "\n";
		}
		array[num45] = text44;
		int num46 = 146;
		string text45;
		if (this._DeformMapUV0Influence <= 0)
		{
			text45 = "";
		}
		else
		{
			string str45 = "_DeformMapUV0Influence = ";
			int4 @int = this.fingerprint._DeformMapUV0Influence;
			text45 = str45 + @int.ToString() + "\n";
		}
		array[num46] = text45;
		int num47 = 147;
		string text46;
		if (this._DeformMapObjectSpaceOffsetsU <= 0)
		{
			text46 = "";
		}
		else
		{
			string str46 = "_DeformMapObjectSpaceOffsetsU = ";
			int4 @int = this.fingerprint._DeformMapObjectSpaceOffsetsU;
			text46 = str46 + @int.ToString() + "\n";
		}
		array[num47] = text46;
		int num48 = 148;
		string text47;
		if (this._DeformMapObjectSpaceOffsetsV <= 0)
		{
			text47 = "";
		}
		else
		{
			string str47 = "_DeformMapObjectSpaceOffsetsV = ";
			int4 @int = this.fingerprint._DeformMapObjectSpaceOffsetsV;
			text47 = str47 + @int.ToString() + "\n";
		}
		array[num48] = text47;
		int num49 = 149;
		string text48;
		if (this._DeformMapWorldSpaceOffsetsU <= 0)
		{
			text48 = "";
		}
		else
		{
			string str48 = "_DeformMapWorldSpaceOffsetsU = ";
			int4 @int = this.fingerprint._DeformMapWorldSpaceOffsetsU;
			text48 = str48 + @int.ToString() + "\n";
		}
		array[num49] = text48;
		int num50 = 150;
		string text49;
		if (this._DeformMapWorldSpaceOffsetsV <= 0)
		{
			text49 = "";
		}
		else
		{
			string str49 = "_DeformMapWorldSpaceOffsetsV = ";
			int4 @int = this.fingerprint._DeformMapWorldSpaceOffsetsV;
			text49 = str49 + @int.ToString() + "\n";
		}
		array[num50] = text49;
		int num51 = 151;
		string text50;
		if (this._RotateOnYAxisBySinTime <= 0)
		{
			text50 = "";
		}
		else
		{
			string str50 = "_RotateOnYAxisBySinTime = ";
			int4 @int = this.fingerprint._RotateOnYAxisBySinTime;
			text50 = str50 + @int.ToString() + "\n";
		}
		array[num51] = text50;
		array[152] = ((this._USE_TEX_ARRAY_ATLAS > 0) ? ("_USE_TEX_ARRAY_ATLAS = " + this.fingerprint._USE_TEX_ARRAY_ATLAS.ToString() + "\n") : "");
		array[153] = ((this._BaseMap_Atlas > 0) ? ("_BaseMap_Atlas = " + this.fingerprint._BaseMap_Atlas + "\n") : "");
		array[154] = ((this._BaseMap_AtlasSlice > 0) ? ("_BaseMap_AtlasSlice = " + this.fingerprint._BaseMap_AtlasSlice.ToString() + "\n") : "");
		array[155] = ((this._BaseMap_AtlasSliceSource > 0) ? ("_BaseMap_AtlasSliceSource = " + this.fingerprint._BaseMap_AtlasSliceSource.ToString() + "\n") : "");
		array[156] = ((this._EmissionMap_Atlas > 0) ? ("_EmissionMap_Atlas = " + this.fingerprint._EmissionMap_Atlas + "\n") : "");
		array[157] = ((this._EmissionMap_AtlasSlice > 0) ? ("_EmissionMap_AtlasSlice = " + this.fingerprint._EmissionMap_AtlasSlice.ToString() + "\n") : "");
		array[158] = ((this._DeformMap_Atlas > 0) ? ("_DeformMap_Atlas = " + this.fingerprint._DeformMap_Atlas + "\n") : "");
		array[159] = ((this._DeformMap_AtlasSlice > 0) ? ("_DeformMap_AtlasSlice = " + this.fingerprint._DeformMap_AtlasSlice.ToString() + "\n") : "");
		array[160] = ((this._WeatherMap_Atlas > 0) ? ("_WeatherMap_Atlas = " + this.fingerprint._WeatherMap_Atlas + "\n") : "");
		array[161] = ((this._WeatherMap_AtlasSlice > 0) ? ("_WeatherMap_AtlasSlice = " + this.fingerprint._WeatherMap_AtlasSlice.ToString() + "\n") : "");
		array[162] = ((this._DEBUG_PAWN_DATA > 0) ? ("_DEBUG_PAWN_DATA = " + this.fingerprint._DEBUG_PAWN_DATA.ToString() + "\n") : "");
		array[163] = ((this._SrcBlend > 0) ? ("_SrcBlend = " + this.fingerprint._SrcBlend.ToString() + "\n") : "");
		array[164] = ((this._DstBlend > 0) ? ("_DstBlend = " + this.fingerprint._DstBlend.ToString() + "\n") : "");
		array[165] = ((this._SrcBlendAlpha > 0) ? ("_SrcBlendAlpha = " + this.fingerprint._SrcBlendAlpha.ToString() + "\n") : "");
		array[166] = ((this._DstBlendAlpha > 0) ? ("_DstBlendAlpha = " + this.fingerprint._DstBlendAlpha.ToString() + "\n") : "");
		array[167] = ((this._ZWrite > 0) ? ("_ZWrite = " + this.fingerprint._ZWrite.ToString() + "\n") : "");
		array[168] = ((this._AlphaToMask > 0) ? ("_AlphaToMask = " + this.fingerprint._AlphaToMask.ToString() + "\n") : "");
		int num52 = 169;
		string text51;
		if (this._Color <= 0)
		{
			text51 = "";
		}
		else
		{
			string str51 = "_Color = ";
			int4 @int = this.fingerprint._Color;
			text51 = str51 + @int.ToString() + "\n";
		}
		array[num52] = text51;
		array[170] = ((this._Surface > 0) ? ("_Surface = " + this.fingerprint._Surface.ToString() + "\n") : "");
		array[171] = ((this._Metallic > 0) ? ("_Metallic = " + this.fingerprint._Metallic.ToString() + "\n") : "");
		int num53 = 172;
		string text52;
		if (this._SpecColor <= 0)
		{
			text52 = "";
		}
		else
		{
			string str52 = "_SpecColor = ";
			int4 @int = this.fingerprint._SpecColor;
			text52 = str52 + @int.ToString() + "\n";
		}
		array[num53] = text52;
		array[173] = ((this._DayNightLightmapArray > 0) ? ("_DayNightLightmapArray = " + this.fingerprint._DayNightLightmapArray + "\n") : "");
		int num54 = 174;
		string text53;
		if (this._DayNightLightmapArray_ST <= 0)
		{
			text53 = "";
		}
		else
		{
			string str53 = "_DayNightLightmapArray_ST = ";
			int4 @int = this.fingerprint._DayNightLightmapArray_ST;
			text53 = str53 + @int.ToString() + "\n";
		}
		array[num54] = text53;
		array[175] = ((this._DayNightLightmapArray_AtlasSlice > 0) ? ("_DayNightLightmapArray_AtlasSlice = " + this.fingerprint._DayNightLightmapArray_AtlasSlice.ToString() + "\n") : "");
		return string.Concat(array);
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x0004BDE0 File Offset: 0x00049FE0
	public string ToStringTSV()
	{
		string[] array = new string[695];
		array[0] = "---- MaterialFingerprint of ";
		int num = 1;
		Material material = this.material;
		array[num] = ((material != null) ? material.name : null);
		array[2] = " ----\nName,\tUsed?,\tRounded Value_TransparencyMode,\t";
		array[3] = (this._TransparencyMode > 0).ToString();
		array[4] = ",\t";
		array[5] = this.fingerprint._TransparencyMode.ToString();
		array[6] = "\n_Cutoff,\t";
		array[7] = (this._Cutoff > 0).ToString();
		array[8] = ",\t";
		array[9] = this.fingerprint._Cutoff.ToString();
		array[10] = "\n_ColorSource,\t";
		array[11] = (this._ColorSource > 0).ToString();
		array[12] = ",\t";
		array[13] = this.fingerprint._ColorSource.ToString();
		array[14] = "\n_BaseColor,\t";
		array[15] = (this._BaseColor > 0).ToString();
		array[16] = ",\t";
		int num2 = 17;
		int4 @int = this.fingerprint._BaseColor;
		array[num2] = @int.ToString();
		array[18] = "\n_GChannelColor,\t";
		array[19] = (this._GChannelColor > 0).ToString();
		array[20] = ",\t";
		int num3 = 21;
		@int = this.fingerprint._GChannelColor;
		array[num3] = @int.ToString();
		array[22] = "\n_BChannelColor,\t";
		array[23] = (this._BChannelColor > 0).ToString();
		array[24] = ",\t";
		int num4 = 25;
		@int = this.fingerprint._BChannelColor;
		array[num4] = @int.ToString();
		array[26] = "\n_AChannelColor,\t";
		array[27] = (this._AChannelColor > 0).ToString();
		array[28] = ",\t";
		int num5 = 29;
		@int = this.fingerprint._AChannelColor;
		array[num5] = @int.ToString();
		array[30] = "\n_TexMipBias,\t";
		array[31] = (this._TexMipBias > 0).ToString();
		array[32] = ",\t";
		array[33] = this.fingerprint._TexMipBias.ToString();
		array[34] = "\n_BaseMap,\t";
		array[35] = (this._BaseMap > 0).ToString();
		array[36] = ",\t";
		array[37] = this.fingerprint._BaseMap;
		array[38] = "\n_BaseMap_ST,\t";
		array[39] = (this._BaseMap_ST > 0).ToString();
		array[40] = ",\t";
		int num6 = 41;
		@int = this.fingerprint._BaseMap_ST;
		array[num6] = @int.ToString();
		array[42] = "\n_BaseMap_WH,\t";
		array[43] = (this._BaseMap_WH > 0).ToString();
		array[44] = ",\t";
		int num7 = 45;
		@int = this.fingerprint._BaseMap_WH;
		array[num7] = @int.ToString();
		array[46] = "\n_TexelSnapToggle,\t";
		array[47] = (this._TexelSnapToggle > 0).ToString();
		array[48] = ",\t";
		array[49] = this.fingerprint._TexelSnapToggle.ToString();
		array[50] = "\n_TexelSnap_Factor,\t";
		array[51] = (this._TexelSnap_Factor > 0).ToString();
		array[52] = ",\t";
		array[53] = this.fingerprint._TexelSnap_Factor.ToString();
		array[54] = "\n_UVSource,\t";
		array[55] = (this._UVSource > 0).ToString();
		array[56] = ",\t";
		array[57] = this.fingerprint._UVSource.ToString();
		array[58] = "\n_AlphaDetailToggle,\t";
		array[59] = (this._AlphaDetailToggle > 0).ToString();
		array[60] = ",\t";
		array[61] = this.fingerprint._AlphaDetailToggle.ToString();
		array[62] = "\n_AlphaDetail_ST,\t";
		array[63] = (this._AlphaDetail_ST > 0).ToString();
		array[64] = ",\t";
		int num8 = 65;
		@int = this.fingerprint._AlphaDetail_ST;
		array[num8] = @int.ToString();
		array[66] = "\n_AlphaDetail_Opacity,\t";
		array[67] = (this._AlphaDetail_Opacity > 0).ToString();
		array[68] = ",\t";
		array[69] = this.fingerprint._AlphaDetail_Opacity.ToString();
		array[70] = "\n_AlphaDetail_WorldSpace,\t";
		array[71] = (this._AlphaDetail_WorldSpace > 0).ToString();
		array[72] = ",\t";
		array[73] = this.fingerprint._AlphaDetail_WorldSpace.ToString();
		array[74] = "\n_MaskMapToggle,\t";
		array[75] = (this._MaskMapToggle > 0).ToString();
		array[76] = ",\t";
		array[77] = this.fingerprint._MaskMapToggle.ToString();
		array[78] = "\n_MaskMap,\t";
		array[79] = (this._MaskMap > 0).ToString();
		array[80] = ",\t";
		array[81] = this.fingerprint._MaskMap;
		array[82] = "\n_MaskMap_ST,\t";
		array[83] = (this._MaskMap_ST > 0).ToString();
		array[84] = ",\t";
		int num9 = 85;
		@int = this.fingerprint._MaskMap_ST;
		array[num9] = @int.ToString();
		array[86] = "\n_MaskMap_WH,\t";
		array[87] = (this._MaskMap_WH > 0).ToString();
		array[88] = ",\t";
		int num10 = 89;
		@int = this.fingerprint._MaskMap_WH;
		array[num10] = @int.ToString();
		array[90] = "\n_LavaLampToggle,\t";
		array[91] = (this._LavaLampToggle > 0).ToString();
		array[92] = ",\t";
		array[93] = this.fingerprint._LavaLampToggle.ToString();
		array[94] = "\n_GradientMapToggle,\t";
		array[95] = (this._GradientMapToggle > 0).ToString();
		array[96] = ",\t";
		array[97] = this.fingerprint._GradientMapToggle.ToString();
		array[98] = "\n_GradientMap,\t";
		array[99] = (this._GradientMap > 0).ToString();
		array[100] = ",\t";
		array[101] = this.fingerprint._GradientMap;
		array[102] = "\n_DoTextureRotation,\t";
		array[103] = (this._DoTextureRotation > 0).ToString();
		array[104] = ",\t";
		array[105] = this.fingerprint._DoTextureRotation.ToString();
		array[106] = "\n_RotateAngle,\t";
		array[107] = (this._RotateAngle > 0).ToString();
		array[108] = ",\t";
		array[109] = this.fingerprint._RotateAngle.ToString();
		array[110] = "\n_RotateAnim,\t";
		array[111] = (this._RotateAnim > 0).ToString();
		array[112] = ",\t";
		array[113] = this.fingerprint._RotateAnim.ToString();
		array[114] = "\n_UseWaveWarp,\t";
		array[115] = (this._UseWaveWarp > 0).ToString();
		array[116] = ",\t";
		array[117] = this.fingerprint._UseWaveWarp.ToString();
		array[118] = "\n_WaveAmplitude,\t";
		array[119] = (this._WaveAmplitude > 0).ToString();
		array[120] = ",\t";
		array[121] = this.fingerprint._WaveAmplitude.ToString();
		array[122] = "\n_WaveFrequency,\t";
		array[123] = (this._WaveFrequency > 0).ToString();
		array[124] = ",\t";
		array[125] = this.fingerprint._WaveFrequency.ToString();
		array[126] = "\n_WaveScale,\t";
		array[127] = (this._WaveScale > 0).ToString();
		array[128] = ",\t";
		array[129] = this.fingerprint._WaveScale.ToString();
		array[130] = "\n_WaveTimeScale,\t";
		array[131] = (this._WaveTimeScale > 0).ToString();
		array[132] = ",\t";
		array[133] = this.fingerprint._WaveTimeScale.ToString();
		array[134] = "\n_UseWeatherMap,\t";
		array[135] = (this._UseWeatherMap > 0).ToString();
		array[136] = ",\t";
		array[137] = this.fingerprint._UseWeatherMap.ToString();
		array[138] = "\n_WeatherMap,\t";
		array[139] = (this._WeatherMap > 0).ToString();
		array[140] = ",\t";
		array[141] = this.fingerprint._WeatherMap;
		array[142] = "\n_WeatherMapDissolveEdgeSize,\t";
		array[143] = (this._WeatherMapDissolveEdgeSize > 0).ToString();
		array[144] = ",\t";
		array[145] = this.fingerprint._WeatherMapDissolveEdgeSize.ToString();
		array[146] = "\n_ReflectToggle,\t";
		array[147] = (this._ReflectToggle > 0).ToString();
		array[148] = ",\t";
		array[149] = this.fingerprint._ReflectToggle.ToString();
		array[150] = "\n_ReflectBoxProjectToggle,\t";
		array[151] = (this._ReflectBoxProjectToggle > 0).ToString();
		array[152] = ",\t";
		array[153] = this.fingerprint._ReflectBoxProjectToggle.ToString();
		array[154] = "\n_ReflectBoxCubePos,\t";
		array[155] = (this._ReflectBoxCubePos > 0).ToString();
		array[156] = ",\t";
		int num11 = 157;
		@int = this.fingerprint._ReflectBoxCubePos;
		array[num11] = @int.ToString();
		array[158] = "\n_ReflectBoxSize,\t";
		array[159] = (this._ReflectBoxSize > 0).ToString();
		array[160] = ",\t";
		int num12 = 161;
		@int = this.fingerprint._ReflectBoxSize;
		array[num12] = @int.ToString();
		array[162] = "\n_ReflectBoxRotation,\t";
		array[163] = (this._ReflectBoxRotation > 0).ToString();
		array[164] = ",\t";
		int num13 = 165;
		@int = this.fingerprint._ReflectBoxRotation;
		array[num13] = @int.ToString();
		array[166] = "\n_ReflectMatcapToggle,\t";
		array[167] = (this._ReflectMatcapToggle > 0).ToString();
		array[168] = ",\t";
		array[169] = this.fingerprint._ReflectMatcapToggle.ToString();
		array[170] = "\n_ReflectMatcapPerspToggle,\t";
		array[171] = (this._ReflectMatcapPerspToggle > 0).ToString();
		array[172] = ",\t";
		array[173] = this.fingerprint._ReflectMatcapPerspToggle.ToString();
		array[174] = "\n_ReflectNormalToggle,\t";
		array[175] = (this._ReflectNormalToggle > 0).ToString();
		array[176] = ",\t";
		array[177] = this.fingerprint._ReflectNormalToggle.ToString();
		array[178] = "\n_ReflectTex,\t";
		array[179] = (this._ReflectTex > 0).ToString();
		array[180] = ",\t";
		array[181] = this.fingerprint._ReflectTex;
		array[182] = "\n_ReflectNormalTex,\t";
		array[183] = (this._ReflectNormalTex > 0).ToString();
		array[184] = ",\t";
		array[185] = this.fingerprint._ReflectNormalTex;
		array[186] = "\n_ReflectAlbedoTint,\t";
		array[187] = (this._ReflectAlbedoTint > 0).ToString();
		array[188] = ",\t";
		array[189] = this.fingerprint._ReflectAlbedoTint.ToString();
		array[190] = "\n_ReflectTint,\t";
		array[191] = (this._ReflectTint > 0).ToString();
		array[192] = ",\t";
		int num14 = 193;
		@int = this.fingerprint._ReflectTint;
		array[num14] = @int.ToString();
		array[194] = "\n_ReflectOpacity,\t";
		array[195] = (this._ReflectOpacity > 0).ToString();
		array[196] = ",\t";
		array[197] = this.fingerprint._ReflectOpacity.ToString();
		array[198] = "\n_ReflectExposure,\t";
		array[199] = (this._ReflectExposure > 0).ToString();
		array[200] = ",\t";
		array[201] = this.fingerprint._ReflectExposure.ToString();
		array[202] = "\n_ReflectOffset,\t";
		array[203] = (this._ReflectOffset > 0).ToString();
		array[204] = ",\t";
		int num15 = 205;
		@int = this.fingerprint._ReflectOffset;
		array[num15] = @int.ToString();
		array[206] = "\n_ReflectScale,\t";
		array[207] = (this._ReflectScale > 0).ToString();
		array[208] = ",\t";
		int num16 = 209;
		@int = this.fingerprint._ReflectScale;
		array[num16] = @int.ToString();
		array[210] = "\n_ReflectRotate,\t";
		array[211] = (this._ReflectRotate > 0).ToString();
		array[212] = ",\t";
		array[213] = this.fingerprint._ReflectRotate.ToString();
		array[214] = "\n_HalfLambertToggle,\t";
		array[215] = (this._HalfLambertToggle > 0).ToString();
		array[216] = ",\t";
		array[217] = this.fingerprint._HalfLambertToggle.ToString();
		array[218] = "\n_ParallaxPlanarToggle,\t";
		array[219] = (this._ParallaxPlanarToggle > 0).ToString();
		array[220] = ",\t";
		array[221] = this.fingerprint._ParallaxPlanarToggle.ToString();
		array[222] = "\n_ParallaxToggle,\t";
		array[223] = (this._ParallaxToggle > 0).ToString();
		array[224] = ",\t";
		array[225] = this.fingerprint._ParallaxToggle.ToString();
		array[226] = "\n_ParallaxAAToggle,\t";
		array[227] = (this._ParallaxAAToggle > 0).ToString();
		array[228] = ",\t";
		array[229] = this.fingerprint._ParallaxAAToggle.ToString();
		array[230] = "\n_ParallaxAABias,\t";
		array[231] = (this._ParallaxAABias > 0).ToString();
		array[232] = ",\t";
		array[233] = this.fingerprint._ParallaxAABias.ToString();
		array[234] = "\n_DepthMap,\t";
		array[235] = (this._DepthMap > 0).ToString();
		array[236] = ",\t";
		array[237] = this.fingerprint._DepthMap;
		array[238] = "\n_ParallaxAmplitude,\t";
		array[239] = (this._ParallaxAmplitude > 0).ToString();
		array[240] = ",\t";
		array[241] = this.fingerprint._ParallaxAmplitude.ToString();
		array[242] = "\n_ParallaxSamplesMinMax,\t";
		array[243] = (this._ParallaxSamplesMinMax > 0).ToString();
		array[244] = ",\t";
		int num17 = 245;
		@int = this.fingerprint._ParallaxSamplesMinMax;
		array[num17] = @int.ToString();
		array[246] = "\n_UvShiftToggle,\t";
		array[247] = (this._UvShiftToggle > 0).ToString();
		array[248] = ",\t";
		array[249] = this.fingerprint._UvShiftToggle.ToString();
		array[250] = "\n_UvShiftSteps,\t";
		array[251] = (this._UvShiftSteps > 0).ToString();
		array[252] = ",\t";
		int num18 = 253;
		@int = this.fingerprint._UvShiftSteps;
		array[num18] = @int.ToString();
		array[254] = "\n_UvShiftRate,\t";
		array[255] = (this._UvShiftRate > 0).ToString();
		array[256] = ",\t";
		int num19 = 257;
		@int = this.fingerprint._UvShiftRate;
		array[num19] = @int.ToString();
		array[258] = "\n_UvShiftOffset,\t";
		array[259] = (this._UvShiftOffset > 0).ToString();
		array[260] = ",\t";
		int num20 = 261;
		@int = this.fingerprint._UvShiftOffset;
		array[num20] = @int.ToString();
		array[262] = "\n_UseGridEffect,\t";
		array[263] = (this._UseGridEffect > 0).ToString();
		array[264] = ",\t";
		array[265] = this.fingerprint._UseGridEffect.ToString();
		array[266] = "\n_UseCrystalEffect,\t";
		array[267] = (this._UseCrystalEffect > 0).ToString();
		array[268] = ",\t";
		array[269] = this.fingerprint._UseCrystalEffect.ToString();
		array[270] = "\n_CrystalPower,\t";
		array[271] = (this._CrystalPower > 0).ToString();
		array[272] = ",\t";
		array[273] = this.fingerprint._CrystalPower.ToString();
		array[274] = "\n_CrystalRimColor,\t";
		array[275] = (this._CrystalRimColor > 0).ToString();
		array[276] = ",\t";
		int num21 = 277;
		@int = this.fingerprint._CrystalRimColor;
		array[num21] = @int.ToString();
		array[278] = "\n_LiquidVolume,\t";
		array[279] = (this._LiquidVolume > 0).ToString();
		array[280] = ",\t";
		array[281] = this.fingerprint._LiquidVolume.ToString();
		array[282] = "\n_LiquidFill,\t";
		array[283] = (this._LiquidFill > 0).ToString();
		array[284] = ",\t";
		array[285] = this.fingerprint._LiquidFill.ToString();
		array[286] = "\n_LiquidFillNormal,\t";
		array[287] = (this._LiquidFillNormal > 0).ToString();
		array[288] = ",\t";
		int num22 = 289;
		@int = this.fingerprint._LiquidFillNormal;
		array[num22] = @int.ToString();
		array[290] = "\n_LiquidSurfaceColor,\t";
		array[291] = (this._LiquidSurfaceColor > 0).ToString();
		array[292] = ",\t";
		int num23 = 293;
		@int = this.fingerprint._LiquidSurfaceColor;
		array[num23] = @int.ToString();
		array[294] = "\n_LiquidSwayX,\t";
		array[295] = (this._LiquidSwayX > 0).ToString();
		array[296] = ",\t";
		array[297] = this.fingerprint._LiquidSwayX.ToString();
		array[298] = "\n_LiquidSwayY,\t";
		array[299] = (this._LiquidSwayY > 0).ToString();
		array[300] = ",\t";
		array[301] = this.fingerprint._LiquidSwayY.ToString();
		array[302] = "\n_LiquidContainer,\t";
		array[303] = (this._LiquidContainer > 0).ToString();
		array[304] = ",\t";
		array[305] = this.fingerprint._LiquidContainer.ToString();
		array[306] = "\n_LiquidPlanePosition,\t";
		array[307] = (this._LiquidPlanePosition > 0).ToString();
		array[308] = ",\t";
		int num24 = 309;
		@int = this.fingerprint._LiquidPlanePosition;
		array[num24] = @int.ToString();
		array[310] = "\n_LiquidPlaneNormal,\t";
		array[311] = (this._LiquidPlaneNormal > 0).ToString();
		array[312] = ",\t";
		int num25 = 313;
		@int = this.fingerprint._LiquidPlaneNormal;
		array[num25] = @int.ToString();
		array[314] = "\n_VertexFlapToggle,\t";
		array[315] = (this._VertexFlapToggle > 0).ToString();
		array[316] = ",\t";
		array[317] = this.fingerprint._VertexFlapToggle.ToString();
		array[318] = "\n_VertexFlapAxis,\t";
		array[319] = (this._VertexFlapAxis > 0).ToString();
		array[320] = ",\t";
		int num26 = 321;
		@int = this.fingerprint._VertexFlapAxis;
		array[num26] = @int.ToString();
		array[322] = "\n_VertexFlapDegreesMinMax,\t";
		array[323] = (this._VertexFlapDegreesMinMax > 0).ToString();
		array[324] = ",\t";
		int num27 = 325;
		@int = this.fingerprint._VertexFlapDegreesMinMax;
		array[num27] = @int.ToString();
		array[326] = "\n_VertexFlapSpeed,\t";
		array[327] = (this._VertexFlapSpeed > 0).ToString();
		array[328] = ",\t";
		array[329] = this.fingerprint._VertexFlapSpeed.ToString();
		array[330] = "\n_VertexFlapPhaseOffset,\t";
		array[331] = (this._VertexFlapPhaseOffset > 0).ToString();
		array[332] = ",\t";
		array[333] = this.fingerprint._VertexFlapPhaseOffset.ToString();
		array[334] = "\n_VertexWaveToggle,\t";
		array[335] = (this._VertexWaveToggle > 0).ToString();
		array[336] = ",\t";
		array[337] = this.fingerprint._VertexWaveToggle.ToString();
		array[338] = "\n_VertexWaveDebug,\t";
		array[339] = (this._VertexWaveDebug > 0).ToString();
		array[340] = ",\t";
		array[341] = this.fingerprint._VertexWaveDebug.ToString();
		array[342] = "\n_VertexWaveEnd,\t";
		array[343] = (this._VertexWaveEnd > 0).ToString();
		array[344] = ",\t";
		int num28 = 345;
		@int = this.fingerprint._VertexWaveEnd;
		array[num28] = @int.ToString();
		array[346] = "\n_VertexWaveParams,\t";
		array[347] = (this._VertexWaveParams > 0).ToString();
		array[348] = ",\t";
		int num29 = 349;
		@int = this.fingerprint._VertexWaveParams;
		array[num29] = @int.ToString();
		array[350] = "\n_VertexWaveFalloff,\t";
		array[351] = (this._VertexWaveFalloff > 0).ToString();
		array[352] = ",\t";
		int num30 = 353;
		@int = this.fingerprint._VertexWaveFalloff;
		array[num30] = @int.ToString();
		array[354] = "\n_VertexWaveSphereMask,\t";
		array[355] = (this._VertexWaveSphereMask > 0).ToString();
		array[356] = ",\t";
		int num31 = 357;
		@int = this.fingerprint._VertexWaveSphereMask;
		array[num31] = @int.ToString();
		array[358] = "\n_VertexWavePhaseOffset,\t";
		array[359] = (this._VertexWavePhaseOffset > 0).ToString();
		array[360] = ",\t";
		array[361] = this.fingerprint._VertexWavePhaseOffset.ToString();
		array[362] = "\n_VertexWaveAxes,\t";
		array[363] = (this._VertexWaveAxes > 0).ToString();
		array[364] = ",\t";
		int num32 = 365;
		@int = this.fingerprint._VertexWaveAxes;
		array[num32] = @int.ToString();
		array[366] = "\n_VertexRotateToggle,\t";
		array[367] = (this._VertexRotateToggle > 0).ToString();
		array[368] = ",\t";
		array[369] = this.fingerprint._VertexRotateToggle.ToString();
		array[370] = "\n_VertexRotateAngles,\t";
		array[371] = (this._VertexRotateAngles > 0).ToString();
		array[372] = ",\t";
		int num33 = 373;
		@int = this.fingerprint._VertexRotateAngles;
		array[num33] = @int.ToString();
		array[374] = "\n_VertexRotateAnim,\t";
		array[375] = (this._VertexRotateAnim > 0).ToString();
		array[376] = ",\t";
		array[377] = this.fingerprint._VertexRotateAnim.ToString();
		array[378] = "\n_VertexLightToggle,\t";
		array[379] = (this._VertexLightToggle > 0).ToString();
		array[380] = ",\t";
		array[381] = this.fingerprint._VertexLightToggle.ToString();
		array[382] = "\n_InnerGlowOn,\t";
		array[383] = (this._InnerGlowOn > 0).ToString();
		array[384] = ",\t";
		array[385] = this.fingerprint._InnerGlowOn.ToString();
		array[386] = "\n_InnerGlowColor,\t";
		array[387] = (this._InnerGlowColor > 0).ToString();
		array[388] = ",\t";
		int num34 = 389;
		@int = this.fingerprint._InnerGlowColor;
		array[num34] = @int.ToString();
		array[390] = "\n_InnerGlowParams,\t";
		array[391] = (this._InnerGlowParams > 0).ToString();
		array[392] = ",\t";
		int num35 = 393;
		@int = this.fingerprint._InnerGlowParams;
		array[num35] = @int.ToString();
		array[394] = "\n_InnerGlowTap,\t";
		array[395] = (this._InnerGlowTap > 0).ToString();
		array[396] = ",\t";
		array[397] = this.fingerprint._InnerGlowTap.ToString();
		array[398] = "\n_InnerGlowSine,\t";
		array[399] = (this._InnerGlowSine > 0).ToString();
		array[400] = ",\t";
		array[401] = this.fingerprint._InnerGlowSine.ToString();
		array[402] = "\n_InnerGlowSinePeriod,\t";
		array[403] = (this._InnerGlowSinePeriod > 0).ToString();
		array[404] = ",\t";
		array[405] = this.fingerprint._InnerGlowSinePeriod.ToString();
		array[406] = "\n_InnerGlowSinePhaseShift,\t";
		array[407] = (this._InnerGlowSinePhaseShift > 0).ToString();
		array[408] = ",\t";
		array[409] = this.fingerprint._InnerGlowSinePhaseShift.ToString();
		array[410] = "\n_StealthEffectOn,\t";
		array[411] = (this._StealthEffectOn > 0).ToString();
		array[412] = ",\t";
		array[413] = this.fingerprint._StealthEffectOn.ToString();
		array[414] = "\n_UseEyeTracking,\t";
		array[415] = (this._UseEyeTracking > 0).ToString();
		array[416] = ",\t";
		array[417] = this.fingerprint._UseEyeTracking.ToString();
		array[418] = "\n_EyeTileOffsetUV,\t";
		array[419] = (this._EyeTileOffsetUV > 0).ToString();
		array[420] = ",\t";
		int num36 = 421;
		@int = this.fingerprint._EyeTileOffsetUV;
		array[num36] = @int.ToString();
		array[422] = "\n_EyeOverrideUV,\t";
		array[423] = (this._EyeOverrideUV > 0).ToString();
		array[424] = ",\t";
		array[425] = this.fingerprint._EyeOverrideUV.ToString();
		array[426] = "\n_EyeOverrideUVTransform,\t";
		array[427] = (this._EyeOverrideUVTransform > 0).ToString();
		array[428] = ",\t";
		int num37 = 429;
		@int = this.fingerprint._EyeOverrideUVTransform;
		array[num37] = @int.ToString();
		array[430] = "\n_UseMouthFlap,\t";
		array[431] = (this._UseMouthFlap > 0).ToString();
		array[432] = ",\t";
		array[433] = this.fingerprint._UseMouthFlap.ToString();
		array[434] = "\n_MouthMap,\t";
		array[435] = (this._MouthMap > 0).ToString();
		array[436] = ",\t";
		array[437] = this.fingerprint._MouthMap;
		array[438] = "\n_MouthMap_ST,\t";
		array[439] = (this._MouthMap_ST > 0).ToString();
		array[440] = ",\t";
		int num38 = 441;
		@int = this.fingerprint._MouthMap_ST;
		array[num38] = @int.ToString();
		array[442] = "\n_UseVertexColor,\t";
		array[443] = (this._UseVertexColor > 0).ToString();
		array[444] = ",\t";
		array[445] = this.fingerprint._UseVertexColor.ToString();
		array[446] = "\n_WaterEffect,\t";
		array[447] = (this._WaterEffect > 0).ToString();
		array[448] = ",\t";
		array[449] = this.fingerprint._WaterEffect.ToString();
		array[450] = "\n_HeightBasedWaterEffect,\t";
		array[451] = (this._HeightBasedWaterEffect > 0).ToString();
		array[452] = ",\t";
		array[453] = this.fingerprint._HeightBasedWaterEffect.ToString();
		array[454] = "\n_WaterCaustics,\t";
		array[455] = (this._WaterCaustics > 0).ToString();
		array[456] = ",\t";
		array[457] = this.fingerprint._WaterCaustics.ToString();
		array[458] = "\n_UseDayNightLightmap,\t";
		array[459] = (this._UseDayNightLightmap > 0).ToString();
		array[460] = ",\t";
		array[461] = this.fingerprint._UseDayNightLightmap.ToString();
		array[462] = "\n_UseSpecular,\t";
		array[463] = (this._UseSpecular > 0).ToString();
		array[464] = ",\t";
		array[465] = this.fingerprint._UseSpecular.ToString();
		array[466] = "\n_UseSpecularAlphaChannel,\t";
		array[467] = (this._UseSpecularAlphaChannel > 0).ToString();
		array[468] = ",\t";
		array[469] = this.fingerprint._UseSpecularAlphaChannel.ToString();
		array[470] = "\n_Smoothness,\t";
		array[471] = (this._Smoothness > 0).ToString();
		array[472] = ",\t";
		array[473] = this.fingerprint._Smoothness.ToString();
		array[474] = "\n_UseSpecHighlight,\t";
		array[475] = (this._UseSpecHighlight > 0).ToString();
		array[476] = ",\t";
		array[477] = this.fingerprint._UseSpecHighlight.ToString();
		array[478] = "\n_SpecularDir,\t";
		array[479] = (this._SpecularDir > 0).ToString();
		array[480] = ",\t";
		int num39 = 481;
		@int = this.fingerprint._SpecularDir;
		array[num39] = @int.ToString();
		array[482] = "\n_SpecularPowerIntensity,\t";
		array[483] = (this._SpecularPowerIntensity > 0).ToString();
		array[484] = ",\t";
		int num40 = 485;
		@int = this.fingerprint._SpecularPowerIntensity;
		array[num40] = @int.ToString();
		array[486] = "\n_SpecularColor,\t";
		array[487] = (this._SpecularColor > 0).ToString();
		array[488] = ",\t";
		int num41 = 489;
		@int = this.fingerprint._SpecularColor;
		array[num41] = @int.ToString();
		array[490] = "\n_SpecularUseDiffuseColor,\t";
		array[491] = (this._SpecularUseDiffuseColor > 0).ToString();
		array[492] = ",\t";
		array[493] = this.fingerprint._SpecularUseDiffuseColor.ToString();
		array[494] = "\n_EmissionToggle,\t";
		array[495] = (this._EmissionToggle > 0).ToString();
		array[496] = ",\t";
		array[497] = this.fingerprint._EmissionToggle.ToString();
		array[498] = "\n_EmissionColor,\t";
		array[499] = (this._EmissionColor > 0).ToString();
		array[500] = ",\t";
		int num42 = 501;
		@int = this.fingerprint._EmissionColor;
		array[num42] = @int.ToString();
		array[502] = "\n_EmissionMap,\t";
		array[503] = (this._EmissionMap > 0).ToString();
		array[504] = ",\t";
		array[505] = this.fingerprint._EmissionMap;
		array[506] = "\n_EmissionMaskByBaseMapAlpha,\t";
		array[507] = (this._EmissionMaskByBaseMapAlpha > 0).ToString();
		array[508] = ",\t";
		array[509] = this.fingerprint._EmissionMaskByBaseMapAlpha.ToString();
		array[510] = "\n_EmissionUVScrollSpeed,\t";
		array[511] = (this._EmissionUVScrollSpeed > 0).ToString();
		array[512] = ",\t";
		int num43 = 513;
		@int = this.fingerprint._EmissionUVScrollSpeed;
		array[num43] = @int.ToString();
		array[514] = "\n_EmissionDissolveProgress,\t";
		array[515] = (this._EmissionDissolveProgress > 0).ToString();
		array[516] = ",\t";
		array[517] = this.fingerprint._EmissionDissolveProgress.ToString();
		array[518] = "\n_EmissionDissolveAnimation,\t";
		array[519] = (this._EmissionDissolveAnimation > 0).ToString();
		array[520] = ",\t";
		int num44 = 521;
		@int = this.fingerprint._EmissionDissolveAnimation;
		array[num44] = @int.ToString();
		array[522] = "\n_EmissionDissolveEdgeSize,\t";
		array[523] = (this._EmissionDissolveEdgeSize > 0).ToString();
		array[524] = ",\t";
		array[525] = this.fingerprint._EmissionDissolveEdgeSize.ToString();
		array[526] = "\n_EmissionIntensityInDynamic,\t";
		array[527] = (this._EmissionIntensityInDynamic > 0).ToString();
		array[528] = ",\t";
		array[529] = this.fingerprint._EmissionIntensityInDynamic.ToString();
		array[530] = "\n_EmissionUseUVWaveWarp,\t";
		array[531] = (this._EmissionUseUVWaveWarp > 0).ToString();
		array[532] = ",\t";
		array[533] = this.fingerprint._EmissionUseUVWaveWarp.ToString();
		array[534] = "\n_GreyZoneException,\t";
		array[535] = (this._GreyZoneException > 0).ToString();
		array[536] = ",\t";
		array[537] = this.fingerprint._GreyZoneException.ToString();
		array[538] = "\n_Cull,\t";
		array[539] = (this._Cull > 0).ToString();
		array[540] = ",\t";
		array[541] = this.fingerprint._Cull.ToString();
		array[542] = "\n_StencilReference,\t";
		array[543] = (this._StencilReference > 0).ToString();
		array[544] = ",\t";
		array[545] = this.fingerprint._StencilReference.ToString();
		array[546] = "\n_StencilComparison,\t";
		array[547] = (this._StencilComparison > 0).ToString();
		array[548] = ",\t";
		array[549] = this.fingerprint._StencilComparison.ToString();
		array[550] = "\n_StencilPassFront,\t";
		array[551] = (this._StencilPassFront > 0).ToString();
		array[552] = ",\t";
		array[553] = this.fingerprint._StencilPassFront.ToString();
		array[554] = "\n_USE_DEFORM_MAP,\t";
		array[555] = (this._USE_DEFORM_MAP > 0).ToString();
		array[556] = ",\t";
		array[557] = this.fingerprint._USE_DEFORM_MAP.ToString();
		array[558] = "\n_DeformMap,\t";
		array[559] = (this._DeformMap > 0).ToString();
		array[560] = ",\t";
		array[561] = this.fingerprint._DeformMap;
		array[562] = "\n_DeformMapIntensity,\t";
		array[563] = (this._DeformMapIntensity > 0).ToString();
		array[564] = ",\t";
		array[565] = this.fingerprint._DeformMapIntensity.ToString();
		array[566] = "\n_DeformMapMaskByVertColorRAmount,\t";
		array[567] = (this._DeformMapMaskByVertColorRAmount > 0).ToString();
		array[568] = ",\t";
		array[569] = this.fingerprint._DeformMapMaskByVertColorRAmount.ToString();
		array[570] = "\n_DeformMapScrollSpeed,\t";
		array[571] = (this._DeformMapScrollSpeed > 0).ToString();
		array[572] = ",\t";
		int num45 = 573;
		@int = this.fingerprint._DeformMapScrollSpeed;
		array[num45] = @int.ToString();
		array[574] = "\n_DeformMapUV0Influence,\t";
		array[575] = (this._DeformMapUV0Influence > 0).ToString();
		array[576] = ",\t";
		int num46 = 577;
		@int = this.fingerprint._DeformMapUV0Influence;
		array[num46] = @int.ToString();
		array[578] = "\n_DeformMapObjectSpaceOffsetsU,\t";
		array[579] = (this._DeformMapObjectSpaceOffsetsU > 0).ToString();
		array[580] = ",\t";
		int num47 = 581;
		@int = this.fingerprint._DeformMapObjectSpaceOffsetsU;
		array[num47] = @int.ToString();
		array[582] = "\n_DeformMapObjectSpaceOffsetsV,\t";
		array[583] = (this._DeformMapObjectSpaceOffsetsV > 0).ToString();
		array[584] = ",\t";
		int num48 = 585;
		@int = this.fingerprint._DeformMapObjectSpaceOffsetsV;
		array[num48] = @int.ToString();
		array[586] = "\n_DeformMapWorldSpaceOffsetsU,\t";
		array[587] = (this._DeformMapWorldSpaceOffsetsU > 0).ToString();
		array[588] = ",\t";
		int num49 = 589;
		@int = this.fingerprint._DeformMapWorldSpaceOffsetsU;
		array[num49] = @int.ToString();
		array[590] = "\n_DeformMapWorldSpaceOffsetsV,\t";
		array[591] = (this._DeformMapWorldSpaceOffsetsV > 0).ToString();
		array[592] = ",\t";
		int num50 = 593;
		@int = this.fingerprint._DeformMapWorldSpaceOffsetsV;
		array[num50] = @int.ToString();
		array[594] = "\n_RotateOnYAxisBySinTime,\t";
		array[595] = (this._RotateOnYAxisBySinTime > 0).ToString();
		array[596] = ",\t";
		int num51 = 597;
		@int = this.fingerprint._RotateOnYAxisBySinTime;
		array[num51] = @int.ToString();
		array[598] = "\n_USE_TEX_ARRAY_ATLAS,\t";
		array[599] = (this._USE_TEX_ARRAY_ATLAS > 0).ToString();
		array[600] = ",\t";
		array[601] = this.fingerprint._USE_TEX_ARRAY_ATLAS.ToString();
		array[602] = "\n_BaseMap_Atlas,\t";
		array[603] = (this._BaseMap_Atlas > 0).ToString();
		array[604] = ",\t";
		array[605] = this.fingerprint._BaseMap_Atlas;
		array[606] = "\n_BaseMap_AtlasSlice,\t";
		array[607] = (this._BaseMap_AtlasSlice > 0).ToString();
		array[608] = ",\t";
		array[609] = this.fingerprint._BaseMap_AtlasSlice.ToString();
		array[610] = "\n_BaseMap_AtlasSliceSource,\t";
		array[611] = (this._BaseMap_AtlasSliceSource > 0).ToString();
		array[612] = ",\t";
		array[613] = this.fingerprint._BaseMap_AtlasSliceSource.ToString();
		array[614] = "\n_EmissionMap_Atlas,\t";
		array[615] = (this._EmissionMap_Atlas > 0).ToString();
		array[616] = ",\t";
		array[617] = this.fingerprint._EmissionMap_Atlas;
		array[618] = "\n_EmissionMap_AtlasSlice,\t";
		array[619] = (this._EmissionMap_AtlasSlice > 0).ToString();
		array[620] = ",\t";
		array[621] = this.fingerprint._EmissionMap_AtlasSlice.ToString();
		array[622] = "\n_DeformMap_Atlas,\t";
		array[623] = (this._DeformMap_Atlas > 0).ToString();
		array[624] = ",\t";
		array[625] = this.fingerprint._DeformMap_Atlas;
		array[626] = "\n_DeformMap_AtlasSlice,\t";
		array[627] = (this._DeformMap_AtlasSlice > 0).ToString();
		array[628] = ",\t";
		array[629] = this.fingerprint._DeformMap_AtlasSlice.ToString();
		array[630] = "\n_WeatherMap_Atlas,\t";
		array[631] = (this._WeatherMap_Atlas > 0).ToString();
		array[632] = ",\t";
		array[633] = this.fingerprint._WeatherMap_Atlas;
		array[634] = "\n_WeatherMap_AtlasSlice,\t";
		array[635] = (this._WeatherMap_AtlasSlice > 0).ToString();
		array[636] = ",\t";
		array[637] = this.fingerprint._WeatherMap_AtlasSlice.ToString();
		array[638] = "\n_DEBUG_PAWN_DATA,\t";
		array[639] = (this._DEBUG_PAWN_DATA > 0).ToString();
		array[640] = ",\t";
		array[641] = this.fingerprint._DEBUG_PAWN_DATA.ToString();
		array[642] = "\n_SrcBlend,\t";
		array[643] = (this._SrcBlend > 0).ToString();
		array[644] = ",\t";
		array[645] = this.fingerprint._SrcBlend.ToString();
		array[646] = "\n_DstBlend,\t";
		array[647] = (this._DstBlend > 0).ToString();
		array[648] = ",\t";
		array[649] = this.fingerprint._DstBlend.ToString();
		array[650] = "\n_SrcBlendAlpha,\t";
		array[651] = (this._SrcBlendAlpha > 0).ToString();
		array[652] = ",\t";
		array[653] = this.fingerprint._SrcBlendAlpha.ToString();
		array[654] = "\n_DstBlendAlpha,\t";
		array[655] = (this._DstBlendAlpha > 0).ToString();
		array[656] = ",\t";
		array[657] = this.fingerprint._DstBlendAlpha.ToString();
		array[658] = "\n_ZWrite,\t";
		array[659] = (this._ZWrite > 0).ToString();
		array[660] = ",\t";
		array[661] = this.fingerprint._ZWrite.ToString();
		array[662] = "\n_AlphaToMask,\t";
		array[663] = (this._AlphaToMask > 0).ToString();
		array[664] = ",\t";
		array[665] = this.fingerprint._AlphaToMask.ToString();
		array[666] = "\n_Color,\t";
		array[667] = (this._Color > 0).ToString();
		array[668] = ",\t";
		int num52 = 669;
		@int = this.fingerprint._Color;
		array[num52] = @int.ToString();
		array[670] = "\n_Surface,\t";
		array[671] = (this._Surface > 0).ToString();
		array[672] = ",\t";
		array[673] = this.fingerprint._Surface.ToString();
		array[674] = "\n_Metallic,\t";
		array[675] = (this._Metallic > 0).ToString();
		array[676] = ",\t";
		array[677] = this.fingerprint._Metallic.ToString();
		array[678] = "\n_SpecColor,\t";
		array[679] = (this._SpecColor > 0).ToString();
		array[680] = ",\t";
		int num53 = 681;
		@int = this.fingerprint._SpecColor;
		array[num53] = @int.ToString();
		array[682] = "\n_DayNightLightmapArray,\t";
		array[683] = (this._DayNightLightmapArray > 0).ToString();
		array[684] = ",\t";
		array[685] = this.fingerprint._DayNightLightmapArray;
		array[686] = "\n_DayNightLightmapArray_ST,\t";
		array[687] = (this._DayNightLightmapArray_ST > 0).ToString();
		array[688] = ",\t";
		int num54 = 689;
		@int = this.fingerprint._DayNightLightmapArray_ST;
		array[num54] = @int.ToString();
		array[690] = "\n_DayNightLightmapArray_AtlasSlice,\t";
		array[691] = (this._DayNightLightmapArray_AtlasSlice > 0).ToString();
		array[692] = ",\t";
		array[693] = this.fingerprint._DayNightLightmapArray_AtlasSlice.ToString();
		array[694] = "\n";
		return string.Concat(array);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0004EE31 File Offset: 0x0004D031
	private static void _g_Macro_DECLARE_ATLASABLE_TEX2D(in GTUberShader_MaterialKeywordStates kw, ref int tex, ref int tex_Atlas)
	{
		tex += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		tex_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0004EE31 File Offset: 0x0004D031
	private static void _g_Macro_DECLARE_ATLASABLE_SAMPLER(in GTUberShader_MaterialKeywordStates kw, ref int sampler, ref int sampler_Atlas)
	{
		sampler += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		sampler_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x0004EE58 File Offset: 0x0004D058
	private static void _g_Macro_SAMPLE_ATLASABLE_TEX2D(in GTUberShader_MaterialKeywordStates kw, ref int tex, ref int tex_Atlas, ref int tex_AtlasSlice, ref int sampler, ref int sampler_Atlas, ref int coord2, ref int mipBias)
	{
		tex += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		tex_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		tex_AtlasSlice += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		sampler += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		sampler_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		mipBias++;
		coord2++;
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0004EE31 File Offset: 0x0004D031
	private static void _g_Macro_SAMPLE_ATLASABLE_TEX2D_LOD(in GTUberShader_MaterialKeywordStates kw, ref int texName, ref int texName_Atlas)
	{
		texName += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		texName_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x0004EECE File Offset: 0x0004D0CE
	private static void _g_Macro_SAMPLE_ATLASABLE_TEX2D_LOD(in GTUberShader_MaterialKeywordStates kw, ref int texName, ref int texName_Atlas, ref int sampler, ref int coord2, ref int lod)
	{
		texName += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		texName_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		sampler++;
		coord2++;
		lod++;
	}

	// Token: 0x04001108 RID: 4360
	public Material material;

	// Token: 0x04001109 RID: 4361
	public GTUberShader_MaterialKeywordStates kw;

	// Token: 0x0400110A RID: 4362
	public MaterialFingerprint fingerprint;

	// Token: 0x0400110B RID: 4363
	public bool IsValid;

	// Token: 0x0400110C RID: 4364
	private readonly int _notAProp;

	// Token: 0x0400110D RID: 4365
	public int _TransparencyMode;

	// Token: 0x0400110E RID: 4366
	public int _Cutoff;

	// Token: 0x0400110F RID: 4367
	public int _ColorSource;

	// Token: 0x04001110 RID: 4368
	public int _BaseColor;

	// Token: 0x04001111 RID: 4369
	public int _GChannelColor;

	// Token: 0x04001112 RID: 4370
	public int _BChannelColor;

	// Token: 0x04001113 RID: 4371
	public int _AChannelColor;

	// Token: 0x04001114 RID: 4372
	public int _TexMipBias;

	// Token: 0x04001115 RID: 4373
	public int _BaseMap;

	// Token: 0x04001116 RID: 4374
	public int _BaseMap_ST;

	// Token: 0x04001117 RID: 4375
	public int _BaseMap_WH;

	// Token: 0x04001118 RID: 4376
	public int _TexelSnapToggle;

	// Token: 0x04001119 RID: 4377
	public int _TexelSnap_Factor;

	// Token: 0x0400111A RID: 4378
	public int _UVSource;

	// Token: 0x0400111B RID: 4379
	public int _AlphaDetailToggle;

	// Token: 0x0400111C RID: 4380
	public int _AlphaDetail_ST;

	// Token: 0x0400111D RID: 4381
	public int _AlphaDetail_Opacity;

	// Token: 0x0400111E RID: 4382
	public int _AlphaDetail_WorldSpace;

	// Token: 0x0400111F RID: 4383
	public int _MaskMapToggle;

	// Token: 0x04001120 RID: 4384
	public int _MaskMap;

	// Token: 0x04001121 RID: 4385
	public int _MaskMap_ST;

	// Token: 0x04001122 RID: 4386
	public int _MaskMap_WH;

	// Token: 0x04001123 RID: 4387
	public int _LavaLampToggle;

	// Token: 0x04001124 RID: 4388
	public int _GradientMapToggle;

	// Token: 0x04001125 RID: 4389
	public int _GradientMap;

	// Token: 0x04001126 RID: 4390
	public int _DoTextureRotation;

	// Token: 0x04001127 RID: 4391
	public int _RotateAngle;

	// Token: 0x04001128 RID: 4392
	public int _RotateAnim;

	// Token: 0x04001129 RID: 4393
	public int _UseWaveWarp;

	// Token: 0x0400112A RID: 4394
	public int _WaveAmplitude;

	// Token: 0x0400112B RID: 4395
	public int _WaveFrequency;

	// Token: 0x0400112C RID: 4396
	public int _WaveScale;

	// Token: 0x0400112D RID: 4397
	public int _WaveTimeScale;

	// Token: 0x0400112E RID: 4398
	public int _UseWeatherMap;

	// Token: 0x0400112F RID: 4399
	public int _WeatherMap;

	// Token: 0x04001130 RID: 4400
	public int _WeatherMapDissolveEdgeSize;

	// Token: 0x04001131 RID: 4401
	public int _ReflectToggle;

	// Token: 0x04001132 RID: 4402
	public int _ReflectBoxProjectToggle;

	// Token: 0x04001133 RID: 4403
	public int _ReflectBoxCubePos;

	// Token: 0x04001134 RID: 4404
	public int _ReflectBoxSize;

	// Token: 0x04001135 RID: 4405
	public int _ReflectBoxRotation;

	// Token: 0x04001136 RID: 4406
	public int _ReflectMatcapToggle;

	// Token: 0x04001137 RID: 4407
	public int _ReflectMatcapPerspToggle;

	// Token: 0x04001138 RID: 4408
	public int _ReflectNormalToggle;

	// Token: 0x04001139 RID: 4409
	public int _ReflectTex;

	// Token: 0x0400113A RID: 4410
	public int _ReflectNormalTex;

	// Token: 0x0400113B RID: 4411
	public int _ReflectAlbedoTint;

	// Token: 0x0400113C RID: 4412
	public int _ReflectTint;

	// Token: 0x0400113D RID: 4413
	public int _ReflectOpacity;

	// Token: 0x0400113E RID: 4414
	public int _ReflectExposure;

	// Token: 0x0400113F RID: 4415
	public int _ReflectOffset;

	// Token: 0x04001140 RID: 4416
	public int _ReflectScale;

	// Token: 0x04001141 RID: 4417
	public int _ReflectRotate;

	// Token: 0x04001142 RID: 4418
	public int _HalfLambertToggle;

	// Token: 0x04001143 RID: 4419
	public int _ParallaxPlanarToggle;

	// Token: 0x04001144 RID: 4420
	public int _ParallaxToggle;

	// Token: 0x04001145 RID: 4421
	public int _ParallaxAAToggle;

	// Token: 0x04001146 RID: 4422
	public int _ParallaxAABias;

	// Token: 0x04001147 RID: 4423
	public int _DepthMap;

	// Token: 0x04001148 RID: 4424
	public int _ParallaxAmplitude;

	// Token: 0x04001149 RID: 4425
	public int _ParallaxSamplesMinMax;

	// Token: 0x0400114A RID: 4426
	public int _UvShiftToggle;

	// Token: 0x0400114B RID: 4427
	public int _UvShiftSteps;

	// Token: 0x0400114C RID: 4428
	public int _UvShiftRate;

	// Token: 0x0400114D RID: 4429
	public int _UvShiftOffset;

	// Token: 0x0400114E RID: 4430
	public int _UseGridEffect;

	// Token: 0x0400114F RID: 4431
	public int _UseCrystalEffect;

	// Token: 0x04001150 RID: 4432
	public int _CrystalPower;

	// Token: 0x04001151 RID: 4433
	public int _CrystalRimColor;

	// Token: 0x04001152 RID: 4434
	public int _LiquidVolume;

	// Token: 0x04001153 RID: 4435
	public int _LiquidFill;

	// Token: 0x04001154 RID: 4436
	public int _LiquidFillNormal;

	// Token: 0x04001155 RID: 4437
	public int _LiquidSurfaceColor;

	// Token: 0x04001156 RID: 4438
	public int _LiquidSwayX;

	// Token: 0x04001157 RID: 4439
	public int _LiquidSwayY;

	// Token: 0x04001158 RID: 4440
	public int _LiquidContainer;

	// Token: 0x04001159 RID: 4441
	public int _LiquidPlanePosition;

	// Token: 0x0400115A RID: 4442
	public int _LiquidPlaneNormal;

	// Token: 0x0400115B RID: 4443
	public int _VertexFlapToggle;

	// Token: 0x0400115C RID: 4444
	public int _VertexFlapAxis;

	// Token: 0x0400115D RID: 4445
	public int _VertexFlapDegreesMinMax;

	// Token: 0x0400115E RID: 4446
	public int _VertexFlapSpeed;

	// Token: 0x0400115F RID: 4447
	public int _VertexFlapPhaseOffset;

	// Token: 0x04001160 RID: 4448
	public int _VertexWaveToggle;

	// Token: 0x04001161 RID: 4449
	public int _VertexWaveDebug;

	// Token: 0x04001162 RID: 4450
	public int _VertexWaveEnd;

	// Token: 0x04001163 RID: 4451
	public int _VertexWaveParams;

	// Token: 0x04001164 RID: 4452
	public int _VertexWaveFalloff;

	// Token: 0x04001165 RID: 4453
	public int _VertexWaveSphereMask;

	// Token: 0x04001166 RID: 4454
	public int _VertexWavePhaseOffset;

	// Token: 0x04001167 RID: 4455
	public int _VertexWaveAxes;

	// Token: 0x04001168 RID: 4456
	public int _VertexRotateToggle;

	// Token: 0x04001169 RID: 4457
	public int _VertexRotateAngles;

	// Token: 0x0400116A RID: 4458
	public int _VertexRotateAnim;

	// Token: 0x0400116B RID: 4459
	public int _VertexLightToggle;

	// Token: 0x0400116C RID: 4460
	public int _InnerGlowOn;

	// Token: 0x0400116D RID: 4461
	public int _InnerGlowColor;

	// Token: 0x0400116E RID: 4462
	public int _InnerGlowParams;

	// Token: 0x0400116F RID: 4463
	public int _InnerGlowTap;

	// Token: 0x04001170 RID: 4464
	public int _InnerGlowSine;

	// Token: 0x04001171 RID: 4465
	public int _InnerGlowSinePeriod;

	// Token: 0x04001172 RID: 4466
	public int _InnerGlowSinePhaseShift;

	// Token: 0x04001173 RID: 4467
	public int _StealthEffectOn;

	// Token: 0x04001174 RID: 4468
	public int _UseEyeTracking;

	// Token: 0x04001175 RID: 4469
	public int _EyeTileOffsetUV;

	// Token: 0x04001176 RID: 4470
	public int _EyeOverrideUV;

	// Token: 0x04001177 RID: 4471
	public int _EyeOverrideUVTransform;

	// Token: 0x04001178 RID: 4472
	public int _UseMouthFlap;

	// Token: 0x04001179 RID: 4473
	public int _MouthMap;

	// Token: 0x0400117A RID: 4474
	public int _MouthMap_ST;

	// Token: 0x0400117B RID: 4475
	public int _UseVertexColor;

	// Token: 0x0400117C RID: 4476
	public int _WaterEffect;

	// Token: 0x0400117D RID: 4477
	public int _HeightBasedWaterEffect;

	// Token: 0x0400117E RID: 4478
	public int _WaterCaustics;

	// Token: 0x0400117F RID: 4479
	public int _UseDayNightLightmap;

	// Token: 0x04001180 RID: 4480
	public int _UseSpecular;

	// Token: 0x04001181 RID: 4481
	public int _UseSpecularAlphaChannel;

	// Token: 0x04001182 RID: 4482
	public int _Smoothness;

	// Token: 0x04001183 RID: 4483
	public int _UseSpecHighlight;

	// Token: 0x04001184 RID: 4484
	public int _SpecularDir;

	// Token: 0x04001185 RID: 4485
	public int _SpecularPowerIntensity;

	// Token: 0x04001186 RID: 4486
	public int _SpecularColor;

	// Token: 0x04001187 RID: 4487
	public int _SpecularUseDiffuseColor;

	// Token: 0x04001188 RID: 4488
	public int _EmissionToggle;

	// Token: 0x04001189 RID: 4489
	public int _EmissionColor;

	// Token: 0x0400118A RID: 4490
	public int _EmissionMap;

	// Token: 0x0400118B RID: 4491
	public int _EmissionMaskByBaseMapAlpha;

	// Token: 0x0400118C RID: 4492
	public int _EmissionUVScrollSpeed;

	// Token: 0x0400118D RID: 4493
	public int _EmissionDissolveProgress;

	// Token: 0x0400118E RID: 4494
	public int _EmissionDissolveAnimation;

	// Token: 0x0400118F RID: 4495
	public int _EmissionDissolveEdgeSize;

	// Token: 0x04001190 RID: 4496
	public int _EmissionIntensityInDynamic;

	// Token: 0x04001191 RID: 4497
	public int _EmissionUseUVWaveWarp;

	// Token: 0x04001192 RID: 4498
	public int _GreyZoneException;

	// Token: 0x04001193 RID: 4499
	public int _Cull;

	// Token: 0x04001194 RID: 4500
	public int _StencilReference;

	// Token: 0x04001195 RID: 4501
	public int _StencilComparison;

	// Token: 0x04001196 RID: 4502
	public int _StencilPassFront;

	// Token: 0x04001197 RID: 4503
	public int _USE_DEFORM_MAP;

	// Token: 0x04001198 RID: 4504
	public int _DeformMap;

	// Token: 0x04001199 RID: 4505
	public int _DeformMapIntensity;

	// Token: 0x0400119A RID: 4506
	public int _DeformMapMaskByVertColorRAmount;

	// Token: 0x0400119B RID: 4507
	public int _DeformMapScrollSpeed;

	// Token: 0x0400119C RID: 4508
	public int _DeformMapUV0Influence;

	// Token: 0x0400119D RID: 4509
	public int _DeformMapObjectSpaceOffsetsU;

	// Token: 0x0400119E RID: 4510
	public int _DeformMapObjectSpaceOffsetsV;

	// Token: 0x0400119F RID: 4511
	public int _DeformMapWorldSpaceOffsetsU;

	// Token: 0x040011A0 RID: 4512
	public int _DeformMapWorldSpaceOffsetsV;

	// Token: 0x040011A1 RID: 4513
	public int _RotateOnYAxisBySinTime;

	// Token: 0x040011A2 RID: 4514
	public int _USE_TEX_ARRAY_ATLAS;

	// Token: 0x040011A3 RID: 4515
	public int _BaseMap_Atlas;

	// Token: 0x040011A4 RID: 4516
	public int _BaseMap_AtlasSlice;

	// Token: 0x040011A5 RID: 4517
	public int _BaseMap_AtlasSliceSource;

	// Token: 0x040011A6 RID: 4518
	public int _EmissionMap_Atlas;

	// Token: 0x040011A7 RID: 4519
	public int _EmissionMap_AtlasSlice;

	// Token: 0x040011A8 RID: 4520
	public int _DeformMap_Atlas;

	// Token: 0x040011A9 RID: 4521
	public int _DeformMap_AtlasSlice;

	// Token: 0x040011AA RID: 4522
	public int _WeatherMap_Atlas;

	// Token: 0x040011AB RID: 4523
	public int _WeatherMap_AtlasSlice;

	// Token: 0x040011AC RID: 4524
	public int _DEBUG_PAWN_DATA;

	// Token: 0x040011AD RID: 4525
	public int _SrcBlend;

	// Token: 0x040011AE RID: 4526
	public int _DstBlend;

	// Token: 0x040011AF RID: 4527
	public int _SrcBlendAlpha;

	// Token: 0x040011B0 RID: 4528
	public int _DstBlendAlpha;

	// Token: 0x040011B1 RID: 4529
	public int _ZWrite;

	// Token: 0x040011B2 RID: 4530
	public int _AlphaToMask;

	// Token: 0x040011B3 RID: 4531
	public int _Color;

	// Token: 0x040011B4 RID: 4532
	public int _Surface;

	// Token: 0x040011B5 RID: 4533
	public int _Metallic;

	// Token: 0x040011B6 RID: 4534
	public int _SpecColor;

	// Token: 0x040011B7 RID: 4535
	public int _DayNightLightmapArray;

	// Token: 0x040011B8 RID: 4536
	public int _DayNightLightmapArray_ST;

	// Token: 0x040011B9 RID: 4537
	public int _DayNightLightmapArray_AtlasSlice;
}
