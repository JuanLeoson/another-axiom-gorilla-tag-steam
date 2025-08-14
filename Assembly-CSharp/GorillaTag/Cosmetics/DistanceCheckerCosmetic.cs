using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F3A RID: 3898
	public class DistanceCheckerCosmetic : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x0600609E RID: 24734 RVA: 0x001EB7A0 File Offset: 0x001E99A0
		// (set) Token: 0x0600609F RID: 24735 RVA: 0x001EB7A8 File Offset: 0x001E99A8
		public bool IsSpawned { get; set; }

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x060060A0 RID: 24736 RVA: 0x001EB7B1 File Offset: 0x001E99B1
		// (set) Token: 0x060060A1 RID: 24737 RVA: 0x001EB7B9 File Offset: 0x001E99B9
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060060A2 RID: 24738 RVA: 0x001EB7C2 File Offset: 0x001E99C2
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x060060A3 RID: 24739 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnDespawn()
		{
		}

		// Token: 0x060060A4 RID: 24740 RVA: 0x001EB7CB File Offset: 0x001E99CB
		private void OnEnable()
		{
			this.currentState = DistanceCheckerCosmetic.State.None;
			this.transferableObject = base.GetComponentInParent<TransferrableObject>();
			if (this.transferableObject != null)
			{
				this.ownerRig = this.transferableObject.ownerRig;
			}
			this.ResetClosestPlayer();
		}

		// Token: 0x060060A5 RID: 24741 RVA: 0x001EB805 File Offset: 0x001E9A05
		private void Update()
		{
			this.UpdateDistance();
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x001EB80D File Offset: 0x001E9A0D
		private bool IsBelowThreshold(Vector3 distance)
		{
			return distance.IsShorterThan(this.distanceThreshold);
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x001EB820 File Offset: 0x001E9A20
		private bool IsAboveThreshold(Vector3 distance)
		{
			return distance.IsLongerThan(this.distanceThreshold);
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x001EB834 File Offset: 0x001E9A34
		private void UpdateClosestPlayer(bool others = false)
		{
			if (!PhotonNetwork.InRoom)
			{
				this.ResetClosestPlayer();
				return;
			}
			VRRig y = this.currentClosestPlayer;
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!others || !(this.ownerRig != null) || !(vrrig == this.ownerRig))
				{
					Vector3 distance = vrrig.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(distance) && distance.sqrMagnitude < this.closestDistance.sqrMagnitude)
					{
						this.closestDistance = distance;
						this.currentClosestPlayer = vrrig;
					}
				}
			}
			if (this.currentClosestPlayer != null && this.currentClosestPlayer != y)
			{
				UnityEvent<VRRig, float> unityEvent = this.onClosestPlayerBelowThresholdChanged;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentClosestPlayer, this.closestDistance.magnitude);
			}
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x001EB950 File Offset: 0x001E9B50
		private void ResetClosestPlayer()
		{
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x001EB964 File Offset: 0x001E9B64
		private void UpdateDistance()
		{
			bool flag = true;
			switch (this.distanceTo)
			{
			case DistanceCheckerCosmetic.DistanceCondition.Owner:
			{
				Vector3 distance = this.myRig.transform.position - this.distanceFrom.position;
				if (this.IsBelowThreshold(distance))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
					return;
				}
				if (this.IsAboveThreshold(distance))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
				}
				break;
			}
			case DistanceCheckerCosmetic.DistanceCondition.Others:
				this.UpdateClosestPlayer(true);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!(this.ownerRig != null) || !(vrrig == this.ownerRig))
					{
						Vector3 distance2 = vrrig.transform.position - this.distanceFrom.position;
						if (this.IsBelowThreshold(distance2))
						{
							this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			case DistanceCheckerCosmetic.DistanceCondition.Everyone:
				this.UpdateClosestPlayer(false);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					Vector3 distance3 = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(distance3))
					{
						this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
						flag = false;
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x001EBB0C File Offset: 0x001E9D0C
		private void UpdateState(DistanceCheckerCosmetic.State newState)
		{
			if (this.currentState == newState)
			{
				return;
			}
			this.currentState = newState;
			if (this.currentState != DistanceCheckerCosmetic.State.AboveThreshold)
			{
				if (this.currentState == DistanceCheckerCosmetic.State.BelowThreshold)
				{
					UnityEvent unityEvent = this.onOneIsBelowThreshold;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
				return;
			}
			UnityEvent unityEvent2 = this.onAllAreAboveThreshold;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
		}

		// Token: 0x04006C39 RID: 27705
		[SerializeField]
		private Transform distanceFrom;

		// Token: 0x04006C3A RID: 27706
		[SerializeField]
		private DistanceCheckerCosmetic.DistanceCondition distanceTo;

		// Token: 0x04006C3B RID: 27707
		[Tooltip("Receive events when above or below this distance")]
		public float distanceThreshold;

		// Token: 0x04006C3C RID: 27708
		public UnityEvent onOneIsBelowThreshold;

		// Token: 0x04006C3D RID: 27709
		public UnityEvent onAllAreAboveThreshold;

		// Token: 0x04006C3E RID: 27710
		public UnityEvent<VRRig, float> onClosestPlayerBelowThresholdChanged;

		// Token: 0x04006C3F RID: 27711
		private VRRig myRig;

		// Token: 0x04006C40 RID: 27712
		private DistanceCheckerCosmetic.State currentState;

		// Token: 0x04006C41 RID: 27713
		private Vector3 closestDistance;

		// Token: 0x04006C42 RID: 27714
		private VRRig currentClosestPlayer;

		// Token: 0x04006C43 RID: 27715
		private VRRig ownerRig;

		// Token: 0x04006C44 RID: 27716
		private TransferrableObject transferableObject;

		// Token: 0x02000F3B RID: 3899
		private enum State
		{
			// Token: 0x04006C48 RID: 27720
			AboveThreshold,
			// Token: 0x04006C49 RID: 27721
			BelowThreshold,
			// Token: 0x04006C4A RID: 27722
			None
		}

		// Token: 0x02000F3C RID: 3900
		private enum DistanceCondition
		{
			// Token: 0x04006C4C RID: 27724
			None,
			// Token: 0x04006C4D RID: 27725
			Owner,
			// Token: 0x04006C4E RID: 27726
			Others,
			// Token: 0x04006C4F RID: 27727
			Everyone
		}
	}
}
