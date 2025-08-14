using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class MonkeVoteProximityTrigger : GorillaTriggerBox
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x060007D3 RID: 2003 RVA: 0x0002C210 File Offset: 0x0002A410
	// (remove) Token: 0x060007D4 RID: 2004 RVA: 0x0002C248 File Offset: 0x0002A448
	public event Action OnEnter;

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x060007D5 RID: 2005 RVA: 0x0002C27D File Offset: 0x0002A47D
	// (set) Token: 0x060007D6 RID: 2006 RVA: 0x0002C285 File Offset: 0x0002A485
	public bool isPlayerNearby { get; private set; }

	// Token: 0x060007D7 RID: 2007 RVA: 0x0002C28E File Offset: 0x0002A48E
	public override void OnBoxTriggered()
	{
		this.isPlayerNearby = true;
		if (this.triggerTime + this.retriggerDelay < Time.unscaledTime)
		{
			this.triggerTime = Time.unscaledTime;
			Action onEnter = this.OnEnter;
			if (onEnter == null)
			{
				return;
			}
			onEnter();
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002C2C6 File Offset: 0x0002A4C6
	public override void OnBoxExited()
	{
		this.isPlayerNearby = false;
	}

	// Token: 0x04000972 RID: 2418
	private float triggerTime = float.MinValue;

	// Token: 0x04000973 RID: 2419
	private float retriggerDelay = 0.25f;
}
