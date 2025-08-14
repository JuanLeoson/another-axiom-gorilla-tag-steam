using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Realtime;

// Token: 0x020002F0 RID: 752
public class RoomConfig
{
	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06001209 RID: 4617 RVA: 0x0006328F File Offset: 0x0006148F
	public bool IsJoiningWithFriends
	{
		get
		{
			return this.joinFriendIDs != null && this.joinFriendIDs.Length != 0;
		}
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x000632A8 File Offset: 0x000614A8
	public void SetFriendIDs(List<string> friendIDs)
	{
		for (int i = 0; i < friendIDs.Count; i++)
		{
			if (friendIDs[i] == NetworkSystem.Instance.GetMyNickName())
			{
				friendIDs.RemoveAt(i);
				i--;
			}
		}
		this.joinFriendIDs = new string[friendIDs.Count];
		for (int j = 0; j < friendIDs.Count; j++)
		{
			this.joinFriendIDs[j] = friendIDs[j];
		}
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x0006331A File Offset: 0x0006151A
	public void ClearExpectedUsers()
	{
		if (this.joinFriendIDs == null || this.joinFriendIDs.Length == 0)
		{
			return;
		}
		this.joinFriendIDs = new string[0];
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0006333C File Offset: 0x0006153C
	public RoomOptions ToPUNOpts()
	{
		return new RoomOptions
		{
			IsVisible = this.isPublic,
			IsOpen = this.isJoinable,
			MaxPlayers = this.MaxPlayers,
			CustomRoomProperties = this.CustomProps,
			PublishUserId = true,
			CustomRoomPropertiesForLobby = this.AutoCustomLobbyProps()
		};
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x00063391 File Offset: 0x00061591
	public void SetFusionOpts(NetworkRunner runnerInst)
	{
		runnerInst.SessionInfo.IsVisible = this.isPublic;
		runnerInst.SessionInfo.IsOpen = this.isJoinable;
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x000633B5 File Offset: 0x000615B5
	public static RoomConfig SPConfig()
	{
		return new RoomConfig
		{
			isPublic = false,
			isJoinable = false,
			MaxPlayers = 1
		};
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x000633D1 File Offset: 0x000615D1
	public static RoomConfig AnyPublicConfig()
	{
		return new RoomConfig
		{
			isPublic = true,
			isJoinable = true,
			createIfMissing = true,
			MaxPlayers = 10
		};
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x000633F8 File Offset: 0x000615F8
	private string[] AutoCustomLobbyProps()
	{
		string[] array = new string[this.CustomProps.Count];
		int num = 0;
		foreach (DictionaryEntry dictionaryEntry in this.CustomProps)
		{
			array[num] = (string)dictionaryEntry.Key;
			num++;
		}
		return array;
	}

	// Token: 0x040019B4 RID: 6580
	public const string Room_GameModePropKey = "gameMode";

	// Token: 0x040019B5 RID: 6581
	public const string Room_PlatformPropKey = "platform";

	// Token: 0x040019B6 RID: 6582
	public bool isPublic;

	// Token: 0x040019B7 RID: 6583
	public bool isJoinable;

	// Token: 0x040019B8 RID: 6584
	public byte MaxPlayers;

	// Token: 0x040019B9 RID: 6585
	public Hashtable CustomProps = new Hashtable();

	// Token: 0x040019BA RID: 6586
	public bool createIfMissing;

	// Token: 0x040019BB RID: 6587
	public string[] joinFriendIDs;
}
