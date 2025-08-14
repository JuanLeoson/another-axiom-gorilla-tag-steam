using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020007A1 RID: 1953
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06003120 RID: 12576 RVA: 0x00100376 File Offset: 0x000FE576
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x0010037F File Offset: 0x000FE57F
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04003CC0 RID: 15552
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04003CC1 RID: 15553
	public AudioMixerSnapshot underwaterSnapshot;
}
