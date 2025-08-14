using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D26 RID: 3366
	public class CowController : MonoBehaviour
	{
		// Token: 0x06005351 RID: 21329 RVA: 0x000023F5 File Offset: 0x000005F5
		private void Start()
		{
		}

		// Token: 0x06005352 RID: 21330 RVA: 0x0019C744 File Offset: 0x0019A944
		public void PlayMooSound()
		{
			this._mooCowAudioSource.timeSamples = 0;
			this._mooCowAudioSource.GTPlay();
		}

		// Token: 0x06005353 RID: 21331 RVA: 0x0019C75D File Offset: 0x0019A95D
		public void GoMooCowGo()
		{
			this._cowAnimation.Rewind();
			this._cowAnimation.Play();
		}

		// Token: 0x04005C91 RID: 23697
		[SerializeField]
		private Animation _cowAnimation;

		// Token: 0x04005C92 RID: 23698
		[SerializeField]
		private AudioSource _mooCowAudioSource;
	}
}
