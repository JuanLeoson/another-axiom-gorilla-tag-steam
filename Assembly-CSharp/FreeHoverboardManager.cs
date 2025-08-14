using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000753 RID: 1875
public class FreeHoverboardManager : NetworkSceneObject
{
	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06002EFD RID: 12029 RVA: 0x000F8DDB File Offset: 0x000F6FDB
	// (set) Token: 0x06002EFE RID: 12030 RVA: 0x000F8DE2 File Offset: 0x000F6FE2
	public static FreeHoverboardManager instance { get; private set; }

	// Token: 0x06002EFF RID: 12031 RVA: 0x000F8DEC File Offset: 0x000F6FEC
	private FreeHoverboardManager.DataPerPlayer GetOrCreatePlayerData(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, out dataPerPlayer))
		{
			dataPerPlayer = default(FreeHoverboardManager.DataPerPlayer);
			dataPerPlayer.Init(actorNumber, this.freeBoardPool);
			this.perPlayerData.Add(actorNumber, dataPerPlayer);
		}
		return dataPerPlayer;
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x000F8E30 File Offset: 0x000F7030
	private void Awake()
	{
		FreeHoverboardManager.instance = this;
		for (int i = 0; i < 20; i++)
		{
			FreeHoverboardInstance freeHoverboardInstance = UnityEngine.Object.Instantiate<FreeHoverboardInstance>(this.freeHoverboardPrefab);
			freeHoverboardInstance.gameObject.SetActive(false);
			this.freeBoardPool.Push(freeHoverboardInstance);
		}
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x000F8EB8 File Offset: 0x000F70B8
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (this.perPlayerData.TryGetValue(netPlayer.ActorNumber, out dataPerPlayer))
		{
			dataPerPlayer.ReturnBoards(this.freeBoardPool);
			this.perPlayerData.Remove(netPlayer.ActorNumber);
		}
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x000F8EFC File Offset: 0x000F70FC
	private void OnLeftRoom()
	{
		foreach (KeyValuePair<int, FreeHoverboardManager.DataPerPlayer> keyValuePair in this.perPlayerData)
		{
			keyValuePair.Value.ReturnBoards(this.freeBoardPool);
		}
		this.perPlayerData.Clear();
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x000F8F68 File Offset: 0x000F7168
	private void SpawnBoard(FreeHoverboardManager.DataPerPlayer playerData, int boardIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardInstance freeHoverboardInstance = (boardIndex == 0) ? playerData.board0 : playerData.board1;
		freeHoverboardInstance.transform.position = position;
		freeHoverboardInstance.transform.rotation = rotation;
		freeHoverboardInstance.Rigidbody.velocity = velocity;
		freeHoverboardInstance.Rigidbody.angularVelocity = avelocity;
		freeHoverboardInstance.SetColor(boardColor);
		freeHoverboardInstance.gameObject.SetActive(true);
		int ownerActorNumber = freeHoverboardInstance.ownerActorNumber;
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		int? num = (localPlayer != null) ? new int?(localPlayer.ActorNumber) : null;
		if (ownerActorNumber == num.GetValueOrDefault() & num != null)
		{
			this.localPlayerLastSpawnedBoardIndex = boardIndex;
		}
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x000F9010 File Offset: 0x000F7210
	public void SendDropBoardRPC(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		int num = (!orCreatePlayerData.board0.gameObject.activeSelf) ? 0 : ((!orCreatePlayerData.board1.gameObject.activeSelf) ? 1 : (1 - this.localPlayerLastSpawnedBoardIndex));
		if (PhotonNetwork.InRoom)
		{
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			long num4 = BitPackUtils.PackWorldPosForNetwork(velocity);
			long num5 = BitPackUtils.PackWorldPosForNetwork(avelocity);
			short num6 = BitPackUtils.PackColorForNetwork(boardColor);
			this.photonView.RPC("DropBoard_RPC", RpcTarget.All, new object[]
			{
				num == 1,
				num2,
				num3,
				num4,
				num5,
				num6
			});
			return;
		}
		this.SpawnBoard(orCreatePlayerData, num, position, rotation, velocity, avelocity, boardColor);
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x000F90F8 File Offset: 0x000F72F8
	[PunRPC]
	public void DropBoard_RPC(bool boardIndex1, long positionPacked, int rotationPacked, long velocityPacked, long avelocityPacked, short colorPacked, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DropBoard_RPC");
		int boardIndex2 = boardIndex1 ? 1 : 0;
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(info.Sender.ActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(positionPacked);
		if (!rigContainer.Rig.IsPositionInRange(position, 5f))
		{
			return;
		}
		this.SpawnBoard(orCreatePlayerData, boardIndex2, position, BitPackUtils.UnpackQuaternionFromNetwork(rotationPacked), BitPackUtils.UnpackWorldPosFromNetwork(velocityPacked), BitPackUtils.UnpackWorldPosFromNetwork(avelocityPacked), BitPackUtils.UnpackColorFromNetwork(colorPacked));
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x000F91A4 File Offset: 0x000F73A4
	public void SendGrabBoardRPC(FreeHoverboardInstance board)
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("GrabBoard_RPC", RpcTarget.All, new object[]
			{
				board.ownerActorNumber,
				board.boardIndex == 1
			});
			board.gameObject.SetActive(false);
			return;
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x000F9208 File Offset: 0x000F7408
	[PunRPC]
	public void GrabBoard_RPC(int ownerActorNumber, bool boardIndex1, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GrabBoard_RPC");
		int boardIndex2 = boardIndex1 ? 1 : 0;
		if (NetworkSystem.Instance.GetNetPlayerByID(ownerActorNumber) == null)
		{
			return;
		}
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(ownerActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FreeHoverboardInstance board = orCreatePlayerData.GetBoard(boardIndex2);
		if (board.IsNull())
		{
			return;
		}
		if (info.Sender.ActorNumber != ownerActorNumber)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if (!rigContainer.Rig.IsPositionInRange(board.transform.position, 5f))
			{
				return;
			}
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x000F92C0 File Offset: 0x000F74C0
	public void PreserveMaxHoverboardsConstraint(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, out dataPerPlayer))
		{
			return;
		}
		if (dataPerPlayer.board0.gameObject.activeSelf && dataPerPlayer.board1.gameObject.activeSelf)
		{
			FreeHoverboardInstance board = dataPerPlayer.GetBoard(1 - this.localPlayerLastSpawnedBoardIndex);
			this.SendGrabBoardRPC(board);
		}
	}

	// Token: 0x04003B07 RID: 15111
	[SerializeField]
	private FreeHoverboardInstance freeHoverboardPrefab;

	// Token: 0x04003B08 RID: 15112
	private Stack<FreeHoverboardInstance> freeBoardPool = new Stack<FreeHoverboardInstance>(20);

	// Token: 0x04003B09 RID: 15113
	private const int NumPlayers = 10;

	// Token: 0x04003B0A RID: 15114
	private const int NumFreeBoardsPerPlayer = 2;

	// Token: 0x04003B0B RID: 15115
	private int localPlayerLastSpawnedBoardIndex;

	// Token: 0x04003B0C RID: 15116
	private Dictionary<int, FreeHoverboardManager.DataPerPlayer> perPlayerData = new Dictionary<int, FreeHoverboardManager.DataPerPlayer>();

	// Token: 0x02000754 RID: 1876
	private struct DataPerPlayer
	{
		// Token: 0x06002F0A RID: 12042 RVA: 0x000F933C File Offset: 0x000F753C
		public void Init(int actorNumber, Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0 = freeBoardPool.Pop();
			this.board0.ownerActorNumber = actorNumber;
			this.board0.boardIndex = 0;
			this.board1 = freeBoardPool.Pop();
			this.board1.ownerActorNumber = actorNumber;
			this.board1.boardIndex = 1;
			this.spamCheck = new CallLimiterWithCooldown(5f, 10, 1f);
		}

		// Token: 0x06002F0B RID: 12043 RVA: 0x000F93A8 File Offset: 0x000F75A8
		public void ReturnBoards(Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0.gameObject.SetActive(false);
			freeBoardPool.Push(this.board0);
			this.board1.gameObject.SetActive(false);
			freeBoardPool.Push(this.board1);
		}

		// Token: 0x06002F0C RID: 12044 RVA: 0x000F93E4 File Offset: 0x000F75E4
		public FreeHoverboardInstance GetBoard(int boardIndex)
		{
			if (boardIndex != 1)
			{
				return this.board0;
			}
			return this.board1;
		}

		// Token: 0x04003B0D RID: 15117
		public FreeHoverboardInstance board0;

		// Token: 0x04003B0E RID: 15118
		public FreeHoverboardInstance board1;

		// Token: 0x04003B0F RID: 15119
		public CallLimiterWithCooldown spamCheck;
	}
}
