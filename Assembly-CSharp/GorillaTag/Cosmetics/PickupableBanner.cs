using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F24 RID: 3876
	public class PickupableBanner : PickupableVariant
	{
		// Token: 0x0600600A RID: 24586 RVA: 0x0001C826 File Offset: 0x0001AA26
		private void Start()
		{
			base.enabled = false;
		}

		// Token: 0x0600600B RID: 24587 RVA: 0x001E8234 File Offset: 0x001E6434
		protected internal override void Pickup()
		{
			UnityEvent onPickup = this.OnPickup;
			if (onPickup != null)
			{
				onPickup.Invoke();
			}
			this.rb.isKinematic = true;
			this.rb.velocity = Vector3.zero;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.placedOnFloorTime = -1f;
			this.placedOnFloor = false;
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			base.enabled = false;
		}

		// Token: 0x0600600C RID: 24588 RVA: 0x001E8300 File Offset: 0x001E6500
		protected internal override void DelayedPickup()
		{
			base.StartCoroutine(this.DelayedPickup_Internal());
		}

		// Token: 0x0600600D RID: 24589 RVA: 0x001E830F File Offset: 0x001E650F
		private IEnumerator DelayedPickup_Internal()
		{
			yield return new WaitForSeconds(1f);
			this.Pickup();
			yield break;
		}

		// Token: 0x0600600E RID: 24590 RVA: 0x001E8320 File Offset: 0x001E6520
		protected internal override void Release(HoldableObject holdable, Vector3 startPosition, Vector3 velocity, float playerScale)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.velocity = velocity;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.scale = playerScale;
			base.enabled = true;
			this.transferrableParent = (this.holdableParent as TransferrableObject);
		}

		// Token: 0x0600600F RID: 24591 RVA: 0x001E83CC File Offset: 0x001E65CC
		private void FixedUpdate()
		{
			if (this.autoPickupAfterSeconds > 0f && this.placedOnFloor && Time.time - this.placedOnFloorTime > this.autoPickupAfterSeconds)
			{
				this.Pickup();
			}
			if (this.autoPickupDistance > 0f && this.transferrableParent != null && (this.transferrableParent.ownerRig.transform.position - base.transform.position).IsLongerThan(this.autoPickupDistance))
			{
				this.Pickup();
			}
			if (!this.placedOnFloor && base.enabled)
			{
				float maxDistance = this.RaycastCheckDist * this.scale;
				int value = this.floorLayerMask.value;
				for (int i = 0; i < this.RaycastChecksMax; i++)
				{
					Vector3 fibonacciSphereDirection = this.GetFibonacciSphereDirection(i, this.RaycastChecksMax);
					RaycastHit hitInfo;
					if (Physics.Raycast(this.raycastOrigin.position, fibonacciSphereDirection, out hitInfo, maxDistance, value, QueryTriggerInteraction.Ignore))
					{
						if (this.dontStickToWall)
						{
							if (Vector3.Angle(hitInfo.normal, Vector3.up) < 40f)
							{
								this.SettleBanner(hitInfo);
							}
						}
						else
						{
							this.SettleBanner(hitInfo);
						}
						UnityEvent onPlaced = this.OnPlaced;
						if (onPlaced != null)
						{
							onPlaced.Invoke();
						}
						this.placedOnFloor = true;
						this.placedOnFloorTime = Time.time;
						return;
					}
				}
			}
		}

		// Token: 0x06006010 RID: 24592 RVA: 0x001E8520 File Offset: 0x001E6720
		private void SettleBanner(RaycastHit hitInfo)
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
			Vector3 normal = hitInfo.normal;
			base.transform.position = hitInfo.point + normal * this.placementOffset;
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
			base.transform.rotation = rotation;
		}

		// Token: 0x06006011 RID: 24593 RVA: 0x001E859C File Offset: 0x001E679C
		private Vector3 GetFibonacciSphereDirection(int index, int total)
		{
			float f = Mathf.Acos(1f - 2f * ((float)index + 0.5f) / (float)total);
			float f2 = 3.1415927f * (1f + Mathf.Sqrt(5f)) * ((float)index + 0.5f);
			float x = Mathf.Sin(f) * Mathf.Cos(f2);
			float y = Mathf.Sin(f) * Mathf.Sin(f2);
			float z = Mathf.Cos(f);
			return new Vector3(x, y, z).normalized;
		}

		// Token: 0x04006B62 RID: 27490
		[Tooltip("The distance to check if the banner is close to the floor (from a raycast check).")]
		public float RaycastCheckDist = 0.2f;

		// Token: 0x04006B63 RID: 27491
		[Tooltip("How many checks should we attempt for a raycast.")]
		public int RaycastChecksMax = 12;

		// Token: 0x04006B64 RID: 27492
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04006B65 RID: 27493
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04006B66 RID: 27494
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04006B67 RID: 27495
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x04006B68 RID: 27496
		[SerializeField]
		private float placementOffset;

		// Token: 0x04006B69 RID: 27497
		[SerializeField]
		private Transform raycastOrigin;

		// Token: 0x04006B6A RID: 27498
		[SerializeField]
		private float autoPickupAfterSeconds;

		// Token: 0x04006B6B RID: 27499
		[SerializeField]
		private float autoPickupDistance;

		// Token: 0x04006B6C RID: 27500
		[SerializeField]
		private bool dontStickToWall;

		// Token: 0x04006B6D RID: 27501
		public UnityEvent OnPickup;

		// Token: 0x04006B6E RID: 27502
		public UnityEvent OnPlaced;

		// Token: 0x04006B6F RID: 27503
		private bool placedOnFloor;

		// Token: 0x04006B70 RID: 27504
		private float placedOnFloorTime = -1f;

		// Token: 0x04006B71 RID: 27505
		private VRRig cachedLocalRig;

		// Token: 0x04006B72 RID: 27506
		private HoldableObject holdableParent;

		// Token: 0x04006B73 RID: 27507
		private TransferrableObject transferrableParent;

		// Token: 0x04006B74 RID: 27508
		private double throwSettledTime = -1.0;

		// Token: 0x04006B75 RID: 27509
		private int landingSide;

		// Token: 0x04006B76 RID: 27510
		private float scale;
	}
}
