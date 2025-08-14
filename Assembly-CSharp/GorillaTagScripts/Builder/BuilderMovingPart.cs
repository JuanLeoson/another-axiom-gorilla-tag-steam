using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C85 RID: 3205
	public class BuilderMovingPart : MonoBehaviour
	{
		// Token: 0x06004F3E RID: 20286 RVA: 0x00189F80 File Offset: 0x00188180
		private void Awake()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				builderAttachGridPlane.movesOnPlace = true;
				builderAttachGridPlane.movingPart = this;
			}
			this.initLocalPos = base.transform.localPosition;
			this.initLocalRotation = base.transform.localRotation;
		}

		// Token: 0x06004F3F RID: 20287 RVA: 0x00189FD4 File Offset: 0x001881D4
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06004F40 RID: 20288 RVA: 0x0018A009 File Offset: 0x00188209
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06004F41 RID: 20289 RVA: 0x0018A018 File Offset: 0x00188218
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x0018A043 File Offset: 0x00188243
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x0018A053 File Offset: 0x00188253
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06004F44 RID: 20292 RVA: 0x0018A073 File Offset: 0x00188273
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06004F45 RID: 20293 RVA: 0x0018A080 File Offset: 0x00188280
		public void ActivateAtNode(byte node, int timestamp)
		{
			float num = (float)node;
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (num >= this.startPercentage)
			{
				int num2 = (int)((num - this.startPercentage) * (float)this.CycleLengthMs());
				int num3 = timestamp - num2;
				if (flag)
				{
					num3 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = num3;
			}
			else
			{
				int num4 = (int)((num + 2f - this.startPercentage) * (float)this.CycleLengthMs());
				if (flag)
				{
					num4 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = timestamp - num4;
			}
			this.SetMoving(true);
		}

		// Token: 0x06004F46 RID: 20294 RVA: 0x0018A138 File Offset: 0x00188338
		public int GetTimeOffsetMS()
		{
			int num = PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp;
			uint num2 = (uint)(this.CycleLengthMs() * 2L);
			return num % (int)num2;
		}

		// Token: 0x06004F47 RID: 20295 RVA: 0x0018A164 File Offset: 0x00188364
		public byte GetNearestNode()
		{
			int num = Mathf.RoundToInt(this.currT * (float)BuilderMovingPart.NUM_PAUSE_NODES);
			if (!this.IsEvenCycle())
			{
				num += BuilderMovingPart.NUM_PAUSE_NODES;
			}
			return (byte)num;
		}

		// Token: 0x06004F48 RID: 20296 RVA: 0x0018A196 File Offset: 0x00188396
		public byte GetStartNode()
		{
			return (byte)Mathf.RoundToInt(this.startPercentage * (float)BuilderMovingPart.NUM_PAUSE_NODES);
		}

		// Token: 0x06004F49 RID: 20297 RVA: 0x0018A1AC File Offset: 0x001883AC
		public void PauseMovement(byte node)
		{
			this.SetMoving(false);
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			float num = (float)node;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (this.reverseDirOnCycle)
			{
				num = (flag ? (1f - num) : num);
			}
			if (this.reverseDir)
			{
				num = 1f - num;
			}
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(num);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(num);
		}

		// Token: 0x06004F4A RID: 20298 RVA: 0x0018A244 File Offset: 0x00188444
		public void SetMoving(bool isMoving)
		{
			this.isMoving = isMoving;
			BuilderAttachGridPlane[] array = this.myGridPlanes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].isMoving = isMoving;
			}
			if (!isMoving)
			{
				this.ResetMovingGrid();
			}
		}

		// Token: 0x06004F4B RID: 20299 RVA: 0x0018A280 File Offset: 0x00188480
		public void InitMovingGrid()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / (this.velocity * this.myPiece.GetScale());
				this.cycleDuration = num + this.cycleDelay;
				float num2 = this.cycleDelay / this.cycleDuration;
				Vector2 vector = new Vector2(num2 / 2f, 0f);
				Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
				float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
				this.lerpAlpha = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(num2 / 2f, 0f, 0f, num3),
					new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
				});
			}
			else
			{
				this.cycleDuration = 1f / this.velocity;
			}
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x06004F4C RID: 20300 RVA: 0x0018A3F4 File Offset: 0x001885F4
		public void UpdateMovingGrid()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x06004F4D RID: 20301 RVA: 0x0018A444 File Offset: 0x00188644
		private Vector3 UpdatePointToPoint(float perc)
		{
			float t = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, t);
		}

		// Token: 0x06004F4E RID: 20302 RVA: 0x0018A47C File Offset: 0x0018867C
		private void UpdateRotation(float perc)
		{
			Quaternion localRotation = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = localRotation;
		}

		// Token: 0x06004F4F RID: 20303 RVA: 0x0018A4A7 File Offset: 0x001886A7
		private void ResetMovingGrid()
		{
			base.transform.SetLocalPositionAndRotation(this.initLocalPos, this.initLocalRotation);
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x0018A4C0 File Offset: 0x001886C0
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x06004F51 RID: 20305 RVA: 0x0018A538 File Offset: 0x00188738
		public bool IsAnchoredToTable()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				if (builderAttachGridPlane.attachIndex == builderAttachGridPlane.piece.attachIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004F52 RID: 20306 RVA: 0x0018A574 File Offset: 0x00188774
		public void OnPieceDestroy()
		{
			this.ResetMovingGrid();
		}

		// Token: 0x04005829 RID: 22569
		public BuilderPiece myPiece;

		// Token: 0x0400582A RID: 22570
		public BuilderAttachGridPlane[] myGridPlanes;

		// Token: 0x0400582B RID: 22571
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x0400582C RID: 22572
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x0400582D RID: 22573
		[SerializeField]
		private float velocity;

		// Token: 0x0400582E RID: 22574
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x0400582F RID: 22575
		[SerializeField]
		private bool reverseDir;

		// Token: 0x04005830 RID: 22576
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x04005831 RID: 22577
		[SerializeField]
		protected Transform startXf;

		// Token: 0x04005832 RID: 22578
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04005833 RID: 22579
		public static int NUM_PAUSE_NODES = 32;

		// Token: 0x04005834 RID: 22580
		private AnimationCurve lerpAlpha;

		// Token: 0x04005835 RID: 22581
		public bool isMoving;

		// Token: 0x04005836 RID: 22582
		private Quaternion initLocalRotation = Quaternion.identity;

		// Token: 0x04005837 RID: 22583
		private Vector3 initLocalPos = Vector3.zero;

		// Token: 0x04005838 RID: 22584
		private float cycleDuration;

		// Token: 0x04005839 RID: 22585
		private float distance;

		// Token: 0x0400583A RID: 22586
		private float currT;

		// Token: 0x0400583B RID: 22587
		private float percent;

		// Token: 0x0400583C RID: 22588
		private bool currForward;

		// Token: 0x0400583D RID: 22589
		private float dtSinceServerUpdate;

		// Token: 0x0400583E RID: 22590
		private int lastServerTimeStamp;

		// Token: 0x0400583F RID: 22591
		private float rotateStartAmt;

		// Token: 0x04005840 RID: 22592
		private float rotateAmt;

		// Token: 0x04005841 RID: 22593
		private uint startPercentageCycleOffset;

		// Token: 0x02000C86 RID: 3206
		public enum BuilderMovingPartType
		{
			// Token: 0x04005843 RID: 22595
			Translation,
			// Token: 0x04005844 RID: 22596
			Rotation
		}
	}
}
