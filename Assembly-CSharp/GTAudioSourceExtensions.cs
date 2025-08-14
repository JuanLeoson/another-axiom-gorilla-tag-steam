using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020001EC RID: 492
public static class GTAudioSourceExtensions
{
	// Token: 0x06000BB7 RID: 2999 RVA: 0x00040837 File Offset: 0x0003EA37
	public static void GTPlayOneShot(this AudioSource audioSource, IList<AudioClip> clips, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volumeScale);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00040852 File Offset: 0x0003EA52
	public static void GTPlayOneShot(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0004085C File Offset: 0x0003EA5C
	public static void GTPlay(this AudioSource audioSource)
	{
		audioSource.Play();
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x00040864 File Offset: 0x0003EA64
	public static void GTPlay(this AudioSource audioSource, ulong delay)
	{
		audioSource.Play(delay);
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0004086D File Offset: 0x0003EA6D
	public static void GTPause(this AudioSource audioSource)
	{
		audioSource.Pause();
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x00040875 File Offset: 0x0003EA75
	public static void GTUnPause(this AudioSource audioSource)
	{
		audioSource.UnPause();
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0004087D File Offset: 0x0003EA7D
	public static void GTStop(this AudioSource audioSource)
	{
		audioSource.Stop();
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00040885 File Offset: 0x0003EA85
	public static void GTPlayDelayed(this AudioSource audioSource, float delay)
	{
		audioSource.PlayDelayed(delay);
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0004088E File Offset: 0x0003EA8E
	public static void GTPlayScheduled(this AudioSource audioSource, double time)
	{
		audioSource.PlayScheduled(time);
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00040897 File Offset: 0x0003EA97
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x000408A0 File Offset: 0x0003EAA0
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x000023F5 File Offset: 0x000005F5
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	private static void _BetaLogIfAudioSourceIsNotActiveAndEnabled(AudioSource audioSource)
	{
	}
}
