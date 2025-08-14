using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009FB RID: 2555
public class MockFriendServer : MonoBehaviourPun
{
	// Token: 0x170005F4 RID: 1524
	// (get) Token: 0x06003E7B RID: 15995 RVA: 0x0013DFB2 File Offset: 0x0013C1B2
	public int LocalPlayerId
	{
		get
		{
			return PhotonNetwork.LocalPlayer.UserId.GetHashCode();
		}
	}

	// Token: 0x06003E7C RID: 15996 RVA: 0x0013DFC4 File Offset: 0x0013C1C4
	private void Awake()
	{
		if (MockFriendServer.Instance == null)
		{
			MockFriendServer.Instance = this;
			PhotonNetwork.AddCallbackTarget(this);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnMultiplayerStarted;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003E7D RID: 15997 RVA: 0x0013E018 File Offset: 0x0013C218
	private void OnMultiplayerStarted()
	{
		this.RegisterLocalPlayer(this.LocalPlayerId);
	}

	// Token: 0x06003E7E RID: 15998 RVA: 0x0013E028 File Offset: 0x0013C228
	private void Update()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.friendRequests.Count; i++)
			{
				if (this.friendRequests[i].requestTime + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = 0; j < this.indexesToRemove.Count; j++)
			{
				this.friendRequests.RemoveAt(this.indexesToRemove[j]);
			}
			this.indexesToRemove.Clear();
			for (int k = 0; k < this.friendRequests.Count; k++)
			{
				if (this.friendRequests[k].requestTime + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(k);
				}
				else if (this.friendRequests[k].completionTime < Time.time)
				{
					for (int l = k + 1; l < this.friendRequests.Count; l++)
					{
						int num;
						int num2;
						if (this.friendRequests[l].completionTime < Time.time && this.friendRequests[k].requestorPublicId == this.friendRequests[l].requesteePublicId && this.friendRequests[k].requesteePublicId == this.friendRequests[l].requestorPublicId && this.TryLookupPrivateId(this.friendRequests[k].requestorPublicId, out num) && this.TryLookupPrivateId(this.friendRequests[k].requesteePublicId, out num2))
						{
							this.AddFriend(this.friendRequests[k].requestorPublicId, this.friendRequests[k].requesteePublicId, num, num2);
							this.indexesToRemove.Add(l);
							this.indexesToRemove.Add(k);
							base.photonView.RPC("AddFriendPairRPC", RpcTarget.Others, new object[]
							{
								this.friendRequests[k].requestorPublicId,
								this.friendRequests[k].requesteePublicId,
								num,
								num2
							});
							break;
						}
					}
				}
			}
			for (int m = 0; m < this.indexesToRemove.Count; m++)
			{
				this.friendRequests.RemoveAt(this.indexesToRemove[m]);
			}
		}
	}

	// Token: 0x06003E7F RID: 15999 RVA: 0x0013E2D4 File Offset: 0x0013C4D4
	public void RegisterLocalPlayer(int localPlayerPublicId)
	{
		int hashCode = PlayFabAuthenticator.instance.GetPlayFabPlayerId().GetHashCode();
		if (base.photonView.IsMine)
		{
			this.RegisterLocalPlayerInternal(localPlayerPublicId, hashCode);
			return;
		}
		base.photonView.RPC("RegisterLocalPlayerRPC", RpcTarget.MasterClient, new object[]
		{
			localPlayerPublicId,
			hashCode
		});
	}

	// Token: 0x06003E80 RID: 16000 RVA: 0x0013E334 File Offset: 0x0013C534
	public void RequestAddFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestAddFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestAddFriendRPC", RpcTarget.MasterClient, new object[]
		{
			this.LocalPlayerId,
			targetPlayerId
		});
	}

	// Token: 0x06003E81 RID: 16001 RVA: 0x0013E38C File Offset: 0x0013C58C
	public void RequestRemoveFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestRemoveFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestRemoveFriendRPC", RpcTarget.MasterClient, new object[]
		{
			this.LocalPlayerId,
			targetPlayerId
		});
	}

	// Token: 0x06003E82 RID: 16002 RVA: 0x0013E3E4 File Offset: 0x0013C5E4
	public void GetFriendList(List<int> friendListResult)
	{
		int localPlayerId = this.LocalPlayerId;
		friendListResult.Clear();
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if (this.friendPairList[i].publicIdPlayerA == localPlayerId)
			{
				friendListResult.Add(this.friendPairList[i].publicIdPlayerB);
			}
			else if (this.friendPairList[i].publicIdPlayerB == localPlayerId)
			{
				friendListResult.Add(this.friendPairList[i].publicIdPlayerA);
			}
		}
	}

	// Token: 0x06003E83 RID: 16003 RVA: 0x0013E46C File Offset: 0x0013C66C
	private void RequestAddFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		if (base.photonView.IsMine)
		{
			bool flag = false;
			for (int i = 0; i < this.friendRequests.Count; i++)
			{
				if (this.friendRequests[i].requestorPublicId == localPlayerPublicId && this.friendRequests[i].requesteePublicId == otherPlayerPublicId)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				float time = Time.time;
				float num = Random.Range(this.friendRequestCompletionDelayRange.x, this.friendRequestCompletionDelayRange.y);
				this.friendRequests.Add(new MockFriendServer.FriendRequest
				{
					requestorPublicId = localPlayerPublicId,
					requesteePublicId = otherPlayerPublicId,
					requestTime = time,
					completionTime = time + num
				});
			}
		}
	}

	// Token: 0x06003E84 RID: 16004 RVA: 0x0013E529 File Offset: 0x0013C729
	[PunRPC]
	public void RequestAddFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestAddFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06003E85 RID: 16005 RVA: 0x0013E534 File Offset: 0x0013C734
	private void RequestRemoveFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		int privateIdA;
		int privateIdB;
		if (base.photonView.IsMine && this.TryLookupPrivateId(localPlayerPublicId, out privateIdA) && this.TryLookupPrivateId(otherPlayerPublicId, out privateIdB))
		{
			this.RemoveFriend(privateIdA, privateIdB);
		}
	}

	// Token: 0x06003E86 RID: 16006 RVA: 0x0013E56C File Offset: 0x0013C76C
	[PunRPC]
	public void RequestRemoveFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestRemoveFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06003E87 RID: 16007 RVA: 0x0013E578 File Offset: 0x0013C778
	private void RegisterLocalPlayerInternal(int publicId, int privateId)
	{
		if (base.photonView.IsMine)
		{
			bool flag = false;
			for (int i = 0; i < this.privateIdLookup.Count; i++)
			{
				if (publicId == this.privateIdLookup[i].playerPublicId || privateId == this.privateIdLookup[i].playerPrivateId)
				{
					MockFriendServer.PrivateIdEncryptionPlaceholder value = this.privateIdLookup[i];
					value.playerPublicId = publicId;
					value.playerPrivateId = privateId;
					this.privateIdLookup[i] = value;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.privateIdLookup.Add(new MockFriendServer.PrivateIdEncryptionPlaceholder
				{
					playerPublicId = publicId,
					playerPrivateId = privateId
				});
			}
		}
	}

	// Token: 0x06003E88 RID: 16008 RVA: 0x0013E62A File Offset: 0x0013C82A
	[PunRPC]
	public void RegisterLocalPlayerRPC(int playerPublicId, int playerPrivateId, PhotonMessageInfo info)
	{
		this.RegisterLocalPlayerInternal(playerPublicId, playerPrivateId);
	}

	// Token: 0x06003E89 RID: 16009 RVA: 0x0013E634 File Offset: 0x0013C834
	[PunRPC]
	public void AddFriendPairRPC(int publicIdA, int publicIdB, int privateIdA, int privateIdB, PhotonMessageInfo info)
	{
		this.AddFriend(publicIdA, publicIdB, privateIdA, privateIdB);
	}

	// Token: 0x06003E8A RID: 16010 RVA: 0x0013E644 File Offset: 0x0013C844
	private void AddFriend(int publicIdA, int publicIdB, int privateIdA, int privateIdB)
	{
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if ((this.friendPairList[i].privateIdPlayerA == privateIdA && this.friendPairList[i].privateIdPlayerB == privateIdB) || (this.friendPairList[i].privateIdPlayerA == privateIdB && this.friendPairList[i].privateIdPlayerB == privateIdA))
			{
				return;
			}
		}
		this.friendPairList.Add(new MockFriendServer.FriendPair
		{
			publicIdPlayerA = publicIdA,
			publicIdPlayerB = publicIdB,
			privateIdPlayerA = privateIdA,
			privateIdPlayerB = privateIdB
		});
	}

	// Token: 0x06003E8B RID: 16011 RVA: 0x0013E6F0 File Offset: 0x0013C8F0
	private void RemoveFriend(int privateIdA, int privateIdB)
	{
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if ((this.friendPairList[i].privateIdPlayerA == privateIdA && this.friendPairList[i].privateIdPlayerB == privateIdB) || (this.friendPairList[i].privateIdPlayerA == privateIdB && this.friendPairList[i].privateIdPlayerB == privateIdA))
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = 0; j < this.friendPairList.Count; j++)
		{
			this.friendPairList.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x06003E8C RID: 16012 RVA: 0x0013E7A8 File Offset: 0x0013C9A8
	private bool TryLookupPrivateId(int publicId, out int privateId)
	{
		for (int i = 0; i < this.privateIdLookup.Count; i++)
		{
			if (this.privateIdLookup[i].playerPublicId == publicId)
			{
				privateId = this.privateIdLookup[i].playerPrivateId;
				return true;
			}
		}
		privateId = -1;
		return false;
	}

	// Token: 0x04004A74 RID: 19060
	[OnEnterPlay_SetNull]
	public static volatile MockFriendServer Instance;

	// Token: 0x04004A75 RID: 19061
	[SerializeField]
	private Vector2 friendRequestCompletionDelayRange = new Vector2(0.5f, 1f);

	// Token: 0x04004A76 RID: 19062
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04004A77 RID: 19063
	private List<MockFriendServer.FriendPair> friendPairList = new List<MockFriendServer.FriendPair>();

	// Token: 0x04004A78 RID: 19064
	private List<MockFriendServer.PrivateIdEncryptionPlaceholder> privateIdLookup = new List<MockFriendServer.PrivateIdEncryptionPlaceholder>();

	// Token: 0x04004A79 RID: 19065
	private List<MockFriendServer.FriendRequest> friendRequests = new List<MockFriendServer.FriendRequest>();

	// Token: 0x04004A7A RID: 19066
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x020009FC RID: 2556
	public struct FriendPair
	{
		// Token: 0x04004A7B RID: 19067
		public int publicIdPlayerA;

		// Token: 0x04004A7C RID: 19068
		public int publicIdPlayerB;

		// Token: 0x04004A7D RID: 19069
		public int privateIdPlayerA;

		// Token: 0x04004A7E RID: 19070
		public int privateIdPlayerB;
	}

	// Token: 0x020009FD RID: 2557
	public struct PrivateIdEncryptionPlaceholder
	{
		// Token: 0x04004A7F RID: 19071
		public int playerPublicId;

		// Token: 0x04004A80 RID: 19072
		public int playerPrivateId;
	}

	// Token: 0x020009FE RID: 2558
	public struct FriendRequest
	{
		// Token: 0x04004A81 RID: 19073
		public int requestorPublicId;

		// Token: 0x04004A82 RID: 19074
		public int requesteePublicId;

		// Token: 0x04004A83 RID: 19075
		public float requestTime;

		// Token: 0x04004A84 RID: 19076
		public float completionTime;
	}
}
