using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020001A8 RID: 424
[Serializable]
public class HandTapOverrides
{
	// Token: 0x04000CE6 RID: 3302
	private const string PREFAB_TOOLTIP = "Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.";

	// Token: 0x04000CE7 RID: 3303
	public bool overrideSurfacePrefab;

	// Token: 0x04000CE8 RID: 3304
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper surfaceTapPrefab;

	// Token: 0x04000CE9 RID: 3305
	public bool overrideGamemodePrefab;

	// Token: 0x04000CEA RID: 3306
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper gamemodeTapPrefab;

	// Token: 0x04000CEB RID: 3307
	public bool overrideSound;

	// Token: 0x04000CEC RID: 3308
	public AudioClip tapSound;
}
