using System;
using UnityEngine;

// Token: 0x020006BF RID: 1727
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x06002ABE RID: 10942 RVA: 0x000E3518 File Offset: 0x000E1718
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2 = new Color(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x0400363B RID: 13883
	public ParticleSystem particleSystem;

	// Token: 0x0400363C RID: 13884
	public float gradientColorPower = 2f;

	// Token: 0x0400363D RID: 13885
	public bool useLinearColor = true;
}
