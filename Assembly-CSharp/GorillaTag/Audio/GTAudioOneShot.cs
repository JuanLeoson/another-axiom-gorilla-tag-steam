using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EE5 RID: 3813
	internal static class GTAudioOneShot
	{
		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06005EA3 RID: 24227 RVA: 0x001DD2B3 File Offset: 0x001DB4B3
		// (set) Token: 0x06005EA4 RID: 24228 RVA: 0x001DD2BA File Offset: 0x001DB4BA
		[OnEnterPlay_Set(false)]
		internal static bool isInitialized { get; private set; }

		// Token: 0x06005EA5 RID: 24229 RVA: 0x001DD2C4 File Offset: 0x001DB4C4
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		// Token: 0x06005EA6 RID: 24230 RVA: 0x001DD323 File Offset: 0x001DB523
		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
		}

		// Token: 0x06005EA7 RID: 24231 RVA: 0x001DD35B File Offset: 0x001DB55B
		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
		}

		// Token: 0x04006915 RID: 26901
		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		// Token: 0x04006916 RID: 26902
		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;
	}
}
