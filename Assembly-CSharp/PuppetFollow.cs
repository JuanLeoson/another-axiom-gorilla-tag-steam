using System;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class PuppetFollow : MonoBehaviour
{
	// Token: 0x06001814 RID: 6164 RVA: 0x00080BA4 File Offset: 0x0007EDA4
	private void FixedUpdate()
	{
		base.transform.position = this.sourceTarget.position - this.sourceBase.position + this.puppetBase.position;
		base.transform.localRotation = this.sourceTarget.localRotation;
	}

	// Token: 0x04001FF9 RID: 8185
	public Transform sourceTarget;

	// Token: 0x04001FFA RID: 8186
	public Transform sourceBase;

	// Token: 0x04001FFB RID: 8187
	public Transform puppetBase;
}
