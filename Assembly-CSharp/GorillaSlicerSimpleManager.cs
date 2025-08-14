using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020006FB RID: 1787
public class GorillaSlicerSimpleManager : MonoBehaviour
{
	// Token: 0x06002CA4 RID: 11428 RVA: 0x000EBFA1 File Offset: 0x000EA1A1
	protected void Awake()
	{
		if (GorillaSlicerSimpleManager.hasInstance && GorillaSlicerSimpleManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSlicerSimpleManager.SetInstance(this);
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x000EBFC4 File Offset: 0x000EA1C4
	public static void CreateManager()
	{
		GorillaSlicerSimpleManager gorillaSlicerSimpleManager = new GameObject("GorillaSlicerSimpleManager").AddComponent<GorillaSlicerSimpleManager>();
		gorillaSlicerSimpleManager.fixedUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.updateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.lateUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.sW = new Stopwatch();
		GorillaSlicerSimpleManager.SetInstance(gorillaSlicerSimpleManager);
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x000EC011 File Offset: 0x000EA211
	private static void SetInstance(GorillaSlicerSimpleManager manager)
	{
		GorillaSlicerSimpleManager.instance = manager;
		GorillaSlicerSimpleManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000EC02C File Offset: 0x000EA22C
	public static void RegisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (!GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (!GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (!GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Add(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x000EC0C0 File Offset: 0x000EA2C0
	public static void UnregisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Remove(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Remove(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Remove(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x000EC158 File Offset: 0x000EA358
	public void FixedUpdate()
	{
		if (this.updateIndex < 0 || this.updateIndex >= this.fixedUpdateSlice.Count + this.updateSlice.Count + this.lateUpdateSlice.Count)
		{
			this.updateIndex = 0;
		}
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && this.updateIndex < this.fixedUpdateSlice.Count)
		{
			int num = 0;
			while (num < this.checkEveryXUpdateSteps && this.updateIndex < this.fixedUpdateSlice.Count)
			{
				IGorillaSliceableSimple gorillaSliceableSimple = this.fixedUpdateSlice[this.updateIndex];
				if (0 <= this.updateIndex && this.updateIndex < this.fixedUpdateSlice.Count)
				{
					MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
					if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
					{
						gorillaSliceableSimple.SliceUpdate();
					}
				}
				this.updateIndex++;
				num++;
			}
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x000EC278 File Offset: 0x000EA478
	public void Update()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int num = count + count2;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && count <= this.updateIndex && this.updateIndex < num)
		{
			int num2 = 0;
			while (num2 < this.checkEveryXUpdateSteps && this.updateIndex < num)
			{
				IGorillaSliceableSimple gorillaSliceableSimple = this.updateSlice[this.updateIndex - count];
				if (0 <= this.updateIndex - count && this.updateIndex - count < this.updateSlice.Count)
				{
					MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
					if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
					{
						gorillaSliceableSimple.SliceUpdate();
					}
				}
				this.updateIndex++;
				num2++;
			}
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x000EC37C File Offset: 0x000EA57C
	public void LateUpdate()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int count3 = this.lateUpdateSlice.Count;
		int num = count + count2;
		int num2 = num + count3;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && num <= this.updateIndex && this.updateIndex < num2)
		{
			int num3 = 0;
			while (num3 < this.checkEveryXUpdateSteps && this.updateIndex < num2)
			{
				IGorillaSliceableSimple gorillaSliceableSimple = this.lateUpdateSlice[this.updateIndex - num];
				if (0 <= this.updateIndex - num && this.updateIndex - num < this.lateUpdateSlice.Count)
				{
					MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
					if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
					{
						gorillaSliceableSimple.SliceUpdate();
					}
				}
				this.updateIndex++;
				num3++;
			}
		}
		this.sW.Stop();
		if (this.updateIndex >= num2)
		{
			this.updateIndex = -1;
		}
		this.ticksThisFrame = 0L;
	}

	// Token: 0x04003811 RID: 14353
	public static GorillaSlicerSimpleManager instance;

	// Token: 0x04003812 RID: 14354
	public static bool hasInstance;

	// Token: 0x04003813 RID: 14355
	public List<IGorillaSliceableSimple> fixedUpdateSlice;

	// Token: 0x04003814 RID: 14356
	public List<IGorillaSliceableSimple> updateSlice;

	// Token: 0x04003815 RID: 14357
	public List<IGorillaSliceableSimple> lateUpdateSlice;

	// Token: 0x04003816 RID: 14358
	public long ticksPerFrame = 1000L;

	// Token: 0x04003817 RID: 14359
	public long ticksThisFrame;

	// Token: 0x04003818 RID: 14360
	public int checkEveryXUpdateSteps = 1;

	// Token: 0x04003819 RID: 14361
	public int updateIndex = -1;

	// Token: 0x0400381A RID: 14362
	public Stopwatch sW;

	// Token: 0x020006FC RID: 1788
	public enum UpdateStep
	{
		// Token: 0x0400381C RID: 14364
		FixedUpdate,
		// Token: 0x0400381D RID: 14365
		Update,
		// Token: 0x0400381E RID: 14366
		LateUpdate
	}
}
