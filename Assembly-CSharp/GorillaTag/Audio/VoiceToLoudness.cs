using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EE7 RID: 3815
	[RequireComponent(typeof(Recorder))]
	public class VoiceToLoudness : MonoBehaviour
	{
		// Token: 0x06005EAB RID: 24235 RVA: 0x001DD441 File Offset: 0x001DB641
		protected void Awake()
		{
			this._recorder = base.GetComponent<Recorder>();
		}

		// Token: 0x06005EAC RID: 24236 RVA: 0x001DD450 File Offset: 0x001DB650
		protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			VoiceInfo info = photonVoiceCreatedParams.Voice.Info;
			LocalVoiceAudioFloat localVoiceAudioFloat = photonVoiceCreatedParams.Voice as LocalVoiceAudioFloat;
			if (localVoiceAudioFloat != null)
			{
				localVoiceAudioFloat.AddPostProcessor(new IProcessor<float>[]
				{
					new ProcessVoiceDataToLoudness(this)
				});
			}
		}

		// Token: 0x0400691B RID: 26907
		[NonSerialized]
		public float loudness;

		// Token: 0x0400691C RID: 26908
		private Recorder _recorder;
	}
}
