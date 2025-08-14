using System;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C95 RID: 3221
	public class BuilderProjectile : MonoBehaviour
	{
		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x06004FDA RID: 20442 RVA: 0x0018DFA5 File Offset: 0x0018C1A5
		// (set) Token: 0x06004FDB RID: 20443 RVA: 0x0018DFAD File Offset: 0x0018C1AD
		public Vector3 launchPosition { get; private set; }

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x06004FDC RID: 20444 RVA: 0x0018DFB8 File Offset: 0x0018C1B8
		// (remove) Token: 0x06004FDD RID: 20445 RVA: 0x0018DFF0 File Offset: 0x0018C1F0
		public event BuilderProjectile.ProjectileImpactEvent OnImpact;

		// Token: 0x06004FDE RID: 20446 RVA: 0x0018E028 File Offset: 0x0018C228
		public void Launch(Vector3 position, Vector3 velocity, BuilderProjectileLauncher sourceObject, int projectileCount, float scale, int timeStamp)
		{
			this.particleLaunched = true;
			this.timeCreated = Time.time;
			this.projectileSource = sourceObject;
			float num = (NetworkSystem.Instance.ServerTimestamp - timeStamp) / 1000f;
			if (num >= this.lifeTime)
			{
				this.Deactivate();
				return;
			}
			this.timeCreated -= num;
			Vector3 vector = Vector3.ProjectOnPlane(velocity, Vector3.up);
			float f = 0.017453292f * Vector3.Angle(vector, velocity);
			float num2 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * 9.8f;
			Vector3 b = num * Mathf.Cos(f) * vector;
			float d = velocity.z * num * Mathf.Sin(f) - 0.5f * num2 * num * num;
			this.launchPosition = position + b + d * Vector3.down;
			Transform transform = base.transform;
			transform.position = position;
			transform.localScale = Vector3.one * scale;
			base.GetComponent<Collider>().contactOffset = 0.01f * scale;
			RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
			if (component != null)
			{
				component.objectRadiusForWaterCollision = 0.02f * scale;
			}
			this.projectileRigidbody.useGravity = false;
			Vector3 vector2 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * Physics.gravity;
			this.forceComponent.force = vector2;
			this.projectileRigidbody.velocity = velocity + num * vector2;
			this.projectileId = projectileCount;
			this.projectileRigidbody.position = position;
			this.projectileSource.RegisterProjectile(this);
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x0018E1E9 File Offset: 0x0018C3E9
		protected void Awake()
		{
			this.projectileRigidbody = base.GetComponent<Rigidbody>();
			this.forceComponent = base.GetComponent<ConstantForce>();
			this.initialScale = base.transform.localScale.x;
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x0018E21C File Offset: 0x0018C41C
		public void Deactivate()
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			this.projectileRigidbody.useGravity = true;
			this.forceComponent.force = Vector3.zero;
			this.OnImpact = null;
			this.aoeKnockbackConfig = null;
			this.impactSoundVolumeOverride = null;
			this.impactSoundPitchOverride = null;
			this.impactEffectScaleMultiplier = 1f;
			this.gravityMultiplier = 1f;
			ObjectPools.instance.Destroy(base.gameObject);
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x0018E2B4 File Offset: 0x0018C4B4
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 position2 = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
			Vector3 localScale = base.transform.localScale;
			gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(localScale.x * this.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
			}
		}

		// Token: 0x06004FE2 RID: 20450 RVA: 0x0018E35C File Offset: 0x0018C55C
		public void ApplyHitKnockback(Vector3 hitNormal)
		{
			if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
			{
				Vector3 a = Vector3.ProjectOnPlane(hitNormal, Vector3.up);
				a.Normalize();
				Vector3 direction = 0.75f * a + 0.25f * Vector3.up;
				direction.Normalize();
				GTPlayer instance = GTPlayer.Instance;
				instance.ApplyKnockback(direction, this.aoeKnockbackConfig.Value.knockbackVelocity, instance.scale < 0.9f);
			}
		}

		// Token: 0x06004FE3 RID: 20451 RVA: 0x0018E3EC File Offset: 0x0018C5EC
		private void OnEnable()
		{
			this.timeCreated = 0f;
			this.particleLaunched = false;
		}

		// Token: 0x06004FE4 RID: 20452 RVA: 0x0018E400 File Offset: 0x0018C600
		protected void OnDisable()
		{
			this.particleLaunched = false;
			if (this.projectileSource != null)
			{
				this.projectileSource.UnRegisterProjectile(this);
			}
			this.projectileSource = null;
		}

		// Token: 0x06004FE5 RID: 20453 RVA: 0x0018E42C File Offset: 0x0018C62C
		public void UpdateProjectile()
		{
			if (this.particleLaunched)
			{
				if (Time.time > this.timeCreated + this.lifeTime)
				{
					this.Deactivate();
				}
				if (this.faceDirectionOfTravel)
				{
					Transform transform = base.transform;
					Vector3 position = transform.position;
					Vector3 forward = position - this.previousPosition;
					transform.rotation = ((forward.sqrMagnitude > 0f) ? Quaternion.LookRotation(forward) : transform.rotation);
					this.previousPosition = position;
				}
			}
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x0018E4A8 File Offset: 0x0018C6A8
		private void OnCollisionEnter(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x0018E560 File Offset: 0x0018C760
		protected void OnCollisionStay(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x0018E618 File Offset: 0x0018C818
		protected void OnTriggerEnter(Collider other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = (componentInParent != null) ? componentInParent.creator : null;
			if (netPlayer == null)
			{
				return;
			}
			if (netPlayer.IsLocal)
			{
				return;
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
			this.Deactivate();
		}

		// Token: 0x040058EB RID: 22763
		public BuilderProjectileLauncher projectileSource;

		// Token: 0x040058EC RID: 22764
		[Tooltip("Rotates to point along the Y axis after spawn.")]
		public GameObject surfaceImpactEffectPrefab;

		// Token: 0x040058ED RID: 22765
		[Tooltip("Distance from the surface that the particle should spawn.")]
		private float impactEffectOffset;

		// Token: 0x040058EE RID: 22766
		public float lifeTime = 20f;

		// Token: 0x040058EF RID: 22767
		public bool faceDirectionOfTravel = true;

		// Token: 0x040058F0 RID: 22768
		private bool particleLaunched;

		// Token: 0x040058F1 RID: 22769
		private float timeCreated;

		// Token: 0x040058F3 RID: 22771
		private Rigidbody projectileRigidbody;

		// Token: 0x040058F4 RID: 22772
		public int projectileId;

		// Token: 0x040058F5 RID: 22773
		private float initialScale;

		// Token: 0x040058F6 RID: 22774
		private Vector3 previousPosition;

		// Token: 0x040058F7 RID: 22775
		[HideInInspector]
		public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

		// Token: 0x040058F8 RID: 22776
		[HideInInspector]
		public float? impactSoundVolumeOverride;

		// Token: 0x040058F9 RID: 22777
		[HideInInspector]
		public float? impactSoundPitchOverride;

		// Token: 0x040058FA RID: 22778
		[HideInInspector]
		public float impactEffectScaleMultiplier = 1f;

		// Token: 0x040058FB RID: 22779
		[HideInInspector]
		public float gravityMultiplier = 1f;

		// Token: 0x040058FC RID: 22780
		private ConstantForce forceComponent;

		// Token: 0x02000C96 RID: 3222
		// (Invoke) Token: 0x06004FEB RID: 20459
		public delegate void ProjectileImpactEvent(BuilderProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
	}
}
