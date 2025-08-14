using System;
using UnityEngine;

// Token: 0x02000AF8 RID: 2808
public class TriggerEventNotifier : MonoBehaviour
{
	// Token: 0x1400007A RID: 122
	// (add) Token: 0x060043AD RID: 17325 RVA: 0x00154594 File Offset: 0x00152794
	// (remove) Token: 0x060043AE RID: 17326 RVA: 0x001545CC File Offset: 0x001527CC
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	// Token: 0x1400007B RID: 123
	// (add) Token: 0x060043AF RID: 17327 RVA: 0x00154604 File Offset: 0x00152804
	// (remove) Token: 0x060043B0 RID: 17328 RVA: 0x0015463C File Offset: 0x0015283C
	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	// Token: 0x060043B1 RID: 17329 RVA: 0x00154671 File Offset: 0x00152871
	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	// Token: 0x060043B2 RID: 17330 RVA: 0x00154685 File Offset: 0x00152885
	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	// Token: 0x04004E3B RID: 20027
	[HideInInspector]
	public int maskIndex;

	// Token: 0x02000AF9 RID: 2809
	// (Invoke) Token: 0x060043B5 RID: 17333
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}
