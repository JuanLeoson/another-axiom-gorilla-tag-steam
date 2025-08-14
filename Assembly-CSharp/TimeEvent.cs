using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B1C RID: 2844
public class TimeEvent : MonoBehaviour
{
	// Token: 0x06004481 RID: 17537 RVA: 0x001566FF File Offset: 0x001548FF
	protected void StartEvent()
	{
		this._ongoing = true;
		UnityEvent unityEvent = this.onEventStart;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06004482 RID: 17538 RVA: 0x00156718 File Offset: 0x00154918
	protected void StopEvent()
	{
		this._ongoing = false;
		UnityEvent unityEvent = this.onEventStop;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04004EAA RID: 20138
	public UnityEvent onEventStart;

	// Token: 0x04004EAB RID: 20139
	public UnityEvent onEventStop;

	// Token: 0x04004EAC RID: 20140
	[SerializeField]
	protected bool _ongoing;
}
