using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020007CB RID: 1995
public class BetterDayNightManager : MonoBehaviour, IGorillaSliceableSimple, ITimeOfDaySystem
{
	// Token: 0x060031FD RID: 12797 RVA: 0x00104658 File Offset: 0x00102858
	public static void Register(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Add(data);
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x00104665 File Offset: 0x00102865
	public static void Unregister(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Remove(data);
	}

	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x060031FF RID: 12799 RVA: 0x00104673 File Offset: 0x00102873
	// (set) Token: 0x06003200 RID: 12800 RVA: 0x0010467B File Offset: 0x0010287B
	public string currentTimeOfDay { get; private set; }

	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x06003201 RID: 12801 RVA: 0x00104684 File Offset: 0x00102884
	public float NormalizedTimeOfDay
	{
		get
		{
			return Mathf.Clamp01((float)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds / this.totalSeconds));
		}
	}

	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x06003202 RID: 12802 RVA: 0x001046AE File Offset: 0x001028AE
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x06003203 RID: 12803 RVA: 0x001046B6 File Offset: 0x001028B6
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06003204 RID: 12804 RVA: 0x001046C0 File Offset: 0x001028C0
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (BetterDayNightManager.instance == null)
		{
			BetterDayNightManager.instance = this;
		}
		else if (BetterDayNightManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.currentLerp = 0f;
		this.totalHours = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.totalHours += this.timeOfDayRange[i];
		}
		this.totalSeconds = this.totalHours * 60.0 * 60.0;
		this.currentTimeIndex = 0;
		this.baseSeconds = 0.0;
		this.computerInit = false;
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateWeatherEventTimes();
		this.ChangeMaps(0, 1);
		base.StartCoroutine(this.InitialUpdate());
	}

	// Token: 0x06003205 RID: 12805 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003206 RID: 12806 RVA: 0x000023F5 File Offset: 0x000005F5
	protected void OnDestroy()
	{
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x001047B4 File Offset: 0x001029B4
	private Vector4 MaterialColorCorrection(Vector4 color)
	{
		if (color.x < 0.5f)
		{
			color.x += 3E-08f;
		}
		if (color.y < 0.5f)
		{
			color.y += 3E-08f;
		}
		if (color.z < 0.5f)
		{
			color.z += 3E-08f;
		}
		if (color.w < 0.5f)
		{
			color.w += 3E-08f;
		}
		return color;
	}

	// Token: 0x06003208 RID: 12808 RVA: 0x00104838 File Offset: 0x00102A38
	public void UpdateTimeOfDay()
	{
		if (Time.time < this.lastTimeChecked + this.currentTimestep)
		{
			return;
		}
		this.lastTimeChecked = Time.time;
		if (this.animatingLightFlash != null)
		{
			return;
		}
		try
		{
			if (!this.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.computerInit = true;
				this.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = (this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds;
				this.currentIndexSeconds = 0.0;
				for (int i = 0; i < this.timeOfDayRange.Length; i++)
				{
					this.currentIndexSeconds += this.timeOfDayRange[i] * 3600.0;
					if (this.currentIndexSeconds > this.currentTime)
					{
						this.currentTimeIndex = i;
						break;
					}
				}
				this.currentWeatherIndex += this.currentTimeIndex;
			}
			else if (!this.computerInit && this.baseSeconds == 0.0)
			{
				this.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = this.baseSeconds % this.totalSeconds;
				this.currentIndexSeconds = 0.0;
				for (int j = 0; j < this.timeOfDayRange.Length; j++)
				{
					this.currentIndexSeconds += this.timeOfDayRange[j] * 3600.0;
					if (this.currentIndexSeconds > this.currentTime)
					{
						this.currentTimeIndex = j;
						break;
					}
				}
				this.currentWeatherIndex += this.currentTimeIndex - 1;
				if (this.currentWeatherIndex < 0)
				{
					this.currentWeatherIndex = this.weatherCycle.Length - 1;
				}
			}
			this.currentTime = ((this.currentSetting == TimeSettings.Normal) ? ((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds) : this.currentTime);
			this.currentIndexSeconds = 0.0;
			for (int k = 0; k < this.timeOfDayRange.Length; k++)
			{
				this.currentIndexSeconds += this.timeOfDayRange[k] * 3600.0;
				if (this.currentIndexSeconds > this.currentTime)
				{
					this.currentTimeIndex = k;
					break;
				}
			}
			if (this.timeIndexOverrideFunc != null)
			{
				this.currentTimeIndex = this.timeIndexOverrideFunc(this.currentTimeIndex);
			}
			if (this.currentTimeIndex != this.lastIndex)
			{
				this.currentWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
				this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
			}
			this.currentLerp = (float)(1.0 - (this.currentIndexSeconds - this.currentTime) / (this.timeOfDayRange[this.currentTimeIndex] * 3600.0));
			this.ChangeLerps(this.currentLerp);
			this.lastIndex = this.currentTimeIndex;
			this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		}
		catch (Exception ex)
		{
			string str = "Error in BetterDayNightManager: ";
			Exception ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null), this);
		}
		this.gameEpochDay = (long)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) / this.totalSeconds + (double)this.initialDayCycles);
		foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
		{
			if (scheduledEvent.lastDayCalled != this.gameEpochDay && scheduledEvent.hour == this.currentTimeIndex)
			{
				scheduledEvent.lastDayCalled = this.gameEpochDay;
				scheduledEvent.action();
			}
		}
	}

	// Token: 0x06003209 RID: 12809 RVA: 0x00104D28 File Offset: 0x00102F28
	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat(this._GlobalDayNightLerpValue, newLerp);
		for (int i = 0; i < this.standardMaterialsUnlit.Length; i++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFrom, this.colorTo, newLerp);
			this.standardMaterialsUnlit[i].color = new Color(this.tempLerp, this.tempLerp, this.tempLerp);
		}
		for (int j = 0; j < this.standardMaterialsUnlitDarker.Length; j++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFromDarker, this.colorToDarker, newLerp);
			Color.RGBToHSV(this.standardMaterialsUnlitDarker[j].color, out this.h, out this.s, out this.v);
			this.standardMaterialsUnlitDarker[j].color = Color.HSVToRGB(this.h, this.s, this.tempLerp);
		}
	}

	// Token: 0x0600320A RID: 12810 RVA: 0x00104E08 File Offset: 0x00103008
	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		this.PopulateAllLightmaps(fromIndex, toIndex);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex1, this.fromSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex2, this.toSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex1, this.fromSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex2, this.toSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex1, this.fromSky3);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex2, this.toSky3);
		this.colorFrom = this.standardUnlitColor[fromIndex];
		this.colorTo = this.standardUnlitColor[toIndex];
		this.colorFromDarker = this.standardUnlitColorWithPremadeColorDarker[fromIndex];
		this.colorToDarker = this.standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	// Token: 0x0600320B RID: 12811 RVA: 0x00104F90 File Offset: 0x00103190
	public void SliceUpdate()
	{
		if (!this.shouldRepopulate)
		{
			using (List<PerSceneRenderData>.Enumerator enumerator = BetterDayNightManager.allScenesRenderData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CheckShouldRepopulate())
					{
						this.shouldRepopulate = true;
					}
				}
			}
		}
		if (this.shouldRepopulate)
		{
			this.PopulateAllLightmaps();
			this.shouldRepopulate = false;
		}
		this.UpdateTimeOfDay();
	}

	// Token: 0x0600320C RID: 12812 RVA: 0x0010500C File Offset: 0x0010320C
	private IEnumerator InitialUpdate()
	{
		yield return null;
		this.SliceUpdate();
		yield break;
	}

	// Token: 0x0600320D RID: 12813 RVA: 0x0010501B File Offset: 0x0010321B
	public void RequestRepopulateLightmaps()
	{
		this.shouldRepopulate = true;
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x00105024 File Offset: 0x00103224
	public void PopulateAllLightmaps()
	{
		this.PopulateAllLightmaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x0600320F RID: 12815 RVA: 0x00105044 File Offset: 0x00103244
	public void PopulateAllLightmaps(int fromIndex, int toIndex)
	{
		string fromTimeOfDay;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			fromTimeOfDay = this.dayNightWeatherLightmapNames[fromIndex];
		}
		else
		{
			fromTimeOfDay = this.dayNightLightmapNames[fromIndex];
		}
		string toTimeOfDay;
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			toTimeOfDay = this.dayNightWeatherLightmapNames[toIndex];
		}
		else
		{
			toTimeOfDay = this.dayNightLightmapNames[toIndex];
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		foreach (PerSceneRenderData perSceneRenderData in BetterDayNightManager.allScenesRenderData)
		{
			perSceneRenderData.PopulateLightmaps(fromTimeOfDay, toTimeOfDay, lightmaps);
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x001050EC File Offset: 0x001032EC
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[this.currentWeatherIndex];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x06003211 RID: 12817 RVA: 0x0010510A File Offset: 0x0010330A
	public BetterDayNightManager.WeatherType NextWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x00105133 File Offset: 0x00103333
	public BetterDayNightManager.WeatherType LastWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x0010515C File Offset: 0x0010335C
	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmapNames.Length];
		this.rainChance = this.rainChance * 2f / (float)this.maxRainDuration;
		for (int i = 1; i < this.weatherCycle.Length; i++)
		{
			this.weatherCycle[i] = (((float)this.randomNumberGenerator.Next(100) < this.rainChance * 100f) ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None);
			if (this.weatherCycle[i] == BetterDayNightManager.WeatherType.Raining)
			{
				this.rainDuration = this.randomNumberGenerator.Next(1, this.maxRainDuration + 1);
				for (int j = 1; j < this.rainDuration; j++)
				{
					if (i + j < this.weatherCycle.Length)
					{
						this.weatherCycle[i + j] = BetterDayNightManager.WeatherType.Raining;
					}
				}
				i += this.rainDuration - 1;
			}
		}
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x00105234 File Offset: 0x00103434
	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int num = (int)(DateTime.Now.Ticks % 2147483647L);
		while (BetterDayNightManager.scheduledEvents.ContainsKey(num))
		{
			num++;
		}
		BetterDayNightManager.scheduledEvents.Add(num, new BetterDayNightManager.ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return num;
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x00105291 File Offset: 0x00103491
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x0010529F File Offset: 0x0010349F
	public void SetTimeIndexOverrideFunction(Func<int, int> overrideFunction)
	{
		this.timeIndexOverrideFunc = overrideFunction;
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x001052A8 File Offset: 0x001034A8
	public void UnsetTimeIndexOverrideFunction()
	{
		this.timeIndexOverrideFunc = null;
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x001052B4 File Offset: 0x001034B4
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x00105310 File Offset: 0x00103510
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x0010533D File Offset: 0x0010353D
	private IEnumerator AnimateLightFlashCo(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		int startMap = (this.currentLerp < 0.5f) ? this.currentTimeIndex : ((this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeMaps(startMap, index);
		float endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(index, index);
		this.ChangeLerps(0f);
		endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			yield return null;
		}
		this.ChangeMaps(index, startMap);
		endTimestamp = Time.time + fadeOutDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeLerps(this.currentLerp);
		this.animatingLightFlash = null;
		yield break;
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x00105364 File Offset: 0x00103564
	public void SetTimeOfDay(int timeIndex)
	{
		double num = 0.0;
		for (int i = 0; i < timeIndex; i++)
		{
			num += this.timeOfDayRange[i];
		}
		this.currentTime = num * 3600.0;
		this.currentSetting = TimeSettings.Static;
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x001053AA File Offset: 0x001035AA
	public void FastForward(float seconds)
	{
		this.baseSeconds += (double)seconds;
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x001053BB File Offset: 0x001035BB
	public void SetFixedWeather(BetterDayNightManager.WeatherType weather)
	{
		this.overrideWeather = true;
		this.overrideWeatherType = weather;
	}

	// Token: 0x0600321E RID: 12830 RVA: 0x001053CB File Offset: 0x001035CB
	public void ClearFixedWeather()
	{
		this.overrideWeather = false;
	}

	// Token: 0x04003E6A RID: 15978
	[OnEnterPlay_SetNull]
	public static volatile BetterDayNightManager instance;

	// Token: 0x04003E6B RID: 15979
	[OnEnterPlay_Clear]
	public static List<PerSceneRenderData> allScenesRenderData = new List<PerSceneRenderData>();

	// Token: 0x04003E6C RID: 15980
	public Shader standard;

	// Token: 0x04003E6D RID: 15981
	public Shader standardCutout;

	// Token: 0x04003E6E RID: 15982
	public Shader gorillaUnlit;

	// Token: 0x04003E6F RID: 15983
	public Shader gorillaUnlitCutout;

	// Token: 0x04003E70 RID: 15984
	public Material[] standardMaterialsUnlit;

	// Token: 0x04003E71 RID: 15985
	public Material[] standardMaterialsUnlitDarker;

	// Token: 0x04003E72 RID: 15986
	public Material[] dayNightSupportedMaterials;

	// Token: 0x04003E73 RID: 15987
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x04003E74 RID: 15988
	public string[] dayNightLightmapNames;

	// Token: 0x04003E75 RID: 15989
	public string[] dayNightWeatherLightmapNames;

	// Token: 0x04003E76 RID: 15990
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04003E77 RID: 15991
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04003E78 RID: 15992
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04003E79 RID: 15993
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04003E7A RID: 15994
	public float[] standardUnlitColor;

	// Token: 0x04003E7B RID: 15995
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x04003E7C RID: 15996
	public float currentLerp;

	// Token: 0x04003E7D RID: 15997
	public float currentTimestep;

	// Token: 0x04003E7E RID: 15998
	public double[] timeOfDayRange;

	// Token: 0x04003E7F RID: 15999
	public double timeMultiplier;

	// Token: 0x04003E80 RID: 16000
	private float lastTime;

	// Token: 0x04003E81 RID: 16001
	private double currentTime;

	// Token: 0x04003E82 RID: 16002
	private double totalHours;

	// Token: 0x04003E83 RID: 16003
	private double totalSeconds;

	// Token: 0x04003E84 RID: 16004
	private float colorFrom;

	// Token: 0x04003E85 RID: 16005
	private float colorTo;

	// Token: 0x04003E86 RID: 16006
	private float colorFromDarker;

	// Token: 0x04003E87 RID: 16007
	private float colorToDarker;

	// Token: 0x04003E88 RID: 16008
	public int currentTimeIndex;

	// Token: 0x04003E89 RID: 16009
	public int currentWeatherIndex;

	// Token: 0x04003E8A RID: 16010
	private int lastIndex;

	// Token: 0x04003E8B RID: 16011
	private double currentIndexSeconds;

	// Token: 0x04003E8C RID: 16012
	private float tempLerp;

	// Token: 0x04003E8D RID: 16013
	private double baseSeconds;

	// Token: 0x04003E8E RID: 16014
	private bool computerInit;

	// Token: 0x04003E8F RID: 16015
	private float h;

	// Token: 0x04003E90 RID: 16016
	private float s;

	// Token: 0x04003E91 RID: 16017
	private float v;

	// Token: 0x04003E92 RID: 16018
	public int mySeed;

	// Token: 0x04003E93 RID: 16019
	public Random randomNumberGenerator = new Random();

	// Token: 0x04003E94 RID: 16020
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x04003E95 RID: 16021
	public bool overrideWeather;

	// Token: 0x04003E96 RID: 16022
	public BetterDayNightManager.WeatherType overrideWeatherType;

	// Token: 0x04003E98 RID: 16024
	public float rainChance = 0.3f;

	// Token: 0x04003E99 RID: 16025
	public int maxRainDuration = 5;

	// Token: 0x04003E9A RID: 16026
	private int rainDuration;

	// Token: 0x04003E9B RID: 16027
	private float remainingSeconds;

	// Token: 0x04003E9C RID: 16028
	private long initialDayCycles;

	// Token: 0x04003E9D RID: 16029
	private long gameEpochDay;

	// Token: 0x04003E9E RID: 16030
	private int currentWeatherCycle;

	// Token: 0x04003E9F RID: 16031
	private int fromWeatherIndex;

	// Token: 0x04003EA0 RID: 16032
	private int toWeatherIndex;

	// Token: 0x04003EA1 RID: 16033
	private Texture2D fromSky;

	// Token: 0x04003EA2 RID: 16034
	private Texture2D fromSky2;

	// Token: 0x04003EA3 RID: 16035
	private Texture2D fromSky3;

	// Token: 0x04003EA4 RID: 16036
	private Texture2D toSky;

	// Token: 0x04003EA5 RID: 16037
	private Texture2D toSky2;

	// Token: 0x04003EA6 RID: 16038
	private Texture2D toSky3;

	// Token: 0x04003EA7 RID: 16039
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x04003EA8 RID: 16040
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x04003EA9 RID: 16041
	private float lastTimeChecked;

	// Token: 0x04003EAA RID: 16042
	private Func<int, int> timeIndexOverrideFunc;

	// Token: 0x04003EAB RID: 16043
	public int overrideIndex = -1;

	// Token: 0x04003EAC RID: 16044
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x04003EAD RID: 16045
	public TimeSettings currentSetting;

	// Token: 0x04003EAE RID: 16046
	private ShaderHashId _Color = "_Color";

	// Token: 0x04003EAF RID: 16047
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x04003EB0 RID: 16048
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x04003EB1 RID: 16049
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x04003EB2 RID: 16050
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x04003EB3 RID: 16051
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x04003EB4 RID: 16052
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x04003EB5 RID: 16053
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x04003EB6 RID: 16054
	private bool shouldRepopulate;

	// Token: 0x04003EB7 RID: 16055
	private Coroutine animatingLightFlash;

	// Token: 0x020007CC RID: 1996
	public enum WeatherType
	{
		// Token: 0x04003EB9 RID: 16057
		None,
		// Token: 0x04003EBA RID: 16058
		Raining,
		// Token: 0x04003EBB RID: 16059
		All
	}

	// Token: 0x020007CD RID: 1997
	private class ScheduledEvent
	{
		// Token: 0x04003EBC RID: 16060
		public long lastDayCalled;

		// Token: 0x04003EBD RID: 16061
		public int hour;

		// Token: 0x04003EBE RID: 16062
		public Action action;
	}
}
