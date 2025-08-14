using System;
using GorillaTagScripts.Builder;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BFB RID: 3067
	public class SurfaceMover : MonoBehaviour
	{
		// Token: 0x06004A97 RID: 19095 RVA: 0x0016A17D File Offset: 0x0016837D
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterSurfaceMover(this);
		}

		// Token: 0x06004A98 RID: 19096 RVA: 0x0016A196 File Offset: 0x00168396
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterSurfaceMover(this);
			}
		}

		// Token: 0x06004A99 RID: 19097 RVA: 0x0016A1B0 File Offset: 0x001683B0
		public void InitMovingSurface()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / this.velocity;
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

		// Token: 0x06004A9A RID: 19098 RVA: 0x0016A317 File Offset: 0x00168517
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06004A9B RID: 19099 RVA: 0x0016A340 File Offset: 0x00168540
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x0016A350 File Offset: 0x00168550
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x0016A37B File Offset: 0x0016857B
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x0016A38B File Offset: 0x0016858B
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06004A9F RID: 19103 RVA: 0x0016A3AB File Offset: 0x001685AB
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x0016A3B8 File Offset: 0x001685B8
		public void Move()
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
				return;
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x0016A400 File Offset: 0x00168600
		private Vector3 UpdatePointToPoint(float perc)
		{
			float t = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, t);
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x0016A438 File Offset: 0x00168638
		private void UpdateRotation(float perc)
		{
			Quaternion localRotation = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = localRotation;
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x0016A464 File Offset: 0x00168664
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

		// Token: 0x04005378 RID: 21368
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04005379 RID: 21369
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x0400537A RID: 21370
		[SerializeField]
		private float velocity;

		// Token: 0x0400537B RID: 21371
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x0400537C RID: 21372
		[SerializeField]
		private bool reverseDir;

		// Token: 0x0400537D RID: 21373
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x0400537E RID: 21374
		[SerializeField]
		protected Transform startXf;

		// Token: 0x0400537F RID: 21375
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04005380 RID: 21376
		private AnimationCurve lerpAlpha;

		// Token: 0x04005381 RID: 21377
		private float cycleDuration;

		// Token: 0x04005382 RID: 21378
		private float distance;

		// Token: 0x04005383 RID: 21379
		private float currT;

		// Token: 0x04005384 RID: 21380
		private float percent;

		// Token: 0x04005385 RID: 21381
		private bool currForward;

		// Token: 0x04005386 RID: 21382
		private float dtSinceServerUpdate;

		// Token: 0x04005387 RID: 21383
		private int lastServerTimeStamp;

		// Token: 0x04005388 RID: 21384
		private float rotateStartAmt;

		// Token: 0x04005389 RID: 21385
		private float rotateAmt;

		// Token: 0x0400538A RID: 21386
		private uint startPercentageCycleOffset;
	}
}
