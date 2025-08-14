using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E26 RID: 3622
	public class GorillaZipline : MonoBehaviour
	{
		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06005A0B RID: 23051 RVA: 0x001C63AE File Offset: 0x001C45AE
		// (set) Token: 0x06005A0C RID: 23052 RVA: 0x001C63B6 File Offset: 0x001C45B6
		public float currentSpeed { get; private set; }

		// Token: 0x06005A0D RID: 23053 RVA: 0x001C63C0 File Offset: 0x001C45C0
		protected void FindTFromDistance(ref float t, float distance, int steps = 1000)
		{
			float num = distance / (float)steps;
			Vector3 b = this.spline.GetPointLocal(t);
			float num2 = 0f;
			for (int i = 0; i < 1000; i++)
			{
				t += num;
				if (t >= 1f || t <= 0f)
				{
					break;
				}
				Vector3 pointLocal = this.spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, b);
				if (num2 >= Mathf.Abs(distance))
				{
					break;
				}
				b = pointLocal;
			}
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x001C6434 File Offset: 0x001C4634
		private float FindSlideHelperSpot(Vector3 grabPoint)
		{
			int i = 0;
			int num = 200;
			float num2 = 0.001f;
			float num3 = 1f / (float)num;
			float3 y = base.transform.InverseTransformPoint(grabPoint);
			float result = 0f;
			float num4 = float.PositiveInfinity;
			while (i < num)
			{
				float num5 = math.distancesq(this.spline.GetPointLocal(num2), y);
				if (num5 < num4)
				{
					num4 = num5;
					result = num2;
				}
				num2 += num3;
				i++;
			}
			return result;
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x001C64B0 File Offset: 0x001C46B0
		protected virtual void Start()
		{
			this.spline = base.GetComponent<BezierSpline>();
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06005A10 RID: 23056 RVA: 0x001C64E5 File Offset: 0x001C46E5
		private void OnDestroy()
		{
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06005A11 RID: 23057 RVA: 0x001C650E File Offset: 0x001C470E
		public Vector3 GetCurrentDirection()
		{
			return this.spline.GetDirection(this.currentT);
		}

		// Token: 0x06005A12 RID: 23058 RVA: 0x001C6524 File Offset: 0x001C4724
		protected void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
		{
			bool flag = this.currentClimber == null;
			this.currentClimber = hand;
			if (climbRef)
			{
				this.climbOffsetHelper.SetParent(climbRef.transform);
				this.climbOffsetHelper.position = hand.transform.position;
				this.climbOffsetHelper.localPosition = new Vector3(0f, 0f, this.climbOffsetHelper.localPosition.z);
			}
			this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
			this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
			if (flag)
			{
				Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
				float num = Vector3.Dot(averagedVelocity.normalized, this.spline.GetDirection(this.currentT));
				this.currentSpeed = averagedVelocity.magnitude * num * this.currentInheritVelocityMulti;
			}
		}

		// Token: 0x06005A13 RID: 23059 RVA: 0x001C6618 File Offset: 0x001C4818
		private void Update()
		{
			if (this.currentClimber)
			{
				Vector3 direction = this.spline.GetDirection(this.currentT);
				float num = Physics.gravity.y * direction.y * this.settings.gravityMulti;
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, num * Time.deltaTime);
				float num2 = MathUtils.Linear(this.currentSpeed, 0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction);
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, num2 * Time.deltaTime);
				this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
				this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
				float value = Mathf.Abs(this.currentSpeed);
				this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime, 1000);
				this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
				if (!this.audioSlide.gameObject.activeSelf)
				{
					this.audioSlide.gameObject.SetActive(true);
				}
				this.audioSlide.volume = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
				this.audioSlide.pitch = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
				if (!this.audioSlide.isPlaying)
				{
					this.audioSlide.GTPlay();
				}
				float num3 = MathUtils.Linear(value, 0f, this.settings.maxSpeed, -0.1f, 0.75f);
				if (num3 > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, num3, Time.deltaTime);
				}
				if (!this.spline.Loop)
				{
					if (this.currentT >= 1f || this.currentT <= 0f)
					{
						this.currentClimber.ForceStopClimbing(false, true);
					}
				}
				else if (this.currentT >= 1f)
				{
					this.currentT = 0f;
				}
				else if (this.currentT <= 0f)
				{
					this.currentT = 1f;
				}
				if (!this.slideHelper.isBeingClimbed)
				{
					this.Stop();
				}
			}
			if (this.currentInheritVelocityMulti < 1f)
			{
				this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
				this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
			}
		}

		// Token: 0x06005A14 RID: 23060 RVA: 0x001C6906 File Offset: 0x001C4B06
		private void Stop()
		{
			this.currentClimber = null;
			this.audioSlide.GTStop();
			this.audioSlide.gameObject.SetActive(false);
			this.currentInheritVelocityMulti = 0.55f;
			this.currentSpeed = 0f;
		}

		// Token: 0x040064CD RID: 25805
		[SerializeField]
		protected Transform segmentsRoot;

		// Token: 0x040064CE RID: 25806
		[SerializeField]
		protected GameObject segmentPrefab;

		// Token: 0x040064CF RID: 25807
		[SerializeField]
		protected GorillaClimbable slideHelper;

		// Token: 0x040064D0 RID: 25808
		[SerializeField]
		private AudioSource audioSlide;

		// Token: 0x040064D1 RID: 25809
		protected BezierSpline spline;

		// Token: 0x040064D2 RID: 25810
		[SerializeField]
		private Transform climbOffsetHelper;

		// Token: 0x040064D3 RID: 25811
		[SerializeField]
		private GorillaZiplineSettings settings;

		// Token: 0x040064D5 RID: 25813
		[SerializeField]
		protected float ziplineDistance = 15f;

		// Token: 0x040064D6 RID: 25814
		[SerializeField]
		protected float segmentDistance = 0.9f;

		// Token: 0x040064D7 RID: 25815
		private GorillaHandClimber currentClimber;

		// Token: 0x040064D8 RID: 25816
		private float currentT;

		// Token: 0x040064D9 RID: 25817
		private const float inheritVelocityRechargeRate = 0.2f;

		// Token: 0x040064DA RID: 25818
		private const float inheritVelocityValueOnRelease = 0.55f;

		// Token: 0x040064DB RID: 25819
		private float currentInheritVelocityMulti = 1f;
	}
}
