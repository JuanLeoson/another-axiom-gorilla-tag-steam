using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.Shared.Scripts.Utilities;
using TagEffects;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000283 RID: 643
[DefaultExecutionOrder(10000)]
public class HandEffectsTriggerRegistry : MonoBehaviour
{
	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000EAB RID: 3755 RVA: 0x0005885F File Offset: 0x00056A5F
	// (set) Token: 0x06000EAC RID: 3756 RVA: 0x00058866 File Offset: 0x00056A66
	public static HandEffectsTriggerRegistry Instance { get; private set; }

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000EAD RID: 3757 RVA: 0x0005886E File Offset: 0x00056A6E
	// (set) Token: 0x06000EAE RID: 3758 RVA: 0x00058875 File Offset: 0x00056A75
	public static bool HasInstance { get; private set; }

	// Token: 0x06000EAF RID: 3759 RVA: 0x0005887D File Offset: 0x00056A7D
	public static void FindInstance()
	{
		HandEffectsTriggerRegistry.Instance = Object.FindAnyObjectByType<HandEffectsTriggerRegistry>();
		HandEffectsTriggerRegistry.HasInstance = true;
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x00058890 File Offset: 0x00056A90
	private void Awake()
	{
		HandEffectsTriggerRegistry.Instance = this;
		HandEffectsTriggerRegistry.HasInstance = true;
		this.existingCollisionBits = new GTBitArray(900);
		this.newCollisionBits = new GTBitArray(900);
		this.job = new HandEffectsTriggerRegistry.HandEffectsJob
		{
			positionInput = new NativeArray<Vector3>(30, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			closeOutput = new NativeArray<bool>(900, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			actualListSize = this.actualListSz
		};
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x00058908 File Offset: 0x00056B08
	public void Register(IHandEffectsTrigger trigger)
	{
		if (this.triggers.Count < 30)
		{
			this.actualListSz++;
			this.triggers.Add(trigger);
			this.triggersDirty = true;
		}
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0005893A File Offset: 0x00056B3A
	public void Unregister(IHandEffectsTrigger trigger)
	{
		this.actualListSz--;
		this.triggers.Remove(trigger);
		this.triggersDirty = true;
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0005895E File Offset: 0x00056B5E
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.job.positionInput.Dispose();
		this.job.closeOutput.Dispose();
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0005898C File Offset: 0x00056B8C
	private void Update()
	{
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CheckForHandEffectOnProcessedOutput();
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x000589D4 File Offset: 0x00056BD4
	public void CheckForHandEffectOnProcessedOutput()
	{
		this.newCollisionBits.Clear();
		if (this.triggersDirty)
		{
			this.localTriggersIdx = 0;
			this.triggersDirty = false;
		}
		else
		{
			this.localTriggersIdx = (this.localTriggersIdx + 1) % this.triggers.Count;
		}
		IHandEffectsTrigger handEffectsTrigger = this.triggers[this.localTriggersIdx];
		int num = this.localTriggersIdx * 30;
		for (int i = 0; i < this.triggers.Count; i++)
		{
			if (i != this.localTriggersIdx && this.job.closeOutput[this.localTriggersIdx * 30 + i])
			{
				IHandEffectsTrigger handEffectsTrigger2 = this.triggers[i];
				if (handEffectsTrigger.InTriggerZone(handEffectsTrigger2) || handEffectsTrigger2.InTriggerZone(handEffectsTrigger))
				{
					int idx = num + i;
					this.newCollisionBits[idx] = true;
					if (!this.existingCollisionBits[idx] && Time.time - this.triggerTimes[this.localTriggersIdx] > 0.5f && Time.time - this.triggerTimes[i] > 0.5f)
					{
						handEffectsTrigger.OnTriggerEntered(handEffectsTrigger2);
						handEffectsTrigger2.OnTriggerEntered(handEffectsTrigger);
						this.triggerTimes[this.localTriggersIdx] = (this.triggerTimes[i] = Time.time);
					}
				}
			}
		}
		for (int j = 0; j < this.newCollisionBits.Length; j++)
		{
			this.existingCollisionBits[j] = this.newCollisionBits[j];
		}
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00058B54 File Offset: 0x00056D54
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.job.positionInput[i] = this.triggers[i].Transform.position;
		}
		this.job.actualListSize = this.actualListSz;
	}

	// Token: 0x04001792 RID: 6034
	private const int MAX_TRIGGERS = 30;

	// Token: 0x04001793 RID: 6035
	private const int BIT_ARRAY_SIZE = 900;

	// Token: 0x04001794 RID: 6036
	private const float COOLDOWN_TIME = 0.5f;

	// Token: 0x04001795 RID: 6037
	private const float DEFAULT_RADIUS = 0.07f;

	// Token: 0x04001796 RID: 6038
	private List<IHandEffectsTrigger> triggers = new List<IHandEffectsTrigger>();

	// Token: 0x04001797 RID: 6039
	private bool triggersDirty = true;

	// Token: 0x04001798 RID: 6040
	private int localTriggersIdx;

	// Token: 0x04001799 RID: 6041
	private float[] triggerTimes = new float[30];

	// Token: 0x0400179A RID: 6042
	private GTBitArray existingCollisionBits;

	// Token: 0x0400179B RID: 6043
	private GTBitArray newCollisionBits;

	// Token: 0x0400179C RID: 6044
	private int actualListSz;

	// Token: 0x0400179D RID: 6045
	private JobHandle jobHandle;

	// Token: 0x0400179E RID: 6046
	private HandEffectsTriggerRegistry.HandEffectsJob job;

	// Token: 0x02000284 RID: 644
	[BurstCompile]
	private struct HandEffectsJob : IJobParallelFor
	{
		// Token: 0x06000EB8 RID: 3768 RVA: 0x00058BD4 File Offset: 0x00056DD4
		public void Execute(int i)
		{
			for (int j = i + 1; j < this.actualListSize; j++)
			{
				this.closeOutput[i * 30 + j] = (this.positionInput[i] - this.positionInput[j]).IsShorterThan(0.07f);
			}
		}

		// Token: 0x040017A1 RID: 6049
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> positionInput;

		// Token: 0x040017A2 RID: 6050
		[NativeDisableParallelForRestriction]
		public NativeArray<bool> closeOutput;

		// Token: 0x040017A3 RID: 6051
		public int actualListSize;
	}
}
