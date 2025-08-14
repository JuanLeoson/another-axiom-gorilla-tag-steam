using System;
using UnityEngine;

// Token: 0x02000562 RID: 1378
public class Campfire : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002197 RID: 8599 RVA: 0x000B6CB4 File Offset: 0x000B4EB4
	private void Start()
	{
		this.lastAngleBottom = 0f;
		this.lastAngleMiddle = 0f;
		this.lastAngleTop = 0f;
		this.perlinBottom = (float)Random.Range(0, 100);
		this.perlinMiddle = (float)Random.Range(200, 300);
		this.perlinTop = (float)Random.Range(400, 500);
		this.startingRotationBottom = this.baseFire.localEulerAngles.x;
		this.startingRotationMiddle = this.middleFire.localEulerAngles.x;
		this.startingRotationTop = this.topFire.localEulerAngles.x;
		this.tempVec = new Vector3(0f, 0f, 0f);
		this.mergedBottom = false;
		this.mergedMiddle = false;
		this.mergedTop = false;
		this.wasActive = false;
		this.lastTime = Time.time;
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000B6DA0 File Offset: 0x000B4FA0
	public void SliceUpdate()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		if ((this.isActive[BetterDayNightManager.instance.currentTimeIndex] && BetterDayNightManager.instance.CurrentWeather() != BetterDayNightManager.WeatherType.Raining) || this.overrideDayNight == 1)
		{
			if (!this.wasActive)
			{
				this.wasActive = true;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 1f);
			}
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
		}
		else
		{
			if (this.wasActive)
			{
				this.wasActive = false;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 0.25f);
			}
			this.ReturnToOff(ref this.baseFire, this.startingRotationBottom, ref this.mergedBottom);
			this.ReturnToOff(ref this.middleFire, this.startingRotationMiddle, ref this.mergedMiddle);
			this.ReturnToOff(ref this.topFire, this.startingRotationTop, ref this.mergedTop);
		}
		this.lastTime = Time.time;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000B6FA4 File Offset: 0x000B51A4
	private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
	{
		perlinValue += perlinStep;
		lastAngle += (Time.time - this.lastTime) * Mathf.PerlinNoise(perlinValue, 0f);
		this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
		if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > flameTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (isMerged)
		{
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
		{
			isMerged = true;
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
		flameTransform.localEulerAngles = this.tempVec;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000B70EC File Offset: 0x000B52EC
	private void ReturnToOff(ref Transform startTransform, float targetAngle, ref bool isMerged)
	{
		this.tempVec.x = targetAngle;
		if (Mathf.Abs(this.tempVec.x - startTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > startTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (!isMerged)
		{
			if (Mathf.Abs(startTransform.localEulerAngles.x - targetAngle) < 1f)
			{
				isMerged = true;
				return;
			}
			this.tempVec.x = (this.tempVec.x - startTransform.localEulerAngles.x) * this.slerp + startTransform.localEulerAngles.x;
			startTransform.localEulerAngles = this.tempVec;
		}
	}

	// Token: 0x04002AF6 RID: 10998
	public Transform baseFire;

	// Token: 0x04002AF7 RID: 10999
	public Transform middleFire;

	// Token: 0x04002AF8 RID: 11000
	public Transform topFire;

	// Token: 0x04002AF9 RID: 11001
	public float baseMultiplier;

	// Token: 0x04002AFA RID: 11002
	public float middleMultiplier;

	// Token: 0x04002AFB RID: 11003
	public float topMultiplier;

	// Token: 0x04002AFC RID: 11004
	public float bottomRange;

	// Token: 0x04002AFD RID: 11005
	public float middleRange;

	// Token: 0x04002AFE RID: 11006
	public float topRange;

	// Token: 0x04002AFF RID: 11007
	private float lastAngleBottom;

	// Token: 0x04002B00 RID: 11008
	private float lastAngleMiddle;

	// Token: 0x04002B01 RID: 11009
	private float lastAngleTop;

	// Token: 0x04002B02 RID: 11010
	public float perlinStepBottom;

	// Token: 0x04002B03 RID: 11011
	public float perlinStepMiddle;

	// Token: 0x04002B04 RID: 11012
	public float perlinStepTop;

	// Token: 0x04002B05 RID: 11013
	private float perlinBottom;

	// Token: 0x04002B06 RID: 11014
	private float perlinMiddle;

	// Token: 0x04002B07 RID: 11015
	private float perlinTop;

	// Token: 0x04002B08 RID: 11016
	public float startingRotationBottom;

	// Token: 0x04002B09 RID: 11017
	public float startingRotationMiddle;

	// Token: 0x04002B0A RID: 11018
	public float startingRotationTop;

	// Token: 0x04002B0B RID: 11019
	public float slerp = 0.01f;

	// Token: 0x04002B0C RID: 11020
	private bool mergedBottom;

	// Token: 0x04002B0D RID: 11021
	private bool mergedMiddle;

	// Token: 0x04002B0E RID: 11022
	private bool mergedTop;

	// Token: 0x04002B0F RID: 11023
	public string lastTimeOfDay;

	// Token: 0x04002B10 RID: 11024
	public Material mat;

	// Token: 0x04002B11 RID: 11025
	private float h;

	// Token: 0x04002B12 RID: 11026
	private float s;

	// Token: 0x04002B13 RID: 11027
	private float v;

	// Token: 0x04002B14 RID: 11028
	public int overrideDayNight;

	// Token: 0x04002B15 RID: 11029
	private Vector3 tempVec;

	// Token: 0x04002B16 RID: 11030
	public bool[] isActive;

	// Token: 0x04002B17 RID: 11031
	public bool wasActive;

	// Token: 0x04002B18 RID: 11032
	private float lastTime;
}
