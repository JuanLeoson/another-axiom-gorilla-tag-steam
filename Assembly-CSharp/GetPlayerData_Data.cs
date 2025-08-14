using System;
using System.Runtime.CompilerServices;
using KID.Model;
using UnityEngine;

// Token: 0x020008A3 RID: 2211
public class GetPlayerData_Data
{
	// Token: 0x060037A3 RID: 14243 RVA: 0x00120720 File Offset: 0x0011E920
	public GetPlayerData_Data(GetSessionResponseType type, GetPlayerDataResponse response)
	{
		this.responseType = type;
		if (response == null)
		{
			if (this.responseType == GetSessionResponseType.OK)
			{
				this.responseType = GetSessionResponseType.ERROR;
				Debug.LogError("[KID::GET_PLAYER_DATA_DATA] Incoming [GetPlayerDataResponse] is NULL");
			}
			return;
		}
		this.AgeStatus = response.AgeStatus;
		this.status = response.Status;
		if (this.status != null)
		{
			this.session = new TMPSession(response.Session, response.DefaultSession, response.Age, this.status.Value);
			this.session.SetOptInPermissions(response.Permissions);
			Debug.Log("[KID::GET_PLAYER_DATA_DATA::OptInRefactor] Setting Opt-in Permissions: " + string.Join(", ", this.session.GetOptedInPermissions()));
		}
		this.HasConfirmedSetup = response.HasConfirmedSetup;
	}

	// Token: 0x04004447 RID: 17479
	public readonly AgeStatusType? AgeStatus;

	// Token: 0x04004448 RID: 17480
	public readonly GetSessionResponseType responseType;

	// Token: 0x04004449 RID: 17481
	public readonly SessionStatus? status;

	// Token: 0x0400444A RID: 17482
	public readonly TMPSession session;

	// Token: 0x0400444B RID: 17483
	[Nullable(new byte[]
	{
		2,
		0
	})]
	public readonly string[] OptInPermissions;

	// Token: 0x0400444C RID: 17484
	public readonly bool HasConfirmedSetup;
}
