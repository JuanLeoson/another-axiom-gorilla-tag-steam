using System;
using System.Collections.Generic;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Audio
{
	// Token: 0x02000EED RID: 3821
	public class GTSpeaker : Speaker
	{
		// Token: 0x06005EC9 RID: 24265 RVA: 0x001DDFB0 File Offset: 0x001DC1B0
		public void Start()
		{
			LoudSpeakerNetwork componentInChildren = base.transform.root.GetComponentInChildren<LoudSpeakerNetwork>();
			if (componentInChildren != null)
			{
				this.AddExternalAudioSources(componentInChildren.SpeakerSources);
			}
		}

		// Token: 0x06005ECA RID: 24266 RVA: 0x001DDFE3 File Offset: 0x001DC1E3
		public void AddExternalAudioSources(AudioSource[] audioSources)
		{
			if (this._initializedExternalAudioSources)
			{
				return;
			}
			this._externalAudioSources = audioSources;
			this.InitializeExternalAudioSources();
			if (this._audioOutputStarted)
			{
				this.ExternalAudioOutputStart(this._frequency, this._channels, this._frameSamplesPerChannel);
			}
		}

		// Token: 0x06005ECB RID: 24267 RVA: 0x001DE01B File Offset: 0x001DC21B
		protected override void Initialize()
		{
			if (base.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already initialized.", Array.Empty<object>());
				}
				return;
			}
			base.Initialize();
		}

		// Token: 0x06005ECC RID: 24268 RVA: 0x001DE050 File Offset: 0x001DC250
		private void InitializeExternalAudioSources()
		{
			this._initializedExternalAudioSources = true;
			this._externalAudioOutputs = new List<IAudioOut<float>>();
			AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig
			{
				Low = this.playbackDelaySettings.MinDelaySoft,
				High = this.playbackDelaySettings.MaxDelaySoft,
				Max = this.playbackDelaySettings.MaxDelayHard
			};
			foreach (AudioSource source in this._externalAudioSources)
			{
				this._externalAudioOutputs.Add(this.GetAudioOutFactoryFromSource(source, pdc)());
			}
		}

		// Token: 0x06005ECD RID: 24269 RVA: 0x001DE0D9 File Offset: 0x001DC2D9
		private Func<IAudioOut<float>> GetAudioOutFactoryFromSource(AudioSource source, AudioOutDelayControl.PlayDelayConfig pdc)
		{
			return () => new UnityAudioOut(source, pdc, this.Logger, string.Empty, this.Logger.IsDebugEnabled);
		}

		// Token: 0x06005ECE RID: 24270 RVA: 0x001DE100 File Offset: 0x001DC300
		protected override void OnAudioFrame(FrameOut<float> frame)
		{
			base.OnAudioFrame(frame);
			if (this.BroadcastExternal)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Push(frame.Buf);
					if (frame.EndOfStream)
					{
						audioOut.Flush();
					}
				}
			}
		}

		// Token: 0x06005ECF RID: 24271 RVA: 0x001DE178 File Offset: 0x001DC378
		protected override void AudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			this._audioOutputStarted = true;
			this._frequency = frequency;
			this._channels = channels;
			this._frameSamplesPerChannel = frameSamplesPerChannel;
			base.AudioOutputStart(frequency, channels, frameSamplesPerChannel);
			this.ExternalAudioOutputStart(frequency, channels, frameSamplesPerChannel);
		}

		// Token: 0x06005ED0 RID: 24272 RVA: 0x001DE1A8 File Offset: 0x001DC3A8
		private void ExternalAudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Start(frequency, channels, frameSamplesPerChannel);
						audioOut.ToggleAudioSource(false);
					}
				}
			}
		}

		// Token: 0x06005ED1 RID: 24273 RVA: 0x001DE214 File Offset: 0x001DC414
		protected override void AudioOutputStop()
		{
			this._audioOutputStarted = false;
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Stop();
				}
			}
			base.AudioOutputStop();
		}

		// Token: 0x06005ED2 RID: 24274 RVA: 0x001DE27C File Offset: 0x001DC47C
		protected override void AudioOutputService()
		{
			base.AudioOutputService();
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Service();
					}
				}
			}
		}

		// Token: 0x06005ED3 RID: 24275 RVA: 0x001DE2E4 File Offset: 0x001DC4E4
		public void ToggleAudioSource(bool toggle)
		{
			if (this._externalAudioOutputs == null)
			{
				return;
			}
			foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
			{
				audioOut.ToggleAudioSource(toggle);
			}
		}

		// Token: 0x0400693B RID: 26939
		[FormerlySerializedAs("UseExternalAudioSources")]
		public bool BroadcastExternal;

		// Token: 0x0400693C RID: 26940
		[SerializeField]
		private AudioSource[] _externalAudioSources;

		// Token: 0x0400693D RID: 26941
		private List<IAudioOut<float>> _externalAudioOutputs;

		// Token: 0x0400693E RID: 26942
		private int _frequency;

		// Token: 0x0400693F RID: 26943
		private int _channels;

		// Token: 0x04006940 RID: 26944
		private int _frameSamplesPerChannel;

		// Token: 0x04006941 RID: 26945
		private bool _initializedExternalAudioSources;

		// Token: 0x04006942 RID: 26946
		private bool _audioOutputStarted;
	}
}
