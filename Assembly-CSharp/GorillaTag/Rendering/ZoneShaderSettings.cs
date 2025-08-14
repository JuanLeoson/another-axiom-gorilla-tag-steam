using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EFB RID: 3835
	public class ZoneShaderSettings : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06005F14 RID: 24340 RVA: 0x001DF4BB File Offset: 0x001DD6BB
		// (set) Token: 0x06005F15 RID: 24341 RVA: 0x001DF4C2 File Offset: 0x001DD6C2
		[DebugReadout]
		public static ZoneShaderSettings defaultsInstance { get; private set; }

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06005F16 RID: 24342 RVA: 0x001DF4CA File Offset: 0x001DD6CA
		// (set) Token: 0x06005F17 RID: 24343 RVA: 0x001DF4D1 File Offset: 0x001DD6D1
		public static bool hasDefaultsInstance { get; private set; }

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06005F18 RID: 24344 RVA: 0x001DF4D9 File Offset: 0x001DD6D9
		// (set) Token: 0x06005F19 RID: 24345 RVA: 0x001DF4E0 File Offset: 0x001DD6E0
		[DebugReadout]
		public static ZoneShaderSettings activeInstance { get; private set; }

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x06005F1A RID: 24346 RVA: 0x001DF4E8 File Offset: 0x001DD6E8
		// (set) Token: 0x06005F1B RID: 24347 RVA: 0x001DF4EF File Offset: 0x001DD6EF
		public static bool hasActiveInstance { get; private set; }

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x06005F1C RID: 24348 RVA: 0x001DF4F7 File Offset: 0x001DD6F7
		public bool isActiveInstance
		{
			get
			{
				return ZoneShaderSettings.activeInstance == this;
			}
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06005F1D RID: 24349 RVA: 0x001DF504 File Offset: 0x001DD704
		[DebugReadout]
		private float GroundFogDepthFadeSq
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogDepthFadeSize * this._groundFogDepthFadeSize);
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06005F1E RID: 24350 RVA: 0x001DF523 File Offset: 0x001DD723
		[DebugReadout]
		private float GroundFogHeightFade
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this._groundFogHeightFadeSize);
			}
		}

		// Token: 0x06005F1F RID: 24351 RVA: 0x001DF53C File Offset: 0x001DD73C
		public void SetZoneLiquidTypeKeywordEnum(ZoneShaderSettings.EZoneLiquidType liquidType)
		{
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.None)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Water)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			if (liquidType == ZoneShaderSettings.EZoneLiquidType.Lava)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
				return;
			}
			Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		}

		// Token: 0x06005F20 RID: 24352 RVA: 0x001DF595 File Offset: 0x001DD795
		public void SetZoneLiquidShapeKeywordEnum(ZoneShaderSettings.ELiquidShape shape)
		{
			if (shape == ZoneShaderSettings.ELiquidShape.Plane)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			else
			{
				Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			if (shape == ZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
				return;
			}
			Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
		}

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06005F21 RID: 24353 RVA: 0x001DF5C9 File Offset: 0x001DD7C9
		// (set) Token: 0x06005F22 RID: 24354 RVA: 0x001DF5D0 File Offset: 0x001DD7D0
		public static int shaderParam_ZoneLiquidPosRadiusSq { get; private set; } = Shader.PropertyToID("_ZoneLiquidPosRadiusSq");

		// Token: 0x06005F23 RID: 24355 RVA: 0x001DF5D8 File Offset: 0x001DD7D8
		public static float GetWaterY()
		{
			return ZoneShaderSettings.activeInstance.mainWaterSurfacePlane.position.y;
		}

		// Token: 0x06005F24 RID: 24356 RVA: 0x001DF5F0 File Offset: 0x001DD7F0
		protected void Awake()
		{
			this.hasMainWaterSurfacePlane = (this.mainWaterSurfacePlane != null && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues));
			this.hasDynamicWaterSurfacePlane = (this.hasMainWaterSurfacePlane && !this.mainWaterSurfacePlane.gameObject.isStatic);
			this.hasLiquidBottomTransform = (this.liquidBottomTransform != null && (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues));
			this.CheckDefaultsInstance();
			if (this._activateOnAwake)
			{
				this.BecomeActiveInstance(false);
			}
		}

		// Token: 0x06005F25 RID: 24357 RVA: 0x001DF688 File Offset: 0x001DD888
		protected void OnEnable()
		{
			if (this.hasDynamicWaterSurfacePlane)
			{
				TickSystem<object>.AddPostTickCallback(this);
			}
		}

		// Token: 0x06005F26 RID: 24358 RVA: 0x00100D37 File Offset: 0x000FEF37
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005F27 RID: 24359 RVA: 0x001DF698 File Offset: 0x001DD898
		protected void OnDestroy()
		{
			if (ZoneShaderSettings.defaultsInstance == this)
			{
				ZoneShaderSettings.hasDefaultsInstance = false;
			}
			if (ZoneShaderSettings.activeInstance == this)
			{
				ZoneShaderSettings.hasActiveInstance = false;
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06005F28 RID: 24360 RVA: 0x001DF6C6 File Offset: 0x001DD8C6
		// (set) Token: 0x06005F29 RID: 24361 RVA: 0x001DF6CE File Offset: 0x001DD8CE
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005F2A RID: 24362 RVA: 0x001DF6D7 File Offset: 0x001DD8D7
		void ITickSystemPost.PostTick()
		{
			if (ZoneShaderSettings.activeInstance == this && Application.isPlaying && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateMainPlaneShaderProperty();
			}
		}

		// Token: 0x06005F2B RID: 24363 RVA: 0x001DF6FC File Offset: 0x001DD8FC
		private void UpdateMainPlaneShaderProperty()
		{
			Transform transform = null;
			bool flag = false;
			if (this.hasMainWaterSurfacePlane && (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues))
			{
				flag = true;
				transform = this.mainWaterSurfacePlane;
			}
			else if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasMainWaterSurfacePlane)
			{
				flag = true;
				transform = ZoneShaderSettings.defaultsInstance.mainWaterSurfacePlane;
			}
			if (!flag)
			{
				return;
			}
			Vector3 position = transform.position;
			Vector3 up = transform.up;
			float w = -Vector3.Dot(up, position);
			Shader.SetGlobalVector(this.shaderParam_GlobalMainWaterSurfacePlane, new Vector4(up.x, up.y, up.z, w));
			ZoneShaderSettings.ELiquidShape eliquidShape;
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				eliquidShape = this.liquidShape;
			}
			else if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance)
			{
				eliquidShape = ZoneShaderSettings.defaultsInstance.liquidShape;
			}
			else
			{
				eliquidShape = ZoneShaderSettings.liquidShape_previousValue;
			}
			ZoneShaderSettings.liquidShape_previousValue = eliquidShape;
			float y;
			if ((this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues) && this.hasLiquidBottomTransform)
			{
				y = this.liquidBottomTransform.position.y;
			}
			else if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance.hasLiquidBottomTransform)
			{
				y = ZoneShaderSettings.defaultsInstance.liquidBottomTransform.position.y;
			}
			else
			{
				y = this.liquidBottomPosY_previousValue;
			}
			float num;
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				num = this.liquidShapeRadius;
			}
			else if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue && ZoneShaderSettings.hasDefaultsInstance)
			{
				num = ZoneShaderSettings.defaultsInstance.liquidShapeRadius;
			}
			else
			{
				num = ZoneShaderSettings.liquidShapeRadius_previousValue;
			}
			if (eliquidShape == ZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Shader.SetGlobalVector(ZoneShaderSettings.shaderParam_ZoneLiquidPosRadiusSq, new Vector4(position.x, y, position.z, num * num));
				ZoneShaderSettings.liquidShapeRadius_previousValue = num;
			}
		}

		// Token: 0x06005F2C RID: 24364 RVA: 0x001DF8B8 File Offset: 0x001DDAB8
		private void CheckDefaultsInstance()
		{
			if (!this.isDefaultValues)
			{
				return;
			}
			if (ZoneShaderSettings.hasDefaultsInstance && ZoneShaderSettings.defaultsInstance != null && ZoneShaderSettings.defaultsInstance != this)
			{
				string path = ZoneShaderSettings.defaultsInstance.transform.GetPath();
				Debug.LogError(string.Concat(new string[]
				{
					"ZoneShaderSettings: Destroying conflicting defaults instance.\n- keeping: \"",
					path,
					"\"\n- destroying (this): \"",
					base.transform.GetPath(),
					"\""
				}), this);
				Object.Destroy(base.gameObject);
				return;
			}
			ZoneShaderSettings.defaultsInstance = this;
			ZoneShaderSettings.hasDefaultsInstance = true;
			this.BecomeActiveInstance(false);
		}

		// Token: 0x06005F2D RID: 24365 RVA: 0x001DF95C File Offset: 0x001DDB5C
		public void BecomeActiveInstance(bool force = false)
		{
			if (ZoneShaderSettings.activeInstance == this && !force)
			{
				return;
			}
			if (ZoneShaderSettings.activeInstance.IsNotNull())
			{
				TickSystem<object>.RemovePostTickCallback(ZoneShaderSettings.activeInstance);
			}
			if (this.hasDynamicWaterSurfacePlane)
			{
				TickSystem<object>.AddPostTickCallback(this);
			}
			this.ApplyValues();
			ZoneShaderSettings.activeInstance = this;
			ZoneShaderSettings.hasActiveInstance = true;
		}

		// Token: 0x06005F2E RID: 24366 RVA: 0x001DF9B0 File Offset: 0x001DDBB0
		public static void ActivateDefaultSettings()
		{
			if (ZoneShaderSettings.hasDefaultsInstance)
			{
				ZoneShaderSettings.defaultsInstance.BecomeActiveInstance(false);
			}
		}

		// Token: 0x06005F2F RID: 24367 RVA: 0x001DF9C4 File Offset: 0x001DDBC4
		public void SetGroundFogValue(Color fogColor, float fogDepthFade, float fogHeight, float fogHeightFade)
		{
			this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogColor = fogColor;
			this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this._groundFogDepthFadeSize = fogDepthFade;
			this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogHeight = fogHeight;
			this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this._groundFogHeightFadeSize = fogHeightFade;
			this.BecomeActiveInstance(true);
		}

		// Token: 0x06005F30 RID: 24368 RVA: 0x001DFA14 File Offset: 0x001DDC14
		private void ApplyValues()
		{
			if (!ZoneShaderSettings.hasDefaultsInstance || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.ApplyColor(ZoneShaderSettings.groundFogColor_shaderProp, this.groundFogColor_overrideMode, this.groundFogColor, ZoneShaderSettings.defaultsInstance.groundFogColor);
			this.ApplyFloat(ZoneShaderSettings.groundFogDepthFadeSq_shaderProp, this.groundFogDepthFade_overrideMode, this.GroundFogDepthFadeSq, ZoneShaderSettings.defaultsInstance.GroundFogDepthFadeSq);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeight_shaderProp, this.groundFogHeight_overrideMode, this.groundFogHeight, ZoneShaderSettings.defaultsInstance.groundFogHeight);
			this.ApplyFloat(ZoneShaderSettings.groundFogHeightFade_shaderProp, this.groundFogHeightFade_overrideMode, this.GroundFogHeightFade, ZoneShaderSettings.defaultsInstance.GroundFogHeightFade);
			if (this.zoneLiquidType_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.EZoneLiquidType ezoneLiquidType = (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.zoneLiquidType : ZoneShaderSettings.defaultsInstance.zoneLiquidType;
				if (ezoneLiquidType != ZoneShaderSettings.liquidType_previousValue || !ZoneShaderSettings.isInitialized)
				{
					this.SetZoneLiquidTypeKeywordEnum(ezoneLiquidType);
					ZoneShaderSettings.liquidType_previousValue = ezoneLiquidType;
				}
			}
			if (this.liquidShape_overrideMode != ZoneShaderSettings.EOverrideMode.LeaveUnchanged)
			{
				ZoneShaderSettings.ELiquidShape eliquidShape = (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.liquidShape : ZoneShaderSettings.defaultsInstance.liquidShape;
				if (eliquidShape != ZoneShaderSettings.liquidShape_previousValue || !ZoneShaderSettings.isInitialized)
				{
					this.SetZoneLiquidShapeKeywordEnum(eliquidShape);
					ZoneShaderSettings.liquidShape_previousValue = eliquidShape;
				}
			}
			this.ApplyFloat(ZoneShaderSettings.shaderParam_GlobalZoneLiquidUVScale, this.zoneLiquidUVScale_overrideMode, this.zoneLiquidUVScale, ZoneShaderSettings.defaultsInstance.zoneLiquidUVScale);
			this.ApplyColor(ZoneShaderSettings.shaderParam_GlobalWaterTintColor, this.underwaterTintColor_overrideMode, this.underwaterTintColor, ZoneShaderSettings.defaultsInstance.underwaterTintColor);
			this.ApplyColor(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, this.underwaterFogColor_overrideMode, this.underwaterFogColor, ZoneShaderSettings.defaultsInstance.underwaterFogColor);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterFogParams, this.underwaterFogParams_overrideMode, this.underwaterFogParams, ZoneShaderSettings.defaultsInstance.underwaterFogParams);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsParams, this.underwaterCausticsParams_overrideMode, this.underwaterCausticsParams, ZoneShaderSettings.defaultsInstance.underwaterCausticsParams);
			this.ApplyTexture(ZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsTex, this.underwaterCausticsTexture_overrideMode, this.underwaterCausticsTexture, ZoneShaderSettings.defaultsInstance.underwaterCausticsTexture);
			this.ApplyVector(ZoneShaderSettings.shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, this.underwaterEffectsDistanceToSurfaceFade_overrideMode, this.underwaterEffectsDistanceToSurfaceFade, ZoneShaderSettings.defaultsInstance.underwaterEffectsDistanceToSurfaceFade);
			this.ApplyTexture(ZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, this.liquidResidueTex_overrideMode, this.liquidResidueTex, ZoneShaderSettings.defaultsInstance.liquidResidueTex);
			this.ApplyFloat(ZoneShaderSettings.shaderParam_ZoneWeatherMapDissolveProgress, this.zoneWeatherMapDissolveProgress_overrideMode, this.zoneWeatherMapDissolveProgress, ZoneShaderSettings.defaultsInstance.zoneWeatherMapDissolveProgress);
			this.UpdateMainPlaneShaderProperty();
			ZoneShaderSettings.isInitialized = true;
		}

		// Token: 0x06005F31 RID: 24369 RVA: 0x001DFC69 File Offset: 0x001DDE69
		private void ApplyColor(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Color value, Color defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(shaderProp, value.linear);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderProp, defaultValue.linear);
			}
		}

		// Token: 0x06005F32 RID: 24370 RVA: 0x001DFC96 File Offset: 0x001DDE96
		private void ApplyFloat(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, float value, float defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalFloat(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalFloat(shaderProp, defaultValue);
			}
		}

		// Token: 0x06005F33 RID: 24371 RVA: 0x001DFCB8 File Offset: 0x001DDEB8
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector2 value, Vector2 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x06005F34 RID: 24372 RVA: 0x001DFCE4 File Offset: 0x001DDEE4
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector3 value, Vector3 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x06005F35 RID: 24373 RVA: 0x001DFD10 File Offset: 0x001DDF10
		private void ApplyVector(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Vector4 value, Vector4 defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		// Token: 0x06005F36 RID: 24374 RVA: 0x001DFD32 File Offset: 0x001DDF32
		private void ApplyTexture(int shaderProp, ZoneShaderSettings.EOverrideMode overrideMode, Texture2D value, Texture2D defaultValue)
		{
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalTexture(shaderProp, value);
				return;
			}
			if (overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(shaderProp, defaultValue);
			}
		}

		// Token: 0x06005F37 RID: 24375 RVA: 0x001DFD54 File Offset: 0x001DDF54
		public void CopySettings(CMSZoneShaderSettings cmsZoneShaderSettings, bool rerunAwake = false)
		{
			this._activateOnAwake = cmsZoneShaderSettings.activateOnLoad;
			if (cmsZoneShaderSettings.applyGroundFog)
			{
				this.groundFogColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogColorOverrideMode();
				this.groundFogColor = cmsZoneShaderSettings.groundFogColor;
				this.groundFogHeight_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogHeightOverrideMode();
				if (cmsZoneShaderSettings.groundFogHeightPlane.IsNotNull())
				{
					this.groundFogHeight = cmsZoneShaderSettings.groundFogHeightPlane.position.y;
				}
				else
				{
					this.groundFogHeight = cmsZoneShaderSettings.groundFogHeight;
				}
				this.groundFogHeightFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogHeightFadeOverrideMode();
				this._groundFogHeightFadeSize = cmsZoneShaderSettings.groundFogHeightFadeSize;
				this.groundFogDepthFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetGroundFogDepthFadeOverrideMode();
				this._groundFogDepthFadeSize = cmsZoneShaderSettings.groundFogDepthFadeSize;
			}
			else
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = new Color(0f, 0f, 0f, 0f);
				this.groundFogHeight = -9999f;
			}
			if (cmsZoneShaderSettings.applyLiquidEffects)
			{
				this.zoneLiquidType_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetZoneLiquidTypeOverrideMode();
				this.zoneLiquidType = (ZoneShaderSettings.EZoneLiquidType)cmsZoneShaderSettings.GetZoneLiquidType();
				this.liquidShape_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidShapeOverrideMode();
				this.liquidShape = (ZoneShaderSettings.ELiquidShape)cmsZoneShaderSettings.GetZoneLiquidShape();
				this.liquidShapeRadius_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidShapeRadiusOverrideMode();
				this.liquidShapeRadius = cmsZoneShaderSettings.liquidShapeRadius;
				this.liquidBottomTransform_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidBottomTransformOverrideMode();
				this.liquidBottomTransform = cmsZoneShaderSettings.liquidBottomTransform;
				this.zoneLiquidUVScale_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetZoneLiquidUVScaleOverrideMode();
				this.zoneLiquidUVScale = cmsZoneShaderSettings.zoneLiquidUVScale;
				this.underwaterTintColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterTintColorOverrideMode();
				this.underwaterTintColor = cmsZoneShaderSettings.underwaterTintColor;
				this.underwaterFogColor_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterFogColorOverrideMode();
				this.underwaterFogColor = cmsZoneShaderSettings.underwaterFogColor;
				this.underwaterFogParams_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterFogParamsOverrideMode();
				this.underwaterFogParams = cmsZoneShaderSettings.underwaterFogParams;
				this.underwaterCausticsParams_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterCausticsParamsOverrideMode();
				this.underwaterCausticsParams = cmsZoneShaderSettings.underwaterCausticsParams;
				this.underwaterCausticsTexture_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterCausticsTextureOverrideMode();
				this.underwaterCausticsTexture = cmsZoneShaderSettings.underwaterCausticsTexture;
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetUnderwaterEffectsDistanceToSurfaceFadeOverrideMode();
				this.underwaterEffectsDistanceToSurfaceFade = cmsZoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
				this.liquidResidueTex_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetLiquidResidueTextureOverrideMode();
				this.liquidResidueTex = cmsZoneShaderSettings.liquidResidueTex;
				this.mainWaterSurfacePlane_overrideMode = (ZoneShaderSettings.EOverrideMode)cmsZoneShaderSettings.GetMainWaterSurfacePlaneOverrideMode();
				this.mainWaterSurfacePlane = cmsZoneShaderSettings.mainWaterSurfacePlane;
			}
			else
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = new Color(0f, 0f, 0f, 0f);
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = new Color(0f, 0f, 0f, 0f);
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				Transform transform = base.gameObject.transform.Find("DummyWaterPlane");
				GameObject gameObject;
				if (transform != null)
				{
					gameObject = transform.gameObject;
				}
				else
				{
					gameObject = new GameObject("DummyWaterPlane");
					gameObject.transform.SetParent(base.gameObject.transform);
					gameObject.transform.rotation = Quaternion.identity;
					gameObject.transform.position = new Vector3(0f, -9999f, 0f);
				}
				this.mainWaterSurfacePlane = gameObject.transform;
			}
			this.zoneWeatherMapDissolveProgress_overrideMode = ZoneShaderSettings.EOverrideMode.LeaveUnchanged;
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x06005F38 RID: 24376 RVA: 0x001E0064 File Offset: 0x001DE264
		public void CopySettings(ZoneShaderSettings zoneShaderSettings, bool rerunAwake = false)
		{
			this._activateOnAwake = zoneShaderSettings._activateOnAwake;
			this.groundFogColor_overrideMode = zoneShaderSettings.groundFogColor_overrideMode;
			this.groundFogColor = zoneShaderSettings.groundFogColor;
			this.groundFogHeight_overrideMode = zoneShaderSettings.groundFogHeight_overrideMode;
			this.groundFogHeight = zoneShaderSettings.groundFogHeight;
			this.groundFogHeightFade_overrideMode = zoneShaderSettings.groundFogHeightFade_overrideMode;
			this._groundFogHeightFadeSize = zoneShaderSettings._groundFogHeightFadeSize;
			this.groundFogDepthFade_overrideMode = zoneShaderSettings.groundFogDepthFade_overrideMode;
			this._groundFogDepthFadeSize = zoneShaderSettings._groundFogDepthFadeSize;
			this.zoneLiquidType_overrideMode = zoneShaderSettings.zoneLiquidType_overrideMode;
			this.zoneLiquidType = zoneShaderSettings.zoneLiquidType;
			this.liquidShape_overrideMode = zoneShaderSettings.liquidShape_overrideMode;
			this.liquidShape = zoneShaderSettings.liquidShape;
			this.liquidShapeRadius_overrideMode = zoneShaderSettings.liquidShapeRadius_overrideMode;
			this.liquidShapeRadius = zoneShaderSettings.liquidShapeRadius;
			this.liquidBottomTransform_overrideMode = zoneShaderSettings.liquidBottomTransform_overrideMode;
			this.liquidBottomTransform = zoneShaderSettings.liquidBottomTransform;
			this.zoneLiquidUVScale_overrideMode = zoneShaderSettings.zoneLiquidUVScale_overrideMode;
			this.zoneLiquidUVScale = zoneShaderSettings.zoneLiquidUVScale;
			this.underwaterTintColor_overrideMode = zoneShaderSettings.underwaterTintColor_overrideMode;
			this.underwaterTintColor = zoneShaderSettings.underwaterTintColor;
			this.underwaterFogColor_overrideMode = zoneShaderSettings.underwaterFogColor_overrideMode;
			this.underwaterFogColor = zoneShaderSettings.underwaterFogColor;
			this.underwaterFogParams_overrideMode = zoneShaderSettings.underwaterFogParams_overrideMode;
			this.underwaterFogParams = zoneShaderSettings.underwaterFogParams;
			this.underwaterCausticsParams_overrideMode = zoneShaderSettings.underwaterCausticsParams_overrideMode;
			this.underwaterCausticsParams = zoneShaderSettings.underwaterCausticsParams;
			this.underwaterCausticsTexture_overrideMode = zoneShaderSettings.underwaterCausticsTexture_overrideMode;
			this.underwaterCausticsTexture = zoneShaderSettings.underwaterCausticsTexture;
			this.underwaterEffectsDistanceToSurfaceFade_overrideMode = zoneShaderSettings.underwaterEffectsDistanceToSurfaceFade_overrideMode;
			this.underwaterEffectsDistanceToSurfaceFade = zoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
			this.liquidResidueTex_overrideMode = zoneShaderSettings.liquidResidueTex_overrideMode;
			this.liquidResidueTex = zoneShaderSettings.liquidResidueTex;
			this.mainWaterSurfacePlane_overrideMode = zoneShaderSettings.mainWaterSurfacePlane_overrideMode;
			this.mainWaterSurfacePlane = zoneShaderSettings.mainWaterSurfacePlane;
			this.zoneWeatherMapDissolveProgress_overrideMode = zoneShaderSettings.zoneWeatherMapDissolveProgress_overrideMode;
			this.zoneWeatherMapDissolveProgress = zoneShaderSettings.zoneWeatherMapDissolveProgress;
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x06005F39 RID: 24377 RVA: 0x001E0238 File Offset: 0x001DE438
		public void ReplaceDefaultValues(ZoneShaderSettings defaultZoneShaderSettings, bool rerunAwake = false)
		{
			if (this.groundFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = defaultZoneShaderSettings.groundFogColor;
			}
			if (this.groundFogHeight_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogHeight = defaultZoneShaderSettings.groundFogHeight;
			}
			if (this.groundFogHeightFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogHeightFadeSize = defaultZoneShaderSettings._groundFogHeightFadeSize;
			}
			if (this.groundFogDepthFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogDepthFadeSize = defaultZoneShaderSettings._groundFogDepthFadeSize;
			}
			if (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidType_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidType = defaultZoneShaderSettings.zoneLiquidType;
			}
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShape_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShape = defaultZoneShaderSettings.liquidShape;
			}
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShapeRadius_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShapeRadius = defaultZoneShaderSettings.liquidShapeRadius;
			}
			if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidBottomTransform_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidBottomTransform = defaultZoneShaderSettings.liquidBottomTransform;
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidUVScale_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidUVScale = defaultZoneShaderSettings.zoneLiquidUVScale;
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = defaultZoneShaderSettings.underwaterTintColor;
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = defaultZoneShaderSettings.underwaterFogColor;
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogParams = defaultZoneShaderSettings.underwaterFogParams;
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsParams = defaultZoneShaderSettings.underwaterCausticsParams;
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsTexture_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsTexture = defaultZoneShaderSettings.underwaterCausticsTexture;
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterEffectsDistanceToSurfaceFade = defaultZoneShaderSettings.underwaterEffectsDistanceToSurfaceFade;
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidResidueTex_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidResidueTex = defaultZoneShaderSettings.liquidResidueTex;
			}
			if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.mainWaterSurfacePlane = defaultZoneShaderSettings.mainWaterSurfacePlane;
			}
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x001E042C File Offset: 0x001DE62C
		public void ReplaceDefaultValues(CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties, bool rerunAwake = false)
		{
			if (this.groundFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogColor = defaultZoneShaderProperties.groundFogColor;
			}
			if (this.groundFogHeight_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeight_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.groundFogHeight = defaultZoneShaderProperties.groundFogHeight;
			}
			if (this.groundFogHeightFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogHeightFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogHeightFadeSize = defaultZoneShaderProperties.groundFogHeightFadeSize;
			}
			if (this.groundFogDepthFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.groundFogDepthFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this._groundFogDepthFadeSize = defaultZoneShaderProperties.groundFogDepthFadeSize;
			}
			if (this.zoneLiquidType_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidType_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidType = (ZoneShaderSettings.EZoneLiquidType)defaultZoneShaderProperties.zoneLiquidType;
			}
			if (this.liquidShape_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShape_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShape = (ZoneShaderSettings.ELiquidShape)defaultZoneShaderProperties.liquidShape;
			}
			if (this.liquidShapeRadius_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidShapeRadius_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidShapeRadius = defaultZoneShaderProperties.liquidShapeRadius;
			}
			if (this.liquidBottomTransform_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidBottomTransform_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidBottomTransform = defaultZoneShaderProperties.liquidBottomTransform;
			}
			if (this.zoneLiquidUVScale_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.zoneLiquidUVScale_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.zoneLiquidUVScale = defaultZoneShaderProperties.zoneLiquidUVScale;
			}
			if (this.underwaterTintColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterTintColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterTintColor = defaultZoneShaderProperties.underwaterTintColor;
			}
			if (this.underwaterFogColor_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogColor_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogColor = defaultZoneShaderProperties.underwaterFogColor;
			}
			if (this.underwaterFogParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterFogParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterFogParams = defaultZoneShaderProperties.underwaterFogParams;
			}
			if (this.underwaterCausticsParams_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsParams_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsParams = defaultZoneShaderProperties.underwaterCausticsParams;
			}
			if (this.underwaterCausticsTexture_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterCausticsTexture_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterCausticsTexture = defaultZoneShaderProperties.underwaterCausticsTexture;
			}
			if (this.underwaterEffectsDistanceToSurfaceFade_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.underwaterEffectsDistanceToSurfaceFade_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.underwaterEffectsDistanceToSurfaceFade = defaultZoneShaderProperties.underwaterEffectsDistanceToSurfaceFade;
			}
			if (this.liquidResidueTex_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.liquidResidueTex_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.liquidResidueTex = defaultZoneShaderProperties.liquidResidueTex;
			}
			if (this.mainWaterSurfacePlane_overrideMode == ZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				this.mainWaterSurfacePlane_overrideMode = ZoneShaderSettings.EOverrideMode.ApplyNewValue;
				this.mainWaterSurfacePlane = defaultZoneShaderProperties.mainWaterSurfacePlane;
			}
			if (rerunAwake)
			{
				this.Awake();
			}
		}

		// Token: 0x04006970 RID: 26992
		[OnEnterPlay_Set(false)]
		private static bool isInitialized;

		// Token: 0x04006975 RID: 26997
		[Tooltip("Set this to true for cases like it is the first ZoneShaderSettings that should be activated when entering a scene.")]
		[SerializeField]
		private bool _activateOnAwake;

		// Token: 0x04006976 RID: 26998
		[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
		public bool isDefaultValues;

		// Token: 0x04006977 RID: 26999
		private static readonly int groundFogColor_shaderProp = Shader.PropertyToID("_ZoneGroundFogColor");

		// Token: 0x04006978 RID: 27000
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogColor_overrideMode;

		// Token: 0x04006979 RID: 27001
		[SerializeField]
		private Color groundFogColor = new Color(0.7f, 0.9f, 1f, 1f);

		// Token: 0x0400697A RID: 27002
		private static readonly int groundFogDepthFadeSq_shaderProp = Shader.PropertyToID("_ZoneGroundFogDepthFadeSq");

		// Token: 0x0400697B RID: 27003
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogDepthFade_overrideMode;

		// Token: 0x0400697C RID: 27004
		[SerializeField]
		private float _groundFogDepthFadeSize = 20f;

		// Token: 0x0400697D RID: 27005
		private static readonly int groundFogHeight_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeight");

		// Token: 0x0400697E RID: 27006
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeight_overrideMode;

		// Token: 0x0400697F RID: 27007
		[SerializeField]
		private float groundFogHeight = 7.45f;

		// Token: 0x04006980 RID: 27008
		private static readonly int groundFogHeightFade_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeightFade");

		// Token: 0x04006981 RID: 27009
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode groundFogHeightFade_overrideMode;

		// Token: 0x04006982 RID: 27010
		[SerializeField]
		private float _groundFogHeightFadeSize = 20f;

		// Token: 0x04006983 RID: 27011
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidType_overrideMode;

		// Token: 0x04006984 RID: 27012
		[SerializeField]
		private ZoneShaderSettings.EZoneLiquidType zoneLiquidType = ZoneShaderSettings.EZoneLiquidType.Water;

		// Token: 0x04006985 RID: 27013
		[OnEnterPlay_Set(ZoneShaderSettings.EZoneLiquidType.None)]
		private static ZoneShaderSettings.EZoneLiquidType liquidType_previousValue = ZoneShaderSettings.EZoneLiquidType.None;

		// Token: 0x04006986 RID: 27014
		[OnEnterPlay_Set(false)]
		private static bool didEverSetLiquidShape;

		// Token: 0x04006987 RID: 27015
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidShape_overrideMode;

		// Token: 0x04006988 RID: 27016
		[SerializeField]
		private ZoneShaderSettings.ELiquidShape liquidShape;

		// Token: 0x04006989 RID: 27017
		[OnEnterPlay_Set(ZoneShaderSettings.ELiquidShape.Plane)]
		private static ZoneShaderSettings.ELiquidShape liquidShape_previousValue = ZoneShaderSettings.ELiquidShape.Plane;

		// Token: 0x0400698B RID: 27019
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidShapeRadius_overrideMode;

		// Token: 0x0400698C RID: 27020
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float liquidShapeRadius = 1f;

		// Token: 0x0400698D RID: 27021
		[OnEnterPlay_Set(1f)]
		private static float liquidShapeRadius_previousValue;

		// Token: 0x0400698E RID: 27022
		private bool hasLiquidBottomTransform;

		// Token: 0x0400698F RID: 27023
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode liquidBottomTransform_overrideMode;

		// Token: 0x04006990 RID: 27024
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform liquidBottomTransform;

		// Token: 0x04006991 RID: 27025
		private float liquidBottomPosY_previousValue;

		// Token: 0x04006992 RID: 27026
		private static readonly int shaderParam_GlobalZoneLiquidUVScale = Shader.PropertyToID("_GlobalZoneLiquidUVScale");

		// Token: 0x04006993 RID: 27027
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneLiquidUVScale_overrideMode;

		// Token: 0x04006994 RID: 27028
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private float zoneLiquidUVScale = 1f;

		// Token: 0x04006995 RID: 27029
		private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

		// Token: 0x04006996 RID: 27030
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterTintColor_overrideMode;

		// Token: 0x04006997 RID: 27031
		[SerializeField]
		private Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

		// Token: 0x04006998 RID: 27032
		private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

		// Token: 0x04006999 RID: 27033
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogColor_overrideMode;

		// Token: 0x0400699A RID: 27034
		[SerializeField]
		private Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

		// Token: 0x0400699B RID: 27035
		private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

		// Token: 0x0400699C RID: 27036
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterFogParams_overrideMode;

		// Token: 0x0400699D RID: 27037
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[SerializeField]
		private Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

		// Token: 0x0400699E RID: 27038
		private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

		// Token: 0x0400699F RID: 27039
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsParams_overrideMode;

		// Token: 0x040069A0 RID: 27040
		[Tooltip("Caustics params are: speed1, scale, alpha, unused")]
		[SerializeField]
		private Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

		// Token: 0x040069A1 RID: 27041
		private static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

		// Token: 0x040069A2 RID: 27042
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterCausticsTexture_overrideMode;

		// Token: 0x040069A3 RID: 27043
		[SerializeField]
		private Texture2D underwaterCausticsTexture;

		// Token: 0x040069A4 RID: 27044
		private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

		// Token: 0x040069A5 RID: 27045
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

		// Token: 0x040069A6 RID: 27046
		[SerializeField]
		private Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

		// Token: 0x040069A7 RID: 27047
		private const string kEdTooltip_liquidResidueTex = "This is used for things like the charred surface effect when lava burns static geo.";

		// Token: 0x040069A8 RID: 27048
		private static readonly int shaderParam_GlobalLiquidResidueTex = Shader.PropertyToID("_GlobalLiquidResidueTex");

		// Token: 0x040069A9 RID: 27049
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private ZoneShaderSettings.EOverrideMode liquidResidueTex_overrideMode;

		// Token: 0x040069AA RID: 27050
		[SerializeField]
		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		private Texture2D liquidResidueTex;

		// Token: 0x040069AB RID: 27051
		private readonly int shaderParam_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

		// Token: 0x040069AC RID: 27052
		private bool hasMainWaterSurfacePlane;

		// Token: 0x040069AD RID: 27053
		private bool hasDynamicWaterSurfacePlane;

		// Token: 0x040069AE RID: 27054
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode mainWaterSurfacePlane_overrideMode;

		// Token: 0x040069AF RID: 27055
		[Tooltip("TODO: remove this when there is a way to precalculate the nearest triangle plane per vertex so it will work better for rivers.")]
		[SerializeField]
		private Transform mainWaterSurfacePlane;

		// Token: 0x040069B0 RID: 27056
		private static readonly int shaderParam_ZoneWeatherMapDissolveProgress = Shader.PropertyToID("_ZoneWeatherMapDissolveProgress");

		// Token: 0x040069B1 RID: 27057
		[SerializeField]
		private ZoneShaderSettings.EOverrideMode zoneWeatherMapDissolveProgress_overrideMode;

		// Token: 0x040069B2 RID: 27058
		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		[Range(0f, 1f)]
		[SerializeField]
		private float zoneWeatherMapDissolveProgress = 1f;

		// Token: 0x02000EFC RID: 3836
		public enum EOverrideMode
		{
			// Token: 0x040069B5 RID: 27061
			LeaveUnchanged,
			// Token: 0x040069B6 RID: 27062
			ApplyNewValue,
			// Token: 0x040069B7 RID: 27063
			ApplyDefaultValue
		}

		// Token: 0x02000EFD RID: 3837
		public enum EZoneLiquidType
		{
			// Token: 0x040069B9 RID: 27065
			None,
			// Token: 0x040069BA RID: 27066
			Water,
			// Token: 0x040069BB RID: 27067
			Lava
		}

		// Token: 0x02000EFE RID: 3838
		public enum ELiquidShape
		{
			// Token: 0x040069BD RID: 27069
			Plane,
			// Token: 0x040069BE RID: 27070
			Cylinder
		}
	}
}
