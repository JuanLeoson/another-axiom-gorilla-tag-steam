using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000233 RID: 563
[StructLayout(LayoutKind.Auto)]
public struct MaterialFingerprint
{
	// Token: 0x06000D34 RID: 3380 RVA: 0x00046A98 File Offset: 0x00044C98
	public MaterialFingerprint(UberShaderMatUsedProps used)
	{
		Material material = used.material;
		this._TransparencyMode = ((used._TransparencyMode > 0) ? material.GetInt(ShaderProps._TransparencyMode) : 0);
		this._Cutoff = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Cutoff), 100, used._Cutoff);
		this._ColorSource = ((used._ColorSource > 0) ? material.GetInt(ShaderProps._ColorSource) : 0);
		this._BaseColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._BaseColor), 100, used._BaseColor);
		this._GChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._GChannelColor), 100, used._GChannelColor);
		this._BChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._BChannelColor), 100, used._BChannelColor);
		this._AChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._AChannelColor), 100, used._AChannelColor);
		this._TexMipBias = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexMipBias), 100, used._TexMipBias);
		this._BaseMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._BaseMap, used._BaseMap);
		this._BaseMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._BaseMap_ST), 100, used._BaseMap_ST);
		this._BaseMap_WH = MaterialFingerprint._Round(material.GetVector(ShaderProps._BaseMap_WH), 100, used._BaseMap_WH);
		this._TexelSnapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexelSnapToggle), 100, used._TexelSnapToggle);
		this._TexelSnap_Factor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexelSnap_Factor), 100, used._TexelSnap_Factor);
		this._UVSource = ((used._UVSource > 0) ? material.GetInt(ShaderProps._UVSource) : 0);
		this._AlphaDetailToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetailToggle), 100, used._AlphaDetailToggle);
		this._AlphaDetail_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._AlphaDetail_ST), 100, used._AlphaDetail_ST);
		this._AlphaDetail_Opacity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetail_Opacity), 100, used._AlphaDetail_Opacity);
		this._AlphaDetail_WorldSpace = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetail_WorldSpace), 100, used._AlphaDetail_WorldSpace);
		this._MaskMapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._MaskMapToggle), 100, used._MaskMapToggle);
		this._MaskMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._MaskMap, used._MaskMap);
		this._MaskMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._MaskMap_ST), 100, used._MaskMap_ST);
		this._MaskMap_WH = MaterialFingerprint._Round(material.GetVector(ShaderProps._MaskMap_WH), 100, used._MaskMap_WH);
		this._LavaLampToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LavaLampToggle), 100, used._LavaLampToggle);
		this._GradientMapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._GradientMapToggle), 100, used._GradientMapToggle);
		this._GradientMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._GradientMap, used._GradientMap);
		this._DoTextureRotation = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DoTextureRotation), 100, used._DoTextureRotation);
		this._RotateAngle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._RotateAngle), 100, used._RotateAngle);
		this._RotateAnim = MaterialFingerprint._Round(material.GetFloat(ShaderProps._RotateAnim), 100, used._RotateAnim);
		this._UseWaveWarp = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseWaveWarp), 100, used._UseWaveWarp);
		this._WaveAmplitude = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveAmplitude), 100, used._WaveAmplitude);
		this._WaveFrequency = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveFrequency), 100, used._WaveFrequency);
		this._WaveScale = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveScale), 100, used._WaveScale);
		this._WaveTimeScale = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveTimeScale), 100, used._WaveTimeScale);
		this._UseWeatherMap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseWeatherMap), 100, used._UseWeatherMap);
		this._WeatherMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._WeatherMap, used._WeatherMap);
		this._WeatherMapDissolveEdgeSize = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WeatherMapDissolveEdgeSize), 100, used._WeatherMapDissolveEdgeSize);
		this._ReflectToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectToggle), 100, used._ReflectToggle);
		this._ReflectBoxProjectToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectBoxProjectToggle), 100, used._ReflectBoxProjectToggle);
		this._ReflectBoxCubePos = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxCubePos), 100, used._ReflectBoxCubePos);
		this._ReflectBoxSize = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxSize), 100, used._ReflectBoxSize);
		this._ReflectBoxRotation = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxRotation), 100, used._ReflectBoxRotation);
		this._ReflectMatcapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectMatcapToggle), 100, used._ReflectMatcapToggle);
		this._ReflectMatcapPerspToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectMatcapPerspToggle), 100, used._ReflectMatcapPerspToggle);
		this._ReflectNormalToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectNormalToggle), 100, used._ReflectNormalToggle);
		this._ReflectTex = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._ReflectTex, used._ReflectTex);
		this._ReflectNormalTex = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._ReflectNormalTex, used._ReflectNormalTex);
		this._ReflectAlbedoTint = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectAlbedoTint), 100, used._ReflectAlbedoTint);
		this._ReflectTint = MaterialFingerprint._Round(material.GetColor(ShaderProps._ReflectTint), 100, used._ReflectTint);
		this._ReflectOpacity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectOpacity), 100, used._ReflectOpacity);
		this._ReflectExposure = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectExposure), 100, used._ReflectExposure);
		this._ReflectOffset = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectOffset), 100, used._ReflectOffset);
		this._ReflectScale = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectScale), 100, used._ReflectScale);
		this._ReflectRotate = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectRotate), 100, used._ReflectRotate);
		this._HalfLambertToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._HalfLambertToggle), 100, used._HalfLambertToggle);
		this._ParallaxPlanarToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxPlanarToggle), 100, used._ParallaxPlanarToggle);
		this._ParallaxToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxToggle), 100, used._ParallaxToggle);
		this._ParallaxAAToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAAToggle), 100, used._ParallaxAAToggle);
		this._ParallaxAABias = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAABias), 100, used._ParallaxAABias);
		this._DepthMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DepthMap, used._DepthMap);
		this._ParallaxAmplitude = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAmplitude), 100, used._ParallaxAmplitude);
		this._ParallaxSamplesMinMax = MaterialFingerprint._Round(material.GetVector(ShaderProps._ParallaxSamplesMinMax), 100, used._ParallaxSamplesMinMax);
		this._UvShiftToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UvShiftToggle), 100, used._UvShiftToggle);
		this._UvShiftSteps = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftSteps), 100, used._UvShiftSteps);
		this._UvShiftRate = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftRate), 100, used._UvShiftRate);
		this._UvShiftOffset = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftOffset), 100, used._UvShiftOffset);
		this._UseGridEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseGridEffect), 100, used._UseGridEffect);
		this._UseCrystalEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseCrystalEffect), 100, used._UseCrystalEffect);
		this._CrystalPower = MaterialFingerprint._Round(material.GetFloat(ShaderProps._CrystalPower), 100, used._CrystalPower);
		this._CrystalRimColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._CrystalRimColor), 100, used._CrystalRimColor);
		this._LiquidVolume = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidVolume), 100, used._LiquidVolume);
		this._LiquidFill = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidFill), 100, used._LiquidFill);
		this._LiquidFillNormal = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidFillNormal), 100, used._LiquidFillNormal);
		this._LiquidSurfaceColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._LiquidSurfaceColor), 100, used._LiquidSurfaceColor);
		this._LiquidSwayX = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidSwayX), 100, used._LiquidSwayX);
		this._LiquidSwayY = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidSwayY), 100, used._LiquidSwayY);
		this._LiquidContainer = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidContainer), 100, used._LiquidContainer);
		this._LiquidPlanePosition = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidPlanePosition), 100, used._LiquidPlanePosition);
		this._LiquidPlaneNormal = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidPlaneNormal), 100, used._LiquidPlaneNormal);
		this._VertexFlapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapToggle), 100, used._VertexFlapToggle);
		this._VertexFlapAxis = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexFlapAxis), 100, used._VertexFlapAxis);
		this._VertexFlapDegreesMinMax = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexFlapDegreesMinMax), 100, used._VertexFlapDegreesMinMax);
		this._VertexFlapSpeed = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapSpeed), 100, used._VertexFlapSpeed);
		this._VertexFlapPhaseOffset = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapPhaseOffset), 100, used._VertexFlapPhaseOffset);
		this._VertexWaveToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWaveToggle), 100, used._VertexWaveToggle);
		this._VertexWaveDebug = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWaveDebug), 100, used._VertexWaveDebug);
		this._VertexWaveEnd = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveEnd), 100, used._VertexWaveEnd);
		this._VertexWaveParams = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveParams), 100, used._VertexWaveParams);
		this._VertexWaveFalloff = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveFalloff), 100, used._VertexWaveFalloff);
		this._VertexWaveSphereMask = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveSphereMask), 100, used._VertexWaveSphereMask);
		this._VertexWavePhaseOffset = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWavePhaseOffset), 100, used._VertexWavePhaseOffset);
		this._VertexWaveAxes = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveAxes), 100, used._VertexWaveAxes);
		this._VertexRotateToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexRotateToggle), 100, used._VertexRotateToggle);
		this._VertexRotateAngles = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexRotateAngles), 100, used._VertexRotateAngles);
		this._VertexRotateAnim = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexRotateAnim), 100, used._VertexRotateAnim);
		this._VertexLightToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexLightToggle), 100, used._VertexLightToggle);
		this._InnerGlowOn = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowOn), 100, used._InnerGlowOn);
		this._InnerGlowColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._InnerGlowColor), 100, used._InnerGlowColor);
		this._InnerGlowParams = MaterialFingerprint._Round(material.GetVector(ShaderProps._InnerGlowParams), 100, used._InnerGlowParams);
		this._InnerGlowTap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowTap), 100, used._InnerGlowTap);
		this._InnerGlowSine = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSine), 100, used._InnerGlowSine);
		this._InnerGlowSinePeriod = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSinePeriod), 100, used._InnerGlowSinePeriod);
		this._InnerGlowSinePhaseShift = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSinePhaseShift), 100, used._InnerGlowSinePhaseShift);
		this._StealthEffectOn = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StealthEffectOn), 100, used._StealthEffectOn);
		this._UseEyeTracking = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseEyeTracking), 100, used._UseEyeTracking);
		this._EyeTileOffsetUV = MaterialFingerprint._Round(material.GetVector(ShaderProps._EyeTileOffsetUV), 100, used._EyeTileOffsetUV);
		this._EyeOverrideUV = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EyeOverrideUV), 100, used._EyeOverrideUV);
		this._EyeOverrideUVTransform = MaterialFingerprint._Round(material.GetVector(ShaderProps._EyeOverrideUVTransform), 100, used._EyeOverrideUVTransform);
		this._UseMouthFlap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseMouthFlap), 100, used._UseMouthFlap);
		this._MouthMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._MouthMap, used._MouthMap);
		this._MouthMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._MouthMap_ST), 100, used._MouthMap_ST);
		this._UseVertexColor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseVertexColor), 100, used._UseVertexColor);
		this._WaterEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaterEffect), 100, used._WaterEffect);
		this._HeightBasedWaterEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._HeightBasedWaterEffect), 100, used._HeightBasedWaterEffect);
		this._WaterCaustics = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaterCaustics), 100, used._WaterCaustics);
		this._UseDayNightLightmap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseDayNightLightmap), 100, used._UseDayNightLightmap);
		this._UseSpecular = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecular), 100, used._UseSpecular);
		this._UseSpecularAlphaChannel = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecularAlphaChannel), 100, used._UseSpecularAlphaChannel);
		this._Smoothness = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Smoothness), 100, used._Smoothness);
		this._UseSpecHighlight = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecHighlight), 100, used._UseSpecHighlight);
		this._SpecularDir = MaterialFingerprint._Round(material.GetVector(ShaderProps._SpecularDir), 100, used._SpecularDir);
		this._SpecularPowerIntensity = MaterialFingerprint._Round(material.GetVector(ShaderProps._SpecularPowerIntensity), 100, used._SpecularPowerIntensity);
		this._SpecularColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._SpecularColor), 100, used._SpecularColor);
		this._SpecularUseDiffuseColor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SpecularUseDiffuseColor), 100, used._SpecularUseDiffuseColor);
		this._EmissionToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionToggle), 100, used._EmissionToggle);
		this._EmissionColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._EmissionColor), 100, used._EmissionColor);
		this._EmissionMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._EmissionMap, used._EmissionMap);
		this._EmissionMaskByBaseMapAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionMaskByBaseMapAlpha), 100, used._EmissionMaskByBaseMapAlpha);
		this._EmissionUVScrollSpeed = MaterialFingerprint._Round(material.GetVector(ShaderProps._EmissionUVScrollSpeed), 100, used._EmissionUVScrollSpeed);
		this._EmissionDissolveProgress = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionDissolveProgress), 100, used._EmissionDissolveProgress);
		this._EmissionDissolveAnimation = MaterialFingerprint._Round(material.GetVector(ShaderProps._EmissionDissolveAnimation), 100, used._EmissionDissolveAnimation);
		this._EmissionDissolveEdgeSize = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionDissolveEdgeSize), 100, used._EmissionDissolveEdgeSize);
		this._EmissionIntensityInDynamic = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionIntensityInDynamic), 100, used._EmissionIntensityInDynamic);
		this._EmissionUseUVWaveWarp = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionUseUVWaveWarp), 100, used._EmissionUseUVWaveWarp);
		this._GreyZoneException = MaterialFingerprint._Round(material.GetFloat(ShaderProps._GreyZoneException), 100, used._GreyZoneException);
		this._Cull = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Cull), 100, used._Cull);
		this._StencilReference = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilReference), 100, used._StencilReference);
		this._StencilComparison = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilComparison), 100, used._StencilComparison);
		this._StencilPassFront = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilPassFront), 100, used._StencilPassFront);
		this._USE_DEFORM_MAP = MaterialFingerprint._Round(material.GetFloat(ShaderProps._USE_DEFORM_MAP), 100, used._USE_DEFORM_MAP);
		this._DeformMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DeformMap, used._DeformMap);
		this._DeformMapIntensity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMapIntensity), 100, used._DeformMapIntensity);
		this._DeformMapMaskByVertColorRAmount = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMapMaskByVertColorRAmount), 100, used._DeformMapMaskByVertColorRAmount);
		this._DeformMapScrollSpeed = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapScrollSpeed), 100, used._DeformMapScrollSpeed);
		this._DeformMapUV0Influence = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapUV0Influence), 100, used._DeformMapUV0Influence);
		this._DeformMapObjectSpaceOffsetsU = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapObjectSpaceOffsetsU), 100, used._DeformMapObjectSpaceOffsetsU);
		this._DeformMapObjectSpaceOffsetsV = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapObjectSpaceOffsetsV), 100, used._DeformMapObjectSpaceOffsetsV);
		this._DeformMapWorldSpaceOffsetsU = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapWorldSpaceOffsetsU), 100, used._DeformMapWorldSpaceOffsetsU);
		this._DeformMapWorldSpaceOffsetsV = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapWorldSpaceOffsetsV), 100, used._DeformMapWorldSpaceOffsetsV);
		this._RotateOnYAxisBySinTime = MaterialFingerprint._Round(material.GetVector(ShaderProps._RotateOnYAxisBySinTime), 100, used._RotateOnYAxisBySinTime);
		this._USE_TEX_ARRAY_ATLAS = MaterialFingerprint._Round(material.GetFloat(ShaderProps._USE_TEX_ARRAY_ATLAS), 100, used._USE_TEX_ARRAY_ATLAS);
		this._BaseMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._BaseMap_Atlas, used._BaseMap_Atlas);
		this._BaseMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._BaseMap_AtlasSlice), 100, used._BaseMap_AtlasSlice);
		this._BaseMap_AtlasSliceSource = MaterialFingerprint._Round(material.GetFloat(ShaderProps._BaseMap_AtlasSliceSource), 100, used._BaseMap_AtlasSliceSource);
		this._EmissionMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._EmissionMap_Atlas, used._EmissionMap_Atlas);
		this._EmissionMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionMap_AtlasSlice), 100, used._EmissionMap_AtlasSlice);
		this._DeformMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DeformMap_Atlas, used._DeformMap_Atlas);
		this._DeformMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMap_AtlasSlice), 100, used._DeformMap_AtlasSlice);
		this._WeatherMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._WeatherMap_Atlas, used._WeatherMap_Atlas);
		this._WeatherMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WeatherMap_AtlasSlice), 100, used._WeatherMap_AtlasSlice);
		this._DEBUG_PAWN_DATA = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DEBUG_PAWN_DATA), 100, used._DEBUG_PAWN_DATA);
		this._SrcBlend = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SrcBlend), 100, used._SrcBlend);
		this._DstBlend = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DstBlend), 100, used._DstBlend);
		this._SrcBlendAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SrcBlendAlpha), 100, used._SrcBlendAlpha);
		this._DstBlendAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DstBlendAlpha), 100, used._DstBlendAlpha);
		this._ZWrite = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ZWrite), 100, used._ZWrite);
		this._AlphaToMask = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaToMask), 100, used._AlphaToMask);
		this._Color = MaterialFingerprint._Round(material.GetColor(ShaderProps._Color), 100, used._Color);
		this._Surface = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Surface), 100, used._Surface);
		this._Metallic = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Metallic), 100, used._Metallic);
		this._SpecColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._SpecColor), 100, used._SpecColor);
		this._DayNightLightmapArray = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DayNightLightmapArray, used._DayNightLightmapArray);
		this._DayNightLightmapArray_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._DayNightLightmapArray_ST), 100, used._DayNightLightmapArray_ST);
		this._DayNightLightmapArray_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DayNightLightmapArray_AtlasSlice), 100, used._DayNightLightmapArray_AtlasSlice);
		this.isValid = true;
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00047E90 File Offset: 0x00046090
	private static int4 _Round(Color c, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return int4.zero;
		}
		return new int4(Mathf.RoundToInt(c.r * (float)mul), Mathf.RoundToInt(c.g * (float)mul), Mathf.RoundToInt(c.b * (float)mul), Mathf.RoundToInt(c.a * (float)mul));
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00047EE4 File Offset: 0x000460E4
	private static int4 _Round(Vector4 v, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return int4.zero;
		}
		return new int4(Mathf.RoundToInt(v.x * (float)mul), Mathf.RoundToInt(v.y * (float)mul), Mathf.RoundToInt(v.z * (float)mul), Mathf.RoundToInt(v.w * (float)mul));
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00047F38 File Offset: 0x00046138
	private static int _Round(float f, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return 0;
		}
		return Mathf.RoundToInt(f * (float)mul);
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00047F4C File Offset: 0x0004614C
	private static TexFormatInfo _GetTexFormatInfo(Material mat, string texPropName, int usedCount)
	{
		if (usedCount > 0)
		{
			Texture2D texture2D = mat.GetTexture(texPropName) as Texture2D;
			if (texture2D != null)
			{
				return new TexFormatInfo(texture2D);
			}
		}
		return default(TexFormatInfo);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00047F83 File Offset: 0x00046183
	private static string _GetTexPropGuid(Material mat, int texPropId, int usedCount)
	{
		return string.Empty;
	}

	// Token: 0x0400101C RID: 4124
	public int _TransparencyMode;

	// Token: 0x0400101D RID: 4125
	public int _Cutoff;

	// Token: 0x0400101E RID: 4126
	public int _ColorSource;

	// Token: 0x0400101F RID: 4127
	public int4 _BaseColor;

	// Token: 0x04001020 RID: 4128
	public int4 _GChannelColor;

	// Token: 0x04001021 RID: 4129
	public int4 _BChannelColor;

	// Token: 0x04001022 RID: 4130
	public int4 _AChannelColor;

	// Token: 0x04001023 RID: 4131
	public int _TexMipBias;

	// Token: 0x04001024 RID: 4132
	public string _BaseMap;

	// Token: 0x04001025 RID: 4133
	public int4 _BaseMap_ST;

	// Token: 0x04001026 RID: 4134
	public int4 _BaseMap_WH;

	// Token: 0x04001027 RID: 4135
	public int _TexelSnapToggle;

	// Token: 0x04001028 RID: 4136
	public int _TexelSnap_Factor;

	// Token: 0x04001029 RID: 4137
	public int _UVSource;

	// Token: 0x0400102A RID: 4138
	public int _AlphaDetailToggle;

	// Token: 0x0400102B RID: 4139
	public int4 _AlphaDetail_ST;

	// Token: 0x0400102C RID: 4140
	public int _AlphaDetail_Opacity;

	// Token: 0x0400102D RID: 4141
	public int _AlphaDetail_WorldSpace;

	// Token: 0x0400102E RID: 4142
	public int _MaskMapToggle;

	// Token: 0x0400102F RID: 4143
	public string _MaskMap;

	// Token: 0x04001030 RID: 4144
	public int4 _MaskMap_ST;

	// Token: 0x04001031 RID: 4145
	public int4 _MaskMap_WH;

	// Token: 0x04001032 RID: 4146
	public int _LavaLampToggle;

	// Token: 0x04001033 RID: 4147
	public int _GradientMapToggle;

	// Token: 0x04001034 RID: 4148
	public string _GradientMap;

	// Token: 0x04001035 RID: 4149
	public int _DoTextureRotation;

	// Token: 0x04001036 RID: 4150
	public int _RotateAngle;

	// Token: 0x04001037 RID: 4151
	public int _RotateAnim;

	// Token: 0x04001038 RID: 4152
	public int _UseWaveWarp;

	// Token: 0x04001039 RID: 4153
	public int _WaveAmplitude;

	// Token: 0x0400103A RID: 4154
	public int _WaveFrequency;

	// Token: 0x0400103B RID: 4155
	public int _WaveScale;

	// Token: 0x0400103C RID: 4156
	public int _WaveTimeScale;

	// Token: 0x0400103D RID: 4157
	public int _UseWeatherMap;

	// Token: 0x0400103E RID: 4158
	public string _WeatherMap;

	// Token: 0x0400103F RID: 4159
	public int _WeatherMapDissolveEdgeSize;

	// Token: 0x04001040 RID: 4160
	public int _ReflectToggle;

	// Token: 0x04001041 RID: 4161
	public int _ReflectBoxProjectToggle;

	// Token: 0x04001042 RID: 4162
	public int4 _ReflectBoxCubePos;

	// Token: 0x04001043 RID: 4163
	public int4 _ReflectBoxSize;

	// Token: 0x04001044 RID: 4164
	public int4 _ReflectBoxRotation;

	// Token: 0x04001045 RID: 4165
	public int _ReflectMatcapToggle;

	// Token: 0x04001046 RID: 4166
	public int _ReflectMatcapPerspToggle;

	// Token: 0x04001047 RID: 4167
	public int _ReflectNormalToggle;

	// Token: 0x04001048 RID: 4168
	public string _ReflectTex;

	// Token: 0x04001049 RID: 4169
	public string _ReflectNormalTex;

	// Token: 0x0400104A RID: 4170
	public int _ReflectAlbedoTint;

	// Token: 0x0400104B RID: 4171
	public int4 _ReflectTint;

	// Token: 0x0400104C RID: 4172
	public int _ReflectOpacity;

	// Token: 0x0400104D RID: 4173
	public int _ReflectExposure;

	// Token: 0x0400104E RID: 4174
	public int4 _ReflectOffset;

	// Token: 0x0400104F RID: 4175
	public int4 _ReflectScale;

	// Token: 0x04001050 RID: 4176
	public int _ReflectRotate;

	// Token: 0x04001051 RID: 4177
	public int _HalfLambertToggle;

	// Token: 0x04001052 RID: 4178
	public int _ParallaxPlanarToggle;

	// Token: 0x04001053 RID: 4179
	public int _ParallaxToggle;

	// Token: 0x04001054 RID: 4180
	public int _ParallaxAAToggle;

	// Token: 0x04001055 RID: 4181
	public int _ParallaxAABias;

	// Token: 0x04001056 RID: 4182
	public string _DepthMap;

	// Token: 0x04001057 RID: 4183
	public int _ParallaxAmplitude;

	// Token: 0x04001058 RID: 4184
	public int4 _ParallaxSamplesMinMax;

	// Token: 0x04001059 RID: 4185
	public int _UvShiftToggle;

	// Token: 0x0400105A RID: 4186
	public int4 _UvShiftSteps;

	// Token: 0x0400105B RID: 4187
	public int4 _UvShiftRate;

	// Token: 0x0400105C RID: 4188
	public int4 _UvShiftOffset;

	// Token: 0x0400105D RID: 4189
	public int _UseGridEffect;

	// Token: 0x0400105E RID: 4190
	public int _UseCrystalEffect;

	// Token: 0x0400105F RID: 4191
	public int _CrystalPower;

	// Token: 0x04001060 RID: 4192
	public int4 _CrystalRimColor;

	// Token: 0x04001061 RID: 4193
	public int _LiquidVolume;

	// Token: 0x04001062 RID: 4194
	public int _LiquidFill;

	// Token: 0x04001063 RID: 4195
	public int4 _LiquidFillNormal;

	// Token: 0x04001064 RID: 4196
	public int4 _LiquidSurfaceColor;

	// Token: 0x04001065 RID: 4197
	public int _LiquidSwayX;

	// Token: 0x04001066 RID: 4198
	public int _LiquidSwayY;

	// Token: 0x04001067 RID: 4199
	public int _LiquidContainer;

	// Token: 0x04001068 RID: 4200
	public int4 _LiquidPlanePosition;

	// Token: 0x04001069 RID: 4201
	public int4 _LiquidPlaneNormal;

	// Token: 0x0400106A RID: 4202
	public int _VertexFlapToggle;

	// Token: 0x0400106B RID: 4203
	public int4 _VertexFlapAxis;

	// Token: 0x0400106C RID: 4204
	public int4 _VertexFlapDegreesMinMax;

	// Token: 0x0400106D RID: 4205
	public int _VertexFlapSpeed;

	// Token: 0x0400106E RID: 4206
	public int _VertexFlapPhaseOffset;

	// Token: 0x0400106F RID: 4207
	public int _VertexWaveToggle;

	// Token: 0x04001070 RID: 4208
	public int _VertexWaveDebug;

	// Token: 0x04001071 RID: 4209
	public int4 _VertexWaveEnd;

	// Token: 0x04001072 RID: 4210
	public int4 _VertexWaveParams;

	// Token: 0x04001073 RID: 4211
	public int4 _VertexWaveFalloff;

	// Token: 0x04001074 RID: 4212
	public int4 _VertexWaveSphereMask;

	// Token: 0x04001075 RID: 4213
	public int _VertexWavePhaseOffset;

	// Token: 0x04001076 RID: 4214
	public int4 _VertexWaveAxes;

	// Token: 0x04001077 RID: 4215
	public int _VertexRotateToggle;

	// Token: 0x04001078 RID: 4216
	public int4 _VertexRotateAngles;

	// Token: 0x04001079 RID: 4217
	public int _VertexRotateAnim;

	// Token: 0x0400107A RID: 4218
	public int _VertexLightToggle;

	// Token: 0x0400107B RID: 4219
	public int _InnerGlowOn;

	// Token: 0x0400107C RID: 4220
	public int4 _InnerGlowColor;

	// Token: 0x0400107D RID: 4221
	public int4 _InnerGlowParams;

	// Token: 0x0400107E RID: 4222
	public int _InnerGlowTap;

	// Token: 0x0400107F RID: 4223
	public int _InnerGlowSine;

	// Token: 0x04001080 RID: 4224
	public int _InnerGlowSinePeriod;

	// Token: 0x04001081 RID: 4225
	public int _InnerGlowSinePhaseShift;

	// Token: 0x04001082 RID: 4226
	public int _StealthEffectOn;

	// Token: 0x04001083 RID: 4227
	public int _UseEyeTracking;

	// Token: 0x04001084 RID: 4228
	public int4 _EyeTileOffsetUV;

	// Token: 0x04001085 RID: 4229
	public int _EyeOverrideUV;

	// Token: 0x04001086 RID: 4230
	public int4 _EyeOverrideUVTransform;

	// Token: 0x04001087 RID: 4231
	public int _UseMouthFlap;

	// Token: 0x04001088 RID: 4232
	public string _MouthMap;

	// Token: 0x04001089 RID: 4233
	public int4 _MouthMap_ST;

	// Token: 0x0400108A RID: 4234
	public int _UseVertexColor;

	// Token: 0x0400108B RID: 4235
	public int _WaterEffect;

	// Token: 0x0400108C RID: 4236
	public int _HeightBasedWaterEffect;

	// Token: 0x0400108D RID: 4237
	public int _WaterCaustics;

	// Token: 0x0400108E RID: 4238
	public int _UseDayNightLightmap;

	// Token: 0x0400108F RID: 4239
	public int _UseSpecular;

	// Token: 0x04001090 RID: 4240
	public int _UseSpecularAlphaChannel;

	// Token: 0x04001091 RID: 4241
	public int _Smoothness;

	// Token: 0x04001092 RID: 4242
	public int _UseSpecHighlight;

	// Token: 0x04001093 RID: 4243
	public int4 _SpecularDir;

	// Token: 0x04001094 RID: 4244
	public int4 _SpecularPowerIntensity;

	// Token: 0x04001095 RID: 4245
	public int4 _SpecularColor;

	// Token: 0x04001096 RID: 4246
	public int _SpecularUseDiffuseColor;

	// Token: 0x04001097 RID: 4247
	public int _EmissionToggle;

	// Token: 0x04001098 RID: 4248
	public int4 _EmissionColor;

	// Token: 0x04001099 RID: 4249
	public string _EmissionMap;

	// Token: 0x0400109A RID: 4250
	public int _EmissionMaskByBaseMapAlpha;

	// Token: 0x0400109B RID: 4251
	public int4 _EmissionUVScrollSpeed;

	// Token: 0x0400109C RID: 4252
	public int _EmissionDissolveProgress;

	// Token: 0x0400109D RID: 4253
	public int4 _EmissionDissolveAnimation;

	// Token: 0x0400109E RID: 4254
	public int _EmissionDissolveEdgeSize;

	// Token: 0x0400109F RID: 4255
	public int _EmissionIntensityInDynamic;

	// Token: 0x040010A0 RID: 4256
	public int _EmissionUseUVWaveWarp;

	// Token: 0x040010A1 RID: 4257
	public int _GreyZoneException;

	// Token: 0x040010A2 RID: 4258
	public int _Cull;

	// Token: 0x040010A3 RID: 4259
	public int _StencilReference;

	// Token: 0x040010A4 RID: 4260
	public int _StencilComparison;

	// Token: 0x040010A5 RID: 4261
	public int _StencilPassFront;

	// Token: 0x040010A6 RID: 4262
	public int _USE_DEFORM_MAP;

	// Token: 0x040010A7 RID: 4263
	public string _DeformMap;

	// Token: 0x040010A8 RID: 4264
	public int _DeformMapIntensity;

	// Token: 0x040010A9 RID: 4265
	public int _DeformMapMaskByVertColorRAmount;

	// Token: 0x040010AA RID: 4266
	public int4 _DeformMapScrollSpeed;

	// Token: 0x040010AB RID: 4267
	public int4 _DeformMapUV0Influence;

	// Token: 0x040010AC RID: 4268
	public int4 _DeformMapObjectSpaceOffsetsU;

	// Token: 0x040010AD RID: 4269
	public int4 _DeformMapObjectSpaceOffsetsV;

	// Token: 0x040010AE RID: 4270
	public int4 _DeformMapWorldSpaceOffsetsU;

	// Token: 0x040010AF RID: 4271
	public int4 _DeformMapWorldSpaceOffsetsV;

	// Token: 0x040010B0 RID: 4272
	public int4 _RotateOnYAxisBySinTime;

	// Token: 0x040010B1 RID: 4273
	public int _USE_TEX_ARRAY_ATLAS;

	// Token: 0x040010B2 RID: 4274
	public string _BaseMap_Atlas;

	// Token: 0x040010B3 RID: 4275
	public int _BaseMap_AtlasSlice;

	// Token: 0x040010B4 RID: 4276
	public int _BaseMap_AtlasSliceSource;

	// Token: 0x040010B5 RID: 4277
	public string _EmissionMap_Atlas;

	// Token: 0x040010B6 RID: 4278
	public int _EmissionMap_AtlasSlice;

	// Token: 0x040010B7 RID: 4279
	public string _DeformMap_Atlas;

	// Token: 0x040010B8 RID: 4280
	public int _DeformMap_AtlasSlice;

	// Token: 0x040010B9 RID: 4281
	public string _WeatherMap_Atlas;

	// Token: 0x040010BA RID: 4282
	public int _WeatherMap_AtlasSlice;

	// Token: 0x040010BB RID: 4283
	public int _DEBUG_PAWN_DATA;

	// Token: 0x040010BC RID: 4284
	public int _SrcBlend;

	// Token: 0x040010BD RID: 4285
	public int _DstBlend;

	// Token: 0x040010BE RID: 4286
	public int _SrcBlendAlpha;

	// Token: 0x040010BF RID: 4287
	public int _DstBlendAlpha;

	// Token: 0x040010C0 RID: 4288
	public int _ZWrite;

	// Token: 0x040010C1 RID: 4289
	public int _AlphaToMask;

	// Token: 0x040010C2 RID: 4290
	public int4 _Color;

	// Token: 0x040010C3 RID: 4291
	public int _Surface;

	// Token: 0x040010C4 RID: 4292
	public int _Metallic;

	// Token: 0x040010C5 RID: 4293
	public int4 _SpecColor;

	// Token: 0x040010C6 RID: 4294
	public string _DayNightLightmapArray;

	// Token: 0x040010C7 RID: 4295
	public int4 _DayNightLightmapArray_ST;

	// Token: 0x040010C8 RID: 4296
	public int _DayNightLightmapArray_AtlasSlice;

	// Token: 0x040010C9 RID: 4297
	public bool isValid;
}
