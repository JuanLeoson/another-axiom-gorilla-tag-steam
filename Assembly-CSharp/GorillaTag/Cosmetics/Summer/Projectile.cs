using System;
using System.Collections.Generic;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics.Summer
{
	// Token: 0x02000F6E RID: 3950
	public class Projectile : MonoBehaviour, IProjectile
	{
		// Token: 0x060061C2 RID: 25026 RVA: 0x001F12F0 File Offset: 0x001EF4F0
		protected void Awake()
		{
			this.rigidbody = base.GetComponentInChildren<Rigidbody>();
			this.impactEffectSpawned = false;
			this.forceComponent = base.GetComponent<ConstantForce>();
		}

		// Token: 0x060061C3 RID: 25027 RVA: 0x000023F5 File Offset: 0x000005F5
		protected void OnEnable()
		{
		}

		// Token: 0x060061C4 RID: 25028 RVA: 0x001F1314 File Offset: 0x001EF514
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep)
		{
			Transform transform = base.transform;
			transform.SetPositionAndRotation(startPosition, startRotation);
			transform.localScale = Vector3.one * ownerRig.scaleFactor;
			if (this.rigidbody != null)
			{
				this.rigidbody.velocity = velocity;
			}
			if (this.audioSource)
			{
				this.audioSource.GTPlayOneShot(this.launchAudio, 1f);
			}
			UnityEvent<float> unityEvent = this.onLaunchShared;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(chargeFrac);
		}

		// Token: 0x060061C5 RID: 25029 RVA: 0x001F1394 File Offset: 0x001EF594
		private bool IsTagValid(GameObject obj)
		{
			return this.collisionTags.Contains(obj.tag);
		}

		// Token: 0x060061C6 RID: 25030 RVA: 0x001F13A8 File Offset: 0x001EF5A8
		private void HandleImpact(GameObject hitObject, Vector3 hitPosition, Vector3 hitNormal)
		{
			if (this.impactEffectSpawned)
			{
				return;
			}
			if (this.collisionTags.Count > 0 && !this.IsTagValid(hitObject))
			{
				return;
			}
			if ((1 << hitObject.layer & this.collisionLayerMasks) == 0)
			{
				return;
			}
			this.SpawnImpactEffect(this.impactEffect, hitPosition, hitNormal);
			SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
			if (component != null && !component.playOnEnable)
			{
				component.Play();
			}
			this.impactEffectSpawned = true;
			if (this.destroyOnCollisionEnter)
			{
				if (this.destroyDelay > 0f)
				{
					base.Invoke("DestroyProjectile", this.destroyDelay);
					return;
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x060061C7 RID: 25031 RVA: 0x001F1458 File Offset: 0x001EF658
		private void GetColliderHitInfo(Collider other, out Vector3 position, out Vector3 normal)
		{
			Vector3 vector = Time.fixedDeltaTime * 2f * this.rigidbody.velocity;
			Vector3 origin = base.transform.position - vector;
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			other.Raycast(new Ray(origin, vector / magnitude), out raycastHit, 2f * magnitude);
			position = raycastHit.point;
			normal = raycastHit.normal;
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x001F14D4 File Offset: 0x001EF6D4
		private void OnCollisionEnter(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x001F1504 File Offset: 0x001EF704
		private void OnCollisionStay(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x001F1534 File Offset: 0x001EF734
		private void OnTriggerEnter(Collider other)
		{
			Vector3 hitPosition;
			Vector3 hitNormal;
			this.GetColliderHitInfo(other, out hitPosition, out hitNormal);
			this.HandleImpact(other.gameObject, hitPosition, hitNormal);
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x001F155C File Offset: 0x001EF75C
		private void OnTriggerStay(Collider other)
		{
			Transform transform = base.transform;
			this.HandleImpact(other.gameObject, transform.position, -transform.forward);
		}

		// Token: 0x060061CC RID: 25036 RVA: 0x001F1590 File Offset: 0x001EF790
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 position2 = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = position2;
			this.onImpactShared.Invoke();
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(position, normal);
			}
		}

		// Token: 0x060061CD RID: 25037 RVA: 0x001F15FC File Offset: 0x001EF7FC
		private void DestroyProjectile()
		{
			this.impactEffectSpawned = false;
			if (this.forceComponent)
			{
				this.forceComponent.enabled = false;
			}
			if (ObjectPools.instance.DoesPoolExist(base.gameObject))
			{
				ObjectPools.instance.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
		}

		// Token: 0x04006E0C RID: 28172
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006E0D RID: 28173
		[SerializeField]
		private GameObject impactEffect;

		// Token: 0x04006E0E RID: 28174
		[SerializeField]
		private AudioClip launchAudio;

		// Token: 0x04006E0F RID: 28175
		[SerializeField]
		private LayerMask collisionLayerMasks;

		// Token: 0x04006E10 RID: 28176
		[SerializeField]
		private List<string> collisionTags = new List<string>();

		// Token: 0x04006E11 RID: 28177
		[SerializeField]
		private bool destroyOnCollisionEnter;

		// Token: 0x04006E12 RID: 28178
		[SerializeField]
		private float destroyDelay = 1f;

		// Token: 0x04006E13 RID: 28179
		[Tooltip("Distance from the surface that the particle should spawn.")]
		[SerializeField]
		private float impactEffectOffset = 0.1f;

		// Token: 0x04006E14 RID: 28180
		[SerializeField]
		private SpawnWorldEffects spawnWorldEffects;

		// Token: 0x04006E15 RID: 28181
		private ConstantForce forceComponent;

		// Token: 0x04006E16 RID: 28182
		public UnityEvent<float> onLaunchShared;

		// Token: 0x04006E17 RID: 28183
		public UnityEvent onImpactShared;

		// Token: 0x04006E18 RID: 28184
		private bool impactEffectSpawned;

		// Token: 0x04006E19 RID: 28185
		private Rigidbody rigidbody;
	}
}
