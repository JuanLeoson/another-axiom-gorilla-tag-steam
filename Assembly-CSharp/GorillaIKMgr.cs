using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x020006DC RID: 1756
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x06002BD3 RID: 11219 RVA: 0x000E8348 File Offset: 0x000E6548
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x000E8350 File Offset: 0x000E6550
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.firstFrame = true;
		this.tAA = new TransformAccessArray(0, -1);
		this.transformList = new List<Transform>();
		this.job = new GorillaIKMgr.IKJob
		{
			constantInput = new NativeArray<GorillaIKMgr.IKConstantInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			input = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			output = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
		this.jobXform = new GorillaIKMgr.IKTransformJob
		{
			transformRotations = new NativeArray<Quaternion>(140, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x000E83E4 File Offset: 0x000E65E4
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.jobXformHandle.Complete();
		this.jobXform.transformRotations.Dispose();
		this.tAA.Dispose();
		this.job.input.Dispose();
		this.job.constantInput.Dispose();
		this.job.output.Dispose();
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x000E8454 File Offset: 0x000E6654
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
		this.updatedSinceLastRun = true;
		if (this.job.constantInput.IsCreated)
		{
			this.SetConstantData(ik, this.actualListSz - 2);
		}
	}

	// Token: 0x06002BD7 RID: 11223 RVA: 0x000E84A4 File Offset: 0x000E66A4
	public void DeregisterIK(GorillaIK ik)
	{
		int num = this.ikList.FindIndex((GorillaIK curr) => curr == ik);
		this.updatedSinceLastRun = true;
		this.ikList.RemoveAt(num);
		this.actualListSz -= 2;
		if (this.job.constantInput.IsCreated)
		{
			for (int i = num; i < this.actualListSz; i++)
			{
				this.job.constantInput[i] = this.job.constantInput[i + 2];
			}
		}
	}

	// Token: 0x06002BD8 RID: 11224 RVA: 0x000E8540 File Offset: 0x000E6740
	private void SetConstantData(GorillaIK ik, int index)
	{
		this.job.constantInput[index] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerLeft,
			initRotUpper = ik.initialUpperLeft
		};
		this.job.constantInput[index + 1] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerRight,
			initRotUpper = ik.initialUpperRight
		};
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x000E85B8 File Offset: 0x000E67B8
	private void CopyInput()
	{
		int num = 0;
		int i = 0;
		while (i < this.actualListSz)
		{
			GorillaIK gorillaIK = this.ikList[i / 2];
			this.job.input[i] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Left()
			};
			this.job.input[i + 1] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Right()
			};
			gorillaIK.ClearOverrides();
			i += 2;
			num++;
		}
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x000E8644 File Offset: 0x000E6844
	private void CopyOutput()
	{
		bool flag = false;
		if (this.updatedSinceLastRun || this.tAA.length != this.ikList.Count * 7)
		{
			flag = true;
			this.tAA.Dispose();
			this.transformList.Clear();
		}
		for (int i = 0; i < this.ikList.Count; i++)
		{
			GorillaIK gorillaIK = this.ikList[i];
			if (flag || this.updatedSinceLastRun)
			{
				this.transformList.Add(gorillaIK.leftUpperArm);
				this.transformList.Add(gorillaIK.leftLowerArm);
				this.transformList.Add(gorillaIK.rightUpperArm);
				this.transformList.Add(gorillaIK.rightLowerArm);
				this.transformList.Add(gorillaIK.headBone);
				this.transformList.Add(gorillaIK.leftHand);
				this.transformList.Add(gorillaIK.rightHand);
			}
			this.jobXform.transformRotations[this.tFormCount * i] = this.job.output[i * 2].upperArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 1] = this.job.output[i * 2].lowerArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 2] = this.job.output[i * 2 + 1].upperArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 3] = this.job.output[i * 2 + 1].lowerArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 4] = gorillaIK.targetHead.rotation;
			this.jobXform.transformRotations[this.tFormCount * i + 5] = gorillaIK.targetLeft.rotation;
			this.jobXform.transformRotations[this.tFormCount * i + 6] = gorillaIK.targetRight.rotation;
		}
		if (flag)
		{
			this.tAA = new TransformAccessArray(this.transformList.ToArray(), -1);
		}
		this.updatedSinceLastRun = false;
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x000E8894 File Offset: 0x000E6A94
	public void LateUpdate()
	{
		if (!this.firstFrame)
		{
			this.jobXformHandle.Complete();
		}
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		this.jobXformHandle = this.jobXform.Schedule(this.tAA, default(JobHandle));
		this.firstFrame = false;
	}

	// Token: 0x04003730 RID: 14128
	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	// Token: 0x04003731 RID: 14129
	private const int MaxSize = 20;

	// Token: 0x04003732 RID: 14130
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04003733 RID: 14131
	private int actualListSz;

	// Token: 0x04003734 RID: 14132
	private JobHandle jobHandle;

	// Token: 0x04003735 RID: 14133
	private JobHandle jobXformHandle;

	// Token: 0x04003736 RID: 14134
	private bool firstFrame = true;

	// Token: 0x04003737 RID: 14135
	private TransformAccessArray tAA;

	// Token: 0x04003738 RID: 14136
	private List<Transform> transformList;

	// Token: 0x04003739 RID: 14137
	private bool updatedSinceLastRun;

	// Token: 0x0400373A RID: 14138
	private int tFormCount = 7;

	// Token: 0x0400373B RID: 14139
	private GorillaIKMgr.IKJob job;

	// Token: 0x0400373C RID: 14140
	private GorillaIKMgr.IKTransformJob jobXform;

	// Token: 0x020006DD RID: 1757
	private struct IKConstantInput
	{
		// Token: 0x0400373D RID: 14141
		public Quaternion initRotLower;

		// Token: 0x0400373E RID: 14142
		public Quaternion initRotUpper;
	}

	// Token: 0x020006DE RID: 1758
	private struct IKInput
	{
		// Token: 0x0400373F RID: 14143
		public Vector3 targetPos;
	}

	// Token: 0x020006DF RID: 1759
	private struct IKOutput
	{
		// Token: 0x06002BDD RID: 11229 RVA: 0x000E8937 File Offset: 0x000E6B37
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
		}

		// Token: 0x04003740 RID: 14144
		public Quaternion upperArmLocalRot;

		// Token: 0x04003741 RID: 14145
		public Quaternion lowerArmLocalRot;
	}

	// Token: 0x020006E0 RID: 1760
	[BurstCompile]
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x06002BDE RID: 11230 RVA: 0x000E8948 File Offset: 0x000E6B48
		public void Execute(int i)
		{
			Quaternion initRotUpper = this.constantInput[i].initRotUpper;
			Vector3 vector = GorillaIKMgr.IKJob.upperArmLocalPos;
			Quaternion rotation = initRotUpper * this.constantInput[i].initRotLower;
			Vector3 vector2 = vector + initRotUpper * GorillaIKMgr.IKJob.forearmLocalPos;
			Vector3 vector3 = vector2 + rotation * GorillaIKMgr.IKJob.handLocalPos;
			float num = 0f;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			float max = magnitude + magnitude2 - num;
			Vector3 normalized = (vector3 - vector).normalized;
			Vector3 normalized2 = (vector2 - vector).normalized;
			Vector3 normalized3 = (vector3 - vector2).normalized;
			Vector3 normalized4 = (this.input[i].targetPos - vector).normalized;
			float num2 = Mathf.Clamp((this.input[i].targetPos - vector).magnitude, num, max);
			float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized2), -1f, 1f));
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-normalized2, normalized3), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized4), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num2 * num2) / (-2f * magnitude * num2), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((num2 * num2 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized5 = Vector3.Cross(normalized, normalized2).normalized;
			Vector3 normalized6 = Vector3.Cross(normalized, normalized4).normalized;
			Quaternion rhs = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized5);
			Quaternion rhs2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(rotation) * normalized5);
			Quaternion rhs3 = Quaternion.AngleAxis(num5 * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized6);
			Quaternion upperArmLocalRot_ = this.constantInput[i].initRotUpper * rhs3 * rhs;
			Quaternion lowerArmLocalRot_ = this.constantInput[i].initRotLower * rhs2;
			this.output[i] = new GorillaIKMgr.IKOutput(upperArmLocalRot_, lowerArmLocalRot_);
		}

		// Token: 0x04003742 RID: 14146
		public NativeArray<GorillaIKMgr.IKConstantInput> constantInput;

		// Token: 0x04003743 RID: 14147
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x04003744 RID: 14148
		public NativeArray<GorillaIKMgr.IKOutput> output;

		// Token: 0x04003745 RID: 14149
		private static readonly Vector3 upperArmLocalPos = new Vector3(-0.0002577677f, 0.1454885f, -0.02598158f);

		// Token: 0x04003746 RID: 14150
		private static readonly Vector3 forearmLocalPos = new Vector3(4.204223E-06f, 0.4061671f, -1.043081E-06f);

		// Token: 0x04003747 RID: 14151
		private static readonly Vector3 handLocalPos = new Vector3(3.073364E-08f, 0.3816895f, 1.117587E-08f);
	}

	// Token: 0x020006E1 RID: 1761
	[BurstCompile]
	private struct IKTransformJob : IJobParallelForTransform
	{
		// Token: 0x06002BE0 RID: 11232 RVA: 0x000E8C50 File Offset: 0x000E6E50
		public void Execute(int index, TransformAccess xform)
		{
			if (index % 7 <= 3)
			{
				xform.localRotation = this.transformRotations[index];
				return;
			}
			xform.rotation = this.transformRotations[index];
		}

		// Token: 0x04003748 RID: 14152
		public NativeArray<Quaternion> transformRotations;
	}
}
