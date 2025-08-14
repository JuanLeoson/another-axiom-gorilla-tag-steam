using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F4E RID: 3918
	public class OnCollisionEventsCosmetic : MonoBehaviour
	{
		// Token: 0x0600610D RID: 24845 RVA: 0x001EE2E1 File Offset: 0x001EC4E1
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x001EE2FE File Offset: 0x001EC4FE
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x0600610F RID: 24847 RVA: 0x001EE318 File Offset: 0x001EC518
		private void FireEvents(Collision other, OnCollisionEventsCosmetic.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			if (listener.collisionTagsList.Count > 0 && !this.IsTagValid(other.gameObject, listener))
			{
				return;
			}
			if (!this.IsInCollisionLayer(other.gameObject, listener))
			{
				return;
			}
			bool arg = this.parentTransferable && this.parentTransferable.InLeftHand();
			UnityEvent<bool, Collision> listenerComponent = listener.listenerComponent;
			if (listenerComponent != null)
			{
				listenerComponent.Invoke(arg, other);
			}
			UnityEvent<Vector3> listenerComponentContactPoint = listener.listenerComponentContactPoint;
			if (listenerComponentContactPoint == null)
			{
				return;
			}
			listenerComponentContactPoint.Invoke(other.contacts[0].point);
		}

		// Token: 0x06006110 RID: 24848 RVA: 0x001EE3D7 File Offset: 0x001EC5D7
		private bool IsTagValid(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return listener.collisionTagsList.Contains(obj.tag);
		}

		// Token: 0x06006111 RID: 24849 RVA: 0x001EE3EA File Offset: 0x001EC5EA
		private bool IsInCollisionLayer(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return (listener.collisionLayerMask.value & 1 << obj.layer) != 0;
		}

		// Token: 0x06006112 RID: 24850 RVA: 0x001EE408 File Offset: 0x001EC608
		private void OnCollisionEnter(Collision other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionEnter)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x001EE444 File Offset: 0x001EC644
		private void OnCollisionExit(Collision other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionExit)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x06006114 RID: 24852 RVA: 0x001EE480 File Offset: 0x001EC680
		private void OnCollisionStay(Collision other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionStay)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x04006CFE RID: 27902
		public OnCollisionEventsCosmetic.Listener[] eventListeners = new OnCollisionEventsCosmetic.Listener[0];

		// Token: 0x04006CFF RID: 27903
		private VRRig _rig;

		// Token: 0x04006D00 RID: 27904
		private TransferrableObject parentTransferable;

		// Token: 0x02000F4F RID: 3919
		[Serializable]
		public class Listener
		{
			// Token: 0x04006D01 RID: 27905
			public LayerMask collisionLayerMask;

			// Token: 0x04006D02 RID: 27906
			public List<string> collisionTagsList = new List<string>();

			// Token: 0x04006D03 RID: 27907
			public OnCollisionEventsCosmetic.EventType eventType;

			// Token: 0x04006D04 RID: 27908
			public UnityEvent<bool, Collision> listenerComponent;

			// Token: 0x04006D05 RID: 27909
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x04006D06 RID: 27910
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04006D07 RID: 27911
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;
		}

		// Token: 0x02000F50 RID: 3920
		public enum EventType
		{
			// Token: 0x04006D09 RID: 27913
			CollisionEnter,
			// Token: 0x04006D0A RID: 27914
			CollisionStay,
			// Token: 0x04006D0B RID: 27915
			CollisionExit
		}
	}
}
