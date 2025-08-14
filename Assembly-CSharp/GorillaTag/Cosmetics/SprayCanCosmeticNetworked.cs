using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F60 RID: 3936
	public class SprayCanCosmeticNetworked : MonoBehaviour
	{
		// Token: 0x0600617D RID: 24957 RVA: 0x001EFE44 File Offset: 0x001EE044
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnShakeEvent;
			}
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x001EFF0C File Offset: 0x001EE10C
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShakeEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600617F RID: 24959 RVA: 0x001EFF5C File Offset: 0x001EE15C
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShakeEvent");
			NetPlayer sender2 = info.Sender;
			VRRig myOnlineRig = this.transferrableObject.myOnlineRig;
			if (sender2 != ((myOnlineRig != null) ? myOnlineRig.creator : null))
			{
				return;
			}
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			if (flag)
			{
				UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
				if (handleOnShakeStart == null)
				{
					return;
				}
				handleOnShakeStart.Invoke();
				return;
			}
			else
			{
				UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
				if (handleOnShakeEnd == null)
				{
					return;
				}
				handleOnShakeEnd.Invoke();
				return;
			}
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x001EFFE8 File Offset: 0x001EE1E8
		public void OnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					true
				});
			}
			UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
			if (handleOnShakeStart == null)
			{
				return;
			}
			handleOnShakeStart.Invoke();
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x001F004C File Offset: 0x001EE24C
		public void OnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					false
				});
			}
			UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
			if (handleOnShakeEnd == null)
			{
				return;
			}
			handleOnShakeEnd.Invoke();
		}

		// Token: 0x04006D95 RID: 28053
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04006D96 RID: 28054
		private RubberDuckEvents _events;

		// Token: 0x04006D97 RID: 28055
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04006D98 RID: 28056
		public UnityEvent HandleOnShakeStart;

		// Token: 0x04006D99 RID: 28057
		public UnityEvent HandleOnShakeEnd;
	}
}
