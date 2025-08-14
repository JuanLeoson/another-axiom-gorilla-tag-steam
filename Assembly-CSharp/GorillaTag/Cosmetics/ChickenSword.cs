using System;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F31 RID: 3889
	public class ChickenSword : MonoBehaviour
	{
		// Token: 0x06006074 RID: 24692 RVA: 0x001EA85E File Offset: 0x001E8A5E
		private void Awake()
		{
			this.lastHitTime = float.PositiveInfinity;
			this.SwitchState(ChickenSword.SwordState.Ready);
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x001EA874 File Offset: 0x001E8A74
		internal void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? ((this.transferrableObject.myRig.creator != null) ? this.transferrableObject.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnReachedLastTransformationStep;
			}
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x001EA958 File Offset: 0x001E8B58
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnReachedLastTransformationStep;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x001EA9A8 File Offset: 0x001E8BA8
		private void Update()
		{
			ChickenSword.SwordState swordState = this.currentState;
			if (swordState != ChickenSword.SwordState.Ready)
			{
				if (swordState != ChickenSword.SwordState.Deflated)
				{
					return;
				}
				if (Time.time - this.lastHitTime > this.rechargeCooldown)
				{
					this.lastHitTime = float.PositiveInfinity;
					this.SwitchState(ChickenSword.SwordState.Ready);
					UnityEvent onRechargedShared = this.OnRechargedShared;
					if (onRechargedShared != null)
					{
						onRechargedShared.Invoke();
					}
					if (this.transferrableObject && this.transferrableObject.IsMyItem())
					{
						UnityEvent<bool> onRechargedLocal = this.OnRechargedLocal;
						if (onRechargedLocal == null)
						{
							return;
						}
						onRechargedLocal.Invoke(this.transferrableObject.InLeftHand());
					}
				}
			}
			else if (this.hitReceievd)
			{
				this.hitReceievd = false;
				this.lastHitTime = Time.time;
				this.SwitchState(ChickenSword.SwordState.Deflated);
				UnityEvent onDeflatedShared = this.OnDeflatedShared;
				if (onDeflatedShared != null)
				{
					onDeflatedShared.Invoke();
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					UnityEvent<bool> onDeflatedLocal = this.OnDeflatedLocal;
					if (onDeflatedLocal == null)
					{
						return;
					}
					onDeflatedLocal.Invoke(this.transferrableObject.InLeftHand());
					return;
				}
			}
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x001EAAA4 File Offset: 0x001E8CA4
		public void OnHitTargetSync(VRRig playerRig)
		{
			if (this.velocityTracker == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (this.currentState == ChickenSword.SwordState.Ready && averageVelocity.magnitude > this.hitVelocityThreshold)
			{
				this.hitReceievd = true;
				UnityEvent<VRRig> onHitTargetShared = this.OnHitTargetShared;
				if (onHitTargetShared != null)
				{
					onHitTargetShared.Invoke(playerRig);
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					bool arg = this.transferrableObject.InLeftHand();
					UnityEvent<bool> onHitTargetLocal = this.OnHitTargetLocal;
					if (onHitTargetLocal != null)
					{
						onHitTargetLocal.Invoke(arg);
					}
				}
				if (this.cosmeticSwapper != null && playerRig == GorillaTagger.Instance.offlineVRRig && this.cosmeticSwapper.GetCurrentStepIndex(playerRig) >= this.cosmeticSwapper.GetNumberOfSteps() && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseAll(Array.Empty<object>());
				}
			}
		}

		// Token: 0x06006079 RID: 24697 RVA: 0x001EABB8 File Offset: 0x001E8DB8
		private void OnReachedLastTransformationStep(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReachedLastTransformationStep");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer) && rigContainer.Rig.IsPositionInRange(base.transform.position, 6f))
			{
				UnityEvent<VRRig> onReachedLastTransformationStepShared = this.OnReachedLastTransformationStepShared;
				if (onReachedLastTransformationStepShared == null)
				{
					return;
				}
				onReachedLastTransformationStepShared.Invoke(rigContainer.Rig);
			}
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x001EAC40 File Offset: 0x001E8E40
		private void SwitchState(ChickenSword.SwordState newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04006BF2 RID: 27634
		[SerializeField]
		private float rechargeCooldown;

		// Token: 0x04006BF3 RID: 27635
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04006BF4 RID: 27636
		[SerializeField]
		private float hitVelocityThreshold;

		// Token: 0x04006BF5 RID: 27637
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04006BF6 RID: 27638
		[SerializeField]
		private CosmeticSwapper cosmeticSwapper;

		// Token: 0x04006BF7 RID: 27639
		[Space]
		[Space]
		public UnityEvent OnDeflatedShared;

		// Token: 0x04006BF8 RID: 27640
		public UnityEvent<bool> OnDeflatedLocal;

		// Token: 0x04006BF9 RID: 27641
		public UnityEvent OnRechargedShared;

		// Token: 0x04006BFA RID: 27642
		public UnityEvent<bool> OnRechargedLocal;

		// Token: 0x04006BFB RID: 27643
		public UnityEvent<VRRig> OnHitTargetShared;

		// Token: 0x04006BFC RID: 27644
		public UnityEvent<bool> OnHitTargetLocal;

		// Token: 0x04006BFD RID: 27645
		public UnityEvent<VRRig> OnReachedLastTransformationStepShared;

		// Token: 0x04006BFE RID: 27646
		private float lastHitTime;

		// Token: 0x04006BFF RID: 27647
		private ChickenSword.SwordState currentState;

		// Token: 0x04006C00 RID: 27648
		private bool hitReceievd;

		// Token: 0x04006C01 RID: 27649
		private RubberDuckEvents _events;

		// Token: 0x04006C02 RID: 27650
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x02000F32 RID: 3890
		private enum SwordState
		{
			// Token: 0x04006C04 RID: 27652
			Ready,
			// Token: 0x04006C05 RID: 27653
			Deflated
		}
	}
}
