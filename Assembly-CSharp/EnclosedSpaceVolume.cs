using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class EnclosedSpaceVolume : GorillaTriggerBox
{
	// Token: 0x06002265 RID: 8805 RVA: 0x000B9ADA File Offset: 0x000B7CDA
	private void Awake()
	{
		this.audioSourceInside.volume = this.quietVolume;
		this.audioSourceOutside.volume = this.loudVolume;
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000B9AFE File Offset: 0x000B7CFE
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.loudVolume;
			this.audioSourceOutside.volume = this.quietVolume;
		}
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000B9B35 File Offset: 0x000B7D35
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.quietVolume;
			this.audioSourceOutside.volume = this.loudVolume;
		}
	}

	// Token: 0x04002BDB RID: 11227
	public AudioSource audioSourceInside;

	// Token: 0x04002BDC RID: 11228
	public AudioSource audioSourceOutside;

	// Token: 0x04002BDD RID: 11229
	public float loudVolume;

	// Token: 0x04002BDE RID: 11230
	public float quietVolume;
}
