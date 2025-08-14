using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x020007D2 RID: 2002
public class DayNightCycle : MonoBehaviour
{
	// Token: 0x06003230 RID: 12848 RVA: 0x001056EC File Offset: 0x001038EC
	public void Awake()
	{
		this.fromMap = new Texture2D(this._sunriseMap.width, this._sunriseMap.height);
		this.fromMap = LightmapSettings.lightmaps[0].lightmapColor;
		this.toMap = new Texture2D(this._dayMap.width, this._dayMap.height);
		this.toMap.SetPixels(this._dayMap.GetPixels());
		this.toMap.Apply();
		this.workBlockMix = new Color[this.subTextureSize * this.subTextureSize];
		this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height, this.fromMap.graphicsFormat, TextureCreationFlags.None);
		this.newData = new LightmapData();
		this.textureHeight = this.fromMap.height;
		this.textureWidth = this.fromMap.width;
		this.subTextureArray = new Texture2D[(int)Mathf.Pow((float)(this.textureHeight / this.subTextureSize), 2f)];
		Debug.Log("aaaa " + this.fromMap.format.ToString());
		Debug.Log("aaaa " + this.fromMap.graphicsFormat.ToString());
		this.startJob = false;
		this.startCoroutine = false;
		this.startedCoroutine = false;
		this.finishedCoroutine = false;
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x00105870 File Offset: 0x00103A70
	public void Update()
	{
		if (this.startJob)
		{
			this.startJob = false;
			this.startTime = Time.realtimeSinceStartup;
			base.StartCoroutine(this.UpdateWork());
			this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
		}
		if (this.jobStarted && this.jobHandle.IsCompleted)
		{
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.jobHandle.Complete();
			this.jobStarted = false;
			this.newTexture.SetPixels(this.job.mixedPixels.ToArray());
			this.newData.lightmapDir = LightmapSettings.lightmaps[0].lightmapDir;
			LightmapSettings.lightmaps = new LightmapData[]
			{
				this.newData
			};
			this.job.fromPixels.Dispose();
			this.job.toPixels.Dispose();
			this.job.mixedPixels.Dispose();
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
		if (this.startCoroutine)
		{
			this.startCoroutine = false;
			this.startTime = Time.realtimeSinceStartup;
			this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height);
			base.StartCoroutine(this.UpdateWork());
		}
		if (this.startedCoroutine && this.finishedCoroutine)
		{
			this.startedCoroutine = false;
			this.finishedCoroutine = false;
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.newData = LightmapSettings.lightmaps[0];
			this.newData.lightmapColor = this.fromMap;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			lightmaps[0].lightmapColor = this.fromMap;
			LightmapSettings.lightmaps = lightmaps;
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x00105A5E File Offset: 0x00103C5E
	public IEnumerator UpdateWork()
	{
		yield return 0;
		this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
		this.startTime = Time.realtimeSinceStartup;
		this.startedCoroutine = true;
		this.currentSubTexture = 0;
		int num;
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.subTextureArray[i] = new Texture2D(this.subTextureSize, this.subTextureSize, this.fromMap.graphicsFormat, TextureCreationFlags.None);
			yield return 0;
			num = i;
		}
		for (int i = 0; i < this.textureWidth / this.subTextureSize; i = num + 1)
		{
			this.currentColumn = i;
			for (int j = 0; j < this.textureHeight / this.subTextureSize; j = num + 1)
			{
				this.currentRow = j;
				this.workBlockFrom = this.fromMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				this.workBlockTo = this.toMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				for (int k = 0; k < this.subTextureSize * this.subTextureSize - 1; k++)
				{
					this.workBlockMix[k] = Color.Lerp(this.workBlockFrom[k], this.workBlockTo[k], this.lerpAmount);
				}
				this.subTextureArray[j * (this.textureWidth / this.subTextureSize) + i].SetPixels(0, 0, this.subTextureSize, this.subTextureSize, this.workBlockMix);
				yield return 0;
				num = j;
			}
			num = i;
		}
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.currentSubTexture = i;
			this.subTextureArray[i].Apply();
			yield return 0;
			Graphics.CopyTexture(this.subTextureArray[i], 0, 0, 0, 0, this.subTextureSize, this.subTextureSize, this.newTexture, 0, 0, i * this.subTextureSize % this.textureHeight, (int)Mathf.Floor((float)(this.subTextureSize * i / this.textureHeight)) * this.subTextureSize);
			yield return 0;
			num = i;
		}
		this.finishedCoroutine = true;
		yield break;
	}

	// Token: 0x04003ECD RID: 16077
	public Texture2D _dayMap;

	// Token: 0x04003ECE RID: 16078
	private Texture2D fromMap;

	// Token: 0x04003ECF RID: 16079
	public Texture2D _sunriseMap;

	// Token: 0x04003ED0 RID: 16080
	private Texture2D toMap;

	// Token: 0x04003ED1 RID: 16081
	public DayNightCycle.LerpBakedLightingJob job;

	// Token: 0x04003ED2 RID: 16082
	public JobHandle jobHandle;

	// Token: 0x04003ED3 RID: 16083
	public bool isComplete;

	// Token: 0x04003ED4 RID: 16084
	private float startTime;

	// Token: 0x04003ED5 RID: 16085
	public float timeTakenStartingJob;

	// Token: 0x04003ED6 RID: 16086
	public float timeTakenPostJob;

	// Token: 0x04003ED7 RID: 16087
	public float timeTakenDuringJob;

	// Token: 0x04003ED8 RID: 16088
	public LightmapData newData;

	// Token: 0x04003ED9 RID: 16089
	private Color[] fromPixels;

	// Token: 0x04003EDA RID: 16090
	private Color[] toPixels;

	// Token: 0x04003EDB RID: 16091
	private Color[] mixedPixels;

	// Token: 0x04003EDC RID: 16092
	private LightmapData[] newDatas;

	// Token: 0x04003EDD RID: 16093
	public Texture2D newTexture;

	// Token: 0x04003EDE RID: 16094
	public int textureWidth;

	// Token: 0x04003EDF RID: 16095
	public int textureHeight;

	// Token: 0x04003EE0 RID: 16096
	private Color[] workBlockFrom;

	// Token: 0x04003EE1 RID: 16097
	private Color[] workBlockTo;

	// Token: 0x04003EE2 RID: 16098
	private Color[] workBlockMix;

	// Token: 0x04003EE3 RID: 16099
	public int subTextureSize = 1024;

	// Token: 0x04003EE4 RID: 16100
	public Texture2D[] subTextureArray;

	// Token: 0x04003EE5 RID: 16101
	public bool startCoroutine;

	// Token: 0x04003EE6 RID: 16102
	public bool startedCoroutine;

	// Token: 0x04003EE7 RID: 16103
	public bool finishedCoroutine;

	// Token: 0x04003EE8 RID: 16104
	public bool startJob;

	// Token: 0x04003EE9 RID: 16105
	public float switchTimeTaken;

	// Token: 0x04003EEA RID: 16106
	public bool jobStarted;

	// Token: 0x04003EEB RID: 16107
	public float lerpAmount;

	// Token: 0x04003EEC RID: 16108
	public int currentRow;

	// Token: 0x04003EED RID: 16109
	public int currentColumn;

	// Token: 0x04003EEE RID: 16110
	public int currentSubTexture;

	// Token: 0x04003EEF RID: 16111
	public int currentRowInSubtexture;

	// Token: 0x020007D3 RID: 2003
	public struct LerpBakedLightingJob : IJob
	{
		// Token: 0x06003234 RID: 12852 RVA: 0x00105A80 File Offset: 0x00103C80
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		// Token: 0x04003EF0 RID: 16112
		public NativeArray<Color> fromPixels;

		// Token: 0x04003EF1 RID: 16113
		public NativeArray<Color> toPixels;

		// Token: 0x04003EF2 RID: 16114
		public NativeArray<Color> mixedPixels;

		// Token: 0x04003EF3 RID: 16115
		public float lerpValue;
	}
}
