using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009F4 RID: 2548
public class FriendSystem : MonoBehaviour
{
	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x06003E63 RID: 15971 RVA: 0x0013D9D2 File Offset: 0x0013BBD2
	public FriendSystem.PlayerPrivacy LocalPlayerPrivacy
	{
		get
		{
			return this.localPlayerPrivacy;
		}
	}

	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06003E64 RID: 15972 RVA: 0x0013D9DC File Offset: 0x0013BBDC
	// (remove) Token: 0x06003E65 RID: 15973 RVA: 0x0013DA14 File Offset: 0x0013BC14
	public event Action<List<FriendBackendController.Friend>> OnFriendListRefresh;

	// Token: 0x06003E66 RID: 15974 RVA: 0x0013DA4C File Offset: 0x0013BC4C
	public void SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy privacyState)
	{
		this.localPlayerPrivacy = privacyState;
		FriendBackendController.PrivacyState privacyState2;
		switch (privacyState)
		{
		default:
			privacyState2 = FriendBackendController.PrivacyState.VISIBLE;
			break;
		case FriendSystem.PlayerPrivacy.PublicOnly:
			privacyState2 = FriendBackendController.PrivacyState.PUBLIC_ONLY;
			break;
		case FriendSystem.PlayerPrivacy.Hidden:
			privacyState2 = FriendBackendController.PrivacyState.HIDDEN;
			break;
		}
		FriendBackendController.Instance.SetPrivacyState(privacyState2);
	}

	// Token: 0x06003E67 RID: 15975 RVA: 0x0013DA89 File Offset: 0x0013BC89
	public void RefreshFriendsList()
	{
		FriendBackendController.Instance.GetFriends();
	}

	// Token: 0x06003E68 RID: 15976 RVA: 0x0013DA98 File Offset: 0x0013BC98
	public void SendFriendRequest(NetPlayer targetPlayer, GTZone stationZone, FriendSystem.FriendRequestCallback callback)
	{
		FriendSystem.FriendRequestData item = new FriendSystem.FriendRequestData
		{
			completionCallback = callback,
			sendingPlayerId = NetworkSystem.Instance.LocalPlayer.UserId.GetHashCode(),
			targetPlayerId = targetPlayer.UserId.GetHashCode(),
			localTimeSent = Time.time,
			zone = stationZone
		};
		this.pendingFriendRequests.Add(item);
		FriendBackendController.Instance.AddFriend(targetPlayer);
	}

	// Token: 0x06003E69 RID: 15977 RVA: 0x0013DB14 File Offset: 0x0013BD14
	public void RemoveFriend(FriendBackendController.Friend friend, FriendSystem.FriendRemovalCallback callback = null)
	{
		this.pendingFriendRemovals.Add(new FriendSystem.FriendRemovalData
		{
			completionCallback = callback,
			targetPlayerId = friend.Presence.FriendLinkId.GetHashCode(),
			localTimeSent = Time.time
		});
		FriendBackendController.Instance.RemoveFriend(friend);
	}

	// Token: 0x06003E6A RID: 15978 RVA: 0x0013DB70 File Offset: 0x0013BD70
	public bool HasPendingFriendRequest(GTZone zone, int senderId)
	{
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].zone == zone && this.pendingFriendRequests[i].sendingPlayerId == senderId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003E6B RID: 15979 RVA: 0x0013DBC0 File Offset: 0x0013BDC0
	public bool CheckFriendshipWithPlayer(int targetActorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		if (player != null)
		{
			int hashCode = player.UserId.GetHashCode();
			List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
			for (int i = 0; i < friendsList.Count; i++)
			{
				if (friendsList[i] != null && friendsList[i].Presence != null && friendsList[i].Presence.FriendLinkId.GetHashCode() == hashCode)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003E6C RID: 15980 RVA: 0x0013DC39 File Offset: 0x0013BE39
	private void Awake()
	{
		if (FriendSystem.Instance == null)
		{
			FriendSystem.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003E6D RID: 15981 RVA: 0x0013DC5C File Offset: 0x0013BE5C
	private void Start()
	{
		FriendBackendController.Instance.OnGetFriendsComplete += this.OnGetFriendsReturned;
		FriendBackendController.Instance.OnAddFriendComplete += this.OnAddFriendReturned;
		FriendBackendController.Instance.OnRemoveFriendComplete += this.OnRemoveFriendReturned;
	}

	// Token: 0x06003E6E RID: 15982 RVA: 0x0013DCB4 File Offset: 0x0013BEB4
	private void OnDestroy()
	{
		if (FriendBackendController.Instance != null)
		{
			FriendBackendController.Instance.OnGetFriendsComplete -= this.OnGetFriendsReturned;
			FriendBackendController.Instance.OnAddFriendComplete -= this.OnAddFriendReturned;
			FriendBackendController.Instance.OnRemoveFriendComplete -= this.OnRemoveFriendReturned;
		}
	}

	// Token: 0x06003E6F RID: 15983 RVA: 0x0013DD18 File Offset: 0x0013BF18
	private void OnGetFriendsReturned(bool succeeded)
	{
		if (succeeded)
		{
			this.lastFriendsListRefresh = Time.time;
			switch (FriendBackendController.Instance.MyPrivacyState)
			{
			default:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Visible;
				break;
			case FriendBackendController.PrivacyState.PUBLIC_ONLY:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.PublicOnly;
				break;
			case FriendBackendController.PrivacyState.HIDDEN:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Hidden;
				break;
			}
			Action<List<FriendBackendController.Friend>> onFriendListRefresh = this.OnFriendListRefresh;
			if (onFriendListRefresh == null)
			{
				return;
			}
			onFriendListRefresh(FriendBackendController.Instance.FriendsList);
		}
	}

	// Token: 0x06003E70 RID: 15984 RVA: 0x0013DD88 File Offset: 0x0013BF88
	private void OnAddFriendReturned(NetPlayer targetPlayer, bool succeeded)
	{
		int hashCode = targetPlayer.UserId.GetHashCode();
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].targetPlayerId == hashCode)
			{
				FriendSystem.FriendRequestCallback completionCallback = this.pendingFriendRequests[i].completionCallback;
				if (completionCallback != null)
				{
					completionCallback(this.pendingFriendRequests[i].zone, this.pendingFriendRequests[i].sendingPlayerId, this.pendingFriendRequests[i].targetPlayerId, succeeded);
				}
				this.indexesToRemove.Add(i);
			}
			else if (this.pendingFriendRequests[i].localTimeSent + this.friendRequestExpirationTime < Time.time)
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
		{
			this.pendingFriendRequests.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x06003E71 RID: 15985 RVA: 0x0013DE94 File Offset: 0x0013C094
	private void OnRemoveFriendReturned(FriendBackendController.Friend friend, bool succeeded)
	{
		if (friend != null && friend.Presence != null)
		{
			int hashCode = friend.Presence.FriendLinkId.GetHashCode();
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.pendingFriendRemovals.Count; i++)
			{
				if (this.pendingFriendRemovals[i].targetPlayerId == hashCode)
				{
					FriendSystem.FriendRemovalCallback completionCallback = this.pendingFriendRemovals[i].completionCallback;
					if (completionCallback != null)
					{
						completionCallback(hashCode, succeeded);
					}
					this.indexesToRemove.Add(i);
				}
				else if (this.pendingFriendRemovals[i].localTimeSent + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
			{
				this.pendingFriendRemovals.RemoveAt(this.indexesToRemove[j]);
			}
		}
	}

	// Token: 0x04004A5C RID: 19036
	[OnEnterPlay_SetNull]
	public static volatile FriendSystem Instance;

	// Token: 0x04004A5D RID: 19037
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04004A5E RID: 19038
	private FriendSystem.PlayerPrivacy localPlayerPrivacy;

	// Token: 0x04004A5F RID: 19039
	private List<FriendSystem.FriendRequestData> pendingFriendRequests = new List<FriendSystem.FriendRequestData>();

	// Token: 0x04004A60 RID: 19040
	private List<FriendSystem.FriendRemovalData> pendingFriendRemovals = new List<FriendSystem.FriendRemovalData>();

	// Token: 0x04004A61 RID: 19041
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x04004A63 RID: 19043
	private float lastFriendsListRefresh;

	// Token: 0x020009F5 RID: 2549
	// (Invoke) Token: 0x06003E74 RID: 15988
	public delegate void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success);

	// Token: 0x020009F6 RID: 2550
	private struct FriendRequestData
	{
		// Token: 0x04004A64 RID: 19044
		public GTZone zone;

		// Token: 0x04004A65 RID: 19045
		public int sendingPlayerId;

		// Token: 0x04004A66 RID: 19046
		public int targetPlayerId;

		// Token: 0x04004A67 RID: 19047
		public float localTimeSent;

		// Token: 0x04004A68 RID: 19048
		public FriendSystem.FriendRequestCallback completionCallback;
	}

	// Token: 0x020009F7 RID: 2551
	// (Invoke) Token: 0x06003E78 RID: 15992
	public delegate void FriendRemovalCallback(int friendId, bool success);

	// Token: 0x020009F8 RID: 2552
	private struct FriendRemovalData
	{
		// Token: 0x04004A69 RID: 19049
		public int targetPlayerId;

		// Token: 0x04004A6A RID: 19050
		public float localTimeSent;

		// Token: 0x04004A6B RID: 19051
		public FriendSystem.FriendRemovalCallback completionCallback;
	}

	// Token: 0x020009F9 RID: 2553
	private enum FriendRequestStatus
	{
		// Token: 0x04004A6D RID: 19053
		Pending,
		// Token: 0x04004A6E RID: 19054
		Succeeded,
		// Token: 0x04004A6F RID: 19055
		Failed
	}

	// Token: 0x020009FA RID: 2554
	public enum PlayerPrivacy
	{
		// Token: 0x04004A71 RID: 19057
		Visible,
		// Token: 0x04004A72 RID: 19058
		PublicOnly,
		// Token: 0x04004A73 RID: 19059
		Hidden
	}
}
