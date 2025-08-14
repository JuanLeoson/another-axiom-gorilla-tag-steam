using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class MicrophoneCosmetic : MonoBehaviour
{
	// Token: 0x060006F0 RID: 1776 RVA: 0x00027610 File Offset: 0x00025810
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0)
		{
			this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 16000);
		}
		else
		{
			int sampleRate = AudioSettings.GetConfiguration().sampleRate;
			this.audioSource.clip = Microphone.Start(null, true, 10, sampleRate);
		}
		this.audioSource.loop = true;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00027690 File Offset: 0x00025890
	private void OnEnable()
	{
		int num = (Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0) ? Microphone.GetPosition(Microphone.devices[0]) : Microphone.GetPosition(null);
		num -= 10;
		if ((float)num < 0f)
		{
			num = this.audioSource.clip.samples + num - 1;
		}
		this.audioSource.GTPlay();
		this.audioSource.timeSamples = num;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x000276FD File Offset: 0x000258FD
	private void OnDisable()
	{
		this.audioSource.GTStop();
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0002770C File Offset: 0x0002590C
	private void Update()
	{
		Vector3 vector = this.mouthTransform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude < this.mouthProximityRampRange.x * this.mouthProximityRampRange.x)
		{
			float magnitude = vector.magnitude;
			num = Mathf.InverseLerp(this.mouthProximityRampRange.x, this.mouthProximityRampRange.y, magnitude);
		}
		if (num != this.audioSource.volume)
		{
			this.audioSource.volume = num;
		}
		int num2 = this.audioSource.timeSamples -= 10;
		if ((float)num2 < 0f)
		{
			num2 = this.audioSource.clip.samples + num2 - 1;
		}
		this.audioSource.clip.SetData(this.zero, num2);
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnAudioFilterRead(float[] data, int channels)
	{
	}

	// Token: 0x04000853 RID: 2131
	[SerializeField]
	private Transform mouthTransform;

	// Token: 0x04000854 RID: 2132
	[SerializeField]
	private Vector2 mouthProximityRampRange = new Vector2(0.6f, 0.3f);

	// Token: 0x04000855 RID: 2133
	private AudioSource audioSource;

	// Token: 0x04000856 RID: 2134
	private float[] zero = new float[1];
}
