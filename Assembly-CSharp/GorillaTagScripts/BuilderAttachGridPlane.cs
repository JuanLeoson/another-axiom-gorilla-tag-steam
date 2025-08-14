using System;
using GorillaTagScripts.Builder;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BFE RID: 3070
	public class BuilderAttachGridPlane : MonoBehaviour
	{
		// Token: 0x06004AA9 RID: 19113 RVA: 0x0016A5A2 File Offset: 0x001687A2
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06004AAA RID: 19114 RVA: 0x0016A5C0 File Offset: 0x001687C0
		public void Setup(BuilderPiece piece, int attachIndex, float gridSize)
		{
			this.piece = piece;
			this.attachIndex = attachIndex;
			this.pieceToGridPosition = piece.transform.InverseTransformPoint(base.transform.position);
			this.pieceToGridRotation = Quaternion.Inverse(piece.transform.rotation) * base.transform.rotation;
			float num = (float)(this.width + 2) * gridSize;
			float num2 = (float)(this.length + 2) * gridSize;
			this.boundingRadius = Mathf.Sqrt(num * num + num2 * num2);
			this.connected = new bool[this.width * this.length];
			this.widthOffset = ((this.width % 2 == 0) ? (gridSize / 2f) : 0f);
			this.lengthOffset = ((this.length % 2 == 0) ? (gridSize / 2f) : 0f);
			this.gridPlaneDataIndex = -1;
			this.childPieceCount = 0;
		}

		// Token: 0x06004AAB RID: 19115 RVA: 0x0016A6AC File Offset: 0x001688AC
		public void OnReturnToPool(BuilderPool pool)
		{
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				SnapOverlap snapOverlap = nextOverlap;
				nextOverlap = nextOverlap.nextOverlap;
				if (snapOverlap.otherPlane != null)
				{
					snapOverlap.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
				}
				this.SetConnected(snapOverlap.bounds, false);
				pool.DestroySnapOverlap(snapOverlap);
			}
			int num = this.width * this.length;
			for (int i = 0; i < num; i++)
			{
				this.connected[i] = false;
			}
			this.childPieceCount = 0;
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x0016A72C File Offset: 0x0016892C
		public Vector3 GetGridPosition(int x, int z, float gridSize)
		{
			float num = (this.width % 2 == 0) ? (gridSize / 2f) : 0f;
			float num2 = (this.length % 2 == 0) ? (gridSize / 2f) : 0f;
			return this.center.position + this.center.rotation * new Vector3((float)x * gridSize - num, (this.male ? 0.002f : -0.002f) * gridSize, (float)z * gridSize - num2);
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x0016A7B2 File Offset: 0x001689B2
		public int GetChildCount()
		{
			return this.childPieceCount;
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x0016A7BC File Offset: 0x001689BC
		public void ChangeChildPieceCount(int delta)
		{
			this.childPieceCount += delta;
			if (this.piece.parentPiece == null)
			{
				return;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return;
			}
			this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].ChangeChildPieceCount(delta);
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x0016A842 File Offset: 0x00168A42
		public void AddSnapOverlap(SnapOverlap newOverlap)
		{
			if (this.firstOverlap == null)
			{
				this.firstOverlap = newOverlap;
			}
			else
			{
				newOverlap.nextOverlap = this.firstOverlap;
				this.firstOverlap = newOverlap;
			}
			this.SetConnected(newOverlap.bounds, true);
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x0016A878 File Offset: 0x00168A78
		public void RemoveSnapsWithDifferentRoot(BuilderPiece root, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				if (nextOverlap.otherPlane == null || nextOverlap.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap2 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap2.bounds, false);
					pool.DestroySnapOverlap(snapOverlap2);
				}
				else if (root == null || nextOverlap.otherPlane.piece.GetRootPiece() != root)
				{
					SnapOverlap snapOverlap3 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					snapOverlap3.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else
				{
					snapOverlap = nextOverlap;
					nextOverlap = nextOverlap.nextOverlap;
				}
			}
		}

		// Token: 0x06004AB1 RID: 19121 RVA: 0x0016A990 File Offset: 0x00168B90
		public void RemoveSnapsWithPiece(BuilderPiece piece, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (piece == null || pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				if (nextOverlap.otherPlane == null || nextOverlap.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap2 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap2.bounds, false);
					pool.DestroySnapOverlap(snapOverlap2);
				}
				else if (nextOverlap.otherPlane.piece == piece)
				{
					SnapOverlap snapOverlap3 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else
				{
					snapOverlap = nextOverlap;
					nextOverlap = nextOverlap.nextOverlap;
				}
			}
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x0016AA90 File Offset: 0x00168C90
		private void SetConnected(SnapBounds bounds, bool connect)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 >= num3 || num4 < 0)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return;
					}
					this.connected[num4] = connect;
				}
			}
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x0016AB60 File Offset: 0x00168D60
		public bool IsConnected(SnapBounds bounds)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 < 0 || num4 >= num3)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return false;
					}
					if (this.connected[num4])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x0016AC34 File Offset: 0x00168E34
		public void CalcGridOverlap(BuilderAttachGridPlane otherGridPlane, Vector3 otherPieceLocalPos, Quaternion otherPieceLocalRot, float gridSize, out Vector2Int min, out Vector2Int max)
		{
			int num = otherGridPlane.width;
			int num2 = otherGridPlane.length;
			Quaternion rotation = otherPieceLocalRot * otherGridPlane.pieceToGridRotation;
			Vector3 lossyScale = base.transform.lossyScale;
			otherPieceLocalPos.Scale(base.transform.lossyScale);
			Vector3 vector = otherPieceLocalPos + otherPieceLocalRot * otherGridPlane.pieceToGridPosition;
			if (Mathf.Abs(Vector3.Dot(rotation * Vector3.forward, Vector3.forward)) < 0.707f)
			{
				num = otherGridPlane.length;
				num2 = otherGridPlane.width;
			}
			float num3 = (num % 2 == 0) ? (gridSize / 2f) : 0f;
			float num4 = (num2 % 2 == 0) ? (gridSize / 2f) : 0f;
			float num5 = (this.width % 2 == 0) ? (gridSize / 2f) : 0f;
			float num6 = (this.length % 2 == 0) ? (gridSize / 2f) : 0f;
			float num7 = num3 - num5;
			float num8 = num4 - num6;
			int num9 = Mathf.RoundToInt((vector.x - num7) / gridSize);
			int num10 = Mathf.RoundToInt((vector.z - num8) / gridSize);
			int num11 = num9 + Mathf.FloorToInt((float)num / 2f);
			int num12 = num10 + Mathf.FloorToInt((float)num2 / 2f);
			int a = num11 - (num - 1);
			int a2 = num12 - (num2 - 1);
			int num13 = Mathf.FloorToInt((float)this.width / 2f);
			int num14 = Mathf.FloorToInt((float)this.length / 2f);
			int b = num13 - (this.width - 1);
			int b2 = num14 - (this.length - 1);
			min = new Vector2Int(Mathf.Max(a, b), Mathf.Max(a2, b2));
			max = new Vector2Int(Mathf.Min(num11, num13), Mathf.Min(num12, num14));
		}

		// Token: 0x06004AB5 RID: 19125 RVA: 0x000023F5 File Offset: 0x000005F5
		protected virtual void OnDrawGizmosSelected()
		{
		}

		// Token: 0x06004AB6 RID: 19126 RVA: 0x0016ADF8 File Offset: 0x00168FF8
		public bool IsAttachedToMovingGrid()
		{
			return this.piece.state == BuilderPiece.State.AttachedAndPlaced && !this.piece.isBuiltIntoTable && (this.isMoving || (!(this.piece.parentPiece == null) && this.piece.parentAttachIndex >= 0 && this.piece.parentAttachIndex < this.piece.parentPiece.gridPlanes.Count && this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].IsAttachedToMovingGrid()));
		}

		// Token: 0x06004AB7 RID: 19127 RVA: 0x0016AE9C File Offset: 0x0016909C
		public BuilderAttachGridPlane GetMovingParentGrid()
		{
			if (this.piece.isBuiltIntoTable)
			{
				return null;
			}
			if (this.movesOnPlace && this.movingPart != null && !this.movingPart.IsAnchoredToTable())
			{
				return this;
			}
			if (this.piece.parentPiece == null)
			{
				return null;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return null;
			}
			return this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].GetMovingParentGrid();
		}

		// Token: 0x04005390 RID: 21392
		public bool male;

		// Token: 0x04005391 RID: 21393
		public Transform center;

		// Token: 0x04005392 RID: 21394
		public int width;

		// Token: 0x04005393 RID: 21395
		public int length;

		// Token: 0x04005394 RID: 21396
		[NonSerialized]
		public int gridPlaneDataIndex;

		// Token: 0x04005395 RID: 21397
		[NonSerialized]
		public BuilderItem item;

		// Token: 0x04005396 RID: 21398
		[NonSerialized]
		public BuilderPiece piece;

		// Token: 0x04005397 RID: 21399
		[NonSerialized]
		public int attachIndex;

		// Token: 0x04005398 RID: 21400
		[NonSerialized]
		public float boundingRadius;

		// Token: 0x04005399 RID: 21401
		[NonSerialized]
		public Vector3 pieceToGridPosition;

		// Token: 0x0400539A RID: 21402
		[NonSerialized]
		public Quaternion pieceToGridRotation;

		// Token: 0x0400539B RID: 21403
		[NonSerialized]
		public bool[] connected;

		// Token: 0x0400539C RID: 21404
		[NonSerialized]
		public SnapOverlap firstOverlap;

		// Token: 0x0400539D RID: 21405
		[NonSerialized]
		public float widthOffset;

		// Token: 0x0400539E RID: 21406
		[NonSerialized]
		public float lengthOffset;

		// Token: 0x0400539F RID: 21407
		private int childPieceCount;

		// Token: 0x040053A0 RID: 21408
		[HideInInspector]
		public bool isMoving;

		// Token: 0x040053A1 RID: 21409
		[HideInInspector]
		public bool movesOnPlace;

		// Token: 0x040053A2 RID: 21410
		[HideInInspector]
		public BuilderMovingPart movingPart;
	}
}
