using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000C92 RID: 3218
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(GorillaSurfaceOverride))]
	public class BuilderPieceTappable : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004FBE RID: 20414 RVA: 0x0018D93E File Offset: 0x0018BB3E
		public virtual bool CanTap()
		{
			return this.isPieceActive && Time.time > this.lastTapTime + this.tapCooldown;
		}

		// Token: 0x06004FBF RID: 20415 RVA: 0x0018D95E File Offset: 0x0018BB5E
		public void OnTapLocal(float tapStrength)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (!this.CanTap())
			{
				return;
			}
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
		}

		// Token: 0x06004FC0 RID: 20416 RVA: 0x000023F5 File Offset: 0x000005F5
		public virtual void OnTapReplicated()
		{
		}

		// Token: 0x06004FC1 RID: 20417 RVA: 0x0018D997 File Offset: 0x0018BB97
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderPieceTappable.FunctionalState.Idle;
		}

		// Token: 0x06004FC2 RID: 20418 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004FC3 RID: 20419 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004FC4 RID: 20420 RVA: 0x0018D9A0 File Offset: 0x0018BBA0
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
		}

		// Token: 0x06004FC5 RID: 20421 RVA: 0x0018D9AC File Offset: 0x0018BBAC
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderPieceTappable.FunctionalState.Tap)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06004FC6 RID: 20422 RVA: 0x0018D9FC File Offset: 0x0018BBFC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderPieceTappable.FunctionalState.Tap)
			{
				this.lastTapTime = Time.time;
				this.OnTapReplicated();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderPieceTappable.FunctionalState)newState;
		}

		// Token: 0x06004FC7 RID: 20423 RVA: 0x0018DA4C File Offset: 0x0018BC4C
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if (newState == 1 && this.CanTap())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06004FC8 RID: 20424 RVA: 0x0018DAA7 File Offset: 0x0018BCA7
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06004FC9 RID: 20425 RVA: 0x0018DAB0 File Offset: 0x0018BCB0
		public void FunctionalPieceUpdate()
		{
			if (this.lastTapTime + this.tapCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x040058D8 RID: 22744
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x040058D9 RID: 22745
		[SerializeField]
		protected float tapCooldown = 0.5f;

		// Token: 0x040058DA RID: 22746
		private bool isPieceActive;

		// Token: 0x040058DB RID: 22747
		private float lastTapTime;

		// Token: 0x040058DC RID: 22748
		private BuilderPieceTappable.FunctionalState currentState;

		// Token: 0x02000C93 RID: 3219
		private enum FunctionalState
		{
			// Token: 0x040058DE RID: 22750
			Idle,
			// Token: 0x040058DF RID: 22751
			Tap
		}
	}
}
