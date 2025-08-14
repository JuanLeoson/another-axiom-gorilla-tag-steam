using System;
using Photon.Pun;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C83 RID: 3203
	public class BuilderConveyorManager : MonoBehaviour
	{
		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06004F2D RID: 20269 RVA: 0x0018982C File Offset: 0x00187A2C
		// (set) Token: 0x06004F2E RID: 20270 RVA: 0x00189833 File Offset: 0x00187A33
		public static BuilderConveyorManager instance { get; private set; }

		// Token: 0x06004F2F RID: 20271 RVA: 0x0018983B File Offset: 0x00187A3B
		private void Awake()
		{
			if (BuilderConveyorManager.instance != null && BuilderConveyorManager.instance != this)
			{
				Object.Destroy(this);
			}
			if (BuilderConveyorManager.instance == null)
			{
				BuilderConveyorManager.instance = this;
			}
		}

		// Token: 0x06004F30 RID: 20272 RVA: 0x00189870 File Offset: 0x00187A70
		public void UpdateManager()
		{
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.UpdateConveyor();
			}
			bool flag = false;
			bool flag2 = this.pieceTransforms.length >= this.pieceTransforms.capacity - 5;
			for (int i = this.jobSplineTimes.Length - 1; i >= 0; i--)
			{
				BuilderConveyor builderConveyor2 = this.table.conveyors[this.conveyorIndices[i]];
				float num = Time.deltaTime * builderConveyor2.GetFrameMovement();
				float num2 = this.jobSplineTimes[i] + num;
				this.jobSplineTimes[i] = Mathf.Clamp(num2, 0f, 1f);
				if (PhotonNetwork.IsMasterClient && (!flag || flag2) && (double)num2 > 0.999)
				{
					builderConveyor2.RemovePieceFromConveyor(this.pieceTransforms[i]);
					this.RemovePieceFromJobAtIndex(i);
					flag = true;
				}
			}
			for (int j = this.shelfSlice; j < this.table.conveyors.Count; j += BuilderTable.SHELF_SLICE_BUCKETS)
			{
				this.table.conveyors[j].UpdateShelfSliced();
			}
			this.shelfSlice = (this.shelfSlice + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
		}

		// Token: 0x06004F31 RID: 20273 RVA: 0x001899EC File Offset: 0x00187BEC
		public void Setup(BuilderTable mytable)
		{
			if (this.isSetup)
			{
				return;
			}
			this.table = mytable;
			this.conveyorSplines = new NativeArray<NativeSpline>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.conveyorRotations = new NativeArray<Quaternion>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = 0;
			for (int i = 0; i < this.table.conveyors.Count; i++)
			{
				this.conveyorSplines[i] = this.table.conveyors[i].nativeSpline;
				this.conveyorRotations[i] = this.table.conveyors[i].GetSpawnTransform().rotation;
				num += this.table.conveyors[i].GetMaxItemsOnConveyor();
			}
			this.maxItemCount = num;
			this.conveyorIndices = new NativeList<int>(this.maxItemCount, Allocator.Persistent);
			this.jobSplineTimes = new NativeList<float>(this.maxItemCount, Allocator.Persistent);
			this.jobShelfOffsets = new NativeList<Vector3>(this.maxItemCount, Allocator.Persistent);
			this.pieceTransforms = new TransformAccessArray(this.maxItemCount, 3);
			this.isSetup = true;
		}

		// Token: 0x06004F32 RID: 20274 RVA: 0x00189B28 File Offset: 0x00187D28
		public float GetSplineProgressForPiece(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					return this.jobSplineTimes[i];
				}
			}
			return 0f;
		}

		// Token: 0x06004F33 RID: 20275 RVA: 0x00189B78 File Offset: 0x00187D78
		public int GetPieceCreateTimestamp(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderConveyor builderConveyor = this.table.conveyors[this.conveyorIndices[i]];
					int num = Mathf.RoundToInt(this.jobSplineTimes[i] / builderConveyor.GetFrameMovement() * 1000f);
					return PhotonNetwork.ServerTimestamp - num;
				}
			}
			return 0;
		}

		// Token: 0x06004F34 RID: 20276 RVA: 0x00189BFC File Offset: 0x00187DFC
		public void OnClearTable()
		{
			if (!this.isSetup)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.OnClearTable();
			}
			for (int i = this.pieceTransforms.length - 1; i >= 0; i--)
			{
				this.pieceTransforms.RemoveAtSwapBack(i);
			}
			this.jobSplineTimes.Clear();
			this.jobShelfOffsets.Clear();
			this.conveyorIndices.Clear();
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x00189CA0 File Offset: 0x00187EA0
		private void OnDestroy()
		{
			this.conveyorSplines.Dispose();
			this.conveyorRotations.Dispose();
			this.conveyorIndices.Dispose();
			this.jobSplineTimes.Dispose();
			this.jobShelfOffsets.Dispose();
			this.pieceTransforms.Dispose();
		}

		// Token: 0x06004F36 RID: 20278 RVA: 0x00189CF0 File Offset: 0x00187EF0
		public JobHandle ConstructJobHandle()
		{
			BuilderConveyorManager.EvaluateSplineJob jobData = new BuilderConveyorManager.EvaluateSplineJob
			{
				conveyorRotations = this.conveyorRotations,
				conveyorIndices = this.conveyorIndices,
				shelfOffsets = this.jobShelfOffsets,
				splineTimes = this.jobSplineTimes
			};
			for (int i = 0; i < this.conveyorSplines.Length; i++)
			{
				jobData.SetSplineAt(i, this.conveyorSplines[i]);
			}
			return jobData.Schedule(this.pieceTransforms, default(JobHandle));
		}

		// Token: 0x06004F37 RID: 20279 RVA: 0x00189D7C File Offset: 0x00187F7C
		public void AddPieceToJob(BuilderPiece piece, float splineTime, int conveyorID)
		{
			if (this.pieceTransforms.length >= this.pieceTransforms.capacity)
			{
				Debug.LogError("Too many pieces on conveyor!");
			}
			this.pieceTransforms.Add(piece.transform);
			this.conveyorIndices.Add(conveyorID);
			this.jobShelfOffsets.Add(piece.desiredShelfOffset);
			this.jobSplineTimes.Add(splineTime);
		}

		// Token: 0x06004F38 RID: 20280 RVA: 0x00189DE7 File Offset: 0x00187FE7
		public void RemovePieceFromJobAtIndex(int index)
		{
			BuilderRenderer.RemoveAt(this.pieceTransforms, index);
			this.jobShelfOffsets.RemoveAt(index);
			this.jobSplineTimes.RemoveAt(index);
			this.conveyorIndices.RemoveAt(index);
		}

		// Token: 0x06004F39 RID: 20281 RVA: 0x00189E1C File Offset: 0x0018801C
		public void RemovePieceFromJob(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderRenderer.RemoveAt(this.pieceTransforms, i);
					this.jobShelfOffsets.RemoveAt(i);
					this.jobSplineTimes.RemoveAt(i);
					this.conveyorIndices.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x04005816 RID: 22550
		private NativeArray<NativeSpline> conveyorSplines;

		// Token: 0x04005817 RID: 22551
		private NativeArray<Quaternion> conveyorRotations;

		// Token: 0x04005818 RID: 22552
		private NativeList<int> conveyorIndices;

		// Token: 0x04005819 RID: 22553
		private NativeList<float> jobSplineTimes;

		// Token: 0x0400581A RID: 22554
		private NativeList<Vector3> jobShelfOffsets;

		// Token: 0x0400581B RID: 22555
		private TransformAccessArray pieceTransforms;

		// Token: 0x0400581C RID: 22556
		private BuilderTable table;

		// Token: 0x0400581D RID: 22557
		private bool isSetup;

		// Token: 0x0400581E RID: 22558
		private int maxItemCount;

		// Token: 0x0400581F RID: 22559
		private int shelfSlice;

		// Token: 0x02000C84 RID: 3204
		[BurstCompile]
		public struct EvaluateSplineJob : IJobParallelForTransform
		{
			// Token: 0x06004F3B RID: 20283 RVA: 0x00189E89 File Offset: 0x00188089
			public NativeSpline GetSplineAt(int index)
			{
				switch (index)
				{
				case 0:
					return this.conveyorSpline0;
				case 1:
					return this.conveyorSpline1;
				case 2:
					return this.conveyorSpline2;
				case 3:
					return this.conveyorSpline3;
				default:
					return this.conveyorSpline0;
				}
			}

			// Token: 0x06004F3C RID: 20284 RVA: 0x00189EC5 File Offset: 0x001880C5
			public void SetSplineAt(int index, NativeSpline s)
			{
				switch (index)
				{
				case 0:
					this.conveyorSpline0 = s;
					return;
				case 1:
					this.conveyorSpline1 = s;
					return;
				case 2:
					this.conveyorSpline2 = s;
					return;
				case 3:
					this.conveyorSpline3 = s;
					return;
				default:
					return;
				}
			}

			// Token: 0x06004F3D RID: 20285 RVA: 0x00189F00 File Offset: 0x00188100
			public void Execute(int index, TransformAccess transform)
			{
				float splineT = this.splineTimes[index];
				Vector3 point = this.shelfOffsets[index];
				int index2 = this.conveyorIndices[index];
				NativeSpline splineAt = this.GetSplineAt(index2);
				Quaternion rotation = this.conveyorRotations[index2];
				float t;
				Vector3 position = CurveUtility.EvaluatePosition(splineAt.GetCurve(splineAt.SplineToCurveT(splineT, out t)), t) + rotation * point;
				transform.position = position;
			}

			// Token: 0x04005821 RID: 22561
			public NativeSpline conveyorSpline0;

			// Token: 0x04005822 RID: 22562
			public NativeSpline conveyorSpline1;

			// Token: 0x04005823 RID: 22563
			public NativeSpline conveyorSpline2;

			// Token: 0x04005824 RID: 22564
			public NativeSpline conveyorSpline3;

			// Token: 0x04005825 RID: 22565
			[ReadOnly]
			public NativeArray<Quaternion> conveyorRotations;

			// Token: 0x04005826 RID: 22566
			[ReadOnly]
			public NativeList<int> conveyorIndices;

			// Token: 0x04005827 RID: 22567
			[ReadOnly]
			public NativeList<float> splineTimes;

			// Token: 0x04005828 RID: 22568
			[ReadOnly]
			public NativeList<Vector3> shelfOffsets;
		}
	}
}
