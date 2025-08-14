using System;
using UnityEngine;

// Token: 0x020007C7 RID: 1991
public class TransformFollow : MonoBehaviour
{
	// Token: 0x060031E3 RID: 12771 RVA: 0x001038A7 File Offset: 0x00101AA7
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x001038BC File Offset: 0x00101ABC
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		Vector3 a;
		Quaternion rotation;
		this.transformToFollow.GetPositionAndRotation(out a, out rotation);
		base.transform.SetPositionAndRotation(a + rotation * this.offset, rotation);
	}

	// Token: 0x04003DAD RID: 15789
	public Transform transformToFollow;

	// Token: 0x04003DAE RID: 15790
	public Vector3 offset;

	// Token: 0x04003DAF RID: 15791
	public Vector3 prevPos;
}
