using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020009DB RID: 2523
public class FriendBackendController : MonoBehaviour
{
	// Token: 0x14000068 RID: 104
	// (add) Token: 0x06003D5D RID: 15709 RVA: 0x00139238 File Offset: 0x00137438
	// (remove) Token: 0x06003D5E RID: 15710 RVA: 0x00139270 File Offset: 0x00137470
	public event Action<bool> OnGetFriendsComplete;

	// Token: 0x14000069 RID: 105
	// (add) Token: 0x06003D5F RID: 15711 RVA: 0x001392A8 File Offset: 0x001374A8
	// (remove) Token: 0x06003D60 RID: 15712 RVA: 0x001392E0 File Offset: 0x001374E0
	public event Action<bool> OnSetPrivacyStateComplete;

	// Token: 0x1400006A RID: 106
	// (add) Token: 0x06003D61 RID: 15713 RVA: 0x00139318 File Offset: 0x00137518
	// (remove) Token: 0x06003D62 RID: 15714 RVA: 0x00139350 File Offset: 0x00137550
	public event Action<NetPlayer, bool> OnAddFriendComplete;

	// Token: 0x1400006B RID: 107
	// (add) Token: 0x06003D63 RID: 15715 RVA: 0x00139388 File Offset: 0x00137588
	// (remove) Token: 0x06003D64 RID: 15716 RVA: 0x001393C0 File Offset: 0x001375C0
	public event Action<FriendBackendController.Friend, bool> OnRemoveFriendComplete;

	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x06003D65 RID: 15717 RVA: 0x001393F5 File Offset: 0x001375F5
	public List<FriendBackendController.Friend> FriendsList
	{
		get
		{
			return this.lastFriendsList;
		}
	}

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x06003D66 RID: 15718 RVA: 0x001393FD File Offset: 0x001375FD
	public FriendBackendController.PrivacyState MyPrivacyState
	{
		get
		{
			return this.lastPrivacyState;
		}
	}

	// Token: 0x06003D67 RID: 15719 RVA: 0x00139405 File Offset: 0x00137605
	public void GetFriends()
	{
		if (!this.getFriendsInProgress)
		{
			this.getFriendsInProgress = true;
			this.GetFriendsInternal();
		}
	}

	// Token: 0x06003D68 RID: 15720 RVA: 0x0013941C File Offset: 0x0013761C
	public void SetPrivacyState(FriendBackendController.PrivacyState state)
	{
		if (!this.setPrivacyStateInProgress)
		{
			this.setPrivacyStateInProgress = true;
			this.setPrivacyStateState = state;
			this.SetPrivacyStateInternal();
			return;
		}
		this.setPrivacyStateQueue.Enqueue(state);
	}

	// Token: 0x06003D69 RID: 15721 RVA: 0x00139448 File Offset: 0x00137648
	public void AddFriend(NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.UserId.GetHashCode();
		if (!this.addFriendInProgress)
		{
			this.addFriendInProgress = true;
			this.addFriendTargetIdHash = hashCode;
			this.addFriendTargetPlayer = target;
			this.AddFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.addFriendRequestQueue.Contains(new ValueTuple<int, NetPlayer>(hashCode, target)))
		{
			this.addFriendRequestQueue.Enqueue(new ValueTuple<int, NetPlayer>(hashCode, target));
		}
	}

	// Token: 0x06003D6A RID: 15722 RVA: 0x001394B8 File Offset: 0x001376B8
	public void RemoveFriend(FriendBackendController.Friend target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.Presence.FriendLinkId.GetHashCode();
		if (!this.removeFriendInProgress)
		{
			this.removeFriendInProgress = true;
			this.removeFriendTargetIdHash = hashCode;
			this.removeFriendTarget = target;
			this.RemoveFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.removeFriendRequestQueue.Contains(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target)))
		{
			this.removeFriendRequestQueue.Enqueue(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target));
		}
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x0013952D File Offset: 0x0013772D
	private void Awake()
	{
		if (FriendBackendController.Instance == null)
		{
			FriendBackendController.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x00139550 File Offset: 0x00137750
	private void GetFriendsInternal()
	{
		base.StartCoroutine(this.SendGetFriendsRequest(new FriendBackendController.GetFriendsRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = ""
		}, new Action<FriendBackendController.GetFriendsResponse>(this.GetFriendsComplete)));
	}

	// Token: 0x06003D6D RID: 15725 RVA: 0x001395AA File Offset: 0x001377AA
	private IEnumerator SendGetFriendsRequest(FriendBackendController.GetFriendsRequest data, Action<FriendBackendController.GetFriendsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/GetFriendsV2", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.GetFriendsResponse obj = JsonConvert.DeserializeObject<FriendBackendController.GetFriendsResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.getFriendsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.getFriendsRetryCount + 1));
				this.getFriendsRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.GetFriendsInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum GetFriends retries attempted. Please check your network connection.", null);
				this.getFriendsRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.getFriendsInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003D6E RID: 15726 RVA: 0x001395C8 File Offset: 0x001377C8
	private void GetFriendsComplete([CanBeNull] FriendBackendController.GetFriendsResponse response)
	{
		this.getFriendsInProgress = false;
		if (response != null)
		{
			this.lastGetFriendsResponse = response;
			if (this.lastGetFriendsResponse.Result != null)
			{
				this.lastPrivacyState = this.lastGetFriendsResponse.Result.MyPrivacyState;
				if (this.lastGetFriendsResponse.Result.Friends != null)
				{
					this.lastFriendsList.Clear();
					foreach (FriendBackendController.Friend item in this.lastGetFriendsResponse.Result.Friends)
					{
						this.lastFriendsList.Add(item);
					}
				}
			}
			Action<bool> onGetFriendsComplete = this.OnGetFriendsComplete;
			if (onGetFriendsComplete == null)
			{
				return;
			}
			onGetFriendsComplete(true);
			return;
		}
		else
		{
			Action<bool> onGetFriendsComplete2 = this.OnGetFriendsComplete;
			if (onGetFriendsComplete2 == null)
			{
				return;
			}
			onGetFriendsComplete2(false);
			return;
		}
	}

	// Token: 0x06003D6F RID: 15727 RVA: 0x001396A4 File Offset: 0x001378A4
	private void SetPrivacyStateInternal()
	{
		base.StartCoroutine(this.SendSetPrivacyStateRequest(new FriendBackendController.SetPrivacyStateRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			PrivacyState = this.setPrivacyStateState.ToString()
		}, new Action<FriendBackendController.SetPrivacyStateResponse>(this.SetPrivacyStateComplete)));
	}

	// Token: 0x06003D70 RID: 15728 RVA: 0x0013970A File Offset: 0x0013790A
	private IEnumerator SendSetPrivacyStateRequest(FriendBackendController.SetPrivacyStateRequest data, Action<FriendBackendController.SetPrivacyStateResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/SetPrivacyState", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.SetPrivacyStateResponse obj = JsonConvert.DeserializeObject<FriendBackendController.SetPrivacyStateResponse>(request.downloadHandler.text);
			callback(obj);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.setPrivacyStateRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.setPrivacyStateRetryCount + 1));
				this.setPrivacyStateRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SetPrivacyStateInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum SetPrivacyState retries attempted. Please check your network connection.", null);
				this.setPrivacyStateRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.setPrivacyStateInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003D71 RID: 15729 RVA: 0x00139728 File Offset: 0x00137928
	private void SetPrivacyStateComplete([CanBeNull] FriendBackendController.SetPrivacyStateResponse response)
	{
		this.setPrivacyStateInProgress = false;
		if (response != null)
		{
			this.lastPrivacyStateResponse = response;
			Action<bool> onSetPrivacyStateComplete = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete != null)
			{
				onSetPrivacyStateComplete(true);
			}
		}
		else
		{
			Action<bool> onSetPrivacyStateComplete2 = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete2 != null)
			{
				onSetPrivacyStateComplete2(false);
			}
		}
		if (this.setPrivacyStateQueue.Count > 0)
		{
			FriendBackendController.PrivacyState privacyState = this.setPrivacyStateQueue.Dequeue();
			this.SetPrivacyState(privacyState);
		}
	}

	// Token: 0x06003D72 RID: 15730 RVA: 0x00139790 File Offset: 0x00137990
	private void AddFriendInternal()
	{
		base.StartCoroutine(this.SendAddFriendRequest(new FriendBackendController.FriendRequestRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipToken = "",
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.addFriendTargetPlayer.UserId
		}, new Action<bool>(this.AddFriendComplete)));
	}

	// Token: 0x06003D73 RID: 15731 RVA: 0x0013981B File Offset: 0x00137A1B
	private IEnumerator SendAddFriendRequest(FriendBackendController.FriendRequestRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RequestFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			if (request.responseCode == 409L)
			{
				flag = false;
			}
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.addFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.addFriendRetryCount + 1));
				this.addFriendRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.addFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.addFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003D74 RID: 15732 RVA: 0x00139838 File Offset: 0x00137A38
	private void AddFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<NetPlayer, bool> onAddFriendComplete = this.OnAddFriendComplete;
			if (onAddFriendComplete != null)
			{
				onAddFriendComplete(this.addFriendTargetPlayer, true);
			}
		}
		else
		{
			Action<NetPlayer, bool> onAddFriendComplete2 = this.OnAddFriendComplete;
			if (onAddFriendComplete2 != null)
			{
				onAddFriendComplete2(this.addFriendTargetPlayer, false);
			}
		}
		this.addFriendInProgress = false;
		this.addFriendTargetIdHash = 0;
		this.addFriendTargetPlayer = null;
		if (this.addFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, NetPlayer> valueTuple = this.addFriendRequestQueue.Dequeue();
			this.AddFriend(valueTuple.Item2);
		}
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x001398B8 File Offset: 0x00137AB8
	private void RemoveFriendInternal()
	{
		base.StartCoroutine(this.SendRemoveFriendRequest(new FriendBackendController.RemoveFriendRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.removeFriendTarget.Presence.FriendLinkId
		}, new Action<bool>(this.RemoveFriendComplete)));
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x0013993D File Offset: 0x00137B3D
	private IEnumerator SendRemoveFriendRequest(FriendBackendController.RemoveFriendRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RemoveFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.removeFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.removeFriendRetryCount + 1));
				this.removeFriendRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.removeFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.removeFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003D77 RID: 15735 RVA: 0x0013995C File Offset: 0x00137B5C
	private void RemoveFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete != null)
			{
				onRemoveFriendComplete(this.removeFriendTarget, true);
			}
		}
		else
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete2 = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete2 != null)
			{
				onRemoveFriendComplete2(this.removeFriendTarget, false);
			}
		}
		this.removeFriendInProgress = false;
		this.removeFriendTargetIdHash = 0;
		this.removeFriendTarget = null;
		if (this.removeFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, FriendBackendController.Friend> valueTuple = this.removeFriendRequestQueue.Dequeue();
			this.RemoveFriend(valueTuple.Item2);
		}
	}

	// Token: 0x06003D78 RID: 15736 RVA: 0x001399DC File Offset: 0x00137BDC
	private void LogNetPlayersInRoom()
	{
		Debug.Log("Local Player PlayfabId: " + PlayFabAuthenticator.instance.GetPlayFabPlayerId());
		int num = 0;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			Debug.Log(string.Format("[{0}] Player: {1}, ActorNumber: {2}, UserID: {3}, IsMasterClient: {4}", new object[]
			{
				num,
				netPlayer.NickName,
				netPlayer.ActorNumber,
				netPlayer.UserId,
				netPlayer.IsMasterClient
			}));
			num++;
		}
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x00139A74 File Offset: 0x00137C74
	private void TestAddFriend()
	{
		this.OnAddFriendComplete -= this.TestAddFriendCompleteCallback;
		this.OnAddFriendComplete += this.TestAddFriendCompleteCallback;
		NetPlayer target = null;
		if (this.netPlayerIndexToAddFriend >= 0 && this.netPlayerIndexToAddFriend < NetworkSystem.Instance.AllNetPlayers.Length)
		{
			target = NetworkSystem.Instance.AllNetPlayers[this.netPlayerIndexToAddFriend];
		}
		this.AddFriend(target);
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x00139ADD File Offset: 0x00137CDD
	private void TestAddFriendCompleteCallback(NetPlayer player, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06003D7B RID: 15739 RVA: 0x00139AF8 File Offset: 0x00137CF8
	private void TestRemoveFriend()
	{
		this.OnRemoveFriendComplete -= this.TestRemoveFriendCompleteCallback;
		this.OnRemoveFriendComplete += this.TestRemoveFriendCompleteCallback;
		FriendBackendController.Friend target = null;
		if (this.friendListIndexToRemoveFriend >= 0 && this.friendListIndexToRemoveFriend < this.FriendsList.Count)
		{
			target = this.FriendsList[this.friendListIndexToRemoveFriend];
		}
		this.RemoveFriend(target);
	}

	// Token: 0x06003D7C RID: 15740 RVA: 0x00139B60 File Offset: 0x00137D60
	private void TestRemoveFriendCompleteCallback(FriendBackendController.Friend friend, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06003D7D RID: 15741 RVA: 0x00139B7A File Offset: 0x00137D7A
	private void TestGetFriends()
	{
		this.OnGetFriendsComplete -= this.TestGetFriendsCompleteCallback;
		this.OnGetFriendsComplete += this.TestGetFriendsCompleteCallback;
		this.GetFriends();
	}

	// Token: 0x06003D7E RID: 15742 RVA: 0x00139BA8 File Offset: 0x00137DA8
	private void TestGetFriendsCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = true");
			if (this.FriendsList != null)
			{
				string text = string.Format("Friend Count: {0} Friends: \n", this.FriendsList.Count);
				for (int i = 0; i < this.FriendsList.Count; i++)
				{
					if (this.FriendsList[i] != null && this.FriendsList[i].Presence != null)
					{
						text = string.Concat(new string[]
						{
							text,
							this.FriendsList[i].Presence.UserName,
							", ",
							this.FriendsList[i].Presence.FriendLinkId,
							", ",
							this.FriendsList[i].Presence.RoomId,
							", ",
							this.FriendsList[i].Presence.Region,
							", ",
							this.FriendsList[i].Presence.Zone,
							"\n"
						});
					}
					else
					{
						text += "null friend\n";
					}
				}
				Debug.Log(text);
				return;
			}
		}
		else
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = false");
		}
	}

	// Token: 0x06003D7F RID: 15743 RVA: 0x00139D05 File Offset: 0x00137F05
	private void TestSetPrivacyState()
	{
		this.OnSetPrivacyStateComplete -= this.TestSetPrivacyStateCompleteCallback;
		this.OnSetPrivacyStateComplete += this.TestSetPrivacyStateCompleteCallback;
		this.SetPrivacyState(this.privacyStateToSet);
	}

	// Token: 0x06003D80 RID: 15744 RVA: 0x00139D38 File Offset: 0x00137F38
	private void TestSetPrivacyStateCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log(string.Format("SetPrivacyState Success: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
			return;
		}
		Debug.Log(string.Format("SetPrivacyState Failed: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
	}

	// Token: 0x040049A0 RID: 18848
	[OnEnterPlay_SetNull]
	public static volatile FriendBackendController Instance;

	// Token: 0x040049A5 RID: 18853
	private int maxRetriesOnFail = 3;

	// Token: 0x040049A6 RID: 18854
	private int getFriendsRetryCount;

	// Token: 0x040049A7 RID: 18855
	private int setPrivacyStateRetryCount;

	// Token: 0x040049A8 RID: 18856
	private int addFriendRetryCount;

	// Token: 0x040049A9 RID: 18857
	private int removeFriendRetryCount;

	// Token: 0x040049AA RID: 18858
	private bool getFriendsInProgress;

	// Token: 0x040049AB RID: 18859
	private FriendBackendController.GetFriendsResponse lastGetFriendsResponse;

	// Token: 0x040049AC RID: 18860
	private List<FriendBackendController.Friend> lastFriendsList = new List<FriendBackendController.Friend>();

	// Token: 0x040049AD RID: 18861
	private bool setPrivacyStateInProgress;

	// Token: 0x040049AE RID: 18862
	private FriendBackendController.PrivacyState setPrivacyStateState;

	// Token: 0x040049AF RID: 18863
	private FriendBackendController.SetPrivacyStateResponse lastPrivacyStateResponse;

	// Token: 0x040049B0 RID: 18864
	private Queue<FriendBackendController.PrivacyState> setPrivacyStateQueue = new Queue<FriendBackendController.PrivacyState>();

	// Token: 0x040049B1 RID: 18865
	private FriendBackendController.PrivacyState lastPrivacyState;

	// Token: 0x040049B2 RID: 18866
	private bool addFriendInProgress;

	// Token: 0x040049B3 RID: 18867
	private int addFriendTargetIdHash;

	// Token: 0x040049B4 RID: 18868
	private NetPlayer addFriendTargetPlayer;

	// Token: 0x040049B5 RID: 18869
	private Queue<ValueTuple<int, NetPlayer>> addFriendRequestQueue = new Queue<ValueTuple<int, NetPlayer>>();

	// Token: 0x040049B6 RID: 18870
	private bool removeFriendInProgress;

	// Token: 0x040049B7 RID: 18871
	private int removeFriendTargetIdHash;

	// Token: 0x040049B8 RID: 18872
	private FriendBackendController.Friend removeFriendTarget;

	// Token: 0x040049B9 RID: 18873
	private Queue<ValueTuple<int, FriendBackendController.Friend>> removeFriendRequestQueue = new Queue<ValueTuple<int, FriendBackendController.Friend>>();

	// Token: 0x040049BA RID: 18874
	[SerializeField]
	private int netPlayerIndexToAddFriend;

	// Token: 0x040049BB RID: 18875
	[SerializeField]
	private int friendListIndexToRemoveFriend;

	// Token: 0x040049BC RID: 18876
	[SerializeField]
	private FriendBackendController.PrivacyState privacyStateToSet;

	// Token: 0x020009DC RID: 2524
	public class Friend
	{
		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06003D82 RID: 15746 RVA: 0x00139DD8 File Offset: 0x00137FD8
		// (set) Token: 0x06003D83 RID: 15747 RVA: 0x00139DE0 File Offset: 0x00137FE0
		public FriendBackendController.FriendPresence Presence { get; set; }

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06003D84 RID: 15748 RVA: 0x00139DE9 File Offset: 0x00137FE9
		// (set) Token: 0x06003D85 RID: 15749 RVA: 0x00139DF1 File Offset: 0x00137FF1
		public DateTime Created { get; set; }
	}

	// Token: 0x020009DD RID: 2525
	public class FriendPresence
	{
		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06003D87 RID: 15751 RVA: 0x00139DFA File Offset: 0x00137FFA
		// (set) Token: 0x06003D88 RID: 15752 RVA: 0x00139E02 File Offset: 0x00138002
		public string FriendLinkId { get; set; }

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06003D89 RID: 15753 RVA: 0x00139E0B File Offset: 0x0013800B
		// (set) Token: 0x06003D8A RID: 15754 RVA: 0x00139E13 File Offset: 0x00138013
		public string UserName { get; set; }

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06003D8B RID: 15755 RVA: 0x00139E1C File Offset: 0x0013801C
		// (set) Token: 0x06003D8C RID: 15756 RVA: 0x00139E24 File Offset: 0x00138024
		public string RoomId { get; set; }

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06003D8D RID: 15757 RVA: 0x00139E2D File Offset: 0x0013802D
		// (set) Token: 0x06003D8E RID: 15758 RVA: 0x00139E35 File Offset: 0x00138035
		public string Zone { get; set; }

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06003D8F RID: 15759 RVA: 0x00139E3E File Offset: 0x0013803E
		// (set) Token: 0x06003D90 RID: 15760 RVA: 0x00139E46 File Offset: 0x00138046
		public string Region { get; set; }

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06003D91 RID: 15761 RVA: 0x00139E4F File Offset: 0x0013804F
		// (set) Token: 0x06003D92 RID: 15762 RVA: 0x00139E57 File Offset: 0x00138057
		public bool? IsPublic { get; set; }
	}

	// Token: 0x020009DE RID: 2526
	public class FriendLink
	{
		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06003D94 RID: 15764 RVA: 0x00139E60 File Offset: 0x00138060
		// (set) Token: 0x06003D95 RID: 15765 RVA: 0x00139E68 File Offset: 0x00138068
		public string my_playfab_id { get; set; }

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x06003D96 RID: 15766 RVA: 0x00139E71 File Offset: 0x00138071
		// (set) Token: 0x06003D97 RID: 15767 RVA: 0x00139E79 File Offset: 0x00138079
		public string my_mothership_id { get; set; }

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x06003D98 RID: 15768 RVA: 0x00139E82 File Offset: 0x00138082
		// (set) Token: 0x06003D99 RID: 15769 RVA: 0x00139E8A File Offset: 0x0013808A
		public string my_friendlink_id { get; set; }

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x06003D9A RID: 15770 RVA: 0x00139E93 File Offset: 0x00138093
		// (set) Token: 0x06003D9B RID: 15771 RVA: 0x00139E9B File Offset: 0x0013809B
		public string friend_playfab_id { get; set; }

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x06003D9C RID: 15772 RVA: 0x00139EA4 File Offset: 0x001380A4
		// (set) Token: 0x06003D9D RID: 15773 RVA: 0x00139EAC File Offset: 0x001380AC
		public string friend_mothership_id { get; set; }

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x06003D9E RID: 15774 RVA: 0x00139EB5 File Offset: 0x001380B5
		// (set) Token: 0x06003D9F RID: 15775 RVA: 0x00139EBD File Offset: 0x001380BD
		public string friend_friendlink_id { get; set; }

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06003DA0 RID: 15776 RVA: 0x00139EC6 File Offset: 0x001380C6
		// (set) Token: 0x06003DA1 RID: 15777 RVA: 0x00139ECE File Offset: 0x001380CE
		public DateTime created { get; set; }
	}

	// Token: 0x020009DF RID: 2527
	[NullableContext(2)]
	[Nullable(0)]
	public class FriendIdResponse
	{
		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06003DA3 RID: 15779 RVA: 0x00139ED7 File Offset: 0x001380D7
		// (set) Token: 0x06003DA4 RID: 15780 RVA: 0x00139EDF File Offset: 0x001380DF
		public string PlayFabId { get; set; }

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06003DA5 RID: 15781 RVA: 0x00139EE8 File Offset: 0x001380E8
		// (set) Token: 0x06003DA6 RID: 15782 RVA: 0x00139EF0 File Offset: 0x001380F0
		public string MothershipId { get; set; } = "";
	}

	// Token: 0x020009E0 RID: 2528
	public class FriendRequestRequest
	{
		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06003DA8 RID: 15784 RVA: 0x00139F0C File Offset: 0x0013810C
		// (set) Token: 0x06003DA9 RID: 15785 RVA: 0x00139F14 File Offset: 0x00138114
		public string PlayFabId { get; set; }

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06003DAA RID: 15786 RVA: 0x00139F1D File Offset: 0x0013811D
		// (set) Token: 0x06003DAB RID: 15787 RVA: 0x00139F25 File Offset: 0x00138125
		public string MothershipId { get; set; } = "";

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06003DAC RID: 15788 RVA: 0x00139F2E File Offset: 0x0013812E
		// (set) Token: 0x06003DAD RID: 15789 RVA: 0x00139F36 File Offset: 0x00138136
		public string PlayFabTicket { get; set; }

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06003DAE RID: 15790 RVA: 0x00139F3F File Offset: 0x0013813F
		// (set) Token: 0x06003DAF RID: 15791 RVA: 0x00139F47 File Offset: 0x00138147
		public string MothershipToken { get; set; }

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x06003DB0 RID: 15792 RVA: 0x00139F50 File Offset: 0x00138150
		// (set) Token: 0x06003DB1 RID: 15793 RVA: 0x00139F58 File Offset: 0x00138158
		public string MyFriendLinkId { get; set; }

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x06003DB2 RID: 15794 RVA: 0x00139F61 File Offset: 0x00138161
		// (set) Token: 0x06003DB3 RID: 15795 RVA: 0x00139F69 File Offset: 0x00138169
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x020009E1 RID: 2529
	public class GetFriendsRequest
	{
		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x06003DB5 RID: 15797 RVA: 0x00139F85 File Offset: 0x00138185
		// (set) Token: 0x06003DB6 RID: 15798 RVA: 0x00139F8D File Offset: 0x0013818D
		public string PlayFabId { get; set; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06003DB7 RID: 15799 RVA: 0x00139F96 File Offset: 0x00138196
		// (set) Token: 0x06003DB8 RID: 15800 RVA: 0x00139F9E File Offset: 0x0013819E
		public string MothershipId { get; set; } = "";

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x06003DB9 RID: 15801 RVA: 0x00139FA7 File Offset: 0x001381A7
		// (set) Token: 0x06003DBA RID: 15802 RVA: 0x00139FAF File Offset: 0x001381AF
		public string MothershipToken { get; set; }

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x06003DBB RID: 15803 RVA: 0x00139FB8 File Offset: 0x001381B8
		// (set) Token: 0x06003DBC RID: 15804 RVA: 0x00139FC0 File Offset: 0x001381C0
		public string PlayFabTicket { get; set; }
	}

	// Token: 0x020009E2 RID: 2530
	public class GetFriendsResponse
	{
		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x06003DBE RID: 15806 RVA: 0x00139FDC File Offset: 0x001381DC
		// (set) Token: 0x06003DBF RID: 15807 RVA: 0x00139FE4 File Offset: 0x001381E4
		[CanBeNull]
		public FriendBackendController.GetFriendsResult Result { get; set; }

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x06003DC0 RID: 15808 RVA: 0x00139FED File Offset: 0x001381ED
		// (set) Token: 0x06003DC1 RID: 15809 RVA: 0x00139FF5 File Offset: 0x001381F5
		public int StatusCode { get; set; }

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06003DC2 RID: 15810 RVA: 0x00139FFE File Offset: 0x001381FE
		// (set) Token: 0x06003DC3 RID: 15811 RVA: 0x0013A006 File Offset: 0x00138206
		[Nullable(2)]
		public string Error { [NullableContext(2)] get; [NullableContext(2)] set; }
	}

	// Token: 0x020009E3 RID: 2531
	public class GetFriendsResult
	{
		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06003DC5 RID: 15813 RVA: 0x0013A00F File Offset: 0x0013820F
		// (set) Token: 0x06003DC6 RID: 15814 RVA: 0x0013A017 File Offset: 0x00138217
		public List<FriendBackendController.Friend> Friends { get; set; }

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06003DC7 RID: 15815 RVA: 0x0013A020 File Offset: 0x00138220
		// (set) Token: 0x06003DC8 RID: 15816 RVA: 0x0013A028 File Offset: 0x00138228
		public FriendBackendController.PrivacyState MyPrivacyState { get; set; }
	}

	// Token: 0x020009E4 RID: 2532
	public class SetPrivacyStateRequest
	{
		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06003DCA RID: 15818 RVA: 0x0013A031 File Offset: 0x00138231
		// (set) Token: 0x06003DCB RID: 15819 RVA: 0x0013A039 File Offset: 0x00138239
		public string PlayFabId { get; set; }

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06003DCC RID: 15820 RVA: 0x0013A042 File Offset: 0x00138242
		// (set) Token: 0x06003DCD RID: 15821 RVA: 0x0013A04A File Offset: 0x0013824A
		public string PlayFabTicket { get; set; }

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06003DCE RID: 15822 RVA: 0x0013A053 File Offset: 0x00138253
		// (set) Token: 0x06003DCF RID: 15823 RVA: 0x0013A05B File Offset: 0x0013825B
		public string PrivacyState { get; set; }
	}

	// Token: 0x020009E5 RID: 2533
	[NullableContext(2)]
	[Nullable(0)]
	public class SetPrivacyStateResponse
	{
		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06003DD1 RID: 15825 RVA: 0x0013A064 File Offset: 0x00138264
		// (set) Token: 0x06003DD2 RID: 15826 RVA: 0x0013A06C File Offset: 0x0013826C
		public int StatusCode { get; set; }

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06003DD3 RID: 15827 RVA: 0x0013A075 File Offset: 0x00138275
		// (set) Token: 0x06003DD4 RID: 15828 RVA: 0x0013A07D File Offset: 0x0013827D
		public string Error { get; set; }
	}

	// Token: 0x020009E6 RID: 2534
	public class RemoveFriendRequest
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06003DD6 RID: 15830 RVA: 0x0013A086 File Offset: 0x00138286
		// (set) Token: 0x06003DD7 RID: 15831 RVA: 0x0013A08E File Offset: 0x0013828E
		public string PlayFabId { get; set; }

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06003DD8 RID: 15832 RVA: 0x0013A097 File Offset: 0x00138297
		// (set) Token: 0x06003DD9 RID: 15833 RVA: 0x0013A09F File Offset: 0x0013829F
		public string MothershipId { get; set; } = "";

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06003DDA RID: 15834 RVA: 0x0013A0A8 File Offset: 0x001382A8
		// (set) Token: 0x06003DDB RID: 15835 RVA: 0x0013A0B0 File Offset: 0x001382B0
		public string PlayFabTicket { get; set; }

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06003DDC RID: 15836 RVA: 0x0013A0B9 File Offset: 0x001382B9
		// (set) Token: 0x06003DDD RID: 15837 RVA: 0x0013A0C1 File Offset: 0x001382C1
		public string MothershipToken { get; set; }

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06003DDE RID: 15838 RVA: 0x0013A0CA File Offset: 0x001382CA
		// (set) Token: 0x06003DDF RID: 15839 RVA: 0x0013A0D2 File Offset: 0x001382D2
		public string MyFriendLinkId { get; set; }

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06003DE0 RID: 15840 RVA: 0x0013A0DB File Offset: 0x001382DB
		// (set) Token: 0x06003DE1 RID: 15841 RVA: 0x0013A0E3 File Offset: 0x001382E3
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x020009E7 RID: 2535
	public enum PendingRequestStatus
	{
		// Token: 0x040049E9 RID: 18921
		I_REQUESTED,
		// Token: 0x040049EA RID: 18922
		THEY_REQUESTED,
		// Token: 0x040049EB RID: 18923
		CONFIRMED,
		// Token: 0x040049EC RID: 18924
		NOT_FOUND
	}

	// Token: 0x020009E8 RID: 2536
	public enum PrivacyState
	{
		// Token: 0x040049EE RID: 18926
		VISIBLE,
		// Token: 0x040049EF RID: 18927
		PUBLIC_ONLY,
		// Token: 0x040049F0 RID: 18928
		HIDDEN
	}
}
