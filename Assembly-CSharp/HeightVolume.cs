using System;
using UnityEngine;

// Token: 0x0200074E RID: 1870
public class HeightVolume : MonoBehaviour
{
	// Token: 0x06002EDB RID: 11995 RVA: 0x000F8275 File Offset: 0x000F6475
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
		this.musicSource = this.audioSource.gameObject.GetComponent<MusicSource>();
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x000F82AC File Offset: 0x000F64AC
	private void Update()
	{
		if (this.audioSource.gameObject.activeSelf && (!(this.musicSource != null) || !this.musicSource.VolumeOverridden))
		{
			if (this.targetTransform.position.y > this.heightTop.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.baseVolume : this.minVolume);
				return;
			}
			if (this.targetTransform.position.y < this.heightBottom.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.minVolume : this.baseVolume);
				return;
			}
			this.audioSource.volume = ((!this.invertHeightVol) ? ((this.targetTransform.position.y - this.heightBottom.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume) : ((this.heightTop.position.y - this.targetTransform.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume));
		}
	}

	// Token: 0x04003AD1 RID: 15057
	public Transform heightTop;

	// Token: 0x04003AD2 RID: 15058
	public Transform heightBottom;

	// Token: 0x04003AD3 RID: 15059
	public AudioSource audioSource;

	// Token: 0x04003AD4 RID: 15060
	public float baseVolume;

	// Token: 0x04003AD5 RID: 15061
	public float minVolume;

	// Token: 0x04003AD6 RID: 15062
	public Transform targetTransform;

	// Token: 0x04003AD7 RID: 15063
	public bool invertHeightVol;

	// Token: 0x04003AD8 RID: 15064
	private MusicSource musicSource;
}
