using System;
using BoingKit;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C1A RID: 3098
	[BurstCompile]
	public struct BuilderFindPotentialSnaps : IJobParallelFor
	{
		// Token: 0x06004C08 RID: 19464 RVA: 0x00177984 File Offset: 0x00175B84
		public void Execute(int index)
		{
			BuilderGridPlaneData builderGridPlaneData = this.gridPlanes[index];
			for (int i = 0; i < this.checkGridPlanes.Length; i++)
			{
				BuilderGridPlaneData builderGridPlaneData2 = this.checkGridPlanes[i];
				BuilderPotentialPlacementData value = default(BuilderPotentialPlacementData);
				if (this.TryPlaceGridPlaneOnGridPlane(ref builderGridPlaneData, ref builderGridPlaneData2, ref value))
				{
					this.potentialPlacements.Enqueue(value);
				}
			}
		}

		// Token: 0x06004C09 RID: 19465 RVA: 0x001779E4 File Offset: 0x00175BE4
		public bool TryPlaceGridPlaneOnGridPlane(ref BuilderGridPlaneData gridPlane, ref BuilderGridPlaneData checkGridPlane, ref BuilderPotentialPlacementData potentialPlacement)
		{
			if (checkGridPlane.male == gridPlane.male)
			{
				return false;
			}
			if (checkGridPlane.pieceId == gridPlane.pieceId)
			{
				return false;
			}
			Vector3 vector = gridPlane.position;
			Quaternion rhs = gridPlane.rotation;
			Vector3 point = this.worldToLocalRot * (vector + this.worldToLocalPos);
			Quaternion rhs2 = this.worldToLocalRot * rhs;
			vector = this.localToWorldPos + this.localToWorldRot * point;
			rhs = this.localToWorldRot * rhs2;
			Vector3 position = checkGridPlane.position;
			float sqrMagnitude = (position - vector).sqrMagnitude;
			float num = checkGridPlane.boundingRadius + gridPlane.boundingRadius;
			if (sqrMagnitude > num * num)
			{
				return false;
			}
			Quaternion rotation = checkGridPlane.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Quaternion quaternion2 = quaternion * rhs;
			float num2 = Vector3.Dot(Vector3.up, quaternion2 * Vector3.up);
			if (num2 < this.currSnapParams.maxUpDotProduct)
			{
				return false;
			}
			Vector3 vector2 = quaternion * (vector - position);
			float y = vector2.y;
			if (Mathf.Abs(y) > 1f)
			{
				return false;
			}
			if ((gridPlane.male && y > this.currSnapParams.minOffsetY) || (!gridPlane.male && y < -this.currSnapParams.minOffsetY))
			{
				return false;
			}
			if (Mathf.Abs(y) > this.currSnapParams.maxOffsetY)
			{
				return false;
			}
			Quaternion identity = Quaternion.identity;
			Vector3 vector3 = new Vector3(quaternion2.x, quaternion2.y, quaternion2.z);
			if (vector3.sqrMagnitude > MathUtil.Epsilon)
			{
				Quaternion quaternion3;
				QuaternionUtil.DecomposeSwingTwist(quaternion2, Vector3.up, out quaternion3, out identity);
			}
			float maxTwistDotProduct = this.currSnapParams.maxTwistDotProduct;
			Vector3 lhs = identity * Vector3.forward;
			float num3 = Vector3.Dot(lhs, Vector3.forward);
			float num4 = Vector3.Dot(lhs, Vector3.right);
			bool flag = Mathf.Abs(num3) > maxTwistDotProduct;
			bool flag2 = Mathf.Abs(num4) > maxTwistDotProduct;
			if (!flag && !flag2)
			{
				return false;
			}
			float y2;
			uint num5;
			if (flag)
			{
				y2 = ((num3 > 0f) ? 0f : 180f);
				num5 = ((num3 > 0f) ? 0U : 2U);
			}
			else
			{
				y2 = ((num4 > 0f) ? 90f : 270f);
				num5 = ((num4 > 0f) ? 1U : 3U);
			}
			int num6 = flag2 ? gridPlane.width : gridPlane.length;
			int num7 = flag2 ? gridPlane.length : gridPlane.width;
			float num8 = (num7 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num9 = (num6 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num10 = (checkGridPlane.width % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num11 = (checkGridPlane.length % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num12 = num8 - num10;
			float num13 = num9 - num11;
			int num14 = Mathf.RoundToInt((vector2.x - num12) / this.gridSize);
			int num15 = Mathf.RoundToInt((vector2.z - num13) / this.gridSize);
			int num16 = num14 + Mathf.FloorToInt((float)num7 / 2f);
			int num17 = num15 + Mathf.FloorToInt((float)num6 / 2f);
			int num18 = num16 - (num7 - 1);
			int num19 = num17 - (num6 - 1);
			int num20 = Mathf.FloorToInt((float)checkGridPlane.width / 2f);
			int num21 = Mathf.FloorToInt((float)checkGridPlane.length / 2f);
			int num22 = num20 - (checkGridPlane.width - 1);
			int num23 = num21 - (checkGridPlane.length - 1);
			if (num18 > num20 || num16 < num22 || num19 > num21 || num17 < num23)
			{
				return false;
			}
			Quaternion rhs3 = Quaternion.Euler(0f, y2, 0f);
			Quaternion lhs2 = rotation * rhs3;
			float x = (float)num14 * this.gridSize + num12;
			float z = (float)num15 * this.gridSize + num13;
			Vector3 point2 = new Vector3(x, 0f, z);
			Vector3 a = position + rotation * point2;
			Quaternion quaternion4 = lhs2 * Quaternion.Inverse(gridPlane.localRotation);
			Vector3 localPosition = a - quaternion4 * gridPlane.localPosition;
			potentialPlacement.localPosition = localPosition;
			potentialPlacement.localRotation = quaternion4;
			float num24 = 0.025f;
			float score = -Mathf.Abs(y) + num2 * num24;
			potentialPlacement.score = score;
			potentialPlacement.pieceId = gridPlane.pieceId;
			potentialPlacement.attachIndex = gridPlane.attachIndex;
			potentialPlacement.parentPieceId = checkGridPlane.pieceId;
			potentialPlacement.parentAttachIndex = checkGridPlane.attachIndex;
			potentialPlacement.attachDistance = Mathf.Abs(y);
			potentialPlacement.attachPlaneNormal = Vector3.up;
			if (!checkGridPlane.male)
			{
				potentialPlacement.attachPlaneNormal *= -1f;
			}
			potentialPlacement.parentAttachBounds.min.x = Mathf.Max(num22, num18);
			potentialPlacement.parentAttachBounds.min.y = Mathf.Max(num23, num19);
			potentialPlacement.parentAttachBounds.max.x = Mathf.Min(num20, num16);
			potentialPlacement.parentAttachBounds.max.y = Mathf.Min(num21, num17);
			potentialPlacement.twist = (byte)num5;
			potentialPlacement.bumpOffsetX = (sbyte)num14;
			potentialPlacement.bumpOffsetZ = (sbyte)num15;
			Vector2Int v = Vector2Int.zero;
			Vector2Int v2 = Vector2Int.zero;
			v.x = potentialPlacement.parentAttachBounds.min.x - num14;
			v2.x = potentialPlacement.parentAttachBounds.max.x - num14;
			v.y = potentialPlacement.parentAttachBounds.min.y - num15;
			v2.y = potentialPlacement.parentAttachBounds.max.y - num15;
			int offsetX = (num7 % 2 == 0) ? 1 : 0;
			int offsetY = (num6 % 2 == 0) ? 1 : 0;
			if (flag && num3 <= 0f)
			{
				v = this.Rotate180(v, offsetX, offsetY);
				v2 = this.Rotate180(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 <= 0f)
			{
				v = this.Rotate270(v, offsetX, offsetY);
				v2 = this.Rotate270(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 >= 0f)
			{
				v = this.Rotate90(v, offsetX, offsetY);
				v2 = this.Rotate90(v2, offsetX, offsetY);
			}
			potentialPlacement.attachBounds.min.x = Mathf.Min(v.x, v2.x);
			potentialPlacement.attachBounds.min.y = Mathf.Min(v.y, v2.y);
			potentialPlacement.attachBounds.max.x = Mathf.Max(v.x, v2.x);
			potentialPlacement.attachBounds.max.y = Mathf.Max(v.y, v2.y);
			return true;
		}

		// Token: 0x06004C0A RID: 19466 RVA: 0x0017373E File Offset: 0x0017193E
		private Vector2Int Rotate90(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y * -1 + offsetY, v.x);
		}

		// Token: 0x06004C0B RID: 19467 RVA: 0x00173757 File Offset: 0x00171957
		private Vector2Int Rotate270(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y, v.x * -1 + offsetX);
		}

		// Token: 0x06004C0C RID: 19468 RVA: 0x00173770 File Offset: 0x00171970
		private Vector2Int Rotate180(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.x * -1 + offsetX, v.y * -1 + offsetY);
		}

		// Token: 0x04005529 RID: 21801
		[ReadOnly]
		public float gridSize;

		// Token: 0x0400552A RID: 21802
		[ReadOnly]
		public BuilderTable.SnapParams currSnapParams;

		// Token: 0x0400552B RID: 21803
		[ReadOnly]
		public NativeList<BuilderGridPlaneData> gridPlanes;

		// Token: 0x0400552C RID: 21804
		[ReadOnly]
		public NativeList<BuilderGridPlaneData> checkGridPlanes;

		// Token: 0x0400552D RID: 21805
		[ReadOnly]
		public Vector3 worldToLocalPos;

		// Token: 0x0400552E RID: 21806
		[ReadOnly]
		public Quaternion worldToLocalRot;

		// Token: 0x0400552F RID: 21807
		[ReadOnly]
		public Vector3 localToWorldPos;

		// Token: 0x04005530 RID: 21808
		[ReadOnly]
		public Quaternion localToWorldRot;

		// Token: 0x04005531 RID: 21809
		public NativeQueue<BuilderPotentialPlacementData>.ParallelWriter potentialPlacements;
	}
}
