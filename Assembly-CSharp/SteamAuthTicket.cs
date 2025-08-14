using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000A16 RID: 2582
public class SteamAuthTicket : IDisposable
{
	// Token: 0x06003EF4 RID: 16116 RVA: 0x0013FD32 File Offset: 0x0013DF32
	private SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		this.m_hAuthTicket = hAuthTicket;
	}

	// Token: 0x06003EF5 RID: 16117 RVA: 0x0013FD41 File Offset: 0x0013DF41
	public static implicit operator SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		return new SteamAuthTicket(hAuthTicket);
	}

	// Token: 0x06003EF6 RID: 16118 RVA: 0x0013FD4C File Offset: 0x0013DF4C
	~SteamAuthTicket()
	{
		this.Dispose();
	}

	// Token: 0x06003EF7 RID: 16119 RVA: 0x0013FD78 File Offset: 0x0013DF78
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		if (this.m_hAuthTicket != HAuthTicket.Invalid)
		{
			try
			{
				SteamUser.CancelAuthTicket(this.m_hAuthTicket);
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning("Failed to invalidate a Steam auth ticket because the Steam API was shut down. Was it supposed to be disposed of sooner?");
			}
			this.m_hAuthTicket = HAuthTicket.Invalid;
		}
	}

	// Token: 0x04004AFA RID: 19194
	private HAuthTicket m_hAuthTicket;
}
