using System;
using UnityEngine;

// Token: 0x02000ADD RID: 2781
public class PredicatableRandomRotation : MonoBehaviour
{
	// Token: 0x060042EE RID: 17134 RVA: 0x0014FEA8 File Offset: 0x0014E0A8
	private void Start()
	{
		if (this.source == null)
		{
			this.source = base.transform;
		}
	}

	// Token: 0x060042EF RID: 17135 RVA: 0x0014FEC4 File Offset: 0x0014E0C4
	private void Update()
	{
		float d = (this.source.position.x * this.source.position.x + this.source.position.y * this.source.position.y + this.source.position.z * this.source.position.z) % 1f;
		base.transform.Rotate(this.rot * d);
	}

	// Token: 0x04004DC7 RID: 19911
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	// Token: 0x04004DC8 RID: 19912
	[SerializeField]
	private Transform source;
}
