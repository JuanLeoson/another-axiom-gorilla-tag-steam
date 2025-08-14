using System;

// Token: 0x02000530 RID: 1328
public interface IBuilderPieceComponent
{
	// Token: 0x06002055 RID: 8277
	void OnPieceCreate(int pieceType, int pieceId);

	// Token: 0x06002056 RID: 8278
	void OnPieceDestroy();

	// Token: 0x06002057 RID: 8279
	void OnPiecePlacementDeserialized();

	// Token: 0x06002058 RID: 8280
	void OnPieceActivate();

	// Token: 0x06002059 RID: 8281
	void OnPieceDeactivate();
}
