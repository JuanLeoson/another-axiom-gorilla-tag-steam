using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class CosmeticCritterShadeHidden : CosmeticCritter
{
	// Token: 0x0600044F RID: 1103 RVA: 0x0001946E File Offset: 0x0001766E
	public void SetCenterAndRadius(Vector3 center, float radius)
	{
		this.orbitCenter = center;
		this.orbitRadius = radius;
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0001947E File Offset: 0x0001767E
	public override void SetRandomVariables()
	{
		this.initialAngle = Random.Range(0f, 6.2831855f);
		this.orbitDirection = ((Random.value > 0.5f) ? 1f : -1f);
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x000194B4 File Offset: 0x000176B4
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		float f = this.initialAngle + this.orbitDegreesPerSecond * num * this.orbitDirection;
		float y = this.verticalBobMagnitude * Mathf.Sin(num * this.verticalBobFrequency);
		base.transform.position = this.orbitCenter + new Vector3(this.orbitRadius * Mathf.Cos(f), y, this.orbitRadius * Mathf.Sin(f));
	}

	// Token: 0x040004EE RID: 1262
	[Space]
	[Tooltip("How quickly the Shade orbits around the point where it spawned (the spawner's position).")]
	[SerializeField]
	private float orbitDegreesPerSecond;

	// Token: 0x040004EF RID: 1263
	[Tooltip("The strength of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobMagnitude;

	// Token: 0x040004F0 RID: 1264
	[Tooltip("The frequency of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobFrequency;

	// Token: 0x040004F1 RID: 1265
	private Vector3 orbitCenter;

	// Token: 0x040004F2 RID: 1266
	private float initialAngle;

	// Token: 0x040004F3 RID: 1267
	private float orbitRadius;

	// Token: 0x040004F4 RID: 1268
	private float orbitDirection;
}
