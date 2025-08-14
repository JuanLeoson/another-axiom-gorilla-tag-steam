using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EE8 RID: 3816
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x06005EAE RID: 24238 RVA: 0x001DD48D File Offset: 0x001DB68D
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x06005EAF RID: 24239 RVA: 0x001DD49C File Offset: 0x001DB69C
		public float[] Process(float[] buf)
		{
			float num = 0f;
			for (int i = 0; i < buf.Length; i++)
			{
				num += Mathf.Abs(buf[i]);
			}
			this._voiceToLoudness.loudness = num / (float)buf.Length;
			return buf;
		}

		// Token: 0x06005EB0 RID: 24240 RVA: 0x000023F5 File Offset: 0x000005F5
		public void Dispose()
		{
		}

		// Token: 0x0400691D RID: 26909
		private VoiceToLoudness _voiceToLoudness;
	}
}
