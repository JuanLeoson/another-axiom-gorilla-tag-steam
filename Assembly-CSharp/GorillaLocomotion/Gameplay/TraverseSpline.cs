using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E31 RID: 3633
	[NetworkBehaviourWeaved(1)]
	public class TraverseSpline : NetworkComponent
	{
		// Token: 0x06005A56 RID: 23126 RVA: 0x001C7A1D File Offset: 0x001C5C1D
		protected override void Awake()
		{
			base.Awake();
			this.progress = this.SplineProgressOffet % 1f;
		}

		// Token: 0x06005A57 RID: 23127 RVA: 0x001C7A38 File Offset: 0x001C5C38
		protected virtual void FixedUpdate()
		{
			if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f);
			}
			else
			{
				if (this.isHeldByLocalPlayer)
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
				}
				else
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime);
				}
				if (this.goingForward)
				{
					this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress > 1f)
					{
						if (this.mode == SplineWalkerMode.Once)
						{
							this.progress = 1f;
						}
						else if (this.mode == SplineWalkerMode.Loop)
						{
							this.progress %= 1f;
						}
						else
						{
							this.progress = 2f - this.progress;
							this.goingForward = false;
						}
					}
				}
				else
				{
					this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress < 0f)
					{
						this.progress = -this.progress;
						this.goingForward = true;
					}
				}
			}
			Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06005A58 RID: 23128 RVA: 0x001C7C01 File Offset: 0x001C5E01
		// (set) Token: 0x06005A59 RID: 23129 RVA: 0x001C7C27 File Offset: 0x001C5E27
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe float Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(float*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(float*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005A5A RID: 23130 RVA: 0x001C7C4E File Offset: 0x001C5E4E
		public override void WriteDataFusion()
		{
			this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
		}

		// Token: 0x06005A5B RID: 23131 RVA: 0x001C7C70 File Offset: 0x001C5E70
		public override void ReadDataFusion()
		{
			this.progressLerpEnd = this.Data;
			this.ReadDataShared();
		}

		// Token: 0x06005A5C RID: 23132 RVA: 0x001C7C84 File Offset: 0x001C5E84
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
		}

		// Token: 0x06005A5D RID: 23133 RVA: 0x001C7CAB File Offset: 0x001C5EAB
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			this.progressLerpEnd = (float)stream.ReceiveNext();
			this.ReadDataShared();
		}

		// Token: 0x06005A5E RID: 23134 RVA: 0x001C7CC4 File Offset: 0x001C5EC4
		private void ReadDataShared()
		{
			if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
			{
				this.progressLerpEnd = 1f;
			}
			else
			{
				this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
				if (this.progressLerpEnd > 1f)
				{
					this.progressLerpEnd = (float)((double)this.progressLerpEnd % 1.0);
				}
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x06005A5F RID: 23135 RVA: 0x001C7D7F File Offset: 0x001C5F7F
		protected float GetProgress()
		{
			return this.progress;
		}

		// Token: 0x06005A60 RID: 23136 RVA: 0x001C7D87 File Offset: 0x001C5F87
		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		// Token: 0x06005A62 RID: 23138 RVA: 0x001C7DDD File Offset: 0x001C5FDD
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x001C7DF5 File Offset: 0x001C5FF5
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400651B RID: 25883
		public BezierSpline spline;

		// Token: 0x0400651C RID: 25884
		public float duration = 30f;

		// Token: 0x0400651D RID: 25885
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x0400651E RID: 25886
		private float currentSpeedMultiplier;

		// Token: 0x0400651F RID: 25887
		public float acceleration = 1f;

		// Token: 0x04006520 RID: 25888
		public float deceleration = 1f;

		// Token: 0x04006521 RID: 25889
		private bool isHeldByLocalPlayer;

		// Token: 0x04006522 RID: 25890
		public bool lookForward = true;

		// Token: 0x04006523 RID: 25891
		public SplineWalkerMode mode;

		// Token: 0x04006524 RID: 25892
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x04006525 RID: 25893
		private float progress;

		// Token: 0x04006526 RID: 25894
		private float progressLerpStart;

		// Token: 0x04006527 RID: 25895
		private float progressLerpEnd;

		// Token: 0x04006528 RID: 25896
		private const float progressLerpDuration = 1f;

		// Token: 0x04006529 RID: 25897
		private float progressLerpStartTime;

		// Token: 0x0400652A RID: 25898
		private bool goingForward = true;

		// Token: 0x0400652B RID: 25899
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x0400652C RID: 25900
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private float _Data;
	}
}
