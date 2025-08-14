using System;
using System.Collections.Generic;
using GorillaLocomotion.Gameplay;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BF8 RID: 3064
	[RequireComponent(typeof(Collider))]
	public class BuilderPieceHandHold : MonoBehaviour, IGorillaGrabable, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06004A7B RID: 19067 RVA: 0x00169DAD File Offset: 0x00167FAD
		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.myCollider = base.GetComponent<Collider>();
			this.initialized = true;
		}

		// Token: 0x06004A7C RID: 19068 RVA: 0x00169DCB File Offset: 0x00167FCB
		public bool IsHandHoldMoving()
		{
			return this.myPiece.IsPieceMoving();
		}

		// Token: 0x06004A7D RID: 19069 RVA: 0x00169DD8 File Offset: 0x00167FD8
		public bool MomentaryGrabOnly()
		{
			return this.forceMomentary;
		}

		// Token: 0x06004A7E RID: 19070 RVA: 0x00169DE0 File Offset: 0x00167FE0
		public virtual bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && (!this.myPiece.GetTable().isTableMutable || grabber.Player.scale < 0.5f);
		}

		// Token: 0x06004A7F RID: 19071 RVA: 0x00169E18 File Offset: 0x00168018
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Initialize();
			grabbedTransform = base.transform;
			Vector3 position = grabber.transform.position;
			localGrabbedPosition = base.transform.InverseTransformPoint(position);
			this.activeGrabbers.Add(grabber);
			this.isGrabbed = true;
			Vector3 vector;
			grabber.Player.AddHandHold(base.transform, localGrabbedPosition, grabber, grabber.IsRightHand, false, out vector);
		}

		// Token: 0x06004A80 RID: 19072 RVA: 0x00169E85 File Offset: 0x00168085
		public void OnGrabReleased(GorillaGrabber grabber)
		{
			this.Initialize();
			this.activeGrabbers.Remove(grabber);
			this.isGrabbed = (this.activeGrabbers.Count < 1);
			grabber.Player.RemoveHandHold(grabber, grabber.IsRightHand);
		}

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06004A81 RID: 19073 RVA: 0x00169EC0 File Offset: 0x001680C0
		// (set) Token: 0x06004A82 RID: 19074 RVA: 0x00169EC8 File Offset: 0x001680C8
		public bool TickRunning { get; set; }

		// Token: 0x06004A83 RID: 19075 RVA: 0x00169ED4 File Offset: 0x001680D4
		public void Tick()
		{
			if (!this.isGrabbed)
			{
				return;
			}
			foreach (GorillaGrabber gorillaGrabber in this.activeGrabbers)
			{
				if (gorillaGrabber != null && gorillaGrabber.Player.scale > 0.5f)
				{
					this.OnGrabReleased(gorillaGrabber);
				}
			}
		}

		// Token: 0x06004A84 RID: 19076 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06004A85 RID: 19077 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004A86 RID: 19078 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004A87 RID: 19079 RVA: 0x00169F4C File Offset: 0x0016814C
		public void OnPieceActivate()
		{
			if (!this.TickRunning && this.myPiece.GetTable().isTableMutable)
			{
				TickSystem<object>.AddCallbackTarget(this);
			}
		}

		// Token: 0x06004A88 RID: 19080 RVA: 0x00169F70 File Offset: 0x00168170
		public void OnPieceDeactivate()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
			}
			foreach (GorillaGrabber grabber in this.activeGrabbers)
			{
				this.OnGrabReleased(grabber);
			}
		}

		// Token: 0x06004A8A RID: 19082 RVA: 0x000139A7 File Offset: 0x00011BA7
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x0400536D RID: 21357
		private bool initialized;

		// Token: 0x0400536E RID: 21358
		private Collider myCollider;

		// Token: 0x0400536F RID: 21359
		[SerializeField]
		private bool forceMomentary = true;

		// Token: 0x04005370 RID: 21360
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04005371 RID: 21361
		private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>(2);

		// Token: 0x04005372 RID: 21362
		private bool isGrabbed;
	}
}
