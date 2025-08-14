using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C87 RID: 3207
	public class BuilderMovingSnapPiece : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004F55 RID: 20309 RVA: 0x0018A5C0 File Offset: 0x001887C0
		private void Awake()
		{
			this.myPiece = base.GetComponent<BuilderPiece>();
			if (this.myPiece == null)
			{
				Debug.LogWarning("Missing BuilderPiece component " + base.gameObject.name);
			}
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				builderMovingPart.myPiece = this.myPiece;
			}
		}

		// Token: 0x06004F56 RID: 20310 RVA: 0x0018A64C File Offset: 0x0018884C
		public int GetTimeOffset()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return 0;
			}
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				if (!builderMovingPart.IsAnchoredToTable())
				{
					return builderMovingPart.GetTimeOffsetMS();
				}
			}
			return 0;
		}

		// Token: 0x06004F57 RID: 20311 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06004F58 RID: 20312 RVA: 0x0018A6BC File Offset: 0x001888BC
		public void OnPieceDestroy()
		{
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				builderMovingPart.OnPieceDestroy();
			}
		}

		// Token: 0x06004F59 RID: 20313 RVA: 0x0018A70C File Offset: 0x0018890C
		public void OnPiecePlacementDeserialized()
		{
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				builderMovingPart.InitMovingGrid();
				builderMovingPart.SetMoving(false);
				if (this.myPiece.functionalPieceState == 0 && !builderMovingPart.IsAnchoredToTable())
				{
					this.currentPauseNode = builderMovingPart.GetStartNode();
				}
			}
			this.moving = false;
			if (!this.activated)
			{
				BuilderTable table = this.myPiece.GetTable();
				table.RegisterFunctionalPiece(this);
				table.RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
			this.OnStateChanged(this.myPiece.functionalPieceState, NetworkSystem.Instance.MasterClient, this.myPiece.activatedTimeStamp);
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x0018A7DC File Offset: 0x001889DC
		public void OnPieceActivate()
		{
			BuilderTable table = this.myPiece.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready && table.GetTableState() != BuilderTable.TableState.ExecuteQueuedCommands)
			{
				return;
			}
			if (!this.activated)
			{
				table.RegisterFunctionalPiece(this);
				table.RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				builderMovingPart.InitMovingGrid();
				if (!builderMovingPart.IsAnchoredToTable())
				{
					int num = 0;
					foreach (BuilderAttachGridPlane builderAttachGridPlane in builderMovingPart.myGridPlanes)
					{
						num += builderAttachGridPlane.GetChildCount();
					}
					if (num <= 5)
					{
						this.currentPauseNode = builderMovingPart.GetStartNode();
						if (this.myPiece.functionalPieceState > 0 && (int)this.myPiece.functionalPieceState < BuilderMovingPart.NUM_PAUSE_NODES * 2 + 1)
						{
							this.currentPauseNode = this.myPiece.functionalPieceState - 1;
						}
						this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.MasterClient, this.myPiece.activatedTimeStamp);
					}
					else
					{
						this.currentPauseNode = builderMovingPart.GetStartNode();
						if (this.myPiece.functionalPieceState > 0 && (int)this.myPiece.functionalPieceState < BuilderMovingPart.NUM_PAUSE_NODES * 2 + 1)
						{
							this.currentPauseNode = this.myPiece.functionalPieceState - 1;
						}
						this.myPiece.SetFunctionalPieceState(this.currentPauseNode + 1, NetworkSystem.Instance.MasterClient, this.myPiece.activatedTimeStamp);
					}
				}
			}
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x0018A990 File Offset: 0x00188B90
		public void OnPieceDeactivate()
		{
			BuilderTable table = this.myPiece.GetTable();
			table.UnregisterFunctionalPiece(this);
			table.UnregisterFunctionalPieceFixedUpdate(this);
			this.myPiece.functionalPieceState = 0;
			this.moving = false;
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				builderMovingPart.SetMoving(false);
			}
			this.activated = false;
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x0018AA14 File Offset: 0x00188C14
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (!this.activated)
			{
				return;
			}
			if (newState == 0 && !this.moving)
			{
				this.moving = true;
				if (this.startMovingFX != null)
				{
					ObjectPools.instance.Instantiate(this.startMovingFX, base.transform.position, true);
				}
				using (List<BuilderMovingPart>.Enumerator enumerator = this.MovingParts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BuilderMovingPart builderMovingPart = enumerator.Current;
						if (!builderMovingPart.IsAnchoredToTable())
						{
							builderMovingPart.ActivateAtNode(this.currentPauseNode, timeStamp);
							this.currentPauseNode = builderMovingPart.GetStartNode();
						}
					}
					return;
				}
			}
			if (this.moving && this.stopMovingFX != null)
			{
				ObjectPools.instance.Instantiate(this.stopMovingFX, base.transform.position, true);
			}
			this.moving = false;
			this.currentPauseNode = newState - 1;
			foreach (BuilderMovingPart builderMovingPart2 in this.MovingParts)
			{
				if (!builderMovingPart2.IsAnchoredToTable())
				{
					builderMovingPart2.PauseMovement(this.currentPauseNode);
				}
			}
		}

		// Token: 0x06004F5D RID: 20317 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
		}

		// Token: 0x06004F5E RID: 20318 RVA: 0x0018AB7C File Offset: 0x00188D7C
		public bool IsStateValid(byte state)
		{
			return (int)state <= BuilderMovingPart.NUM_PAUSE_NODES * 2 + 1;
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x0018AB8D File Offset: 0x00188D8D
		public void FunctionalPieceUpdate()
		{
			this.UpdateMaster();
		}

		// Token: 0x06004F60 RID: 20320 RVA: 0x0018AB98 File Offset: 0x00188D98
		public void FunctionalPieceFixedUpdate()
		{
			if (!this.moving)
			{
				return;
			}
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				if (!builderMovingPart.IsAnchoredToTable())
				{
					builderMovingPart.UpdateMovingGrid();
				}
			}
		}

		// Token: 0x06004F61 RID: 20321 RVA: 0x0018ABFC File Offset: 0x00188DFC
		private void UpdateMaster()
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.myPiece.GetTable();
			foreach (BuilderMovingPart builderMovingPart in this.MovingParts)
			{
				if (!builderMovingPart.IsAnchoredToTable())
				{
					int num = 0;
					foreach (BuilderAttachGridPlane builderAttachGridPlane in builderMovingPart.myGridPlanes)
					{
						num += builderAttachGridPlane.GetChildCount();
					}
					bool flag = num <= 5;
					if (flag && !this.moving)
					{
						table.builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 0, PhotonNetwork.MasterClient, NetworkSystem.Instance.ServerTimestamp);
					}
					if (!flag && this.moving)
					{
						byte state = builderMovingPart.GetNearestNode() + 1;
						table.builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, state, PhotonNetwork.MasterClient, NetworkSystem.Instance.ServerTimestamp);
					}
				}
			}
		}

		// Token: 0x04005845 RID: 22597
		public List<BuilderMovingPart> MovingParts;

		// Token: 0x04005846 RID: 22598
		public BuilderPiece myPiece;

		// Token: 0x04005847 RID: 22599
		public const int MAX_MOVING_CHILDREN = 5;

		// Token: 0x04005848 RID: 22600
		[SerializeField]
		private GameObject startMovingFX;

		// Token: 0x04005849 RID: 22601
		[SerializeField]
		private GameObject stopMovingFX;

		// Token: 0x0400584A RID: 22602
		private bool activated;

		// Token: 0x0400584B RID: 22603
		private bool moving;

		// Token: 0x0400584C RID: 22604
		private const byte MOVING_STATE = 0;

		// Token: 0x0400584D RID: 22605
		private byte currentPauseNode;
	}
}
