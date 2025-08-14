using System;
using UnityEngine;

// Token: 0x02000A7B RID: 2683
internal abstract class TickSystemTickMono : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700063D RID: 1597
	// (get) Token: 0x06004167 RID: 16743 RVA: 0x0014A91B File Offset: 0x00148B1B
	// (set) Token: 0x06004168 RID: 16744 RVA: 0x0014A923 File Offset: 0x00148B23
	public bool TickRunning { get; set; }

	// Token: 0x06004169 RID: 16745 RVA: 0x0002EF95 File Offset: 0x0002D195
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600416A RID: 16746 RVA: 0x0002EF9D File Offset: 0x0002D19D
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0600416B RID: 16747 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void Tick()
	{
	}
}
