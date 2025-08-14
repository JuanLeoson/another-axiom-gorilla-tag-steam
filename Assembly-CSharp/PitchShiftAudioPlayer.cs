using System;
using UnityEngine;

// Token: 0x020007A0 RID: 1952
public class PitchShiftAudioPlayer : MonoBehaviour
{
	// Token: 0x0600311A RID: 12570 RVA: 0x001002B6 File Offset: 0x000FE4B6
	private void Awake()
	{
		if (this._source == null)
		{
			this._source = base.GetComponent<AudioSource>();
		}
		if (this._pitch == null)
		{
			this._pitch = base.GetComponent<RangedFloat>();
		}
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x001002EC File Offset: 0x000FE4EC
	private void OnEnable()
	{
		this._pitchMixVars.Rent(out this._pitchMix);
		this._source.outputAudioMixerGroup = this._pitchMix.group;
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x00100316 File Offset: 0x000FE516
	private void OnDisable()
	{
		this._source.Stop();
		this._source.outputAudioMixerGroup = null;
		AudioMixVar pitchMix = this._pitchMix;
		if (pitchMix == null)
		{
			return;
		}
		pitchMix.ReturnToPool();
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x0010033F File Offset: 0x000FE53F
	private void Update()
	{
		if (this.apply)
		{
			this.ApplyPitch();
		}
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x0010034F File Offset: 0x000FE54F
	private void ApplyPitch()
	{
		this._pitchMix.value = this._pitch.curved;
	}

	// Token: 0x04003CBB RID: 15547
	public bool apply = true;

	// Token: 0x04003CBC RID: 15548
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04003CBD RID: 15549
	[SerializeField]
	private AudioMixVarPool _pitchMixVars;

	// Token: 0x04003CBE RID: 15550
	[SerializeReference]
	private AudioMixVar _pitchMix;

	// Token: 0x04003CBF RID: 15551
	[SerializeField]
	private RangedFloat _pitch;
}
