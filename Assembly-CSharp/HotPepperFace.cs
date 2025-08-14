using System;
using UnityEngine;

// Token: 0x02000498 RID: 1176
public class HotPepperFace : MonoBehaviour
{
	// Token: 0x06001D19 RID: 7449 RVA: 0x0009C453 File Offset: 0x0009A653
	public void PlayFX(float delay)
	{
		if (delay < 0f)
		{
			this.PlayFX();
			return;
		}
		base.Invoke("PlayFX", delay);
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x0009C470 File Offset: 0x0009A670
	public void PlayFX()
	{
		this._faceMesh.SetActive(true);
		this._thermalSourceVolume.SetActive(true);
		this._fireFX.Play();
		this._flameSpeaker.GTPlay();
		this._breathSpeaker.GTPlay();
		base.Invoke("StopFX", this._effectLength);
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x0009C4C7 File Offset: 0x0009A6C7
	public void StopFX()
	{
		this._faceMesh.SetActive(false);
		this._thermalSourceVolume.SetActive(false);
		this._fireFX.Stop();
		this._flameSpeaker.GTStop();
		this._breathSpeaker.GTStop();
	}

	// Token: 0x0400257D RID: 9597
	[SerializeField]
	private GameObject _faceMesh;

	// Token: 0x0400257E RID: 9598
	[SerializeField]
	private ParticleSystem _fireFX;

	// Token: 0x0400257F RID: 9599
	[SerializeField]
	private AudioSource _flameSpeaker;

	// Token: 0x04002580 RID: 9600
	[SerializeField]
	private AudioSource _breathSpeaker;

	// Token: 0x04002581 RID: 9601
	[SerializeField]
	private float _effectLength = 1.5f;

	// Token: 0x04002582 RID: 9602
	[SerializeField]
	private GameObject _thermalSourceVolume;
}
