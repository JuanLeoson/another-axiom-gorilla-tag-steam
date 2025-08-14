using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F27 RID: 3879
	[RequireComponent(typeof(TransferrableObject))]
	public class SeedPacketHoldable : MonoBehaviour
	{
		// Token: 0x0600601D RID: 24605 RVA: 0x001E86BE File Offset: 0x001E68BE
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.flowerEffectHash = PoolUtils.GameObjHashCode(this.flowerEffectPrefab);
		}

		// Token: 0x0600601E RID: 24606 RVA: 0x001E86E0 File Offset: 0x001E68E0
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
				this._events.Activate += this.SyncTriggerEffect;
			}
		}

		// Token: 0x0600601F RID: 24607 RVA: 0x001E87A8 File Offset: 0x001E69A8
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.SyncTriggerEffect;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006020 RID: 24608 RVA: 0x001E87F7 File Offset: 0x001E69F7
		private void OnDestroy()
		{
			this.pooledObjects.Clear();
		}

		// Token: 0x06006021 RID: 24609 RVA: 0x001E8804 File Offset: 0x001E6A04
		private void Update()
		{
			if (!this.transferrableObject.InHand())
			{
				return;
			}
			if (!this.isPouring && Vector3.Angle(base.transform.up, Vector3.down) <= this.pouringAngle)
			{
				this.StartPouring();
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, this.pouringRaycastDistance, this.raycastLayerMask))
				{
					this.hitPoint = raycastHit.point;
					base.Invoke("SpawnEffect", raycastHit.distance * this.placeEffectDelayMultiplier);
				}
			}
			if (this.isPouring && Time.time - this.pouringStartedTime >= this.cooldown)
			{
				this.isPouring = false;
			}
		}

		// Token: 0x06006022 RID: 24610 RVA: 0x001E88BD File Offset: 0x001E6ABD
		private void StartPouring()
		{
			if (this.particles)
			{
				this.particles.Play();
			}
			this.isPouring = true;
			this.pouringStartedTime = Time.time;
		}

		// Token: 0x06006023 RID: 24611 RVA: 0x001E88EC File Offset: 0x001E6AEC
		private void SpawnEffect()
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(this.flowerEffectHash, true);
			gameObject.transform.position = this.hitPoint;
			OnTriggerEventsHandlerCosmetic onTriggerEventsHandlerCosmetic;
			if (gameObject.TryGetComponent<OnTriggerEventsHandlerCosmetic>(out onTriggerEventsHandlerCosmetic))
			{
				this.pooledObjects.Add(onTriggerEventsHandlerCosmetic);
				onTriggerEventsHandlerCosmetic.onTriggerEntered.AddListener(new UnityAction<OnTriggerEventsHandlerCosmetic>(this.SyncTriggerEffectForOthers));
			}
		}

		// Token: 0x06006024 RID: 24612 RVA: 0x001E8948 File Offset: 0x001E6B48
		private void SyncTriggerEffectForOthers(OnTriggerEventsHandlerCosmetic onTriggerEventsHandlerCosmeticTriggerEvent)
		{
			int num = this.pooledObjects.IndexOf(onTriggerEventsHandlerCosmeticTriggerEvent);
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					num
				});
			}
		}

		// Token: 0x06006025 RID: 24613 RVA: 0x001E89AC File Offset: 0x001E6BAC
		private void SyncTriggerEffect(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "SyncTriggerEffect");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			int num = (int)args[0];
			if (num < 0 && num >= this.pooledObjects.Count)
			{
				return;
			}
			this.pooledObjects[num].ToggleEffects();
		}

		// Token: 0x04006B7A RID: 27514
		[SerializeField]
		private float cooldown;

		// Token: 0x04006B7B RID: 27515
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04006B7C RID: 27516
		[SerializeField]
		private float pouringAngle;

		// Token: 0x04006B7D RID: 27517
		[SerializeField]
		private float pouringRaycastDistance = 5f;

		// Token: 0x04006B7E RID: 27518
		[SerializeField]
		private LayerMask raycastLayerMask;

		// Token: 0x04006B7F RID: 27519
		[SerializeField]
		private float placeEffectDelayMultiplier = 10f;

		// Token: 0x04006B80 RID: 27520
		[SerializeField]
		private GameObject flowerEffectPrefab;

		// Token: 0x04006B81 RID: 27521
		private List<OnTriggerEventsHandlerCosmetic> pooledObjects = new List<OnTriggerEventsHandlerCosmetic>();

		// Token: 0x04006B82 RID: 27522
		private CallLimiter callLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04006B83 RID: 27523
		private int flowerEffectHash;

		// Token: 0x04006B84 RID: 27524
		private Vector3 hitPoint;

		// Token: 0x04006B85 RID: 27525
		private TransferrableObject transferrableObject;

		// Token: 0x04006B86 RID: 27526
		private bool isPouring = true;

		// Token: 0x04006B87 RID: 27527
		private float pouringStartedTime;

		// Token: 0x04006B88 RID: 27528
		private RubberDuckEvents _events;
	}
}
