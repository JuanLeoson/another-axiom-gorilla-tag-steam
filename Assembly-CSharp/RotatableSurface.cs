using System;
using UnityEngine;

// Token: 0x0200048B RID: 1163
public class RotatableSurface : MonoBehaviour
{
	// Token: 0x06001CE2 RID: 7394 RVA: 0x0009BC1C File Offset: 0x00099E1C
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.localRotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x0400254F RID: 9551
	public ManipulatableSpinner spinner;

	// Token: 0x04002550 RID: 9552
	public float rotationScale = 1f;
}
