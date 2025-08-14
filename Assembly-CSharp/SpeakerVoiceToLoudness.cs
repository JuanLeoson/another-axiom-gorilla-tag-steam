using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200024A RID: 586
[RequireComponent(typeof(Speaker))]
public class SpeakerVoiceToLoudness : MonoBehaviour
{
	// Token: 0x06000DB5 RID: 3509 RVA: 0x00053EF4 File Offset: 0x000520F4
	private void Awake()
	{
		Speaker component = base.GetComponent<Speaker>();
		component.CustomAudioOutFactory = this.GetVolumeTracking(component);
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00053F18 File Offset: 0x00052118
	private Func<IAudioOut<float>> GetVolumeTracking(Speaker speaker)
	{
		AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig
		{
			Low = this.playbackDelaySettings.MinDelaySoft,
			High = this.playbackDelaySettings.MaxDelaySoft,
			Max = this.playbackDelaySettings.MaxDelayHard
		};
		return () => new SpeakerVoiceLoudnessAudioOut(this, speaker.GetComponent<AudioSource>(), pdc, speaker.Logger, string.Empty, speaker.Logger.IsDebugEnabled);
	}

	// Token: 0x04001589 RID: 5513
	[SerializeField]
	private PlaybackDelaySettings playbackDelaySettings = new PlaybackDelaySettings
	{
		MinDelaySoft = 200,
		MaxDelaySoft = 400,
		MaxDelayHard = 1000
	};

	// Token: 0x0400158A RID: 5514
	public float loudness;
}
