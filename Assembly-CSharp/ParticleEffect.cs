using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffect : MonoBehaviour
{
	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x0003A42B File Offset: 0x0003862B
	public long effectID
	{
		get
		{
			return this._effectID;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x0003A433 File Offset: 0x00038633
	public bool isPlaying
	{
		get
		{
			return this.system && this.system.isPlaying;
		}
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0003A44F File Offset: 0x0003864F
	public virtual void Play()
	{
		base.gameObject.SetActive(true);
		this.system.Play(true);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0003A469 File Offset: 0x00038669
	public virtual void Stop()
	{
		this.system.Stop(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0003A483 File Offset: 0x00038683
	private void OnParticleSystemStopped()
	{
		base.gameObject.SetActive(false);
		if (this.pool)
		{
			this.pool.Return(this);
		}
	}

	// Token: 0x04000D4B RID: 3403
	public ParticleSystem system;

	// Token: 0x04000D4C RID: 3404
	[SerializeField]
	private long _effectID;

	// Token: 0x04000D4D RID: 3405
	public ParticleEffectsPool pool;

	// Token: 0x04000D4E RID: 3406
	[NonSerialized]
	public int poolIndex = -1;
}
