using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000032 RID: 50
public class CosmeticPlaySoundOnColision : MonoBehaviour
{
	// Token: 0x060000BB RID: 187 RVA: 0x00005288 File Offset: 0x00003488
	private void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.soundLookup = new Dictionary<int, int>();
		this.audioSource = base.GetComponent<AudioSource>();
		for (int i = 0; i < this.soundIdRemappings.Length; i++)
		{
			this.soundLookup.Add(this.soundIdRemappings[i].SoundIn, this.soundIdRemappings[i].SoundOut);
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x000052F0 File Offset: 0x000034F0
	private void OnTriggerEnter(Collider other)
	{
		GorillaSurfaceOverride gorillaSurfaceOverride;
		if (this.speed >= this.minSpeed && other.TryGetComponent<GorillaSurfaceOverride>(out gorillaSurfaceOverride))
		{
			int soundIndex;
			if (this.soundLookup.TryGetValue(gorillaSurfaceOverride.overrideIndex, out soundIndex))
			{
				this.playSound(soundIndex, this.invokeEventOnOverideSound);
				return;
			}
			this.playSound(this.defaultSound, this.invokeEventOnDefaultSound);
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000534C File Offset: 0x0000354C
	private void playSound(int soundIndex, bool invokeEvent)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
				{
					this.OnStopPlayback.Invoke();
				}
				if (this.crWaitForStopPlayback != null)
				{
					base.StopCoroutine(this.crWaitForStopPlayback);
					this.crWaitForStopPlayback = null;
				}
			}
			this.audioSource.clip = GTPlayer.Instance.materialData[soundIndex].audio;
			this.audioSource.GTPlay();
			if (invokeEvent && (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem()))
			{
				this.OnStartPlayback.Invoke();
				this.crWaitForStopPlayback = base.StartCoroutine(this.waitForStopPlayback());
			}
		}
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00005428 File Offset: 0x00003628
	private IEnumerator waitForStopPlayback()
	{
		while (this.audioSource.isPlaying)
		{
			yield return null;
		}
		if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
		{
			this.OnStopPlayback.Invoke();
		}
		this.crWaitForStopPlayback = null;
		yield break;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00005437 File Offset: 0x00003637
	private void FixedUpdate()
	{
		this.speed = Vector3.Distance(base.transform.position, this.previousFramePosition) * Time.fixedDeltaTime * 100f;
		this.previousFramePosition = base.transform.position;
	}

	// Token: 0x040000CE RID: 206
	[GorillaSoundLookup]
	[SerializeField]
	private int defaultSound = 1;

	// Token: 0x040000CF RID: 207
	[SerializeField]
	private SoundIdRemapping[] soundIdRemappings;

	// Token: 0x040000D0 RID: 208
	[SerializeField]
	private UnityEvent OnStartPlayback;

	// Token: 0x040000D1 RID: 209
	[SerializeField]
	private UnityEvent OnStopPlayback;

	// Token: 0x040000D2 RID: 210
	[SerializeField]
	private float minSpeed = 0.1f;

	// Token: 0x040000D3 RID: 211
	private TransferrableObject transferrableObject;

	// Token: 0x040000D4 RID: 212
	private Dictionary<int, int> soundLookup;

	// Token: 0x040000D5 RID: 213
	private AudioSource audioSource;

	// Token: 0x040000D6 RID: 214
	private Coroutine crWaitForStopPlayback;

	// Token: 0x040000D7 RID: 215
	private float speed;

	// Token: 0x040000D8 RID: 216
	private Vector3 previousFramePosition;

	// Token: 0x040000D9 RID: 217
	[SerializeField]
	private bool invokeEventsOnAllClients;

	// Token: 0x040000DA RID: 218
	[SerializeField]
	private bool invokeEventOnOverideSound = true;

	// Token: 0x040000DB RID: 219
	[SerializeField]
	private bool invokeEventOnDefaultSound;
}
