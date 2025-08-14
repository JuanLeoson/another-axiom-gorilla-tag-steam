using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C26 RID: 3110
	public class DecorativeItem : TransferrableObject
	{
		// Token: 0x06004C77 RID: 19575 RVA: 0x0016B9BC File Offset: 0x00169BBC
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x0017B376 File Offset: 0x00179576
		public override void OnSpawn(VRRig rig)
		{
			base.OnSpawn(rig);
			this.parent = base.transform.parent;
		}

		// Token: 0x06004C79 RID: 19577 RVA: 0x0016BA3F File Offset: 0x00169C3F
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06004C7A RID: 19578 RVA: 0x0017B390 File Offset: 0x00179590
		private new void OnStateChanged()
		{
			TransferrableObject.ItemStates itemState = this.itemState;
			if (itemState == TransferrableObject.ItemStates.State2)
			{
				this.SnapItem(this.reliableState.isSnapped, this.reliableState.snapPosition);
				return;
			}
			if (itemState != TransferrableObject.ItemStates.State3)
			{
				return;
			}
			this.Respawn(this.reliableState.respawnPosition, this.reliableState.respawnRotation);
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x0017B3E8 File Offset: 0x001795E8
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			DecorativeItem.DecorativeItemState itemState = (DecorativeItem.DecorativeItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x0017B427 File Offset: 0x00179627
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.itemState == TransferrableObject.ItemStates.State4 && this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine)
			{
				this.InvokeRespawn();
			}
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x0017B45E File Offset: 0x0017965E
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x06004C7E RID: 19582 RVA: 0x0017B46F File Offset: 0x0017966F
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			return true;
		}

		// Token: 0x06004C7F RID: 19583 RVA: 0x00158C70 File Offset: 0x00156E70
		private void SetWillTeleport()
		{
			this.worldShareableInstance.SetWillTeleport();
		}

		// Token: 0x06004C80 RID: 19584 RVA: 0x0017B490 File Offset: 0x00179690
		public void Respawn(Vector3 randPosition, Quaternion randRotation)
		{
			if (base.InHand())
			{
				return;
			}
			if (this.shatterVFX && this.ShouldPlayFX())
			{
				this.PlayVFX(this.shatterVFX);
			}
			this.itemState = TransferrableObject.ItemStates.State3;
			this.SetWillTeleport();
			Transform transform = base.transform;
			transform.position = randPosition;
			transform.rotation = randRotation;
			if (this.reliableState)
			{
				this.reliableState.respawnPosition = randPosition;
				this.reliableState.respawnRotation = randRotation;
			}
		}

		// Token: 0x06004C81 RID: 19585 RVA: 0x000AD03E File Offset: 0x000AB23E
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x0017B50C File Offset: 0x0017970C
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x0017B548 File Offset: 0x00179748
		public void SnapItem(bool snap, Vector3 attachPoint)
		{
			if (!this.reliableState)
			{
				return;
			}
			if (snap)
			{
				AttachPoint currentAttachPointByPosition = DecorativeItemsManager.Instance.getCurrentAttachPointByPosition(attachPoint);
				if (!currentAttachPointByPosition)
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				Transform attachPoint2 = currentAttachPointByPosition.attachPoint;
				if (!this.Reparent(attachPoint2))
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				this.itemState = TransferrableObject.ItemStates.State2;
				base.transform.parent.localPosition = Vector3.zero;
				base.transform.localPosition = Vector3.zero;
				this.reliableState.isSnapped = true;
				if (this.audioSource && this.snapAudio && this.ShouldPlayFX())
				{
					this.audioSource.GTPlayOneShot(this.snapAudio, 1f);
				}
				currentAttachPointByPosition.SetIsHook(true);
			}
			else
			{
				this.Reparent(null);
				this.reliableState.isSnapped = false;
			}
			this.reliableState.snapPosition = attachPoint;
		}

		// Token: 0x06004C84 RID: 19588 RVA: 0x0017B663 File Offset: 0x00179863
		private void InvokeRespawn()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				return;
			}
			UnityAction<DecorativeItem> unityAction = this.respawnItem;
			if (unityAction == null)
			{
				return;
			}
			unityAction(this);
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x0017B680 File Offset: 0x00179880
		private bool ShouldPlayFX()
		{
			return this.previousItemState == DecorativeItem.DecorativeItemState.isHeld || this.previousItemState == DecorativeItem.DecorativeItemState.dropped;
		}

		// Token: 0x06004C86 RID: 19590 RVA: 0x0017B697 File Offset: 0x00179897
		private void OnCollisionEnter(Collision other)
		{
			if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
			{
				return;
			}
			this.InvokeRespawn();
		}

		// Token: 0x04005594 RID: 21908
		public DecorativeItemReliableState reliableState;

		// Token: 0x04005595 RID: 21909
		public UnityAction<DecorativeItem> respawnItem;

		// Token: 0x04005596 RID: 21910
		public LayerMask breakItemLayerMask;

		// Token: 0x04005597 RID: 21911
		private Coroutine respawnTimer;

		// Token: 0x04005598 RID: 21912
		private Transform parent;

		// Token: 0x04005599 RID: 21913
		private float _respawnTimestamp;

		// Token: 0x0400559A RID: 21914
		private bool isSnapped;

		// Token: 0x0400559B RID: 21915
		private Vector3 currentPosition;

		// Token: 0x0400559C RID: 21916
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400559D RID: 21917
		public AudioClip snapAudio;

		// Token: 0x0400559E RID: 21918
		public GameObject shatterVFX;

		// Token: 0x0400559F RID: 21919
		private DecorativeItem.DecorativeItemState previousItemState = DecorativeItem.DecorativeItemState.dropped;

		// Token: 0x02000C27 RID: 3111
		private enum DecorativeItemState
		{
			// Token: 0x040055A1 RID: 21921
			isHeld = 1,
			// Token: 0x040055A2 RID: 21922
			dropped,
			// Token: 0x040055A3 RID: 21923
			snapped = 4,
			// Token: 0x040055A4 RID: 21924
			respawn = 8,
			// Token: 0x040055A5 RID: 21925
			none = 16
		}
	}
}
