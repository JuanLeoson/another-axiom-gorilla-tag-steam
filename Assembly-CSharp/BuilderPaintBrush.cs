using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public class BuilderPaintBrush : HoldableObject
{
	// Token: 0x06001F98 RID: 8088 RVA: 0x000A6D28 File Offset: 0x000A4F28
	private void Awake()
	{
		this.pieceLayers |= 1 << LayerMask.NameToLayer("Gorilla Object");
		this.pieceLayers |= 1 << LayerMask.NameToLayer("BuilderProp");
		this.pieceLayers |= 1 << LayerMask.NameToLayer("Prop");
		this.paintDistance = Vector3.SqrMagnitude(this.paintVolumeHalfExtents);
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x000A6DC4 File Offset: 0x000A4FC4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.holdingHand = grabbingHand;
		this.handVelocity = grabbingHand.GetComponent<GorillaVelocityTracker>();
		if (this.handVelocity == null)
		{
			Debug.Log("No Velocity Estimator");
		}
		this.inLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		if (this.inLeftHand)
		{
			base.transform.SetParent(myBodyDockPositions.leftHandTransform, true);
		}
		else
		{
			base.transform.SetParent(myBodyDockPositions.rightHandTransform, true);
		}
		base.transform.localScale = Vector3.one;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.inLeftHand);
		GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		this.brushState = BuilderPaintBrush.PaintBrushState.Held;
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000A6EC4 File Offset: 0x000A50C4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (base.OnRelease(zoneReleased, releasingHand))
		{
			this.holdingHand = null;
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.inLeftHand);
			this.inLeftHand = false;
			this.handVelocity = null;
			this.ClearHoveredPiece();
			base.transform.parent = null;
			base.transform.localScale = Vector3.one;
			this.rb.isKinematic = false;
			this.rb.velocity = Vector3.zero;
			this.rb.angularVelocity = Vector3.zero;
			this.rb.useGravity = true;
			return true;
		}
		return false;
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x000A6F63 File Offset: 0x000A5163
	private void LateUpdate()
	{
		if (this.brushState == BuilderPaintBrush.PaintBrushState.Inactive)
		{
			return;
		}
		if (this.holdingHand == null || this.materialType == -1)
		{
			this.brushState = BuilderPaintBrush.PaintBrushState.Inactive;
			return;
		}
		this.FindPieceToPaint();
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000A6F94 File Offset: 0x000A5194
	private void FindPieceToPaint()
	{
		switch (this.brushState)
		{
		case BuilderPaintBrush.PaintBrushState.Held:
		{
			if (this.materialType == -1)
			{
				return;
			}
			Array.Clear(this.hitColliders, 0, this.hitColliders.Length);
			int num = Physics.OverlapBoxNonAlloc(this.brushSurface.transform.position - this.brushSurface.up * this.paintVolumeHalfExtents.y, this.paintVolumeHalfExtents, this.hitColliders, this.brushSurface.transform.rotation, this.pieceLayers, QueryTriggerInteraction.Ignore);
			BuilderPieceCollider builderPieceCollider = null;
			Collider collider = null;
			float num2 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				BuilderPieceCollider component = this.hitColliders[i].GetComponent<BuilderPieceCollider>();
				if (component != null && component.piece.materialType != this.materialType && component.piece.materialType != -1)
				{
					float sqrMagnitude = (this.brushSurface.transform.position - component.transform.position).sqrMagnitude;
					if (sqrMagnitude < num2 && component.piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, component.piece.transform.position))
					{
						num2 = sqrMagnitude;
						builderPieceCollider = component;
						collider = this.hitColliders[i];
					}
				}
			}
			if (builderPieceCollider != null)
			{
				this.ClearHoveredPiece();
				this.hoveredPiece = builderPieceCollider.piece;
				this.hoveredPieceCollider = collider;
				this.hoveredPiece.PaintingTint(true);
				GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
				this.positionDelta = 0f;
				this.lastPosition = this.brushSurface.transform.position;
				this.brushState = BuilderPaintBrush.PaintBrushState.Hover;
				return;
			}
			break;
		}
		case BuilderPaintBrush.PaintBrushState.Hover:
		{
			if (this.hoveredPiece == null || this.hoveredPieceCollider == null)
			{
				this.ClearHoveredPiece();
				return;
			}
			float sqrMagnitude2 = this.handVelocity.GetLatestVelocity(false).sqrMagnitude;
			float sqrMagnitude3 = this.handVelocity.GetAverageVelocity(false, 0.15f, false).sqrMagnitude;
			if (this.handVelocity != null && (sqrMagnitude2 > this.maxPaintVelocitySqrMag || sqrMagnitude3 > this.maxPaintVelocitySqrMag))
			{
				this.ClearHoveredPiece();
				return;
			}
			Vector3 vector = this.brushSurface.position - this.brushSurface.up * this.paintVolumeHalfExtents.y;
			Vector3 b = this.hoveredPieceCollider.ClosestPointOnBounds(vector);
			if (Vector3.SqrMagnitude(vector - b) > this.paintDistance)
			{
				this.ClearHoveredPiece();
				return;
			}
			GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, Time.deltaTime);
			float num3 = Vector3.Distance(this.lastPosition, this.brushSurface.position);
			if (num3 < this.minimumWiggleFrameDistance)
			{
				this.lastPosition = this.brushSurface.position;
				return;
			}
			this.positionDelta += Math.Min(num3, this.maximumWiggleFrameDistance);
			this.lastPosition = this.brushSurface.position;
			if (this.positionDelta >= this.wiggleDistanceRequirement)
			{
				this.positionDelta = 0f;
				this.audioSource.clip = this.paintSound;
				this.audioSource.GTPlay();
				this.PaintPiece();
				this.brushState = BuilderPaintBrush.PaintBrushState.JustPainted;
				return;
			}
			break;
		}
		case BuilderPaintBrush.PaintBrushState.JustPainted:
			if (this.paintTimeElapsed > this.paintDelay)
			{
				this.paintTimeElapsed = 0f;
				this.brushState = BuilderPaintBrush.PaintBrushState.Held;
				return;
			}
			this.paintTimeElapsed += Time.deltaTime;
			break;
		default:
			return;
		}
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000A735C File Offset: 0x000A555C
	private void PaintPiece()
	{
		this.hoveredPiece.GetTable().RequestPaintPiece(this.hoveredPiece.pieceId, this.materialType);
		this.hoveredPiece.PaintingTint(false);
		this.hoveredPiece = null;
		this.hoveredPieceCollider = null;
		this.paintTimeElapsed = 0f;
		GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x000A73DC File Offset: 0x000A55DC
	private void ClearHoveredPiece()
	{
		if (this.hoveredPiece != null)
		{
			this.hoveredPiece.PaintingTint(false);
		}
		this.hoveredPiece = null;
		this.hoveredPieceCollider = null;
		this.positionDelta = 0f;
		this.brushState = ((this.holdingHand == null || this.materialType == -1) ? BuilderPaintBrush.PaintBrushState.Inactive : BuilderPaintBrush.PaintBrushState.Held);
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000A7440 File Offset: 0x000A5640
	public void SetBrushMaterial(int inMaterialType)
	{
		this.materialType = inMaterialType;
		this.audioSource.clip = this.paintSound;
		this.audioSource.GTPlay();
		if (this.holdingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.inLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
		if (this.materialType == -1)
		{
			this.ClearHoveredPiece();
		}
		else if (this.brushState == BuilderPaintBrush.PaintBrushState.Inactive && this.holdingHand != null)
		{
			this.brushState = BuilderPaintBrush.PaintBrushState.Held;
		}
		if (this.paintBrushMaterialOptions != null && this.brushRenderer != null)
		{
			Material material;
			int num;
			this.paintBrushMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.brushRenderer.material = material;
			}
		}
	}

	// Token: 0x04002829 RID: 10281
	[SerializeField]
	private Transform brushSurface;

	// Token: 0x0400282A RID: 10282
	[SerializeField]
	private Vector3 paintVolumeHalfExtents;

	// Token: 0x0400282B RID: 10283
	[SerializeField]
	private BuilderMaterialOptions paintBrushMaterialOptions;

	// Token: 0x0400282C RID: 10284
	[SerializeField]
	private MeshRenderer brushRenderer;

	// Token: 0x0400282D RID: 10285
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400282E RID: 10286
	[SerializeField]
	private AudioClip paintSound;

	// Token: 0x0400282F RID: 10287
	[SerializeField]
	private AudioClip brushStrokeSound;

	// Token: 0x04002830 RID: 10288
	private GameObject holdingHand;

	// Token: 0x04002831 RID: 10289
	private bool inLeftHand;

	// Token: 0x04002832 RID: 10290
	private GorillaVelocityTracker handVelocity;

	// Token: 0x04002833 RID: 10291
	private BuilderPiece hoveredPiece;

	// Token: 0x04002834 RID: 10292
	private Collider hoveredPieceCollider;

	// Token: 0x04002835 RID: 10293
	private Collider[] hitColliders = new Collider[16];

	// Token: 0x04002836 RID: 10294
	private LayerMask pieceLayers = 0;

	// Token: 0x04002837 RID: 10295
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x04002838 RID: 10296
	private float positionDelta;

	// Token: 0x04002839 RID: 10297
	private float wiggleDistanceRequirement = 0.08f;

	// Token: 0x0400283A RID: 10298
	private float minimumWiggleFrameDistance = 0.005f;

	// Token: 0x0400283B RID: 10299
	private float maximumWiggleFrameDistance = 0.04f;

	// Token: 0x0400283C RID: 10300
	private float maxPaintVelocitySqrMag = 0.5f;

	// Token: 0x0400283D RID: 10301
	private float paintDelay = 0.2f;

	// Token: 0x0400283E RID: 10302
	private float paintTimeElapsed = -1f;

	// Token: 0x0400283F RID: 10303
	private float paintDistance;

	// Token: 0x04002840 RID: 10304
	private int materialType = -1;

	// Token: 0x04002841 RID: 10305
	private BuilderPaintBrush.PaintBrushState brushState;

	// Token: 0x04002842 RID: 10306
	private Rigidbody rb;

	// Token: 0x0200050F RID: 1295
	public enum PaintBrushState
	{
		// Token: 0x04002844 RID: 10308
		Inactive,
		// Token: 0x04002845 RID: 10309
		HeldRemote,
		// Token: 0x04002846 RID: 10310
		Held,
		// Token: 0x04002847 RID: 10311
		Hover,
		// Token: 0x04002848 RID: 10312
		JustPainted
	}
}
