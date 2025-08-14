using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004AD RID: 1197
public class MonkeCandle : RubberDuck
{
	// Token: 0x06001D93 RID: 7571 RVA: 0x0009E8FC File Offset: 0x0009CAFC
	protected override void Start()
	{
		base.Start();
		if (!this.IsMyItem())
		{
			this.movingFxAudio.volume = this.movingFxAudio.volume * 0.5f;
			this.fxExplodeAudio.volume = this.fxExplodeAudio.volume * 0.5f;
		}
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x0009E950 File Offset: 0x0009CB50
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (!this.particleFX.isPlaying)
		{
			return;
		}
		int particles = this.particleFX.GetParticles(this.fxParticleArray);
		if (particles <= 0)
		{
			this.movingFxAudio.GTStop();
			if (this.currentParticles.Count == 0)
			{
				return;
			}
		}
		for (int i = 0; i < particles; i++)
		{
			if (this.currentParticles.Contains(this.fxParticleArray[i].randomSeed))
			{
				this.currentParticles.Remove(this.fxParticleArray[i].randomSeed);
			}
		}
		foreach (uint key in this.currentParticles)
		{
			if (this.particleInfoDict.TryGetValue(key, out this.outPosition))
			{
				this.fxExplodeAudio.transform.position = this.outPosition;
				this.fxExplodeAudio.GTPlayOneShot(this.fxExplodeAudio.clip, 1f);
				this.particleInfoDict.Remove(key);
			}
		}
		this.currentParticles.Clear();
		for (int j = 0; j < particles; j++)
		{
			if (j == 0)
			{
				this.movingFxAudio.transform.position = this.fxParticleArray[j].position;
			}
			if (this.particleInfoDict.TryGetValue(this.fxParticleArray[j].randomSeed, out this.outPosition))
			{
				this.particleInfoDict[this.fxParticleArray[j].randomSeed] = this.fxParticleArray[j].position;
			}
			else
			{
				this.particleInfoDict.Add(this.fxParticleArray[j].randomSeed, this.fxParticleArray[j].position);
				if (j == 0 && !this.movingFxAudio.isPlaying)
				{
					this.movingFxAudio.GTPlay();
				}
			}
			this.currentParticles.Add(this.fxParticleArray[j].randomSeed);
		}
	}

	// Token: 0x04002624 RID: 9764
	private ParticleSystem.Particle[] fxParticleArray = new ParticleSystem.Particle[20];

	// Token: 0x04002625 RID: 9765
	public AudioSource movingFxAudio;

	// Token: 0x04002626 RID: 9766
	public AudioSource fxExplodeAudio;

	// Token: 0x04002627 RID: 9767
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04002628 RID: 9768
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04002629 RID: 9769
	private Vector3 outPosition;
}
