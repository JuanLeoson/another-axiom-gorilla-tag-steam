using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class RotateXform : MonoBehaviour
{
	// Token: 0x060004FB RID: 1275 RVA: 0x0001D12C File Offset: 0x0001B32C
	private void Update()
	{
		if (!this.xform)
		{
			return;
		}
		Vector3 vector = (this.mode == RotateXform.Mode.Local) ? this.xform.localEulerAngles : this.xform.eulerAngles;
		float num = Time.deltaTime * this.speedFactor;
		vector.x += this.speed.x * num;
		vector.y += this.speed.y * num;
		vector.z += this.speed.z * num;
		if (this.mode == RotateXform.Mode.Local)
		{
			this.xform.localEulerAngles = vector;
			return;
		}
		this.xform.eulerAngles = vector;
	}

	// Token: 0x040005F6 RID: 1526
	public Transform xform;

	// Token: 0x040005F7 RID: 1527
	public Vector3 speed = Vector3.zero;

	// Token: 0x040005F8 RID: 1528
	public RotateXform.Mode mode;

	// Token: 0x040005F9 RID: 1529
	public float speedFactor = 0.0625f;

	// Token: 0x020000CA RID: 202
	public enum Mode
	{
		// Token: 0x040005FB RID: 1531
		Local,
		// Token: 0x040005FC RID: 1532
		World
	}
}
