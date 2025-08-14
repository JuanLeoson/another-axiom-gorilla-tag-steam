using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace GorillaTagScripts
{
	// Token: 0x02000C1C RID: 3100
	[BurstCompile]
	internal struct FindNearbyPiecesJob : IJobParallelForTransform
	{
		// Token: 0x06004C10 RID: 19472 RVA: 0x001781D4 File Offset: 0x001763D4
		public void Execute(int index, TransformAccess transform)
		{
			if (!transform.isValid)
			{
				return;
			}
			this.CheckGridPlane(index, this.leftPieceInHandIndex, transform, this.leftHandPos, true, this.leftHandGridPlanes);
			this.CheckGridPlane(index, this.rightPieceInHandIndex, transform, this.rightHandPos, false, this.rightHandGridPlanes);
		}

		// Token: 0x06004C11 RID: 19473 RVA: 0x00178224 File Offset: 0x00176424
		private void CheckGridPlane(int gridPlaneIndex, int handPieceIndex, TransformAccess transform, Vector3 handPos, bool isLeft, NativeList<BuilderGridPlaneData>.ParallelWriter checkGridPlanes)
		{
			if (handPieceIndex < 0)
			{
				return;
			}
			if ((transform.position - handPos).sqrMagnitude > this.distanceThreshSq)
			{
				return;
			}
			BuilderGridPlaneData builderGridPlaneData = this.gridPlaneData[gridPlaneIndex];
			int pieceIndex = builderGridPlaneData.pieceIndex;
			int rootPieceIndex = this.GetRootPieceIndex(pieceIndex);
			if (rootPieceIndex == handPieceIndex)
			{
				return;
			}
			if (!this.CanPiecesPotentiallySnap(this.localPlayerActorNumber, handPieceIndex, pieceIndex, rootPieceIndex, this.pieceData[pieceIndex].requestedParentPieceIndex, isLeft))
			{
				return;
			}
			transform.GetPositionAndRotation(out builderGridPlaneData.position, out builderGridPlaneData.rotation);
			checkGridPlanes.AddNoResize(builderGridPlaneData);
		}

		// Token: 0x06004C12 RID: 19474 RVA: 0x001782B8 File Offset: 0x001764B8
		public bool CanPiecesPotentiallySnap(int localActorNumber, int pieceInHandIndex, int attachToPieceIndex, int attachToPieceRootIndex, int requestedParentPieceIndex, bool isLeft)
		{
			return this.CanPlayerAttachToRootPiece(localActorNumber, attachToPieceRootIndex, isLeft) && (requestedParentPieceIndex == -1 || pieceInHandIndex != this.GetRootPieceIndex(requestedParentPieceIndex));
		}

		// Token: 0x06004C13 RID: 19475 RVA: 0x001782E4 File Offset: 0x001764E4
		public bool CanPlayerAttachToRootPiece(int playerActorNumber, int attachToPieceRootIndex, bool isLeft)
		{
			BuilderPieceData builderPieceData = this.pieceData[attachToPieceRootIndex];
			if (builderPieceData.state != BuilderPiece.State.AttachedAndPlaced && builderPieceData.privatePlotIndex < 0 && builderPieceData.state != BuilderPiece.State.AttachedToArm)
			{
				return true;
			}
			int attachedBuiltInPiece = this.GetAttachedBuiltInPiece(attachToPieceRootIndex);
			if (attachedBuiltInPiece == -1)
			{
				return true;
			}
			BuilderPieceData builderPieceData2 = this.pieceData[attachedBuiltInPiece];
			if (builderPieceData2.privatePlotIndex < 0 && !builderPieceData2.isArmPiece)
			{
				return true;
			}
			if (builderPieceData2.isArmPiece)
			{
				if (builderPieceData2.heldByActorNumber == playerActorNumber)
				{
					int playerIndex = this.GetPlayerIndex(playerActorNumber);
					return playerIndex >= 0 && this.playerData[playerIndex].scale >= 1f;
				}
				return false;
			}
			else
			{
				if (builderPieceData2.privatePlotIndex < 0)
				{
					return true;
				}
				if (!this.CanPlayerAttachToPlot(builderPieceData2.privatePlotIndex, playerActorNumber))
				{
					return false;
				}
				if (!isLeft)
				{
					return this.privatePlotData[builderPieceData2.privatePlotIndex].isUnderCapacityRight;
				}
				return this.privatePlotData[builderPieceData2.privatePlotIndex].isUnderCapacityLeft;
			}
		}

		// Token: 0x06004C14 RID: 19476 RVA: 0x001783D4 File Offset: 0x001765D4
		public bool CanPlayerAttachToPlot(int privatePlotIndex, int actorNumber)
		{
			BuilderPrivatePlotData builderPrivatePlotData = this.privatePlotData[privatePlotIndex];
			return (builderPrivatePlotData.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && builderPrivatePlotData.ownerActorNumber == actorNumber) || (builderPrivatePlotData.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && this.localPlayerPlotIndex < 0);
		}

		// Token: 0x06004C15 RID: 19477 RVA: 0x00178418 File Offset: 0x00176618
		private int GetPlayerIndex(int playerActorNumber)
		{
			for (int i = 0; i < this.playerData.Length; i++)
			{
				if (this.playerData[i].playerActorNumber == playerActorNumber)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004C16 RID: 19478 RVA: 0x00178454 File Offset: 0x00176654
		public int GetAttachedBuiltInPiece(int pieceIndex)
		{
			BuilderPieceData builderPieceData = this.pieceData[pieceIndex];
			if (builderPieceData.isBuiltIntoTable)
			{
				return pieceIndex;
			}
			if (builderPieceData.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return -1;
			}
			int num = this.GetRootPieceIndex(pieceIndex);
			int parentPieceIndex = this.pieceData[num].parentPieceIndex;
			if (parentPieceIndex != -1)
			{
				num = parentPieceIndex;
			}
			if (this.pieceData[num].isBuiltIntoTable)
			{
				return num;
			}
			return -1;
		}

		// Token: 0x06004C17 RID: 19479 RVA: 0x001784B8 File Offset: 0x001766B8
		private int GetRootPieceIndex(int pieceIndex)
		{
			int num = pieceIndex;
			while (num != -1 && this.pieceData[num].parentPieceIndex != -1 && !this.pieceData[this.pieceData[num].parentPieceIndex].isBuiltIntoTable)
			{
				num = this.pieceData[num].parentPieceIndex;
			}
			return num;
		}

		// Token: 0x04005532 RID: 21810
		[ReadOnly]
		public float distanceThreshSq;

		// Token: 0x04005533 RID: 21811
		[ReadOnly]
		public Vector3 leftHandPos;

		// Token: 0x04005534 RID: 21812
		[ReadOnly]
		public int leftPieceInHandIndex;

		// Token: 0x04005535 RID: 21813
		[ReadOnly]
		public Vector3 rightHandPos;

		// Token: 0x04005536 RID: 21814
		[ReadOnly]
		public int rightPieceInHandIndex;

		// Token: 0x04005537 RID: 21815
		[ReadOnly]
		public int localPlayerPlotIndex;

		// Token: 0x04005538 RID: 21816
		[ReadOnly]
		public int localPlayerActorNumber;

		// Token: 0x04005539 RID: 21817
		[ReadOnly]
		public NativeArray<BuilderPieceData> pieceData;

		// Token: 0x0400553A RID: 21818
		[ReadOnly]
		public NativeArray<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x0400553B RID: 21819
		[ReadOnly]
		public NativeArray<BuilderPrivatePlotData> privatePlotData;

		// Token: 0x0400553C RID: 21820
		[ReadOnly]
		public NativeArray<BuilderPlayerData> playerData;

		// Token: 0x0400553D RID: 21821
		public NativeList<BuilderGridPlaneData>.ParallelWriter leftHandGridPlanes;

		// Token: 0x0400553E RID: 21822
		public NativeList<BuilderGridPlaneData>.ParallelWriter rightHandGridPlanes;
	}
}
