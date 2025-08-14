using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class CrittersVoiceNoise : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060002C6 RID: 710 RVA: 0x00010F61 File Offset: 0x0000F161
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00010F84 File Offset: 0x0000F184
	public void SliceUpdate()
	{
		float num = 0f;
		if (this.speaker.IsSpeaking)
		{
			num = this.speaker.Loudness;
		}
		if (num > this.minTriggerThreshold && CrittersManager.instance.IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.rig].rigActors[4].actorSet;
			if (crittersLoudNoise.IsNotNull() && !crittersLoudNoise.soundEnabled)
			{
				float volume = Mathf.Lerp(this.noiseVolumeMin, this.noisVolumeMax, Mathf.Clamp01((num - this.minTriggerThreshold) / this.maxTriggerThreshold));
				crittersLoudNoise.PlayVoiceSpeechLocal(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), 0.016666668f, volume);
			}
		}
	}

	// Token: 0x04000335 RID: 821
	[SerializeField]
	private GorillaSpeakerLoudness speaker;

	// Token: 0x04000336 RID: 822
	[SerializeField]
	private VRRig rig;

	// Token: 0x04000337 RID: 823
	[SerializeField]
	private float minTriggerThreshold = 0.01f;

	// Token: 0x04000338 RID: 824
	[SerializeField]
	private float maxTriggerThreshold = 0.3f;

	// Token: 0x04000339 RID: 825
	[SerializeField]
	private float noiseVolumeMin = 1f;

	// Token: 0x0400033A RID: 826
	[SerializeField]
	private float noisVolumeMax = 9f;
}
