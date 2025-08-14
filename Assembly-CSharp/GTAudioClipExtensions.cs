using System;
using UnityEngine;

// Token: 0x020001EB RID: 491
public static class GTAudioClipExtensions
{
	// Token: 0x06000BB5 RID: 2997 RVA: 0x0004077C File Offset: 0x0003E97C
	public static float GetPeakMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = float.NegativeInfinity;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float f in array)
		{
			num = Mathf.Max(num, Mathf.Abs(f));
		}
		return num;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x000407D8 File Offset: 0x0003E9D8
	public static float GetRMSMagnitude(this AudioClip audioClip)
	{
		if (audioClip == null)
		{
			return 0f;
		}
		float num = 0f;
		float[] array = new float[audioClip.samples];
		audioClip.GetData(array, 0);
		foreach (float num2 in array)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}
}
