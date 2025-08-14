using System;
using UnityEngine;

// Token: 0x020004D4 RID: 1236
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x06001E53 RID: 7763 RVA: 0x000A128C File Offset: 0x0009F48C
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000A12A6 File Offset: 0x0009F4A6
	private void FixedUpdate()
	{
		this.direction = this.lastPosition - base.transform.position;
		this.lastLastPosition = this.lastPosition;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x040026D8 RID: 9944
	public Vector3 direction;

	// Token: 0x040026D9 RID: 9945
	private Vector3 lastPosition;

	// Token: 0x040026DA RID: 9946
	private Vector3 lastLastPosition;
}
