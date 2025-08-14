using System;
using UnityEngine;

// Token: 0x02000A7A RID: 2682
internal abstract class TickSystemPreTickMono : MonoBehaviour, ITickSystemPre
{
	// Token: 0x1700063C RID: 1596
	// (get) Token: 0x06004161 RID: 16737 RVA: 0x0014A8FA File Offset: 0x00148AFA
	// (set) Token: 0x06004162 RID: 16738 RVA: 0x0014A902 File Offset: 0x00148B02
	public bool PreTickRunning { get; set; }

	// Token: 0x06004163 RID: 16739 RVA: 0x0014A90B File Offset: 0x00148B0B
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPreTickCallback(this);
	}

	// Token: 0x06004164 RID: 16740 RVA: 0x0014A913 File Offset: 0x00148B13
	public void OnDisable()
	{
		TickSystem<object>.RemovePreTickCallback(this);
	}

	// Token: 0x06004165 RID: 16741 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void PreTick()
	{
	}
}
