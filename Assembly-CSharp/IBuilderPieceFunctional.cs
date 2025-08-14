using System;

// Token: 0x02000531 RID: 1329
public interface IBuilderPieceFunctional
{
	// Token: 0x0600205A RID: 8282
	void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x0600205B RID: 8283
	void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x0600205C RID: 8284
	bool IsStateValid(byte state);

	// Token: 0x0600205D RID: 8285
	void FunctionalPieceUpdate();

	// Token: 0x0600205E RID: 8286 RVA: 0x00002628 File Offset: 0x00000828
	void FunctionalPieceFixedUpdate()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x000AB497 File Offset: 0x000A9697
	float GetInteractionDistace()
	{
		return 2.5f;
	}
}
