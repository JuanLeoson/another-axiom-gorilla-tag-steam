using System;
using UnityEngine;

// Token: 0x020002B1 RID: 689
[Serializable]
public struct SerializableVector2
{
	// Token: 0x06000FEC RID: 4076 RVA: 0x0005C90D File Offset: 0x0005AB0D
	public SerializableVector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0005C91D File Offset: 0x0005AB1D
	public static implicit operator SerializableVector2(Vector2 v)
	{
		return new SerializableVector2(v.x, v.y);
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x0005C930 File Offset: 0x0005AB30
	public static implicit operator Vector2(SerializableVector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	// Token: 0x0400186F RID: 6255
	public float x;

	// Token: 0x04001870 RID: 6256
	public float y;
}
