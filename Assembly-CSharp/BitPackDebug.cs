using System;
using UnityEngine;

// Token: 0x02000AA0 RID: 2720
public class BitPackDebug : MonoBehaviour
{
	// Token: 0x04004D41 RID: 19777
	public bool debugPos;

	// Token: 0x04004D42 RID: 19778
	public Vector3 pos;

	// Token: 0x04004D43 RID: 19779
	public Vector3 min = Vector3.one * -2f;

	// Token: 0x04004D44 RID: 19780
	public Vector3 max = Vector3.one * 2f;

	// Token: 0x04004D45 RID: 19781
	public float rad = 4f;

	// Token: 0x04004D46 RID: 19782
	[Space]
	public bool debug32;

	// Token: 0x04004D47 RID: 19783
	public uint packed;

	// Token: 0x04004D48 RID: 19784
	public Vector3 unpacked;

	// Token: 0x04004D49 RID: 19785
	[Space]
	public bool debug16;

	// Token: 0x04004D4A RID: 19786
	public ushort packed16;

	// Token: 0x04004D4B RID: 19787
	public Vector3 unpacked16;
}
