using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x02000104 RID: 260
[NetworkStructWeaved(3)]
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct BeeSwarmData : INetworkStruct
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000683 RID: 1667 RVA: 0x00025B24 File Offset: 0x00023D24
	// (set) Token: 0x06000684 RID: 1668 RVA: 0x00025B2C File Offset: 0x00023D2C
	public int TargetActorNumber { readonly get; set; }

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000685 RID: 1669 RVA: 0x00025B35 File Offset: 0x00023D35
	// (set) Token: 0x06000686 RID: 1670 RVA: 0x00025B3D File Offset: 0x00023D3D
	public int CurrentState { readonly get; set; }

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000687 RID: 1671 RVA: 0x00025B46 File Offset: 0x00023D46
	// (set) Token: 0x06000688 RID: 1672 RVA: 0x00025B4E File Offset: 0x00023D4E
	public float CurrentSpeed { readonly get; set; }

	// Token: 0x06000689 RID: 1673 RVA: 0x00025B57 File Offset: 0x00023D57
	public BeeSwarmData(int actorNr, int state, float speed)
	{
		this.TargetActorNumber = actorNr;
		this.CurrentState = state;
		this.CurrentSpeed = speed;
	}
}
