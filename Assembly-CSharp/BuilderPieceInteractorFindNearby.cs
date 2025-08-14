using System;
using UnityEngine;

// Token: 0x02000512 RID: 1298
public class BuilderPieceInteractorFindNearby : MonoBehaviour
{
	// Token: 0x06001FA7 RID: 8103 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000A7662 File Offset: 0x000A5862
	private void LateUpdate()
	{
		if (this.pieceInteractor != null)
		{
			this.pieceInteractor.StartFindNearbyPieces();
		}
	}

	// Token: 0x04002853 RID: 10323
	public BuilderPieceInteractor pieceInteractor;
}
