using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C49 RID: 3145
	public class PlayerTimerManager : MonoBehaviourPunCallbacks
	{
		// Token: 0x06004DD0 RID: 19920 RVA: 0x00182A60 File Offset: 0x00180C60
		private void Awake()
		{
			if (PlayerTimerManager.instance == null)
			{
				PlayerTimerManager.instance = this;
			}
			else if (PlayerTimerManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.callLimiters = new CallLimiter[2];
			this.callLimiters[0] = new CallLimiter(10, 1f, 0.5f);
			this.callLimiters[1] = new CallLimiter(30, 1f, 0.5f);
			this.playerTimerData = new Dictionary<int, PlayerTimerManager.PlayerTimerData>(10);
			this.timerToggleLimiters = new Dictionary<int, CallLimiter>(10);
			this.limiterPool = new List<CallLimiter>(10);
			this.serializedTimerData = new byte[256];
		}

		// Token: 0x06004DD1 RID: 19921 RVA: 0x00182B10 File Offset: 0x00180D10
		private CallLimiter CreateLimiterFromPool()
		{
			if (this.limiterPool.Count > 0)
			{
				CallLimiter result = this.limiterPool[this.limiterPool.Count - 1];
				this.limiterPool.RemoveAt(this.limiterPool.Count - 1);
				return result;
			}
			return new CallLimiter(5, 1f, 0.5f);
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x00182B6C File Offset: 0x00180D6C
		private void ReturnCallLimiterToPool(CallLimiter limiter)
		{
			if (limiter == null)
			{
				return;
			}
			limiter.Reset();
			this.limiterPool.Add(limiter);
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x00182B84 File Offset: 0x00180D84
		public void RegisterTimerBoard(PlayerTimerBoard board)
		{
			if (!PlayerTimerManager.timerBoards.Contains(board))
			{
				PlayerTimerManager.timerBoards.Add(board);
				this.UpdateTimerBoard(board);
			}
		}

		// Token: 0x06004DD4 RID: 19924 RVA: 0x00182BA5 File Offset: 0x00180DA5
		public void UnregisterTimerBoard(PlayerTimerBoard board)
		{
			PlayerTimerManager.timerBoards.Remove(board);
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x00182BB4 File Offset: 0x00180DB4
		public bool IsLocalTimerStarted()
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			return this.playerTimerData.TryGetValue(NetworkSystem.Instance.LocalPlayer.ActorNumber, out playerTimerData) && playerTimerData.isStarted;
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x00182BE8 File Offset: 0x00180DE8
		public float GetTimeForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (!this.playerTimerData.TryGetValue(actorNumber, out playerTimerData))
			{
				return 0f;
			}
			if (playerTimerData.isStarted)
			{
				return Mathf.Clamp((PhotonNetwork.ServerTimestamp - playerTimerData.startTimeStamp) / 1000f, 0f, 3599.99f);
			}
			return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
		}

		// Token: 0x06004DD7 RID: 19927 RVA: 0x00182C54 File Offset: 0x00180E54
		public float GetLastDurationForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(actorNumber, out playerTimerData))
			{
				return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
			}
			return -1f;
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x00182C94 File Offset: 0x00180E94
		[PunRPC]
		private void InitTimersMasterRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "InitTimersMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.InitTimersMaster, info))
			{
				return;
			}
			if (this.areTimersInitialized)
			{
				return;
			}
			this.DeserializeTimerState(bytes.Length, bytes);
			this.areTimersInitialized = true;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004DD9 RID: 19929 RVA: 0x00182CE8 File Offset: 0x00180EE8
		private int SerializeTimerState()
		{
			Array.Clear(this.serializedTimerData, 0, this.serializedTimerData.Length);
			MemoryStream memoryStream = new MemoryStream(this.serializedTimerData);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.playerTimerData.Count > 10)
			{
				this.ClearOldPlayerData();
			}
			binaryWriter.Write(this.playerTimerData.Count);
			foreach (KeyValuePair<int, PlayerTimerManager.PlayerTimerData> keyValuePair in this.playerTimerData)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.startTimeStamp);
				binaryWriter.Write(keyValuePair.Value.endTimeStamp);
				binaryWriter.Write(keyValuePair.Value.isStarted ? 1 : 0);
				binaryWriter.Write(keyValuePair.Value.lastTimerDuration);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x00182DE4 File Offset: 0x00180FE4
		private void DeserializeTimerState(int numBytes, byte[] bytes)
		{
			if (numBytes <= 0 || numBytes > 256)
			{
				return;
			}
			if (bytes == null || bytes.Length < numBytes)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			this.playerTimerData.Clear();
			try
			{
				List<Player> list = PhotonNetwork.PlayerList.ToList<Player>();
				if (bytes.Length < 4)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num = binaryReader.ReadInt32();
				if (num < 0 || num > 10)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num2 = 17;
				if (memoryStream.Position + (long)(num2 * num) > (long)bytes.Length)
				{
					this.playerTimerData.Clear();
					return;
				}
				for (int i = 0; i < num; i++)
				{
					int actorNum = binaryReader.ReadInt32();
					int startTimeStamp = binaryReader.ReadInt32();
					int endTimeStamp = binaryReader.ReadInt32();
					bool isStarted = binaryReader.ReadByte() > 0;
					uint lastTimerDuration = binaryReader.ReadUInt32();
					if (list.FindIndex((Player x) => x.ActorNumber == actorNum) >= 0)
					{
						PlayerTimerManager.PlayerTimerData value = new PlayerTimerManager.PlayerTimerData
						{
							startTimeStamp = startTimeStamp,
							endTimeStamp = endTimeStamp,
							isStarted = isStarted,
							lastTimerDuration = lastTimerDuration
						};
						this.playerTimerData.TryAdd(actorNum, value);
					}
				}
			}
			catch (Exception value2)
			{
				Console.WriteLine(value2);
				this.playerTimerData.Clear();
			}
			if (Time.time - this.requestSendTime < 5f && this.IsLocalTimerStarted() != this.localPlayerRequestedStart)
			{
				this.timerPV.RPC("RequestTimerToggleRPC", RpcTarget.MasterClient, new object[]
				{
					this.localPlayerRequestedStart
				});
			}
		}

		// Token: 0x06004DDB RID: 19931 RVA: 0x00182FA8 File Offset: 0x001811A8
		private void ClearOldPlayerData()
		{
			List<int> list = new List<int>(this.playerTimerData.Count);
			List<Player> list2 = PhotonNetwork.PlayerList.ToList<Player>();
			using (Dictionary<int, PlayerTimerManager.PlayerTimerData>.KeyCollection.Enumerator enumerator = this.playerTimerData.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int actorNum = enumerator.Current;
					if (list2.FindIndex((Player x) => x.ActorNumber == actorNum) < 0)
					{
						list.Add(actorNum);
					}
				}
			}
			foreach (int key in list)
			{
				this.playerTimerData.Remove(key);
			}
		}

		// Token: 0x06004DDC RID: 19932 RVA: 0x00183088 File Offset: 0x00181288
		public void RequestTimerToggle(bool startTimer)
		{
			this.requestSendTime = Time.time;
			this.localPlayerRequestedStart = startTimer;
			this.timerPV.RPC("RequestTimerToggleRPC", RpcTarget.MasterClient, new object[]
			{
				startTimer
			});
		}

		// Token: 0x06004DDD RID: 19933 RVA: 0x001830BC File Offset: 0x001812BC
		[PunRPC]
		private void RequestTimerToggleRPC(bool startTimer, PhotonMessageInfo info)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "RequestTimerToggleRPC");
			CallLimiter callLimiter;
			if (this.timerToggleLimiters.TryGetValue(info.Sender.ActorNumber, out callLimiter))
			{
				if (!callLimiter.CheckCallTime(Time.time))
				{
					return;
				}
			}
			else
			{
				CallLimiter callLimiter2 = this.CreateLimiterFromPool();
				this.timerToggleLimiters.Add(info.Sender.ActorNumber, callLimiter2);
				callLimiter2.CheckCallTime(Time.time);
			}
			if (info.Sender == null)
			{
				return;
			}
			PlayerTimerManager.PlayerTimerData playerTimerData;
			bool flag = this.playerTimerData.TryGetValue(info.Sender.ActorNumber, out playerTimerData);
			if (!startTimer && !flag)
			{
				return;
			}
			if (flag && !startTimer && !playerTimerData.isStarted)
			{
				return;
			}
			int num = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.timerPV.RPC("TimerToggledMasterRPC", RpcTarget.All, new object[]
			{
				startTimer,
				num,
				info.Sender
			});
		}

		// Token: 0x06004DDE RID: 19934 RVA: 0x001831D4 File Offset: 0x001813D4
		[PunRPC]
		private void TimerToggledMasterRPC(bool startTimer, int toggleTimeStamp, Player player, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "TimerToggledMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.ToggleTimerMaster, info))
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (!this.areTimersInitialized)
			{
				return;
			}
			int num = toggleTimeStamp;
			int num2 = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num2 > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num2 = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			if (num2 - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = num2 - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.OnToggleTimerForPlayer(startTimer, player, num);
		}

		// Token: 0x06004DDF RID: 19935 RVA: 0x0018327C File Offset: 0x0018147C
		private void OnToggleTimerForPlayer(bool startTimer, Player player, int toggleTime)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(player.ActorNumber, out playerTimerData))
			{
				if (startTimer && !playerTimerData.isStarted)
				{
					playerTimerData.startTimeStamp = toggleTime;
					playerTimerData.isStarted = true;
					UnityEvent<int> onTimerStartedForPlayer = this.OnTimerStartedForPlayer;
					if (onTimerStartedForPlayer != null)
					{
						onTimerStartedForPlayer.Invoke(player.ActorNumber);
					}
					if (player.IsLocal)
					{
						UnityEvent onLocalTimerStarted = this.OnLocalTimerStarted;
						if (onLocalTimerStarted != null)
						{
							onLocalTimerStarted.Invoke();
						}
					}
				}
				else if (!startTimer && playerTimerData.isStarted)
				{
					playerTimerData.endTimeStamp = toggleTime;
					playerTimerData.isStarted = false;
					playerTimerData.lastTimerDuration = (uint)(playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					UnityEvent<int, int> onTimerStopped = this.OnTimerStopped;
					if (onTimerStopped != null)
					{
						onTimerStopped.Invoke(player.ActorNumber, playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					}
				}
				this.playerTimerData[player.ActorNumber] = playerTimerData;
			}
			else
			{
				PlayerTimerManager.PlayerTimerData value = new PlayerTimerManager.PlayerTimerData
				{
					startTimeStamp = (startTimer ? toggleTime : 0),
					endTimeStamp = (startTimer ? 0 : toggleTime),
					isStarted = startTimer,
					lastTimerDuration = 0U
				};
				this.playerTimerData.TryAdd(player.ActorNumber, value);
				UnityEvent<int> onTimerStartedForPlayer2 = this.OnTimerStartedForPlayer;
				if (onTimerStartedForPlayer2 != null)
				{
					onTimerStartedForPlayer2.Invoke(player.ActorNumber);
				}
				if (player.IsLocal)
				{
					UnityEvent onLocalTimerStarted2 = this.OnLocalTimerStarted;
					if (onLocalTimerStarted2 != null)
					{
						onLocalTimerStarted2.Invoke();
					}
				}
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004DE0 RID: 19936 RVA: 0x001833D4 File Offset: 0x001815D4
		private bool ValidateCallLimits(PlayerTimerManager.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= PlayerTimerManager.RPC.InitTimersMaster && rpcCall < PlayerTimerManager.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x06004DE1 RID: 19937 RVA: 0x00183404 File Offset: 0x00181604
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			base.OnMasterClientSwitched(newMasterClient);
			if (newMasterClient.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", RpcTarget.Others, new object[]
				{
					num,
					this.serializedTimerData
				});
				return;
			}
			this.playerTimerData.Clear();
			this.areTimersInitialized = false;
		}

		// Token: 0x06004DE2 RID: 19938 RVA: 0x00183464 File Offset: 0x00181664
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			base.OnPlayerEnteredRoom(newPlayer);
			if (PhotonNetwork.IsMasterClient && !newPlayer.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", newPlayer, new object[]
				{
					num,
					this.serializedTimerData
				});
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004DE3 RID: 19939 RVA: 0x001834C0 File Offset: 0x001816C0
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			this.playerTimerData.Remove(otherPlayer.ActorNumber);
			CallLimiter limiter;
			if (this.timerToggleLimiters.TryGetValue(otherPlayer.ActorNumber, out limiter))
			{
				this.ReturnCallLimiterToPool(limiter);
				this.timerToggleLimiters.Remove(otherPlayer.ActorNumber);
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x0018351C File Offset: 0x0018171C
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			this.joinedRoom = true;
			if (PhotonNetwork.IsMasterClient)
			{
				this.playerTimerData.Clear();
				foreach (CallLimiter limiter in this.timerToggleLimiters.Values)
				{
					this.ReturnCallLimiterToPool(limiter);
				}
				this.timerToggleLimiters.Clear();
				this.areTimersInitialized = true;
				this.UpdateAllTimerBoards();
				return;
			}
			this.requestSendTime = 0f;
			this.areTimersInitialized = false;
		}

		// Token: 0x06004DE5 RID: 19941 RVA: 0x001835C0 File Offset: 0x001817C0
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			this.joinedRoom = false;
			this.playerTimerData.Clear();
			foreach (CallLimiter limiter in this.timerToggleLimiters.Values)
			{
				this.ReturnCallLimiterToPool(limiter);
			}
			this.timerToggleLimiters.Clear();
			this.areTimersInitialized = false;
			this.requestSendTime = 0f;
			this.localPlayerRequestedStart = false;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004DE6 RID: 19942 RVA: 0x0018365C File Offset: 0x0018185C
		private void UpdateAllTimerBoards()
		{
			foreach (PlayerTimerBoard board in PlayerTimerManager.timerBoards)
			{
				this.UpdateTimerBoard(board);
			}
		}

		// Token: 0x06004DE7 RID: 19943 RVA: 0x001836B0 File Offset: 0x001818B0
		private void UpdateTimerBoard(PlayerTimerBoard board)
		{
			board.SetSleepState(this.joinedRoom);
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (!this.joinedRoom)
			{
				if (board.notInRoomText != null)
				{
					board.notInRoomText.gameObject.SetActive(true);
					board.notInRoomText.text = GorillaComputer.instance.offlineTextInitialString;
				}
				for (int i = 0; i < board.lines.Count; i++)
				{
					board.lines[i].ResetData();
				}
				return;
			}
			if (board.notInRoomText != null)
			{
				board.notInRoomText.gameObject.SetActive(false);
			}
			for (int j = 0; j < board.lines.Count; j++)
			{
				PlayerTimerBoardLine playerTimerBoardLine = board.lines[j];
				if (j < PhotonNetwork.PlayerList.Length)
				{
					playerTimerBoardLine.gameObject.SetActive(true);
					playerTimerBoardLine.SetLineData(NetworkSystem.Instance.GetPlayer(PhotonNetwork.PlayerList[j]));
					playerTimerBoardLine.UpdateLine();
				}
				else
				{
					playerTimerBoardLine.ResetData();
					playerTimerBoardLine.gameObject.SetActive(false);
				}
			}
			board.RedrawPlayerLines();
		}

		// Token: 0x040056D1 RID: 22225
		public static PlayerTimerManager instance;

		// Token: 0x040056D2 RID: 22226
		public PhotonView timerPV;

		// Token: 0x040056D3 RID: 22227
		public UnityEvent OnLocalTimerStarted;

		// Token: 0x040056D4 RID: 22228
		public UnityEvent<int> OnTimerStartedForPlayer;

		// Token: 0x040056D5 RID: 22229
		public UnityEvent<int, int> OnTimerStopped;

		// Token: 0x040056D6 RID: 22230
		public const float MAX_DURATION_SECONDS = 3599.99f;

		// Token: 0x040056D7 RID: 22231
		private float requestSendTime;

		// Token: 0x040056D8 RID: 22232
		private bool localPlayerRequestedStart;

		// Token: 0x040056D9 RID: 22233
		private CallLimiter[] callLimiters;

		// Token: 0x040056DA RID: 22234
		private Dictionary<int, CallLimiter> timerToggleLimiters;

		// Token: 0x040056DB RID: 22235
		private List<CallLimiter> limiterPool;

		// Token: 0x040056DC RID: 22236
		private bool areTimersInitialized;

		// Token: 0x040056DD RID: 22237
		private Dictionary<int, PlayerTimerManager.PlayerTimerData> playerTimerData;

		// Token: 0x040056DE RID: 22238
		private const int MAX_TIMER_INIT_BYTES = 256;

		// Token: 0x040056DF RID: 22239
		private byte[] serializedTimerData;

		// Token: 0x040056E0 RID: 22240
		private static List<PlayerTimerBoard> timerBoards = new List<PlayerTimerBoard>(10);

		// Token: 0x040056E1 RID: 22241
		private bool joinedRoom;

		// Token: 0x02000C4A RID: 3146
		private enum RPC
		{
			// Token: 0x040056E3 RID: 22243
			InitTimersMaster,
			// Token: 0x040056E4 RID: 22244
			ToggleTimerMaster,
			// Token: 0x040056E5 RID: 22245
			Count
		}

		// Token: 0x02000C4B RID: 3147
		public struct PlayerTimerData
		{
			// Token: 0x040056E6 RID: 22246
			public int startTimeStamp;

			// Token: 0x040056E7 RID: 22247
			public int endTimeStamp;

			// Token: 0x040056E8 RID: 22248
			public bool isStarted;

			// Token: 0x040056E9 RID: 22249
			public uint lastTimerDuration;
		}
	}
}
