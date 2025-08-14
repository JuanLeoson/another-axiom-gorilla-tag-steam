using System;
using UnityEngine;

// Token: 0x0200021D RID: 541
[CreateAssetMenu(fileName = "PlatformTagJoin", menuName = "ScriptableObjects/PlatformTagJoin", order = 0)]
public class PlatformTagJoin : ScriptableObject
{
	// Token: 0x06000CB5 RID: 3253 RVA: 0x000445C6 File Offset: 0x000427C6
	public override string ToString()
	{
		return this.PlatformTag;
	}

	// Token: 0x04000FA7 RID: 4007
	public string PlatformTag = " ";
}
