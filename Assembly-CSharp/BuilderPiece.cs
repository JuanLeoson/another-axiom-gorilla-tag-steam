using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class BuilderPiece : MonoBehaviour
{
	// Token: 0x06002066 RID: 8294 RVA: 0x000AB5A0 File Offset: 0x000A97A0
	private void Awake()
	{
		if (this.vFXInfo == null)
		{
			Debug.LogErrorFormat("BuilderPiece {0} is missing Effect Info", new object[]
			{
				base.gameObject.name
			});
		}
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.placedOnlyColliders = new List<Collider>(4);
		List<Collider> list = new List<Collider>(4);
		foreach (GameObject gameObject in this.onlyWhenPlaced)
		{
			list.Clear();
			gameObject.GetComponentsInChildren<Collider>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isTrigger)
				{
					BuilderPieceCollider builderPieceCollider = list[i].GetComponent<BuilderPieceCollider>();
					if (builderPieceCollider == null)
					{
						builderPieceCollider = list[i].AddComponent<BuilderPieceCollider>();
					}
					builderPieceCollider.piece = this;
					this.placedOnlyColliders.Add(list[i]);
				}
			}
		}
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		for (int j = this.colliders.Count - 1; j >= 0; j--)
		{
			if (this.colliders[j].isTrigger)
			{
				this.colliders.RemoveAt(j);
			}
			else
			{
				BuilderPieceCollider builderPieceCollider2 = this.colliders[j].GetComponent<BuilderPieceCollider>();
				if (builderPieceCollider2 == null)
				{
					builderPieceCollider2 = this.colliders[j].AddComponent<BuilderPieceCollider>();
				}
				builderPieceCollider2.piece = this;
			}
		}
		this.gridPlanes = new List<BuilderAttachGridPlane>(8);
		base.GetComponentsInChildren<BuilderAttachGridPlane>(this.gridPlanes);
		this.pieceComponents = new List<IBuilderPieceComponent>(1);
		base.GetComponentsInChildren<IBuilderPieceComponent>(true, this.pieceComponents);
		this.pieceComponentsActive = false;
		this.functionalPieceComponent = base.GetComponentInChildren<IBuilderPieceFunctional>();
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		this.renderingIndirect = new List<MeshRenderer>(4);
		this.renderingDirect = new List<MeshRenderer>(4);
		this.FindActiveRenderers();
		this.paintingCount = 0;
		this.potentialGrabCount = 0;
		this.potentialGrabChildCount = 0;
		this.isPrivatePlot = (this.plotComponent != null);
		this.privatePlotIndex = -1;
		this.ClearCollisionHistory();
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000AB85C File Offset: 0x000A9A5C
	public void SetTable(BuilderTable table)
	{
		this.tableOwner = table;
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000AB865 File Offset: 0x000A9A65
	public BuilderTable GetTable()
	{
		return this.tableOwner;
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000AB870 File Offset: 0x000A9A70
	public void OnReturnToPool()
	{
		this.tableOwner.builderRenderer.RemovePiece(this);
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceDestroy();
		}
		this.functionalPieceState = 0;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.overrideSavedPiece = false;
		this.savedMaterialType = -1;
		this.savedPieceType = -1;
		this.shelfOwner = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.activatedTimeStamp = 0;
		this.forcedFrozen = false;
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		base.transform.localScale = Vector3.one;
		if (this.isArmShelf)
		{
			if (this.armShelf != null)
			{
				this.armShelf.piece = null;
			}
			this.armShelf = null;
		}
		for (int j = 0; j < this.gridPlanes.Count; j++)
		{
			this.gridPlanes[j].OnReturnToPool(this.tableOwner.builderPool);
		}
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000AB9F2 File Offset: 0x000A9BF2
	public void OnCreatedByPool()
	{
		this.materialSwapTargets = new List<MeshRenderer>(4);
		base.GetComponentsInChildren<MeshRenderer>(this.areMeshesToggledOnPlace, this.materialSwapTargets);
		this.surfaceOverrides = new List<GorillaSurfaceOverride>(4);
		base.GetComponentsInChildren<GorillaSurfaceOverride>(this.areMeshesToggledOnPlace, this.surfaceOverrides);
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000ABA30 File Offset: 0x000A9C30
	public void SetupPiece(float gridSize)
	{
		for (int i = 0; i < this.gridPlanes.Count; i++)
		{
			this.gridPlanes[i].Setup(this, i, gridSize);
		}
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000ABA68 File Offset: 0x000A9C68
	public void SetMaterial(int inMaterialType, bool force = false)
	{
		if (this.materialOptions == null || this.materialSwapTargets == null || this.materialSwapTargets.Count < 1)
		{
			return;
		}
		if (this.materialType == inMaterialType && !force)
		{
			return;
		}
		this.materialType = inMaterialType;
		Material material = null;
		int num = -1;
		if (inMaterialType == -1)
		{
			this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
		}
		else
		{
			this.materialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material == null)
			{
				this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
			}
		}
		if (material == null)
		{
			Debug.LogErrorFormat("Piece {0} has no material matching Type {1}", new object[]
			{
				this.GetPieceId(),
				inMaterialType
			});
			return;
		}
		foreach (MeshRenderer meshRenderer in this.materialSwapTargets)
		{
			if (!(meshRenderer == null) && meshRenderer.enabled)
			{
				meshRenderer.material = material;
			}
		}
		if (this.surfaceOverrides != null && num != -1)
		{
			foreach (GorillaSurfaceOverride gorillaSurfaceOverride in this.surfaceOverrides)
			{
				gorillaSurfaceOverride.overrideIndex = num;
			}
		}
		if (this.renderingIndirect.Count > 0)
		{
			this.tableOwner.builderRenderer.ChangePieceIndirectMaterial(this, this.materialSwapTargets, material);
		}
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x000ABBFC File Offset: 0x000A9DFC
	public int GetPieceId()
	{
		return this.pieceId;
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x000ABC04 File Offset: 0x000A9E04
	public int GetParentPieceId()
	{
		if (!(this.parentPiece == null))
		{
			return this.parentPiece.pieceId;
		}
		return -1;
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000ABC21 File Offset: 0x000A9E21
	public int GetAttachIndex()
	{
		return this.attachIndex;
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000ABC29 File Offset: 0x000A9E29
	public int GetParentAttachIndex()
	{
		return this.parentAttachIndex;
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000ABC34 File Offset: 0x000A9E34
	private void SetPieceActive(List<IBuilderPieceComponent> components, bool active)
	{
		if (components == null || active == this.pieceComponentsActive)
		{
			return;
		}
		this.pieceComponentsActive = active;
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				if (active)
				{
					components[i].OnPieceActivate();
				}
				else
				{
					components[i].OnPieceDeactivate();
				}
			}
		}
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000ABC8C File Offset: 0x000A9E8C
	private void SetBehavioursEnabled<T>(List<T> components, bool enabled) where T : Behaviour
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06002073 RID: 8307 RVA: 0x000ABCD4 File Offset: 0x000A9ED4
	private void SetCollidersEnabled<T>(List<T> components, bool enabled) where T : Collider
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x000ABD1C File Offset: 0x000A9F1C
	public void SetColliderLayers<T>(List<T> components, int layer) where T : Collider
	{
		this.currentColliderLayer = layer;
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].gameObject.layer = layer;
			}
		}
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x000ABD70 File Offset: 0x000A9F70
	private void SetActive(List<GameObject> gameObjects, bool active)
	{
		if (gameObjects == null)
		{
			return;
		}
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] != null)
			{
				gameObjects[i].SetActive(active);
			}
		}
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x000ABDAE File Offset: 0x000A9FAE
	public void SetFunctionalPieceState(byte fState, NetPlayer instigator, int timeStamp)
	{
		if (this.functionalPieceComponent == null || !this.functionalPieceComponent.IsStateValid(fState))
		{
			fState = 0;
		}
		this.functionalPieceState = fState;
		IBuilderPieceFunctional builderPieceFunctional = this.functionalPieceComponent;
		if (builderPieceFunctional == null)
		{
			return;
		}
		builderPieceFunctional.OnStateChanged(fState, instigator, timeStamp);
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x000ABDE3 File Offset: 0x000A9FE3
	public void SetScale(float scale)
	{
		if (this.scaleRoot != null)
		{
			this.scaleRoot.localScale = Vector3.one * scale;
		}
		this.pieceScale = scale;
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x000ABE10 File Offset: 0x000AA010
	public float GetScale()
	{
		return this.pieceScale;
	}

	// Token: 0x06002079 RID: 8313 RVA: 0x000ABE18 File Offset: 0x000AA018
	public void PaintingTint(bool enable)
	{
		if (enable)
		{
			this.paintingCount++;
			if (this.paintingCount == 1)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.paintingCount--;
			if (this.paintingCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x000ABE58 File Offset: 0x000AA058
	public void PotentialGrab(bool enable)
	{
		if (enable)
		{
			this.potentialGrabCount++;
			if (this.potentialGrabCount == 1 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.potentialGrabCount--;
			if (this.potentialGrabCount == 0 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x0600207B RID: 8315 RVA: 0x000ABEB4 File Offset: 0x000AA0B4
	public static void PotentialGrabChildren(BuilderPiece piece, bool enable)
	{
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			if (enable)
			{
				builderPiece.potentialGrabChildCount++;
				if (builderPiece.potentialGrabChildCount == 1 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			else
			{
				builderPiece.potentialGrabChildCount--;
				if (builderPiece.potentialGrabChildCount == 0 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			BuilderPiece.PotentialGrabChildren(builderPiece, enable);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x000ABF30 File Offset: 0x000AA130
	private void RefreshTint()
	{
		if (this.potentialGrabCount > 0 || this.potentialGrabChildCount > 0)
		{
			this.SetTint(this.tableOwner.potentialGrabTint);
			return;
		}
		if (this.paintingCount > 0)
		{
			this.SetTint(this.tableOwner.paintingTint);
			return;
		}
		switch (this.state)
		{
		case BuilderPiece.State.AttachedToDropped:
		case BuilderPiece.State.Dropped:
			this.SetTint(this.tableOwner.droppedTint);
			return;
		case BuilderPiece.State.Grabbed:
		case BuilderPiece.State.GrabbedLocal:
		case BuilderPiece.State.AttachedToArm:
			this.SetTint(this.tableOwner.grabbedTint);
			return;
		case BuilderPiece.State.OnShelf:
		case BuilderPiece.State.OnConveyor:
			this.SetTint(this.tableOwner.shelfTint);
			return;
		}
		this.SetTint(this.tableOwner.defaultTint);
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x000ABFF4 File Offset: 0x000AA1F4
	private void SetTint(float tint)
	{
		if (tint == this.tint)
		{
			return;
		}
		this.tint = tint;
		this.tableOwner.builderRenderer.SetPieceTint(this, tint);
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x000AC01C File Offset: 0x000AA21C
	public void SetParentPiece(int newAttachIndex, BuilderPiece newParentPiece, int newParentAttachIndex)
	{
		if (this.parentHeld != null)
		{
			Debug.LogErrorFormat(newParentPiece.gameObject, "Cannot attach to piece {0} while already held", new object[]
			{
				(newParentPiece == null) ? null : newParentPiece.gameObject.name
			});
			return;
		}
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = newAttachIndex;
		this.parentPiece = newParentPiece;
		this.parentAttachIndex = newParentAttachIndex;
		this.AddPieceToParent(this);
		Transform parent = null;
		if (newParentPiece != null)
		{
			if (newParentAttachIndex >= 0)
			{
				parent = newParentPiece.gridPlanes[newParentAttachIndex].transform;
			}
			else
			{
				parent = newParentPiece.transform;
			}
		}
		base.transform.SetParent(parent, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x000AC0D4 File Offset: 0x000AA2D4
	public void ClearParentPiece(bool ignoreSnaps = false)
	{
		if (this.parentPiece == null)
		{
			if (!ignoreSnaps)
			{
				BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this, this.tableOwner.builderPool);
			}
			return;
		}
		BuilderPiece builderPiece = this.parentPiece;
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = -1;
		this.parentPiece = null;
		this.parentAttachIndex = -1;
		base.transform.SetParent(null, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
		if (!ignoreSnaps)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this.GetRootPiece(), this.tableOwner.builderPool);
		}
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x000AC164 File Offset: 0x000AA364
	public static void RemoveOverlapsWithDifferentPieceRoot(BuilderPiece piece, BuilderPiece root, BuilderPool pool)
	{
		for (int i = 0; i < piece.gridPlanes.Count; i++)
		{
			piece.gridPlanes[i].RemoveSnapsWithDifferentRoot(root, pool);
		}
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(builderPiece, root, pool);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000AC1BC File Offset: 0x000AA3BC
	private void AddPieceToParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		this.nextSiblingPiece = builderPiece.firstChildPiece;
		builderPiece.firstChildPiece = piece;
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(1 + piece.GetChildCount());
		}
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x000AC228 File Offset: 0x000AA428
	private static void RemovePieceFromParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		BuilderPiece builderPiece2 = builderPiece.firstChildPiece;
		if (builderPiece2 == null)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have any children", new object[]
			{
				builderPiece.name,
				piece.name
			});
		}
		bool flag = false;
		if (builderPiece2 == piece)
		{
			builderPiece.firstChildPiece = builderPiece2.nextSiblingPiece;
			flag = true;
		}
		else
		{
			while (builderPiece2 != null)
			{
				if (builderPiece2.nextSiblingPiece == piece)
				{
					builderPiece2.nextSiblingPiece = piece.nextSiblingPiece;
					piece.nextSiblingPiece = null;
					flag = true;
					break;
				}
				builderPiece2 = builderPiece2.nextSiblingPiece;
			}
		}
		if (!flag)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have the piece a child", new object[]
			{
				builderPiece.name,
				piece.name
			});
			return;
		}
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(-1 * (1 + piece.GetChildCount()));
		}
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x000AC32C File Offset: 0x000AA52C
	public void SetParentHeld(Transform parentHeld, int heldByPlayerActorNumber, bool heldInLeftHand)
	{
		if (this.parentPiece != null)
		{
			Debug.LogErrorFormat(this.parentPiece.gameObject, "Cannot hold while already attached to piece {0}", new object[]
			{
				this.parentPiece.gameObject.name
			});
			return;
		}
		this.heldByPlayerActorNumber = heldByPlayerActorNumber;
		this.parentHeld = parentHeld;
		this.heldInLeftHand = heldInLeftHand;
		base.transform.SetParent(parentHeld);
		this.tableOwner.UpdatePieceData(this);
		if (heldByPlayerActorNumber != -1)
		{
			this.OnGrabbedAsRoot();
			return;
		}
		this.OnReleasedAsRoot();
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x000AC3B4 File Offset: 0x000AA5B4
	public void ClearParentHeld()
	{
		if (this.parentHeld == null)
		{
			return;
		}
		if (this.isArmShelf && this.armShelf != null)
		{
			this.armShelf.piece = null;
			this.armShelf = null;
		}
		this.heldByPlayerActorNumber = -1;
		this.parentHeld = null;
		this.heldInLeftHand = false;
		base.transform.SetParent(this.parentHeld);
		this.tableOwner.UpdatePieceData(this);
		this.OnReleasedAsRoot();
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x000AC431 File Offset: 0x000AA631
	public bool IsHeldLocal()
	{
		return this.heldByPlayerActorNumber != -1 && this.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x000AC450 File Offset: 0x000AA650
	public bool IsHeldBy(int actorNumber)
	{
		return actorNumber != -1 && this.heldByPlayerActorNumber == actorNumber;
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x000AC461 File Offset: 0x000AA661
	public bool IsHeldInLeftHand()
	{
		return this.heldInLeftHand;
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000AC469 File Offset: 0x000AA669
	public static bool IsDroppedState(BuilderPiece.State state)
	{
		return state == BuilderPiece.State.Dropped || state == BuilderPiece.State.AttachedToDropped || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.OnConveyor;
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000AC480 File Offset: 0x000AA680
	public void SetActivateTimeStamp(int timeStamp)
	{
		this.activatedTimeStamp = timeStamp;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetActivateTimeStamp(timeStamp);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x000AC4B4 File Offset: 0x000AA6B4
	public void SetState(BuilderPiece.State newState, bool force = false)
	{
		if (newState == this.state && !force)
		{
			if (newState == BuilderPiece.State.Grabbed)
			{
				int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
				if (this.currentColliderLayer != expectedGrabCollisionLayer)
				{
					this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
					this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
				}
			}
			return;
		}
		if (newState == BuilderPiece.State.Dropped && this.state != BuilderPiece.State.Dropped)
		{
			this.tableOwner.AddPieceToDropList(this);
		}
		else if (this.state == BuilderPiece.State.Dropped && newState != BuilderPiece.State.Dropped)
		{
			this.tableOwner.RemovePieceFromDropList(this);
		}
		BuilderPiece.State state = this.state;
		this.state = newState;
		if (this.pieceDataIndex >= 0)
		{
			this.tableOwner.UpdatePieceData(this);
		}
		switch (this.state)
		{
		case BuilderPiece.State.None:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, false);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.None, force);
			this.tableOwner.builderRenderer.RemovePiece(this);
			this.isStatic = true;
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedAndPlaced:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, true);
			this.SetActive(this.onlyWhenPlaced, true);
			this.SetActive(this.onlyWhenNotPlaced, false);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.placedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedAndPlaced, force);
			this.SetStatic(false, force || this.areMeshesToggledOnPlace);
			this.SetPieceActive(this.pieceComponents, true);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToDropped:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Grabbed:
		{
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			int expectedGrabCollisionLayer2 = this.GetExpectedGrabCollisionLayer();
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer2);
			this.SetChildrenState(BuilderPiece.State.Grabbed, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		}
		case BuilderPiece.State.Dropped:
			this.ClearCollisionHistory();
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(false, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.OnShelf:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnShelf, force);
			this.SetStatic(true, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Displayed:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetChildrenState(BuilderPiece.State.Displayed, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.GrabbedLocal:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.GrabbedLocal, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		case BuilderPiece.State.OnConveyor:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnConveyor, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToArm:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.AttachedToArm, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x000ACA48 File Offset: 0x000AAC48
	public void OnGrabbedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.heldByPlayerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.listeningToHandLinks)
		{
			HandLink.OnHandLinkChanged = (Action)Delegate.Combine(HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = true;
		}
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x000ACAA4 File Offset: 0x000AACA4
	public void OnReleasedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.listeningToHandLinks)
		{
			HandLink.OnHandLinkChanged = (Action)Delegate.Remove(HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = false;
		}
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x000ACAE0 File Offset: 0x000AACE0
	public void SetKinematic(bool kinematic, bool destroyImmediate = true)
	{
		if (kinematic && this.rigidBody != null)
		{
			if (destroyImmediate)
			{
				Object.DestroyImmediate(this.rigidBody);
				this.rigidBody = null;
			}
			else
			{
				Object.Destroy(this.rigidBody);
				this.rigidBody = null;
			}
		}
		else if (!kinematic && this.rigidBody == null)
		{
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			if (this.rigidBody != null)
			{
				Debug.LogErrorFormat("We should never already have a rigid body here {0} {1}", new object[]
				{
					this.pieceId,
					this.pieceType
				});
			}
			if (this.rigidBody == null)
			{
				this.rigidBody = base.gameObject.AddComponent<Rigidbody>();
			}
			if (this.rigidBody != null)
			{
				this.rigidBody.isKinematic = kinematic;
			}
		}
		if (this.rigidBody != null)
		{
			this.rigidBody.mass = 1f;
		}
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000ACBE8 File Offset: 0x000AADE8
	public void ClearCollisionHistory()
	{
		if (this.collisionEnterHistory == null)
		{
			this.collisionEnterHistory = new float[this.collisionEnterLimit];
		}
		for (int i = 0; i < this.collisionEnterLimit; i++)
		{
			this.collisionEnterHistory[i] = float.MinValue;
		}
		this.collidersEntered.Clear();
		this.oldCollisionTimeIndex = 0;
		this.forcedFrozen = false;
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x000ACC48 File Offset: 0x000AAE48
	private void OnCollisionEnter(Collision other)
	{
		if (this.state != BuilderPiece.State.Dropped || this.forcedFrozen)
		{
			return;
		}
		BuilderPieceCollider component = other.collider.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if ((piece.state == BuilderPiece.State.AttachedAndPlaced || piece.forcedFrozen) && !this.collidersEntered.Add(other.collider.GetInstanceID()))
			{
				if (this.collisionEnterHistory[this.oldCollisionTimeIndex] > Time.time)
				{
					this.tableOwner.FreezeDroppedPiece(this);
					return;
				}
				this.collisionEnterHistory[this.oldCollisionTimeIndex] = Time.time + this.collisionEnterCooldown;
				int num = this.oldCollisionTimeIndex + 1;
				this.oldCollisionTimeIndex = num;
				this.oldCollisionTimeIndex = num % this.collisionEnterLimit;
			}
		}
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x000ACD08 File Offset: 0x000AAF08
	public int GetExpectedGrabCollisionLayer()
	{
		if (this.heldByPlayerActorNumber != -1)
		{
			if (!GorillaTagger.Instance.offlineVRRig.IsInHandHoldChainWithOtherPlayer(this.heldByPlayerActorNumber))
			{
				return BuilderTable.heldLayer;
			}
			return BuilderTable.heldLayerLocal;
		}
		else
		{
			if (this.parentPiece != null)
			{
				return this.parentPiece.currentColliderLayer;
			}
			return BuilderTable.heldLayer;
		}
	}

	// Token: 0x06002091 RID: 8337 RVA: 0x000ACD60 File Offset: 0x000AAF60
	public void UpdateGrabbedPieceCollisionLayer()
	{
		int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
		if (this.currentColliderLayer != expectedGrabCollisionLayer)
		{
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
			this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
		}
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x000ACD94 File Offset: 0x000AAF94
	private void SetChildrenCollisionLayer(int layer)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetColliderLayers<Collider>(builderPiece.colliders, layer);
			builderPiece.SetChildrenCollisionLayer(layer);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000ACDCE File Offset: 0x000AAFCE
	public void SetStatic(bool isStatic, bool force = false)
	{
		if (this.areMeshesToggledOnPlace)
		{
			this.FindActiveRenderers();
		}
		this.SetDirectRenderersVisible(this.tableOwner.IsInBuilderZone());
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x000ACDF0 File Offset: 0x000AAFF0
	private void FindActiveRenderers()
	{
		if (this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = true;
			}
		}
		this.renderingDirect.Clear();
		BuilderPiece.tempRenderers.Clear();
		base.GetComponentsInChildren<MeshRenderer>(false, BuilderPiece.tempRenderers);
		foreach (MeshRenderer meshRenderer2 in BuilderPiece.tempRenderers)
		{
			if (meshRenderer2.enabled)
			{
				this.renderingDirect.Add(meshRenderer2);
			}
		}
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x000ACEC0 File Offset: 0x000AB0C0
	public void SetDirectRenderersVisible(bool visible)
	{
		if (this.renderingDirect != null && this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = visible;
			}
		}
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x000ACF28 File Offset: 0x000AB128
	private void SetChildrenState(BuilderPiece.State newState, bool force)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetState(newState, force);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x000ACF58 File Offset: 0x000AB158
	public void OnCreate()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceCreate(this.pieceType, this.pieceId);
		}
	}

	// Token: 0x06002098 RID: 8344 RVA: 0x000ACF98 File Offset: 0x000AB198
	public void OnPlacementDeserialized()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPiecePlacementDeserialized();
		}
	}

	// Token: 0x06002099 RID: 8345 RVA: 0x000ACFCC File Offset: 0x000AB1CC
	public void PlayPlacementFx()
	{
		this.PlayVFX(this.vFXInfo.placeVFX);
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x000ACFDF File Offset: 0x000AB1DF
	public void PlayDisconnectFx()
	{
		this.PlayVFX(this.vFXInfo.disconnectVFX);
	}

	// Token: 0x0600209B RID: 8347 RVA: 0x000ACFF2 File Offset: 0x000AB1F2
	public void PlayGrabbedFx()
	{
		this.PlayVFX(this.vFXInfo.grabbedVFX);
	}

	// Token: 0x0600209C RID: 8348 RVA: 0x000AD005 File Offset: 0x000AB205
	public void PlayTooHeavyFx()
	{
		this.PlayVFX(this.vFXInfo.tooHeavyVFX);
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x000AD018 File Offset: 0x000AB218
	public void PlayLocationLockFx()
	{
		this.PlayVFX(this.vFXInfo.locationLockVFX);
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x000AD02B File Offset: 0x000AB22B
	public void PlayRecycleFx()
	{
		this.PlayVFX(this.vFXInfo.recycleVFX);
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x000AD03E File Offset: 0x000AB23E
	private void PlayVFX(GameObject vfx)
	{
		ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x000AD058 File Offset: 0x000AB258
	public static BuilderPiece GetBuilderPieceFromCollider(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		BuilderPieceCollider component = collider.GetComponent<BuilderPieceCollider>();
		if (!(component == null))
		{
			return component.piece;
		}
		return null;
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000AD088 File Offset: 0x000AB288
	public static BuilderPiece GetBuilderPieceFromTransform(Transform transform)
	{
		while (transform != null)
		{
			BuilderPiece component = transform.GetComponent<BuilderPiece>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x000AD0BC File Offset: 0x000AB2BC
	public static void MakePieceRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (piece.parentPiece == null || piece.parentPiece.isBuiltIntoTable)
		{
			return;
		}
		BuilderPiece.MakePieceRoot(piece.parentPiece);
		int newAttachIndex = piece.parentAttachIndex;
		int newParentAttachIndex = piece.attachIndex;
		BuilderPiece builderPiece = piece.parentPiece;
		bool ignoreSnaps = true;
		piece.ClearParentPiece(ignoreSnaps);
		builderPiece.SetParentPiece(newAttachIndex, piece, newParentAttachIndex);
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x000AD120 File Offset: 0x000AB320
	public BuilderPiece GetRootPiece()
	{
		BuilderPiece builderPiece = this;
		while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
		{
			builderPiece = builderPiece.parentPiece;
		}
		return builderPiece;
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000AD154 File Offset: 0x000AB354
	public bool IsPrivatePlot()
	{
		return this.isPrivatePlot;
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000AD15C File Offset: 0x000AB35C
	public bool TryGetPlotComponent(out BuilderPiecePrivatePlot plot)
	{
		plot = this.plotComponent;
		return this.isPrivatePlot;
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000AD174 File Offset: 0x000AB374
	public static bool CanPlayerAttachPieceToPiece(int playerActorNumber, BuilderPiece attachingPiece, BuilderPiece attachToPiece)
	{
		if (attachToPiece.state != BuilderPiece.State.AttachedAndPlaced && !attachToPiece.IsPrivatePlot() && attachToPiece.state != BuilderPiece.State.AttachedToArm)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = attachToPiece.GetAttachedBuiltInPiece();
		if (attachedBuiltInPiece == null || (!attachedBuiltInPiece.isPrivatePlot && !attachedBuiltInPiece.isArmShelf))
		{
			return true;
		}
		if (attachedBuiltInPiece.isArmShelf)
		{
			return attachedBuiltInPiece.heldByPlayerActorNumber == playerActorNumber && attachedBuiltInPiece.armShelf != null && attachedBuiltInPiece.armShelf.CanAttachToArmPiece();
		}
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || (builderPiecePrivatePlot.CanPlayerAttachToPlot(playerActorNumber) && builderPiecePrivatePlot.IsChainUnderCapacity(attachingPiece));
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000AD20C File Offset: 0x000AB40C
	public bool CanPlayerGrabPiece(int actorNumber, Vector3 worldPosition)
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced && !this.isPrivatePlot)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = this.GetAttachedBuiltInPiece();
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return attachedBuiltInPiece == null || !attachedBuiltInPiece.isPrivatePlot || !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || builderPiecePrivatePlot.CanPlayerGrabFromPlot(actorNumber, worldPosition) || this.tableOwner.IsLocationWithinSharedBuildArea(worldPosition);
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000AD26C File Offset: 0x000AB46C
	public bool IsPieceMoving()
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return false;
		}
		if (this.attachPlayerToPiece)
		{
			return true;
		}
		if (this.attachIndex < 0 || this.attachIndex >= this.gridPlanes.Count)
		{
			return false;
		}
		if (this.gridPlanes[this.attachIndex].IsAttachedToMovingGrid())
		{
			return true;
		}
		using (List<BuilderAttachGridPlane>.Enumerator enumerator = this.gridPlanes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isMoving)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000AD314 File Offset: 0x000AB514
	public BuilderPiece GetAttachedBuiltInPiece()
	{
		if (this.isBuiltIntoTable)
		{
			return this;
		}
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return null;
		}
		BuilderPiece rootPiece = this.GetRootPiece();
		if (rootPiece.parentPiece != null)
		{
			rootPiece = rootPiece.parentPiece;
		}
		if (rootPiece.isBuiltIntoTable)
		{
			return rootPiece;
		}
		return null;
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x000AD35C File Offset: 0x000AB55C
	public int GetChainCostAndCount(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		return 1 + this.GetChildCountAndCost(costArray);
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000AD3F0 File Offset: 0x000AB5F0
	public int GetChildCountAndCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			num += builderPiece.GetChildCountAndCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
		return num;
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000AD494 File Offset: 0x000AB694
	public int GetChildCount()
	{
		int num = 0;
		foreach (BuilderAttachGridPlane builderAttachGridPlane in this.gridPlanes)
		{
			num += builderAttachGridPlane.GetChildCount();
		}
		return num;
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000AD4EC File Offset: 0x000AB6EC
	public void GetChainCost(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		this.AddChildCost(costArray);
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000AD580 File Offset: 0x000AB780
	public void AddChildCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			builderPiece.AddChildCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000AD620 File Offset: 0x000AB820
	public void BumpTwistToPositionRotation(byte twist, sbyte xOffset, sbyte zOffset, int potentialAttachIndex, BuilderAttachGridPlane potentialParentGridPlane, out Vector3 localPosition, out Quaternion localRotation, out Vector3 worldPosition, out Quaternion worldRotation)
	{
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		Transform center = potentialParentGridPlane.center;
		Vector3 position = center.position;
		Quaternion rotation = center.rotation;
		float num = flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset;
		float num2 = flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset;
		float num3 = num - potentialParentGridPlane.widthOffset;
		float num4 = num2 - potentialParentGridPlane.lengthOffset;
		Quaternion quaternion = Quaternion.Euler(0f, (float)twist * 90f, 0f);
		Quaternion lhs = rotation * quaternion;
		float x = (float)xOffset * gridSize + num3;
		float z = (float)zOffset * gridSize + num4;
		Vector3 point = new Vector3(x, 0f, z);
		Vector3 a = position + rotation * point;
		Transform center2 = builderAttachGridPlane.center;
		Quaternion quaternion2 = lhs * Quaternion.Inverse(center2.localRotation);
		Vector3 point2 = base.transform.InverseTransformPoint(center2.position);
		Vector3 vector = a - quaternion2 * point2;
		localPosition = potentialParentGridPlane.transform.InverseTransformPoint(vector);
		localRotation = quaternion * Quaternion.Inverse(center2.localRotation);
		worldPosition = vector;
		worldRotation = quaternion2;
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x000AD774 File Offset: 0x000AB974
	public Quaternion TwistToLocalRotation(byte twist, int potentialAttachIndex)
	{
		float y = 90f * (float)twist;
		Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
		if (potentialAttachIndex < 0 || potentialAttachIndex >= this.gridPlanes.Count)
		{
			return quaternion;
		}
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		Transform transform = (builderAttachGridPlane.center != null) ? builderAttachGridPlane.center : builderAttachGridPlane.transform;
		return quaternion * Quaternion.Inverse(transform.localRotation);
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000AD7EC File Offset: 0x000AB9EC
	public int GetPiecePlacement()
	{
		byte pieceTwist = this.GetPieceTwist();
		sbyte xOffset;
		sbyte zOffset;
		this.GetPieceBumpOffset(pieceTwist, out xOffset, out zOffset);
		return BuilderTable.PackPiecePlacement(pieceTwist, xOffset, zOffset);
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x000AD814 File Offset: 0x000ABA14
	public byte GetPieceTwist()
	{
		if (this.attachIndex == -1)
		{
			return 0;
		}
		Quaternion localRotation = base.transform.localRotation;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		Quaternion rotation = localRotation * builderAttachGridPlane.transform.localRotation;
		float num = 0.866f;
		Vector3 lhs = rotation * Vector3.forward;
		float num2 = Vector3.Dot(lhs, Vector3.forward);
		float num3 = Vector3.Dot(lhs, Vector3.right);
		bool flag = Mathf.Abs(num2) > num;
		bool flag2 = Mathf.Abs(num3) > num;
		if (!flag && !flag2)
		{
			return 0;
		}
		uint num4;
		if (flag)
		{
			num4 = ((num2 > 0f) ? 0U : 2U);
		}
		else
		{
			num4 = ((num3 > 0f) ? 1U : 3U);
		}
		return (byte)num4;
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000AD8C8 File Offset: 0x000ABAC8
	public void GetPieceBumpOffset(byte twist, out sbyte xOffset, out sbyte zOffset)
	{
		if (this.attachIndex == -1 || this.parentPiece == null)
		{
			xOffset = 0;
			zOffset = 0;
			return;
		}
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		BuilderAttachGridPlane builderAttachGridPlane2 = this.parentPiece.gridPlanes[this.parentAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		float num = flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset;
		float num2 = flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset;
		float num3 = num - builderAttachGridPlane2.widthOffset;
		float num4 = num2 - builderAttachGridPlane2.lengthOffset;
		Vector3 position = builderAttachGridPlane.center.position;
		Vector3 position2 = builderAttachGridPlane2.center.position;
		Vector3 vector = Quaternion.Inverse(builderAttachGridPlane2.center.rotation) * (position - position2);
		xOffset = (sbyte)Mathf.RoundToInt((vector.x - num3) / gridSize);
		zOffset = (sbyte)Mathf.RoundToInt((vector.z - num4) / gridSize);
	}

	// Token: 0x0400292B RID: 10539
	public const int INVALID = -1;

	// Token: 0x0400292C RID: 10540
	public const float LIGHT_MASS = 1f;

	// Token: 0x0400292D RID: 10541
	public const float HEAVY_MASS = 10000f;

	// Token: 0x0400292E RID: 10542
	public string displayName;

	// Token: 0x0400292F RID: 10543
	public BuilderMaterialOptions materialOptions;

	// Token: 0x04002930 RID: 10544
	public BuilderResources cost;

	// Token: 0x04002931 RID: 10545
	public Vector3 desiredShelfOffset = Vector3.zero;

	// Token: 0x04002932 RID: 10546
	public Vector3 desiredShelfRotationOffset = Vector3.zero;

	// Token: 0x04002933 RID: 10547
	[SerializeField]
	private BuilderPieceEffectInfo vFXInfo;

	// Token: 0x04002934 RID: 10548
	private List<MeshRenderer> materialSwapTargets;

	// Token: 0x04002935 RID: 10549
	private List<GorillaSurfaceOverride> surfaceOverrides;

	// Token: 0x04002936 RID: 10550
	public Transform scaleRoot;

	// Token: 0x04002937 RID: 10551
	public bool isBuiltIntoTable;

	// Token: 0x04002938 RID: 10552
	public bool isArmShelf;

	// Token: 0x04002939 RID: 10553
	[HideInInspector]
	public BuilderArmShelf armShelf;

	// Token: 0x0400293A RID: 10554
	public bool suppressMaterialWarnings;

	// Token: 0x0400293B RID: 10555
	private bool isPrivatePlot;

	// Token: 0x0400293C RID: 10556
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x0400293D RID: 10557
	public BuilderPiecePrivatePlot plotComponent;

	// Token: 0x0400293E RID: 10558
	public bool attachPlayerToPiece;

	// Token: 0x0400293F RID: 10559
	public int pieceType;

	// Token: 0x04002940 RID: 10560
	public int pieceId;

	// Token: 0x04002941 RID: 10561
	public int pieceDataIndex;

	// Token: 0x04002942 RID: 10562
	public int materialType = -1;

	// Token: 0x04002943 RID: 10563
	public int heldByPlayerActorNumber;

	// Token: 0x04002944 RID: 10564
	public bool heldInLeftHand;

	// Token: 0x04002945 RID: 10565
	public Transform parentHeld;

	// Token: 0x04002946 RID: 10566
	[HideInInspector]
	public BuilderPiece parentPiece;

	// Token: 0x04002947 RID: 10567
	[HideInInspector]
	public BuilderPiece firstChildPiece;

	// Token: 0x04002948 RID: 10568
	[HideInInspector]
	public BuilderPiece nextSiblingPiece;

	// Token: 0x04002949 RID: 10569
	[HideInInspector]
	public int attachIndex;

	// Token: 0x0400294A RID: 10570
	[HideInInspector]
	public int parentAttachIndex;

	// Token: 0x0400294B RID: 10571
	public int shelfOwner = -1;

	// Token: 0x0400294C RID: 10572
	[HideInInspector]
	public List<BuilderAttachGridPlane> gridPlanes;

	// Token: 0x0400294D RID: 10573
	[HideInInspector]
	public List<Collider> colliders;

	// Token: 0x0400294E RID: 10574
	public List<Collider> placedOnlyColliders;

	// Token: 0x0400294F RID: 10575
	private int currentColliderLayer = BuilderTable.droppedLayer;

	// Token: 0x04002950 RID: 10576
	public List<Behaviour> onlyWhenPlacedBehaviours;

	// Token: 0x04002951 RID: 10577
	public List<GameObject> onlyWhenPlaced;

	// Token: 0x04002952 RID: 10578
	public List<GameObject> onlyWhenNotPlaced;

	// Token: 0x04002953 RID: 10579
	public List<IBuilderPieceComponent> pieceComponents;

	// Token: 0x04002954 RID: 10580
	public IBuilderPieceFunctional functionalPieceComponent;

	// Token: 0x04002955 RID: 10581
	public byte functionalPieceState;

	// Token: 0x04002956 RID: 10582
	public List<IBuilderPieceFunctional> pieceFunctionComponents;

	// Token: 0x04002957 RID: 10583
	private bool pieceComponentsActive;

	// Token: 0x04002958 RID: 10584
	public bool areMeshesToggledOnPlace;

	// Token: 0x04002959 RID: 10585
	[NonSerialized]
	public Rigidbody rigidBody;

	// Token: 0x0400295A RID: 10586
	[NonSerialized]
	public int activatedTimeStamp;

	// Token: 0x0400295B RID: 10587
	[HideInInspector]
	public int preventSnapUntilMoved;

	// Token: 0x0400295C RID: 10588
	[HideInInspector]
	public Vector3 preventSnapUntilMovedFromPos;

	// Token: 0x0400295D RID: 10589
	[HideInInspector]
	public BuilderPiece requestedParentPiece;

	// Token: 0x0400295E RID: 10590
	private BuilderTable tableOwner;

	// Token: 0x0400295F RID: 10591
	public PieceFallbackInfo fallbackInfo;

	// Token: 0x04002960 RID: 10592
	[NonSerialized]
	public bool overrideSavedPiece;

	// Token: 0x04002961 RID: 10593
	[NonSerialized]
	public int savedPieceType = -1;

	// Token: 0x04002962 RID: 10594
	[NonSerialized]
	public int savedMaterialType = -1;

	// Token: 0x04002963 RID: 10595
	public List<MeshRenderer> meshesToCombine;

	// Token: 0x04002964 RID: 10596
	public GameObject bumpPrefab;

	// Token: 0x04002965 RID: 10597
	public List<GameObject> bumps;

	// Token: 0x04002966 RID: 10598
	private float pieceScale;

	// Token: 0x04002967 RID: 10599
	private float[] collisionEnterHistory;

	// Token: 0x04002968 RID: 10600
	private int collisionEnterLimit = 10;

	// Token: 0x04002969 RID: 10601
	private float collisionEnterCooldown = 2f;

	// Token: 0x0400296A RID: 10602
	private int oldCollisionTimeIndex;

	// Token: 0x0400296B RID: 10603
	[HideInInspector]
	public BuilderPiece.State state;

	// Token: 0x0400296C RID: 10604
	[HideInInspector]
	public bool isStatic;

	// Token: 0x0400296D RID: 10605
	[NonSerialized]
	private bool listeningToHandLinks;

	// Token: 0x0400296E RID: 10606
	[HideInInspector]
	public List<MeshRenderer> renderingDirect;

	// Token: 0x0400296F RID: 10607
	[HideInInspector]
	public List<MeshRenderer> renderingIndirect;

	// Token: 0x04002970 RID: 10608
	[HideInInspector]
	public List<int> renderingIndirectTransformIndex;

	// Token: 0x04002971 RID: 10609
	[HideInInspector]
	public float tint;

	// Token: 0x04002972 RID: 10610
	private int paintingCount;

	// Token: 0x04002973 RID: 10611
	private int potentialGrabCount;

	// Token: 0x04002974 RID: 10612
	private int potentialGrabChildCount;

	// Token: 0x04002975 RID: 10613
	internal bool forcedFrozen;

	// Token: 0x04002976 RID: 10614
	private HashSet<int> collidersEntered = new HashSet<int>(128);

	// Token: 0x04002977 RID: 10615
	private static List<MeshRenderer> tempRenderers = new List<MeshRenderer>(48);

	// Token: 0x02000537 RID: 1335
	public enum State
	{
		// Token: 0x04002979 RID: 10617
		None = -1,
		// Token: 0x0400297A RID: 10618
		AttachedAndPlaced,
		// Token: 0x0400297B RID: 10619
		AttachedToDropped,
		// Token: 0x0400297C RID: 10620
		Grabbed,
		// Token: 0x0400297D RID: 10621
		Dropped,
		// Token: 0x0400297E RID: 10622
		OnShelf,
		// Token: 0x0400297F RID: 10623
		Displayed,
		// Token: 0x04002980 RID: 10624
		GrabbedLocal,
		// Token: 0x04002981 RID: 10625
		OnConveyor,
		// Token: 0x04002982 RID: 10626
		AttachedToArm
	}
}
