using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000520 RID: 1312
public class BuilderArmShelf : MonoBehaviour
{
	// Token: 0x06001FD7 RID: 8151 RVA: 0x000A81B8 File Offset: 0x000A63B8
	private void Start()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x000A81C6 File Offset: 0x000A63C6
	public bool IsOwnedLocally()
	{
		return this.ownerRig != null && this.ownerRig.isLocal;
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x000A81E3 File Offset: 0x000A63E3
	public bool CanAttachToArmPiece()
	{
		return this.ownerRig != null && this.ownerRig.scaleFactor >= 1f;
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x000A820C File Offset: 0x000A640C
	public void DropAttachedPieces()
	{
		if (this.ownerRig != null && this.piece != null)
		{
			Vector3 velocity = Vector3.zero;
			if (this.piece.firstChildPiece == null)
			{
				return;
			}
			BuilderTable table = this.piece.GetTable();
			Vector3 point = table.roomCenter.position - this.piece.transform.position;
			point.Normalize();
			Vector3 a = Quaternion.Euler(0f, 180f, 0f) * point;
			velocity = BuilderTable.DROP_ZONE_REPEL * a;
			BuilderPiece builderPiece = this.piece.firstChildPiece;
			while (builderPiece != null)
			{
				table.RequestDropPiece(builderPiece, builderPiece.transform.position + a * 0.1f, builderPiece.transform.rotation, velocity, Vector3.zero);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}

	// Token: 0x0400289A RID: 10394
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x0400289B RID: 10395
	public Transform pieceAnchor;

	// Token: 0x0400289C RID: 10396
	private VRRig ownerRig;
}
