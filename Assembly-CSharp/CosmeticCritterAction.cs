using System;

// Token: 0x02000573 RID: 1395
[Flags]
public enum CosmeticCritterAction
{
	// Token: 0x04002B98 RID: 11160
	None = 0,
	// Token: 0x04002B99 RID: 11161
	RPC = 1,
	// Token: 0x04002B9A RID: 11162
	Spawn = 2,
	// Token: 0x04002B9B RID: 11163
	Despawn = 4,
	// Token: 0x04002B9C RID: 11164
	SpawnLinked = 8,
	// Token: 0x04002B9D RID: 11165
	ShadeHeartbeat = 16
}
