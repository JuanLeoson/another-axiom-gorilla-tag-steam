using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class BeePerchPoint : MonoBehaviour
{
	// Token: 0x0600069A RID: 1690 RVA: 0x00026AEB File Offset: 0x00024CEB
	public Vector3 GetPoint()
	{
		return base.transform.TransformPoint(this.localPosition);
	}

	// Token: 0x04000819 RID: 2073
	[SerializeField]
	private Vector3 localPosition;
}
