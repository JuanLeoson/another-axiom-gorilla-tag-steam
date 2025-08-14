using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000ADF RID: 2783
public class SoundBankPlayer : MonoBehaviour
{
	// Token: 0x1700065B RID: 1627
	// (get) Token: 0x060042FC RID: 17148 RVA: 0x0014FFE0 File Offset: 0x0014E1E0
	public bool isPlaying
	{
		get
		{
			return Time.realtimeSinceStartup < this.playEndTime;
		}
	}

	// Token: 0x1700065C RID: 1628
	// (get) Token: 0x060042FD RID: 17149 RVA: 0x0014FFEF File Offset: 0x0014E1EF
	public float NormalizedTime
	{
		get
		{
			if (this.clipDuration != 0f)
			{
				return Mathf.Clamp01(this.CurrentTime / this.clipDuration);
			}
			return 1f;
		}
	}

	// Token: 0x1700065D RID: 1629
	// (get) Token: 0x060042FE RID: 17150 RVA: 0x00150016 File Offset: 0x0014E216
	public float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup - this.playStartTime;
		}
	}

	// Token: 0x060042FF RID: 17151 RVA: 0x00150024 File Offset: 0x0014E224
	protected void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.outputAudioMixerGroup = this.outputAudioMixerGroup;
			this.audioSource.spatialize = this.spatialize;
			this.audioSource.spatializePostEffects = this.spatializePostEffects;
			this.audioSource.bypassEffects = this.bypassEffects;
			this.audioSource.bypassListenerEffects = this.bypassListenerEffects;
			this.audioSource.bypassReverbZones = this.bypassReverbZones;
			this.audioSource.priority = this.priority;
			this.audioSource.spatialBlend = this.spatialBlend;
			this.audioSource.dopplerLevel = this.dopplerLevel;
			this.audioSource.spread = this.spread;
			this.audioSource.rolloffMode = this.rolloffMode;
			this.audioSource.minDistance = this.minDistance;
			this.audioSource.maxDistance = this.maxDistance;
			this.audioSource.reverbZoneMix = this.reverbZoneMix;
		}
		this.audioSource.volume = 1f;
		this.audioSource.playOnAwake = false;
		if (this.shuffleOrder)
		{
			int[] array = new int[this.soundBank.sounds.Length / 2];
			this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
			for (int i = 0; i < this.playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = Random.Range(0, this.soundBank.sounds.Length);
					if (Array.IndexOf<int>(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				this.playlist[i] = new SoundBankPlayer.PlaylistEntry
				{
					index = num,
					volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
					pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
				};
			}
			return;
		}
		this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
		for (int k = 0; k < this.playlist.Length; k++)
		{
			this.playlist[k] = new SoundBankPlayer.PlaylistEntry
			{
				index = k % this.soundBank.sounds.Length,
				volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
				pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
			};
		}
	}

	// Token: 0x06004300 RID: 17152 RVA: 0x0015031D File Offset: 0x0014E51D
	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x06004301 RID: 17153 RVA: 0x00150330 File Offset: 0x0014E530
	public void Play()
	{
		this.Play(null, null);
	}

	// Token: 0x06004302 RID: 17154 RVA: 0x00150358 File Offset: 0x0014E558
	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (!base.enabled || this.soundBank.sounds.Length == 0 || this.playlist == null)
		{
			return;
		}
		SoundBankPlayer.PlaylistEntry playlistEntry = this.playlist[this.nextIndex];
		this.audioSource.pitch = ((pitchOverride != null) ? pitchOverride.Value : playlistEntry.pitch);
		AudioClip audioClip = this.soundBank.sounds[playlistEntry.index];
		if (audioClip != null)
		{
			this.audioSource.GTPlayOneShot(audioClip, (volumeOverride != null) ? volumeOverride.Value : playlistEntry.volume);
			this.clipDuration = audioClip.length;
			this.playStartTime = Time.realtimeSinceStartup;
			this.playEndTime = Mathf.Max(this.playEndTime, this.playStartTime + audioClip.length);
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		if (this.missingSoundsAreOk)
		{
			this.clipDuration = 0f;
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		Debug.LogErrorFormat("Sounds bank {0} is missing a clip at {1}", new object[]
		{
			base.gameObject.name,
			playlistEntry.index
		});
	}

	// Token: 0x06004303 RID: 17155 RVA: 0x0015049D File Offset: 0x0014E69D
	public void RestartSequence()
	{
		this.nextIndex = 0;
	}

	// Token: 0x04004DCB RID: 19915
	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	// Token: 0x04004DCC RID: 19916
	public bool playOnEnable = true;

	// Token: 0x04004DCD RID: 19917
	public bool shuffleOrder = true;

	// Token: 0x04004DCE RID: 19918
	public bool missingSoundsAreOk;

	// Token: 0x04004DCF RID: 19919
	public SoundBankSO soundBank;

	// Token: 0x04004DD0 RID: 19920
	public AudioMixerGroup outputAudioMixerGroup;

	// Token: 0x04004DD1 RID: 19921
	public bool spatialize;

	// Token: 0x04004DD2 RID: 19922
	public bool spatializePostEffects;

	// Token: 0x04004DD3 RID: 19923
	public bool bypassEffects;

	// Token: 0x04004DD4 RID: 19924
	public bool bypassListenerEffects;

	// Token: 0x04004DD5 RID: 19925
	public bool bypassReverbZones;

	// Token: 0x04004DD6 RID: 19926
	public int priority = 128;

	// Token: 0x04004DD7 RID: 19927
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04004DD8 RID: 19928
	public float reverbZoneMix = 1f;

	// Token: 0x04004DD9 RID: 19929
	public float dopplerLevel = 1f;

	// Token: 0x04004DDA RID: 19930
	public float spread;

	// Token: 0x04004DDB RID: 19931
	public AudioRolloffMode rolloffMode;

	// Token: 0x04004DDC RID: 19932
	public float minDistance = 1f;

	// Token: 0x04004DDD RID: 19933
	public float maxDistance = 100f;

	// Token: 0x04004DDE RID: 19934
	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04004DDF RID: 19935
	private int nextIndex;

	// Token: 0x04004DE0 RID: 19936
	private float playStartTime;

	// Token: 0x04004DE1 RID: 19937
	private float playEndTime;

	// Token: 0x04004DE2 RID: 19938
	private float clipDuration;

	// Token: 0x04004DE3 RID: 19939
	private SoundBankPlayer.PlaylistEntry[] playlist;

	// Token: 0x02000AE0 RID: 2784
	private struct PlaylistEntry
	{
		// Token: 0x04004DE4 RID: 19940
		public int index;

		// Token: 0x04004DE5 RID: 19941
		public float volume;

		// Token: 0x04004DE6 RID: 19942
		public float pitch;
	}
}
