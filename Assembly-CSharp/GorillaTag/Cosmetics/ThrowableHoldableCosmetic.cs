using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Shared.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F29 RID: 3881
	public class ThrowableHoldableCosmetic : TransferrableObject
	{
		// Token: 0x06006036 RID: 24630 RVA: 0x001E8BC4 File Offset: 0x001E6DC4
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
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnThrowEvent;
			}
		}

		// Token: 0x06006037 RID: 24631 RVA: 0x001E8C88 File Offset: 0x001E6E88
		protected override void Awake()
		{
			base.Awake();
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
			if (this.alternativeProjectilePrefab != null)
			{
				this.alternativeProjectileHash = PoolUtils.GameObjHashCode(this.alternativeProjectilePrefab);
			}
			this.currentPorjectileHash = this.projectileHash;
			this.playersEffect = base.GetComponentInChildren<CosmeticEffectsOnPlayers>();
		}

		// Token: 0x06006038 RID: 24632 RVA: 0x001E8CE3 File Offset: 0x001E6EE3
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (!this.disableWhenThrown.gameObject.activeSelf)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x06006039 RID: 24633 RVA: 0x001E8D00 File Offset: 0x001E6F00
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
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Vector3 vector = (releasingHand == EquipmentInteractor.instance.leftHand) ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					position,
					rotation,
					vector,
					scale
				});
			}
			this.OnThrowLocal(position, rotation, vector, this.ownerRig);
			return true;
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x001E8E10 File Offset: 0x001E7010
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnThrowEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600603B RID: 24635 RVA: 0x001E8E65 File Offset: 0x001E7065
		public void UseAlternativeProjectile()
		{
			if (this.alternativeProjectilePrefab != null)
			{
				this.currentPorjectileHash = this.alternativeProjectileHash;
			}
		}

		// Token: 0x0600603C RID: 24636 RVA: 0x001E8E84 File Offset: 0x001E7084
		private void OnThrowEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 4)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnThrowEvent");
			if (this.firecrackerCallLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Quaternion)
					{
						Quaternion rotation = (Quaternion)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 vector2 = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float value = (float)obj;
								vector2 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 40f);
								value.ClampSafe(0.01f, 1f);
								if (!rotation.IsValid())
								{
									return;
								}
								float num = 10000f;
								if (!vector.IsValid(num) || !this.targetRig.IsPositionInRange(vector, 4f))
								{
									return;
								}
								this.OnThrowLocal(vector, rotation, vector2, this.ownerRig);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600603D RID: 24637 RVA: 0x001E8F94 File Offset: 0x001E7194
		private void OnThrowLocal(Vector3 startPos, Quaternion rotation, Vector3 velocity, VRRig ownerRig)
		{
			this.disableWhenThrown.SetActive(false);
			IProjectile component = ObjectPools.instance.Instantiate(this.currentPorjectileHash, true).GetComponent<IProjectile>();
			FirecrackerProjectile firecrackerProjectile = component as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				firecrackerProjectile.OnDetonationComplete.AddListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				firecrackerProjectile.OnDetonationStart.AddListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
			}
			else
			{
				FartBagThrowable fartBagThrowable = component as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated += this.HitComplete;
					fartBagThrowable.ParentTransferable = this;
				}
			}
			component.Launch(startPos, rotation, velocity, 1f, ownerRig, -1);
			this.currentPorjectileHash = this.projectileHash;
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x001E903E File Offset: 0x001E723E
		private void HitStart(FirecrackerProjectile firecracker, Vector3 contactPos)
		{
			if (firecracker == null)
			{
				return;
			}
			if (this.playersEffect == null)
			{
				return;
			}
			this.playersEffect.ApplyAllEffectsByDistance(contactPos);
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x001E9068 File Offset: 0x001E7268
		private void HitComplete(IProjectile projectile)
		{
			if (projectile == null)
			{
				return;
			}
			this.disableWhenThrown.SetActive(true);
			FirecrackerProjectile firecrackerProjectile = projectile as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				firecrackerProjectile.OnDetonationStart.RemoveListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
				firecrackerProjectile.OnDetonationComplete.RemoveListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				ObjectPools.instance.Destroy(firecrackerProjectile.gameObject);
				return;
			}
			FartBagThrowable fartBagThrowable = projectile as FartBagThrowable;
			if (fartBagThrowable != null)
			{
				fartBagThrowable.OnDeflated -= this.HitComplete;
				ObjectPools.instance.Destroy(fartBagThrowable.gameObject);
			}
		}

		// Token: 0x04006B95 RID: 27541
		[FormerlySerializedAs("firecrackerProjectilePrefab")]
		[SerializeField]
		private GameObject projectilePrefab;

		// Token: 0x04006B96 RID: 27542
		[SerializeField]
		private GameObject alternativeProjectilePrefab;

		// Token: 0x04006B97 RID: 27543
		[SerializeField]
		private GameObject disableWhenThrown;

		// Token: 0x04006B98 RID: 27544
		private CallLimiter firecrackerCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04006B99 RID: 27545
		private CosmeticEffectsOnPlayers playersEffect;

		// Token: 0x04006B9A RID: 27546
		private int projectileHash;

		// Token: 0x04006B9B RID: 27547
		private int alternativeProjectileHash;

		// Token: 0x04006B9C RID: 27548
		private int currentPorjectileHash;

		// Token: 0x04006B9D RID: 27549
		private RubberDuckEvents _events;
	}
}
