using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005EB RID: 1515
[Serializable]
public class GameAbilityEvent
{
	// Token: 0x06002537 RID: 9527 RVA: 0x000C8473 File Offset: 0x000C6673
	public void Reset()
	{
		this.played = false;
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000C847C File Offset: 0x000C667C
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		if (abilityTime < this.time || this.played)
		{
			return;
		}
		this.played = true;
		if (this.sound.IsValid())
		{
			this.sound.Play(audioSource);
		}
		for (int i = 0; i < this.triggerEvent.Count; i++)
		{
			this.triggerEvent[i].Invoke();
		}
	}

	// Token: 0x04002F16 RID: 12054
	public float time;

	// Token: 0x04002F17 RID: 12055
	public AbilitySound sound;

	// Token: 0x04002F18 RID: 12056
	public List<UnityEvent> triggerEvent;

	// Token: 0x04002F19 RID: 12057
	[NonSerialized]
	public bool played;
}
