using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class ParachuteProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x06000AC5 RID: 2757 RVA: 0x00039E6A File Offset: 0x0003806A
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00039E78 File Offset: 0x00038078
	private void OnEnable()
	{
		this.launched = false;
		this.landTime = 0f;
		this.launchedTime = 0f;
		this.peakTime = float.MaxValue;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (!this.TickRunning)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00039ED8 File Offset: 0x000380D8
	private void OnDisable()
	{
		this.launched = false;
		if (this.TickRunning)
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00039EF0 File Offset: 0x000380F0
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		this.parachuteDeployed = false;
		this.landed = false;
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.ChangeUp(Vector3.up);
		this.rb.freezeRotation = true;
		if (ownerRig == null)
		{
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		}
		this.rb.isKinematic = false;
		this.rb.velocity = velocity;
		this.rb.drag = this.initialDrag;
		this.rb.angularDrag = this.initialAngularDrag;
		this.launchedTime = Time.time;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (velocity.y > 0f)
		{
			this.peakTime = velocity.y / (-1f * Physics.gravity.y);
		}
		else
		{
			this.peakTime = 0f;
		}
		this.launched = true;
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0003A02C File Offset: 0x0003822C
	private void OnPeakReached()
	{
		this.parachuteDeployed = true;
		this.parachute.SetActive(true);
		this.monkeMeshFilter.mesh = this.parachutingMesh;
		this.ChangeUp(Vector3.up);
		this.rb.drag = this.parachuteDrag;
		this.rb.angularDrag = this.parachuteAngularDrag;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0003A08C File Offset: 0x0003828C
	private void OnLanded(Collision collision)
	{
		this.landTime = Time.time;
		this.landed = true;
		ContactPoint contact = collision.GetContact(0);
		this.rb.isKinematic = true;
		this.rb.position = contact.point + contact.normal * (this.groundOffset * base.transform.localScale.x);
		this.ChangeUp(contact.normal);
		this.monkeMeshFilter.mesh = this.landedMesh;
		this.parachute.SetActive(false);
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0003A124 File Offset: 0x00038324
	private void ChangeUp(Vector3 newUp)
	{
		Vector3 forward = Vector3.Cross(this.rb.transform.right, newUp);
		if (forward.sqrMagnitude < 1E-45f)
		{
			forward = Vector3.Cross(Vector3.Cross(newUp, this.rb.transform.forward), newUp);
		}
		this.rb.rotation = Quaternion.LookRotation(forward, newUp);
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x0003A188 File Offset: 0x00038388
	private void PlayImpactEffects(Vector3 position, Vector3 normal)
	{
		if (this.impactEffect != null)
		{
			Vector3 position2 = position + this.impactEffectOffset * normal;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.impactEffect, position2, true);
			gameObject.transform.localScale = base.transform.localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
		}
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x0003A204 File Offset: 0x00038404
	public void OnTriggerEvent(bool isLeft, Collider col)
	{
		if (this.parachuteDeployed)
		{
			this.PlayImpactEffects(base.transform.position, Vector3.up);
			GorillaTriggerColliderHandIndicator componentInParent = col.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent != null)
			{
				float amplitude = GorillaTagger.Instance.tapHapticStrength / 2f;
				float fixedDeltaTime = Time.fixedDeltaTime;
				GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, amplitude, fixedDeltaTime);
			}
		}
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x0003A268 File Offset: 0x00038468
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.launched || this.landed)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		if (collision.collider.attachedRigidbody != null)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (!this.parachuteDeployed)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (Vector3.Angle(contact.normal, Vector3.up) < this.groudUpThreshold)
		{
			this.OnLanded(collision);
			return;
		}
		this.PlayImpactEffects(contact.point, contact.normal);
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x0003A331 File Offset: 0x00038531
	// (set) Token: 0x06000AD0 RID: 2768 RVA: 0x0003A339 File Offset: 0x00038539
	public bool TickRunning { get; set; }

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0003A344 File Offset: 0x00038544
	public void Tick()
	{
		if (!this.parachuteDeployed && Time.time > this.launchedTime + this.parachuteDeployDelay && Time.time >= this.launchedTime + this.peakTime)
		{
			this.OnPeakReached();
		}
		if (this.landed && Time.time > this.landTime + this.destroyOnLandDelay)
		{
			this.PlayImpactEffects(base.transform.position, base.transform.up);
		}
	}

	// Token: 0x04000D33 RID: 3379
	[SerializeField]
	private MeshFilter monkeMeshFilter;

	// Token: 0x04000D34 RID: 3380
	[SerializeField]
	private GameObject parachute;

	// Token: 0x04000D35 RID: 3381
	[SerializeField]
	private Mesh launchMesh;

	// Token: 0x04000D36 RID: 3382
	[SerializeField]
	private Mesh parachutingMesh;

	// Token: 0x04000D37 RID: 3383
	[SerializeField]
	private Mesh landedMesh;

	// Token: 0x04000D38 RID: 3384
	[Tooltip("time to wait after launch before deploying the parachute")]
	[SerializeField]
	private float parachuteDeployDelay = 1f;

	// Token: 0x04000D39 RID: 3385
	[Tooltip("time to wait after landing before destroying")]
	[SerializeField]
	private float destroyOnLandDelay = 3f;

	// Token: 0x04000D3A RID: 3386
	[Tooltip("How far from the collision point should the projectile sit when landed")]
	[SerializeField]
	private float groundOffset;

	// Token: 0x04000D3B RID: 3387
	[Tooltip("Acceptable angle in degrees of surface from world up to be considered the ground")]
	[SerializeField]
	private float groudUpThreshold = 45f;

	// Token: 0x04000D3C RID: 3388
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialDrag;

	// Token: 0x04000D3D RID: 3389
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialAngularDrag = 0.05f;

	// Token: 0x04000D3E RID: 3390
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteDrag = 5f;

	// Token: 0x04000D3F RID: 3391
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteAngularDrag = 10f;

	// Token: 0x04000D40 RID: 3392
	[SerializeField]
	private GameObject impactEffect;

	// Token: 0x04000D41 RID: 3393
	[SerializeField]
	private float impactEffectScaleMultiplier = 1f;

	// Token: 0x04000D42 RID: 3394
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x04000D43 RID: 3395
	private Rigidbody rb;

	// Token: 0x04000D44 RID: 3396
	private bool launched;

	// Token: 0x04000D45 RID: 3397
	private float launchedTime;

	// Token: 0x04000D46 RID: 3398
	private float landTime;

	// Token: 0x04000D47 RID: 3399
	private float peakTime = float.MaxValue;

	// Token: 0x04000D48 RID: 3400
	private bool parachuteDeployed;

	// Token: 0x04000D49 RID: 3401
	private bool landed;
}
