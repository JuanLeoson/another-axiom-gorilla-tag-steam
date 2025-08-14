using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020007A8 RID: 1960
public class ReplacementVoice : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003146 RID: 12614 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x00100A9C File Offset: 0x000FEC9C
	public void SliceUpdate()
	{
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.ShouldPlayReplacementVoice())
		{
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < this.loudReplacementVoiceThreshold)
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClips[Random.Range(0, this.replacementVoiceClips.Length - 1)];
				this.replacementVoiceSource.volume = this.normalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClipsLoud[Random.Range(0, this.replacementVoiceClipsLoud.Length - 1)];
				this.replacementVoiceSource.volume = this.loudVolume;
			}
			this.replacementVoiceSource.GTPlay();
			return;
		}
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride, out cosmeticEffect))
		{
			if (this.myVRRig.SpeakingLoudness < this.myVRRig.replacementVoiceLoudnessThreshold)
			{
				return;
			}
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < cosmeticEffect.voiceOverrideLoudThreshold)
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideNormalClips[Random.Range(0, cosmeticEffect.voiceOverrideNormalClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideNormalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideLoudClips[Random.Range(0, cosmeticEffect.voiceOverrideLoudClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideLoudVolume;
			}
			this.replacementVoiceSource.GTPlay();
		}
	}

	// Token: 0x04003CE0 RID: 15584
	public AudioSource replacementVoiceSource;

	// Token: 0x04003CE1 RID: 15585
	public AudioClip[] replacementVoiceClips;

	// Token: 0x04003CE2 RID: 15586
	public AudioClip[] replacementVoiceClipsLoud;

	// Token: 0x04003CE3 RID: 15587
	public float loudReplacementVoiceThreshold = 0.1f;

	// Token: 0x04003CE4 RID: 15588
	public VRRig myVRRig;

	// Token: 0x04003CE5 RID: 15589
	public float normalVolume = 0.5f;

	// Token: 0x04003CE6 RID: 15590
	public float loudVolume = 0.8f;
}
