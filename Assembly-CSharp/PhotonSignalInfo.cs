using System;
using Photon.Pun;

// Token: 0x02000A32 RID: 2610
[Serializable]
public struct PhotonSignalInfo
{
	// Token: 0x06003FA1 RID: 16289 RVA: 0x00143D1C File Offset: 0x00141F1C
	public PhotonSignalInfo(NetPlayer sender, int timestamp)
	{
		this.sender = sender;
		this.timestamp = timestamp;
	}

	// Token: 0x17000602 RID: 1538
	// (get) Token: 0x06003FA2 RID: 16290 RVA: 0x00143D2C File Offset: 0x00141F2C
	public double sentServerTime
	{
		get
		{
			return this.timestamp / 1000.0;
		}
	}

	// Token: 0x06003FA3 RID: 16291 RVA: 0x00143D40 File Offset: 0x00141F40
	public override string ToString()
	{
		return string.Format("[{0}: Sender = '{1}' sentTime = {2}]", "PhotonSignalInfo", this.sender.ActorNumber, this.sentServerTime);
	}

	// Token: 0x06003FA4 RID: 16292 RVA: 0x00143D6C File Offset: 0x00141F6C
	public static implicit operator PhotonMessageInfo(PhotonSignalInfo psi)
	{
		return new PhotonMessageInfo(psi.sender.GetPlayerRef(), psi.timestamp, null);
	}

	// Token: 0x04004BBA RID: 19386
	public readonly int timestamp;

	// Token: 0x04004BBB RID: 19387
	public readonly NetPlayer sender;
}
