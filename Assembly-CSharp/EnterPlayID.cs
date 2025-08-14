using System;

// Token: 0x02000AD5 RID: 2773
public struct EnterPlayID
{
	// Token: 0x060042DE RID: 17118 RVA: 0x0014FD68 File Offset: 0x0014DF68
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	// Token: 0x060042DF RID: 17119 RVA: 0x0014FD78 File Offset: 0x0014DF78
	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	// Token: 0x17000658 RID: 1624
	// (get) Token: 0x060042E0 RID: 17120 RVA: 0x0014FD9A File Offset: 0x0014DF9A
	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Token: 0x04004DC4 RID: 19908
	private static int currentID = 1;

	// Token: 0x04004DC5 RID: 19909
	private int id;
}
