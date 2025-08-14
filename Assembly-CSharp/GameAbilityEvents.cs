using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005EC RID: 1516
[Serializable]
public class GameAbilityEvents
{
	// Token: 0x0600253A RID: 9530 RVA: 0x000C84E4 File Offset: 0x000C66E4
	public void Reset()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].Reset();
		}
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x000C8518 File Offset: 0x000C6718
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].TryPlay(abilityTime, audioSource);
		}
	}

	// Token: 0x04002F1A RID: 12058
	public List<GameAbilityEvent> events;
}
