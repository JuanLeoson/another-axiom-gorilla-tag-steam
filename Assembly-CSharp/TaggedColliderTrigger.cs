using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000891 RID: 2193
public class TaggedColliderTrigger : MonoBehaviour
{
	// Token: 0x06003731 RID: 14129 RVA: 0x0011F15E File Offset: 0x0011D35E
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastEnter.HasElapsed(this.enterHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x06003732 RID: 14130 RVA: 0x0011F194 File Offset: 0x0011D394
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastExit.HasElapsed(this.exitHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x040043D4 RID: 17364
	public new UnityTag tag;

	// Token: 0x040043D5 RID: 17365
	public UnityEvent<Collider> onEnter = new UnityEvent<Collider>();

	// Token: 0x040043D6 RID: 17366
	public UnityEvent<Collider> onExit = new UnityEvent<Collider>();

	// Token: 0x040043D7 RID: 17367
	public float enterHysteresis = 0.125f;

	// Token: 0x040043D8 RID: 17368
	public float exitHysteresis = 0.125f;

	// Token: 0x040043D9 RID: 17369
	private TimeSince _sinceLastEnter;

	// Token: 0x040043DA RID: 17370
	private TimeSince _sinceLastExit;
}
