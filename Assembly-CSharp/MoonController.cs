using System;
using System.Collections.Generic;
using CjLib;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class MoonController : MonoBehaviour
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000709 RID: 1801 RVA: 0x0002824A File Offset: 0x0002644A
	public float Distance
	{
		get
		{
			return this.distance;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x0600070A RID: 1802 RVA: 0x00028252 File Offset: 0x00026452
	private float TimeOfDay
	{
		get
		{
			if (this.debugOverrideTimeOfDay)
			{
				return Mathf.Repeat(this.timeOfDayOverride, 1f);
			}
			if (!(BetterDayNightManager.instance != null))
			{
				return 1f;
			}
			return BetterDayNightManager.instance.NormalizedTimeOfDay;
		}
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0002828E File Offset: 0x0002648E
	public void SetEyeOpenAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x000282A2 File Offset: 0x000264A2
	public void StartEyeCloseAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x000282B8 File Offset: 0x000264B8
	private void Start()
	{
		this.eyeOpenHash = Animator.StringToHash("EyeOpen");
		this.zoneToSceneMapping.Add(GTZone.forest, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.city, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.basement, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.canyon, MoonController.Scenes.Canyon);
		this.zoneToSceneMapping.Add(GTZone.beach, MoonController.Scenes.Beach);
		this.zoneToSceneMapping.Add(GTZone.mountain, MoonController.Scenes.Mountain);
		this.zoneToSceneMapping.Add(GTZone.skyJungle, MoonController.Scenes.Clouds);
		this.zoneToSceneMapping.Add(GTZone.cave, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.cityWithSkyJungle, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.tutorial, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.rotating, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.none, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.Metropolis, MoonController.Scenes.Metropolis);
		this.zoneToSceneMapping.Add(GTZone.cityNoBuildings, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.attic, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.arcade, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.bayou, MoonController.Scenes.Bayou);
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.RegisterMoon(this);
		}
		this.crackStartDayOfYear = new DateTime(2024, 10, 4).DayOfYear;
		this.crackEndDayOfYear = new DateTime(2024, 10, 25).DayOfYear;
		if (this.crackRenderer != null)
		{
			this.currentlySetCrackProgress = 1f;
			this.crackMaterialPropertyBlock = new MaterialPropertyBlock();
			this.crackRenderer.GetPropertyBlock(this.crackMaterialPropertyBlock);
			this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
			this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
		}
		this.orbitAngle = 0f;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x000284B3 File Offset: 0x000266B3
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.UnregisterMoon(this);
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x000284D4 File Offset: 0x000266D4
	private void OnZoneChanged()
	{
		ZoneManagement instance = ZoneManagement.instance;
		MoonController.Scenes scenes = MoonController.Scenes.Forest;
		for (int i = 0; i < instance.activeZones.Count; i++)
		{
			MoonController.Scenes scenes2;
			if (this.zoneToSceneMapping.TryGetValue(instance.activeZones[i], out scenes2) && scenes2 > scenes)
			{
				scenes = scenes2;
			}
		}
		this.UpdateActiveScene(scenes);
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00028527 File Offset: 0x00026727
	private void UpdateActiveScene(MoonController.Scenes nextScene)
	{
		this.activeScene = nextScene;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0002853C File Offset: 0x0002673C
	private void Update()
	{
		this.UpdateCrack();
		if (!this.alwaysInTheSky)
		{
			float timeOfDay = this.TimeOfDay;
			bool flag = timeOfDay > 0.53999996f && timeOfDay < 0.6733333f;
			bool flag2 = timeOfDay > 0.086666666f && timeOfDay < 0.22f;
			bool flag3 = timeOfDay <= 0.086666666f || timeOfDay >= 0.6733333f;
			if (timeOfDay >= 0.22f)
			{
				bool flag4 = timeOfDay <= 0.53999996f;
			}
			float num = this.orbitAngle;
			if (flag)
			{
				num = Mathf.Lerp(3.1415927f, 0f, (timeOfDay - 0.53999996f) / 0.13333333f);
			}
			else if (flag2)
			{
				num = Mathf.Lerp(0f, -3.1415927f, (timeOfDay - 0.086666666f) / 0.13333333f);
			}
			else if (flag3)
			{
				num = 0f;
			}
			else
			{
				num = 3.1415927f;
			}
			if (this.orbitAngle != num)
			{
				this.orbitAngle = num;
				this.UpdateCrack();
				this.UpdatePlacement();
			}
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x0002862D File Offset: 0x0002682D
	public void UpdateDistance(float nextDistance)
	{
		this.distance = nextDistance;
		this.UpdateVisualState();
		this.UpdatePlacement();
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00028644 File Offset: 0x00026844
	public void UpdateVisualState()
	{
		bool flag = false;
		if (GreyZoneManager.Instance != null)
		{
			flag = GreyZoneManager.Instance.GreyZoneActive;
		}
		if (flag && this.openEyeModelEnabled && this.distance < this.eyeOpenDistThreshold && !this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
			return;
		}
		if (!flag && this.distance > this.eyeCloseDistThreshold && this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x000286E4 File Offset: 0x000268E4
	public void UpdatePlacement()
	{
		if (this.alwaysInTheSky)
		{
			this.UpdatePlacementSimple();
			return;
		}
		this.UpdatePlacementOrbit();
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000286FC File Offset: 0x000268FC
	private void UpdatePlacementSimple()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement;
		float num = Mathf.Lerp(placement.heightRange.x, placement.heightRange.y, this.distance);
		float num2 = Mathf.Lerp(placement.radiusRange.x, placement.radiusRange.y, this.distance);
		float d = Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		float restAngle = placement.restAngle;
		Vector3 position = referencePoint.position;
		position.y += num;
		position.x += num2 * Mathf.Cos(restAngle * 0.017453292f);
		position.z += num2 * Mathf.Sin(restAngle * 0.017453292f);
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * d;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00028840 File Offset: 0x00026A40
	public void UpdatePlacementOrbit()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement;
		float y = placement.heightRange.y;
		float y2 = placement.radiusRange.y;
		Vector3 position = referencePoint.position;
		position.y += y;
		position.x += y2 * Mathf.Cos(placement.restAngle * 0.017453292f);
		position.z += y2 * Mathf.Sin(placement.restAngle * 0.017453292f);
		float d = Mathf.Sqrt(y * y + y2 * y2);
		float num = Mathf.Atan2(y, y2);
		Quaternion rotation = Quaternion.AngleAxis(57.29578f * num, Vector3.Cross(position - referencePoint.position, Vector3.up));
		float f = placement.restAngle * 0.017453292f + this.orbitAngle;
		Vector3 vector = referencePoint.position + rotation * new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * d;
		if (this.distance < 1f)
		{
			Vector3 position2 = referencePoint.position;
			position2.y += placement.heightRange.x;
			position2.x += placement.radiusRange.x * Mathf.Cos(placement.restAngle * 0.017453292f);
			position2.z += placement.radiusRange.x * Mathf.Sin(placement.restAngle * 0.017453292f);
			if (Mathf.Abs(this.orbitAngle) < 0.9424779f)
			{
				vector = Vector3.Lerp(position2, vector, this.distance);
			}
			else
			{
				vector = Vector3.Lerp(position2, position, this.distance);
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		if (this.debugDrawOrbit)
		{
			int num2 = 32;
			float timeOfDay = this.TimeOfDay;
			float num3 = 0.086666666f;
			float num4 = 0.24666667f;
			float num5 = 0.6333333f;
			float num6 = 0.76f;
			bool flag = timeOfDay > num3 && timeOfDay < num4;
			bool flag2 = timeOfDay > num5 && timeOfDay < num6;
			bool flag3 = timeOfDay <= num3 || timeOfDay >= num6;
			if (timeOfDay >= num4)
			{
				bool flag4 = timeOfDay <= num5;
			}
			Color color = flag2 ? Color.red : (flag3 ? Color.green : (flag ? Color.yellow : Color.blue));
			Vector3 v = referencePoint.position + rotation * new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * d;
			for (int i = 1; i <= num2; i++)
			{
				float num7 = (float)i / (float)num2;
				Vector3 vector2 = referencePoint.position + rotation * new Vector3(Mathf.Cos(6.2831855f * num7), 0f, Mathf.Sin(6.2831855f * num7)) * d;
				DebugUtil.DrawLine(v, vector2, color, false);
				v = vector2;
			}
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00028BD0 File Offset: 0x00026DD0
	private void UpdateCrack()
	{
		bool flag = GreyZoneManager.Instance != null && GreyZoneManager.Instance.GreyZoneAvailable;
		if (flag && !this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = true;
			this.defaultMoon.gameObject.SetActive(false);
			this.openMoon.gameObject.SetActive(true);
		}
		else if (!flag && this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = false;
			this.defaultMoon.gameObject.SetActive(true);
			this.openMoon.gameObject.SetActive(false);
		}
		if (!flag && GorillaComputer.instance != null)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (this.debugOverrideCrackDayInOctober)
			{
				serverTime = new DateTime(2024, 10, Mathf.Clamp(this.crackDayInOctoberOverride, 1, 31));
			}
			float value = Mathf.InverseLerp((float)this.crackStartDayOfYear, (float)this.crackEndDayOfYear, (float)serverTime.DayOfYear);
			if (this.debugOverrideCrackProgress)
			{
				value = this.crackProgress;
			}
			float num = 1f - Mathf.Clamp01(value);
			if (this.crackRenderer != null && Mathf.Abs(num - this.currentlySetCrackProgress) > Mathf.Epsilon)
			{
				this.currentlySetCrackProgress = num;
				this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
				this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
			}
		}
	}

	// Token: 0x0400087C RID: 2172
	[SerializeField]
	private List<MoonController.SceneData> scenes = new List<MoonController.SceneData>();

	// Token: 0x0400087D RID: 2173
	[SerializeField]
	private MoonController.Scenes activeScene;

	// Token: 0x0400087E RID: 2174
	[SerializeField]
	private MoonController.Placement defaultPlacement;

	// Token: 0x0400087F RID: 2175
	[SerializeField]
	[Range(0f, 1f)]
	private float distance;

	// Token: 0x04000880 RID: 2176
	[SerializeField]
	private bool alwaysInTheSky;

	// Token: 0x04000881 RID: 2177
	[Header("Model Swap")]
	[SerializeField]
	private Transform defaultMoon;

	// Token: 0x04000882 RID: 2178
	[SerializeField]
	private Transform openMoon;

	// Token: 0x04000883 RID: 2179
	[Header("Animation")]
	[SerializeField]
	private Animator openMoonAnimator;

	// Token: 0x04000884 RID: 2180
	[SerializeField]
	private float eyeOpenDistThreshold = 0.9f;

	// Token: 0x04000885 RID: 2181
	[SerializeField]
	private float eyeCloseDistThreshold = 0.05f;

	// Token: 0x04000886 RID: 2182
	[Header("Debug")]
	[SerializeField]
	private bool debugOverrideTimeOfDay;

	// Token: 0x04000887 RID: 2183
	[SerializeField]
	[Range(0f, 4f)]
	private float timeOfDayOverride;

	// Token: 0x04000888 RID: 2184
	[SerializeField]
	private bool debugOverrideCrackProgress;

	// Token: 0x04000889 RID: 2185
	[SerializeField]
	[Range(0f, 1f)]
	private float crackProgress;

	// Token: 0x0400088A RID: 2186
	[SerializeField]
	private bool debugOverrideCrackDayInOctober;

	// Token: 0x0400088B RID: 2187
	[SerializeField]
	[Range(1f, 31f)]
	private int crackDayInOctoberOverride = 4;

	// Token: 0x0400088C RID: 2188
	[SerializeField]
	private MeshRenderer crackRenderer;

	// Token: 0x0400088D RID: 2189
	private int crackStartDayOfYear;

	// Token: 0x0400088E RID: 2190
	private int crackEndDayOfYear;

	// Token: 0x0400088F RID: 2191
	private float orbitAngle;

	// Token: 0x04000890 RID: 2192
	private int eyeOpenHash;

	// Token: 0x04000891 RID: 2193
	private bool openEyeModelEnabled;

	// Token: 0x04000892 RID: 2194
	private float currentlySetCrackProgress;

	// Token: 0x04000893 RID: 2195
	private MaterialPropertyBlock crackMaterialPropertyBlock;

	// Token: 0x04000894 RID: 2196
	private bool debugDrawOrbit;

	// Token: 0x04000895 RID: 2197
	private Dictionary<GTZone, MoonController.Scenes> zoneToSceneMapping = new Dictionary<GTZone, MoonController.Scenes>();

	// Token: 0x04000896 RID: 2198
	private const float moonFallStart = 0.086666666f;

	// Token: 0x04000897 RID: 2199
	private const float moonFallEnd = 0.22f;

	// Token: 0x04000898 RID: 2200
	private const float moonRiseStart = 0.53999996f;

	// Token: 0x04000899 RID: 2201
	private const float moonRiseEnd = 0.6733333f;

	// Token: 0x02000116 RID: 278
	public enum Scenes
	{
		// Token: 0x0400089B RID: 2203
		Forest,
		// Token: 0x0400089C RID: 2204
		Bayou,
		// Token: 0x0400089D RID: 2205
		Beach,
		// Token: 0x0400089E RID: 2206
		Canyon,
		// Token: 0x0400089F RID: 2207
		Clouds,
		// Token: 0x040008A0 RID: 2208
		City,
		// Token: 0x040008A1 RID: 2209
		Metropolis,
		// Token: 0x040008A2 RID: 2210
		Mountain
	}

	// Token: 0x02000117 RID: 279
	[Serializable]
	public struct SceneData
	{
		// Token: 0x040008A3 RID: 2211
		public MoonController.Scenes scene;

		// Token: 0x040008A4 RID: 2212
		public Transform referencePoint;

		// Token: 0x040008A5 RID: 2213
		public bool overridePlacement;

		// Token: 0x040008A6 RID: 2214
		public MoonController.Placement PlacementOverride;
	}

	// Token: 0x02000118 RID: 280
	[Serializable]
	public struct Placement
	{
		// Token: 0x040008A7 RID: 2215
		public Vector2 radiusRange;

		// Token: 0x040008A8 RID: 2216
		public Vector2 heightRange;

		// Token: 0x040008A9 RID: 2217
		public Vector2 scaleRange;

		// Token: 0x040008AA RID: 2218
		public float restAngle;
	}
}
