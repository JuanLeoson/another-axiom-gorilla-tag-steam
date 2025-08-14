using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
[Serializable]
public class ZoneData
{
	// Token: 0x04001729 RID: 5929
	public GTZone zone;

	// Token: 0x0400172A RID: 5930
	public string sceneName;

	// Token: 0x0400172B RID: 5931
	public float CameraFarClipPlane = 500f;

	// Token: 0x0400172C RID: 5932
	public GameObject[] rootGameObjects;

	// Token: 0x0400172D RID: 5933
	[NonSerialized]
	public bool active;
}
