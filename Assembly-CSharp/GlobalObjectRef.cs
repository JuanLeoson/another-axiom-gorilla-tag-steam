using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000870 RID: 2160
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct GlobalObjectRef
{
	// Token: 0x0600362F RID: 13871 RVA: 0x0011C624 File Offset: 0x0011A824
	public static GlobalObjectRef ObjectToRefSlow(Object target)
	{
		return default(GlobalObjectRef);
	}

	// Token: 0x06003630 RID: 13872 RVA: 0x00058615 File Offset: 0x00056815
	public static Object RefToObjectSlow(GlobalObjectRef @ref)
	{
		return null;
	}

	// Token: 0x04004320 RID: 17184
	[FieldOffset(0)]
	public ulong targetObjectId;

	// Token: 0x04004321 RID: 17185
	[FieldOffset(8)]
	public ulong targetPrefabId;

	// Token: 0x04004322 RID: 17186
	[FieldOffset(16)]
	public Guid assetGUID;

	// Token: 0x04004323 RID: 17187
	[HideInInspector]
	[FieldOffset(32)]
	public int identifierType;

	// Token: 0x04004324 RID: 17188
	[NonSerialized]
	[FieldOffset(32)]
	private GlobalObjectRefType refType;
}
