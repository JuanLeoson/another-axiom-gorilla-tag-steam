using System;
using System.Collections;
using System.Threading;
using UnityEngine;

// Token: 0x020006C2 RID: 1730
public class GorillaDayNight : MonoBehaviour
{
	// Token: 0x06002AC3 RID: 10947 RVA: 0x000E35BC File Offset: 0x000E17BC
	public void Awake()
	{
		if (GorillaDayNight.instance == null)
		{
			GorillaDayNight.instance = this;
		}
		else if (GorillaDayNight.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.test = false;
		this.working = false;
		this.lerpValue = 0.5f;
		this.workingLightMapDatas = new LightmapData[3];
		this.workingLightMapData = new LightmapData();
		this.workingLightMapData.lightmapColor = this.lightmapDatas[0].lightTextures[0];
		this.workingLightMapData.lightmapDir = this.lightmapDatas[0].dirTextures[0];
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000E3660 File Offset: 0x000E1860
	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000E3690 File Offset: 0x000E1890
	public void DoWork()
	{
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		this.done = true;
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000E389C File Offset: 0x000E1A9C
	public void DoLightsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x000E3960 File Offset: 0x000E1B60
	public void DoDirsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x000E3A23 File Offset: 0x000E1C23
	private IEnumerator LightMapSet(int setFirstData, int setSecondData, float setLerp)
	{
		this.working = true;
		this.firstData = setFirstData;
		this.secondData = setSecondData;
		this.lerpValue = setLerp;
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.lightsThread = new Thread(new ThreadStart(this.DoLightsStep));
			this.lightsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapColor.Apply(false);
			this.dirsThread = new Thread(new ThreadStart(this.DoDirsStep));
			this.dirsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		LightmapSettings.lightmaps = this.workingLightMapDatas;
		this.working = false;
		this.done = true;
		yield break;
	}

	// Token: 0x04003643 RID: 13891
	[OnEnterPlay_SetNull]
	public static volatile GorillaDayNight instance;

	// Token: 0x04003644 RID: 13892
	public GorillaLightmapData[] lightmapDatas;

	// Token: 0x04003645 RID: 13893
	private LightmapData[] workingLightMapDatas;

	// Token: 0x04003646 RID: 13894
	private LightmapData workingLightMapData;

	// Token: 0x04003647 RID: 13895
	public float lerpValue;

	// Token: 0x04003648 RID: 13896
	public bool done;

	// Token: 0x04003649 RID: 13897
	public bool finishedStep;

	// Token: 0x0400364A RID: 13898
	private Color[] fromPixels;

	// Token: 0x0400364B RID: 13899
	private Color[] toPixels;

	// Token: 0x0400364C RID: 13900
	private Color[] mixedPixels;

	// Token: 0x0400364D RID: 13901
	public int firstData;

	// Token: 0x0400364E RID: 13902
	public int secondData;

	// Token: 0x0400364F RID: 13903
	public int i;

	// Token: 0x04003650 RID: 13904
	public int j;

	// Token: 0x04003651 RID: 13905
	public int k;

	// Token: 0x04003652 RID: 13906
	public int l;

	// Token: 0x04003653 RID: 13907
	private Thread lightsThread;

	// Token: 0x04003654 RID: 13908
	private Thread dirsThread;

	// Token: 0x04003655 RID: 13909
	public bool test;

	// Token: 0x04003656 RID: 13910
	public bool working;
}
