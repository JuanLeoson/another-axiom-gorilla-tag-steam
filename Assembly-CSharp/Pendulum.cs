using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class Pendulum : MonoBehaviour
{
	// Token: 0x06000474 RID: 1140 RVA: 0x00019F88 File Offset: 0x00018188
	private void Start()
	{
		this.pendulum = (this.ClockPendulum = base.gameObject.GetComponent<Transform>());
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00019FB0 File Offset: 0x000181B0
	private void Update()
	{
		if (this.pendulum)
		{
			float z = this.MaxAngleDeflection * Mathf.Sin(Time.time * this.SpeedOfPendulum);
			this.pendulum.localRotation = Quaternion.Euler(0f, 0f, z);
			return;
		}
	}

	// Token: 0x0400053D RID: 1341
	public float MaxAngleDeflection = 10f;

	// Token: 0x0400053E RID: 1342
	public float SpeedOfPendulum = 1f;

	// Token: 0x0400053F RID: 1343
	public Transform ClockPendulum;

	// Token: 0x04000540 RID: 1344
	private Transform pendulum;
}
