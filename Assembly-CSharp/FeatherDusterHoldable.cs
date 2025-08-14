using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class FeatherDusterHoldable : MonoBehaviour
{
	// Token: 0x0600063D RID: 1597 RVA: 0x000241AB File Offset: 0x000223AB
	protected void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x000241DB File Offset: 0x000223DB
	protected void OnEnable()
	{
		this.lastWorldPos = base.transform.position;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00024200 File Offset: 0x00022400
	protected void Update()
	{
		this.timeSinceLastSound += Time.deltaTime;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num = (position - this.lastWorldPos).magnitude / Time.deltaTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num >= this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play();
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	// Token: 0x04000774 RID: 1908
	public LayerMask collisionLayer;

	// Token: 0x04000775 RID: 1909
	public float overlapSphereRadius = 0.08f;

	// Token: 0x04000776 RID: 1910
	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	// Token: 0x04000777 RID: 1911
	public ParticleSystem particleFx;

	// Token: 0x04000778 RID: 1912
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000779 RID: 1913
	private float soundCooldown = 0.8f;

	// Token: 0x0400077A RID: 1914
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x0400077B RID: 1915
	private float initialRateOverTime;

	// Token: 0x0400077C RID: 1916
	private float timeSinceLastSound;

	// Token: 0x0400077D RID: 1917
	private Vector3 lastWorldPos;

	// Token: 0x0400077E RID: 1918
	private Collider[] colliderResult = new Collider[1];
}
