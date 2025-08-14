using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000E38 RID: 3640
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x06005A7E RID: 23166 RVA: 0x001C9504 File Offset: 0x001C7704
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x001C9514 File Offset: 0x001C7714
		private void Update()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				GorillaClimbable gorillaClimbable = this.potentialClimbables[i];
				if (gorillaClimbable == null || !gorillaClimbable.isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
				else if (gorillaClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && this.player.scale > 0.99f)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && !this.equipmentInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.builderPieceInteractor.GetIsHolding(this.xrNode) && closestClimbable != this.dontReclimbLast && !this.player.inOverlay)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
						return;
					}
					this.player.BeginClimbing(closestClimbable, this, null);
					return;
				}
			}
			else if (grabRelease && this.canRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x001C96AB File Offset: 0x001C78AB
		public void SetCanRelease(bool canRelease)
		{
			this.canRelease = canRelease;
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x001C96B4 File Offset: 0x001C78B4
		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			Bounds bounds = this.col.bounds;
			float num = 0.15f;
			GorillaClimbable result = null;
			foreach (GorillaClimbable gorillaClimbable in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable.colliderCache)
				{
					if (!bounds.Intersects(gorillaClimbable.colliderCache.bounds))
					{
						continue;
					}
					Vector3 b = gorillaClimbable.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, b);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable.transform.position);
				}
				if (num2 < num)
				{
					result = gorillaClimbable;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x001C97B4 File Offset: 0x001C79B4
		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Add(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Add(item2);
			}
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x001C97F0 File Offset: 0x001C79F0
		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Remove(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Remove(item2);
			}
		}

		// Token: 0x06005A84 RID: 23172 RVA: 0x001C982C File Offset: 0x001C7A2C
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x04006558 RID: 25944
		[SerializeField]
		private GTPlayer player;

		// Token: 0x04006559 RID: 25945
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x0400655A RID: 25946
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x0400655B RID: 25947
		[Header("Non-hand input should have the component disabled")]
		public XRNode xrNode = XRNode.LeftHand;

		// Token: 0x0400655C RID: 25948
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x0400655D RID: 25949
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x0400655E RID: 25950
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x0400655F RID: 25951
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04006560 RID: 25952
		public Transform handRoot;

		// Token: 0x04006561 RID: 25953
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x04006562 RID: 25954
		private const float DIST_FOR_GRAB = 0.15f;

		// Token: 0x04006563 RID: 25955
		private Collider col;

		// Token: 0x04006564 RID: 25956
		private bool canRelease = true;
	}
}
