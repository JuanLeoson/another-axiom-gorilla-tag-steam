using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class FreezePosition : MonoBehaviour
{
	// Token: 0x06000393 RID: 915 RVA: 0x00016433 File Offset: 0x00014633
	private void FixedUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00016433 File Offset: 0x00014633
	private void LateUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x04000411 RID: 1041
	public Transform target;

	// Token: 0x04000412 RID: 1042
	public Vector3 localPosition;
}
