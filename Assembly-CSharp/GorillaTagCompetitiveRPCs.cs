using System;
using Photon.Pun;

// Token: 0x02000A56 RID: 2646
internal class GorillaTagCompetitiveRPCs : RPCNetworkBase
{
	// Token: 0x06004093 RID: 16531 RVA: 0x00146E0D File Offset: 0x0014500D
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.tagCompManager = (GorillaTagCompetitiveManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06004094 RID: 16532 RVA: 0x00146E28 File Offset: 0x00145028
	[PunRPC]
	public void SendScoresToLateJoinerRPC(int[] playerId, int[] numTags, float[] pointsOnDefense, float[] joinTime, bool[] infected, float[] taggedTime, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SendScoresToLateJoinerRPC");
		if (info.Sender == null || !info.Sender.IsMasterClient)
		{
			return;
		}
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (photonMessageInfoWrapped.Sender.CheckSingleCallRPC(NetPlayer.SingleCallRPC.RankedSendScoreToLateJoiner))
		{
			return;
		}
		photonMessageInfoWrapped.Sender.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.RankedSendScoreToLateJoiner);
		if (playerId == null || numTags == null || pointsOnDefense == null || joinTime == null || infected == null || taggedTime == null)
		{
			return;
		}
		int num = playerId.Length;
		if (num > 10)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = i + 1; j < num; j++)
			{
				if (playerId[i] == playerId[j])
				{
					return;
				}
			}
		}
		if (numTags.Length != num || pointsOnDefense.Length != num || joinTime.Length != num || infected.Length != num || taggedTime.Length != num)
		{
			return;
		}
		for (int k = 0; k < num; k++)
		{
			if (NetworkSystem.Instance.GetNetPlayerByID(playerId[k]) == null)
			{
				return;
			}
			if (numTags[k] < 0 || numTags[k] >= 15)
			{
				return;
			}
			if (pointsOnDefense[k] < 0f)
			{
				return;
			}
			float num2 = joinTime[k];
			if (float.IsNaN(num2) || float.IsInfinity(num2) || num2 < 0f || num2 > this.tagCompManager.GetRoundDuration() + 15f)
			{
				return;
			}
			float num3 = taggedTime[k];
			if (float.IsNaN(num3) || float.IsInfinity(num3) || num3 < 0f || num3 > this.tagCompManager.GetRoundDuration() + 15f)
			{
				return;
			}
		}
		this.tagCompManager.GetScoring().ReceivedScoresForLateJoiner(playerId, numTags, pointsOnDefense, joinTime, infected, taggedTime);
	}

	// Token: 0x04004C3E RID: 19518
	private GameModeSerializer serializer;

	// Token: 0x04004C3F RID: 19519
	private GorillaTagCompetitiveManager tagCompManager;
}
