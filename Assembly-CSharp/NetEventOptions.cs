using System;
using Photon.Realtime;

// Token: 0x020002EC RID: 748
public class NetEventOptions
{
	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x060011E8 RID: 4584 RVA: 0x00062FCB File Offset: 0x000611CB
	public bool HasWebHooks
	{
		get
		{
			return this.Flags != WebFlags.Default;
		}
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x00062FDD File Offset: 0x000611DD
	public NetEventOptions()
	{
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x00062FF0 File Offset: 0x000611F0
	public NetEventOptions(int reciever, int[] actors, byte flags)
	{
		this.Reciever = (NetEventOptions.RecieverTarget)reciever;
		this.TargetActors = actors;
		this.Flags = new WebFlags(flags);
	}

	// Token: 0x040019A0 RID: 6560
	public NetEventOptions.RecieverTarget Reciever;

	// Token: 0x040019A1 RID: 6561
	public int[] TargetActors;

	// Token: 0x040019A2 RID: 6562
	public WebFlags Flags = WebFlags.Default;

	// Token: 0x020002ED RID: 749
	public enum RecieverTarget
	{
		// Token: 0x040019A4 RID: 6564
		others,
		// Token: 0x040019A5 RID: 6565
		all,
		// Token: 0x040019A6 RID: 6566
		master
	}
}
