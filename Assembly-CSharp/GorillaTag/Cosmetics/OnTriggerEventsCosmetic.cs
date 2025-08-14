using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F51 RID: 3921
	public class OnTriggerEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06006117 RID: 24855 RVA: 0x001EE4EF File Offset: 0x001EC6EF
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x06006118 RID: 24856 RVA: 0x001EE50C File Offset: 0x001EC70C
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myCollider = base.GetComponentInParent<Collider>();
		}

		// Token: 0x06006119 RID: 24857 RVA: 0x001EE534 File Offset: 0x001EC734
		private void FireEvents(Collider other, OnTriggerEventsCosmetic.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			if (listener.triggerTagsList.Count > 0 && !this.IsTagValid(other.gameObject, listener))
			{
				return;
			}
			if ((1 << other.gameObject.layer & listener.triggerLayerMask) == 0)
			{
				return;
			}
			bool arg = this.parentTransferable && this.parentTransferable.InLeftHand();
			UnityEvent<bool, Collider> listenerComponent = listener.listenerComponent;
			if (listenerComponent != null)
			{
				listenerComponent.Invoke(arg, other);
			}
			Vector3 center = this.myCollider.bounds.center;
			Vector3 arg2 = other.ClosestPoint(center);
			UnityEvent<Vector3> listenerComponentContactPoint = listener.listenerComponentContactPoint;
			if (listenerComponentContactPoint != null)
			{
				listenerComponentContactPoint.Invoke(arg2);
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				UnityEvent<VRRig> onTriggeredVRRig = listener.onTriggeredVRRig;
				if (onTriggeredVRRig == null)
				{
					return;
				}
				onTriggeredVRRig.Invoke(componentInParent);
			}
		}

		// Token: 0x0600611A RID: 24858 RVA: 0x001EE631 File Offset: 0x001EC831
		private bool IsTagValid(GameObject obj, OnTriggerEventsCosmetic.Listener listener)
		{
			return listener.triggerTagsList.Contains(obj.tag);
		}

		// Token: 0x0600611B RID: 24859 RVA: 0x001EE644 File Offset: 0x001EC844
		private void OnTriggerEnter(Collider other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnTriggerEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerEnter)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x0600611C RID: 24860 RVA: 0x001EE680 File Offset: 0x001EC880
		private void OnTriggerExit(Collider other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnTriggerEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerExit)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x0600611D RID: 24861 RVA: 0x001EE6BC File Offset: 0x001EC8BC
		private void OnTriggerStay(Collider other)
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				OnTriggerEventsCosmetic.Listener listener = this.eventListeners[i];
				if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerStay)
				{
					this.FireEvents(other, listener);
				}
			}
		}

		// Token: 0x04006D0C RID: 27916
		public OnTriggerEventsCosmetic.Listener[] eventListeners = new OnTriggerEventsCosmetic.Listener[0];

		// Token: 0x04006D0D RID: 27917
		private Collider myCollider;

		// Token: 0x04006D0E RID: 27918
		private VRRig _rig;

		// Token: 0x04006D0F RID: 27919
		private TransferrableObject parentTransferable;

		// Token: 0x02000F52 RID: 3922
		[Serializable]
		public class Listener
		{
			// Token: 0x04006D10 RID: 27920
			public LayerMask triggerLayerMask;

			// Token: 0x04006D11 RID: 27921
			public List<string> triggerTagsList = new List<string>();

			// Token: 0x04006D12 RID: 27922
			public OnTriggerEventsCosmetic.EventType eventType;

			// Token: 0x04006D13 RID: 27923
			public UnityEvent<bool, Collider> listenerComponent;

			// Token: 0x04006D14 RID: 27924
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x04006D15 RID: 27925
			public UnityEvent<VRRig> onTriggeredVRRig;

			// Token: 0x04006D16 RID: 27926
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04006D17 RID: 27927
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;
		}

		// Token: 0x02000F53 RID: 3923
		public enum EventType
		{
			// Token: 0x04006D19 RID: 27929
			TriggerEnter,
			// Token: 0x04006D1A RID: 27930
			TriggerStay,
			// Token: 0x04006D1B RID: 27931
			TriggerExit
		}
	}
}
