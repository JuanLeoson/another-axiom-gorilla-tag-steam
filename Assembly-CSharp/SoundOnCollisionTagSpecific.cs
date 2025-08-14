using System;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class SoundOnCollisionTagSpecific : MonoBehaviour
{
	// Token: 0x06000DB3 RID: 3507 RVA: 0x00053E7C File Offset: 0x0005207C
	private void OnTriggerEnter(Collider collider)
	{
		if (Time.time > this.nextSound && collider.gameObject.CompareTag(this.tagName))
		{
			this.nextSound = Time.time + this.noiseCooldown;
			this.audioSource.GTPlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)], 0.5f);
		}
	}

	// Token: 0x04001584 RID: 5508
	public string tagName;

	// Token: 0x04001585 RID: 5509
	public float noiseCooldown = 1f;

	// Token: 0x04001586 RID: 5510
	private float nextSound;

	// Token: 0x04001587 RID: 5511
	public AudioSource audioSource;

	// Token: 0x04001588 RID: 5512
	public AudioClip[] collisionSounds;
}
