using System;
using UnityEngine;

// Token: 0x02000445 RID: 1093
public class TestManipulatableSpinner : MonoBehaviour
{
	// Token: 0x06001ACD RID: 6861 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x0008F01C File Offset: 0x0008D21C
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.rotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x0400230D RID: 8973
	public ManipulatableSpinner spinner;

	// Token: 0x0400230E RID: 8974
	public float rotationScale = 1f;
}
