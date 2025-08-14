using System;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class CosmeticCritterCatcherButterflyNet : CosmeticCritterCatcher
{
	// Token: 0x06000428 RID: 1064 RVA: 0x000187F4 File Offset: 0x000169F4
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (!(critter is CosmeticCritterButterfly) || (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude > this.maxCatchRadius * this.maxCatchRadius || this.velocityEstimator.linearVelocity.sqrMagnitude < this.minCatchSpeed * this.minCatchSpeed)
		{
			return CosmeticCritterAction.None;
		}
		return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00018868 File Offset: 0x00016A68
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return base.ValidateRemoteCatchAction(critter, catchAction, serverTime) && critter is CosmeticCritterButterfly && (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude <= this.maxCatchRadius * this.maxCatchRadius + 1f && this.velocityEstimator.linearVelocity.sqrMagnitude >= this.minCatchSpeed * this.minCatchSpeed - 1f && catchAction == (CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn);
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x000188F3 File Offset: 0x00016AF3
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.caughtButterflyParticleSystem.Emit((critter as CosmeticCritterButterfly).GetEmitParams, 1);
		this.catchFX.Play();
		this.catchSFX.Play();
	}

	// Token: 0x040004B2 RID: 1202
	[Tooltip("Use this for calculating the catch position and velocity.")]
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040004B3 RID: 1203
	[Tooltip("Catch the Butterfly if it is within this radius.")]
	[SerializeField]
	private float maxCatchRadius;

	// Token: 0x040004B4 RID: 1204
	[Tooltip("Only catch the Butterfly if the net is moving faster than this speed.")]
	[SerializeField]
	private float minCatchSpeed;

	// Token: 0x040004B5 RID: 1205
	[Tooltip("Spawn a particle inside the net representing the caught Butterfly.")]
	[SerializeField]
	private ParticleSystem caughtButterflyParticleSystem;

	// Token: 0x040004B6 RID: 1206
	[Tooltip("Play this particle effect when catching a Butterfly.")]
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x040004B7 RID: 1207
	[Tooltip("Play this sound when catching a Butterfly.")]
	[SerializeField]
	private AudioSource catchSFX;
}
