using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F5E RID: 3934
	public class SnakeInCanHoldable : TransferrableObject
	{
		// Token: 0x0600616E RID: 24942 RVA: 0x001EFA56 File Offset: 0x001EDC56
		protected override void Awake()
		{
			base.Awake();
			this.topRigPosition = this.topRigObject.transform.position;
		}

		// Token: 0x0600616F RID: 24943 RVA: 0x001EFA74 File Offset: 0x001EDC74
		internal override void OnEnable()
		{
			base.OnEnable();
			this.disableObjectBeforeTrigger.SetActive(false);
			if (this.compressedPoint != null)
			{
				this.topRigObject.transform.position = this.compressedPoint.position;
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnEnableObject;
			}
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x001EFB6C File Offset: 0x001EDD6C
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnEnableObject;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006171 RID: 24945 RVA: 0x001EFBC4 File Offset: 0x001EDDC4
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return false;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					false
				});
			}
			this.EnableObjectLocal(false);
			return true;
		}

		// Token: 0x06006172 RID: 24946 RVA: 0x001EFC4C File Offset: 0x001EDE4C
		private void OnEnableObject(int sender, int target, object[] arg, PhotonMessageInfoWrapped info)
		{
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			if (arg.Length != 1 || !(arg[0] is bool))
			{
				return;
			}
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnEnableObject");
			if (!this.snakeInCanCallLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			bool enable = (bool)arg[0];
			this.EnableObjectLocal(enable);
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x001EFCB8 File Offset: 0x001EDEB8
		private void EnableObjectLocal(bool enable)
		{
			this.disableObjectBeforeTrigger.SetActive(enable);
			if (!enable)
			{
				if (this.compressedPoint != null)
				{
					this.topRigObject.transform.position = this.compressedPoint.position;
				}
				return;
			}
			if (this.stretchedPoint != null)
			{
				base.StartCoroutine(this.SmoothTransition());
				return;
			}
			this.topRigObject.transform.position = this.topRigPosition;
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x001EFD30 File Offset: 0x001EDF30
		private IEnumerator SmoothTransition()
		{
			while (Vector3.Distance(this.topRigObject.transform.position, this.stretchedPoint.position) > 0.01f)
			{
				this.topRigObject.transform.position = Vector3.MoveTowards(this.topRigObject.transform.position, this.stretchedPoint.position, this.jumpSpeed * Time.deltaTime);
				yield return null;
			}
			this.topRigObject.transform.position = this.stretchedPoint.position;
			yield break;
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x001EFD3F File Offset: 0x001EDF3F
		public void OnButtonPressed()
		{
			this.EnableObjectLocal(true);
		}

		// Token: 0x04006D89 RID: 28041
		[SerializeField]
		private float jumpSpeed;

		// Token: 0x04006D8A RID: 28042
		[SerializeField]
		private Transform stretchedPoint;

		// Token: 0x04006D8B RID: 28043
		[SerializeField]
		private Transform compressedPoint;

		// Token: 0x04006D8C RID: 28044
		[SerializeField]
		private GameObject topRigObject;

		// Token: 0x04006D8D RID: 28045
		[SerializeField]
		private GameObject disableObjectBeforeTrigger;

		// Token: 0x04006D8E RID: 28046
		private CallLimiter snakeInCanCallLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04006D8F RID: 28047
		private Vector3 topRigPosition;

		// Token: 0x04006D90 RID: 28048
		private Vector3 originalTopRigPosition;

		// Token: 0x04006D91 RID: 28049
		private RubberDuckEvents _events;
	}
}
