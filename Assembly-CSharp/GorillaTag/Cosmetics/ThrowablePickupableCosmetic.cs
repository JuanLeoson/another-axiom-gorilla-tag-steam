using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F2A RID: 3882
	public class ThrowablePickupableCosmetic : TransferrableObject
	{
		// Token: 0x06006041 RID: 24641 RVA: 0x001E911C File Offset: 0x001E731C
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
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
				this._events.Activate += this.OnReleaseEvent;
				this._events.Deactivate += this.OnReturnToDockEvent;
			}
		}

		// Token: 0x06006042 RID: 24642 RVA: 0x001E920C File Offset: 0x001E740C
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnReleaseEvent;
				this._events.Deactivate -= this.OnReturnToDockEvent;
				Object.Destroy(this._events);
				this._events = null;
			}
			if (this.pickupableVariant.enabled)
			{
				this.pickupableVariant.DelayedPickup();
			}
		}

		// Token: 0x06006043 RID: 24643 RVA: 0x001E929C File Offset: 0x001E749C
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return;
			}
			if (this.pickupableVariant.enabled)
			{
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[]
					{
						false
					});
				}
				base.transform.position = this.pickupableVariant.transform.position;
				base.transform.rotation = this.pickupableVariant.transform.rotation;
				this.pickupableVariant.Pickup();
				if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
				{
					this.canAutoGrabLeft = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InRightHand;
				}
				else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
				{
					this.canAutoGrabRight = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
			}
			UnityEvent onGrabFromDockPosition = this.OnGrabFromDockPosition;
			if (onGrabFromDockPosition != null)
			{
				onGrabFromDockPosition.Invoke();
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x06006044 RID: 24644 RVA: 0x001E93E0 File Offset: 0x001E75E0
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (!(VRRigCache.Instance.localRig.Rig == this.ownerRig))
			{
				return false;
			}
			Vector3 position = base.transform.position;
			Vector3 vector = (releasingHand == EquipmentInteractor.instance.leftHand) ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			bool flag = this.DistanceToDock() > this.returnToDockDistanceThreshold;
			if (PhotonNetwork.InRoom && this._events != null)
			{
				if (flag && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[]
					{
						true,
						position,
						vector,
						scale
					});
					this.OnReleaseEventLocal(position, vector, scale);
				}
				else if (!flag && this._events.Deactivate != null)
				{
					this._events.Deactivate.RaiseAll(Array.Empty<object>());
					UnityEvent onReturnToDockPosition = this.OnReturnToDockPosition;
					if (onReturnToDockPosition != null)
					{
						onReturnToDockPosition.Invoke();
					}
				}
			}
			else if (flag)
			{
				this.OnReleaseEventLocal(position, vector, scale);
			}
			else
			{
				UnityEvent onReturnToDockPosition2 = this.OnReturnToDockPosition;
				if (onReturnToDockPosition2 != null)
				{
					onReturnToDockPosition2.Invoke();
				}
				UnityEvent onReturnToDockPositionShared = this.OnReturnToDockPositionShared;
				if (onReturnToDockPositionShared != null)
				{
					onReturnToDockPositionShared.Invoke();
				}
			}
			return true;
		}

		// Token: 0x06006045 RID: 24645 RVA: 0x001E9568 File Offset: 0x001E7768
		private void OnReleaseEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReleaseEvent");
			if (!this.callLimiterRelease.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (obj is bool)
			{
				bool flag = (bool)obj;
				if (flag)
				{
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 inVel = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float value = (float)obj;
								Vector3 position = base.transform.position;
								Vector3 releaseVelocity = base.transform.forward;
								ref position.SetValueSafe(vector);
								if (this.ownerRig.IsPositionInRange(position, 20f))
								{
									releaseVelocity = this.ownerRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f);
									float playerScale = value.ClampSafe(0.01f, 1f);
									this.OnReleaseEventLocal(position, releaseVelocity, playerScale);
									return;
								}
								return;
							}
						}
					}
					return;
				}
				this.pickupableVariant.Pickup();
				return;
			}
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x001E9684 File Offset: 0x001E7884
		private void OnReturnToDockEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReturnToDockEvent");
			if (!this.callLimiterReturn.CheckCallTime(Time.time))
			{
				return;
			}
			UnityEvent onReturnToDockPositionShared = this.OnReturnToDockPositionShared;
			if (onReturnToDockPositionShared == null)
			{
				return;
			}
			onReturnToDockPositionShared.Invoke();
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x001E96DF File Offset: 0x001E78DF
		private void OnReleaseEventLocal(Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
			this.pickupableVariant.Release(this, startPosition, releaseVelocity, playerScale);
		}

		// Token: 0x06006048 RID: 24648 RVA: 0x001E96F0 File Offset: 0x001E78F0
		private float DistanceToDock()
		{
			float result = 0f;
			if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnChest)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.chestTransform.position, base.transform.position);
			}
			return result;
		}

		// Token: 0x04006B9E RID: 27550
		[SerializeField]
		private PickupableVariant pickupableVariant;

		// Token: 0x04006B9F RID: 27551
		[SerializeField]
		private float returnToDockDistanceThreshold = 0.7f;

		// Token: 0x04006BA0 RID: 27552
		public UnityEvent OnReturnToDockPosition;

		// Token: 0x04006BA1 RID: 27553
		public UnityEvent OnReturnToDockPositionShared;

		// Token: 0x04006BA2 RID: 27554
		public UnityEvent OnGrabFromDockPosition;

		// Token: 0x04006BA3 RID: 27555
		private RubberDuckEvents _events;

		// Token: 0x04006BA4 RID: 27556
		private CallLimiter callLimiterRelease = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04006BA5 RID: 27557
		private CallLimiter callLimiterReturn = new CallLimiter(10, 2f, 0.5f);
	}
}
