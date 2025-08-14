using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C50 RID: 3152
	public class UnMuteAudioSourceOnEnable : MonoBehaviour
	{
		// Token: 0x06004DFC RID: 19964 RVA: 0x00183928 File Offset: 0x00181B28
		public void Awake()
		{
			this.originalVolume = this.audioSource.volume;
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x0018393B File Offset: 0x00181B3B
		public void OnEnable()
		{
			this.audioSource.volume = this.originalVolume;
		}

		// Token: 0x06004DFE RID: 19966 RVA: 0x0018394E File Offset: 0x00181B4E
		public void OnDisable()
		{
			this.audioSource.volume = 0f;
		}

		// Token: 0x040056F9 RID: 22265
		public AudioSource audioSource;

		// Token: 0x040056FA RID: 22266
		public float originalVolume;
	}
}
