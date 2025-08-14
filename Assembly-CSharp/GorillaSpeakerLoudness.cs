using System;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Oculus.VoiceSDK.Utilities;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class GorillaSpeakerLoudness : MonoBehaviour, IGorillaSliceableSimple, IDynamicFloat
{
	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x06002CAD RID: 11437 RVA: 0x000EC4B2 File Offset: 0x000EA6B2
	public bool IsSpeaking
	{
		get
		{
			return this.isSpeaking;
		}
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06002CAE RID: 11438 RVA: 0x000EC4BA File Offset: 0x000EA6BA
	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x06002CAF RID: 11439 RVA: 0x000EC4C2 File Offset: 0x000EA6C2
	public float LoudnessNormalized
	{
		get
		{
			return Mathf.Min(this.loudness / this.normalizedMax, 1f);
		}
	}

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x06002CB0 RID: 11440 RVA: 0x000EC4DB File Offset: 0x000EA6DB
	public float floatValue
	{
		get
		{
			return this.LoudnessNormalized;
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002CB1 RID: 11441 RVA: 0x000EC4E3 File Offset: 0x000EA6E3
	public bool IsMicEnabled
	{
		get
		{
			return this.isMicEnabled;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002CB2 RID: 11442 RVA: 0x000EC4EB File Offset: 0x000EA6EB
	public float SmoothedLoudness
	{
		get
		{
			return this.smoothedLoudness;
		}
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x000EC4F3 File Offset: 0x000EA6F3
	private void Start()
	{
		this.rigContainer = base.GetComponent<RigContainer>();
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x000EC517 File Offset: 0x000EA717
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.UpdateMicEnabled();
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x000EC548 File Offset: 0x000EA748
	private void UpdateMicEnabled()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig)
		{
			this.permission = (this.permission || MicPermissionsManager.HasMicPermission());
			if (this.permission && !this.micConnected && Microphone.devices != null)
			{
				this.micConnected = (Microphone.devices.Length != 0);
			}
			this.isMicEnabled = (this.permission && this.micConnected);
			rig.IsMicEnabled = this.isMicEnabled;
			return;
		}
		this.isMicEnabled = rig.IsMicEnabled;
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x000EC5E4 File Offset: 0x000EA7E4
	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		PhotonVoiceView voice = this.rigContainer.Voice;
		if (voice != null && this.speaker == null)
		{
			this.speaker = voice.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = ((voice != null) ? voice.RecorderInUse : null);
		}
		VRRig rig = this.rigContainer.Rig;
		if ((rig.remoteUseReplacementVoice || rig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && rig.SpeakingLoudness > 0f && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
		{
			this.isSpeaking = true;
			this.loudness = rig.SpeakingLoudness;
			return;
		}
		if (voice != null && voice.IsSpeaking)
		{
			this.isSpeaking = true;
			if (!(this.speaker != null))
			{
				this.loudness = 0f;
				return;
			}
			if (this.speakerVoiceToLoudness == null)
			{
				this.speakerVoiceToLoudness = this.speaker.GetComponent<SpeakerVoiceToLoudness>();
			}
			if (this.speakerVoiceToLoudness != null)
			{
				this.loudness = this.speakerVoiceToLoudness.loudness;
				return;
			}
		}
		else if (voice != null && this.recorder != null && NetworkSystem.Instance.IsObjectLocallyOwned(voice.gameObject) && this.recorder.IsCurrentlyTransmitting)
		{
			if (this.voiceToLoudness == null)
			{
				this.voiceToLoudness = this.recorder.GetComponent<VoiceToLoudness>();
			}
			this.isSpeaking = true;
			if (this.voiceToLoudness != null)
			{
				this.loudness = this.voiceToLoudness.loudness;
				return;
			}
			this.loudness = 0f;
			return;
		}
		else
		{
			this.isSpeaking = false;
			this.loudness = 0f;
		}
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x000EC7CC File Offset: 0x000EA9CC
	private void UpdateSmoothedLoudness()
	{
		if (!this.isSpeaking)
		{
			this.smoothedLoudness = 0f;
			return;
		}
		if (!Mathf.Approximately(this.loudness, this.lastLoudness))
		{
			this.timeSinceLoudnessChange = 0f;
			this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
			this.lastLoudness = this.loudness;
			return;
		}
		if (this.timeSinceLoudnessChange > this.loudnessUpdateCheckRate)
		{
			this.smoothedLoudness = 0.001f;
			return;
		}
		this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
		this.timeSinceLoudnessChange += this.deltaTime;
	}

	// Token: 0x0400381F RID: 14367
	private bool isSpeaking;

	// Token: 0x04003820 RID: 14368
	private float loudness;

	// Token: 0x04003821 RID: 14369
	[SerializeField]
	private float normalizedMax = 0.175f;

	// Token: 0x04003822 RID: 14370
	private bool isMicEnabled;

	// Token: 0x04003823 RID: 14371
	private RigContainer rigContainer;

	// Token: 0x04003824 RID: 14372
	private Speaker speaker;

	// Token: 0x04003825 RID: 14373
	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	// Token: 0x04003826 RID: 14374
	private Recorder recorder;

	// Token: 0x04003827 RID: 14375
	private VoiceToLoudness voiceToLoudness;

	// Token: 0x04003828 RID: 14376
	private float smoothedLoudness;

	// Token: 0x04003829 RID: 14377
	private float lastLoudness;

	// Token: 0x0400382A RID: 14378
	private float timeSinceLoudnessChange;

	// Token: 0x0400382B RID: 14379
	private float loudnessUpdateCheckRate = 0.2f;

	// Token: 0x0400382C RID: 14380
	private float loudnessBlendStrength = 2f;

	// Token: 0x0400382D RID: 14381
	private bool permission;

	// Token: 0x0400382E RID: 14382
	private bool micConnected;

	// Token: 0x0400382F RID: 14383
	private float timeLastUpdated;

	// Token: 0x04003830 RID: 14384
	private float deltaTime;
}
