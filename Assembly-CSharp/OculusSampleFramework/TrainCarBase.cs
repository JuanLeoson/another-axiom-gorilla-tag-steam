using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D30 RID: 3376
	public abstract class TrainCarBase : MonoBehaviour
	{
		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x0600538E RID: 21390 RVA: 0x0019D725 File Offset: 0x0019B925
		// (set) Token: 0x0600538F RID: 21391 RVA: 0x0019D72D File Offset: 0x0019B92D
		public float Distance { get; protected set; }

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x06005390 RID: 21392 RVA: 0x0019D736 File Offset: 0x0019B936
		// (set) Token: 0x06005391 RID: 21393 RVA: 0x0019D73E File Offset: 0x0019B93E
		public float Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		// Token: 0x06005392 RID: 21394 RVA: 0x000023F5 File Offset: 0x000005F5
		protected virtual void Awake()
		{
		}

		// Token: 0x06005393 RID: 21395 RVA: 0x0019D748 File Offset: 0x0019B948
		public void UpdatePose(float distance, TrainCarBase train, Pose pose)
		{
			distance = (train._trainTrack.TrackLength + distance) % train._trainTrack.TrackLength;
			if (distance < 0f)
			{
				distance += train._trainTrack.TrackLength;
			}
			TrackSegment segment = train._trainTrack.GetSegment(distance);
			float distanceIntoSegment = distance - segment.StartDistance;
			segment.UpdatePose(distanceIntoSegment, pose);
		}

		// Token: 0x06005394 RID: 21396 RVA: 0x0019D7A8 File Offset: 0x0019B9A8
		protected void UpdateCarPosition()
		{
			this.UpdatePose(this.Distance + this._frontWheels.transform.localPosition.z * this.scale, this, this._frontPose);
			this.UpdatePose(this.Distance + this._rearWheels.transform.localPosition.z * this.scale, this, this._rearPose);
			Vector3 a = 0.5f * (this._frontPose.Position + this._rearPose.Position);
			Vector3 forward = this._frontPose.Position - this._rearPose.Position;
			base.transform.position = a + TrainCarBase.OFFSET;
			base.transform.rotation = Quaternion.LookRotation(forward, base.transform.up);
			this._frontWheels.transform.rotation = this._frontPose.Rotation;
			this._rearWheels.transform.rotation = this._rearPose.Rotation;
		}

		// Token: 0x06005395 RID: 21397 RVA: 0x0019D8C0 File Offset: 0x0019BAC0
		protected void RotateCarWheels()
		{
			float num = this.Distance / 0.027f % 6.2831855f;
			Transform[] individualWheels = this._individualWheels;
			for (int i = 0; i < individualWheels.Length; i++)
			{
				individualWheels[i].localRotation = Quaternion.AngleAxis(57.29578f * num, Vector3.right);
			}
		}

		// Token: 0x06005396 RID: 21398
		public abstract void UpdatePosition();

		// Token: 0x04005CD7 RID: 23767
		private static Vector3 OFFSET = new Vector3(0f, 0.0195f, 0f);

		// Token: 0x04005CD8 RID: 23768
		private const float WHEEL_RADIUS = 0.027f;

		// Token: 0x04005CD9 RID: 23769
		private const float TWO_PI = 6.2831855f;

		// Token: 0x04005CDA RID: 23770
		[SerializeField]
		protected Transform _frontWheels;

		// Token: 0x04005CDB RID: 23771
		[SerializeField]
		protected Transform _rearWheels;

		// Token: 0x04005CDC RID: 23772
		[SerializeField]
		protected TrainTrack _trainTrack;

		// Token: 0x04005CDD RID: 23773
		[SerializeField]
		protected Transform[] _individualWheels;

		// Token: 0x04005CDF RID: 23775
		protected float scale = 1f;

		// Token: 0x04005CE0 RID: 23776
		private Pose _frontPose = new Pose();

		// Token: 0x04005CE1 RID: 23777
		private Pose _rearPose = new Pose();
	}
}
