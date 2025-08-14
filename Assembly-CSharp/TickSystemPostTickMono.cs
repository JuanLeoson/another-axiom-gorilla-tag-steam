using System;
using UnityEngine;

// Token: 0x02000A7C RID: 2684
internal abstract class TickSystemPostTickMono : MonoBehaviour, ITickSystemPost
{
	// Token: 0x1700063E RID: 1598
	// (get) Token: 0x0600416D RID: 16749 RVA: 0x0014A92C File Offset: 0x00148B2C
	// (set) Token: 0x0600416E RID: 16750 RVA: 0x0014A934 File Offset: 0x00148B34
	public bool PostTickRunning { get; set; }

	// Token: 0x0600416F RID: 16751 RVA: 0x0014A93D File Offset: 0x00148B3D
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004170 RID: 16752 RVA: 0x00100D37 File Offset: 0x000FEF37
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06004171 RID: 16753 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void PostTick()
	{
	}
}
