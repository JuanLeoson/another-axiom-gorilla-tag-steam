using System;
using UnityEngine;

// Token: 0x02000AA6 RID: 2726
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x14000076 RID: 118
	// (add) Token: 0x060041F2 RID: 16882 RVA: 0x0014C474 File Offset: 0x0014A674
	// (remove) Token: 0x060041F3 RID: 16883 RVA: 0x0014C4AC File Offset: 0x0014A6AC
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x060041F4 RID: 16884 RVA: 0x0014C4E4 File Offset: 0x0014A6E4
	// (remove) Token: 0x060041F5 RID: 16885 RVA: 0x0014C51C File Offset: 0x0014A71C
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x060041F6 RID: 16886 RVA: 0x0014C551 File Offset: 0x0014A751
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x060041F7 RID: 16887 RVA: 0x0014C565 File Offset: 0x0014A765
	private void OnCollisionExit(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionExitEvent = this.CollisionExitEvent;
		if (collisionExitEvent == null)
		{
			return;
		}
		collisionExitEvent(this, collision);
	}

	// Token: 0x02000AA7 RID: 2727
	// (Invoke) Token: 0x060041FA RID: 16890
	public delegate void CollisionEvent(CollisionEventNotifier notifier, Collision collision);
}
