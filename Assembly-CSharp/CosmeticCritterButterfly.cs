using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class CosmeticCritterButterfly : CosmeticCritter
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000423 RID: 1059 RVA: 0x0001873E File Offset: 0x0001693E
	public ParticleSystem.EmitParams GetEmitParams
	{
		get
		{
			return this.emitParams;
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x00018746 File Offset: 0x00016946
	public void SetStartPos(Vector3 initialPos)
	{
		this.startPosition = initialPos;
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00018750 File Offset: 0x00016950
	public override void SetRandomVariables()
	{
		this.direction = Random.insideUnitSphere;
		this.emitParams.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
		this.particleSystem.Emit(this.emitParams, 1);
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x000187AD File Offset: 0x000169AD
	public override void Tick()
	{
		base.transform.position = this.startPosition + (float)base.GetAliveTime() * this.speed * this.direction;
	}

	// Token: 0x040004AD RID: 1197
	[Tooltip("The speed this Butterfly will move at.")]
	[SerializeField]
	private float speed = 1f;

	// Token: 0x040004AE RID: 1198
	[Tooltip("Emit one particle from this particle system when spawning.")]
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x040004AF RID: 1199
	private Vector3 startPosition;

	// Token: 0x040004B0 RID: 1200
	private Vector3 direction;

	// Token: 0x040004B1 RID: 1201
	private ParticleSystem.EmitParams emitParams;
}
