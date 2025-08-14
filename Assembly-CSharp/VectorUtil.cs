using System;
using UnityEngine;

// Token: 0x02000355 RID: 853
public static class VectorUtil
{
	// Token: 0x0600143D RID: 5181 RVA: 0x0006CDD9 File Offset: 0x0006AFD9
	public static Vector4 ToVector(this Rect rect)
	{
		return new Vector4(rect.x, rect.y, rect.width, rect.height);
	}
}
