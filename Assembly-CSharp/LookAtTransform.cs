using System;
using UnityEngine;

// Token: 0x02000AC3 RID: 2755
public class LookAtTransform : MonoBehaviour
{
	// Token: 0x06004275 RID: 17013 RVA: 0x0014E23F File Offset: 0x0014C43F
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	// Token: 0x04004DAC RID: 19884
	[SerializeField]
	private Transform lookAt;
}
