using System;
using UnityEngine;

// Token: 0x02000888 RID: 2184
[Serializable]
public struct OrientedBounds
{
	// Token: 0x17000536 RID: 1334
	// (get) Token: 0x060036BE RID: 14014 RVA: 0x0011E0DC File Offset: 0x0011C2DC
	public static OrientedBounds Empty { get; } = new OrientedBounds
	{
		size = Vector3.zero,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x17000537 RID: 1335
	// (get) Token: 0x060036BF RID: 14015 RVA: 0x0011E0E3 File Offset: 0x0011C2E3
	public static OrientedBounds Identity { get; } = new OrientedBounds
	{
		size = Vector3.one,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x060036C0 RID: 14016 RVA: 0x0011E0EA File Offset: 0x0011C2EA
	public Matrix4x4 TRS()
	{
		return Matrix4x4.TRS(this.center, this.rotation, this.size);
	}

	// Token: 0x040043AB RID: 17323
	public Vector3 size;

	// Token: 0x040043AC RID: 17324
	public Vector3 center;

	// Token: 0x040043AD RID: 17325
	public Quaternion rotation;
}
