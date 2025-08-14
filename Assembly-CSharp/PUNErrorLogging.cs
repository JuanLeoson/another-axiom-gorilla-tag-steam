using System;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;

// Token: 0x02000A3E RID: 2622
public class PUNErrorLogging : MonoBehaviour
{
	// Token: 0x06004040 RID: 16448 RVA: 0x00146088 File Offset: 0x00144288
	private void Start()
	{
		PhotonNetwork.InternalEventError = (Action<EventData, Exception>)Delegate.Combine(PhotonNetwork.InternalEventError, new Action<EventData, Exception>(this.PUNError));
		PlayFabTitleDataCache.Instance.GetTitleData("PUNErrorLogging", delegate(string data)
		{
			int num;
			if (!int.TryParse(data, out num))
			{
				return;
			}
			PUNErrorLogging.LogFlags logFlags = (PUNErrorLogging.LogFlags)num;
			this.m_logSerializeView = logFlags.HasFlag(PUNErrorLogging.LogFlags.SerializeView);
			this.m_logOwnershipTransfer = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipTransfer);
			this.m_logOwnershipRequest = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipRequest);
			this.m_logOwnershipUpdate = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipUpdate);
			this.m_logRPC = logFlags.HasFlag(PUNErrorLogging.LogFlags.RPC);
			this.m_logInstantiate = logFlags.HasFlag(PUNErrorLogging.LogFlags.Instantiate);
			this.m_logDestroy = logFlags.HasFlag(PUNErrorLogging.LogFlags.Destroy);
			this.m_logDestroyPlayer = logFlags.HasFlag(PUNErrorLogging.LogFlags.DestroyPlayer);
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x06004041 RID: 16449 RVA: 0x001460F0 File Offset: 0x001442F0
	private void PUNError(EventData data, Exception exception)
	{
		NetworkSystem.Instance.GetPlayer(data.Sender);
		byte code = data.Code;
		switch (code)
		{
		case 200:
			this.PrintException(exception, this.m_logRPC);
			return;
		case 201:
		case 206:
			this.PrintException(exception, this.m_logSerializeView);
			return;
		case 202:
			this.PrintException(exception, this.m_logInstantiate);
			return;
		case 203:
		case 205:
		case 208:
		case 211:
			break;
		case 204:
			this.PrintException(exception, this.m_logDestroy);
			return;
		case 207:
			this.PrintException(exception, this.m_logDestroyPlayer);
			return;
		case 209:
			this.PrintException(exception, this.m_logOwnershipRequest);
			return;
		case 210:
			this.PrintException(exception, this.m_logOwnershipTransfer);
			return;
		case 212:
			this.PrintException(exception, this.m_logOwnershipUpdate);
			return;
		default:
			if (code == 254)
			{
				this.PrintException(exception, true);
				return;
			}
			break;
		}
		this.PrintException(exception, true);
	}

	// Token: 0x06004042 RID: 16450 RVA: 0x001461DE File Offset: 0x001443DE
	private void PrintException(Exception e, bool print)
	{
		if (print)
		{
			Debug.LogException(e);
		}
	}

	// Token: 0x04004BE7 RID: 19431
	[SerializeField]
	private bool m_logSerializeView = true;

	// Token: 0x04004BE8 RID: 19432
	[SerializeField]
	private bool m_logOwnershipTransfer = true;

	// Token: 0x04004BE9 RID: 19433
	[SerializeField]
	private bool m_logOwnershipRequest = true;

	// Token: 0x04004BEA RID: 19434
	[SerializeField]
	private bool m_logOwnershipUpdate = true;

	// Token: 0x04004BEB RID: 19435
	[SerializeField]
	private bool m_logRPC = true;

	// Token: 0x04004BEC RID: 19436
	[SerializeField]
	private bool m_logInstantiate = true;

	// Token: 0x04004BED RID: 19437
	[SerializeField]
	private bool m_logDestroy = true;

	// Token: 0x04004BEE RID: 19438
	[SerializeField]
	private bool m_logDestroyPlayer = true;

	// Token: 0x02000A3F RID: 2623
	[Flags]
	private enum LogFlags
	{
		// Token: 0x04004BF0 RID: 19440
		SerializeView = 1,
		// Token: 0x04004BF1 RID: 19441
		OwnershipTransfer = 2,
		// Token: 0x04004BF2 RID: 19442
		OwnershipRequest = 4,
		// Token: 0x04004BF3 RID: 19443
		OwnershipUpdate = 8,
		// Token: 0x04004BF4 RID: 19444
		RPC = 16,
		// Token: 0x04004BF5 RID: 19445
		Instantiate = 32,
		// Token: 0x04004BF6 RID: 19446
		Destroy = 64,
		// Token: 0x04004BF7 RID: 19447
		DestroyPlayer = 128
	}
}
