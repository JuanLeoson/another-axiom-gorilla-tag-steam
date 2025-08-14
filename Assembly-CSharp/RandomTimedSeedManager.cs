using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200022A RID: 554
[NetworkBehaviourWeaved(2)]
public class RandomTimedSeedManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000D06 RID: 3334 RVA: 0x00045DEA File Offset: 0x00043FEA
	// (set) Token: 0x06000D07 RID: 3335 RVA: 0x00045DF1 File Offset: 0x00043FF1
	public static RandomTimedSeedManager instance { get; private set; }

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000D08 RID: 3336 RVA: 0x00045DF9 File Offset: 0x00043FF9
	// (set) Token: 0x06000D09 RID: 3337 RVA: 0x00045E01 File Offset: 0x00044001
	public int seed { get; private set; }

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000D0A RID: 3338 RVA: 0x00045E0A File Offset: 0x0004400A
	// (set) Token: 0x06000D0B RID: 3339 RVA: 0x00045E12 File Offset: 0x00044012
	public float currentSyncTime { get; private set; }

	// Token: 0x06000D0C RID: 3340 RVA: 0x00045E1B File Offset: 0x0004401B
	protected override void Awake()
	{
		base.Awake();
		RandomTimedSeedManager.instance = this;
		this.seed = Random.Range(-1000000, -1000000);
		this.idealSyncTime = 0f;
		this.currentSyncTime = 0f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00045E5A File Offset: 0x0004405A
	public void AddCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Add(callback);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00045E68 File Offset: 0x00044068
	public void RemoveCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Remove(callback);
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000D0F RID: 3343 RVA: 0x00045E77 File Offset: 0x00044077
	// (set) Token: 0x06000D10 RID: 3344 RVA: 0x00045E7F File Offset: 0x0004407F
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06000D11 RID: 3345 RVA: 0x00045E88 File Offset: 0x00044088
	void ITickSystemTick.Tick()
	{
		this.currentSyncTime += Time.deltaTime;
		this.idealSyncTime += Time.deltaTime;
		if (this.idealSyncTime > 1E+09f)
		{
			this.idealSyncTime -= 1E+09f;
			this.currentSyncTime -= 1E+09f;
		}
		if (!base.GetView.AmOwner)
		{
			this.currentSyncTime = Mathf.Lerp(this.currentSyncTime, this.idealSyncTime, 0.1f);
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000D12 RID: 3346 RVA: 0x00045F13 File Offset: 0x00044113
	// (set) Token: 0x06000D13 RID: 3347 RVA: 0x00045F3D File Offset: 0x0004413D
	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe RandomTimedSeedManager.RandomTimedSeedManagerData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00045F68 File Offset: 0x00044168
	public override void WriteDataFusion()
	{
		this.Data = new RandomTimedSeedManager.RandomTimedSeedManagerData(this.seed, this.currentSyncTime);
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00045F84 File Offset: 0x00044184
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.seed, this.Data.currentSyncTime);
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00045FB3 File Offset: 0x000441B3
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.seed);
		stream.SendNext(this.currentSyncTime);
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00045FE8 File Offset: 0x000441E8
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int seedVal = (int)stream.ReceiveNext();
		float testTime = (float)stream.ReceiveNext();
		this.ReadDataShared(seedVal, testTime);
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x00046024 File Offset: 0x00044224
	private void ReadDataShared(int seedVal, float testTime)
	{
		if (!float.IsFinite(testTime))
		{
			return;
		}
		this.seed = seedVal;
		if (testTime >= 0f && testTime <= 1E+09f)
		{
			if (this.idealSyncTime - testTime > 500000000f)
			{
				this.currentSyncTime = testTime;
			}
			this.idealSyncTime = testTime;
		}
		if (this.seed != this.cachedSeed && this.seed >= -1000000 && this.seed <= -1000000)
		{
			this.currentSyncTime = this.idealSyncTime;
			this.cachedSeed = this.seed;
			foreach (Action action in this.callbacksOnSeedChanged)
			{
				action();
			}
		}
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00046107 File Offset: 0x00044307
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0004611F File Offset: 0x0004431F
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000FFB RID: 4091
	private List<Action> callbacksOnSeedChanged = new List<Action>();

	// Token: 0x04000FFD RID: 4093
	private float idealSyncTime;

	// Token: 0x04000FFF RID: 4095
	private int cachedSeed;

	// Token: 0x04001000 RID: 4096
	private const int SeedMin = -1000000;

	// Token: 0x04001001 RID: 4097
	private const int SeedMax = -1000000;

	// Token: 0x04001002 RID: 4098
	private const float MaxSyncTime = 1E+09f;

	// Token: 0x04001004 RID: 4100
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 2)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private RandomTimedSeedManager.RandomTimedSeedManagerData _Data;

	// Token: 0x0200022B RID: 555
	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct RandomTimedSeedManagerData : INetworkStruct
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000D1C RID: 3356 RVA: 0x00046133 File Offset: 0x00044333
		// (set) Token: 0x06000D1D RID: 3357 RVA: 0x00046141 File Offset: 0x00044341
		[Networked]
		public unsafe int seed
		{
			readonly get
			{
				return *(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed);
			}
			set
			{
				*(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed) = value;
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000D1E RID: 3358 RVA: 0x00046150 File Offset: 0x00044350
		// (set) Token: 0x06000D1F RID: 3359 RVA: 0x0004615E File Offset: 0x0004435E
		[Networked]
		public unsafe float currentSyncTime
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime) = value;
			}
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0004616D File Offset: 0x0004436D
		public RandomTimedSeedManagerData(int seed, float currentSyncTime)
		{
			this.seed = seed;
			this.currentSyncTime = currentSyncTime;
		}

		// Token: 0x04001005 RID: 4101
		[FixedBufferProperty(typeof(int), typeof(UnityValueSurrogate@ReaderWriter@System_Int32), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _seed;

		// Token: 0x04001006 RID: 4102
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _currentSyncTime;
	}
}
