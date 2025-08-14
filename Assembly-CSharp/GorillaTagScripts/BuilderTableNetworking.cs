using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C1D RID: 3101
	public class BuilderTableNetworking : MonoBehaviourPunCallbacks
	{
		// Token: 0x06004C18 RID: 19480 RVA: 0x00178518 File Offset: 0x00176718
		private void Awake()
		{
			this.masterClientTableInit = new List<BuilderTableNetworking.PlayerTableInitState>(10);
			this.masterClientTableValidators = new List<BuilderTableNetworking.PlayerTableInitState>(10);
			this.localClientTableInit = new BuilderTableNetworking.PlayerTableInitState();
			this.localValidationTable = new BuilderTableNetworking.PlayerTableInitState();
			this.callLimiters = new CallLimiter[26];
			this.callLimiters[0] = new CallLimiter(20, 30f, 0.5f);
			this.callLimiters[1] = new CallLimiter(200, 1f, 0.5f);
			this.callLimiters[2] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[3] = new CallLimiter(2, 1f, 0.5f);
			this.callLimiters[4] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[5] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[6] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[7] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[8] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[9] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[10] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[11] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[12] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[13] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[14] = new CallLimiter(100, 1f, 0.5f);
			this.callLimiters[15] = new CallLimiter(100, 1f, 0.5f);
			this.callLimiters[16] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[17] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[18] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[19] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[20] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[21] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[22] = new CallLimiter(20, 1f, 0.5f);
			this.callLimiters[23] = new CallLimiter(20, 1f, 0.5f);
			this.callLimiters[24] = new CallLimiter(3, 30f, 0.5f);
			this.callLimiters[25] = new CallLimiter(10, 1f, 0.5f);
			this.armShelfRequests = new List<Player>(10);
		}

		// Token: 0x06004C19 RID: 19481 RVA: 0x0017880B File Offset: 0x00176A0B
		public void SetTable(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x06004C1A RID: 19482 RVA: 0x00178814 File Offset: 0x00176A14
		private BuilderTable GetTable()
		{
			return this.currTable;
		}

		// Token: 0x06004C1B RID: 19483 RVA: 0x0017881C File Offset: 0x00176A1C
		private int CreateLocalCommandId()
		{
			int result = this.nextLocalCommandId;
			this.nextLocalCommandId++;
			return result;
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x00178832 File Offset: 0x00176A32
		public BuilderTableNetworking.PlayerTableInitState GetLocalTableInit()
		{
			return this.localClientTableInit;
		}

		// Token: 0x06004C1D RID: 19485 RVA: 0x0017883C File Offset: 0x00176A3C
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (!newMasterClient.IsLocal)
			{
				this.localClientTableInit.Reset();
				BuilderTable table = this.GetTable();
				if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
				{
					if (table.GetTableState() == BuilderTable.TableState.Ready)
					{
						table.SetTableState(BuilderTable.TableState.WaitForMasterResync);
					}
					else if (table.GetTableState() == BuilderTable.TableState.WaitForMasterResync || table.GetTableState() == BuilderTable.TableState.ReceivingMasterResync)
					{
						table.SetTableState(BuilderTable.TableState.WaitForMasterResync);
					}
					else
					{
						table.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
					}
					this.PlayerEnterBuilder();
				}
				return;
			}
			this.masterClientTableInit.Clear();
			this.localClientTableInit.Reset();
			BuilderTable table2 = this.GetTable();
			BuilderTable.TableState tableState = table2.GetTableState();
			bool flag = (tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.WaitingForZoneAndRoom && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.ReceivingMasterResync) || table2.pieces.Count <= 0;
			if (!flag)
			{
				flag |= (table2.pieces.Count <= 0);
			}
			if (flag)
			{
				table2.ClearTable();
				table2.ClearQueuedCommands();
				table2.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
				return;
			}
			for (int i = 0; i < table2.pieces.Count; i++)
			{
				BuilderPiece builderPiece = table2.pieces[i];
				Player player = PhotonNetwork.CurrentRoom.GetPlayer(builderPiece.heldByPlayerActorNumber, false);
				if (table2.pieces[i].state == BuilderPiece.State.Grabbed && player == null)
				{
					Vector3 position = builderPiece.transform.position;
					Quaternion rotation = builderPiece.transform.rotation;
					Debug.LogErrorFormat("We have a piece {0} {1} held by an invalid player {2} dropping", new object[]
					{
						builderPiece.name,
						builderPiece.pieceId,
						builderPiece.heldByPlayerActorNumber
					});
					this.CreateLocalCommandId();
					builderPiece.ClearParentHeld();
					builderPiece.ClearParentPiece(false);
					builderPiece.transform.localScale = Vector3.one;
					builderPiece.SetState(BuilderPiece.State.Dropped, false);
					builderPiece.transform.SetLocalPositionAndRotation(position, rotation);
					if (builderPiece.rigidBody != null)
					{
						builderPiece.rigidBody.position = position;
						builderPiece.rigidBody.rotation = rotation;
						builderPiece.rigidBody.velocity = Vector3.zero;
						builderPiece.rigidBody.angularVelocity = Vector3.zero;
					}
				}
			}
			table2.ClearQueuedCommands();
			table2.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x06004C1E RID: 19486 RVA: 0x00178A78 File Offset: 0x00176C78
		public override void OnPlayerLeftRoom(Player player)
		{
			Debug.LogFormat("Player {0} left room", new object[]
			{
				player.ActorNumber
			});
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				if (table.isTableMutable)
				{
					if (!PhotonNetwork.IsMasterClient)
					{
						table.DropAllPiecesForPlayerLeaving(player.ActorNumber);
					}
					else
					{
						table.RecycleAllPiecesForPlayerLeaving(player.ActorNumber);
					}
				}
				table.PlayerLeftRoom(player.ActorNumber);
			}
			if (!table.isTableMutable && table.linkedTerminal != null && table.linkedTerminal.IsPlayerDriver(player))
			{
				table.linkedTerminal.ResetTerminalControl();
				if (NetworkSystem.Instance.IsMasterClient)
				{
					base.photonView.RPC("SetBlocksTerminalDriverRPC", RpcTarget.All, new object[]
					{
						-2
					});
				}
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			table.RemoveArmShelfForPlayer(player);
			table.VerifySetSelections();
			if (player != PhotonNetwork.LocalPlayer)
			{
				this.DestroyPlayerTableInit(player);
			}
		}

		// Token: 0x06004C1F RID: 19487 RVA: 0x00178B67 File Offset: 0x00176D67
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			BuilderTable table = this.GetTable();
			table.SetPendingMap(null);
			table.SetInRoom(true);
		}

		// Token: 0x06004C20 RID: 19488 RVA: 0x00178B82 File Offset: 0x00176D82
		public override void OnLeftRoom()
		{
			this.PlayerExitBuilder();
			BuilderTable table = this.GetTable();
			table.SetPendingMap(null);
			table.SetInRoom(false);
			this.armShelfRequests.Clear();
		}

		// Token: 0x06004C21 RID: 19489 RVA: 0x00178BA8 File Offset: 0x00176DA8
		private void Update()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				this.UpdateNewPlayerInit();
			}
		}

		// Token: 0x06004C22 RID: 19490 RVA: 0x00178BB8 File Offset: 0x00176DB8
		public void PlayerEnterBuilder()
		{
			this.tablePhotonView.RPC("PlayerEnterBuilderRPC", PhotonNetwork.MasterClient, new object[]
			{
				PhotonNetwork.LocalPlayer,
				true
			});
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x06004C23 RID: 19491 RVA: 0x00178C2C File Offset: 0x00176E2C
		[PunRPC]
		public void PlayerEnterBuilderRPC(Player player, bool entered, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerEnterBuilderRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlayerEnterMaster, info))
			{
				return;
			}
			if (player == null || !player.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (entered)
			{
				BuilderTable.TableState tableState = table.GetTableState();
				if (tableState == BuilderTable.TableState.WaitingForInitalBuild || (this.IsPrivateMasterClient() && tableState == BuilderTable.TableState.WaitingForZoneAndRoom))
				{
					table.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
				}
				if (player != PhotonNetwork.LocalPlayer)
				{
					this.CreateSerializedTableForNewPlayerInit(player);
				}
				if (table.isTableMutable)
				{
					this.RequestCreateArmShelfForPlayer(player);
					return;
				}
				if (table.linkedTerminal != null)
				{
					base.photonView.RPC("SetBlocksTerminalDriverRPC", player, new object[]
					{
						table.linkedTerminal.GetDriverID
					});
					return;
				}
			}
			else
			{
				if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.DestroyPlayerTableInit(player);
				}
				if (table.isTableMutable)
				{
					table.RemoveArmShelfForPlayer(player);
				}
			}
		}

		// Token: 0x06004C24 RID: 19492 RVA: 0x00178D10 File Offset: 0x00176F10
		public void PlayerExitBuilder()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.tablePhotonView.RPC("PlayerEnterBuilderRPC", PhotonNetwork.MasterClient, new object[]
				{
					PhotonNetwork.LocalPlayer,
					false
				});
			}
			BuilderTable table = this.GetTable();
			table.ClearTable();
			table.ClearQueuedCommands();
			this.localClientTableInit.Reset();
			this.armShelfRequests.Clear();
			this.masterClientTableInit.Clear();
		}

		// Token: 0x06004C25 RID: 19493 RVA: 0x00178D87 File Offset: 0x00176F87
		public bool IsPrivateMasterClient()
		{
			return PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient && NetworkSystem.Instance.SessionIsPrivate;
		}

		// Token: 0x06004C26 RID: 19494 RVA: 0x00178DA4 File Offset: 0x00176FA4
		private void UpdateNewPlayerInit()
		{
			if (this.GetTable().GetTableState() == BuilderTable.TableState.Ready)
			{
				for (int i = 0; i < this.masterClientTableInit.Count; i++)
				{
					if (this.masterClientTableInit[i].waitForInitTimeRemaining >= 0f)
					{
						this.masterClientTableInit[i].waitForInitTimeRemaining -= Time.deltaTime;
						if (this.masterClientTableInit[i].waitForInitTimeRemaining <= 0f)
						{
							this.StartCreatingSerializedTable(this.masterClientTableInit[i].player);
							this.masterClientTableInit[i].waitForInitTimeRemaining = -1f;
							this.masterClientTableInit[i].sendNextChunkTimeRemaining = 0f;
						}
					}
					else if (this.masterClientTableInit[i].sendNextChunkTimeRemaining >= 0f)
					{
						this.masterClientTableInit[i].sendNextChunkTimeRemaining -= Time.deltaTime;
						if (this.masterClientTableInit[i].sendNextChunkTimeRemaining <= 0f)
						{
							this.SendNextTableData(this.masterClientTableInit[i].player);
							if (this.masterClientTableInit[i].numSerializedBytes < this.masterClientTableInit[i].totalSerializedBytes)
							{
								this.masterClientTableInit[i].sendNextChunkTimeRemaining = 0f;
							}
							else
							{
								this.masterClientTableInit[i].sendNextChunkTimeRemaining = -1f;
							}
						}
					}
				}
			}
		}

		// Token: 0x06004C27 RID: 19495 RVA: 0x00178F34 File Offset: 0x00177134
		private void StartCreatingSerializedTable(Player newPlayer)
		{
			BuilderTable table = this.GetTable();
			BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(newPlayer);
			playerTableInit.totalSerializedBytes = table.SerializeTableState(playerTableInit.serializedTableState, 1048576);
			byte[] array = GZipStream.CompressBuffer(playerTableInit.serializedTableState);
			playerTableInit.totalSerializedBytes = array.Length;
			Array.Copy(array, 0, playerTableInit.serializedTableState, 0, playerTableInit.totalSerializedBytes);
			playerTableInit.numSerializedBytes = 0;
			this.tablePhotonView.RPC("StartBuildTableRPC", newPlayer, new object[]
			{
				playerTableInit.totalSerializedBytes
			});
		}

		// Token: 0x06004C28 RID: 19496 RVA: 0x00178FBC File Offset: 0x001771BC
		[PunRPC]
		public void StartBuildTableRPC(int totalBytes, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "StartBuildTableRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.TableDataStart, info))
			{
				return;
			}
			if (totalBytes <= 0 || totalBytes > 1048576)
			{
				Debug.LogError("Builder Table Bytes is too large: " + totalBytes.ToString());
				return;
			}
			BuilderTable table = this.GetTable();
			GTDev.Log<string>("StartBuildTableRPC with current state " + table.GetTableState().ToString(), null);
			if (table.GetTableState() != BuilderTable.TableState.WaitForMasterResync && table.GetTableState() != BuilderTable.TableState.WaitingForInitalBuild)
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.WaitForMasterResync)
			{
				table.SetTableState(BuilderTable.TableState.ReceivingMasterResync);
			}
			else
			{
				table.SetTableState(BuilderTable.TableState.ReceivingInitialBuild);
			}
			this.localClientTableInit.Reset();
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.localClientTableInit;
			playerTableInitState.player = PhotonNetwork.LocalPlayer;
			playerTableInitState.totalSerializedBytes = totalBytes;
			table.ClearQueuedCommands();
		}

		// Token: 0x06004C29 RID: 19497 RVA: 0x00179098 File Offset: 0x00177298
		private void SendNextTableData(Player requestingPlayer)
		{
			BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(requestingPlayer);
			if (playerTableInit == null)
			{
				Debug.LogErrorFormat("No Table init found for player {0}", new object[]
				{
					requestingPlayer.ActorNumber
				});
				return;
			}
			int num = Mathf.Min(1000, playerTableInit.totalSerializedBytes - playerTableInit.numSerializedBytes);
			if (num <= 0)
			{
				return;
			}
			Array.Copy(playerTableInit.serializedTableState, playerTableInit.numSerializedBytes, playerTableInit.chunk, 0, num);
			playerTableInit.numSerializedBytes += num;
			this.tablePhotonView.RPC("SendTableDataRPC", requestingPlayer, new object[]
			{
				num,
				playerTableInit.chunk
			});
		}

		// Token: 0x06004C2A RID: 19498 RVA: 0x0017913C File Offset: 0x0017733C
		[PunRPC]
		public void SendTableDataRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SendTableDataRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (numBytes <= 0 || numBytes > 1000)
			{
				Debug.LogErrorFormat("Builder Table Send Data numBytes is too large {0}", new object[]
				{
					numBytes
				});
				return;
			}
			if (bytes.Length > 1000)
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.TableData, info))
			{
				return;
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.localClientTableInit;
			if (playerTableInitState.numSerializedBytes + bytes.Length > 1048576)
			{
				Debug.LogErrorFormat("Builder Table serialized bytes is larger than buffer {0}", new object[]
				{
					playerTableInitState.numSerializedBytes + bytes.Length
				});
				return;
			}
			Array.Copy(bytes, 0, playerTableInitState.serializedTableState, playerTableInitState.numSerializedBytes, bytes.Length);
			playerTableInitState.numSerializedBytes += bytes.Length;
			if (playerTableInitState.numSerializedBytes >= playerTableInitState.totalSerializedBytes)
			{
				this.GetTable().SetTableState(BuilderTable.TableState.InitialBuild);
			}
		}

		// Token: 0x06004C2B RID: 19499 RVA: 0x00179224 File Offset: 0x00177424
		private bool DoesTableInitExist(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004C2C RID: 19500 RVA: 0x00179268 File Offset: 0x00177468
		private BuilderTableNetworking.PlayerTableInitState CreatePlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					this.masterClientTableInit[i].Reset();
					return this.masterClientTableInit[i];
				}
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = new BuilderTableNetworking.PlayerTableInitState();
			playerTableInitState.player = player;
			this.masterClientTableInit.Add(playerTableInitState);
			return playerTableInitState;
		}

		// Token: 0x06004C2D RID: 19501 RVA: 0x001792E4 File Offset: 0x001774E4
		public void ResetSerializedTableForAllPlayers()
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				this.masterClientTableInit[i].waitForInitTimeRemaining = 1f;
				this.masterClientTableInit[i].sendNextChunkTimeRemaining = -1f;
				this.masterClientTableInit[i].numSerializedBytes = 0;
				this.masterClientTableInit[i].totalSerializedBytes = 0;
			}
		}

		// Token: 0x06004C2E RID: 19502 RVA: 0x00179357 File Offset: 0x00177557
		private void CreateSerializedTableForNewPlayerInit(Player newPlayer)
		{
			if (this.DoesTableInitExist(newPlayer))
			{
				return;
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.CreatePlayerTableInit(newPlayer);
			playerTableInitState.waitForInitTimeRemaining = 1f;
			playerTableInitState.sendNextChunkTimeRemaining = -1f;
		}

		// Token: 0x06004C2F RID: 19503 RVA: 0x00179380 File Offset: 0x00177580
		private void DestroyPlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					this.masterClientTableInit.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06004C30 RID: 19504 RVA: 0x001793D4 File Offset: 0x001775D4
		private BuilderTableNetworking.PlayerTableInitState GetPlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					return this.masterClientTableInit[i];
				}
			}
			return null;
		}

		// Token: 0x06004C31 RID: 19505 RVA: 0x00179424 File Offset: 0x00177624
		private bool ValidateMasterClientIsReady(Player player)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return false;
			}
			if (player != null && !player.IsMasterClient)
			{
				BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(player);
				if (playerTableInit != null && playerTableInit.numSerializedBytes < playerTableInit.totalSerializedBytes)
				{
					return false;
				}
			}
			return this.GetTable().GetTableState() == BuilderTable.TableState.Ready;
		}

		// Token: 0x06004C32 RID: 19506 RVA: 0x00179474 File Offset: 0x00177674
		private bool ValidateCallLimits(BuilderTableNetworking.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= BuilderTableNetworking.RPC.PlayerEnterMaster && rpcCall < BuilderTableNetworking.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x06004C33 RID: 19507 RVA: 0x001794A2 File Offset: 0x001776A2
		[PunRPC]
		public void RequestFailedRPC(int localCommandId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestFailedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RequestFailed, info))
			{
				return;
			}
			this.GetTable().RollbackFailedCommand(localCommandId);
		}

		// Token: 0x06004C34 RID: 19508 RVA: 0x000023F5 File Offset: 0x000005F5
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x000023F5 File Offset: 0x000005F5
		public void RequestCreatePieceRPC(int newPieceType, long packedPosition, int packedRotation, int materialType, PhotonMessageInfo info)
		{
		}

		// Token: 0x06004C36 RID: 19510 RVA: 0x000023F5 File Offset: 0x000005F5
		public void PieceCreatedRPC(int pieceType, int pieceId, long packedPosition, int packedRotation, int materialType, Player creatingPlayer, PhotonMessageInfo info)
		{
		}

		// Token: 0x06004C37 RID: 19511 RVA: 0x001794D8 File Offset: 0x001776D8
		public void CreateShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, int shelfID)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			BuilderPiece piecePrefab = table.GetPiecePrefab(pieceType);
			if (!table.HasEnoughResources(piecePrefab))
			{
				Debug.Log("Not Enough Resources");
				return;
			}
			if (state != BuilderPiece.State.OnShelf)
			{
				if (state != BuilderPiece.State.OnConveyor)
				{
					return;
				}
				if (shelfID < 0 || shelfID >= table.conveyors.Count)
				{
					return;
				}
			}
			else if (shelfID < 0 || shelfID >= table.dispenserShelves.Count)
			{
				return;
			}
			int num = table.CreatePieceId();
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			base.photonView.RPC("PieceCreatedByShelfRPC", RpcTarget.All, new object[]
			{
				pieceType,
				num,
				num2,
				num3,
				materialType,
				(byte)state,
				shelfID,
				PhotonNetwork.LocalPlayer
			});
		}

		// Token: 0x06004C38 RID: 19512 RVA: 0x001795D4 File Offset: 0x001777D4
		[PunRPC]
		public void PieceCreatedByShelfRPC(int pieceType, int pieceId, long packedPosition, int packedRotation, int materialType, byte state, int shelfID, Player creatingPlayer, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.CreateShelfPieceMaster, info))
			{
				return;
			}
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(packedPosition);
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRotation);
			if (!table.ValidatePieceWorldTransform(position, rotation))
			{
				return;
			}
			if (state == 4)
			{
				table.CreateDispenserShelfPiece(pieceType, pieceId, position, rotation, materialType, shelfID);
				return;
			}
			if (state != 7)
			{
				return;
			}
			table.CreateConveyorPiece(pieceType, pieceId, position, rotation, materialType, shelfID, info.SentServerTimestamp);
		}

		// Token: 0x06004C39 RID: 19513 RVA: 0x00179658 File Offset: 0x00177858
		public void RequestRecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			if (!table.isTableMutable)
			{
				return;
			}
			float num = 10000f;
			if (!position.IsValid(num) || !rotation.IsValid())
			{
				return;
			}
			if (recyclerID > 32767 || recyclerID < -1)
			{
				return;
			}
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			base.photonView.RPC("PieceDestroyedRPC", RpcTarget.All, new object[]
			{
				pieceId,
				num2,
				num3,
				playFX,
				(short)recyclerID
			});
		}

		// Token: 0x06004C3A RID: 19514 RVA: 0x00179708 File Offset: 0x00177908
		[PunRPC]
		public void PieceDestroyedRPC(int pieceId, long packedPosition, int packedRotation, bool playFX, short recyclerID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceDestroyedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RecyclePieceMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(packedPosition);
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRotation);
			float num = 10000f;
			if (!position.IsValid(num) || !rotation.IsValid())
			{
				return;
			}
			table.RecyclePiece(pieceId, position, rotation, playFX, (int)recyclerID, info.Sender);
		}

		// Token: 0x06004C3B RID: 19515 RVA: 0x0017978C File Offset: 0x0017798C
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (piece == null)
			{
				return;
			}
			int pieceId = piece.pieceId;
			int num = (parentPiece != null) ? parentPiece.pieceId : -1;
			int num2 = (attachPiece != null) ? attachPiece.pieceId : -1;
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidatePlacePieceParams(pieceId, num2, bumpOffsetX, bumpOffsetZ, twist, num, attachIndex, parentAttachIndex, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			int num3 = this.CreateLocalCommandId();
			attachPiece.requestedParentPiece = parentPiece;
			table.UpdatePieceData(attachPiece);
			table.PlacePiece(num3, pieceId, num2, bumpOffsetX, bumpOffsetZ, twist, num, attachIndex, parentAttachIndex, NetPlayer.Get(PhotonNetwork.LocalPlayer), PhotonNetwork.ServerTimestamp, true);
			int num4 = BuilderTable.PackPiecePlacement(twist, bumpOffsetX, bumpOffsetZ);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestPlacePieceRPC", RpcTarget.MasterClient, new object[]
				{
					num3,
					pieceId,
					num2,
					num4,
					num,
					attachIndex,
					parentAttachIndex,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x06004C3C RID: 19516 RVA: 0x001798B4 File Offset: 0x00177AB4
		[PunRPC]
		public void RequestPlacePieceRPC(int localCommandId, int pieceId, int attachPieceId, int placement, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestPlacePieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlacePieceMaster, info) || placedByPlayer == null || !placedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			bool isMasterClient = info.Sender.IsMasterClient;
			byte twist;
			sbyte bumpOffsetX;
			sbyte bumpOffsetZ;
			BuilderTable.UnpackPiecePlacement(placement, out twist, out bumpOffsetX, out bumpOffsetZ);
			bool flag = isMasterClient || table.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, NetPlayer.Get(placedByPlayer));
			if (flag)
			{
				flag &= (isMasterClient || table.ValidatePlacePieceState(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer));
			}
			if (flag)
			{
				BuilderPiece piece = table.GetPiece(parentPieceId);
				BuilderPiecePrivatePlot builderPiecePrivatePlot;
				if (piece != null && piece.TryGetPlotComponent(out builderPiecePrivatePlot) && !builderPiecePrivatePlot.IsPlotClaimed())
				{
					base.photonView.RPC("PlotClaimedRPC", RpcTarget.All, new object[]
					{
						parentPieceId,
						placedByPlayer,
						true
					});
				}
				base.photonView.RPC("PiecePlacedRPC", RpcTarget.All, new object[]
				{
					localCommandId,
					pieceId,
					attachPieceId,
					placement,
					parentPieceId,
					attachIndex,
					parentAttachIndex,
					placedByPlayer,
					info.SentServerTimestamp
				});
				return;
			}
			base.photonView.RPC("RequestFailedRPC", info.Sender, new object[]
			{
				localCommandId
			});
		}

		// Token: 0x06004C3D RID: 19517 RVA: 0x00179A68 File Offset: 0x00177C68
		[PunRPC]
		public void PiecePlacedRPC(int localCommandId, int pieceId, int attachPieceId, int placement, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer, int timeStamp, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PiecePlacedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlacePiece, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (placedByPlayer == null)
			{
				return;
			}
			if ((ulong)(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout) || (ulong)(info.SentServerTimestamp - timeStamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout))
			{
				timeStamp = PhotonNetwork.ServerTimestamp;
			}
			byte twist;
			sbyte bumpOffsetX;
			sbyte bumpOffsetZ;
			BuilderTable.UnpackPiecePlacement(placement, out twist, out bumpOffsetX, out bumpOffsetZ);
			table.PlacePiece(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, NetPlayer.Get(placedByPlayer), timeStamp, false);
		}

		// Token: 0x06004C3E RID: 19518 RVA: 0x00179B20 File Offset: 0x00177D20
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (piece == null)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateGrabPieceParams(piece.pieceId, isLefHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				this.CheckForFreedPlot(piece.pieceId, PhotonNetwork.LocalPlayer);
			}
			int num = this.CreateLocalCommandId();
			table.GrabPiece(num, piece.pieceId, isLefHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer), true);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				long num2 = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
				base.photonView.RPC("RequestGrabPieceRPC", RpcTarget.MasterClient, new object[]
				{
					num,
					piece.pieceId,
					isLefHand,
					num2,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x00179BFC File Offset: 0x00177DFC
		[PunRPC]
		public void RequestGrabPieceRPC(int localCommandId, int pieceId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestGrabPieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.GrabPieceMaster, info) || !grabbedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 localPosition;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				bool isMasterClient = info.Sender.IsMasterClient;
				bool flag = isMasterClient || table.ValidateGrabPieceParams(pieceId, isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer));
				if (flag)
				{
					flag &= (isMasterClient || table.ValidateGrabPieceState(pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer));
				}
				if (flag)
				{
					if (!info.Sender.IsMasterClient)
					{
						this.CheckForFreedPlot(pieceId, grabbedByPlayer);
					}
					base.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, new object[]
					{
						localCommandId,
						pieceId,
						isLeftHand,
						packedPosRot,
						grabbedByPlayer
					});
					return;
				}
				base.photonView.RPC("RequestFailedRPC", info.Sender, new object[]
				{
					localCommandId
				});
			}
		}

		// Token: 0x06004C40 RID: 19520 RVA: 0x00179D34 File Offset: 0x00177F34
		private void CheckForFreedPlot(int pieceId, Player grabbedByPlayer)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderPiece piece = this.GetTable().GetPiece(pieceId);
			if (piece != null && piece.parentPiece != null && piece.parentPiece.IsPrivatePlot() && piece.parentPiece.firstChildPiece.Equals(piece) && piece.nextSiblingPiece == null)
			{
				base.photonView.RPC("PlotClaimedRPC", RpcTarget.All, new object[]
				{
					piece.parentPiece.pieceId,
					grabbedByPlayer,
					false
				});
			}
		}

		// Token: 0x06004C41 RID: 19521 RVA: 0x00179DD4 File Offset: 0x00177FD4
		[PunRPC]
		public void PieceGrabbedRPC(int localCommandId, int pieceId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceGrabbedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.GrabPiece, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 localPosition;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
			table.GrabPiece(localCommandId, pieceId, isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer), false);
		}

		// Token: 0x06004C42 RID: 19522 RVA: 0x00179E38 File Offset: 0x00178038
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (piece == null)
			{
				return;
			}
			int pieceId = piece.pieceId;
			float num = 10000f;
			if (velocity.IsValid(num) && velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
			{
				velocity = velocity.normalized * BuilderTable.MAX_DROP_VELOCITY;
			}
			num = 10000f;
			if (angVelocity.IsValid(num) && angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
			{
				angVelocity = angVelocity.normalized * BuilderTable.MAX_DROP_ANG_VELOCITY;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateDropPieceParams(pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			int num2 = this.CreateLocalCommandId();
			table.DropPiece(num2, pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer), true);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestDropPieceRPC", RpcTarget.MasterClient, new object[]
				{
					num2,
					pieceId,
					position,
					rotation,
					velocity,
					angVelocity,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x06004C43 RID: 19523 RVA: 0x00179F70 File Offset: 0x00178170
		[PunRPC]
		public void RequestDropPieceRPC(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestDropPieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.DropPieceMaster, info) || !droppedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			bool isMasterClient = info.Sender.IsMasterClient;
			bool flag = isMasterClient || table.ValidateDropPieceParams(pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(droppedByPlayer));
			if (flag)
			{
				flag &= (isMasterClient || table.ValidateDropPieceState(pieceId, position, rotation, velocity, angVelocity, droppedByPlayer));
			}
			if (flag)
			{
				base.photonView.RPC("PieceDroppedRPC", RpcTarget.All, new object[]
				{
					localCommandId,
					pieceId,
					position,
					rotation,
					velocity,
					angVelocity,
					droppedByPlayer
				});
				return;
			}
			base.photonView.RPC("RequestFailedRPC", info.Sender, new object[]
			{
				localCommandId
			});
		}

		// Token: 0x06004C44 RID: 19524 RVA: 0x0017A09C File Offset: 0x0017829C
		[PunRPC]
		public void PieceDroppedRPC(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceDroppedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.DropPiece, info))
			{
				return;
			}
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3))
					{
						BuilderTable table = this.GetTable();
						if (!table.isTableMutable)
						{
							return;
						}
						table.DropPiece(localCommandId, pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(droppedByPlayer), false);
						return;
					}
				}
			}
		}

		// Token: 0x06004C45 RID: 19525 RVA: 0x0017A138 File Offset: 0x00178338
		public void PieceEnteredDropZone(BuilderPiece piece, BuilderDropZone.DropType dropType, int dropZoneId)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			BuilderPiece rootPiece = piece.GetRootPiece();
			if (!table.ValidateRepelPiece(rootPiece))
			{
				return;
			}
			long num = BitPackUtils.PackWorldPosForNetwork(rootPiece.transform.position);
			int num2 = BitPackUtils.PackQuaternionForNetwork(rootPiece.transform.rotation);
			base.photonView.RPC("PieceEnteredDropZoneRPC", RpcTarget.All, new object[]
			{
				rootPiece.pieceId,
				num,
				num2,
				dropZoneId
			});
		}

		// Token: 0x06004C46 RID: 19526 RVA: 0x0017A1D0 File Offset: 0x001783D0
		[PunRPC]
		public void PieceEnteredDropZoneRPC(int pieceId, long position, int rotation, int dropZoneId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceEnteredDropZoneRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PieceDropZone, info))
			{
				return;
			}
			Vector3 worldPos = BitPackUtils.UnpackWorldPosFromNetwork(position);
			float num = 10000f;
			if (!worldPos.IsValid(num))
			{
				return;
			}
			Quaternion worldRot = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
			if (!worldRot.IsValid())
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			table.PieceEnteredDropZone(pieceId, worldPos, worldRot, dropZoneId);
		}

		// Token: 0x06004C47 RID: 19527 RVA: 0x0017A24C File Offset: 0x0017844C
		[PunRPC]
		public void PlotClaimedRPC(int pieceId, Player claimingPlayer, bool claimed, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlotClaimedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlotClaimedMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (claimed)
			{
				table.PlotClaimed(pieceId, claimingPlayer);
				return;
			}
			table.PlotFreed(pieceId, claimingPlayer);
		}

		// Token: 0x06004C48 RID: 19528 RVA: 0x0017A2A8 File Offset: 0x001784A8
		public void RequestCreateArmShelfForPlayer(Player player)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				if (!this.armShelfRequests.Contains(player))
				{
					this.armShelfRequests.Add(player);
				}
				return;
			}
			if (table.playerToArmShelfLeft.ContainsKey(player.ActorNumber))
			{
				return;
			}
			int num = table.CreatePieceId();
			int num2 = table.CreatePieceId();
			int staticHash = table.armShelfPieceType.name.GetStaticHash();
			base.photonView.RPC("ArmShelfCreatedRPC", RpcTarget.All, new object[]
			{
				num,
				num2,
				staticHash,
				player
			});
		}

		// Token: 0x06004C49 RID: 19529 RVA: 0x0017A35C File Offset: 0x0017855C
		[PunRPC]
		public void ArmShelfCreatedRPC(int pieceIdLeft, int pieceIdRight, int pieceType, Player owningPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ArmShelfCreatedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ArmShelfCreated, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (pieceType != table.armShelfPieceType.name.GetStaticHash())
			{
				return;
			}
			table.CreateArmShelf(pieceIdLeft, pieceIdRight, pieceType, owningPlayer);
		}

		// Token: 0x06004C4A RID: 19530 RVA: 0x0017A3C0 File Offset: 0x001785C0
		public void RequestShelfSelection(int shelfID, int setId, bool isConveyor)
		{
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (isConveyor)
			{
				if (shelfID < 0 || shelfID >= table.conveyors.Count)
				{
					return;
				}
			}
			else if (shelfID < 0 || shelfID >= table.dispenserShelves.Count)
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestShelfSelectionRPC", RpcTarget.MasterClient, new object[]
				{
					shelfID,
					setId,
					isConveyor
				});
			}
		}

		// Token: 0x06004C4B RID: 19531 RVA: 0x0017A444 File Offset: 0x00178644
		[PunRPC]
		public void RequestShelfSelectionRPC(int shelfId, int setId, bool isConveyor, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestShelfSelectionRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ShelfSelection, info))
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateShelfSelectionParams(shelfId, setId, isConveyor, info.Sender))
			{
				return;
			}
			base.photonView.RPC("ShelfSelectionChangedRPC", RpcTarget.All, new object[]
			{
				shelfId,
				setId,
				isConveyor,
				info.Sender
			});
		}

		// Token: 0x06004C4C RID: 19532 RVA: 0x0017A4E4 File Offset: 0x001786E4
		[PunRPC]
		public void ShelfSelectionChangedRPC(int shelfId, int setId, bool isConveyor, Player caller, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ShelfSelectionChangedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ShelfSelectionMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (shelfId < 0 || ((!isConveyor || shelfId >= table.conveyors.Count) && (isConveyor || shelfId >= table.dispenserShelves.Count)))
			{
				return;
			}
			table.ChangeSetSelection(shelfId, setId, isConveyor);
		}

		// Token: 0x06004C4D RID: 19533 RVA: 0x0017A564 File Offset: 0x00178764
		public void RequestFunctionalPieceStateChange(int pieceID, byte state)
		{
			BuilderTable table = this.GetTable();
			if (!table.ValidateFunctionalPieceState(pieceID, state, NetworkSystem.Instance.LocalPlayer))
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestFunctionalPieceStateChangeRPC", RpcTarget.MasterClient, new object[]
				{
					pieceID,
					state
				});
			}
		}

		// Token: 0x06004C4E RID: 19534 RVA: 0x0017A5C0 File Offset: 0x001787C0
		[PunRPC]
		public void RequestFunctionalPieceStateChangeRPC(int pieceID, byte state, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestFunctionalPieceStateChangeRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetFunctionalState, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(info.Sender)))
			{
				table.OnFunctionalStateRequest(pieceID, state, NetPlayer.Get(info.Sender), info.SentServerTimestamp);
			}
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x0017A63C File Offset: 0x0017883C
		public void FunctionalPieceStateChangeMaster(int pieceID, byte state, Player instigator, int timeStamp)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(instigator)) && state != table.GetPiece(pieceID).functionalPieceState)
			{
				base.photonView.RPC("FunctionalPieceStateChangeRPC", RpcTarget.All, new object[]
				{
					pieceID,
					state,
					instigator,
					timeStamp
				});
			}
		}

		// Token: 0x06004C50 RID: 19536 RVA: 0x0017A6B0 File Offset: 0x001788B0
		[PunRPC]
		public void FunctionalPieceStateChangeRPC(int pieceID, byte state, Player caller, int timeStamp, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "FunctionalPieceStateChangeRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetFunctionalStateMaster, info))
			{
				return;
			}
			if (caller == null)
			{
				return;
			}
			if ((ulong)(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout) || (ulong)(info.SentServerTimestamp - timeStamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout))
			{
				timeStamp = PhotonNetwork.ServerTimestamp;
			}
			BuilderTable table = this.GetTable();
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(info.Sender)))
			{
				table.SetFunctionalPieceState(pieceID, state, NetPlayer.Get(caller), timeStamp);
			}
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x0017A75C File Offset: 0x0017895C
		public void RequestBlocksTerminalControl(bool locked)
		{
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			if (table.linkedTerminal.IsTerminalLocked == locked)
			{
				return;
			}
			base.photonView.RPC("RequestBlocksTerminalControlRPC", RpcTarget.MasterClient, new object[]
			{
				locked
			});
		}

		// Token: 0x06004C52 RID: 19538 RVA: 0x0017A7B8 File Offset: 0x001789B8
		[PunRPC]
		private void RequestBlocksTerminalControlRPC(bool lockedStatus, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestBlocksTerminalControlRPC");
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RequestTerminalControl, info))
			{
				return;
			}
			if (info.Sender == null)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			RigContainer rigContainer;
			if (!(VRRigCache.Instance != null) || !VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if ((table.linkedTerminal.transform.position - rigContainer.Rig.bodyTransform.position).sqrMagnitude > 9f)
			{
				return;
			}
			if (table.linkedTerminal.ValidateTerminalControlRequest(lockedStatus, info.Sender.ActorNumber))
			{
				int num = lockedStatus ? info.Sender.ActorNumber : -2;
				base.photonView.RPC("SetBlocksTerminalDriverRPC", RpcTarget.All, new object[]
				{
					num
				});
			}
		}

		// Token: 0x06004C53 RID: 19539 RVA: 0x0017A8B4 File Offset: 0x00178AB4
		[PunRPC]
		private void SetBlocksTerminalDriverRPC(int driver, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SetBlocksTerminalDriverRPC");
			if (info.Sender == null || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (driver != -2 && NetworkSystem.Instance.GetPlayer(driver) == null)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetTerminalDriver, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			table.linkedTerminal.SetTerminalDriver(driver);
		}

		// Token: 0x06004C54 RID: 19540 RVA: 0x0017A92B File Offset: 0x00178B2B
		public void RequestLoadSharedBlocksMap(string mapID)
		{
			base.photonView.RPC("LoadSharedBlocksMapRPC", RpcTarget.MasterClient, new object[]
			{
				mapID
			});
		}

		// Token: 0x06004C55 RID: 19541 RVA: 0x0017A948 File Offset: 0x00178B48
		[PunRPC]
		private void LoadSharedBlocksMapRPC(string mapID, PhotonMessageInfo info)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "LoadSharedBlocksMapRPC");
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.LoadSharedBlocksMap, info))
			{
				return;
			}
			if (info.Sender == null || mapID.IsNullOrEmpty())
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			if (!table.linkedTerminal.ValidateLoadMapRequest(mapID, info.Sender.ActorNumber))
			{
				GTDev.LogWarning<string>("SharedBlocks ValidateLoadMapRequest fail", null);
				return;
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (tableState == BuilderTable.TableState.Ready || tableState == BuilderTable.TableState.BadData)
			{
				table.SetPendingMap(mapID);
				base.photonView.RPC("SharedTableEventRPC", RpcTarget.Others, new object[]
				{
					0,
					mapID
				});
				this.localClientTableInit.Reset();
				UnityEvent onMapCleared = table.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
				table.SetTableState(BuilderTable.TableState.WaitingForSharedMapLoad);
				table.FindAndLoadSharedBlocksMap(mapID);
				return;
			}
			GTDev.LogWarning<string>("SharedBlocks Invalid state " + tableState.ToString(), null);
			this.LoadSharedBlocksFailedMaster(mapID);
		}

		// Token: 0x06004C56 RID: 19542 RVA: 0x0017AA5D File Offset: 0x00178C5D
		public void LoadSharedBlocksFailedMaster(string mapID)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (mapID.Length > 8)
			{
				return;
			}
			base.photonView.RPC("SharedTableEventRPC", RpcTarget.All, new object[]
			{
				1,
				mapID
			});
		}

		// Token: 0x06004C57 RID: 19543 RVA: 0x0017AA9A File Offset: 0x00178C9A
		public void SharedBlocksOutOfBoundsMaster(string mapID)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (mapID.Length > 8)
			{
				return;
			}
			base.photonView.RPC("SharedTableEventRPC", RpcTarget.All, new object[]
			{
				2,
				mapID
			});
		}

		// Token: 0x06004C58 RID: 19544 RVA: 0x0017AAD8 File Offset: 0x00178CD8
		[PunRPC]
		private void SharedTableEventRPC(byte eventType, string mapID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SharedTableEventRPC");
			if (eventType >= 3)
			{
				return;
			}
			if (!SharedBlocksManager.IsMapIDValid(mapID) && eventType != 1)
			{
				GTDev.LogWarning<string>("BuilderTableNetworking SharedTableEventRPC Invalid Map ID", null);
				return;
			}
			if (info.Sender == null || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SharedTableEvent, info))
			{
				GTDev.LogError<string>("SharedTableEventRPC Failed call limits", null);
				return;
			}
			if (this.GetTable().isTableMutable)
			{
				return;
			}
			switch (eventType)
			{
			case 0:
				this.OnSharedBlocksLoadStarted(mapID);
				return;
			case 1:
				this.OnLoadSharedBlocksFailed(mapID);
				return;
			case 2:
				this.OnSharedBlocksOutOfBounds(mapID);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004C59 RID: 19545 RVA: 0x0017AB74 File Offset: 0x00178D74
		private void OnSharedBlocksLoadStarted(string mapID)
		{
			this.localClientTableInit.Reset();
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				table.ClearTable();
				table.ClearQueuedCommands();
				table.SetPendingMap(mapID);
				table.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.PlayerEnterBuilder();
			}
		}

		// Token: 0x06004C5A RID: 19546 RVA: 0x0017ABBC File Offset: 0x00178DBC
		private void OnLoadSharedBlocksFailed(string mapID)
		{
			BuilderTable table = this.GetTable();
			string pendingMap = table.GetPendingMap();
			if (!pendingMap.IsNullOrEmpty() && !pendingMap.Equals(mapID))
			{
				GTDev.LogWarning<string>("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected map ID " + mapID, null);
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (!NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.WaitingForInitalBuild && tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected table state {0}", tableState), null);
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitingForSharedMapLoad && tableState != BuilderTable.TableState.WaitForInitialBuildMaster && tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected table state {0}", tableState), null);
				return;
			}
			table.SetPendingMap(null);
			if (table != null && !table.isTableMutable && table.linkedTerminal != null)
			{
				if (!SharedBlocksManager.IsMapIDValid(mapID))
				{
					UnityEvent<string> onMapLoadFailed = table.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("BAD MAP ID");
					return;
				}
				else
				{
					UnityEvent<string> onMapLoadFailed2 = table.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("LOAD FAILED");
				}
			}
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x0017ACC4 File Offset: 0x00178EC4
		private void OnSharedBlocksOutOfBounds(string mapID)
		{
			BuilderTable table = this.GetTable();
			string pendingMap = table.GetPendingMap();
			if (!pendingMap.IsNullOrEmpty() && !pendingMap.Equals(mapID))
			{
				GTDev.LogWarning<string>("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected map ID " + mapID, null);
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (!NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.WaitingForInitalBuild)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected table state {0}", tableState), null);
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForInitialBuildMaster && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected table state {0}", tableState), null);
				return;
			}
			table.SetPendingMap(null);
			if (table != null && !table.isTableMutable && table.linkedTerminal != null)
			{
				UnityEvent<string> onMapLoadFailed = table.OnMapLoadFailed;
				if (onMapLoadFailed == null)
				{
					return;
				}
				onMapLoadFailed.Invoke("BLOCKS ARE OUT OF BOUNDS FOR SHARED BLOCKS ROOM");
			}
		}

		// Token: 0x06004C5C RID: 19548 RVA: 0x000023F5 File Offset: 0x000005F5
		public void RequestPaintPiece(int pieceID, int materialType)
		{
		}

		// Token: 0x0400553F RID: 21823
		public PhotonView tablePhotonView;

		// Token: 0x04005540 RID: 21824
		private const int MAX_TABLE_BYTES = 1048576;

		// Token: 0x04005541 RID: 21825
		private const int MAX_TABLE_CHUNK_BYTES = 1000;

		// Token: 0x04005542 RID: 21826
		private const float DELAY_CLIENT_TABLE_CREATION_TIME = 1f;

		// Token: 0x04005543 RID: 21827
		private const float SEND_INIT_DATA_COOLDOWN = 0f;

		// Token: 0x04005544 RID: 21828
		private const int PIECE_SYNC_BYTES = 128;

		// Token: 0x04005545 RID: 21829
		private BuilderTable currTable;

		// Token: 0x04005546 RID: 21830
		private int nextLocalCommandId;

		// Token: 0x04005547 RID: 21831
		private List<BuilderTableNetworking.PlayerTableInitState> masterClientTableInit;

		// Token: 0x04005548 RID: 21832
		private List<BuilderTableNetworking.PlayerTableInitState> masterClientTableValidators;

		// Token: 0x04005549 RID: 21833
		private BuilderTableNetworking.PlayerTableInitState localClientTableInit;

		// Token: 0x0400554A RID: 21834
		private BuilderTableNetworking.PlayerTableInitState localValidationTable;

		// Token: 0x0400554B RID: 21835
		[HideInInspector]
		public List<Player> armShelfRequests;

		// Token: 0x0400554C RID: 21836
		private CallLimiter[] callLimiters;

		// Token: 0x02000C1E RID: 3102
		public class PlayerTableInitState
		{
			// Token: 0x06004C5E RID: 19550 RVA: 0x0017AD9A File Offset: 0x00178F9A
			public PlayerTableInitState()
			{
				this.serializedTableState = new byte[1048576];
				this.chunk = new byte[1000];
				this.Reset();
			}

			// Token: 0x06004C5F RID: 19551 RVA: 0x0017ADC8 File Offset: 0x00178FC8
			public void Reset()
			{
				this.player = null;
				this.numSerializedBytes = 0;
				this.totalSerializedBytes = 0;
			}

			// Token: 0x0400554D RID: 21837
			public Player player;

			// Token: 0x0400554E RID: 21838
			public int numSerializedBytes;

			// Token: 0x0400554F RID: 21839
			public int totalSerializedBytes;

			// Token: 0x04005550 RID: 21840
			public byte[] serializedTableState;

			// Token: 0x04005551 RID: 21841
			public byte[] chunk;

			// Token: 0x04005552 RID: 21842
			public float waitForInitTimeRemaining;

			// Token: 0x04005553 RID: 21843
			public float sendNextChunkTimeRemaining;
		}

		// Token: 0x02000C1F RID: 3103
		private enum RPC
		{
			// Token: 0x04005555 RID: 21845
			PlayerEnterMaster,
			// Token: 0x04005556 RID: 21846
			TableDataMaster,
			// Token: 0x04005557 RID: 21847
			TableData,
			// Token: 0x04005558 RID: 21848
			TableDataStart,
			// Token: 0x04005559 RID: 21849
			PlacePieceMaster,
			// Token: 0x0400555A RID: 21850
			PlacePiece,
			// Token: 0x0400555B RID: 21851
			GrabPieceMaster,
			// Token: 0x0400555C RID: 21852
			GrabPiece,
			// Token: 0x0400555D RID: 21853
			DropPieceMaster,
			// Token: 0x0400555E RID: 21854
			DropPiece,
			// Token: 0x0400555F RID: 21855
			RequestFailed,
			// Token: 0x04005560 RID: 21856
			PieceDropZone,
			// Token: 0x04005561 RID: 21857
			CreatePiece,
			// Token: 0x04005562 RID: 21858
			CreatePieceMaster,
			// Token: 0x04005563 RID: 21859
			CreateShelfPieceMaster,
			// Token: 0x04005564 RID: 21860
			RecyclePieceMaster,
			// Token: 0x04005565 RID: 21861
			PlotClaimedMaster,
			// Token: 0x04005566 RID: 21862
			ArmShelfCreated,
			// Token: 0x04005567 RID: 21863
			ShelfSelection,
			// Token: 0x04005568 RID: 21864
			ShelfSelectionMaster,
			// Token: 0x04005569 RID: 21865
			SetFunctionalState,
			// Token: 0x0400556A RID: 21866
			SetFunctionalStateMaster,
			// Token: 0x0400556B RID: 21867
			RequestTerminalControl,
			// Token: 0x0400556C RID: 21868
			SetTerminalDriver,
			// Token: 0x0400556D RID: 21869
			LoadSharedBlocksMap,
			// Token: 0x0400556E RID: 21870
			SharedTableEvent,
			// Token: 0x0400556F RID: 21871
			Count
		}

		// Token: 0x02000C20 RID: 3104
		private enum SharedTableEventTypes
		{
			// Token: 0x04005571 RID: 21873
			LOAD_STARTED,
			// Token: 0x04005572 RID: 21874
			LOAD_FAILED,
			// Token: 0x04005573 RID: 21875
			OUT_OF_BOUNDS,
			// Token: 0x04005574 RID: 21876
			COUNT
		}
	}
}
