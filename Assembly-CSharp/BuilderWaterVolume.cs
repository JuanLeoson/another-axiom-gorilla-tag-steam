using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200051B RID: 1307
public class BuilderWaterVolume : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x06001FC2 RID: 8130 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPieceDestroy()
	{
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000A7AE0 File Offset: 0x000A5CE0
	public void OnPiecePlacementDeserialized()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000A7B6C File Offset: 0x000A5D6C
	public void OnPieceActivate()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x000A7BF8 File Offset: 0x000A5DF8
	public void OnPieceDeactivate()
	{
		this.waterVolume.SetActive(false);
		this.waterMesh.SetActive(true);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = this.floating.localPosition;
		}
	}

	// Token: 0x04002875 RID: 10357
	[SerializeField]
	private BuilderPiece piece;

	// Token: 0x04002876 RID: 10358
	[SerializeField]
	private GameObject waterVolume;

	// Token: 0x04002877 RID: 10359
	[SerializeField]
	private GameObject waterMesh;

	// Token: 0x04002878 RID: 10360
	[FormerlySerializedAs("lillyPads")]
	[SerializeField]
	private Transform floatingObjects;

	// Token: 0x04002879 RID: 10361
	[SerializeField]
	private Transform floating;

	// Token: 0x0400287A RID: 10362
	[SerializeField]
	private Transform sunk;
}
