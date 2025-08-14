using System;

// Token: 0x020005A2 RID: 1442
public interface IGameEntityComponent
{
	// Token: 0x06002345 RID: 9029
	void OnEntityInit();

	// Token: 0x06002346 RID: 9030
	void OnEntityDestroy();

	// Token: 0x06002347 RID: 9031
	void OnEntityStateChange(long prevState, long newState);
}
