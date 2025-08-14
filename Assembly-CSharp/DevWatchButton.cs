using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000099 RID: 153
public class DevWatchButton : MonoBehaviour
{
	// Token: 0x060003C4 RID: 964 RVA: 0x00017074 File Offset: 0x00015274
	public void OnTriggerEnter(Collider other)
	{
		this.SearchEvent.Invoke();
	}

	// Token: 0x0400044C RID: 1100
	public UnityEvent SearchEvent = new UnityEvent();
}
