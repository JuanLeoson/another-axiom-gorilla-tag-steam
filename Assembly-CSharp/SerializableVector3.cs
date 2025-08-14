using System;
using UnityEngine;

// Token: 0x020002B2 RID: 690
[Serializable]
public struct SerializableVector3
{
	// Token: 0x06000FEF RID: 4079 RVA: 0x0005C943 File Offset: 0x0005AB43
	public SerializableVector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x0005C95A File Offset: 0x0005AB5A
	public static implicit operator SerializableVector3(Vector3 v)
	{
		return new SerializableVector3(v.x, v.y, v.z);
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x0005C973 File Offset: 0x0005AB73
	public static implicit operator Vector3(SerializableVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	// Token: 0x04001871 RID: 6257
	public float x;

	// Token: 0x04001872 RID: 6258
	public float y;

	// Token: 0x04001873 RID: 6259
	public float z;
}
