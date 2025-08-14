using System;

// Token: 0x020005B9 RID: 1465
public interface IGameHittable
{
	// Token: 0x06002400 RID: 9216
	bool IsHitValid(GameHitData hit);

	// Token: 0x06002401 RID: 9217
	void OnHit(GameHitData hit);
}
