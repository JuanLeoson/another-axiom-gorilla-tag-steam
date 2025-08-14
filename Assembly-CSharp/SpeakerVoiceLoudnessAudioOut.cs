using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class SpeakerVoiceLoudnessAudioOut : UnityAudioOut
{
	// Token: 0x06000DBA RID: 3514 RVA: 0x00054008 File Offset: 0x00052208
	public SpeakerVoiceLoudnessAudioOut(SpeakerVoiceToLoudness speaker, AudioSource audioSource, AudioOutDelayControl.PlayDelayConfig playDelayConfig, Photon.Voice.ILogger logger, string logPrefix, bool debugInfo) : base(audioSource, playDelayConfig, logger, logPrefix, debugInfo)
	{
		this.voiceToLoudness = speaker;
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00054020 File Offset: 0x00052220
	public override void OutWrite(float[] data, int offsetSamples)
	{
		float num = 0f;
		for (int i = 0; i < data.Length; i++)
		{
			float num2 = data[i];
			if (!float.IsFinite(num2))
			{
				num2 = 0f;
				data[i] = num2;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
				data[i] = num2;
			}
			else if (num2 < -1f)
			{
				num2 = -1f;
				data[i] = num2;
			}
			num += Mathf.Abs(num2);
		}
		if (num > 0f)
		{
			this.voiceToLoudness.loudness = num / (float)data.Length;
		}
		base.OutWrite(data, offsetSamples);
	}

	// Token: 0x0400158E RID: 5518
	private SpeakerVoiceToLoudness voiceToLoudness;
}
