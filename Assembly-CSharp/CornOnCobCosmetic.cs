using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class CornOnCobCosmetic : MonoBehaviour
{
	// Token: 0x060005B6 RID: 1462 RVA: 0x000212B8 File Offset: 0x0001F4B8
	protected void Awake()
	{
		this.emissionModule = this.particleSys.emission;
		this.maxBurstProbability = ((this.emissionModule.burstCount > 0) ? this.emissionModule.GetBurst(0).probability : 0.2f);
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00021308 File Offset: 0x0001F508
	protected void LateUpdate()
	{
		for (int i = 0; i < this.emissionModule.burstCount; i++)
		{
			ParticleSystem.Burst burst = this.emissionModule.GetBurst(i);
			burst.probability = this.maxBurstProbability * this.particleEmissionCurve.Evaluate(this.thermalReceiver.celsius);
			this.emissionModule.SetBurst(i, burst);
		}
		int particleCount = this.particleSys.particleCount;
		if (particleCount > this.previousParticleCount)
		{
			this.soundBankPlayer.Play();
		}
		this.previousParticleCount = particleCount;
	}

	// Token: 0x040006D7 RID: 1751
	[Tooltip("The corn will start popping based on the temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x040006D8 RID: 1752
	[Tooltip("The particle system that will be emitted when the heat source is hot enough.")]
	public ParticleSystem particleSys;

	// Token: 0x040006D9 RID: 1753
	[Tooltip("The curve that determines how many particles will be emitted based on the heat source's temperature.\n\nThe x-axis is the heat source's temperature and the y-axis is the number of particles to emit.")]
	public AnimationCurve particleEmissionCurve;

	// Token: 0x040006DA RID: 1754
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040006DB RID: 1755
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x040006DC RID: 1756
	private float maxBurstProbability;

	// Token: 0x040006DD RID: 1757
	private int previousParticleCount;
}
