using System;
using UnityEngine;

// Token: 0x02000A79 RID: 2681
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x17000639 RID: 1593
	// (get) Token: 0x06004155 RID: 16725 RVA: 0x0014A8B7 File Offset: 0x00148AB7
	// (set) Token: 0x06004156 RID: 16726 RVA: 0x0014A8BF File Offset: 0x00148ABF
	public bool PreTickRunning { get; set; }

	// Token: 0x1700063A RID: 1594
	// (get) Token: 0x06004157 RID: 16727 RVA: 0x0014A8C8 File Offset: 0x00148AC8
	// (set) Token: 0x06004158 RID: 16728 RVA: 0x0014A8D0 File Offset: 0x00148AD0
	public bool TickRunning { get; set; }

	// Token: 0x1700063B RID: 1595
	// (get) Token: 0x06004159 RID: 16729 RVA: 0x0014A8D9 File Offset: 0x00148AD9
	// (set) Token: 0x0600415A RID: 16730 RVA: 0x0014A8E1 File Offset: 0x00148AE1
	public bool PostTickRunning { get; set; }

	// Token: 0x0600415B RID: 16731 RVA: 0x0014A8EA File Offset: 0x00148AEA
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x0600415C RID: 16732 RVA: 0x0014A8F2 File Offset: 0x00148AF2
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickSystemCallback(this);
	}

	// Token: 0x0600415D RID: 16733 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void PreTick()
	{
	}

	// Token: 0x0600415E RID: 16734 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void Tick()
	{
	}

	// Token: 0x0600415F RID: 16735 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void PostTick()
	{
	}
}
