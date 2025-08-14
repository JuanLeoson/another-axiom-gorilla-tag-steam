using System;
using UnityEngine;

// Token: 0x02000511 RID: 1297
[CreateAssetMenu(fileName = "BuilderPieceEffectInfo", menuName = "Gorilla Tag/Builder/EffectInfo", order = 0)]
public class BuilderPieceEffectInfo : ScriptableObject
{
	// Token: 0x0400284D RID: 10317
	public GameObject placeVFX;

	// Token: 0x0400284E RID: 10318
	public GameObject disconnectVFX;

	// Token: 0x0400284F RID: 10319
	public GameObject grabbedVFX;

	// Token: 0x04002850 RID: 10320
	public GameObject locationLockVFX;

	// Token: 0x04002851 RID: 10321
	public GameObject recycleVFX;

	// Token: 0x04002852 RID: 10322
	public GameObject tooHeavyVFX;
}
