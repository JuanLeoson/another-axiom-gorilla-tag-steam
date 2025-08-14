using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C01 RID: 3073
	public class BuilderItem : TransferrableObject
	{
		// Token: 0x06004ACE RID: 19150 RVA: 0x0016B9BC File Offset: 0x00169BBC
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06004ACF RID: 19151 RVA: 0x0016B9DC File Offset: 0x00169BDC
		protected override void Awake()
		{
			base.Awake();
			this.parent = base.transform.parent;
			this.currTable = null;
			this.initialPosition = base.transform.position;
			this.initialRotation = base.transform.rotation;
			this.initialGrabInteractorScale = this.gripInteractor.transform.localScale;
		}

		// Token: 0x06004AD0 RID: 19152 RVA: 0x0008CD68 File Offset: 0x0008AF68
		internal override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x06004AD1 RID: 19153 RVA: 0x000236A3 File Offset: 0x000218A3
		internal override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x06004AD2 RID: 19154 RVA: 0x0016BA3F File Offset: 0x00169C3F
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06004AD3 RID: 19155 RVA: 0x0016BA5C File Offset: 0x00169C5C
		public void AttachPiece(BuilderPiece piece)
		{
			base.transform.SetPositionAndRotation(piece.transform.position, piece.transform.rotation);
			piece.transform.localScale = Vector3.one;
			piece.transform.SetParent(this.itemRoot.transform);
			Debug.LogFormat(piece.gameObject, "Attach Piece {0} to container {1}", new object[]
			{
				piece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = piece;
		}

		// Token: 0x06004AD4 RID: 19156 RVA: 0x0016BAF4 File Offset: 0x00169CF4
		public void DetachPiece(BuilderPiece piece)
		{
			if (piece != this.attachedPiece)
			{
				Debug.LogErrorFormat("Trying to detach piece {0} from a container containing {1}", new object[]
				{
					piece.pieceId,
					this.attachedPiece.pieceId
				});
				return;
			}
			piece.transform.SetParent(null);
			Debug.LogFormat(this.attachedPiece.gameObject, "Detach Piece {0} from container {1}", new object[]
			{
				this.attachedPiece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = null;
		}

		// Token: 0x06004AD5 RID: 19157 RVA: 0x0016BB9C File Offset: 0x00169D9C
		private new void OnStateChanged()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				this.enableCollidersWhenReady = true;
				this.gripInteractor.transform.localScale = this.initialGrabInteractorScale * 2f;
				this.handsFreeOfCollidersTime = 0f;
				return;
			}
			this.enableCollidersWhenReady = false;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			this.handsFreeOfCollidersTime = 0f;
		}

		// Token: 0x06004AD6 RID: 19158 RVA: 0x0016BC10 File Offset: 0x00169E10
		public override Matrix4x4 GetDefaultTransformationMatrix()
		{
			if (this.reliableState.dirty)
			{
				base.SetupHandMatrix(this.reliableState.leftHandAttachPos, this.reliableState.leftHandAttachRot, this.reliableState.rightHandAttachPos, this.reliableState.rightHandAttachRot);
				this.reliableState.dirty = false;
			}
			return base.GetDefaultTransformationMatrix();
		}

		// Token: 0x06004AD7 RID: 19159 RVA: 0x0016BC70 File Offset: 0x00169E70
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			BuilderItem.BuilderItemState itemState = (BuilderItem.BuilderItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
			if (this.enableCollidersWhenReady)
			{
				bool flag = this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsRight) || this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsLeft);
				this.handsFreeOfCollidersTime += (flag ? 0f : Time.deltaTime);
				if (this.handsFreeOfCollidersTime > 0.1f)
				{
					this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
					this.enableCollidersWhenReady = false;
				}
			}
		}

		// Token: 0x06004AD8 RID: 19160 RVA: 0x0016BD28 File Offset: 0x00169F28
		private bool IsOverlapping(List<InteractionPoint> interactionPoints)
		{
			if (interactionPoints == null)
			{
				return false;
			}
			for (int i = 0; i < interactionPoints.Count; i++)
			{
				if (interactionPoints[i] == this.gripInteractor)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AD9 RID: 19161 RVA: 0x0016BD62 File Offset: 0x00169F62
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
		}

		// Token: 0x06004ADA RID: 19162 RVA: 0x0016BD6A File Offset: 0x00169F6A
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x06004ADB RID: 19163 RVA: 0x0016BD92 File Offset: 0x00169F92
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			this.parentItem = null;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			return true;
		}

		// Token: 0x06004ADC RID: 19164 RVA: 0x0016BDCD File Offset: 0x00169FCD
		public void OnHoverOverTableStart(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x06004ADD RID: 19165 RVA: 0x0016BDD6 File Offset: 0x00169FD6
		public void OnHoverOverTableEnd(BuilderTable table)
		{
			this.currTable = null;
		}

		// Token: 0x06004ADE RID: 19166 RVA: 0x0016BDDF File Offset: 0x00169FDF
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
		}

		// Token: 0x06004ADF RID: 19167 RVA: 0x0016BDE8 File Offset: 0x00169FE8
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			base.transform.position = this.initialPosition;
			base.transform.rotation = this.initialRotation;
			if (this.worldShareableInstance != null)
			{
				this.worldShareableInstance.transform.position = this.initialPosition;
				this.worldShareableInstance.transform.rotation = this.initialRotation;
			}
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06004AE0 RID: 19168 RVA: 0x000AD03E File Offset: 0x000AB23E
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x06004AE1 RID: 19169 RVA: 0x0016BE6A File Offset: 0x0016A06A
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x06004AE2 RID: 19170 RVA: 0x0016BEA3 File Offset: 0x0016A0A3
		private bool ShouldPlayFX()
		{
			return this.previousItemState == BuilderItem.BuilderItemState.isHeld || this.previousItemState == BuilderItem.BuilderItemState.dropped;
		}

		// Token: 0x06004AE3 RID: 19171 RVA: 0x0016BEBA File Offset: 0x0016A0BA
		public static GameObject BuildEnvItem(int prefabHash, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(prefabHash, true);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x06004AE4 RID: 19172 RVA: 0x0016BED8 File Offset: 0x0016A0D8
		protected override void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
		{
			if (leftHand)
			{
				this.reliableState.leftHandAttachPos = localPosition;
				this.reliableState.leftHandAttachRot = localRotation;
			}
			else
			{
				this.reliableState.rightHandAttachPos = localPosition;
				this.reliableState.rightHandAttachRot = localRotation;
			}
			this.reliableState.dirty = true;
		}

		// Token: 0x06004AE5 RID: 19173 RVA: 0x0016BF26 File Offset: 0x0016A126
		public int GetPhotonViewId()
		{
			if (this.worldShareableInstance == null)
			{
				return -1;
			}
			return this.worldShareableInstance.ViewID;
		}

		// Token: 0x040053B9 RID: 21433
		public BuilderItemReliableState reliableState;

		// Token: 0x040053BA RID: 21434
		public string builtItemPath;

		// Token: 0x040053BB RID: 21435
		public GameObject itemRoot;

		// Token: 0x040053BC RID: 21436
		private bool enableCollidersWhenReady;

		// Token: 0x040053BD RID: 21437
		private float handsFreeOfCollidersTime;

		// Token: 0x040053BE RID: 21438
		[NonSerialized]
		public BuilderPiece attachedPiece;

		// Token: 0x040053BF RID: 21439
		public List<Behaviour> onlyWhenPlacedBehaviours;

		// Token: 0x040053C0 RID: 21440
		[NonSerialized]
		public BuilderItem parentItem;

		// Token: 0x040053C1 RID: 21441
		public List<BuilderAttachGridPlane> gridPlanes;

		// Token: 0x040053C2 RID: 21442
		public List<BuilderAttachEdge> edges;

		// Token: 0x040053C3 RID: 21443
		private List<Collider> colliders;

		// Token: 0x040053C4 RID: 21444
		private Transform parent;

		// Token: 0x040053C5 RID: 21445
		private Vector3 initialPosition;

		// Token: 0x040053C6 RID: 21446
		private Quaternion initialRotation;

		// Token: 0x040053C7 RID: 21447
		private Vector3 initialGrabInteractorScale;

		// Token: 0x040053C8 RID: 21448
		private BuilderTable currTable;

		// Token: 0x040053C9 RID: 21449
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040053CA RID: 21450
		public AudioClip snapAudio;

		// Token: 0x040053CB RID: 21451
		public AudioClip placeAudio;

		// Token: 0x040053CC RID: 21452
		public GameObject placeVFX;

		// Token: 0x040053CD RID: 21453
		private BuilderItem.BuilderItemState previousItemState = BuilderItem.BuilderItemState.dropped;

		// Token: 0x02000C02 RID: 3074
		private enum BuilderItemState
		{
			// Token: 0x040053CF RID: 21455
			isHeld = 1,
			// Token: 0x040053D0 RID: 21456
			dropped,
			// Token: 0x040053D1 RID: 21457
			placed = 4,
			// Token: 0x040053D2 RID: 21458
			unused0 = 8,
			// Token: 0x040053D3 RID: 21459
			none = 16
		}
	}
}
