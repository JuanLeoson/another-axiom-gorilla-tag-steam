using System;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E2A RID: 3626
	public class NoncontrollableBroomstick : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x06005A1D RID: 23069 RVA: 0x001C69D8 File Offset: 0x001C4BD8
		private void Start()
		{
			this.smoothRotationTrackingRateExp = Mathf.Exp(this.smoothRotationTrackingRate);
			this.progressPerFixedUpdate = Time.fixedDeltaTime / this.duration;
			this.progress = this.SplineProgressOffet;
			this.secondsToCycles = 1.0 / (double)this.duration;
			if (this.unitySpline != null)
			{
				this.nativeSpline = new NativeSpline(this.unitySpline.Spline, this.unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			}
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x001C6A68 File Offset: 0x001C4C68
		protected virtual void FixedUpdate()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this.secondsToCycles + (double)this.SplineProgressOffet;
				this.progress = (float)(num % 1.0);
			}
			else
			{
				this.progress = (this.progress + this.progressPerFixedUpdate) % 1f;
			}
			Quaternion a = Quaternion.identity;
			if (this.unitySpline != null)
			{
				float3 v;
				float3 @float;
				float3 float2;
				this.nativeSpline.Evaluate(this.progress, out v, out @float, out float2);
				base.transform.position = v;
				if (this.lookForward)
				{
					a = Quaternion.LookRotation(new Vector3(@float.x, @float.y, @float.z));
				}
			}
			else if (this.spline != null)
			{
				Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
				base.transform.position = point;
				if (this.lookForward)
				{
					a = Quaternion.LookRotation(this.spline.GetDirection(this.progress, this.constantVelocity));
				}
			}
			if (this.lookForward)
			{
				base.transform.rotation = Quaternion.Slerp(a, base.transform.rotation, Mathf.Exp(-this.smoothRotationTrackingRateExp * Time.deltaTime));
			}
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x0001D558 File Offset: 0x0001B758
		bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
		{
			return true;
		}

		// Token: 0x06005A20 RID: 23072 RVA: 0x001C6BB1 File Offset: 0x001C4DB1
		void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosition)
		{
			grabbedObject = base.transform;
			grabbedLocalPosition = base.transform.InverseTransformPoint(g.transform.position);
		}

		// Token: 0x06005A21 RID: 23073 RVA: 0x000023F5 File Offset: 0x000005F5
		void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
		{
		}

		// Token: 0x06005A22 RID: 23074 RVA: 0x001C6BD7 File Offset: 0x001C4DD7
		private void OnDestroy()
		{
			this.nativeSpline.Dispose();
		}

		// Token: 0x06005A23 RID: 23075 RVA: 0x001C6BE4 File Offset: 0x001C4DE4
		public bool MomentaryGrabOnly()
		{
			return this.momentaryGrabOnly;
		}

		// Token: 0x06005A25 RID: 23077 RVA: 0x000139A7 File Offset: 0x00011BA7
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x040064E6 RID: 25830
		public SplineContainer unitySpline;

		// Token: 0x040064E7 RID: 25831
		public BezierSpline spline;

		// Token: 0x040064E8 RID: 25832
		public float duration = 30f;

		// Token: 0x040064E9 RID: 25833
		public float smoothRotationTrackingRate = 0.5f;

		// Token: 0x040064EA RID: 25834
		public bool lookForward = true;

		// Token: 0x040064EB RID: 25835
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x040064EC RID: 25836
		private float progress;

		// Token: 0x040064ED RID: 25837
		private float smoothRotationTrackingRateExp;

		// Token: 0x040064EE RID: 25838
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x040064EF RID: 25839
		private float progressPerFixedUpdate;

		// Token: 0x040064F0 RID: 25840
		private double secondsToCycles;

		// Token: 0x040064F1 RID: 25841
		private NativeSpline nativeSpline;

		// Token: 0x040064F2 RID: 25842
		[SerializeField]
		private bool momentaryGrabOnly = true;
	}
}
