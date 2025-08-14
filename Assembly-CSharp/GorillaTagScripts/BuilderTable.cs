using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BoingKit;
using CjLib;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C0A RID: 3082
	public class BuilderTable : MonoBehaviour
	{
		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06004B0C RID: 19212 RVA: 0x0016D045 File Offset: 0x0016B245
		[HideInInspector]
		public float gridSize
		{
			get
			{
				return this.pieceScale / 2f;
			}
		}

		// Token: 0x06004B0D RID: 19213 RVA: 0x0016D054 File Offset: 0x0016B254
		private void ExecuteAction(BuilderAction action)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(action.pieceId);
			BuilderPiece piece2 = this.GetPiece(action.parentPieceId);
			int playerActorNumber = action.playerActorNumber;
			bool flag = PhotonNetwork.LocalPlayer.ActorNumber == action.playerActorNumber;
			switch (action.type)
			{
			case BuilderActionType.AttachToPlayer:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				RigContainer rigContainer;
				if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerActorNumber), out rigContainer))
				{
					string.Format("Execute Builder Action {0} {1} {2} {3} {4}", new object[]
					{
						action.localCommandId,
						action.type,
						action.pieceId,
						action.playerActorNumber,
						action.isLeftHand
					});
					return;
				}
				BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
				Transform parentHeld = action.isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				piece.SetParentHeld(parentHeld, playerActorNumber, action.isLeftHand);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				BuilderPiece.State newState = flag ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed;
				piece.SetState(newState, false);
				if (!flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				if (flag)
				{
					BuilderPieceInteractor.instance.AddPieceToHeld(piece, action.isLeftHand, action.localPosition, action.localRotation);
					return;
				}
				break;
			}
			case BuilderActionType.DetachFromPlayer:
				if (flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				return;
			case BuilderActionType.AttachToPiece:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				Quaternion identity = Quaternion.identity;
				Vector3 zero = Vector3.zero;
				Vector3 position = piece.transform.position;
				Quaternion rotation = piece.transform.rotation;
				if (piece2 != null)
				{
					piece.BumpTwistToPositionRotation(action.twist, action.bumpOffsetx, action.bumpOffsetz, action.attachIndex, piece2.gridPlanes[action.parentAttachIndex], out zero, out identity, out position, out rotation);
				}
				piece.transform.SetPositionAndRotation(position, rotation);
				BuilderPiece.State stateWhenPlaced;
				if (piece2 == null)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedToArm;
				}
				else if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.state == BuilderPiece.State.Grabbed)
				{
					stateWhenPlaced = BuilderPiece.State.Grabbed;
				}
				else if (piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					stateWhenPlaced = BuilderPiece.State.GrabbedLocal;
				}
				else
				{
					stateWhenPlaced = BuilderPiece.State.AttachedToDropped;
				}
				BuilderPiece rootPiece = piece2.GetRootPiece();
				this.gridPlaneData.Clear();
				this.checkGridPlaneData.Clear();
				this.allPotentialPlacements.Clear();
				BuilderTable.tempPieceSet.Clear();
				QueryParameters queryParameters = new QueryParameters
				{
					layerMask = this.allPiecesMask
				};
				OverlapSphereCommand value = new OverlapSphereCommand(position, 1f, queryParameters);
				this.nearbyPiecesCommands[0] = value;
				OverlapSphereCommand.ScheduleBatch(this.nearbyPiecesCommands, this.nearbyPiecesResults, 1, 1024, default(JobHandle)).Complete();
				int num = 0;
				while (num < 1024 && this.nearbyPiecesResults[num].instanceID != 0)
				{
					BuilderPiece pieceInHand = piece;
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.nearbyPiecesResults[num].collider);
					if (builderPieceFromCollider != null && !BuilderTable.tempPieceSet.Contains(builderPieceFromCollider))
					{
						BuilderTable.tempPieceSet.Add(builderPieceFromCollider);
						if (this.CanPiecesPotentiallyOverlap(pieceInHand, rootPiece, stateWhenPlaced, builderPieceFromCollider))
						{
							for (int i = 0; i < builderPieceFromCollider.gridPlanes.Count; i++)
							{
								BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[i], -1);
								this.checkGridPlaneData.Add(builderGridPlaneData);
							}
						}
					}
					num++;
				}
				BuilderTableJobs.BuildTestPieceListForJob(piece, this.gridPlaneData);
				BuilderPotentialPlacement potentialPlacement = new BuilderPotentialPlacement
				{
					localPosition = zero,
					localRotation = identity,
					attachIndex = action.attachIndex,
					parentAttachIndex = action.parentAttachIndex,
					attachPiece = piece,
					parentPiece = piece2
				};
				this.CalcAllPotentialPlacements(this.gridPlaneData, this.checkGridPlaneData, potentialPlacement, this.allPotentialPlacements);
				piece.SetParentPiece(action.attachIndex, piece2, action.parentAttachIndex);
				for (int j = 0; j < this.allPotentialPlacements.Count; j++)
				{
					BuilderPotentialPlacement builderPotentialPlacement = this.allPotentialPlacements[j];
					BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex];
					BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
					BuilderAttachGridPlane movingParentGrid = builderAttachGridPlane.GetMovingParentGrid();
					bool flag2 = movingParentGrid != null;
					BuilderAttachGridPlane movingParentGrid2 = builderAttachGridPlane2.GetMovingParentGrid();
					bool flag3 = movingParentGrid2 != null;
					if (flag2 == flag3 && (!flag2 || !(movingParentGrid != movingParentGrid2)))
					{
						SnapOverlap newOverlap = this.builderPool.CreateSnapOverlap(builderAttachGridPlane2, builderPotentialPlacement.attachBounds);
						builderAttachGridPlane.AddSnapOverlap(newOverlap);
						SnapOverlap newOverlap2 = this.builderPool.CreateSnapOverlap(builderAttachGridPlane, builderPotentialPlacement.parentAttachBounds);
						builderAttachGridPlane2.AddSnapOverlap(newOverlap2);
					}
				}
				piece.transform.SetLocalPositionAndRotation(zero, identity);
				if (piece2 != null && piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					BuilderPiece rootPiece2 = piece2.GetRootPiece();
					BuilderPieceInteractor.instance.OnCountChangedForRoot(rootPiece2);
				}
				if (piece2 == null)
				{
					piece.SetActivateTimeStamp(action.timeStamp);
					piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
					this.SetIsDirty(true);
					if (flag)
					{
						BuilderPieceInteractor.instance.DisableCollisionsWithHands();
						return;
					}
				}
				else
				{
					if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
					{
						piece.SetState(BuilderPiece.State.AttachedToArm, false);
						return;
					}
					if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
					{
						piece.SetActivateTimeStamp(action.timeStamp);
						piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
						if (piece2 != null)
						{
							BuilderPiece attachedBuiltInPiece = piece2.GetAttachedBuiltInPiece();
							BuilderPiecePrivatePlot builderPiecePrivatePlot;
							if (attachedBuiltInPiece != null && attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
							{
								builderPiecePrivatePlot.OnPieceAttachedToPlot(piece);
							}
						}
						this.SetIsDirty(true);
						if (flag)
						{
							BuilderPieceInteractor.instance.DisableCollisionsWithHands();
							return;
						}
					}
					else
					{
						if (piece2.state == BuilderPiece.State.Grabbed)
						{
							piece.SetState(BuilderPiece.State.Grabbed, false);
							return;
						}
						if (piece2.state == BuilderPiece.State.GrabbedLocal)
						{
							piece.SetState(BuilderPiece.State.GrabbedLocal, false);
							return;
						}
						piece.SetState(BuilderPiece.State.AttachedToDropped, false);
						return;
					}
				}
				break;
			}
			case BuilderActionType.DetachFromPiece:
			{
				BuilderPiece piece3 = piece;
				bool flag4 = piece.state == BuilderPiece.State.GrabbedLocal;
				if (flag4)
				{
					piece3 = piece.GetRootPiece();
				}
				if (piece.state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.SetIsDirty(true);
					BuilderPiece attachedBuiltInPiece2 = piece.GetAttachedBuiltInPiece();
					BuilderPiecePrivatePlot builderPiecePrivatePlot2;
					if (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.TryGetPlotComponent(out builderPiecePrivatePlot2))
					{
						builderPiecePrivatePlot2.OnPieceDetachedFromPlot(piece);
					}
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				if (flag4)
				{
					BuilderPieceInteractor.instance.OnCountChangedForRoot(piece3);
					return;
				}
				break;
			}
			case BuilderActionType.MakePieceRoot:
				BuilderPiece.MakePieceRoot(piece);
				return;
			case BuilderActionType.DropPiece:
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				piece.SetState(BuilderPiece.State.Dropped, false);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				if (piece.rigidBody != null)
				{
					piece.rigidBody.position = action.localPosition;
					piece.rigidBody.rotation = action.localRotation;
					piece.rigidBody.velocity = action.velocity;
					piece.rigidBody.angularVelocity = action.angVelocity;
					return;
				}
				break;
			case BuilderActionType.AttachToShelf:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				int attachIndex = action.attachIndex;
				bool isLeftHand = action.isLeftHand;
				int parentAttachIndex = action.parentAttachIndex;
				float x = action.velocity.x;
				piece.transform.localScale = Vector3.one;
				piece.SetState(isLeftHand ? BuilderPiece.State.OnConveyor : BuilderPiece.State.OnShelf, false);
				if (isLeftHand)
				{
					if (attachIndex >= 0 && attachIndex < this.conveyors.Count)
					{
						BuilderConveyor builderConveyor = this.conveyors[attachIndex];
						float num2 = x / builderConveyor.GetFrameMovement();
						if (PhotonNetwork.ServerTimestamp >= parentAttachIndex)
						{
							uint num3 = (uint)(PhotonNetwork.ServerTimestamp - parentAttachIndex);
							num2 += num3 / 1000f;
						}
						piece.shelfOwner = attachIndex;
						builderConveyor.OnShelfPieceCreated(piece, num2);
						return;
					}
				}
				else
				{
					if (attachIndex >= 0 && attachIndex < this.dispenserShelves.Count)
					{
						BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[attachIndex];
						piece.shelfOwner = attachIndex;
						builderDispenserShelf.OnShelfPieceCreated(piece, false);
						return;
					}
					piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06004B0E RID: 19214 RVA: 0x0016D908 File Offset: 0x0016BB08
		public static bool AreStatesCompatibleForOverlap(BuilderPiece.State stateA, BuilderPiece.State stateB, BuilderPiece rootA, BuilderPiece rootB)
		{
			switch (stateA)
			{
			case BuilderPiece.State.None:
				return false;
			case BuilderPiece.State.AttachedAndPlaced:
				return stateB == BuilderPiece.State.AttachedAndPlaced;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Dropped:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.OnConveyor:
				return (stateB == BuilderPiece.State.AttachedToDropped || stateB == BuilderPiece.State.Dropped || stateB == BuilderPiece.State.OnShelf || stateB == BuilderPiece.State.OnConveyor) && rootA.Equals(rootB);
			case BuilderPiece.State.Grabbed:
				return stateB == BuilderPiece.State.Grabbed && rootA.Equals(rootB);
			case BuilderPiece.State.Displayed:
				return false;
			case BuilderPiece.State.GrabbedLocal:
				return stateB == BuilderPiece.State.GrabbedLocal && rootA.heldInLeftHand == rootB.heldInLeftHand;
			case BuilderPiece.State.AttachedToArm:
			{
				if (stateB != BuilderPiece.State.AttachedToArm)
				{
					return false;
				}
				object obj = (rootA.parentPiece != null) ? rootA.parentPiece : rootA;
				BuilderPiece obj2 = (rootB.parentPiece != null) ? rootB.parentPiece : rootB;
				return obj.Equals(obj2);
			}
			default:
				return false;
			}
		}

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06004B0F RID: 19215 RVA: 0x0016D9CD File Offset: 0x0016BBCD
		// (set) Token: 0x06004B10 RID: 19216 RVA: 0x0016D9D5 File Offset: 0x0016BBD5
		public int CurrentSaveSlot
		{
			get
			{
				return this.currentSaveSlot;
			}
			set
			{
				if (this.saveInProgress)
				{
					return;
				}
				if (!BuilderScanKiosk.IsSaveSlotValid(value))
				{
					this.currentSaveSlot = -1;
				}
				if (this.currentSaveSlot != value)
				{
					this.SetIsDirty(true);
				}
				this.currentSaveSlot = value;
			}
		}

		// Token: 0x06004B11 RID: 19217 RVA: 0x0016DA08 File Offset: 0x0016BC08
		private void Awake()
		{
			if (BuilderTable.zoneToInstance == null)
			{
				BuilderTable.zoneToInstance = new Dictionary<GTZone, BuilderTable>(2);
			}
			if (!BuilderTable.zoneToInstance.TryAdd(this.tableZone, this))
			{
				Object.Destroy(this);
			}
			this.acceptableSqrDistFromCenter = Mathf.Pow(217f * this.pieceScale, 2f);
			if (this.buttonSnapRotation != null)
			{
				this.buttonSnapRotation.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreeRotation));
				this.buttonSnapRotation.SetPressed(this.useSnapRotation);
			}
			if (this.buttonSnapPosition != null)
			{
				this.buttonSnapPosition.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreePosition));
				this.buttonSnapPosition.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
			}
			if (this.buttonSaveLayout != null)
			{
				this.buttonSaveLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonSaveLayout));
			}
			if (this.buttonClearLayout != null)
			{
				this.buttonClearLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonClearLayout));
			}
			this.isSetup = false;
			this.nextPieceId = 10000;
			BuilderTable.placedLayer = LayerMask.NameToLayer("Gorilla Object");
			BuilderTable.heldLayerLocal = LayerMask.NameToLayer("Prop");
			BuilderTable.heldLayer = LayerMask.NameToLayer("BuilderProp");
			BuilderTable.droppedLayer = LayerMask.NameToLayer("BuilderProp");
			this.currSnapParams = this.pushAndEaseParams;
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
			this.inRoom = false;
			this.inBuilderZone = false;
			this.builderNetworking.SetTable(this);
			this.plotOwners = new Dictionary<int, int>(10);
			this.doesLocalPlayerOwnPlot = false;
			this.queuedBuildCommands = new List<BuilderTable.BuilderCommand>(1028);
			if (this.isTableMutable)
			{
				this.playerToArmShelfLeft = new Dictionary<int, int>(10);
				this.playerToArmShelfRight = new Dictionary<int, int>(10);
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.rollBackActions = new List<BuilderAction>(1028);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.droppedPieces = new List<BuilderPiece>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.droppedPieceData = new List<BuilderTable.DroppedPieceData>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.SetupMonkeBlocksRoom();
				this.gridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.checkGridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.nearbyPiecesResults = new NativeArray<ColliderHit>(1024, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.nearbyPiecesCommands = new NativeArray<OverlapSphereCommand>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.allPotentialPlacements = new List<BuilderPotentialPlacement>(1024);
			}
			else
			{
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(128);
				this.rollBackActions = new List<BuilderAction>(128);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(128);
			}
			this.SetupResources();
			if (!this.isTableMutable && this.linkedTerminal != null)
			{
				this.linkedTerminal.Init(this);
			}
		}

		// Token: 0x06004B12 RID: 19218 RVA: 0x0016DCEB File Offset: 0x0016BEEB
		public static bool TryGetBuilderTableForZone(GTZone zone, out BuilderTable table)
		{
			if (BuilderTable.zoneToInstance == null)
			{
				table = null;
				return false;
			}
			return BuilderTable.zoneToInstance.TryGetValue(zone, out table);
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x0016DD08 File Offset: 0x0016BF08
		private void SetupMonkeBlocksRoom()
		{
			if (this.shelves == null)
			{
				this.shelves = new List<BuilderShelf>(64);
			}
			if (this.shelvesRoot != null)
			{
				this.shelvesRoot.GetComponentsInChildren<BuilderShelf>(this.shelves);
			}
			this.conveyors = new List<BuilderConveyor>(32);
			this.dispenserShelves = new List<BuilderDispenserShelf>(32);
			if (this.allShelvesRoot != null)
			{
				for (int i = 0; i < this.allShelvesRoot.Count; i++)
				{
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderConveyor>(BuilderTable.tempConveyors);
					this.conveyors.AddRange(BuilderTable.tempConveyors);
					BuilderTable.tempConveyors.Clear();
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderDispenserShelf>(BuilderTable.tempDispensers);
					this.dispenserShelves.AddRange(BuilderTable.tempDispensers);
					BuilderTable.tempDispensers.Clear();
				}
			}
			this.recyclers = new List<BuilderRecycler>(5);
			if (this.recyclerRoot != null)
			{
				for (int j = 0; j < this.recyclerRoot.Count; j++)
				{
					this.recyclerRoot[j].GetComponentsInChildren<BuilderRecycler>(BuilderTable.tempRecyclers);
					this.recyclers.AddRange(BuilderTable.tempRecyclers);
					BuilderTable.tempRecyclers.Clear();
				}
			}
			for (int k = 0; k < this.recyclers.Count; k++)
			{
				this.recyclers[k].recyclerID = k;
				this.recyclers[k].table = this;
			}
			this.dropZones = new List<BuilderDropZone>(6);
			this.dropZoneRoot.GetComponentsInChildren<BuilderDropZone>(this.dropZones);
			for (int l = 0; l < this.dropZones.Count; l++)
			{
				this.dropZones[l].dropZoneID = l;
				this.dropZones[l].table = this;
			}
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this;
			}
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x0016DF14 File Offset: 0x0016C114
		private void SetupResources()
		{
			this.maxResources = new int[3];
			if (this.totalResources != null && this.totalResources.quantities != null)
			{
				for (int i = 0; i < this.totalResources.quantities.Count; i++)
				{
					if (this.totalResources.quantities[i].type >= BuilderResourceType.Basic && this.totalResources.quantities[i].type < BuilderResourceType.Count)
					{
						this.maxResources[(int)this.totalResources.quantities[i].type] += this.totalResources.quantities[i].count;
					}
				}
			}
			this.usedResources = new int[3];
			this.reservedResources = new int[3];
			if (this.totalReservedResources != null && this.totalReservedResources.quantities != null)
			{
				for (int j = 0; j < this.totalReservedResources.quantities.Count; j++)
				{
					if (this.totalReservedResources.quantities[j].type >= BuilderResourceType.Basic && this.totalReservedResources.quantities[j].type < BuilderResourceType.Count)
					{
						this.reservedResources[(int)this.totalReservedResources.quantities[j].type] += this.totalReservedResources.quantities[j].count;
					}
				}
			}
			this.plotMaxResources = new int[3];
			if (this.resourcesPerPrivatePlot != null && this.resourcesPerPrivatePlot.quantities != null)
			{
				for (int k = 0; k < this.resourcesPerPrivatePlot.quantities.Count; k++)
				{
					if (this.resourcesPerPrivatePlot.quantities[k].type >= BuilderResourceType.Basic && this.resourcesPerPrivatePlot.quantities[k].type < BuilderResourceType.Count)
					{
						this.plotMaxResources[(int)this.resourcesPerPrivatePlot.quantities[k].type] += this.resourcesPerPrivatePlot.quantities[k].count;
					}
				}
			}
			this.OnAvailableResourcesChange();
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x0016E15C File Offset: 0x0016C35C
		private void Start()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom != this.inRoom)
			{
				this.SetInRoom(NetworkSystem.Instance.InRoom);
			}
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			this.HandleOnZoneChanged();
			this.RequestTableConfiguration();
			this.FetchSharedBlocksStartingMapConfig();
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdate));
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x0016E1EB File Offset: 0x0016C3EB
		private void OnApplicationQuit()
		{
			this.ClearTable();
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x06004B17 RID: 19223 RVA: 0x0016E1FC File Offset: 0x0016C3FC
		private void OnDestroy()
		{
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdate));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (this.isTableMutable)
			{
				if (this.gridPlaneData.IsCreated)
				{
					this.gridPlaneData.Dispose();
				}
				if (this.checkGridPlaneData.IsCreated)
				{
					this.checkGridPlaneData.Dispose();
				}
				if (this.nearbyPiecesResults.IsCreated)
				{
					this.nearbyPiecesResults.Dispose();
				}
				if (this.nearbyPiecesCommands.IsCreated)
				{
					this.nearbyPiecesCommands.Dispose();
				}
			}
			this.DestroyData();
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x0016E2B8 File Offset: 0x0016C4B8
		private void HandleOnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.tableZone);
			this.SetInBuilderZone(flag);
		}

		// Token: 0x06004B19 RID: 19225 RVA: 0x0016E2E0 File Offset: 0x0016C4E0
		public void InitIfNeeded()
		{
			if (!this.isSetup)
			{
				if (BuilderSetManager.instance == null)
				{
					return;
				}
				BuilderSetManager.instance.InitPieceDictionary();
				this.builderRenderer.BuildRenderer(BuilderSetManager.pieceList);
				this.baseGridPlanes.Clear();
				this.basePieces = new List<BuilderPiece>(1024);
				for (int i = 0; i < this.builtInPieceRoots.Count; i++)
				{
					this.builtInPieceRoots[i].SetActive(true);
					this.builtInPieceRoots[i].GetComponentsInChildren<BuilderPiece>(false, BuilderTable.tempPieces);
					this.basePieces.AddRange(BuilderTable.tempPieces);
				}
				this.allPrivatePlots = new List<BuilderPiecePrivatePlot>(20);
				this.CreateData();
				for (int j = 0; j < this.basePieces.Count; j++)
				{
					BuilderPiece builderPiece = this.basePieces[j];
					builderPiece.SetTable(this);
					builderPiece.pieceId = 5 + j;
					builderPiece.SetScale(this.pieceScale);
					builderPiece.SetupPiece(this.gridSize);
					builderPiece.OnCreate();
					builderPiece.SetState(BuilderPiece.State.OnShelf, true);
					this.baseGridPlanes.AddRange(builderPiece.gridPlanes);
					BuilderPiecePrivatePlot item;
					if (builderPiece.IsPrivatePlot() && builderPiece.TryGetPlotComponent(out item))
					{
						this.allPrivatePlots.Add(item);
					}
					this.AddPieceData(builderPiece);
				}
				this.builderPool = BuilderPool.instance;
				this.builderPool.Setup();
				this.builderPool.BuildFromPieceSets();
				if (this.isTableMutable)
				{
					for (int k = 0; k < this.conveyors.Count; k++)
					{
						this.conveyors[k].table = this;
						this.conveyors[k].shelfID = k;
						this.conveyors[k].Setup();
					}
					for (int l = 0; l < this.dispenserShelves.Count; l++)
					{
						this.dispenserShelves[l].table = this;
						this.dispenserShelves[l].shelfID = l;
						this.dispenserShelves[l].Setup();
					}
					this.conveyorManager.Setup(this);
					this.repelledPieceRoots = new HashSet<int>[this.repelHistoryLength];
					for (int m = 0; m < this.repelHistoryLength; m++)
					{
						this.repelledPieceRoots[m] = new HashSet<int>(10);
					}
					this.sharedBuildAreas = this.sharedBuildArea.GetComponents<BoxCollider>();
					BoxCollider[] array = this.sharedBuildAreas;
					for (int n = 0; n < array.Length; n++)
					{
						array[n].enabled = false;
					}
					this.sharedBuildArea.SetActive(false);
				}
				BoxCollider[] components = this.noBlocksArea.GetComponents<BoxCollider>();
				this.noBlocksAreas = new List<BuilderTable.BoxCheckParams>(components.Length);
				foreach (BoxCollider boxCollider in components)
				{
					boxCollider.enabled = true;
					BuilderTable.BoxCheckParams item2 = new BuilderTable.BoxCheckParams
					{
						center = boxCollider.transform.TransformPoint(boxCollider.center),
						halfExtents = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size) / 2f,
						rotation = boxCollider.transform.rotation
					};
					this.noBlocksAreas.Add(item2);
					boxCollider.enabled = false;
				}
				this.noBlocksArea.SetActive(false);
				this.isSetup = true;
			}
		}

		// Token: 0x06004B1A RID: 19226 RVA: 0x0016E65B File Offset: 0x0016C85B
		private void SetIsDirty(bool dirty)
		{
			if (this.isDirty != dirty)
			{
				UnityEvent<bool> onSaveDirtyChanged = this.OnSaveDirtyChanged;
				if (onSaveDirtyChanged != null)
				{
					onSaveDirtyChanged.Invoke(dirty);
				}
			}
			this.isDirty = dirty;
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x0016E680 File Offset: 0x0016C880
		private void FixedUpdate()
		{
			if (this.tableState != BuilderTable.TableState.Ready && this.tableState != BuilderTable.TableState.WaitForMasterResync)
			{
				return;
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegisterFixed)
			{
				if (builderPieceFunctional != null)
				{
					this.fixedUpdateFunctionalComponents.Add(builderPieceFunctional);
				}
			}
			foreach (IBuilderPieceFunctional item in this.funcComponentsToUnregisterFixed)
			{
				this.fixedUpdateFunctionalComponents.Remove(item);
			}
			this.funcComponentsToRegisterFixed.Clear();
			this.funcComponentsToUnregisterFixed.Clear();
			foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.fixedUpdateFunctionalComponents)
			{
				builderPieceFunctional2.FunctionalPieceFixedUpdate();
			}
		}

		// Token: 0x06004B1C RID: 19228 RVA: 0x0016E78C File Offset: 0x0016C98C
		private void Update()
		{
			this.InitIfNeeded();
			this.UpdateTableState();
			if (this.isTableMutable)
			{
				this.UpdateDroppedPieces(Time.deltaTime);
				this.repelHistoryIndex = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				int num = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				this.repelledPieceRoots[num].Clear();
			}
		}

		// Token: 0x06004B1D RID: 19229 RVA: 0x0016E7EA File Offset: 0x0016C9EA
		public void AddQueuedCommand(BuilderTable.BuilderCommand cmd)
		{
			this.queuedBuildCommands.Add(cmd);
		}

		// Token: 0x06004B1E RID: 19230 RVA: 0x0016E7F8 File Offset: 0x0016C9F8
		public void ClearQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				this.queuedBuildCommands.Clear();
			}
			this.RemoveRollBackActions();
			if (this.rollBackBufferedCommands != null)
			{
				this.rollBackBufferedCommands.Clear();
			}
			this.RemoveRollForwardCommands();
		}

		// Token: 0x06004B1F RID: 19231 RVA: 0x0016E82C File Offset: 0x0016CA2C
		public int GetNumQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				return this.queuedBuildCommands.Count;
			}
			return 0;
		}

		// Token: 0x06004B20 RID: 19232 RVA: 0x0016E843 File Offset: 0x0016CA43
		public void AddRollbackAction(BuilderAction action)
		{
			this.rollBackActions.Add(action);
		}

		// Token: 0x06004B21 RID: 19233 RVA: 0x0016E851 File Offset: 0x0016CA51
		public void RemoveRollBackActions()
		{
			this.rollBackActions.Clear();
		}

		// Token: 0x06004B22 RID: 19234 RVA: 0x0016E860 File Offset: 0x0016CA60
		public void RemoveRollBackActions(int localCommandId)
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollBackActions[i].localCommandId == localCommandId)
				{
					this.rollBackActions.RemoveAt(i);
				}
			}
		}

		// Token: 0x06004B23 RID: 19235 RVA: 0x0016E8AC File Offset: 0x0016CAAC
		public bool HasRollBackActionsForCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollBackActions.Count; i++)
			{
				if (this.rollBackActions[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004B24 RID: 19236 RVA: 0x0016E8E6 File Offset: 0x0016CAE6
		public void AddRollForwardCommand(BuilderTable.BuilderCommand command)
		{
			this.rollForwardCommands.Add(command);
		}

		// Token: 0x06004B25 RID: 19237 RVA: 0x0016E8F4 File Offset: 0x0016CAF4
		public void RemoveRollForwardCommands()
		{
			this.rollForwardCommands.Clear();
		}

		// Token: 0x06004B26 RID: 19238 RVA: 0x0016E904 File Offset: 0x0016CB04
		public void RemoveRollForwardCommands(int localCommandId)
		{
			for (int i = this.rollForwardCommands.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					this.rollForwardCommands.RemoveAt(i);
				}
			}
		}

		// Token: 0x06004B27 RID: 19239 RVA: 0x0016E950 File Offset: 0x0016CB50
		public bool HasRollForwardCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				if (this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004B28 RID: 19240 RVA: 0x0016E98C File Offset: 0x0016CB8C
		public bool ShouldRollbackBufferCommand(BuilderTable.BuilderCommand cmd)
		{
			return cmd.type != BuilderTable.BuilderCommandType.Create && cmd.type != BuilderTable.BuilderCommandType.CreateArmShelf && this.rollBackActions.Count > 0 && (cmd.player == null || !cmd.player.IsLocal || !this.HasRollForwardCommand(cmd.localCommandId));
		}

		// Token: 0x06004B29 RID: 19241 RVA: 0x0016E9E3 File Offset: 0x0016CBE3
		public void AddRollbackBufferedCommand(BuilderTable.BuilderCommand bufferedCmd)
		{
			this.rollBackBufferedCommands.Add(bufferedCmd);
		}

		// Token: 0x06004B2A RID: 19242 RVA: 0x0016E9F4 File Offset: 0x0016CBF4
		private void ExecuteRollBackActions()
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				this.ExecuteAction(this.rollBackActions[i]);
			}
			this.rollBackActions.Clear();
		}

		// Token: 0x06004B2B RID: 19243 RVA: 0x0016EA38 File Offset: 0x0016CC38
		private void ExecuteRollbackBufferedCommands()
		{
			for (int i = 0; i < this.rollBackBufferedCommands.Count; i++)
			{
				BuilderTable.BuilderCommand cmd = this.rollBackBufferedCommands[i];
				cmd.isQueued = false;
				cmd.canRollback = false;
				this.ExecuteBuildCommand(cmd);
			}
			this.rollBackBufferedCommands.Clear();
		}

		// Token: 0x06004B2C RID: 19244 RVA: 0x0016EA8C File Offset: 0x0016CC8C
		private void ExecuteRollForwardCommands()
		{
			BuilderTable.tempRollForwardCommands.Clear();
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.tempRollForwardCommands.Add(this.rollForwardCommands[i]);
			}
			this.rollForwardCommands.Clear();
			for (int j = 0; j < BuilderTable.tempRollForwardCommands.Count; j++)
			{
				BuilderTable.BuilderCommand cmd = BuilderTable.tempRollForwardCommands[j];
				cmd.isQueued = true;
				cmd.canRollback = true;
				this.ExecuteBuildCommand(cmd);
			}
			BuilderTable.tempRollForwardCommands.Clear();
		}

		// Token: 0x06004B2D RID: 19245 RVA: 0x0016EB1C File Offset: 0x0016CD1C
		private void UpdateRollForwardCommandData()
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollForwardCommands[i];
				if (builderCommand.type == BuilderTable.BuilderCommandType.Drop)
				{
					BuilderPiece piece = this.GetPiece(builderCommand.pieceId);
					if (piece != null && piece.rigidBody != null)
					{
						builderCommand.localPosition = piece.rigidBody.position;
						builderCommand.localRotation = piece.rigidBody.rotation;
						builderCommand.velocity = piece.rigidBody.velocity;
						builderCommand.angVelocity = piece.rigidBody.angularVelocity;
						this.rollForwardCommands[i] = builderCommand;
					}
				}
			}
		}

		// Token: 0x06004B2E RID: 19246 RVA: 0x0016EBD4 File Offset: 0x0016CDD4
		public bool TryRollbackAndReExecute(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				if (this.rollBackBufferedCommands.Count > 0)
				{
					this.UpdateRollForwardCommandData();
					this.ExecuteRollBackActions();
					this.ExecuteRollbackBufferedCommands();
					this.ExecuteRollForwardCommands();
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				else
				{
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004B2F RID: 19247 RVA: 0x0016EC33 File Offset: 0x0016CE33
		public void RollbackFailedCommand(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				this.UpdateRollForwardCommandData();
				this.ExecuteRollBackActions();
				this.ExecuteRollbackBufferedCommands();
				this.RemoveRollForwardCommands(-1);
				this.ExecuteRollForwardCommands();
			}
		}

		// Token: 0x06004B30 RID: 19248 RVA: 0x0016EC5D File Offset: 0x0016CE5D
		public BuilderTable.TableState GetTableState()
		{
			return this.tableState;
		}

		// Token: 0x06004B31 RID: 19249 RVA: 0x0016EC68 File Offset: 0x0016CE68
		public void SetTableState(BuilderTable.TableState newState)
		{
			this.InitIfNeeded();
			if (newState == this.tableState)
			{
				return;
			}
			BuilderTable.TableState tableState = this.tableState;
			this.tableState = newState;
			switch (this.tableState)
			{
			case BuilderTable.TableState.WaitingForInitalBuild:
				if (!this.isTableMutable && !NetworkSystem.Instance.IsMasterClient)
				{
					this.sharedBlocksMap = null;
					UnityEvent onMapCleared = this.OnMapCleared;
					if (onMapCleared == null)
					{
						return;
					}
					onMapCleared.Invoke();
					return;
				}
				break;
			case BuilderTable.TableState.ReceivingInitialBuild:
			case BuilderTable.TableState.ReceivingMasterResync:
			case BuilderTable.TableState.InitialBuild:
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.WaitForInitialBuildMaster:
				this.nextPieceId = 10000;
				if (this.isTableMutable)
				{
					this.BuildInitialTableForPlayer();
					return;
				}
				this.BuildSelectedSharedMap();
				return;
			case BuilderTable.TableState.WaitForMasterResync:
				this.ClearQueuedCommands();
				this.ResetConveyors();
				return;
			case BuilderTable.TableState.Ready:
				this.OnAvailableResourcesChange();
				if (!this.isTableMutable)
				{
					string arg = (this.sharedBlocksMap == null) ? "" : this.sharedBlocksMap.MapID;
					UnityEvent<string> onMapLoaded = this.OnMapLoaded;
					if (onMapLoaded != null)
					{
						onMapLoaded.Invoke(arg);
					}
					this.SetPendingMap(null);
					return;
				}
				break;
			case BuilderTable.TableState.BadData:
				this.ClearTable();
				this.ClearQueuedCommands();
				break;
			case BuilderTable.TableState.WaitingForSharedMapLoad:
				this.ClearTable();
				this.ClearQueuedCommands();
				this.builderNetworking.ResetSerializedTableForAllPlayers();
				return;
			default:
				return;
			}
		}

		// Token: 0x06004B32 RID: 19250 RVA: 0x0016ED94 File Offset: 0x0016CF94
		public void SetPendingMap(string mapID)
		{
			this.pendingMapID = mapID;
		}

		// Token: 0x06004B33 RID: 19251 RVA: 0x0016ED9D File Offset: 0x0016CF9D
		public string GetPendingMap()
		{
			return this.pendingMapID;
		}

		// Token: 0x06004B34 RID: 19252 RVA: 0x0016EDA5 File Offset: 0x0016CFA5
		public string GetCurrentMapID()
		{
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap = this.sharedBlocksMap;
			if (sharedBlocksMap == null)
			{
				return null;
			}
			return sharedBlocksMap.MapID;
		}

		// Token: 0x06004B35 RID: 19253 RVA: 0x0016EDB8 File Offset: 0x0016CFB8
		public void LoadSharedMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (map.MapID.IsNullOrEmpty())
				{
					GTDev.LogWarning<string>("Invalid map to load", null);
					UnityEvent<string> onMapLoadFailed = this.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("Invalid Map ID");
					return;
				}
				else
				{
					if (this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.BadData)
					{
						this.builderNetworking.RequestLoadSharedBlocksMap(map.MapID);
						return;
					}
					UnityEvent<string> onMapLoadFailed2 = this.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("WAIT FOR LOAD IN PROGRESS");
					return;
				}
			}
			else
			{
				UnityEvent<string> onMapLoadFailed3 = this.OnMapLoadFailed;
				if (onMapLoadFailed3 == null)
				{
					return;
				}
				onMapLoadFailed3.Invoke("Not In Room");
				return;
			}
		}

		// Token: 0x06004B36 RID: 19254 RVA: 0x0016EE50 File Offset: 0x0016D050
		public void SetInRoom(bool inRoom)
		{
			this.inRoom = inRoom;
			bool flag = inRoom && this.inBuilderZone;
			if (!inRoom)
			{
				this.pendingMapID = null;
				this.sharedBlocksMap = null;
				UnityEvent onMapCleared = this.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
			}
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient && this.isTableMutable)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient() && this.isTableMutable)
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x06004B37 RID: 19255 RVA: 0x0016EF24 File Offset: 0x0016D124
		public static bool IsLocalPlayerInBuilderZone()
		{
			GorillaTagger instance = GorillaTagger.Instance;
			ZoneEntity zoneEntity;
			if (instance == null)
			{
				zoneEntity = null;
			}
			else
			{
				VRRig offlineVRRig = instance.offlineVRRig;
				zoneEntity = ((offlineVRRig != null) ? offlineVRRig.zoneEntity : null);
			}
			ZoneEntity zoneEntity2 = zoneEntity;
			BuilderTable builderTable;
			return !(zoneEntity2 == null) && BuilderTable.TryGetBuilderTableForZone(zoneEntity2.currentZone, out builderTable) && builderTable.IsInBuilderZone();
		}

		// Token: 0x06004B38 RID: 19256 RVA: 0x0016EF71 File Offset: 0x0016D171
		public bool IsInBuilderZone()
		{
			return this.inBuilderZone;
		}

		// Token: 0x06004B39 RID: 19257 RVA: 0x0016EF7C File Offset: 0x0016D17C
		public void SetInBuilderZone(bool inBuilderZone)
		{
			this.inBuilderZone = inBuilderZone;
			this.ShowPieces(inBuilderZone);
			bool flag = this.inRoom && inBuilderZone;
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient())
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x06004B3A RID: 19258 RVA: 0x0016F020 File Offset: 0x0016D220
		private void ShowPieces(bool show)
		{
			if (this.builderRenderer != null)
			{
				this.builderRenderer.Show(show);
			}
			if (this.pieces == null || this.basePieces == null)
			{
				return;
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				this.pieces[i].SetDirectRenderersVisible(show);
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				this.basePieces[j].SetDirectRenderersVisible(show);
			}
		}

		// Token: 0x06004B3B RID: 19259 RVA: 0x0016F0A8 File Offset: 0x0016D2A8
		private void UpdateTableState()
		{
			switch (this.tableState)
			{
			case BuilderTable.TableState.InitialBuild:
			{
				BuilderTableNetworking.PlayerTableInitState localTableInit = this.builderNetworking.GetLocalTableInit();
				try
				{
					this.ClearTable();
					this.ClearQueuedCommands();
					byte[] array = GZipStream.UncompressBuffer(localTableInit.serializedTableState);
					localTableInit.totalSerializedBytes = array.Length;
					Array.Copy(array, 0, localTableInit.serializedTableState, 0, localTableInit.totalSerializedBytes);
					this.DeserializeTableState(localTableInit.serializedTableState, localTableInit.numSerializedBytes);
					if (this.tableState == BuilderTable.TableState.BadData)
					{
						return;
					}
					this.SetTableState(BuilderTable.TableState.ExecuteQueuedCommands);
					this.SetIsDirty(true);
					return;
				}
				catch (Exception)
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				break;
			}
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.Ready:
			{
				JobHandle jobHandle = default(JobHandle);
				if (this.isTableMutable)
				{
					this.conveyorManager.UpdateManager();
					jobHandle = this.conveyorManager.ConstructJobHandle();
					JobHandle.ScheduleBatchedJobs();
					foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
					{
						builderDispenserShelf.UpdateShelf();
					}
					foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
					{
						builderPiecePrivatePlot.UpdatePlot();
					}
					foreach (BuilderRecycler builderRecycler in this.recyclers)
					{
						builderRecycler.UpdateRecycler();
					}
					for (int i = this.shelfSliceUpdateIndex; i < this.dispenserShelves.Count; i += BuilderTable.SHELF_SLICE_BUCKETS)
					{
						this.dispenserShelves[i].UpdateShelfSliced();
					}
					this.shelfSliceUpdateIndex = (this.shelfSliceUpdateIndex + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegister)
				{
					if (builderPieceFunctional != null)
					{
						this.activeFunctionalComponents.Add(builderPieceFunctional);
					}
				}
				foreach (IBuilderPieceFunctional item in this.funcComponentsToUnregister)
				{
					this.activeFunctionalComponents.Remove(item);
				}
				this.funcComponentsToRegister.Clear();
				this.funcComponentsToUnregister.Clear();
				foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.activeFunctionalComponents)
				{
					if (builderPieceFunctional2 != null)
					{
						builderPieceFunctional2.FunctionalPieceUpdate();
					}
				}
				if (this.isTableMutable)
				{
					foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
					{
						builderResourceMeter.UpdateMeterFill();
					}
					this.CleanUpDroppedPiece();
					jobHandle.Complete();
					return;
				}
				return;
			}
			default:
				return;
			}
			for (int j = 0; j < this.queuedBuildCommands.Count; j++)
			{
				BuilderTable.BuilderCommand cmd = this.queuedBuildCommands[j];
				cmd.isQueued = true;
				this.ExecuteBuildCommand(cmd);
			}
			this.queuedBuildCommands.Clear();
			this.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x06004B3C RID: 19260 RVA: 0x0016F434 File Offset: 0x0016D634
		private void RouteNewCommand(BuilderTable.BuilderCommand cmd, bool force)
		{
			bool flag = this.ShouldExecuteCommand();
			if (force)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (flag && this.ShouldRollbackBufferCommand(cmd))
			{
				this.AddRollbackBufferedCommand(cmd);
				return;
			}
			if (flag)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (this.ShouldQueueCommand())
			{
				this.AddQueuedCommand(cmd);
				return;
			}
			this.ShouldDiscardCommand();
		}

		// Token: 0x06004B3D RID: 19261 RVA: 0x0016F48C File Offset: 0x0016D68C
		private void ExecuteBuildCommand(BuilderTable.BuilderCommand cmd)
		{
			if (!this.isTableMutable && cmd.type != BuilderTable.BuilderCommandType.FunctionalStateChange)
			{
				return;
			}
			switch (cmd.type)
			{
			case BuilderTable.BuilderCommandType.Create:
				this.ExecutePieceCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.Place:
				this.ExecutePiecePlacedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Grab:
				this.ExecutePieceGrabbedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Drop:
				this.ExecutePieceDroppedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Remove:
				break;
			case BuilderTable.BuilderCommandType.Paint:
				this.ExecutePiecePainted(cmd);
				return;
			case BuilderTable.BuilderCommandType.Recycle:
				this.ExecutePieceRecycled(cmd);
				return;
			case BuilderTable.BuilderCommandType.ClaimPlot:
				this.ExecuteClaimPlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.FreePlot:
				this.ExecuteFreePlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.CreateArmShelf:
				this.ExecuteArmShelfCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.PlayerLeftRoom:
				this.ExecutePlayerLeftRoom(cmd);
				return;
			case BuilderTable.BuilderCommandType.FunctionalStateChange:
				this.ExecuteSetFunctionalPieceState(cmd);
				return;
			case BuilderTable.BuilderCommandType.SetSelection:
				this.ExecuteSetSelection(cmd);
				return;
			case BuilderTable.BuilderCommandType.Repel:
				this.ExecutePieceRepelled(cmd);
				break;
			default:
				return;
			}
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x0016F559 File Offset: 0x0016D759
		public void ClearTable()
		{
			this.ClearTableInternal();
		}

		// Token: 0x06004B3F RID: 19263 RVA: 0x0016F564 File Offset: 0x0016D764
		private void ClearTableInternal()
		{
			BuilderTable.tempDeletePieces.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				BuilderTable.tempDeletePieces.Add(this.pieces[i]);
			}
			if (this.isTableMutable)
			{
				this.droppedPieces.Clear();
				this.droppedPieceData.Clear();
			}
			for (int j = 0; j < BuilderTable.tempDeletePieces.Count; j++)
			{
				BuilderTable.tempDeletePieces[j].ClearParentPiece(false);
				BuilderTable.tempDeletePieces[j].ClearParentHeld();
				BuilderTable.tempDeletePieces[j].SetState(BuilderPiece.State.None, false);
				this.RemovePiece(BuilderTable.tempDeletePieces[j]);
			}
			for (int k = 0; k < BuilderTable.tempDeletePieces.Count; k++)
			{
				this.builderPool.DestroyPiece(BuilderTable.tempDeletePieces[k]);
			}
			BuilderTable.tempDeletePieces.Clear();
			this.pieces.Clear();
			this.pieceIDToIndexCache.Clear();
			this.nextPieceId = 10000;
			if (this.isTableMutable)
			{
				this.conveyorManager.OnClearTable();
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					builderDispenserShelf.OnClearTable();
				}
				for (int l = 0; l < this.repelHistoryLength; l++)
				{
					this.repelledPieceRoots[l].Clear();
				}
			}
			this.funcComponentsToRegister.Clear();
			this.funcComponentsToUnregister.Clear();
			this.activeFunctionalComponents.Clear();
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				foreach (BuilderAttachGridPlane builderAttachGridPlane in builderPiece.gridPlanes)
				{
					builderAttachGridPlane.OnReturnToPool(this.builderPool);
				}
			}
			if (this.isTableMutable)
			{
				this.ClearBuiltInPlots();
				this.playerToArmShelfLeft.Clear();
				this.playerToArmShelfRight.Clear();
				if (BuilderPieceInteractor.instance != null)
				{
					BuilderPieceInteractor.instance.RemovePiecesFromHands();
				}
			}
		}

		// Token: 0x06004B40 RID: 19264 RVA: 0x0016F7D0 File Offset: 0x0016D9D0
		private void ClearBuiltInPlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.ClearPlot();
			}
			this.plotOwners.Clear();
			this.SetLocalPlayerOwnsPlot(false);
		}

		// Token: 0x06004B41 RID: 19265 RVA: 0x0016F834 File Offset: 0x0016DA34
		private void OnDeserializeUpdatePlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.RecountPlotCost();
			}
		}

		// Token: 0x06004B42 RID: 19266 RVA: 0x0016F884 File Offset: 0x0016DA84
		public void BuildPiecesOnShelves()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (this.shelves == null)
			{
				return;
			}
			for (int i = 0; i < this.shelves.Count; i++)
			{
				if (this.shelves[i] != null)
				{
					this.shelves[i].Init();
				}
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int j = 0; j < this.shelves.Count; j++)
				{
					if (this.shelves[j].HasOpenSlot())
					{
						this.shelves[j].BuildNextPiece(this);
						if (this.shelves[j].HasOpenSlot())
						{
							flag = true;
						}
					}
				}
			}
		}

		// Token: 0x06004B43 RID: 19267 RVA: 0x0016F937 File Offset: 0x0016DB37
		private void OnFinishedInitialTableBuild()
		{
			this.BuildPiecesOnShelves();
			this.SetTableState(BuilderTable.TableState.Ready);
			this.CreateArmShelvesForPlayersInBuilder();
		}

		// Token: 0x06004B44 RID: 19268 RVA: 0x0016F94C File Offset: 0x0016DB4C
		public int CreatePieceId()
		{
			int result = this.nextPieceId;
			if (this.nextPieceId == 2147483647)
			{
				this.nextPieceId = 20000;
			}
			this.nextPieceId++;
			return result;
		}

		// Token: 0x06004B45 RID: 19269 RVA: 0x0016F97C File Offset: 0x0016DB7C
		public void ResetConveyors()
		{
			if (this.isTableMutable)
			{
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					builderConveyor.ResetConveyorState();
				}
			}
		}

		// Token: 0x06004B46 RID: 19270 RVA: 0x0016F9D4 File Offset: 0x0016DBD4
		public void RequestCreateConveyorPiece(int newPieceType, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			BuilderConveyor builderConveyor = this.conveyors[shelfID];
			if (builderConveyor == null)
			{
				return;
			}
			Transform spawnTransform = builderConveyor.GetSpawnTransform();
			this.builderNetworking.CreateShelfPiece(newPieceType, spawnTransform.position, spawnTransform.rotation, materialType, BuilderPiece.State.OnConveyor, shelfID);
		}

		// Token: 0x06004B47 RID: 19271 RVA: 0x0016FA2D File Offset: 0x0016DC2D
		public void RequestCreateDispenserShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			this.builderNetworking.CreateShelfPiece(pieceType, position, rotation, materialType, BuilderPiece.State.OnShelf, shelfID);
		}

		// Token: 0x06004B48 RID: 19272 RVA: 0x0016FA70 File Offset: 0x0016DC70
		public void CreateConveyorPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID, int sendTimestamp)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			if (this.conveyors[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnConveyor,
				parentPieceId = shelfID,
				parentAttachIndex = sendTimestamp,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B49 RID: 19273 RVA: 0x0016FB18 File Offset: 0x0016DD18
		public void CreateDispenserShelfPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnShelf,
				parentPieceId = shelfID,
				isLeft = true,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B4A RID: 19274 RVA: 0x0016FBBE File Offset: 0x0016DDBE
		public void RequestShelfSelection(int shelfId, int setId, bool isConveyor)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestShelfSelection(shelfId, setId, isConveyor);
		}

		// Token: 0x06004B4B RID: 19275 RVA: 0x0016FBD8 File Offset: 0x0016DDD8
		public void VerifySetSelections()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.conveyors)
			{
				builderConveyor.VerifySetSelection();
			}
			foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
			{
				builderDispenserShelf.VerifySetSelection();
			}
		}

		// Token: 0x06004B4C RID: 19276 RVA: 0x0016FC70 File Offset: 0x0016DE70
		public bool ValidateShelfSelectionParams(int shelfId, int setId, bool isConveyor, Player player)
		{
			bool flag = shelfId >= 0 && ((isConveyor && shelfId < this.conveyors.Count) || (!isConveyor && shelfId < this.dispenserShelves.Count)) && BuilderSetManager.instance.DoesPlayerOwnPieceSet(player, setId);
			if (PhotonNetwork.IsMasterClient)
			{
				if (isConveyor)
				{
					BuilderConveyor builderConveyor = this.conveyors[shelfId];
					bool flag2 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderConveyor.transform.position, false, true, 4f);
					flag = (flag && flag2);
				}
				else
				{
					BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[shelfId];
					bool flag3 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderDispenserShelf.transform.position, false, true, 4f);
					flag = (flag && flag3);
				}
			}
			return flag;
		}

		// Token: 0x06004B4D RID: 19277 RVA: 0x0016FD2C File Offset: 0x0016DF2C
		private void SetConveyorSelection(int conveyorId, int setId)
		{
			BuilderConveyor builderConveyor = this.conveyors[conveyorId];
			if (builderConveyor == null)
			{
				return;
			}
			builderConveyor.SetSelection(setId);
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x0016FD58 File Offset: 0x0016DF58
		private void SetDispenserSelection(int conveyorId, int setId)
		{
			BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[conveyorId];
			if (builderDispenserShelf == null)
			{
				return;
			}
			builderDispenserShelf.SetSelection(setId);
		}

		// Token: 0x06004B4F RID: 19279 RVA: 0x0016FD84 File Offset: 0x0016DF84
		public void ChangeSetSelection(int shelfID, int setID, bool isConveyor)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.SetSelection,
				parentPieceId = shelfID,
				pieceType = setID,
				isLeft = isConveyor,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B50 RID: 19280 RVA: 0x0016FDD8 File Offset: 0x0016DFD8
		public void ExecuteSetSelection(BuilderTable.BuilderCommand cmd)
		{
			bool isLeft = cmd.isLeft;
			int parentPieceId = cmd.parentPieceId;
			int pieceType = cmd.pieceType;
			if (isLeft)
			{
				this.SetConveyorSelection(parentPieceId, pieceType);
				return;
			}
			this.SetDispenserSelection(parentPieceId, pieceType);
		}

		// Token: 0x06004B51 RID: 19281 RVA: 0x0016FE0C File Offset: 0x0016E00C
		public bool ValidateFunctionalPieceState(int pieceID, byte state, NetPlayer player)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			return !(piece == null) && piece.functionalPieceComponent != null && (!NetworkSystem.Instance.IsMasterClient || player.IsMasterClient || this.IsPlayerHandNearAction(player, piece.transform.position, true, false, piece.functionalPieceComponent.GetInteractionDistace())) && piece.functionalPieceComponent.IsStateValid(state);
		}

		// Token: 0x06004B52 RID: 19282 RVA: 0x0016FE7C File Offset: 0x0016E07C
		public void OnFunctionalStateRequest(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			if (piece.functionalPieceComponent == null)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			piece.functionalPieceComponent.OnStateRequest(state, player, timeStamp);
		}

		// Token: 0x06004B53 RID: 19283 RVA: 0x0016FEB8 File Offset: 0x0016E0B8
		public void SetFunctionalPieceState(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FunctionalStateChange,
				pieceId = pieceID,
				twist = state,
				player = player,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B54 RID: 19284 RVA: 0x0016FF04 File Offset: 0x0016E104
		public void ExecuteSetFunctionalPieceState(BuilderTable.BuilderCommand cmd)
		{
			BuilderPiece piece = this.GetPiece(cmd.pieceId);
			if (piece == null)
			{
				return;
			}
			piece.SetFunctionalPieceState(cmd.twist, cmd.player, cmd.serverTimeStamp);
		}

		// Token: 0x06004B55 RID: 19285 RVA: 0x0016FF40 File Offset: 0x0016E140
		public void RegisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegister.Add(component);
			}
		}

		// Token: 0x06004B56 RID: 19286 RVA: 0x0016FF51 File Offset: 0x0016E151
		public void UnregisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToUnregister.Add(component);
			}
		}

		// Token: 0x06004B57 RID: 19287 RVA: 0x0016FF62 File Offset: 0x0016E162
		public void RegisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Add(component);
			}
		}

		// Token: 0x06004B58 RID: 19288 RVA: 0x0016FF73 File Offset: 0x0016E173
		public void UnregisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Remove(component);
			}
		}

		// Token: 0x06004B59 RID: 19289 RVA: 0x000023F5 File Offset: 0x000005F5
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x06004B5A RID: 19290 RVA: 0x0016FF88 File Offset: 0x0016E188
		public void CreatePiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = state,
				player = NetPlayer.Get(player)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B5B RID: 19291 RVA: 0x0016FFF0 File Offset: 0x0016E1F0
		public void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, playFX, recyclerID);
		}

		// Token: 0x06004B5C RID: 19292 RVA: 0x0017001C File Offset: 0x0016E21C
		public void RecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Recycle,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				player = NetPlayer.Get(player),
				isLeft = playFX,
				parentPieceId = recyclerID
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B5D RID: 19293 RVA: 0x0017007B File Offset: 0x0016E27B
		private bool ShouldExecuteCommand()
		{
			return this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster;
		}

		// Token: 0x06004B5E RID: 19294 RVA: 0x00170091 File Offset: 0x0016E291
		private bool ShouldQueueCommand()
		{
			return this.tableState == BuilderTable.TableState.ReceivingInitialBuild || this.tableState == BuilderTable.TableState.ReceivingMasterResync || this.tableState == BuilderTable.TableState.InitialBuild || this.tableState == BuilderTable.TableState.ExecuteQueuedCommands;
		}

		// Token: 0x06004B5F RID: 19295 RVA: 0x001700B9 File Offset: 0x0016E2B9
		private bool ShouldDiscardCommand()
		{
			return this.tableState == BuilderTable.TableState.WaitingForInitalBuild || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster || this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x001700D8 File Offset: 0x0016E2D8
		public bool DoesChainContainPiece(BuilderPiece targetPiece, BuilderPiece firstInChain, BuilderPiece nextInChain)
		{
			return !(targetPiece == null) && !(firstInChain == null) && (targetPiece.Equals(firstInChain) || (!(nextInChain == null) && (targetPiece.Equals(nextInChain) || (!(firstInChain == nextInChain) && this.DoesChainContainPiece(targetPiece, firstInChain, nextInChain.parentPiece)))));
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x00170134 File Offset: 0x0016E334
		public bool DoesChainContainChain(BuilderPiece chainARoot, BuilderPiece chainBAttachPiece)
		{
			if (chainARoot == null || chainBAttachPiece == null)
			{
				return false;
			}
			if (this.DoesChainContainPiece(chainARoot, chainBAttachPiece, chainBAttachPiece.parentPiece))
			{
				return true;
			}
			BuilderPiece builderPiece = chainARoot.firstChildPiece;
			while (builderPiece != null)
			{
				if (this.DoesChainContainChain(builderPiece, chainBAttachPiece))
				{
					return true;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
			return false;
		}

		// Token: 0x06004B62 RID: 19298 RVA: 0x00170190 File Offset: 0x0016E390
		private bool IsPlayerHandNearAction(NetPlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 2.5f)
		{
			bool flag = true;
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				if (isLeftHand || checkBothHands)
				{
					flag = ((worldPosition - rigContainer.Rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius);
				}
				if (!isLeftHand || checkBothHands)
				{
					float sqrMagnitude = (worldPosition - rigContainer.Rig.rightHandTransform.position).sqrMagnitude;
					flag = (flag && sqrMagnitude < acceptableRadius * acceptableRadius);
				}
			}
			return flag;
		}

		// Token: 0x06004B63 RID: 19299 RVA: 0x00170224 File Offset: 0x0016E424
		public bool ValidatePlacePieceParams(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return false;
			}
			if (piece.heldByPlayerActorNumber != placedByPlayer.ActorNumber)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece2.isBuiltIntoTable)
			{
				return false;
			}
			if (twist > 3)
			{
				return false;
			}
			BuilderPiece piece3 = this.GetPiece(parentPieceId);
			if (!(piece3 != null))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(placedByPlayer.ActorNumber, piece2, piece3))
			{
				return false;
			}
			if (this.DoesChainContainChain(piece2, piece3))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			if (piece3 != null && (parentAttachIndex < 0 || parentAttachIndex >= piece3.gridPlanes.Count))
			{
				return false;
			}
			if (piece3 != null)
			{
				bool flag = (long)(twist % 2) == 1L;
				BuilderAttachGridPlane builderAttachGridPlane = piece2.gridPlanes[attachIndex];
				int num = flag ? builderAttachGridPlane.length : builderAttachGridPlane.width;
				int num2 = flag ? builderAttachGridPlane.width : builderAttachGridPlane.length;
				BuilderAttachGridPlane builderAttachGridPlane2 = piece3.gridPlanes[parentAttachIndex];
				int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
				int num4 = num3 - (builderAttachGridPlane2.width - 1);
				if ((int)bumpOffsetX < num4 - num || (int)bumpOffsetX > num3 + num)
				{
					return false;
				}
				int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
				int num6 = num5 - (builderAttachGridPlane2.length - 1);
				if ((int)bumpOffsetZ < num6 - num2 || (int)bumpOffsetZ > num5 + num2)
				{
					return false;
				}
			}
			if (placedByPlayer == null)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient && piece3 != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				Vector3 vector2;
				Quaternion rotation;
				piece2.BumpTwistToPositionRotation(twist, bumpOffsetX, bumpOffsetZ, attachIndex, piece3.gridPlanes[parentAttachIndex], out vector, out quaternion, out vector2, out rotation);
				Vector3 point = piece2.transform.InverseTransformPoint(piece.transform.position);
				Vector3 worldPosition = vector2 + rotation * point;
				if (!this.IsPlayerHandNearAction(placedByPlayer, worldPosition, piece.heldInLeftHand, false, 2.5f))
				{
					return false;
				}
				if (!this.ValidatePieceWorldTransform(vector2, rotation))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004B64 RID: 19300 RVA: 0x00170438 File Offset: 0x0016E638
		public bool ValidatePlacePieceState(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			return !(piece2 == null) && !(this.GetPiece(parentPieceId) == null) && placedByPlayer != null && !piece2.GetRootPiece() != piece;
		}

		// Token: 0x06004B65 RID: 19301 RVA: 0x0017049C File Offset: 0x0016E69C
		public void ExecutePieceCreated(BuilderTable.BuilderCommand cmd)
		{
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateCreatePieceParams(cmd.pieceType, cmd.pieceId, cmd.state, cmd.materialType))
			{
				return;
			}
			BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, cmd.localPosition, cmd.localRotation, cmd.state, cmd.materialType, 0, this);
			if (!(builderPiece != null) || cmd.state != BuilderPiece.State.OnConveyor)
			{
				if (builderPiece != null && cmd.isLeft && cmd.state == BuilderPiece.State.OnShelf)
				{
					if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.dispenserShelves.Count)
					{
						return;
					}
					builderPiece.shelfOwner = cmd.parentPieceId;
					this.dispenserShelves[builderPiece.shelfOwner].OnShelfPieceCreated(builderPiece, true);
				}
				return;
			}
			if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.conveyors.Count)
			{
				return;
			}
			builderPiece.shelfOwner = cmd.parentPieceId;
			BuilderConveyor builderConveyor = this.conveyors[builderPiece.shelfOwner];
			int parentAttachIndex = cmd.parentAttachIndex;
			float timeOffset = 0f;
			if (PhotonNetwork.ServerTimestamp > parentAttachIndex)
			{
				timeOffset = (PhotonNetwork.ServerTimestamp - parentAttachIndex) / 1000f;
			}
			builderConveyor.OnShelfPieceCreated(builderPiece, timeOffset);
		}

		// Token: 0x06004B66 RID: 19302 RVA: 0x001705E3 File Offset: 0x0016E7E3
		public void ExecutePieceRecycled(BuilderTable.BuilderCommand cmd)
		{
			this.RecyclePieceInternal(cmd.pieceId, false, cmd.isLeft, cmd.parentPieceId);
		}

		// Token: 0x06004B67 RID: 19303 RVA: 0x001705FE File Offset: 0x0016E7FE
		private bool ValidateCreatePieceParams(int newPieceType, int newPieceId, BuilderPiece.State state, int materialType)
		{
			return !(this.GetPiecePrefab(newPieceType) == null) && !(this.GetPiece(newPieceId) != null);
		}

		// Token: 0x06004B68 RID: 19304 RVA: 0x00170624 File Offset: 0x0016E824
		private bool ValidateDeserializedRootPieceState(int pieceId, BuilderPiece.State state, int shelfOwner, int heldByActor, Vector3 localPosition, Quaternion localRotation)
		{
			switch (state)
			{
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. held piece in immutable table {0}", pieceId), null);
				}
				else if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			case BuilderPiece.State.Dropped:
				if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. dropped piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				if (shelfOwner == -1 && !this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				break;
			case BuilderPiece.State.OnConveyor:
				if (shelfOwner == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. OnConveyor piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.AttachedToArm:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. AttachedToArm piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		// Token: 0x06004B69 RID: 19305 RVA: 0x0017073C File Offset: 0x0016E93C
		private bool ValidateDeserializedChildPieceState(int pieceId, BuilderPiece.State state)
		{
			switch (state)
			{
			case BuilderPiece.State.AttachedAndPlaced:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				return true;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
			case BuilderPiece.State.AttachedToArm:
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. Invalid state {0} of child piece {1} in Immutable table", state, pieceId), null);
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x001707A0 File Offset: 0x0016E9A0
		public bool ValidatePieceWorldTransform(Vector3 position, Quaternion rotation)
		{
			float num = 10000f;
			return position.IsValid(num) && rotation.IsValid() && (this.roomCenter.position - position).sqrMagnitude <= this.acceptableSqrDistFromCenter;
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x001707F0 File Offset: 0x0016E9F0
		private BuilderPiece CreatePieceInternal(int newPieceType, int newPieceId, Vector3 position, Quaternion rotation, BuilderPiece.State state, int materialType, int activateTimeStamp, BuilderTable table)
		{
			if (this.GetPiecePrefab(newPieceType) == null)
			{
				return null;
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				this.nextPieceId = newPieceId + 1;
			}
			BuilderPiece builderPiece = this.builderPool.CreatePiece(newPieceType, false);
			builderPiece.SetScale(table.pieceScale);
			builderPiece.transform.SetPositionAndRotation(position, rotation);
			builderPiece.pieceType = newPieceType;
			builderPiece.pieceId = newPieceId;
			builderPiece.SetTable(table);
			builderPiece.gameObject.SetActive(true);
			builderPiece.SetupPiece(this.gridSize);
			builderPiece.OnCreate();
			builderPiece.activatedTimeStamp = ((state == BuilderPiece.State.AttachedAndPlaced) ? activateTimeStamp : 0);
			builderPiece.SetMaterial(materialType, true);
			builderPiece.SetState(state, true);
			this.AddPiece(builderPiece);
			return builderPiece;
		}

		// Token: 0x06004B6C RID: 19308 RVA: 0x001708A4 File Offset: 0x0016EAA4
		private void RecyclePieceInternal(int pieceId, bool ignoreHaptics, bool playFX, int recyclerId)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (playFX)
			{
				try
				{
					piece.PlayRecycleFx();
				}
				catch (Exception)
				{
				}
			}
			if (!ignoreHaptics)
			{
				BuilderPiece rootPiece = piece.GetRootPiece();
				if (rootPiece != null && rootPiece.IsHeldLocal())
				{
					GorillaTagger.Instance.StartVibration(piece.IsHeldInLeftHand(), GorillaTagger.Instance.tapHapticStrength, this.pushAndEaseParams.snapDelayTime * 2f);
				}
			}
			BuilderPiece builderPiece = piece.firstChildPiece;
			while (builderPiece != null)
			{
				int pieceId2 = builderPiece.pieceId;
				builderPiece = builderPiece.nextSiblingPiece;
				this.RecyclePieceInternal(pieceId2, true, playFX, recyclerId);
			}
			if (this.isTableMutable && recyclerId >= 0 && recyclerId < this.recyclers.Count)
			{
				this.recyclers[recyclerId].OnRecycleRequestedAtRecycler(piece);
			}
			if (piece.state == BuilderPiece.State.OnConveyor && piece.shelfOwner >= 0 && piece.shelfOwner < this.conveyors.Count)
			{
				this.conveyors[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			else if ((piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed) && piece.shelfOwner >= 0 && piece.shelfOwner < this.dispenserShelves.Count)
			{
				this.dispenserShelves[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			if (piece.isArmShelf && this.isTableMutable)
			{
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				int num;
				if (piece.heldInLeftHand && this.playerToArmShelfLeft.TryGetValue(piece.heldByPlayerActorNumber, out num) && num == piece.pieceId)
				{
					this.playerToArmShelfLeft.Remove(piece.heldByPlayerActorNumber);
				}
				int num2;
				if (!piece.heldInLeftHand && this.playerToArmShelfRight.TryGetValue(piece.heldByPlayerActorNumber, out num2) && num2 == piece.pieceId)
				{
					this.playerToArmShelfRight.Remove(piece.heldByPlayerActorNumber);
				}
			}
			else if (PhotonNetwork.LocalPlayer.ActorNumber == piece.heldByPlayerActorNumber)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			int pieceId3 = piece.pieceId;
			piece.ClearParentPiece(false);
			piece.ClearParentHeld();
			piece.SetState(BuilderPiece.State.None, false);
			this.RemovePiece(piece);
			this.builderPool.DestroyPiece(piece);
		}

		// Token: 0x06004B6D RID: 19309 RVA: 0x00170B00 File Offset: 0x0016ED00
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			return BuilderSetManager.instance.GetPiecePrefab(pieceType);
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x00170B10 File Offset: 0x0016ED10
		private bool ValidateAttachPieceParams(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int piecePlacement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece2 == null)
			{
				return false;
			}
			if ((piecePlacement & 262143) != piecePlacement)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (this.DoesChainContainChain(piece, piece2))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece.gridPlanes.Count)
			{
				return false;
			}
			if (parentAttachIndex < 0 || parentAttachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(piecePlacement, out b, out b2, out b3);
			bool flag = (long)(b % 2) == 1L;
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[attachIndex];
			int num = flag ? builderAttachGridPlane.length : builderAttachGridPlane.width;
			int num2 = flag ? builderAttachGridPlane.width : builderAttachGridPlane.length;
			BuilderAttachGridPlane builderAttachGridPlane2 = piece2.gridPlanes[parentAttachIndex];
			int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
			int num4 = num3 - (builderAttachGridPlane2.width - 1);
			if ((int)b2 < num4 - num || (int)b2 > num3 + num)
			{
				return false;
			}
			int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
			int num6 = num5 - (builderAttachGridPlane2.length - 1);
			return (int)b3 >= num6 - num2 && (int)b3 <= num5 + num2;
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x00170C5C File Offset: 0x0016EE5C
		private void AttachPieceInternal(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int placement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece == null)
			{
				return;
			}
			byte b;
			sbyte xOffset;
			sbyte zOffset;
			BuilderTable.UnpackPiecePlacement(placement, out b, out xOffset, out zOffset);
			Vector3 zero = Vector3.zero;
			Quaternion localRotation;
			if (piece2 != null && parentAttachIndex >= 0 && parentAttachIndex < piece2.gridPlanes.Count)
			{
				Vector3 vector;
				Quaternion quaternion;
				piece.BumpTwistToPositionRotation(b, xOffset, zOffset, attachIndex, piece2.gridPlanes[parentAttachIndex], out zero, out localRotation, out vector, out quaternion);
			}
			else
			{
				localRotation = Quaternion.Euler(0f, (float)b * 90f, 0f);
			}
			piece.SetParentPiece(attachIndex, piece2, parentAttachIndex);
			piece.transform.SetLocalPositionAndRotation(zero, localRotation);
		}

		// Token: 0x06004B70 RID: 19312 RVA: 0x00170D08 File Offset: 0x0016EF08
		private void AttachPieceToActorInternal(int pieceId, int actorNumber, bool isLeftHand)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				return;
			}
			VRRig rig = rigContainer.Rig;
			BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
			Transform parentHeld = isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
			if (piece.isArmShelf)
			{
				if (!this.isTableMutable)
				{
					return;
				}
				parentHeld = (isLeftHand ? rig.builderArmShelfLeft.pieceAnchor : rig.builderArmShelfRight.pieceAnchor);
				if (isLeftHand)
				{
					rig.builderArmShelfLeft.piece = piece;
					piece.armShelf = rig.builderArmShelfLeft;
					int num;
					if (this.playerToArmShelfLeft.TryGetValue(actorNumber, out num) && num != pieceId)
					{
						BuilderPiece piece2 = this.GetPiece(num);
						if (piece2 != null && piece2.isArmShelf)
						{
							piece2.ClearParentHeld();
							this.playerToArmShelfLeft.Remove(actorNumber);
						}
					}
					this.playerToArmShelfLeft.TryAdd(actorNumber, pieceId);
				}
				else
				{
					rig.builderArmShelfRight.piece = piece;
					piece.armShelf = rig.builderArmShelfRight;
					int num2;
					if (this.playerToArmShelfRight.TryGetValue(actorNumber, out num2) && num2 != pieceId)
					{
						BuilderPiece piece3 = this.GetPiece(num2);
						if (piece3 != null && piece3.isArmShelf)
						{
							piece3.ClearParentHeld();
							this.playerToArmShelfRight.Remove(actorNumber);
						}
					}
					this.playerToArmShelfRight.TryAdd(actorNumber, pieceId);
				}
			}
			Vector3 localPosition = piece.transform.localPosition;
			Quaternion localRotation = piece.transform.localRotation;
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.SetParentHeld(parentHeld, actorNumber, isLeftHand);
			piece.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			BuilderPiece.State newState = player.IsLocal ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed;
			if (piece.isArmShelf)
			{
				newState = BuilderPiece.State.AttachedToArm;
				piece.transform.localScale = Vector3.one;
			}
			piece.SetState(newState, false);
			if (!player.IsLocal)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			if (player.IsLocal && !piece.isArmShelf)
			{
				BuilderPieceInteractor.instance.AddPieceToHeld(piece, isLeftHand, localPosition, localRotation);
			}
		}

		// Token: 0x06004B71 RID: 19313 RVA: 0x00170F20 File Offset: 0x0016F120
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
		}

		// Token: 0x06004B72 RID: 19314 RVA: 0x00170F50 File Offset: 0x0016F150
		public void PlacePiece(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			this.PiecePlacedInternal(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer, timeStamp, force);
		}

		// Token: 0x06004B73 RID: 19315 RVA: 0x00170F78 File Offset: 0x0016F178
		public void PiecePlacedInternal(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			if (!force && placedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Place,
				pieceId = pieceId,
				bumpOffsetX = bumpOffsetX,
				bumpOffsetZ = bumpOffsetZ,
				twist = twist,
				attachPieceId = attachPieceId,
				parentPieceId = parentPieceId,
				attachIndex = attachIndex,
				parentAttachIndex = parentAttachIndex,
				player = placedByPlayer,
				canRollback = force,
				localCommandId = localCommandId,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x00171030 File Offset: 0x0016F230
		public void ExecutePiecePlacedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int attachPieceId = cmd.attachPieceId;
			int parentPieceId = cmd.parentPieceId;
			int parentAttachIndex = cmd.parentAttachIndex;
			int attachIndex = cmd.attachIndex;
			NetPlayer player = cmd.player;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			byte twist = cmd.twist;
			sbyte bumpOffsetX = cmd.bumpOffsetX;
			sbyte bumpOffsetZ = cmd.bumpOffsetZ;
			if ((player == null || !player.IsLocal) && !this.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return;
			}
			BuilderAction action = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction action2 = BuilderActions.CreateMakeRoot(localCommandId, attachPieceId);
			BuilderAction action3 = BuilderActions.CreateAttachToPiece(localCommandId, attachPieceId, cmd.parentPieceId, cmd.attachIndex, cmd.parentAttachIndex, bumpOffsetX, bumpOffsetZ, twist, actorNumber, cmd.serverTimeStamp);
			if (cmd.canRollback)
			{
				BuilderAction action4 = BuilderActions.CreateDetachFromPiece(localCommandId, attachPieceId, actorNumber);
				BuilderAction action5 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderAction action6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(action6);
				this.AddRollbackAction(action5);
				this.AddRollbackAction(action4);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
			this.ExecuteAction(action3);
			if (!cmd.isQueued)
			{
				piece2.PlayPlacementFx();
			}
		}

		// Token: 0x06004B75 RID: 19317 RVA: 0x00171194 File Offset: 0x0016F394
		public bool ValidateGrabPieceParams(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
		{
			float num = 10000f;
			if (!localPosition.IsValid(num) || !localRotation.IsValid())
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (grabbedByPlayer == null)
			{
				return false;
			}
			if (!piece.CanPlayerGrabPiece(grabbedByPlayer.ActorNumber, piece.transform.position))
			{
				return false;
			}
			if (localPosition.sqrMagnitude > 6400f)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				Vector3 position = piece.transform.position;
				if (!this.IsPlayerHandNearAction(grabbedByPlayer, position, isLeftHand, false, 2.5f))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004B76 RID: 19318 RVA: 0x00171234 File Offset: 0x0016F434
		public bool ValidateGrabPieceState(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, Player grabbedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			return !(piece == null) && piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.None;
		}

		// Token: 0x06004B77 RID: 19319 RVA: 0x0017126C File Offset: 0x0016F46C
		public bool IsLocationWithinSharedBuildArea(Vector3 worldPosition)
		{
			Vector3 vector = this.sharedBuildArea.transform.InverseTransformPoint(worldPosition);
			foreach (BoxCollider boxCollider in this.sharedBuildAreas)
			{
				Vector3 vector2 = boxCollider.center + boxCollider.size / 2f;
				Vector3 vector3 = boxCollider.center - boxCollider.size / 2f;
				if (vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004B78 RID: 19320 RVA: 0x0017134C File Offset: 0x0016F54C
		private bool NoBlocksCheck()
		{
			foreach (BuilderTable.BoxCheckParams boxCheckParams in this.noBlocksAreas)
			{
				DebugUtil.DrawBox(boxCheckParams.center, boxCheckParams.rotation, boxCheckParams.halfExtents * 2f, Color.magenta, true, DebugUtil.Style.Wireframe);
				int num = 0;
				num |= 1 << BuilderTable.placedLayer;
				int num2 = Physics.OverlapBoxNonAlloc(boxCheckParams.center, boxCheckParams.halfExtents, this.noBlocksCheckResults, boxCheckParams.rotation, num);
				for (int i = 0; i < num2; i++)
				{
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.noBlocksCheckResults[i]);
					if (builderPieceFromCollider != null && builderPieceFromCollider.GetTable() == this && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable)
					{
						GTDev.LogError<string>(string.Format("Builder Table found piece {0} {1} in NO BLOCK AREA {2}", builderPieceFromCollider.pieceId, builderPieceFromCollider.displayName, builderPieceFromCollider.transform.position), null);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06004B79 RID: 19321 RVA: 0x0017147C File Offset: 0x0016F67C
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
		}

		// Token: 0x06004B7A RID: 19322 RVA: 0x00171498 File Offset: 0x0016F698
		public void GrabPiece(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			this.PieceGrabbedInternal(localCommandId, pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer, force);
		}

		// Token: 0x06004B7B RID: 19323 RVA: 0x001714AC File Offset: 0x0016F6AC
		public void PieceGrabbedInternal(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			if (!force && grabbedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Grab,
				pieceId = pieceId,
				attachPieceId = -1,
				isLeft = isLeftHand,
				localPosition = localPosition,
				localRotation = localRotation,
				player = grabbedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06004B7C RID: 19324 RVA: 0x00171540 File Offset: 0x0016F740
		public void ExecutePieceGrabbedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			bool isLeft = cmd.isLeft;
			NetPlayer player = cmd.player;
			Vector3 localPosition = cmd.localPosition;
			Quaternion localRotation = cmd.localRotation;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((player == null || !player.Equals(NetworkSystem.Instance.LocalPlayer)) && !this.ValidateGrabPieceParams(pieceId, isLeft, localPosition, localRotation, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			bool flag = PhotonNetwork.CurrentRoom.GetPlayer(piece.heldByPlayerActorNumber, false) != null;
			bool flag2 = BuilderPiece.IsDroppedState(piece.state);
			bool flag3 = piece.state == BuilderPiece.State.OnConveyor || piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed;
			BuilderAction action = BuilderActions.CreateAttachToPlayer(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, actorNumber, cmd.isLeft);
			BuilderAction action2 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			if (flag)
			{
				BuilderAction action3 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				if (cmd.canRollback)
				{
					BuilderAction action4 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
					this.AddRollbackAction(action4);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action3);
				this.ExecuteAction(action);
				return;
			}
			if (flag3)
			{
				BuilderAction action5;
				if (piece.state == BuilderPiece.State.OnConveyor)
				{
					int serverTimestamp = PhotonNetwork.ServerTimestamp;
					float splineProgressForPiece = this.conveyorManager.GetSplineProgressForPiece(piece);
					action5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, true, serverTimestamp, splineProgressForPiece);
				}
				else
				{
					if (piece.state == BuilderPiece.State.Displayed)
					{
						int actorNumber2 = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					}
					action5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, false, 0, 0f);
				}
				BuilderAction action6 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece = piece.GetRootPiece();
				BuilderAction action7 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action5);
					this.AddRollbackAction(action7);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action6);
				this.ExecuteAction(action);
				return;
			}
			if (flag2)
			{
				BuilderAction action8 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece2 = piece.GetRootPiece();
				BuilderAction action9 = BuilderActions.CreateDropPieceRollback(localCommandId, rootPiece2, actorNumber);
				BuilderAction action10 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece2.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action9);
					this.AddRollbackAction(action10);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action8);
				this.ExecuteAction(action);
				return;
			}
			if (piece.parentPiece != null)
			{
				BuilderAction action11 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction action12 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action12);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action11);
				this.ExecuteAction(action);
			}
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x00171814 File Offset: 0x0016FA14
		public bool ValidateDropPieceParams(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer)
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3))
					{
						BuilderPiece piece = this.GetPiece(pieceId);
						if (piece == null)
						{
							return false;
						}
						if (piece.isBuiltIntoTable)
						{
							return false;
						}
						if (droppedByPlayer == null)
						{
							return false;
						}
						if (velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
						{
							return false;
						}
						if (angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
						{
							return false;
						}
						if ((this.roomCenter.position - position).sqrMagnitude > this.acceptableSqrDistFromCenter)
						{
							return false;
						}
						if (piece.state == BuilderPiece.State.AttachedToArm)
						{
							if (piece.parentPiece == null)
							{
								return false;
							}
							if (piece.parentPiece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
							{
								return false;
							}
						}
						else if (piece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
						{
							return false;
						}
						return !PhotonNetwork.IsMasterClient || this.IsPlayerHandNearAction(droppedByPlayer, position, piece.heldInLeftHand, false, 2.5f);
					}
				}
			}
			return false;
		}

		// Token: 0x06004B7E RID: 19326 RVA: 0x00171934 File Offset: 0x0016FB34
		public bool ValidateDropPieceState(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			bool flag = piece.state == BuilderPiece.State.AttachedToArm;
			return (flag || piece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber) && (!flag || piece.parentPiece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber);
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x0017198A File Offset: 0x0016FB8A
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x001719A8 File Offset: 0x0016FBA8
		public void DropPiece(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			this.PieceDroppedInternal(localCommandId, pieceId, position, rotation, velocity, angVelocity, droppedByPlayer, force);
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x001719C8 File Offset: 0x0016FBC8
		public void PieceDroppedInternal(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			if (!force && droppedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Drop,
				pieceId = pieceId,
				parentPieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				velocity = velocity,
				angVelocity = angVelocity,
				player = droppedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x00171A64 File Offset: 0x0016FC64
		public void ExecutePieceDroppedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateDropPieceParams(pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, cmd.player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm)
			{
				BuilderPiece parentPiece = piece.parentPiece;
				BuilderAction action = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction action2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
				if (cmd.canRollback)
				{
					BuilderAction action3 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
					this.AddRollbackAction(action3);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action);
				this.ExecuteAction(action2);
				return;
			}
			BuilderAction action4 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction action5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
			if (cmd.canRollback)
			{
				BuilderAction action6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(action6);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(action4);
			this.ExecuteAction(action5);
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x00171BA8 File Offset: 0x0016FDA8
		public void ExecutePieceRepelled(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			int attachPieceId = cmd.attachPieceId;
			BuilderPiece piece = this.GetPiece(pieceId);
			Vector3 velocity = cmd.velocity;
			if (piece == null)
			{
				return;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return;
			}
			if (piece.state != BuilderPiece.State.Grabbed && piece.state != BuilderPiece.State.GrabbedLocal && piece.state != BuilderPiece.State.Dropped && piece.state != BuilderPiece.State.AttachedToDropped && piece.state != BuilderPiece.State.AttachedToArm)
			{
				return;
			}
			if (attachPieceId >= 0 && attachPieceId < this.dropZones.Count)
			{
				BuilderDropZone builderDropZone = this.dropZones[attachPieceId];
				builderDropZone.PlayEffect();
				if (builderDropZone.overrideDirection)
				{
					velocity = builderDropZone.GetRepelDirectionWorld() * BuilderTable.DROP_ZONE_REPEL;
				}
			}
			if (piece.heldByPlayerActorNumber >= 0)
			{
				BuilderAction action = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction action2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
				this.ExecuteAction(action);
				this.ExecuteAction(action2);
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm && piece.parentPiece != null)
			{
				BuilderAction action3 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction action4 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
				this.ExecuteAction(action3);
				this.ExecuteAction(action4);
				return;
			}
			BuilderAction action5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
			this.ExecuteAction(action5);
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x00171D48 File Offset: 0x0016FF48
		private void CleanUpDroppedPiece()
		{
			if (!PhotonNetwork.IsMasterClient || this.droppedPieces.Count <= BuilderTable.DROPPED_PIECE_LIMIT)
			{
				return;
			}
			BuilderPiece builderPiece = this.FindFirstSleepingPiece();
			if (builderPiece != null && builderPiece.state == BuilderPiece.State.Dropped)
			{
				this.RequestRecyclePiece(builderPiece, false, -1);
				return;
			}
			Debug.LogErrorFormat("Piece {0} in Dropped List is {1}", new object[]
			{
				builderPiece.pieceId,
				builderPiece.state
			});
		}

		// Token: 0x06004B85 RID: 19333 RVA: 0x00171DC0 File Offset: 0x0016FFC0
		public void FreezeDroppedPiece(BuilderPiece piece)
		{
			int num = this.droppedPieces.IndexOf(piece);
			if (num >= 0)
			{
				BuilderTable.DroppedPieceData value = this.droppedPieceData[num];
				value.droppedState = BuilderTable.DroppedPieceState.Frozen;
				value.speedThreshCrossedTime = 0f;
				this.droppedPieceData[num] = value;
				if (piece.rigidBody != null)
				{
					piece.SetKinematic(true, false);
				}
				piece.forcedFrozen = true;
			}
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x00171E2C File Offset: 0x0017002C
		public void AddPieceToDropList(BuilderPiece piece)
		{
			this.droppedPieces.Add(piece);
			this.droppedPieceData.Add(new BuilderTable.DroppedPieceData
			{
				speedThreshCrossedTime = 0f,
				droppedState = BuilderTable.DroppedPieceState.Light,
				filteredSpeed = 0f
			});
		}

		// Token: 0x06004B87 RID: 19335 RVA: 0x00171E7C File Offset: 0x0017007C
		private BuilderPiece FindFirstSleepingPiece()
		{
			if (this.droppedPieces.Count < 1)
			{
				return null;
			}
			BuilderPiece builderPiece = this.droppedPieces[0];
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieces[i].rigidBody != null && this.droppedPieces[i].rigidBody.IsSleeping())
				{
					BuilderPiece result = this.droppedPieces[i];
					this.droppedPieces.RemoveAt(i);
					this.droppedPieceData.RemoveAt(i);
					return result;
				}
			}
			BuilderPiece result2 = this.droppedPieces[0];
			this.droppedPieces.RemoveAt(0);
			this.droppedPieceData.RemoveAt(0);
			return result2;
		}

		// Token: 0x06004B88 RID: 19336 RVA: 0x00171F36 File Offset: 0x00170136
		public void RemovePieceFromDropList(BuilderPiece piece)
		{
			if (piece.state == BuilderPiece.State.Dropped)
			{
				this.droppedPieces.Remove(piece);
			}
		}

		// Token: 0x06004B89 RID: 19337 RVA: 0x00171F50 File Offset: 0x00170150
		private void UpdateDroppedPieces(float dt)
		{
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieceData[i].droppedState == BuilderTable.DroppedPieceState.Frozen && this.droppedPieces[i].state == BuilderPiece.State.Dropped)
				{
					BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[i];
					droppedPieceData.speedThreshCrossedTime += dt;
					if (droppedPieceData.speedThreshCrossedTime > 60f)
					{
						this.droppedPieces[i].forcedFrozen = false;
						this.droppedPieces[i].ClearCollisionHistory();
						this.droppedPieces[i].SetKinematic(false, true);
						droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Light;
						droppedPieceData.speedThreshCrossedTime = 0f;
					}
					this.droppedPieceData[i] = droppedPieceData;
				}
				else
				{
					Rigidbody rigidBody = this.droppedPieces[i].rigidBody;
					if (rigidBody != null)
					{
						BuilderTable.DroppedPieceData droppedPieceData2 = this.droppedPieceData[i];
						float magnitude = rigidBody.velocity.magnitude;
						droppedPieceData2.filteredSpeed = droppedPieceData2.filteredSpeed * 0.95f + magnitude * 0.05f;
						switch (droppedPieceData2.droppedState)
						{
						case BuilderTable.DroppedPieceState.Light:
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed < 0.05f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0f)
							{
								rigidBody.mass = 10000f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Heavy;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						case BuilderTable.DroppedPieceState.Heavy:
							droppedPieceData2.speedThreshCrossedTime += dt;
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed > 0.075f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0.5f)
							{
								rigidBody.mass = 1f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Light;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						}
						this.droppedPieceData[i] = droppedPieceData2;
					}
				}
			}
		}

		// Token: 0x06004B8A RID: 19338 RVA: 0x00172169 File Offset: 0x00170369
		private void SetLocalPlayerOwnsPlot(bool ownsPlot)
		{
			this.doesLocalPlayerOwnPlot = ownsPlot;
			UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
			if (onLocalPlayerClaimedPlot == null)
			{
				return;
			}
			onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
		}

		// Token: 0x06004B8B RID: 19339 RVA: 0x00172188 File Offset: 0x00170388
		public void PlotClaimed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.ClaimPlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B8C RID: 19340 RVA: 0x001721C4 File Offset: 0x001703C4
		public void ExecuteClaimPlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (pieceId == -1)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (this.plotOwners.TryAdd(player.ActorNumber, pieceId) && piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				builderPiecePrivatePlot.ClaimPlotForPlayerNumber(player.ActorNumber);
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(true);
				}
			}
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x00172248 File Offset: 0x00170448
		public void PlayerLeftRoom(int playerActorNumber)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.PlayerLeftRoom,
				pieceId = playerActorNumber,
				player = null
			};
			bool force = this.tableState == BuilderTable.TableState.WaitForMasterResync;
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06004B8E RID: 19342 RVA: 0x0017228C File Offset: 0x0017048C
		public void ExecutePlayerLeftRoom(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			int num = (player != null) ? player.ActorNumber : cmd.pieceId;
			this.FreePlotInternal(-1, num);
			int pieceId;
			if (this.playerToArmShelfLeft.TryGetValue(num, out pieceId))
			{
				this.RecyclePieceInternal(pieceId, true, false, -1);
			}
			this.playerToArmShelfLeft.Remove(num);
			int pieceId2;
			if (this.playerToArmShelfRight.TryGetValue(num, out pieceId2))
			{
				this.RecyclePieceInternal(pieceId2, true, false, -1);
			}
			this.playerToArmShelfRight.Remove(num);
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x00172308 File Offset: 0x00170508
		public void PlotFreed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FreePlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x00172344 File Offset: 0x00170544
		public void ExecuteFreePlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			this.FreePlotInternal(pieceId, player.ActorNumber);
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x00172370 File Offset: 0x00170570
		private void FreePlotInternal(int plotPieceId, int requestingPlayer)
		{
			if (plotPieceId == -1 && !this.plotOwners.TryGetValue(requestingPlayer, out plotPieceId))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(plotPieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				int ownerActorNumber = builderPiecePrivatePlot.GetOwnerActorNumber();
				this.plotOwners.Remove(ownerActorNumber);
				builderPiecePrivatePlot.FreePlot();
				if (ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(false);
				}
			}
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x001723E4 File Offset: 0x001705E4
		public bool DoesPlayerOwnPlot(int actorNum)
		{
			return this.plotOwners.ContainsKey(actorNum);
		}

		// Token: 0x06004B93 RID: 19347 RVA: 0x001723F2 File Offset: 0x001705F2
		public void RequestPaintPiece(int pieceId, int materialType)
		{
			this.builderNetworking.RequestPaintPiece(pieceId, materialType);
		}

		// Token: 0x06004B94 RID: 19348 RVA: 0x00172401 File Offset: 0x00170601
		public void PaintPiece(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			this.PaintPieceInternal(pieceId, materialType, paintingPlayer, force);
		}

		// Token: 0x06004B95 RID: 19349 RVA: 0x00172410 File Offset: 0x00170610
		private void PaintPieceInternal(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			if (!force && paintingPlayer == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Paint,
				pieceId = pieceId,
				materialType = materialType,
				player = NetPlayer.Get(paintingPlayer)
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06004B96 RID: 19350 RVA: 0x00172464 File Offset: 0x00170664
		public void ExecutePiecePainted(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int materialType = cmd.materialType;
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece != null && !piece.isBuiltIntoTable)
			{
				piece.SetMaterial(materialType, false);
			}
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x001724A0 File Offset: 0x001706A0
		public void CreateArmShelvesForPlayersInBuilder()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				foreach (Player player in this.builderNetworking.armShelfRequests)
				{
					if (player != null)
					{
						this.builderNetworking.RequestCreateArmShelfForPlayer(player);
					}
				}
				this.builderNetworking.armShelfRequests.Clear();
			}
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x00172528 File Offset: 0x00170728
		public void RemoveArmShelfForPlayer(Player player)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				this.builderNetworking.armShelfRequests.Remove(player);
				return;
			}
			int pieceId;
			if (this.playerToArmShelfLeft.TryGetValue(player.ActorNumber, out pieceId))
			{
				BuilderPiece piece = this.GetPiece(pieceId);
				this.playerToArmShelfLeft.Remove(player.ActorNumber);
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(pieceId, piece.transform.position, piece.transform.rotation, false, -1);
				}
				else
				{
					this.DropPieceForPlayerLeavingInternal(piece, player.ActorNumber);
				}
			}
			int pieceId2;
			if (this.playerToArmShelfRight.TryGetValue(player.ActorNumber, out pieceId2))
			{
				BuilderPiece piece2 = this.GetPiece(pieceId2);
				this.playerToArmShelfRight.Remove(player.ActorNumber);
				if (piece2.armShelf != null)
				{
					piece2.armShelf.piece = null;
					piece2.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(pieceId2, piece2.transform.position, piece2.transform.rotation, false, -1);
					return;
				}
				this.DropPieceForPlayerLeavingInternal(piece2, player.ActorNumber);
			}
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x00172674 File Offset: 0x00170874
		public void DropAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.DropPieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06004B9A RID: 19354 RVA: 0x001726D4 File Offset: 0x001708D4
		public void RecycleAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.RecyclePieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06004B9B RID: 19355 RVA: 0x00172734 File Offset: 0x00170934
		private void DropPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction action = BuilderActions.CreateDetachFromPlayer(-1, piece.pieceId, playerActorNumber);
			BuilderAction action2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
		}

		// Token: 0x06004B9C RID: 19356 RVA: 0x0017278B File Offset: 0x0017098B
		private void RecyclePieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, false, -1);
		}

		// Token: 0x06004B9D RID: 19357 RVA: 0x001727B8 File Offset: 0x001709B8
		private void DetachPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction action = BuilderActions.CreateDetachFromPiece(-1, piece.pieceId, playerActorNumber);
			BuilderAction action2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
		}

		// Token: 0x06004B9E RID: 19358 RVA: 0x00172810 File Offset: 0x00170A10
		public void CreateArmShelf(int pieceIdLeft, int pieceIdRight, int pieceType, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdLeft,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = true
			};
			this.RouteNewCommand(cmd, false);
			BuilderTable.BuilderCommand cmd2 = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdRight,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = false
			};
			this.RouteNewCommand(cmd2, false);
		}

		// Token: 0x06004B9F RID: 19359 RVA: 0x001728A0 File Offset: 0x00170AA0
		public void ExecuteArmShelfCreated(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			bool isLeft = cmd.isLeft;
			if (this.GetPiece(cmd.pieceId) != null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				BuilderArmShelf builderArmShelf = isLeft ? rigContainer.Rig.builderArmShelfLeft : rigContainer.Rig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					if (builderArmShelf.piece != null)
					{
						if (builderArmShelf.piece.isArmShelf && builderArmShelf.piece.isActiveAndEnabled)
						{
							builderArmShelf.piece.armShelf = null;
							this.RecyclePiece(builderArmShelf.piece.pieceId, builderArmShelf.piece.transform.position, builderArmShelf.piece.transform.rotation, false, -1, PhotonNetwork.LocalPlayer);
						}
						else
						{
							builderArmShelf.piece = null;
						}
						BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece;
						builderPiece.armShelf = builderArmShelf;
						builderPiece.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.AddOrUpdate(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.AddOrUpdate(player.ActorNumber, cmd.pieceId);
						return;
					}
					else
					{
						BuilderPiece builderPiece2 = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece2;
						builderPiece2.armShelf = builderArmShelf;
						builderPiece2.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece2.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.TryAdd(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.TryAdd(player.ActorNumber, cmd.pieceId);
					}
				}
			}
		}

		// Token: 0x06004BA0 RID: 19360 RVA: 0x00172AEC File Offset: 0x00170CEC
		public void ClearLocalArmShelf()
		{
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig != null)
			{
				BuilderArmShelf builderArmShelf = offlineVRRig.builderArmShelfLeft;
				if (builderArmShelf != null)
				{
					BuilderPiece piece = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece != null)
					{
						piece.transform.SetParent(null);
					}
				}
				builderArmShelf = offlineVRRig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					BuilderPiece piece2 = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece2 != null)
					{
						piece2.transform.SetParent(null);
					}
				}
			}
		}

		// Token: 0x06004BA1 RID: 19361 RVA: 0x00172B74 File Offset: 0x00170D74
		public void PieceEnteredDropZone(int pieceId, Vector3 worldPos, Quaternion worldRot, int dropZoneId)
		{
			Vector3 velocity = (this.roomCenter.position - worldPos).normalized * BuilderTable.DROP_ZONE_REPEL;
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Repel,
				pieceId = pieceId,
				parentPieceId = pieceId,
				attachPieceId = dropZoneId,
				localPosition = worldPos,
				localRotation = worldRot,
				velocity = velocity,
				angVelocity = Vector3.zero,
				player = NetworkSystem.Instance.MasterClient,
				canRollback = false
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06004BA2 RID: 19362 RVA: 0x00172C18 File Offset: 0x00170E18
		public bool ValidateRepelPiece(BuilderPiece piece)
		{
			if (!this.isSetup)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return false;
			}
			if (piece.state == BuilderPiece.State.Grabbed || piece.state == BuilderPiece.State.GrabbedLocal || piece.state == BuilderPiece.State.Dropped || piece.state == BuilderPiece.State.AttachedToDropped || piece.state == BuilderPiece.State.AttachedToArm)
			{
				bool flag = false;
				for (int i = 0; i < this.repelHistoryLength; i++)
				{
					flag = (flag || this.repelledPieceRoots[i].Contains(piece.pieceId));
					if (flag)
					{
						return false;
					}
				}
				this.repelledPieceRoots[this.repelHistoryIndex].Add(piece.pieceId);
				return true;
			}
			return false;
		}

		// Token: 0x06004BA3 RID: 19363 RVA: 0x00172CBC File Offset: 0x00170EBC
		public void RepelPieceTowardTable(int pieceID)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			Vector3 position = piece.transform.position;
			Quaternion rotation = piece.transform.rotation;
			if (position.y < this.tableCenter.position.y)
			{
				position.y = this.tableCenter.position.y;
			}
			Vector3 velocity = (this.tableCenter.position - position).normalized * BuilderTable.DROP_ZONE_REPEL;
			if (piece.IsHeldLocal())
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.transform.localScale = Vector3.one;
			piece.SetState(BuilderPiece.State.Dropped, false);
			piece.transform.SetLocalPositionAndRotation(position, rotation);
			if (piece.rigidBody != null)
			{
				piece.rigidBody.position = position;
				piece.rigidBody.rotation = rotation;
				piece.rigidBody.velocity = velocity;
				piece.rigidBody.AddForce(Vector3.up * (BuilderTable.DROP_ZONE_REPEL / 2f), ForceMode.VelocityChange);
				piece.rigidBody.angularVelocity = Vector3.zero;
			}
		}

		// Token: 0x06004BA4 RID: 19364 RVA: 0x00172DF4 File Offset: 0x00170FF4
		public BuilderPiece GetPiece(int pieceId)
		{
			int num;
			if (this.pieceIDToIndexCache.TryGetValue(pieceId, out num))
			{
				if (num >= 0 && num < this.pieces.Count)
				{
					return this.pieces[num];
				}
				this.pieceIDToIndexCache.Remove(pieceId);
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].pieceId == pieceId)
				{
					this.pieceIDToIndexCache.Add(pieceId, i);
					return this.pieces[i];
				}
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				if (this.basePieces[j].pieceId == pieceId)
				{
					return this.basePieces[j];
				}
			}
			return null;
		}

		// Token: 0x06004BA5 RID: 19365 RVA: 0x00172EB9 File Offset: 0x001710B9
		public void AddPiece(BuilderPiece piece)
		{
			this.pieces.Add(piece);
			this.UseResources(piece);
			this.AddPieceData(piece);
		}

		// Token: 0x06004BA6 RID: 19366 RVA: 0x00172ED6 File Offset: 0x001710D6
		public void RemovePiece(BuilderPiece piece)
		{
			this.pieces.Remove(piece);
			this.AddResources(piece);
			this.RemovePieceData(piece);
			this.pieceIDToIndexCache.Clear();
		}

		// Token: 0x06004BA7 RID: 19367 RVA: 0x000023F5 File Offset: 0x000005F5
		private void CreateData()
		{
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x000023F5 File Offset: 0x000005F5
		private void DestroyData()
		{
		}

		// Token: 0x06004BA9 RID: 19369 RVA: 0x000E3FB3 File Offset: 0x000E21B3
		private int AddPieceData(BuilderPiece piece)
		{
			return -1;
		}

		// Token: 0x06004BAA RID: 19370 RVA: 0x000023F5 File Offset: 0x000005F5
		public void UpdatePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06004BAB RID: 19371 RVA: 0x000023F5 File Offset: 0x000005F5
		private void RemovePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06004BAC RID: 19372 RVA: 0x000E3FB3 File Offset: 0x000E21B3
		private int AddGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
			return -1;
		}

		// Token: 0x06004BAD RID: 19373 RVA: 0x000023F5 File Offset: 0x000005F5
		private void RemoveGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x000E3FB3 File Offset: 0x000E21B3
		private int AddPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			return -1;
		}

		// Token: 0x06004BAF RID: 19375 RVA: 0x000023F5 File Offset: 0x000005F5
		private void RemovePrivatePlotData(BuilderPiecePrivatePlot plot)
		{
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x00172EFE File Offset: 0x001710FE
		public void OnButtonFreeRotation(BuilderOptionButton button, bool isLeftHand)
		{
			this.useSnapRotation = !this.useSnapRotation;
			button.SetPressed(this.useSnapRotation);
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x00172F1B File Offset: 0x0017111B
		public void OnButtonFreePosition(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.usePlacementStyle == BuilderPlacementStyle.Float)
			{
				this.usePlacementStyle = BuilderPlacementStyle.SnapDown;
			}
			else if (this.usePlacementStyle == BuilderPlacementStyle.SnapDown)
			{
				this.usePlacementStyle = BuilderPlacementStyle.Float;
			}
			button.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
		}

		// Token: 0x06004BB2 RID: 19378 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnButtonSaveLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x06004BB3 RID: 19379 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnButtonClearLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x06004BB4 RID: 19380 RVA: 0x00172F50 File Offset: 0x00171150
		public bool TryPlaceGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, List<BuilderAttachGridPlane> checkGridPlanes, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			Vector3 position = gridPlane.transform.position;
			Quaternion rotation = gridPlane.transform.rotation;
			if (this.gridSize <= 0f)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < checkGridPlanes.Count; i++)
			{
				BuilderAttachGridPlane checkGridPlane = checkGridPlanes[i];
				this.TryPlaceGridPlaneOnGridPlane(piece, gridPlane, position, rotation, checkGridPlane, ref potentialPlacement, ref result);
			}
			return result;
		}

		// Token: 0x06004BB5 RID: 19381 RVA: 0x00172FC4 File Offset: 0x001711C4
		public bool TryPlaceGridPlaneOnGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, Vector3 gridPlanePos, Quaternion gridPlaneRot, BuilderAttachGridPlane checkGridPlane, ref BuilderPotentialPlacement potentialPlacement, ref bool success)
		{
			if (checkGridPlane.male == gridPlane.male)
			{
				return false;
			}
			if (checkGridPlane.piece == gridPlane.piece)
			{
				return false;
			}
			Transform center = checkGridPlane.center;
			Vector3 position = center.position;
			float sqrMagnitude = (position - gridPlanePos).sqrMagnitude;
			float num = checkGridPlane.boundingRadius + gridPlane.boundingRadius;
			if (sqrMagnitude > num * num)
			{
				return false;
			}
			Quaternion rotation = center.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Quaternion quaternion2 = quaternion * gridPlaneRot;
			if (Vector3.Dot(Vector3.up, quaternion2 * Vector3.up) < this.currSnapParams.maxUpDotProduct)
			{
				return false;
			}
			Vector3 vector = quaternion * (gridPlanePos - position);
			float y = vector.y;
			float num2 = -Mathf.Abs(y);
			if (success && num2 < potentialPlacement.score)
			{
				return false;
			}
			if (Mathf.Abs(y) > 1f)
			{
				return false;
			}
			if ((gridPlane.male && y > this.currSnapParams.minOffsetY) || (!gridPlane.male && y < -this.currSnapParams.minOffsetY))
			{
				return false;
			}
			if (Mathf.Abs(y) > this.currSnapParams.maxOffsetY)
			{
				return false;
			}
			Quaternion quaternion3;
			Quaternion rotation2;
			BoingKit.QuaternionUtil.DecomposeSwingTwist(quaternion2, Vector3.up, out quaternion3, out rotation2);
			float maxTwistDotProduct = this.currSnapParams.maxTwistDotProduct;
			Vector3 lhs = rotation2 * Vector3.forward;
			float num3 = Vector3.Dot(lhs, Vector3.forward);
			float num4 = Vector3.Dot(lhs, Vector3.right);
			bool flag = Mathf.Abs(num3) > maxTwistDotProduct;
			bool flag2 = Mathf.Abs(num4) > maxTwistDotProduct;
			if (!flag && !flag2)
			{
				return false;
			}
			float y2;
			uint num5;
			if (flag)
			{
				y2 = ((num3 > 0f) ? 0f : 180f);
				num5 = ((num3 > 0f) ? 0U : 2U);
			}
			else
			{
				y2 = ((num4 > 0f) ? 90f : 270f);
				num5 = ((num4 > 0f) ? 1U : 3U);
			}
			int num6 = flag2 ? gridPlane.width : gridPlane.length;
			int num7 = flag2 ? gridPlane.length : gridPlane.width;
			float num8 = (num7 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num9 = (num6 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num10 = (checkGridPlane.width % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num11 = (checkGridPlane.length % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num12 = num8 - num10;
			float num13 = num9 - num11;
			int num14 = Mathf.RoundToInt((vector.x - num12) / this.gridSize);
			int num15 = Mathf.RoundToInt((vector.z - num13) / this.gridSize);
			int num16 = num14 + Mathf.FloorToInt((float)num7 / 2f);
			int num17 = num15 + Mathf.FloorToInt((float)num6 / 2f);
			int num18 = num16 - (num7 - 1);
			int num19 = num17 - (num6 - 1);
			int num20 = Mathf.FloorToInt((float)checkGridPlane.width / 2f);
			int num21 = Mathf.FloorToInt((float)checkGridPlane.length / 2f);
			int num22 = num20 - (checkGridPlane.width - 1);
			int num23 = num21 - (checkGridPlane.length - 1);
			if (num18 > num20 || num16 < num22 || num19 > num21 || num17 < num23)
			{
				return false;
			}
			BuilderPiece rootPiece = checkGridPlane.piece.GetRootPiece();
			if (BuilderTable.ShareSameRoot(gridPlane.piece, rootPiece))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, gridPlane.piece, rootPiece))
			{
				return false;
			}
			BuilderPiece piece2 = checkGridPlane.piece;
			if (piece2 != null)
			{
				if (piece2.preventSnapUntilMoved > 0)
				{
					return false;
				}
				if (piece2.requestedParentPiece != null && BuilderTable.ShareSameRoot(piece, piece2.requestedParentPiece))
				{
					return false;
				}
			}
			Quaternion rhs = Quaternion.Euler(0f, y2, 0f);
			Quaternion lhs2 = rotation * rhs;
			float x = (float)num14 * this.gridSize + num12;
			float z = (float)num15 * this.gridSize + num13;
			Vector3 point = new Vector3(x, 0f, z);
			Vector3 a = position + rotation * point;
			Transform center2 = gridPlane.center;
			Quaternion quaternion4 = lhs2 * Quaternion.Inverse(center2.localRotation);
			Vector3 point2 = piece.transform.InverseTransformPoint(center2.position);
			Vector3 localPosition = a - quaternion4 * point2;
			potentialPlacement.localPosition = localPosition;
			potentialPlacement.localRotation = quaternion4;
			potentialPlacement.score = num2;
			success = true;
			potentialPlacement.parentPiece = piece2;
			potentialPlacement.parentAttachIndex = checkGridPlane.attachIndex;
			potentialPlacement.attachDistance = Mathf.Abs(y);
			potentialPlacement.attachPlaneNormal = Vector3.up;
			if (!checkGridPlane.male)
			{
				potentialPlacement.attachPlaneNormal *= -1f;
			}
			if (potentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				potentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(potentialPlacement.localPosition);
				potentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * potentialPlacement.localRotation;
			}
			potentialPlacement.parentAttachBounds.min.x = Mathf.Max(num22, num18);
			potentialPlacement.parentAttachBounds.min.y = Mathf.Max(num23, num19);
			potentialPlacement.parentAttachBounds.max.x = Mathf.Min(num20, num16);
			potentialPlacement.parentAttachBounds.max.y = Mathf.Min(num21, num17);
			Vector2Int v = Vector2Int.zero;
			Vector2Int v2 = Vector2Int.zero;
			v.x = potentialPlacement.parentAttachBounds.min.x - num14;
			v2.x = potentialPlacement.parentAttachBounds.max.x - num14;
			v.y = potentialPlacement.parentAttachBounds.min.y - num15;
			v2.y = potentialPlacement.parentAttachBounds.max.y - num15;
			potentialPlacement.twist = (byte)num5;
			potentialPlacement.bumpOffsetX = (sbyte)num14;
			potentialPlacement.bumpOffsetZ = (sbyte)num15;
			int offsetX = (num7 % 2 == 0) ? 1 : 0;
			int offsetY = (num6 % 2 == 0) ? 1 : 0;
			if (flag && num3 < 0f)
			{
				v = this.Rotate180(v, offsetX, offsetY);
				v2 = this.Rotate180(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 < 0f)
			{
				v = this.Rotate270(v, offsetX, offsetY);
				v2 = this.Rotate270(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 > 0f)
			{
				v = this.Rotate90(v, offsetX, offsetY);
				v2 = this.Rotate90(v2, offsetX, offsetY);
			}
			potentialPlacement.attachBounds.min.x = Mathf.Min(v.x, v2.x);
			potentialPlacement.attachBounds.min.y = Mathf.Min(v.y, v2.y);
			potentialPlacement.attachBounds.max.x = Mathf.Max(v.x, v2.x);
			potentialPlacement.attachBounds.max.y = Mathf.Max(v.y, v2.y);
			return true;
		}

		// Token: 0x06004BB6 RID: 19382 RVA: 0x0017373E File Offset: 0x0017193E
		private Vector2Int Rotate90(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y * -1 + offsetY, v.x);
		}

		// Token: 0x06004BB7 RID: 19383 RVA: 0x00173757 File Offset: 0x00171957
		private Vector2Int Rotate270(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y, v.x * -1 + offsetX);
		}

		// Token: 0x06004BB8 RID: 19384 RVA: 0x00173770 File Offset: 0x00171970
		private Vector2Int Rotate180(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.x * -1 + offsetX, v.y * -1 + offsetY);
		}

		// Token: 0x06004BB9 RID: 19385 RVA: 0x0017378D File Offset: 0x0017198D
		public bool ShareSameRoot(BuilderAttachGridPlane plane, BuilderAttachGridPlane otherPlane)
		{
			return !(plane == null) && !(otherPlane == null) && !(otherPlane.piece == null) && BuilderTable.ShareSameRoot(plane.piece, otherPlane.piece);
		}

		// Token: 0x06004BBA RID: 19386 RVA: 0x001737C4 File Offset: 0x001719C4
		public static bool ShareSameRoot(BuilderPiece piece, BuilderPiece otherPiece)
		{
			if (otherPiece == null || piece == null)
			{
				return false;
			}
			if (piece == otherPiece)
			{
				return true;
			}
			BuilderPiece builderPiece = piece;
			int num = 2048;
			while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
			{
				builderPiece = builderPiece.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			num = 2048;
			BuilderPiece builderPiece2 = otherPiece;
			while (builderPiece2.parentPiece != null && !builderPiece2.parentPiece.isBuiltIntoTable)
			{
				builderPiece2 = builderPiece2.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			return builderPiece == builderPiece2;
		}

		// Token: 0x06004BBB RID: 19387 RVA: 0x00173864 File Offset: 0x00171A64
		public bool TryPlacePieceOnTableNoDrop(bool leftHand, BuilderPiece testPiece, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			if (testPiece == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			return this.TryPlacePieceGridPlanesOnTableInternal(testPiece, this.maxPlacementChildDepth, checkGridPlanesMale, checkGridPlanesFemale, out potentialPlacement);
		}

		// Token: 0x06004BBC RID: 19388 RVA: 0x001738B4 File Offset: 0x00171AB4
		public bool TryPlacePieceOnTableNoDropJobs(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderPieceData> pieceData, NativeList<BuilderGridPlaneData> checkGridPlaneData, NativeList<BuilderPieceData> checkPieceData, out BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = Vector3.zero,
				worldToLocalRot = Quaternion.identity,
				localToWorldPos = Vector3.zero,
				localToWorldRot = Quaternion.identity,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			BuilderPotentialPlacementData builderPotentialPlacementData = default(BuilderPotentialPlacementData);
			bool flag = false;
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData2 = nativeQueue.Dequeue();
				if (!flag || builderPotentialPlacementData2.score > builderPotentialPlacementData.score)
				{
					builderPotentialPlacementData = builderPotentialPlacementData2;
					flag = true;
				}
			}
			if (flag)
			{
				potentialPlacement = builderPotentialPlacementData.ToPotentialPlacement(this);
			}
			if (flag)
			{
				nativeQueue.Clear();
				this.currSnapParams = this.overlapParams;
				Vector3 worldToLocalPos = -potentialPlacement.attachPiece.transform.position;
				Quaternion worldToLocalRot = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				Quaternion localToWorldRot = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
				Vector3 localToWorldPos = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
				new BuilderFindPotentialSnaps
				{
					gridSize = this.gridSize,
					currSnapParams = this.currSnapParams,
					gridPlanes = gridPlaneData,
					checkGridPlanes = checkGridPlaneData,
					worldToLocalPos = worldToLocalPos,
					worldToLocalRot = worldToLocalRot,
					localToWorldPos = localToWorldPos,
					localToWorldRot = localToWorldRot,
					potentialPlacements = nativeQueue.AsParallelWriter()
				}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
				while (!nativeQueue.IsEmpty())
				{
					BuilderPotentialPlacementData builderPotentialPlacementData3 = nativeQueue.Dequeue();
					if (builderPotentialPlacementData3.attachDistance < this.currSnapParams.maxBlockSnapDist)
					{
						allPlacements.Add(builderPotentialPlacementData3.ToPotentialPlacement(this));
					}
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06004BBD RID: 19389 RVA: 0x00173B20 File Offset: 0x00171D20
		public bool CalcAllPotentialPlacements(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderGridPlaneData> checkGridPlaneData, BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			if (this == null)
			{
				return false;
			}
			bool result = false;
			this.currSnapParams = this.overlapParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			nativeQueue.Clear();
			Vector3 worldToLocalPos = -potentialPlacement.attachPiece.transform.position;
			Quaternion worldToLocalRot = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
			BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
			Quaternion localToWorldRot = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
			Vector3 localToWorldPos = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = worldToLocalPos,
				worldToLocalRot = worldToLocalRot,
				localToWorldPos = localToWorldPos,
				localToWorldRot = localToWorldRot,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData = nativeQueue.Dequeue();
				if (builderPotentialPlacementData.attachDistance < this.currSnapParams.maxBlockSnapDist)
				{
					allPlacements.Add(builderPotentialPlacementData.ToPotentialPlacement(this));
				}
			}
			nativeQueue.Dispose();
			return result;
		}

		// Token: 0x06004BBE RID: 19390 RVA: 0x00173C8C File Offset: 0x00171E8C
		public bool CanPiecesPotentiallySnap(BuilderPiece pieceInHand, BuilderPiece piece)
		{
			BuilderPiece rootPiece = piece.GetRootPiece();
			return !(rootPiece == pieceInHand) && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece) && (!(piece.requestedParentPiece != null) || !BuilderTable.ShareSameRoot(pieceInHand, piece.requestedParentPiece)) && piece.preventSnapUntilMoved <= 0;
		}

		// Token: 0x06004BBF RID: 19391 RVA: 0x00173CF0 File Offset: 0x00171EF0
		public bool CanPiecesPotentiallyOverlap(BuilderPiece pieceInHand, BuilderPiece rootWhenPlaced, BuilderPiece.State stateWhenPlaced, BuilderPiece otherPiece)
		{
			BuilderPiece rootPiece = otherPiece.GetRootPiece();
			if (rootPiece == pieceInHand)
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece))
			{
				return false;
			}
			if (otherPiece.requestedParentPiece != null && BuilderTable.ShareSameRoot(pieceInHand, otherPiece.requestedParentPiece))
			{
				return false;
			}
			if (otherPiece.preventSnapUntilMoved > 0)
			{
				return false;
			}
			BuilderPiece.State stateB = otherPiece.state;
			if (otherPiece.isBuiltIntoTable && !otherPiece.isArmShelf)
			{
				stateB = BuilderPiece.State.AttachedAndPlaced;
			}
			return BuilderTable.AreStatesCompatibleForOverlap(stateWhenPlaced, stateB, rootWhenPlaced, rootPiece);
		}

		// Token: 0x06004BC0 RID: 19392 RVA: 0x00173D79 File Offset: 0x00171F79
		public void TryDropPiece(bool leftHand, BuilderPiece testPiece, Vector3 velocity, Vector3 angVelocity)
		{
			if (this == null)
			{
				return;
			}
			if (testPiece == null)
			{
				return;
			}
			this.RequestDropPiece(testPiece, testPiece.transform.position, testPiece.transform.rotation, velocity, angVelocity);
		}

		// Token: 0x06004BC1 RID: 19393 RVA: 0x00173DB0 File Offset: 0x00171FB0
		public bool TryPlacePieceGridPlanesOnTableInternal(BuilderPiece testPiece, int recurse, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			bool result = false;
			bool flag = false;
			if (testPiece != null && testPiece.gridPlanes != null && testPiece.gridPlanes.Count > 0 && testPiece.gridPlanes != null)
			{
				for (int i = 0; i < testPiece.gridPlanes.Count; i++)
				{
					List<BuilderAttachGridPlane> checkGridPlanes = testPiece.gridPlanes[i].male ? checkGridPlanesFemale : checkGridPlanesMale;
					BuilderPotentialPlacement builderPotentialPlacement;
					if (this.TryPlaceGridPlane(testPiece, testPiece.gridPlanes[i], checkGridPlanes, out builderPotentialPlacement))
					{
						if (builderPotentialPlacement.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag = true;
						}
						if (builderPotentialPlacement.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement;
							potentialPlacement.attachIndex = i;
							potentialPlacement.attachPiece = testPiece;
							result = true;
						}
					}
				}
			}
			if (recurse > 0)
			{
				BuilderPiece builderPiece = testPiece.firstChildPiece;
				while (builderPiece != null)
				{
					BuilderPotentialPlacement builderPotentialPlacement2;
					if (this.TryPlacePieceGridPlanesOnTableInternal(builderPiece, recurse - 1, checkGridPlanesMale, checkGridPlanesFemale, out builderPotentialPlacement2))
					{
						if (builderPotentialPlacement2.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag = true;
						}
						if (builderPotentialPlacement2.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement2;
							result = true;
						}
					}
					builderPiece = builderPiece.nextSiblingPiece;
				}
			}
			if (testPiece.preventSnapUntilMoved > 0 && !flag)
			{
				testPiece.preventSnapUntilMoved--;
				this.UpdatePieceData(testPiece);
			}
			return result;
		}

		// Token: 0x06004BC2 RID: 19394 RVA: 0x00173F34 File Offset: 0x00172134
		public void TryPlaceRandomlyOnTable(BuilderPiece piece)
		{
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[Random.Range(0, piece.gridPlanes.Count)];
			List<BuilderAttachGridPlane> list = this.baseGridPlanes;
			int num = Random.Range(0, list.Count);
			int i = 0;
			while (i < list.Count)
			{
				int index = (i + num) % list.Count;
				BuilderAttachGridPlane builderAttachGridPlane2 = list[index];
				if (builderAttachGridPlane2.male != builderAttachGridPlane.male && !(builderAttachGridPlane2.piece == builderAttachGridPlane.piece) && !this.ShareSameRoot(builderAttachGridPlane, builderAttachGridPlane2))
				{
					Vector3 zero = Vector3.zero;
					Quaternion identity = Quaternion.identity;
					BuilderPiece piece2 = builderAttachGridPlane2.piece;
					int attachIndex = builderAttachGridPlane2.attachIndex;
					Transform center = builderAttachGridPlane.center;
					Quaternion quaternion = builderAttachGridPlane2.transform.rotation * Quaternion.Inverse(center.localRotation);
					Vector3 point = piece.transform.InverseTransformPoint(center.position);
					Vector3 a = builderAttachGridPlane2.transform.position - quaternion * point;
					if (piece2 != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = piece2.gridPlanes[attachIndex];
						Vector3 lossyScale = builderAttachGridPlane3.transform.lossyScale;
						Vector3 b = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * Vector3.Scale(a - builderAttachGridPlane3.transform.position, b);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * quaternion;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x001740F0 File Offset: 0x001722F0
		public void UseResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.UseResource(cost.quantities[i]);
			}
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x00174136 File Offset: 0x00172336
		private void UseResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] += quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x00174178 File Offset: 0x00172378
		public void AddResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.AddResource(cost.quantities[i]);
			}
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x001741BE File Offset: 0x001723BE
		private void AddResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] -= quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x00174200 File Offset: 0x00172400
		public bool HasEnoughUnreservedResources(BuilderResources resources)
		{
			if (resources == null)
			{
				return false;
			}
			for (int i = 0; i < resources.quantities.Count; i++)
			{
				if (!this.HasEnoughUnreservedResource(resources.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x00174248 File Offset: 0x00172448
		public bool HasEnoughUnreservedResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + this.reservedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x001742A0 File Offset: 0x001724A0
		public bool HasEnoughResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return false;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				if (!this.HasEnoughResource(cost.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004BCA RID: 19402 RVA: 0x001742EC File Offset: 0x001724EC
		public bool HasEnoughResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06004BCB RID: 19403 RVA: 0x00174328 File Offset: 0x00172528
		public int GetAvailableResources(BuilderResourceType type)
		{
			if (type < BuilderResourceType.Basic || type >= BuilderResourceType.Count)
			{
				return 0;
			}
			return this.maxResources[(int)type] - this.usedResources[(int)type];
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x00174348 File Offset: 0x00172548
		private void OnAvailableResourcesChange()
		{
			if (this.isSetup && this.isTableMutable)
			{
				for (int i = 0; i < this.conveyors.Count; i++)
				{
					this.conveyors[i].OnAvailableResourcesChange();
				}
				foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
				{
					builderResourceMeter.OnAvailableResourcesChange();
				}
			}
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x001743D0 File Offset: 0x001725D0
		public int GetPrivateResourceLimitForType(int type)
		{
			if (this.plotMaxResources == null)
			{
				return 0;
			}
			return this.plotMaxResources[type];
		}

		// Token: 0x06004BCE RID: 19406 RVA: 0x001743E4 File Offset: 0x001725E4
		private void WriteVector3(BinaryWriter writer, Vector3 data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x0017440A File Offset: 0x0017260A
		private void WriteQuaternion(BinaryWriter writer, Quaternion data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
			writer.Write(data.w);
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x0017443C File Offset: 0x0017263C
		private Vector3 ReadVector3(BinaryReader reader)
		{
			Vector3 result;
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			return result;
		}

		// Token: 0x06004BD1 RID: 19409 RVA: 0x00174474 File Offset: 0x00172674
		private Quaternion ReadQuaternion(BinaryReader reader)
		{
			Quaternion result;
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			result.w = reader.ReadSingle();
			return result;
		}

		// Token: 0x06004BD2 RID: 19410 RVA: 0x001744B8 File Offset: 0x001726B8
		public static int PackPiecePlacement(byte twist, sbyte xOffset, sbyte zOffset)
		{
			int num = (int)(twist & 3);
			int num2 = (int)xOffset + 128;
			int num3 = (int)zOffset + 128;
			return num2 + (num3 << 8) + (num << 16);
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x001744E4 File Offset: 0x001726E4
		public static void UnpackPiecePlacement(int packed, out byte twist, out sbyte xOffset, out sbyte zOffset)
		{
			int num = packed & 255;
			int num2 = packed >> 8 & 255;
			int num3 = packed >> 16 & 3;
			twist = (byte)num3;
			xOffset = (sbyte)(num - 128);
			zOffset = (sbyte)(num2 - 128);
		}

		// Token: 0x06004BD4 RID: 19412 RVA: 0x00174524 File Offset: 0x00172724
		private long PackSnapInfo(int attachGridIndex, int otherAttachGridIndex, Vector2Int min, Vector2Int max)
		{
			long num = (long)Mathf.Clamp(attachGridIndex, 0, 31);
			long num2 = (long)Mathf.Clamp(otherAttachGridIndex, 0, 31);
			long num3 = (long)Mathf.Clamp(min.x + 1024, 0, 2047);
			long num4 = (long)Mathf.Clamp(min.y + 1024, 0, 2047);
			long num5 = (long)Mathf.Clamp(max.x + 1024, 0, 2047);
			long num6 = (long)Mathf.Clamp(max.y + 1024, 0, 2047);
			return num + (num2 << 5) + (num3 << 10) + (num4 << 21) + (num5 << 32) + (num6 << 43);
		}

		// Token: 0x06004BD5 RID: 19413 RVA: 0x001745C8 File Offset: 0x001727C8
		private void UnpackSnapInfo(long packed, out int attachGridIndex, out int otherAttachGridIndex, out Vector2Int min, out Vector2Int max)
		{
			long num = packed & 31L;
			attachGridIndex = (int)num;
			num = (packed >> 5 & 31L);
			otherAttachGridIndex = (int)num;
			int x = (int)(packed >> 10 & 2047L) - 1024;
			int y = (int)(packed >> 21 & 2047L) - 1024;
			min = new Vector2Int(x, y);
			int x2 = (int)(packed >> 32 & 2047L) - 1024;
			int y2 = (int)(packed >> 43 & 2047L) - 1024;
			max = new Vector2Int(x2, y2);
		}

		// Token: 0x06004BD6 RID: 19414 RVA: 0x00174655 File Offset: 0x00172855
		private void OnTitleDataUpdate(string key)
		{
			if (key.Equals(this.SharedMapConfigTitleDataKey))
			{
				this.FetchSharedBlocksStartingMapConfig();
			}
		}

		// Token: 0x06004BD7 RID: 19415 RVA: 0x0017466B File Offset: 0x0017286B
		private void FetchSharedBlocksStartingMapConfig()
		{
			if (!this.isTableMutable)
			{
				PlayFabTitleDataCache.Instance.GetTitleData(this.SharedMapConfigTitleDataKey, new Action<string>(this.OnGetStartingMapConfigSuccess), new Action<PlayFabError>(this.OnGetStartingMapConfigFail));
			}
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x001746A0 File Offset: 0x001728A0
		private void OnGetStartingMapConfigSuccess(string result)
		{
			this.ResetStartingMapConfig();
			if (result.IsNullOrEmpty())
			{
				return;
			}
			try
			{
				SharedBlocksManager.StartingMapConfig startingMapConfig = JsonUtility.FromJson<SharedBlocksManager.StartingMapConfig>(result);
				if (startingMapConfig.useMapID)
				{
					if (SharedBlocksManager.IsMapIDValid(startingMapConfig.mapID))
					{
						this.startingMapConfig.useMapID = true;
						this.startingMapConfig.mapID = startingMapConfig.mapID;
					}
					else
					{
						GTDev.LogError<string>(string.Format("BuilderTable {0} OnGetStartingMapConfigSuccess Title Data Default Map Config has Invalid Map ID", this.tableZone), null);
					}
				}
				else
				{
					this.startingMapConfig.pageNumber = Mathf.Max(startingMapConfig.pageNumber, 0);
					this.startingMapConfig.pageSize = Mathf.Max(startingMapConfig.pageSize, 1);
					if (!startingMapConfig.sortMethod.IsNullOrEmpty() && (startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.Top.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.NewlyCreated.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.RecentlyUpdated.ToString())))
					{
						this.startingMapConfig.sortMethod = startingMapConfig.sortMethod;
					}
					else
					{
						GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Unknown sort method " + startingMapConfig.sortMethod, null);
					}
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Exception Deserializing " + ex.Message, null);
			}
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x00174838 File Offset: 0x00172A38
		private void OnGetStartingMapConfigFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigFail " + error.Error.ToString(), null);
			this.ResetStartingMapConfig();
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x00174878 File Offset: 0x00172A78
		private void ResetStartingMapConfig()
		{
			this.startingMapConfig = new SharedBlocksManager.StartingMapConfig
			{
				pageNumber = 0,
				pageSize = 10,
				sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
				useMapID = false,
				mapID = null
			};
		}

		// Token: 0x06004BDB RID: 19419 RVA: 0x001748CB File Offset: 0x00172ACB
		private void RequestTableConfiguration()
		{
			SharedBlocksManager.instance.OnGetTableConfiguration += this.OnGetTableConfiguration;
			SharedBlocksManager.instance.RequestTableConfiguration();
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x001748ED File Offset: 0x00172AED
		private void OnGetTableConfiguration(string configString)
		{
			SharedBlocksManager.instance.OnGetTableConfiguration -= this.OnGetTableConfiguration;
			if (!configString.IsNullOrEmpty())
			{
				this.ParseTableConfiguration(configString);
			}
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x00174914 File Offset: 0x00172B14
		private void ParseTableConfiguration(string dataRecord)
		{
			if (string.IsNullOrEmpty(dataRecord))
			{
				return;
			}
			BuilderTableConfiguration builderTableConfiguration = JsonUtility.FromJson<BuilderTableConfiguration>(dataRecord);
			if (builderTableConfiguration != null)
			{
				if (builderTableConfiguration.TableResourceLimits != null)
				{
					for (int i = 0; i < builderTableConfiguration.TableResourceLimits.Length; i++)
					{
						int num = builderTableConfiguration.TableResourceLimits[i];
						if (num >= 0)
						{
							this.maxResources[i] = num;
						}
					}
				}
				if (builderTableConfiguration.PlotResourceLimits != null)
				{
					for (int j = 0; j < builderTableConfiguration.PlotResourceLimits.Length; j++)
					{
						int num2 = builderTableConfiguration.PlotResourceLimits[j];
						if (num2 >= 0)
						{
							this.plotMaxResources[j] = num2;
						}
					}
				}
				int droppedPieceLimit = builderTableConfiguration.DroppedPieceLimit;
				if (droppedPieceLimit >= 0)
				{
					BuilderTable.DROPPED_PIECE_LIMIT = droppedPieceLimit;
				}
				if (builderTableConfiguration.updateCountdownDate != null && !string.IsNullOrEmpty(builderTableConfiguration.updateCountdownDate))
				{
					try
					{
						DateTime.Parse(builderTableConfiguration.updateCountdownDate, CultureInfo.InvariantCulture);
						BuilderTable.nextUpdateOverride = builderTableConfiguration.updateCountdownDate;
						goto IL_DC;
					}
					catch
					{
						BuilderTable.nextUpdateOverride = string.Empty;
						goto IL_DC;
					}
				}
				BuilderTable.nextUpdateOverride = string.Empty;
				IL_DC:
				this.OnAvailableResourcesChange();
				UnityEvent onTableConfigurationUpdated = this.OnTableConfigurationUpdated;
				if (onTableConfigurationUpdated == null)
				{
					return;
				}
				onTableConfigurationUpdated.Invoke();
			}
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x00174A24 File Offset: 0x00172C24
		private void DumpTableConfig()
		{
			BuilderTableConfiguration builderTableConfiguration = new BuilderTableConfiguration();
			Array.Clear(builderTableConfiguration.TableResourceLimits, 0, builderTableConfiguration.TableResourceLimits.Length);
			Array.Clear(builderTableConfiguration.PlotResourceLimits, 0, builderTableConfiguration.PlotResourceLimits.Length);
			foreach (BuilderResourceQuantity builderResourceQuantity in this.totalResources.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < (BuilderResourceType)builderTableConfiguration.TableResourceLimits.Length)
				{
					builderTableConfiguration.TableResourceLimits[(int)builderResourceQuantity.type] = builderResourceQuantity.count;
				}
			}
			foreach (BuilderResourceQuantity builderResourceQuantity2 in this.resourcesPerPrivatePlot.quantities)
			{
				if (builderResourceQuantity2.type >= BuilderResourceType.Basic && builderResourceQuantity2.type < (BuilderResourceType)builderTableConfiguration.PlotResourceLimits.Length)
				{
					builderTableConfiguration.PlotResourceLimits[(int)builderResourceQuantity2.type] = builderResourceQuantity2.count;
				}
			}
			builderTableConfiguration.DroppedPieceLimit = BuilderTable.DROPPED_PIECE_LIMIT;
			builderTableConfiguration.updateCountdownDate = "1/10/2025 16:00:00";
			string str = JsonUtility.ToJson(builderTableConfiguration);
			Debug.Log("Configuration Dump \n" + str);
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x00174B70 File Offset: 0x00172D70
		private string GetSaveDataTimeKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2") + "Time";
		}

		// Token: 0x06004BE0 RID: 19424 RVA: 0x00174B8D File Offset: 0x00172D8D
		private string GetSaveDataKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2");
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x00174BA5 File Offset: 0x00172DA5
		public void FindAndLoadSharedBlocksMap(string mapID)
		{
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundSharedBlocksMap));
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x00174BBE File Offset: 0x00172DBE
		public string GetSharedBlocksMapID()
		{
			if (this.sharedBlocksMap != null)
			{
				return this.sharedBlocksMap.MapID;
			}
			return string.Empty;
		}

		// Token: 0x06004BE3 RID: 19427 RVA: 0x00174BDC File Offset: 0x00172DDC
		private void FoundSharedBlocksMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (map == null || map.MapData.IsNullOrEmpty())
			{
				this.builderNetworking.LoadSharedBlocksFailedMaster((map == null) ? string.Empty : map.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			this.sharedBlocksMap = map;
			this.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
		}

		// Token: 0x06004BE4 RID: 19428 RVA: 0x00174C58 File Offset: 0x00172E58
		private void BuildInitialTableForPlayer()
		{
			if (NetworkSystem.Instance.IsNull() || !NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate || NetworkSystem.Instance.GetLocalPlayer() == null || !NetworkSystem.Instance.IsMasterClient)
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			SharedBlocksManager.instance.OnFetchPrivateScanComplete += this.OnFetchPrivateScanComplete;
			SharedBlocksManager.instance.RequestFetchPrivateScan(this.currentSaveSlot);
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x00174CE4 File Offset: 0x00172EE4
		private void OnFetchPrivateScanComplete(int slot, bool success)
		{
			SharedBlocksManager.instance.OnFetchPrivateScanComplete -= this.OnFetchPrivateScanComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			string tableJson;
			if (!success || !SharedBlocksManager.instance.TryGetPrivateScanResponse(slot, out tableJson))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!this.BuildTableFromJson(tableJson, false))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			this.SetIsDirty(false);
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x00174D48 File Offset: 0x00172F48
		private void BuildSelectedSharedMap()
		{
			if (!NetworkSystem.Instance.IsNull() && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				if (this.sharedBlocksMap != null && !this.sharedBlocksMap.MapData.IsNullOrEmpty())
				{
					this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
					return;
				}
				if (SharedBlocksManager.IsMapIDValid(this.pendingMapID))
				{
					SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = this.pendingMapID
					};
					this.LoadSharedMap(map);
					return;
				}
				this.FindStartingMap();
			}
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x00174DD4 File Offset: 0x00172FD4
		private void FindStartingMap()
		{
			if (this.hasStartingMap && Time.timeAsDouble < this.startingMapCacheTime + 60.0)
			{
				this.FoundDefaultSharedBlocksMap(true, this.startingMap);
				return;
			}
			if (this.getStartingMapInProgress)
			{
				return;
			}
			this.hasStartingMap = false;
			this.getStartingMapInProgress = true;
			if (this.startingMapConfig.useMapID && SharedBlocksManager.IsMapIDValid(this.startingMapConfig.mapID))
			{
				this.startingMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = this.startingMapConfig.mapID
				};
				SharedBlocksManager.instance.RequestMapDataFromID(this.startingMapConfig.mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
				return;
			}
			if (this.hasCachedTopMaps && Time.timeAsDouble <= this.lastGetTopMapsTime + 60.0)
			{
				this.ChooseMapFromList();
				return;
			}
			SharedBlocksManager.instance.OnGetPopularMapsComplete += this.FoundStartingMapList;
			if (!SharedBlocksManager.instance.RequestGetTopMaps(this.startingMapConfig.pageNumber, this.startingMapConfig.pageSize, this.startingMapConfig.sortMethod.ToString()))
			{
				this.FoundStartingMapList(false);
			}
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x00174EF8 File Offset: 0x001730F8
		private void FoundStartingMapList(bool success)
		{
			SharedBlocksManager.instance.OnGetPopularMapsComplete -= this.FoundStartingMapList;
			if (success && SharedBlocksManager.instance.LatestPopularMaps.Count > 0)
			{
				this.startingMapList.Clear();
				this.startingMapList.AddRange(SharedBlocksManager.instance.LatestPopularMaps);
				this.hasCachedTopMaps = (this.startingMapList.Count > 0);
				this.lastGetTopMapsTime = (double)Time.time;
				this.ChooseMapFromList();
				return;
			}
			this.FoundDefaultSharedBlocksMap(false, null);
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x00174F80 File Offset: 0x00173180
		private void ChooseMapFromList()
		{
			int index = Random.Range(0, this.startingMapList.Count);
			this.startingMap = this.startingMapList[index];
			if (this.startingMap == null || !SharedBlocksManager.IsMapIDValid(this.startingMap.MapID))
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			SharedBlocksManager.instance.RequestMapDataFromID(this.startingMap.MapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
		}

		// Token: 0x06004BEA RID: 19434 RVA: 0x00174FF8 File Offset: 0x001731F8
		private void FoundTopMapData(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null || !SharedBlocksManager.IsMapIDValid(map.MapID) || map.MapID != this.startingMap.MapID)
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			this.hasStartingMap = true;
			this.startingMapCacheTime = Time.timeAsDouble;
			this.startingMap.MapData = map.MapData;
			this.FoundDefaultSharedBlocksMap(true, this.startingMap);
		}

		// Token: 0x06004BEB RID: 19435 RVA: 0x00175068 File Offset: 0x00173268
		private void FoundDefaultSharedBlocksMap(bool success, SharedBlocksManager.SharedBlocksMap map)
		{
			this.getStartingMapInProgress = false;
			if (success && !map.MapData.IsNullOrEmpty())
			{
				this.startingMapCacheTime = Time.timeAsDouble;
				this.startingMap = map;
				this.hasStartingMap = true;
				this.sharedBlocksMap = map;
				this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
				return;
			}
			this.TryBuildingFromTitleData();
		}

		// Token: 0x06004BEC RID: 19436 RVA: 0x001750C4 File Offset: 0x001732C4
		private void TryBuildingSharedBlocksMap(string mapData)
		{
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!this.BuildTableFromJson(mapData, true))
			{
				GTDev.LogWarning<string>("Unable to build shared blocks map", null);
				this.builderNetworking.LoadSharedBlocksFailedMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			base.StartCoroutine(this.CheckForNoBlocks());
		}

		// Token: 0x06004BED RID: 19437 RVA: 0x00175139 File Offset: 0x00173339
		private IEnumerator CheckForNoBlocks()
		{
			yield return null;
			if (!this.NoBlocksCheck())
			{
				GTDev.LogError<string>("Failed No Blocks Check", null);
				this.builderNetworking.SharedBlocksOutOfBoundsMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				yield break;
			}
			this.OnFinishedInitialTableBuild();
			yield break;
		}

		// Token: 0x06004BEE RID: 19438 RVA: 0x00175148 File Offset: 0x00173348
		private void TryBuildingFromTitleData()
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete += this.OnGetTitleDataBuildComplete;
			SharedBlocksManager.instance.FetchTitleDataBuild();
		}

		// Token: 0x06004BEF RID: 19439 RVA: 0x0017516C File Offset: 0x0017336C
		private void OnGetTitleDataBuildComplete(string titleDataBuild)
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete -= this.OnGetTitleDataBuildComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!titleDataBuild.IsNullOrEmpty())
			{
				if (!this.BuildTableFromJson(titleDataBuild, true))
				{
					this.tableData = new BuilderTableData();
				}
			}
			else
			{
				this.tableData = new BuilderTableData();
			}
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x001751CC File Offset: 0x001733CC
		public void SaveTableForPlayer()
		{
			if (SharedBlocksManager.instance.IsWaitingOnRequest())
			{
				this.SetIsDirty(true);
				UnityEvent<string> onSaveFailure = this.OnSaveFailure;
				if (onSaveFailure == null)
				{
					return;
				}
				onSaveFailure.Invoke("Busy");
				return;
			}
			else
			{
				this.saveInProgress = true;
				if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
				{
					this.saveInProgress = false;
					return;
				}
				if (!this.isDirty)
				{
					this.saveInProgress = false;
					UnityEvent onSaveTimeUpdated = this.OnSaveTimeUpdated;
					if (onSaveTimeUpdated == null)
					{
						return;
					}
					onSaveTimeUpdated.Invoke();
					return;
				}
				else
				{
					if (this.NoBlocksCheck())
					{
						if (this.tableData == null)
						{
							this.tableData = new BuilderTableData();
						}
						this.SetIsDirty(false);
						this.tableData.numEdits++;
						string text = this.WriteTableToJson();
						text = Convert.ToBase64String(GZipStream.CompressString(text));
						SharedBlocksManager.instance.OnSavePrivateScanSuccess += this.OnSaveScanSuccess;
						SharedBlocksManager.instance.OnSavePrivateScanFailed += this.OnSaveScanFailure;
						SharedBlocksManager.instance.RequestSavePrivateScan(this.currentSaveSlot, text);
						return;
					}
					this.saveInProgress = false;
					this.SetIsDirty(true);
					UnityEvent<string> onSaveFailure2 = this.OnSaveFailure;
					if (onSaveFailure2 == null)
					{
						return;
					}
					onSaveFailure2.Invoke("PLEASE REMOVE BLOCKS CONNECTED OUTSIDE OF TABLE PLATFORM");
					return;
				}
			}
		}

		// Token: 0x06004BF1 RID: 19441 RVA: 0x001752E8 File Offset: 0x001734E8
		private void OnSaveScanSuccess(int scan)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			UnityEvent onSaveSuccess = this.OnSaveSuccess;
			if (onSaveSuccess == null)
			{
				return;
			}
			onSaveSuccess.Invoke();
		}

		// Token: 0x06004BF2 RID: 19442 RVA: 0x00175338 File Offset: 0x00173538
		private void OnSaveScanFailure(int scan, string message)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			this.SetIsDirty(true);
			UnityEvent<string> onSaveFailure = this.OnSaveFailure;
			if (onSaveFailure == null)
			{
				return;
			}
			onSaveFailure.Invoke(message);
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x00175390 File Offset: 0x00173590
		private string WriteTableToJson()
		{
			this.tableData.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.tableData.pieceType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedPieceType : this.pieces[i].pieceType);
					this.tableData.pieceId.Add(this.pieces[i].pieceId);
					this.tableData.parentId.Add((this.pieces[i].parentPiece == null) ? -1 : this.pieces[i].parentPiece.pieceId);
					this.tableData.attachIndex.Add(this.pieces[i].attachIndex);
					this.tableData.parentAttachIndex.Add((this.pieces[i].parentPiece == null) ? -1 : this.pieces[i].parentAttachIndex);
					this.tableData.placement.Add(this.pieces[i].GetPiecePlacement());
					this.tableData.materialType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedMaterialType : this.pieces[i].materialType);
					BuilderMovingSnapPiece component = this.pieces[i].GetComponent<BuilderMovingSnapPiece>();
					int item = (component == null) ? 0 : component.GetTimeOffset();
					this.tableData.timeOffset.Add(item);
					for (int j = 0; j < this.pieces[i].gridPlanes.Count; j++)
					{
						if (!(this.pieces[i].gridPlanes[j] == null))
						{
							for (SnapOverlap snapOverlap = this.pieces[i].gridPlanes[j].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								if (snapOverlap.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey item2 = BuilderTable.BuildOverlapKey(this.pieces[i].pieceId, snapOverlap.otherPlane.piece.pieceId, j, snapOverlap.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(item2))
									{
										BuilderTable.tempDuplicateOverlaps.Add(item2);
										long item3 = this.PackSnapInfo(j, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
										this.tableData.overlapingPieces.Add(this.pieces[i].pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(item3);
									}
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				if (!(builderPiece == null))
				{
					for (int k = 0; k < builderPiece.gridPlanes.Count; k++)
					{
						if (!(builderPiece.gridPlanes[k] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece.gridPlanes[k].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								if (snapOverlap2.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap2.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey item4 = BuilderTable.BuildOverlapKey(builderPiece.pieceId, snapOverlap2.otherPlane.piece.pieceId, k, snapOverlap2.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(item4))
									{
										BuilderTable.tempDuplicateOverlaps.Add(item4);
										long item5 = this.PackSnapInfo(k, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
										this.tableData.overlapingPieces.Add(builderPiece.pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(item5);
									}
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			this.tableData.numPieces = this.tableData.pieceType.Count;
			return JsonUtility.ToJson(this.tableData);
		}

		// Token: 0x06004BF4 RID: 19444 RVA: 0x001758E4 File Offset: 0x00173AE4
		private static BuilderTable.SnapOverlapKey BuildOverlapKey(int pieceId, int otherPieceId, int attachGridIndex, int otherAttachGridIndex)
		{
			BuilderTable.SnapOverlapKey result = default(BuilderTable.SnapOverlapKey);
			result.piece = (long)pieceId;
			result.piece <<= 32;
			result.piece |= (long)attachGridIndex;
			result.otherPiece = (long)otherPieceId;
			result.otherPiece <<= 32;
			result.otherPiece |= (long)otherAttachGridIndex;
			return result;
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x00175940 File Offset: 0x00173B40
		private bool BuildTableFromJson(string tableJson, bool fromTitleData)
		{
			if (string.IsNullOrEmpty(tableJson))
			{
				return false;
			}
			this.tableData = null;
			try
			{
				this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
			}
			catch
			{
			}
			try
			{
				if (this.tableData == null)
				{
					tableJson = GZipStream.UncompressString(Convert.FromBase64String(tableJson));
					this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
			if (this.tableData == null)
			{
				return false;
			}
			if (this.tableData.version < 4)
			{
				return false;
			}
			int num = (this.tableData.pieceType == null) ? 0 : this.tableData.pieceType.Count;
			if (num == 0)
			{
				this.OnDeserializeUpdatePlots();
				return true;
			}
			if (this.tableData.pieceId == null || this.tableData.pieceId.Count != num || this.tableData.placement == null || this.tableData.placement.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch", null);
				return false;
			}
			if (num >= this.maxResources[0])
			{
				GTDev.LogError<string>(string.Format("BuildTableFromJson Failed sanity piece count check {0}", num), null);
				return false;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>(num);
			bool flag = this.tableData.timeOffset != null && this.tableData.timeOffset.Count > 0;
			if (flag && this.tableData.timeOffset.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch (Time Offsets)", null);
				return false;
			}
			int i = 0;
			while (i < this.tableData.pieceType.Count)
			{
				int num2 = this.CreatePieceId();
				if (!dictionary.TryAdd(this.tableData.pieceId[i], num2))
				{
					GTDev.LogError<string>("BuildTableFromJson Piece id duplicate in save", null);
					this.ClearTable();
					return false;
				}
				int num3 = (this.tableData.materialType != null && this.tableData.materialType.Count > i) ? this.tableData.materialType[i] : -1;
				int newPieceType = this.tableData.pieceType[i];
				int num4 = num3;
				bool flag2 = true;
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.tableData.pieceType[i]);
				if (piecePrefab == null)
				{
					this.ClearTable();
					return false;
				}
				if (fromTitleData)
				{
					goto IL_2B2;
				}
				if (num4 == -1 && piecePrefab.materialOptions != null)
				{
					int num5;
					Material material;
					int num6;
					piecePrefab.materialOptions.GetDefaultMaterial(out num5, out material, out num6);
					num4 = num5;
				}
				flag2 = BuilderSetManager.instance.IsPieceOwnedLocally(this.tableData.pieceType[i], num4);
				if (!fromTitleData && !flag2)
				{
					if (!piecePrefab.fallbackInfo.materialSwapThisPrefab)
					{
						if (piecePrefab.fallbackInfo.prefab == null)
						{
							goto IL_3E0;
						}
						newPieceType = piecePrefab.fallbackInfo.prefab.name.GetStaticHash();
					}
					num4 = -1;
				}
				goto IL_2B2;
				IL_3E0:
				i++;
				continue;
				IL_2B2:
				if (piecePrefab.cost != null && piecePrefab.cost.quantities != null)
				{
					for (int j = 0; j < piecePrefab.cost.quantities.Count; j++)
					{
						BuilderResourceQuantity builderResourceQuantity = piecePrefab.cost.quantities[j];
						if (!this.HasEnoughResource(builderResourceQuantity))
						{
							if (builderResourceQuantity.type == BuilderResourceType.Basic)
							{
								this.ClearTable();
								GTDev.LogError<string>("BuildTableFromJson saved table uses too many basic resource", null);
								return false;
							}
							GTDev.LogWarning<string>("BuildTableFromJson saved table uses too many functional or decorative resource", null);
						}
					}
				}
				int num7 = flag ? this.tableData.timeOffset[i] : 0;
				BuilderPiece builderPiece = this.CreatePieceInternal(newPieceType, num2, Vector3.zero, Quaternion.identity, BuilderPiece.State.AttachedAndPlaced, num4, NetworkSystem.Instance.ServerTimestamp - num7, this);
				if (builderPiece == null)
				{
					this.ClearTable();
					GTDev.LogError<string>(string.Format("Piece Type {0} is not defined", this.tableData.pieceType[i]), null);
					return false;
				}
				if (!fromTitleData && !flag2)
				{
					builderPiece.overrideSavedPiece = true;
					builderPiece.savedPieceType = this.tableData.pieceType[i];
					builderPiece.savedMaterialType = num3;
				}
				goto IL_3E0;
			}
			for (int k = 0; k < this.tableData.pieceType.Count; k++)
			{
				int parentAttachIndex = (this.tableData.parentAttachIndex == null || this.tableData.parentAttachIndex.Count <= k) ? -1 : this.tableData.parentAttachIndex[k];
				int attachIndex = (this.tableData.attachIndex == null || this.tableData.attachIndex.Count <= k) ? -1 : this.tableData.attachIndex[k];
				int valueOrDefault = dictionary.GetValueOrDefault(this.tableData.pieceId[k], -1);
				int parentId = -1;
				int num8;
				if (dictionary.TryGetValue(this.tableData.parentId[k], out num8))
				{
					parentId = num8;
				}
				else if (this.tableData.parentId[k] < 10000 && this.tableData.parentId[k] >= 5)
				{
					parentId = this.tableData.parentId[k];
				}
				this.AttachPieceInternal(valueOrDefault, attachIndex, parentId, parentAttachIndex, this.tableData.placement[k]);
			}
			foreach (BuilderPiece builderPiece2 in this.pieces)
			{
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece2.OnPlacementDeserialized();
				}
			}
			this.OnDeserializeUpdatePlots();
			BuilderTable.tempDuplicateOverlaps.Clear();
			if (this.tableData.overlapingPieces != null)
			{
				int num9 = 0;
				while (num9 < this.tableData.overlapingPieces.Count && num9 < this.tableData.overlappedPieces.Count && num9 < this.tableData.overlapInfo.Count)
				{
					int num10 = -1;
					int num11;
					if (dictionary.TryGetValue(this.tableData.overlapingPieces[num9], out num11))
					{
						num10 = num11;
					}
					else if (this.tableData.overlapingPieces[num9] < 10000 && this.tableData.overlapingPieces[num9] >= 5)
					{
						num10 = this.tableData.overlapingPieces[num9];
					}
					int num12 = -1;
					int num13;
					if (dictionary.TryGetValue(this.tableData.overlappedPieces[num9], out num13))
					{
						num12 = num13;
					}
					else if (this.tableData.overlappedPieces[num9] < 10000 && this.tableData.overlappedPieces[num9] >= 5)
					{
						num12 = this.tableData.overlappedPieces[num9];
					}
					if (num10 != -1 && num12 != -1)
					{
						long packed = this.tableData.overlapInfo[num9];
						BuilderPiece piece = this.GetPiece(num10);
						if (!(piece == null))
						{
							BuilderPiece piece2 = this.GetPiece(num12);
							if (!(piece2 == null))
							{
								int num14;
								int num15;
								Vector2Int min;
								Vector2Int max;
								this.UnpackSnapInfo(packed, out num14, out num15, out min, out max);
								if (num14 >= 0 && num14 < piece.gridPlanes.Count && num15 >= 0 && num15 < piece2.gridPlanes.Count)
								{
									BuilderTable.SnapOverlapKey item = BuilderTable.BuildOverlapKey(num10, num12, num14, num15);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(item))
									{
										BuilderTable.tempDuplicateOverlaps.Add(item);
										piece.gridPlanes[num14].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num15], new SnapBounds(min, max)));
									}
								}
							}
						}
					}
					num9++;
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			return true;
		}

		// Token: 0x06004BF6 RID: 19446 RVA: 0x00176128 File Offset: 0x00174328
		public int SerializeTableState(byte[] bytes, int maxBytes)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.conveyors == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.conveyors.Count);
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					int selectedSetID = builderConveyor.GetSelectedSetID();
					binaryWriter.Write(selectedSetID);
				}
			}
			if (this.dispenserShelves == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.dispenserShelves.Count);
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					int selectedSetID2 = builderDispenserShelf.GetSelectedSetID();
					binaryWriter.Write(selectedSetID2);
				}
			}
			BuilderTable.childPieces.Clear();
			BuilderTable.rootPieces.Clear();
			BuilderTable.childPieces.EnsureCapacity(this.pieces.Count);
			BuilderTable.rootPieces.EnsureCapacity(this.pieces.Count);
			foreach (BuilderPiece builderPiece in this.pieces)
			{
				if (builderPiece.parentPiece == null)
				{
					BuilderTable.rootPieces.Add(builderPiece);
				}
				else
				{
					BuilderTable.childPieces.Add(builderPiece);
				}
			}
			binaryWriter.Write(BuilderTable.rootPieces.Count);
			for (int i = 0; i < BuilderTable.rootPieces.Count; i++)
			{
				BuilderPiece builderPiece2 = BuilderTable.rootPieces[i];
				binaryWriter.Write(builderPiece2.pieceType);
				binaryWriter.Write(builderPiece2.pieceId);
				binaryWriter.Write((byte)builderPiece2.state);
				if (builderPiece2.state == BuilderPiece.State.OnConveyor || builderPiece2.state == BuilderPiece.State.OnShelf || builderPiece2.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece2.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece2.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece2.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece2.materialType);
				long value = BitPackUtils.PackWorldPosForNetwork(builderPiece2.transform.localPosition);
				int value2 = BitPackUtils.PackQuaternionForNetwork(builderPiece2.transform.localRotation);
				binaryWriter.Write(value);
				binaryWriter.Write(value2);
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece2.functionalPieceState);
					binaryWriter.Write(builderPiece2.activatedTimeStamp);
				}
				if (builderPiece2.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece2));
				}
			}
			binaryWriter.Write(BuilderTable.childPieces.Count);
			for (int j = 0; j < BuilderTable.childPieces.Count; j++)
			{
				BuilderPiece builderPiece3 = BuilderTable.childPieces[j];
				binaryWriter.Write(builderPiece3.pieceType);
				binaryWriter.Write(builderPiece3.pieceId);
				int value3 = (builderPiece3.parentPiece == null) ? -1 : builderPiece3.parentPiece.pieceId;
				binaryWriter.Write(value3);
				binaryWriter.Write(builderPiece3.attachIndex);
				binaryWriter.Write(builderPiece3.parentAttachIndex);
				binaryWriter.Write((byte)builderPiece3.state);
				if (builderPiece3.state == BuilderPiece.State.OnConveyor || builderPiece3.state == BuilderPiece.State.OnShelf || builderPiece3.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece3.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece3.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece3.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece3.materialType);
				int piecePlacement = builderPiece3.GetPiecePlacement();
				binaryWriter.Write(piecePlacement);
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece3.functionalPieceState);
					binaryWriter.Write(builderPiece3.activatedTimeStamp);
				}
				if (builderPiece3.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece3));
				}
			}
			if (this.isTableMutable)
			{
				binaryWriter.Write(this.plotOwners.Count);
				using (Dictionary<int, int>.Enumerator enumerator4 = this.plotOwners.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<int, int> keyValuePair = enumerator4.Current;
						binaryWriter.Write(keyValuePair.Key);
						binaryWriter.Write(keyValuePair.Value);
					}
					goto IL_4F9;
				}
			}
			if (this.sharedBlocksMap == null || this.sharedBlocksMap.MapID == null || !SharedBlocksManager.IsMapIDValid(this.sharedBlocksMap.MapID))
			{
				for (int k = 0; k < BuilderTable.mapIDBuffer.Length; k++)
				{
					BuilderTable.mapIDBuffer[k] = 'a';
				}
			}
			else
			{
				for (int l = 0; l < BuilderTable.mapIDBuffer.Length; l++)
				{
					BuilderTable.mapIDBuffer[l] = this.sharedBlocksMap.MapID[l];
				}
			}
			binaryWriter.Write(BuilderTable.mapIDBuffer);
			IL_4F9:
			long position = memoryStream.Position;
			BuilderTable.overlapPieces.Clear();
			BuilderTable.overlapOtherPieces.Clear();
			BuilderTable.overlapPacked.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			foreach (BuilderPiece builderPiece4 in this.pieces)
			{
				if (!(builderPiece4 == null))
				{
					for (int m = 0; m < builderPiece4.gridPlanes.Count; m++)
					{
						if (!(builderPiece4.gridPlanes[m] == null))
						{
							for (SnapOverlap snapOverlap = builderPiece4.gridPlanes[m].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								BuilderTable.SnapOverlapKey item = BuilderTable.BuildOverlapKey(builderPiece4.pieceId, snapOverlap.otherPlane.piece.pieceId, m, snapOverlap.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(item))
								{
									BuilderTable.tempDuplicateOverlaps.Add(item);
									long item2 = this.PackSnapInfo(m, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece4.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(item2);
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece5 in this.basePieces)
			{
				if (!(builderPiece5 == null))
				{
					for (int n = 0; n < builderPiece5.gridPlanes.Count; n++)
					{
						if (!(builderPiece5.gridPlanes[n] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece5.gridPlanes[n].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								BuilderTable.SnapOverlapKey item3 = BuilderTable.BuildOverlapKey(builderPiece5.pieceId, snapOverlap2.otherPlane.piece.pieceId, n, snapOverlap2.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(item3))
								{
									BuilderTable.tempDuplicateOverlaps.Add(item3);
									long item4 = this.PackSnapInfo(n, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece5.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(item4);
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			binaryWriter.Write(BuilderTable.overlapPieces.Count);
			for (int num = 0; num < BuilderTable.overlapPieces.Count; num++)
			{
				binaryWriter.Write(BuilderTable.overlapPieces[num]);
				binaryWriter.Write(BuilderTable.overlapOtherPieces[num]);
				binaryWriter.Write(BuilderTable.overlapPacked[num]);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x06004BF7 RID: 19447 RVA: 0x00176A10 File Offset: 0x00174C10
		public void DeserializeTableState(byte[] bytes, int numBytes)
		{
			if (numBytes <= 0)
			{
				return;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentPeiceIds.Clear();
			BuilderTable.tempAttachIndexes.Clear();
			BuilderTable.tempParentAttachIndexes.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			BuilderTable.tempPiecePlacement.Clear();
			int num = binaryReader.ReadInt32();
			bool flag = this.conveyors != null;
			for (int i = 0; i < num; i++)
			{
				int selection = binaryReader.ReadInt32();
				if (flag && i < this.conveyors.Count)
				{
					this.conveyors[i].SetSelection(selection);
				}
			}
			int num2 = binaryReader.ReadInt32();
			bool flag2 = this.dispenserShelves != null;
			for (int j = 0; j < num2; j++)
			{
				int selection2 = binaryReader.ReadInt32();
				if (flag2 && j < this.dispenserShelves.Count)
				{
					this.dispenserShelves[j].SetSelection(selection2);
				}
			}
			int num3 = binaryReader.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				int newPieceType = binaryReader.ReadInt32();
				int num4 = binaryReader.ReadInt32();
				BuilderPiece.State state = (BuilderPiece.State)binaryReader.ReadByte();
				int num5 = binaryReader.ReadInt32();
				bool item = binaryReader.ReadByte() > 0;
				int materialType = binaryReader.ReadInt32();
				long data = binaryReader.ReadInt64();
				int data2 = binaryReader.ReadInt32();
				Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
				Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data2);
				byte fState = (state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0;
				int activateTimeStamp = (state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0;
				int num6 = (state == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0;
				float num7 = 10000f;
				if (!vector.IsValid(num7) || !quaternion.IsValid() || !this.ValidateCreatePieceParams(newPieceType, num4, state, materialType))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num8 = -1;
				if (state == BuilderPiece.State.OnConveyor || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
				{
					num8 = num5;
					num5 = -1;
				}
				if (this.ValidateDeserializedRootPieceState(num4, state, num8, num5, vector, quaternion))
				{
					BuilderPiece builderPiece = this.CreatePieceInternal(newPieceType, num4, vector, quaternion, state, materialType, activateTimeStamp, this);
					BuilderTable.tempPeiceIds.Add(num4);
					BuilderTable.tempParentActorNumbers.Add(num5);
					BuilderTable.tempInLeftHand.Add(item);
					builderPiece.SetFunctionalPieceState(fState, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					if (num8 >= 0 && this.isTableMutable)
					{
						builderPiece.shelfOwner = num8;
						if (state == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor = this.conveyors[num8];
							float timeOffset = 0f;
							if (PhotonNetwork.ServerTimestamp > num6)
							{
								timeOffset = (PhotonNetwork.ServerTimestamp - num6) / 1000f;
							}
							builderConveyor.OnShelfPieceCreated(builderPiece, timeOffset);
						}
						else if (state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num8].OnShelfPieceCreated(builderPiece, false);
						}
					}
				}
			}
			for (int l = 0; l < BuilderTable.tempPeiceIds.Count; l++)
			{
				if (BuilderTable.tempParentActorNumbers[l] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[l], BuilderTable.tempParentActorNumbers[l], BuilderTable.tempInLeftHand[l]);
				}
			}
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			int num9 = binaryReader.ReadInt32();
			for (int m = 0; m < num9; m++)
			{
				int newPieceType2 = binaryReader.ReadInt32();
				int num10 = binaryReader.ReadInt32();
				int item2 = binaryReader.ReadInt32();
				int item3 = binaryReader.ReadInt32();
				int item4 = binaryReader.ReadInt32();
				BuilderPiece.State state2 = (BuilderPiece.State)binaryReader.ReadByte();
				int num11 = binaryReader.ReadInt32();
				bool item5 = binaryReader.ReadByte() > 0;
				int materialType2 = binaryReader.ReadInt32();
				int item6 = binaryReader.ReadInt32();
				byte fState2 = (state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0;
				int activateTimeStamp2 = (state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0;
				int num12 = (state2 == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0;
				if (!this.ValidateCreatePieceParams(newPieceType2, num10, state2, materialType2))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num13 = -1;
				if (state2 == BuilderPiece.State.OnConveyor || state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
				{
					num13 = num11;
					num11 = -1;
				}
				if (this.ValidateDeserializedChildPieceState(num10, state2))
				{
					BuilderPiece builderPiece2 = this.CreatePieceInternal(newPieceType2, num10, this.roomCenter.position, Quaternion.identity, state2, materialType2, activateTimeStamp2, this);
					builderPiece2.SetFunctionalPieceState(fState2, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					BuilderTable.tempPeiceIds.Add(num10);
					BuilderTable.tempParentPeiceIds.Add(item2);
					BuilderTable.tempAttachIndexes.Add(item3);
					BuilderTable.tempParentAttachIndexes.Add(item4);
					BuilderTable.tempParentActorNumbers.Add(num11);
					BuilderTable.tempInLeftHand.Add(item5);
					BuilderTable.tempPiecePlacement.Add(item6);
					if (num13 >= 0 && this.isTableMutable)
					{
						builderPiece2.shelfOwner = num13;
						if (state2 == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor2 = this.conveyors[num13];
							float timeOffset2 = 0f;
							if (PhotonNetwork.ServerTimestamp > num12)
							{
								timeOffset2 = (PhotonNetwork.ServerTimestamp - num12) / 1000f;
							}
							builderConveyor2.OnShelfPieceCreated(builderPiece2, timeOffset2);
						}
						else if (state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num13].OnShelfPieceCreated(builderPiece2, false);
						}
					}
				}
			}
			for (int n = 0; n < BuilderTable.tempPeiceIds.Count; n++)
			{
				if (!this.ValidateAttachPieceParams(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]))
				{
					this.RecyclePieceInternal(BuilderTable.tempPeiceIds[n], true, false, -1);
				}
				else
				{
					this.AttachPieceInternal(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]);
				}
			}
			for (int num14 = 0; num14 < BuilderTable.tempPeiceIds.Count; num14++)
			{
				if (BuilderTable.tempParentActorNumbers[num14] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[num14], BuilderTable.tempParentActorNumbers[num14], BuilderTable.tempInLeftHand[num14]);
				}
			}
			foreach (BuilderPiece builderPiece3 in this.pieces)
			{
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece3.OnPlacementDeserialized();
				}
			}
			if (this.isTableMutable)
			{
				this.plotOwners.Clear();
				this.doesLocalPlayerOwnPlot = false;
				int num15 = binaryReader.ReadInt32();
				for (int num16 = 0; num16 < num15; num16++)
				{
					int num17 = binaryReader.ReadInt32();
					int num18 = binaryReader.ReadInt32();
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (this.plotOwners.TryAdd(num17, num18) && this.GetPiece(num18).TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						builderPiecePrivatePlot.ClaimPlotForPlayerNumber(num17);
						if (num17 == PhotonNetwork.LocalPlayer.ActorNumber)
						{
							this.doesLocalPlayerOwnPlot = true;
						}
					}
				}
				UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
				if (onLocalPlayerClaimedPlot != null)
				{
					onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
				}
				this.OnDeserializeUpdatePlots();
			}
			else
			{
				BuilderTable.mapIDBuffer = binaryReader.ReadChars(BuilderTable.mapIDBuffer.Length);
				string mapID = new string(BuilderTable.mapIDBuffer);
				if (SharedBlocksManager.IsMapIDValid(mapID))
				{
					this.sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = mapID
					};
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			int num19 = binaryReader.ReadInt32();
			for (int num20 = 0; num20 < num19; num20++)
			{
				int pieceId = binaryReader.ReadInt32();
				int num21 = binaryReader.ReadInt32();
				long packed = binaryReader.ReadInt64();
				BuilderPiece piece = this.GetPiece(pieceId);
				if (!(piece == null))
				{
					BuilderPiece piece2 = this.GetPiece(num21);
					if (!(piece2 == null))
					{
						int num22;
						int num23;
						Vector2Int min;
						Vector2Int max;
						this.UnpackSnapInfo(packed, out num22, out num23, out min, out max);
						if (num22 >= 0 && num22 < piece.gridPlanes.Count && num23 >= 0 && num23 < piece2.gridPlanes.Count)
						{
							BuilderTable.SnapOverlapKey item7 = BuilderTable.BuildOverlapKey(pieceId, num21, num22, num23);
							if (!BuilderTable.tempDuplicateOverlaps.Contains(item7))
							{
								BuilderTable.tempDuplicateOverlaps.Add(item7);
								piece.gridPlanes[num22].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num23], new SnapBounds(min, max)));
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
		}

		// Token: 0x04005410 RID: 21520
		public const GTZone BUILDER_ZONE = GTZone.monkeBlocks;

		// Token: 0x04005411 RID: 21521
		private const int INITIAL_BUILTIN_PIECE_ID = 5;

		// Token: 0x04005412 RID: 21522
		private const int INITIAL_CREATED_PIECE_ID = 10000;

		// Token: 0x04005413 RID: 21523
		public static float MAX_DROP_VELOCITY = 20f;

		// Token: 0x04005414 RID: 21524
		public static float MAX_DROP_ANG_VELOCITY = 50f;

		// Token: 0x04005415 RID: 21525
		private const float MAX_DISTANCE_FROM_CENTER = 217f;

		// Token: 0x04005416 RID: 21526
		private const float MAX_LOCAL_MAGNITUDE = 80f;

		// Token: 0x04005417 RID: 21527
		public const float MAX_DISTANCE_FROM_HAND = 2.5f;

		// Token: 0x04005418 RID: 21528
		public static float DROP_ZONE_REPEL = 2.25f;

		// Token: 0x04005419 RID: 21529
		public static int placedLayer;

		// Token: 0x0400541A RID: 21530
		public static int heldLayer;

		// Token: 0x0400541B RID: 21531
		public static int heldLayerLocal;

		// Token: 0x0400541C RID: 21532
		public static int droppedLayer;

		// Token: 0x0400541D RID: 21533
		private float acceptableSqrDistFromCenter = 47089f;

		// Token: 0x0400541E RID: 21534
		public float pieceScale = 0.04f;

		// Token: 0x0400541F RID: 21535
		public GTZone tableZone = GTZone.monkeBlocks;

		// Token: 0x04005420 RID: 21536
		[SerializeField]
		private string SharedMapConfigTitleDataKey = "SharedBlocksStartingMapConfig";

		// Token: 0x04005421 RID: 21537
		public BuilderTableNetworking builderNetworking;

		// Token: 0x04005422 RID: 21538
		public BuilderRenderer builderRenderer;

		// Token: 0x04005423 RID: 21539
		[HideInInspector]
		public BuilderPool builderPool;

		// Token: 0x04005424 RID: 21540
		public Transform tableCenter;

		// Token: 0x04005425 RID: 21541
		public Transform roomCenter;

		// Token: 0x04005426 RID: 21542
		public Transform worldCenter;

		// Token: 0x04005427 RID: 21543
		public GameObject noBlocksArea;

		// Token: 0x04005428 RID: 21544
		public List<GameObject> builtInPieceRoots;

		// Token: 0x04005429 RID: 21545
		[Tooltip("Optional terminal to control loaded blocks")]
		public SharedBlocksTerminal linkedTerminal;

		// Token: 0x0400542A RID: 21546
		[Tooltip("Can Blocks Be Placed and Grabbed")]
		public bool isTableMutable;

		// Token: 0x0400542B RID: 21547
		public GameObject shelvesRoot;

		// Token: 0x0400542C RID: 21548
		public GameObject dropZoneRoot;

		// Token: 0x0400542D RID: 21549
		public List<GameObject> recyclerRoot;

		// Token: 0x0400542E RID: 21550
		public List<GameObject> allShelvesRoot;

		// Token: 0x0400542F RID: 21551
		[NonSerialized]
		public List<BuilderConveyor> conveyors = new List<BuilderConveyor>();

		// Token: 0x04005430 RID: 21552
		[NonSerialized]
		public List<BuilderDispenserShelf> dispenserShelves = new List<BuilderDispenserShelf>();

		// Token: 0x04005431 RID: 21553
		public BuilderConveyorManager conveyorManager;

		// Token: 0x04005432 RID: 21554
		public List<BuilderResourceMeter> resourceMeters;

		// Token: 0x04005433 RID: 21555
		public GameObject sharedBuildArea;

		// Token: 0x04005434 RID: 21556
		private BoxCollider[] sharedBuildAreas;

		// Token: 0x04005435 RID: 21557
		public BuilderPiece armShelfPieceType;

		// Token: 0x04005436 RID: 21558
		[NonSerialized]
		public List<BuilderRecycler> recyclers;

		// Token: 0x04005437 RID: 21559
		[NonSerialized]
		public List<BuilderDropZone> dropZones;

		// Token: 0x04005438 RID: 21560
		private int shelfSliceUpdateIndex;

		// Token: 0x04005439 RID: 21561
		public static int SHELF_SLICE_BUCKETS = 6;

		// Token: 0x0400543A RID: 21562
		public float defaultTint = 1f;

		// Token: 0x0400543B RID: 21563
		public float droppedTint = 0.75f;

		// Token: 0x0400543C RID: 21564
		public float grabbedTint = 0.75f;

		// Token: 0x0400543D RID: 21565
		public float shelfTint = 1f;

		// Token: 0x0400543E RID: 21566
		public float potentialGrabTint = 0.75f;

		// Token: 0x0400543F RID: 21567
		public float paintingTint = 0.6f;

		// Token: 0x04005440 RID: 21568
		private List<BuilderTable.BoxCheckParams> noBlocksAreas;

		// Token: 0x04005441 RID: 21569
		private Collider[] noBlocksCheckResults = new Collider[64];

		// Token: 0x04005442 RID: 21570
		public LayerMask allPiecesMask;

		// Token: 0x04005443 RID: 21571
		public bool useSnapRotation;

		// Token: 0x04005444 RID: 21572
		public BuilderPlacementStyle usePlacementStyle;

		// Token: 0x04005445 RID: 21573
		public BuilderOptionButton buttonSnapRotation;

		// Token: 0x04005446 RID: 21574
		public BuilderOptionButton buttonSnapPosition;

		// Token: 0x04005447 RID: 21575
		public BuilderOptionButton buttonSaveLayout;

		// Token: 0x04005448 RID: 21576
		public BuilderOptionButton buttonClearLayout;

		// Token: 0x04005449 RID: 21577
		[HideInInspector]
		public List<BuilderAttachGridPlane> baseGridPlanes;

		// Token: 0x0400544A RID: 21578
		private List<BuilderPiece> basePieces;

		// Token: 0x0400544B RID: 21579
		[HideInInspector]
		public List<BuilderPiecePrivatePlot> allPrivatePlots;

		// Token: 0x0400544C RID: 21580
		private int nextPieceId;

		// Token: 0x0400544D RID: 21581
		[HideInInspector]
		public List<BuilderTable.BuildPieceSpawn> buildPieceSpawns;

		// Token: 0x0400544E RID: 21582
		[HideInInspector]
		public List<BuilderShelf> shelves;

		// Token: 0x0400544F RID: 21583
		[NonSerialized]
		public List<BuilderPiece> pieces = new List<BuilderPiece>(1024);

		// Token: 0x04005450 RID: 21584
		private Dictionary<int, int> pieceIDToIndexCache = new Dictionary<int, int>(1024);

		// Token: 0x04005451 RID: 21585
		[HideInInspector]
		public Dictionary<int, int> plotOwners;

		// Token: 0x04005452 RID: 21586
		private bool doesLocalPlayerOwnPlot;

		// Token: 0x04005453 RID: 21587
		public Dictionary<int, int> playerToArmShelfLeft;

		// Token: 0x04005454 RID: 21588
		public Dictionary<int, int> playerToArmShelfRight;

		// Token: 0x04005455 RID: 21589
		private HashSet<int> builderPiecesVisited = new HashSet<int>(128);

		// Token: 0x04005456 RID: 21590
		public BuilderResources totalResources;

		// Token: 0x04005457 RID: 21591
		[Tooltip("Resources reserved for conveyors and dispensers")]
		public BuilderResources totalReservedResources;

		// Token: 0x04005458 RID: 21592
		public BuilderResources resourcesPerPrivatePlot;

		// Token: 0x04005459 RID: 21593
		[NonSerialized]
		public int[] maxResources;

		// Token: 0x0400545A RID: 21594
		private int[] plotMaxResources;

		// Token: 0x0400545B RID: 21595
		[NonSerialized]
		public int[] usedResources;

		// Token: 0x0400545C RID: 21596
		[NonSerialized]
		public int[] reservedResources;

		// Token: 0x0400545D RID: 21597
		private List<int> playersInBuilder;

		// Token: 0x0400545E RID: 21598
		private List<IBuilderPieceFunctional> activeFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x0400545F RID: 21599
		private List<IBuilderPieceFunctional> funcComponentsToRegister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04005460 RID: 21600
		private List<IBuilderPieceFunctional> funcComponentsToUnregister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04005461 RID: 21601
		private List<IBuilderPieceFunctional> fixedUpdateFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04005462 RID: 21602
		private List<IBuilderPieceFunctional> funcComponentsToRegisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04005463 RID: 21603
		private List<IBuilderPieceFunctional> funcComponentsToUnregisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04005464 RID: 21604
		private const int MAX_SPHERE_CHECK_RESULTS = 1024;

		// Token: 0x04005465 RID: 21605
		private NativeList<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x04005466 RID: 21606
		private NativeList<BuilderGridPlaneData> checkGridPlaneData;

		// Token: 0x04005467 RID: 21607
		private NativeArray<ColliderHit> nearbyPiecesResults;

		// Token: 0x04005468 RID: 21608
		private NativeArray<OverlapSphereCommand> nearbyPiecesCommands;

		// Token: 0x04005469 RID: 21609
		private List<BuilderPotentialPlacement> allPotentialPlacements;

		// Token: 0x0400546A RID: 21610
		private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

		// Token: 0x0400546B RID: 21611
		private BuilderTable.TableState tableState;

		// Token: 0x0400546C RID: 21612
		private bool inRoom;

		// Token: 0x0400546D RID: 21613
		private bool inBuilderZone;

		// Token: 0x0400546E RID: 21614
		private static int DROPPED_PIECE_LIMIT = 100;

		// Token: 0x0400546F RID: 21615
		public static string nextUpdateOverride = string.Empty;

		// Token: 0x04005470 RID: 21616
		private List<BuilderPiece> droppedPieces;

		// Token: 0x04005471 RID: 21617
		private List<BuilderTable.DroppedPieceData> droppedPieceData;

		// Token: 0x04005472 RID: 21618
		private HashSet<int>[] repelledPieceRoots;

		// Token: 0x04005473 RID: 21619
		private int repelHistoryLength = 3;

		// Token: 0x04005474 RID: 21620
		private int repelHistoryIndex;

		// Token: 0x04005475 RID: 21621
		private bool hasRequestedConfig;

		// Token: 0x04005476 RID: 21622
		private bool isDirty;

		// Token: 0x04005477 RID: 21623
		private bool saveInProgress;

		// Token: 0x04005478 RID: 21624
		private int currentSaveSlot = -1;

		// Token: 0x04005479 RID: 21625
		[HideInInspector]
		public UnityEvent OnSaveTimeUpdated;

		// Token: 0x0400547A RID: 21626
		[HideInInspector]
		public UnityEvent<bool> OnSaveDirtyChanged;

		// Token: 0x0400547B RID: 21627
		[HideInInspector]
		public UnityEvent OnSaveSuccess;

		// Token: 0x0400547C RID: 21628
		[HideInInspector]
		public UnityEvent<string> OnSaveFailure;

		// Token: 0x0400547D RID: 21629
		[HideInInspector]
		public UnityEvent OnTableConfigurationUpdated;

		// Token: 0x0400547E RID: 21630
		[HideInInspector]
		public UnityEvent<bool> OnLocalPlayerClaimedPlot;

		// Token: 0x0400547F RID: 21631
		[HideInInspector]
		public UnityEvent OnMapCleared;

		// Token: 0x04005480 RID: 21632
		[HideInInspector]
		public UnityEvent<string> OnMapLoaded;

		// Token: 0x04005481 RID: 21633
		[HideInInspector]
		public UnityEvent<string> OnMapLoadFailed;

		// Token: 0x04005482 RID: 21634
		private List<BuilderTable.BuilderCommand> queuedBuildCommands;

		// Token: 0x04005483 RID: 21635
		private List<BuilderAction> rollBackActions;

		// Token: 0x04005484 RID: 21636
		private List<BuilderTable.BuilderCommand> rollBackBufferedCommands;

		// Token: 0x04005485 RID: 21637
		private List<BuilderTable.BuilderCommand> rollForwardCommands;

		// Token: 0x04005486 RID: 21638
		private static Dictionary<GTZone, BuilderTable> zoneToInstance;

		// Token: 0x04005487 RID: 21639
		private bool isSetup;

		// Token: 0x04005488 RID: 21640
		public BuilderTable.SnapParams pushAndEaseParams;

		// Token: 0x04005489 RID: 21641
		public BuilderTable.SnapParams overlapParams;

		// Token: 0x0400548A RID: 21642
		private BuilderTable.SnapParams currSnapParams;

		// Token: 0x0400548B RID: 21643
		public int maxPlacementChildDepth = 5;

		// Token: 0x0400548C RID: 21644
		private static List<BuilderPiece> tempPieces = new List<BuilderPiece>(256);

		// Token: 0x0400548D RID: 21645
		private static List<BuilderConveyor> tempConveyors = new List<BuilderConveyor>(256);

		// Token: 0x0400548E RID: 21646
		private static List<BuilderDispenserShelf> tempDispensers = new List<BuilderDispenserShelf>(256);

		// Token: 0x0400548F RID: 21647
		private static List<BuilderRecycler> tempRecyclers = new List<BuilderRecycler>(5);

		// Token: 0x04005490 RID: 21648
		private static List<BuilderTable.BuilderCommand> tempRollForwardCommands = new List<BuilderTable.BuilderCommand>(128);

		// Token: 0x04005491 RID: 21649
		private static List<BuilderPiece> tempDeletePieces = new List<BuilderPiece>(1024);

		// Token: 0x04005492 RID: 21650
		public const int MAX_PIECE_DATA = 2560;

		// Token: 0x04005493 RID: 21651
		public const int MAX_GRID_PLANE_DATA = 10240;

		// Token: 0x04005494 RID: 21652
		public const int MAX_PRIVATE_PLOT_DATA = 64;

		// Token: 0x04005495 RID: 21653
		public const int MAX_PLAYER_DATA = 64;

		// Token: 0x04005496 RID: 21654
		private BuilderTableData tableData;

		// Token: 0x04005497 RID: 21655
		private int fetchConfigurationAttempts;

		// Token: 0x04005498 RID: 21656
		private int maxRetries = 3;

		// Token: 0x04005499 RID: 21657
		private SharedBlocksManager.SharedBlocksMap sharedBlocksMap;

		// Token: 0x0400549A RID: 21658
		private string pendingMapID;

		// Token: 0x0400549B RID: 21659
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x0400549C RID: 21660
		private List<SharedBlocksManager.SharedBlocksMap> startingMapList = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x0400549D RID: 21661
		private SharedBlocksManager.SharedBlocksMap startingMap;

		// Token: 0x0400549E RID: 21662
		private bool hasStartingMap;

		// Token: 0x0400549F RID: 21663
		private double startingMapCacheTime = double.MinValue;

		// Token: 0x040054A0 RID: 21664
		private bool getStartingMapInProgress;

		// Token: 0x040054A1 RID: 21665
		private bool hasCachedTopMaps;

		// Token: 0x040054A2 RID: 21666
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x040054A3 RID: 21667
		private static string personalBuildKey = "MyBuild";

		// Token: 0x040054A4 RID: 21668
		private static HashSet<BuilderTable.SnapOverlapKey> tempDuplicateOverlaps = new HashSet<BuilderTable.SnapOverlapKey>(16384);

		// Token: 0x040054A5 RID: 21669
		private static List<BuilderPiece> childPieces = new List<BuilderPiece>(4096);

		// Token: 0x040054A6 RID: 21670
		private static List<BuilderPiece> rootPieces = new List<BuilderPiece>(4096);

		// Token: 0x040054A7 RID: 21671
		private static List<int> overlapPieces = new List<int>(4096);

		// Token: 0x040054A8 RID: 21672
		private static List<int> overlapOtherPieces = new List<int>(4096);

		// Token: 0x040054A9 RID: 21673
		private static List<long> overlapPacked = new List<long>(4096);

		// Token: 0x040054AA RID: 21674
		private static char[] mapIDBuffer = new char[8];

		// Token: 0x040054AB RID: 21675
		private static Dictionary<long, int> snapOverlapSanity = new Dictionary<long, int>(16384);

		// Token: 0x040054AC RID: 21676
		private static List<int> tempPeiceIds = new List<int>(4096);

		// Token: 0x040054AD RID: 21677
		private static List<int> tempParentPeiceIds = new List<int>(4096);

		// Token: 0x040054AE RID: 21678
		private static List<int> tempAttachIndexes = new List<int>(4096);

		// Token: 0x040054AF RID: 21679
		private static List<int> tempParentAttachIndexes = new List<int>(4096);

		// Token: 0x040054B0 RID: 21680
		private static List<int> tempParentActorNumbers = new List<int>(4096);

		// Token: 0x040054B1 RID: 21681
		private static List<bool> tempInLeftHand = new List<bool>(4096);

		// Token: 0x040054B2 RID: 21682
		private static List<int> tempPiecePlacement = new List<int>(4096);

		// Token: 0x02000C0B RID: 3083
		private struct BoxCheckParams
		{
			// Token: 0x040054B3 RID: 21683
			public Vector3 center;

			// Token: 0x040054B4 RID: 21684
			public Vector3 halfExtents;

			// Token: 0x040054B5 RID: 21685
			public Quaternion rotation;
		}

		// Token: 0x02000C0C RID: 3084
		[Serializable]
		public class BuildPieceSpawn
		{
			// Token: 0x040054B6 RID: 21686
			public GameObject buildPiecePrefab;

			// Token: 0x040054B7 RID: 21687
			public int count = 1;
		}

		// Token: 0x02000C0D RID: 3085
		public enum BuilderCommandType
		{
			// Token: 0x040054B9 RID: 21689
			Create,
			// Token: 0x040054BA RID: 21690
			Place,
			// Token: 0x040054BB RID: 21691
			Grab,
			// Token: 0x040054BC RID: 21692
			Drop,
			// Token: 0x040054BD RID: 21693
			Remove,
			// Token: 0x040054BE RID: 21694
			Paint,
			// Token: 0x040054BF RID: 21695
			Recycle,
			// Token: 0x040054C0 RID: 21696
			ClaimPlot,
			// Token: 0x040054C1 RID: 21697
			FreePlot,
			// Token: 0x040054C2 RID: 21698
			CreateArmShelf,
			// Token: 0x040054C3 RID: 21699
			PlayerLeftRoom,
			// Token: 0x040054C4 RID: 21700
			FunctionalStateChange,
			// Token: 0x040054C5 RID: 21701
			SetSelection,
			// Token: 0x040054C6 RID: 21702
			Repel
		}

		// Token: 0x02000C0E RID: 3086
		public enum TableState
		{
			// Token: 0x040054C8 RID: 21704
			WaitingForZoneAndRoom,
			// Token: 0x040054C9 RID: 21705
			WaitingForInitalBuild,
			// Token: 0x040054CA RID: 21706
			ReceivingInitialBuild,
			// Token: 0x040054CB RID: 21707
			WaitForInitialBuildMaster,
			// Token: 0x040054CC RID: 21708
			WaitForMasterResync,
			// Token: 0x040054CD RID: 21709
			ReceivingMasterResync,
			// Token: 0x040054CE RID: 21710
			InitialBuild,
			// Token: 0x040054CF RID: 21711
			ExecuteQueuedCommands,
			// Token: 0x040054D0 RID: 21712
			Ready,
			// Token: 0x040054D1 RID: 21713
			BadData,
			// Token: 0x040054D2 RID: 21714
			WaitingForSharedMapLoad
		}

		// Token: 0x02000C0F RID: 3087
		public enum DroppedPieceState
		{
			// Token: 0x040054D4 RID: 21716
			None = -1,
			// Token: 0x040054D5 RID: 21717
			Light,
			// Token: 0x040054D6 RID: 21718
			Heavy,
			// Token: 0x040054D7 RID: 21719
			Frozen
		}

		// Token: 0x02000C10 RID: 3088
		private struct DroppedPieceData
		{
			// Token: 0x040054D8 RID: 21720
			public BuilderTable.DroppedPieceState droppedState;

			// Token: 0x040054D9 RID: 21721
			public float speedThreshCrossedTime;

			// Token: 0x040054DA RID: 21722
			public float filteredSpeed;
		}

		// Token: 0x02000C11 RID: 3089
		public struct BuilderCommand
		{
			// Token: 0x040054DB RID: 21723
			public BuilderTable.BuilderCommandType type;

			// Token: 0x040054DC RID: 21724
			public int pieceType;

			// Token: 0x040054DD RID: 21725
			public int pieceId;

			// Token: 0x040054DE RID: 21726
			public int attachPieceId;

			// Token: 0x040054DF RID: 21727
			public int parentPieceId;

			// Token: 0x040054E0 RID: 21728
			public int parentAttachIndex;

			// Token: 0x040054E1 RID: 21729
			public int attachIndex;

			// Token: 0x040054E2 RID: 21730
			public Vector3 localPosition;

			// Token: 0x040054E3 RID: 21731
			public Quaternion localRotation;

			// Token: 0x040054E4 RID: 21732
			public byte twist;

			// Token: 0x040054E5 RID: 21733
			public sbyte bumpOffsetX;

			// Token: 0x040054E6 RID: 21734
			public sbyte bumpOffsetZ;

			// Token: 0x040054E7 RID: 21735
			public Vector3 velocity;

			// Token: 0x040054E8 RID: 21736
			public Vector3 angVelocity;

			// Token: 0x040054E9 RID: 21737
			public bool isLeft;

			// Token: 0x040054EA RID: 21738
			public int materialType;

			// Token: 0x040054EB RID: 21739
			public NetPlayer player;

			// Token: 0x040054EC RID: 21740
			public BuilderPiece.State state;

			// Token: 0x040054ED RID: 21741
			public bool isQueued;

			// Token: 0x040054EE RID: 21742
			public bool canRollback;

			// Token: 0x040054EF RID: 21743
			public int localCommandId;

			// Token: 0x040054F0 RID: 21744
			public int serverTimeStamp;
		}

		// Token: 0x02000C12 RID: 3090
		[Serializable]
		public struct SnapParams
		{
			// Token: 0x040054F1 RID: 21745
			public float minOffsetY;

			// Token: 0x040054F2 RID: 21746
			public float maxOffsetY;

			// Token: 0x040054F3 RID: 21747
			public float maxUpDotProduct;

			// Token: 0x040054F4 RID: 21748
			public float maxTwistDotProduct;

			// Token: 0x040054F5 RID: 21749
			public float snapAttachDistance;

			// Token: 0x040054F6 RID: 21750
			public float snapDelayTime;

			// Token: 0x040054F7 RID: 21751
			public float snapDelayOffsetDist;

			// Token: 0x040054F8 RID: 21752
			public float unSnapDelayTime;

			// Token: 0x040054F9 RID: 21753
			public float unSnapDelayDist;

			// Token: 0x040054FA RID: 21754
			public float maxBlockSnapDist;
		}

		// Token: 0x02000C13 RID: 3091
		private struct SnapOverlapKey
		{
			// Token: 0x06004BFB RID: 19451 RVA: 0x001775D1 File Offset: 0x001757D1
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int>(this.piece.GetHashCode(), this.otherPiece.GetHashCode());
			}

			// Token: 0x06004BFC RID: 19452 RVA: 0x001775EE File Offset: 0x001757EE
			public bool Equals(BuilderTable.SnapOverlapKey other)
			{
				return this.piece == other.piece && this.otherPiece == other.otherPiece;
			}

			// Token: 0x06004BFD RID: 19453 RVA: 0x0017760E File Offset: 0x0017580E
			public override bool Equals(object o)
			{
				return o is BuilderTable.SnapOverlapKey && this.Equals((BuilderTable.SnapOverlapKey)o);
			}

			// Token: 0x040054FB RID: 21755
			public long piece;

			// Token: 0x040054FC RID: 21756
			public long otherPiece;
		}
	}
}
