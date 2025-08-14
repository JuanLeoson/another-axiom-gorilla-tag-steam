using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000851 RID: 2129
[DefaultExecutionOrder(0)]
public class VRRigJobManager : MonoBehaviour
{
	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x0600358E RID: 13710 RVA: 0x00118CE0 File Offset: 0x00116EE0
	public static VRRigJobManager Instance
	{
		get
		{
			return VRRigJobManager._instance;
		}
	}

	// Token: 0x0600358F RID: 13711 RVA: 0x00118CE7 File Offset: 0x00116EE7
	private void Awake()
	{
		VRRigJobManager._instance = this;
		this.cachedInput = new NativeArray<VRRigJobManager.VRRigTransformInput>(9, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.tAA = new TransformAccessArray(9, 2);
		this.job = default(VRRigJobManager.VRRigTransformJob);
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x00118D18 File Offset: 0x00116F18
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.tAA.Dispose();
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x00118D3B File Offset: 0x00116F3B
	public void RegisterVRRig(VRRig rig)
	{
		this.rigList.Add(rig);
		this.tAA.Add(rig.transform);
		this.actualListSz++;
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x00118D68 File Offset: 0x00116F68
	public void DeregisterVRRig(VRRig rig)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.rigList.Remove(rig);
		for (int i = this.actualListSz - 1; i >= 0; i--)
		{
			if (this.tAA[i] == rig.transform)
			{
				this.tAA.RemoveAtSwapBack(i);
				break;
			}
		}
		this.actualListSz--;
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x00118DD4 File Offset: 0x00116FD4
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.cachedInput[i] = new VRRigJobManager.VRRigTransformInput
			{
				rigPosition = this.rigList[i].jobPos,
				rigRotaton = this.rigList[i].jobRotation
			};
			this.tAA[i] = this.rigList[i].transform;
		}
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00118E54 File Offset: 0x00117054
	public void Update()
	{
		this.jobHandle.Complete();
		for (int i = 0; i < this.rigList.Count; i++)
		{
			this.rigList[i].RemoteRigUpdate();
		}
		this.CopyInput();
		this.job.input = this.cachedInput;
		this.jobHandle = this.job.Schedule(this.tAA, default(JobHandle));
	}

	// Token: 0x04004270 RID: 17008
	[OnEnterPlay_SetNull]
	private static VRRigJobManager _instance;

	// Token: 0x04004271 RID: 17009
	private const int MaxSize = 9;

	// Token: 0x04004272 RID: 17010
	private const int questJobThreads = 2;

	// Token: 0x04004273 RID: 17011
	private List<VRRig> rigList = new List<VRRig>(9);

	// Token: 0x04004274 RID: 17012
	private NativeArray<VRRigJobManager.VRRigTransformInput> cachedInput;

	// Token: 0x04004275 RID: 17013
	private TransformAccessArray tAA;

	// Token: 0x04004276 RID: 17014
	private int actualListSz;

	// Token: 0x04004277 RID: 17015
	private JobHandle jobHandle;

	// Token: 0x04004278 RID: 17016
	private VRRigJobManager.VRRigTransformJob job;

	// Token: 0x02000852 RID: 2130
	private struct VRRigTransformInput
	{
		// Token: 0x04004279 RID: 17017
		public Vector3 rigPosition;

		// Token: 0x0400427A RID: 17018
		public Quaternion rigRotaton;
	}

	// Token: 0x02000853 RID: 2131
	[BurstCompile]
	private struct VRRigTransformJob : IJobParallelForTransform
	{
		// Token: 0x06003596 RID: 13718 RVA: 0x00118EDF File Offset: 0x001170DF
		public void Execute(int i, TransformAccess tA)
		{
			if (i < this.input.Length)
			{
				tA.position = this.input[i].rigPosition;
				tA.rotation = this.input[i].rigRotaton;
			}
		}

		// Token: 0x0400427B RID: 17019
		[ReadOnly]
		public NativeArray<VRRigJobManager.VRRigTransformInput> input;
	}
}
