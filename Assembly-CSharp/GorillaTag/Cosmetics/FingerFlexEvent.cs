using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F42 RID: 3906
	public class FingerFlexEvent : MonoBehaviour
	{
		// Token: 0x060060CD RID: 24781 RVA: 0x001ED3D3 File Offset: 0x001EB5D3
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x060060CE RID: 24782 RVA: 0x001ED3ED File Offset: 0x001EB5ED
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x060060CF RID: 24783 RVA: 0x001ED40C File Offset: 0x001EB60C
		private void Update()
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				FingerFlexEvent.Listener listener = this.eventListeners[i];
				this.FireEvents(listener);
			}
		}

		// Token: 0x060060D0 RID: 24784 RVA: 0x001ED43C File Offset: 0x001EB63C
		private void FireEvents(FingerFlexEvent.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (!this.ignoreTransferable && this.parentTransferable && !this.parentTransferable.InHand() && listener.eventType == FingerFlexEvent.EventType.OnFingerReleased)
			{
				if (listener.fingerRightLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(false, 0f);
					}
					listener.fingerRightLastValue = 0f;
				}
				if (listener.fingerLeftLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(true, 0f);
					}
					listener.fingerLeftLastValue = 0f;
				}
			}
			if (!this.ignoreTransferable && this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			switch (this.fingerType)
			{
			case FingerFlexEvent.FingerType.Thumb:
			{
				float calcT = this._rig.leftThumb.calcT;
				float calcT2 = this._rig.rightThumb.calcT;
				this.FireEvents(listener, calcT, calcT2);
				return;
			}
			case FingerFlexEvent.FingerType.Index:
			{
				float calcT3 = this._rig.leftIndex.calcT;
				float calcT4 = this._rig.rightIndex.calcT;
				this.FireEvents(listener, calcT3, calcT4);
				return;
			}
			case FingerFlexEvent.FingerType.Middle:
			{
				float calcT5 = this._rig.leftMiddle.calcT;
				float calcT6 = this._rig.rightMiddle.calcT;
				this.FireEvents(listener, calcT5, calcT6);
				return;
			}
			case FingerFlexEvent.FingerType.IndexAndMiddleMin:
			{
				float leftFinger = Mathf.Min(this._rig.leftIndex.calcT, this._rig.leftMiddle.calcT);
				float rightFinger = Mathf.Min(this._rig.rightIndex.calcT, this._rig.rightMiddle.calcT);
				this.FireEvents(listener, leftFinger, rightFinger);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x001ED614 File Offset: 0x001EB814
		private void FireEvents(FingerFlexEvent.Listener listener, float leftFinger, float rightFinger)
		{
			if ((this.ignoreTransferable && listener.checkLeftHand) || (this.parentTransferable && this.FingerFlexValidation(true)))
			{
				this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
				return;
			}
			if ((this.ignoreTransferable && !listener.checkLeftHand) || (this.parentTransferable && this.FingerFlexValidation(false)))
			{
				this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
				return;
			}
			this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
			this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x001ED6AC File Offset: 0x001EB8AC
		private void CheckFingerValue(FingerFlexEvent.Listener listener, float fingerValue, bool isLeft, ref float lastValue)
		{
			if (fingerValue > listener.fingerFlexValue)
			{
				listener.frameCounter++;
			}
			switch (listener.eventType)
			{
			case FingerFlexEvent.EventType.OnFingerFlexed:
				if (fingerValue > listener.fingerFlexValue && lastValue < listener.fingerFlexValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(isLeft, fingerValue);
					}
				}
				break;
			case FingerFlexEvent.EventType.OnFingerReleased:
				if (fingerValue <= listener.fingerReleaseValue && lastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			case FingerFlexEvent.EventType.OnFingerFlexStayed:
				if (fingerValue > listener.fingerFlexValue && lastValue >= listener.fingerFlexValue && listener.frameCounter % listener.frameInterval == 0)
				{
					UnityEvent<bool, float> listenerComponent3 = listener.listenerComponent;
					if (listenerComponent3 != null)
					{
						listenerComponent3.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			}
			lastValue = fingerValue;
		}

		// Token: 0x060060D3 RID: 24787 RVA: 0x001ED78E File Offset: 0x001EB98E
		private bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.parentTransferable.InLeftHand() || isLeftHand) && (this.parentTransferable.InLeftHand() || !isLeftHand);
		}

		// Token: 0x04006CBA RID: 27834
		[SerializeField]
		public bool ignoreTransferable;

		// Token: 0x04006CBB RID: 27835
		[SerializeField]
		private FingerFlexEvent.FingerType fingerType = FingerFlexEvent.FingerType.Index;

		// Token: 0x04006CBC RID: 27836
		public FingerFlexEvent.Listener[] eventListeners = new FingerFlexEvent.Listener[0];

		// Token: 0x04006CBD RID: 27837
		private VRRig _rig;

		// Token: 0x04006CBE RID: 27838
		private TransferrableObject parentTransferable;

		// Token: 0x02000F43 RID: 3907
		[Serializable]
		public class Listener
		{
			// Token: 0x04006CBF RID: 27839
			public FingerFlexEvent.EventType eventType;

			// Token: 0x04006CC0 RID: 27840
			public UnityEvent<bool, float> listenerComponent;

			// Token: 0x04006CC1 RID: 27841
			public float fingerFlexValue = 0.75f;

			// Token: 0x04006CC2 RID: 27842
			public float fingerReleaseValue = 0.01f;

			// Token: 0x04006CC3 RID: 27843
			[Tooltip("How many frames should pass to fire a finger flex stayed event")]
			public int frameInterval = 20;

			// Token: 0x04006CC4 RID: 27844
			[Tooltip("This event will be fired for everyone in the room (synced) by default unless you uncheck this box so that it will be fired only for the local player.")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04006CC5 RID: 27845
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04006CC6 RID: 27846
			[Tooltip("Whether to check the left hand or the right hand, only works if \"ignoreTransferable\" is true.")]
			public bool checkLeftHand;

			// Token: 0x04006CC7 RID: 27847
			internal int frameCounter;

			// Token: 0x04006CC8 RID: 27848
			internal float fingerRightLastValue;

			// Token: 0x04006CC9 RID: 27849
			internal float fingerLeftLastValue;
		}

		// Token: 0x02000F44 RID: 3908
		public enum EventType
		{
			// Token: 0x04006CCB RID: 27851
			OnFingerFlexed,
			// Token: 0x04006CCC RID: 27852
			OnFingerReleased,
			// Token: 0x04006CCD RID: 27853
			OnFingerFlexStayed
		}

		// Token: 0x02000F45 RID: 3909
		private enum FingerType
		{
			// Token: 0x04006CCF RID: 27855
			Thumb,
			// Token: 0x04006CD0 RID: 27856
			Index,
			// Token: 0x04006CD1 RID: 27857
			Middle,
			// Token: 0x04006CD2 RID: 27858
			IndexAndMiddleMin
		}
	}
}
