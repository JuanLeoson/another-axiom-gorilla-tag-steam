using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003E1 RID: 993
public class SlingshotProjectile : MonoBehaviour
{
	// Token: 0x17000286 RID: 646
	// (get) Token: 0x06001744 RID: 5956 RVA: 0x0007DEC4 File Offset: 0x0007C0C4
	// (set) Token: 0x06001745 RID: 5957 RVA: 0x0007DECC File Offset: 0x0007C0CC
	public Vector3 launchPosition { get; private set; }

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06001746 RID: 5958 RVA: 0x0007DED8 File Offset: 0x0007C0D8
	// (remove) Token: 0x06001747 RID: 5959 RVA: 0x0007DF10 File Offset: 0x0007C110
	public event SlingshotProjectile.ProjectileImpactEvent OnImpact;

	// Token: 0x06001748 RID: 5960 RVA: 0x0007DF48 File Offset: 0x0007C148
	public void Launch(Vector3 position, Vector3 velocity, NetPlayer player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (this.launchSoundBankPlayer != null)
		{
			this.launchSoundBankPlayer.Play();
		}
		this.particleLaunched = true;
		this.timeCreated = Time.time;
		this.launchPosition = position;
		Transform transform = base.transform;
		transform.position = position;
		transform.localScale = Vector3.one * scale;
		base.GetComponent<Collider>().contactOffset = 0.01f * scale;
		RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
		if (component != null)
		{
			component.objectRadiusForWaterCollision = 0.02f * scale;
		}
		this.projectileRigidbody.isKinematic = false;
		this.projectileRigidbody.useGravity = false;
		this.projectileRigidbody.velocity = velocity;
		this.projectileOwner = player;
		this.myProjectileCount = projectileCount;
		this.projectileRigidbody.position = position;
		this.ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
		this.remainingLifeTime = this.lifeTime;
		if (this.forceComponent)
		{
			this.forceComponent.enabled = true;
			this.forceComponent.force = Physics.gravity * this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f);
			if (this.useForwardForce)
			{
				this.forceComponent.force += this.projectileRigidbody.velocity.normalized * this.forwardForceMultiplier;
			}
		}
		this.isSettled = false;
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x0007E0D0 File Offset: 0x0007C2D0
	protected void Awake()
	{
		if (this.playerImpactEffectPrefab == null)
		{
			this.playerImpactEffectPrefab = this.surfaceImpactEffectPrefab;
		}
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
		this.remainingLifeTime = this.lifeTime;
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x0007E148 File Offset: 0x0007C348
	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * this.initialScale;
		this.projectileRigidbody.useGravity = true;
		if (this.forceComponent)
		{
			this.forceComponent.force = Vector3.zero;
		}
		this.OnImpact = null;
		this.aoeKnockbackConfig = null;
		this.impactSoundVolumeOverride = null;
		this.impactSoundPitchOverride = null;
		this.impactEffectScaleMultiplier = 1f;
		this.projectileRigidbody.isKinematic = false;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x0007E1EC File Offset: 0x0007C3EC
	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		if (prefab == null)
		{
			return;
		}
		Vector3 position2 = position + normal * this.impactEffectOffset;
		GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
		Vector3 localScale = base.transform.localScale;
		gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
		gameObject.transform.up = normal;
		GorillaColorizableBase component = gameObject.GetComponent<GorillaColorizableBase>();
		if (component != null)
		{
			component.SetColor(this.teamColor);
		}
		SurfaceImpactFX component2 = gameObject.GetComponent<SurfaceImpactFX>();
		if (component2 != null)
		{
			component2.SetScale(localScale.x * this.impactEffectScaleMultiplier);
		}
		SoundBankPlayer component3 = gameObject.GetComponent<SoundBankPlayer>();
		if (component3 != null && !component3.playOnEnable)
		{
			component3.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
		}
		if (this.spawnWorldEffects != null)
		{
			this.spawnWorldEffects.RequestSpawn(position, normal);
		}
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x0007E2DC File Offset: 0x0007C4DC
	public void CheckForAOEKnockback(Vector3 impactPosition, float impactSpeed)
	{
		if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
		{
			Vector3 a = GTPlayer.Instance.HeadCenterPosition - impactPosition;
			if (a.sqrMagnitude < this.aoeKnockbackConfig.Value.aeoOuterRadius * this.aoeKnockbackConfig.Value.aeoOuterRadius)
			{
				float magnitude = a.magnitude;
				Vector3 direction = (magnitude > 0.001f) ? (a / magnitude) : Vector3.up;
				float num = Mathf.InverseLerp(this.aoeKnockbackConfig.Value.aeoOuterRadius, this.aoeKnockbackConfig.Value.aeoInnerRadius, magnitude);
				float num2 = Mathf.InverseLerp(0f, this.aoeKnockbackConfig.Value.impactVelocityThreshold, impactSpeed);
				GTPlayer.Instance.ApplyKnockback(direction, this.aoeKnockbackConfig.Value.knockbackVelocity * num * num2, false);
				this.impactEffectScaleMultiplier = Mathf.Lerp(1f, this.impactEffectScaleMultiplier, num2);
				if (this.impactSoundVolumeOverride != null)
				{
					this.impactSoundVolumeOverride = new float?(Mathf.Lerp(this.impactSoundVolumeOverride.Value * 0.5f, this.impactSoundVolumeOverride.Value, num2));
				}
				float num3 = Mathf.Lerp(this.aoeKnockbackConfig.Value.aeoInnerRadius, this.aoeKnockbackConfig.Value.aeoOuterRadius, 0.25f);
				if (this.aoeKnockbackConfig.Value.playerProximityEffect != PlayerEffect.NONE && a.sqrMagnitude < num3 * num3)
				{
					RoomSystem.SendPlayerEffect(PlayerEffect.SNOWBALL_IMPACT, NetworkSystem.Instance.LocalPlayer);
				}
			}
		}
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x0007E480 File Offset: 0x0007C680
	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			this.teamColor = overrideColor;
		}
		else
		{
			this.teamColor = (blueTeam ? this.blueColor : (orangeTeam ? this.orangeColor : this.defaultColor));
		}
		this.blueBall.enabled = blueTeam;
		this.orangeBall.enabled = orangeTeam;
		this.defaultBall.enabled = (!blueTeam && !orangeTeam);
		this.teamRenderer = (blueTeam ? this.blueBall : (orangeTeam ? this.orangeBall : this.defaultBall));
		this.ApplyColor(this.teamRenderer, (this.colorizeBalls || shouldOverrideColor) ? this.teamColor : Color.white);
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x0007E52E File Offset: 0x0007C72E
	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
		SlingshotProjectileManager.RegisterSP(this);
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0007E548 File Offset: 0x0007C748
	protected void OnDisable()
	{
		this.particleLaunched = false;
		SlingshotProjectileManager.UnregisterSP(this);
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x0007E558 File Offset: 0x0007C758
	public void InvokeUpdate()
	{
		if (this.particleLaunched || this.dontDestroyOnHit)
		{
			if (Time.time > this.timeCreated + this.GetRemainingLifeTime())
			{
				this.DestroyAfterRelease();
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
		if (this.dontDestroyOnHit)
		{
			this.SettleProjectile();
		}
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x0007E5E9 File Offset: 0x0007C7E9
	public void DestroyAfterRelease()
	{
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
		this.Deactivate();
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x0007E60D File Offset: 0x0007C80D
	public float GetRemainingLifeTime()
	{
		return this.remainingLifeTime;
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x0007E615 File Offset: 0x0007C815
	public void UpdateRemainingLifeTime(float newLifeTime)
	{
		this.remainingLifeTime = newLifeTime;
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x0007E620 File Offset: 0x0007C820
	public float GetDistanceTraveled()
	{
		return (base.transform.position - this.launchPosition).magnitude;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0007E64C File Offset: 0x0007C84C
	private void SettleProjectile()
	{
		if (!this.isSettled)
		{
			int value = this.floorLayerMask.value;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 0.1f, value, QueryTriggerInteraction.Ignore) && Vector3.Angle(raycastHit.normal, Vector3.up) < 40f)
			{
				if (this.forceComponent)
				{
					this.forceComponent.force = Vector3.zero;
				}
				this.projectileRigidbody.angularVelocity = Vector3.zero;
				this.projectileRigidbody.velocity = Vector3.zero;
				this.projectileRigidbody.isKinematic = true;
				base.transform.position = raycastHit.point + Vector3.up * this.placementOffset;
				this.isSettled = true;
				return;
			}
		}
		else if (this.keepRotationUpright)
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.up, Vector3.up).normalized, Vector3.up);
			base.transform.rotation = rotation;
		}
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0007E764 File Offset: 0x0007C964
	protected void OnCollisionEnter(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.collider.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeHit(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x0007E7FC File Offset: 0x0007C9FC
	protected void OnCollisionStay(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeCollisionStay(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x0007E890 File Offset: 0x0007CA90
	protected void OnTriggerExit(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerExit(this, other);
		}
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x0007E8C0 File Offset: 0x0007CAC0
	protected void OnTriggerEnter(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerEnter(this, other);
		}
		if (this.projectileOwner == NetworkSystem.Instance.LocalPlayer)
		{
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = (componentInParent != null) ? componentInParent.creator : null;
			if (netPlayer == null)
			{
				return;
			}
			SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, base.transform.position, netPlayer);
			}
			if (NetworkSystem.Instance.LocalPlayer == netPlayer)
			{
				return;
			}
			if (component && !component.LocalCanHit(NetworkSystem.Instance.LocalPlayer, netPlayer))
			{
				return;
			}
			if (component && GameMode.ActiveNetworkHandler)
			{
				GameMode.ActiveNetworkHandler.SendRPC("RPC_ReportSlingshotHit", false, new object[]
				{
					(netPlayer as PunNetPlayer).PlayerRef,
					base.transform.position,
					this.myProjectileCount
				});
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
			if (this.m_sendNetworkedImpact)
			{
				RoomSystem.SendImpactEffect(base.transform.position, this.teamColor.r, this.teamColor.g, this.teamColor.b, this.teamColor.a, this.myProjectileCount);
			}
			this.Deactivate();
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		VRRig arg;
		if (attachedRigidbody.IsNotNull() && attachedRigidbody.gameObject.TryGetComponent<VRRig>(out arg))
		{
			UnityEvent<VRRig> onHitPlayer = this.OnHitPlayer;
			if (onHitPlayer == null)
			{
				return;
			}
			onHitPlayer.Invoke(arg);
		}
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x0007EA8D File Offset: 0x0007CC8D
	private void ApplyColor(Renderer rend, Color color)
	{
		if (!rend)
		{
			return;
		}
		this.matPropBlock.SetColor(ShaderProps._BaseColor, color);
		this.matPropBlock.SetColor(ShaderProps._Color, color);
		rend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x04001F28 RID: 7976
	public NetPlayer projectileOwner;

	// Token: 0x04001F29 RID: 7977
	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	// Token: 0x04001F2A RID: 7978
	[Tooltip("if left empty, the default player impact that is set in Room System Setting will be played")]
	public GameObject playerImpactEffectPrefab;

	// Token: 0x04001F2B RID: 7979
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x04001F2C RID: 7980
	[SerializeField]
	private SoundBankPlayer launchSoundBankPlayer;

	// Token: 0x04001F2D RID: 7981
	[SerializeField]
	private bool dontDestroyOnHit;

	// Token: 0x04001F2E RID: 7982
	[SerializeField]
	private LayerMask floorLayerMask;

	// Token: 0x04001F2F RID: 7983
	[SerializeField]
	private float placementOffset = 0.01f;

	// Token: 0x04001F30 RID: 7984
	[SerializeField]
	private bool keepRotationUpright = true;

	// Token: 0x04001F31 RID: 7985
	public float lifeTime = 20f;

	// Token: 0x04001F32 RID: 7986
	public float gravityMultiplier = 1f;

	// Token: 0x04001F33 RID: 7987
	public bool useForwardForce;

	// Token: 0x04001F34 RID: 7988
	public float forwardForceMultiplier = 0.1f;

	// Token: 0x04001F35 RID: 7989
	public Color defaultColor = Color.white;

	// Token: 0x04001F36 RID: 7990
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x04001F37 RID: 7991
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x04001F38 RID: 7992
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	// Token: 0x04001F39 RID: 7993
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	// Token: 0x04001F3A RID: 7994
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	// Token: 0x04001F3B RID: 7995
	public bool colorizeBalls;

	// Token: 0x04001F3C RID: 7996
	public bool faceDirectionOfTravel = true;

	// Token: 0x04001F3D RID: 7997
	private bool particleLaunched;

	// Token: 0x04001F3E RID: 7998
	private float timeCreated;

	// Token: 0x04001F40 RID: 8000
	private Rigidbody projectileRigidbody;

	// Token: 0x04001F41 RID: 8001
	private Color teamColor = Color.white;

	// Token: 0x04001F42 RID: 8002
	private Renderer teamRenderer;

	// Token: 0x04001F43 RID: 8003
	public int myProjectileCount;

	// Token: 0x04001F44 RID: 8004
	private float initialScale;

	// Token: 0x04001F45 RID: 8005
	private Vector3 previousPosition;

	// Token: 0x04001F46 RID: 8006
	[HideInInspector]
	public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

	// Token: 0x04001F47 RID: 8007
	[HideInInspector]
	public float? impactSoundVolumeOverride;

	// Token: 0x04001F48 RID: 8008
	[HideInInspector]
	public float? impactSoundPitchOverride;

	// Token: 0x04001F49 RID: 8009
	[HideInInspector]
	public float impactEffectScaleMultiplier = 1f;

	// Token: 0x04001F4A RID: 8010
	private ConstantForce forceComponent;

	// Token: 0x04001F4B RID: 8011
	public bool m_sendNetworkedImpact = true;

	// Token: 0x04001F4D RID: 8013
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04001F4E RID: 8014
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x04001F4F RID: 8015
	public UnityEvent<VRRig> OnHitPlayer;

	// Token: 0x04001F50 RID: 8016
	private float remainingLifeTime;

	// Token: 0x04001F51 RID: 8017
	private bool isSettled;

	// Token: 0x04001F52 RID: 8018
	private float distanceTraveled;

	// Token: 0x020003E2 RID: 994
	[Serializable]
	public struct AOEKnockbackConfig
	{
		// Token: 0x04001F53 RID: 8019
		public bool applyAOEKnockback;

		// Token: 0x04001F54 RID: 8020
		[Tooltip("Full knockback velocity is imparted within the inner radius")]
		public float aeoInnerRadius;

		// Token: 0x04001F55 RID: 8021
		[Tooltip("Partial knockback velocity is imparted between the inner and outer radius")]
		public float aeoOuterRadius;

		// Token: 0x04001F56 RID: 8022
		public float knockbackVelocity;

		// Token: 0x04001F57 RID: 8023
		[Tooltip("The required impact velocity to achieve full knockback velocity")]
		public float impactVelocityThreshold;

		// Token: 0x04001F58 RID: 8024
		[SerializeField]
		public PlayerEffect playerProximityEffect;
	}

	// Token: 0x020003E3 RID: 995
	// (Invoke) Token: 0x0600175D RID: 5981
	public delegate void ProjectileImpactEvent(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
}
