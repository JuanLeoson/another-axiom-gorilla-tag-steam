using System;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class SpitballEvents : SubEmitterListener
{
	// Token: 0x06001D64 RID: 7524 RVA: 0x0009D7A9 File Offset: 0x0009B9A9
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.GTPlayOneShot(this._sfxHit, 1f);
		}
	}

	// Token: 0x040025DE RID: 9694
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x040025DF RID: 9695
	[SerializeField]
	private AudioClip _sfxHit;
}
