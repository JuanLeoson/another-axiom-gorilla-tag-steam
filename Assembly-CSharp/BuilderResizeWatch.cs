using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000549 RID: 1353
public class BuilderResizeWatch : MonoBehaviour
{
	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06002111 RID: 8465 RVA: 0x000B2D30 File Offset: 0x000B0F30
	public int SizeLayerMaskGrow
	{
		get
		{
			int num = 0;
			if (this.growSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.growSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.growSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.growSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06002112 RID: 8466 RVA: 0x000B2D84 File Offset: 0x000B0F84
	public int SizeLayerMaskShrink
	{
		get
		{
			int num = 0;
			if (this.shrinkSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.shrinkSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.shrinkSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.shrinkSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000B2DD8 File Offset: 0x000B0FD8
	private void Start()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.AddListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.AddListener(new UnityAction(this.OnShrinkButtonPressed));
		}
		this.ownerRig = base.GetComponentInParent<VRRig>();
		this.enableDist = GTPlayer.Instance.bodyCollider.height;
		this.enableDistSq = this.enableDist * this.enableDist;
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x000B2E70 File Offset: 0x000B1070
	private void OnDestroy()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.RemoveListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.RemoveListener(new UnityAction(this.OnShrinkButtonPressed));
		}
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x000B2ED4 File Offset: 0x000B10D4
	private void OnEnlargeButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
				return;
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskGrow && !this.updateCollision)
		{
			this.DisableCollisionWithPieces();
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskGrow;
			if (this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, this.ownerRig.transform.position, true);
			}
			this.timeToCheckCollision = (double)(Time.time + this.growDelay);
			this.updateCollision = true;
		}
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x000B2FA4 File Offset: 0x000B11A4
	private void DisableCollisionWithPieces()
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.ownerRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(GTPlayer.Instance.headCollider.transform.position, 1f, this.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable && !this.collisionDisabledPieces.Contains(builderPieceFromCollider))
			{
				foreach (Collider collider in builderPieceFromCollider.colliders)
				{
					collider.enabled = false;
				}
				foreach (Collider collider2 in builderPieceFromCollider.placedOnlyColliders)
				{
					collider2.enabled = false;
				}
				this.collisionDisabledPieces.Add(builderPieceFromCollider);
			}
		}
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000B30E0 File Offset: 0x000B12E0
	private void EnableCollisionWithPieces()
	{
		for (int i = this.collisionDisabledPieces.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.collisionDisabledPieces[i];
			if (builderPiece == null)
			{
				this.collisionDisabledPieces.RemoveAt(i);
			}
			else if (Vector3.SqrMagnitude(GTPlayer.Instance.bodyCollider.transform.position - builderPiece.transform.position) >= this.enableDistSq)
			{
				this.EnableCollisionWithPiece(builderPiece);
				this.collisionDisabledPieces.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000B3170 File Offset: 0x000B1370
	private void EnableCollisionWithPiece(BuilderPiece piece)
	{
		foreach (Collider collider in piece.colliders)
		{
			collider.enabled = (piece.state != BuilderPiece.State.None && piece.state != BuilderPiece.State.Displayed);
		}
		foreach (Collider collider2 in piece.placedOnlyColliders)
		{
			collider2.enabled = (piece.state == BuilderPiece.State.AttachedAndPlaced);
		}
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x000B3220 File Offset: 0x000B1420
	private void Update()
	{
		if (this.updateCollision && (double)Time.time >= this.timeToCheckCollision)
		{
			this.EnableCollisionWithPieces();
			if (this.collisionDisabledPieces.Count <= 0)
			{
				this.updateCollision = false;
			}
		}
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000B3254 File Offset: 0x000B1454
	private void OnShrinkButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskShrink)
		{
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskShrink;
		}
	}

	// Token: 0x04002A48 RID: 10824
	[SerializeField]
	private HeldButton enlargeButton;

	// Token: 0x04002A49 RID: 10825
	[SerializeField]
	private HeldButton shrinkButton;

	// Token: 0x04002A4A RID: 10826
	[SerializeField]
	private GameObject fxForLayerChange;

	// Token: 0x04002A4B RID: 10827
	private VRRig ownerRig;

	// Token: 0x04002A4C RID: 10828
	private SizeManager sizeManager;

	// Token: 0x04002A4D RID: 10829
	[HideInInspector]
	public Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x04002A4E RID: 10830
	[HideInInspector]
	public List<BuilderPiece> collisionDisabledPieces = new List<BuilderPiece>();

	// Token: 0x04002A4F RID: 10831
	private float enableDist = 1f;

	// Token: 0x04002A50 RID: 10832
	private float enableDistSq = 1f;

	// Token: 0x04002A51 RID: 10833
	private bool updateCollision;

	// Token: 0x04002A52 RID: 10834
	private float growDelay = 1f;

	// Token: 0x04002A53 RID: 10835
	private double timeToCheckCollision;

	// Token: 0x04002A54 RID: 10836
	public BuilderResizeWatch.BuilderSizeChangeSettings growSettings;

	// Token: 0x04002A55 RID: 10837
	public BuilderResizeWatch.BuilderSizeChangeSettings shrinkSettings;

	// Token: 0x0200054A RID: 1354
	[Serializable]
	public struct BuilderSizeChangeSettings
	{
		// Token: 0x04002A56 RID: 10838
		public bool affectLayerA;

		// Token: 0x04002A57 RID: 10839
		public bool affectLayerB;

		// Token: 0x04002A58 RID: 10840
		public bool affectLayerC;

		// Token: 0x04002A59 RID: 10841
		public bool affectLayerD;
	}
}
