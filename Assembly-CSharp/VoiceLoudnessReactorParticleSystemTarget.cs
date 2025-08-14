using System;
using UnityEngine;

// Token: 0x02000B56 RID: 2902
[Serializable]
public class VoiceLoudnessReactorParticleSystemTarget
{
	// Token: 0x1700068B RID: 1675
	// (get) Token: 0x0600457F RID: 17791 RVA: 0x0015B3B3 File Offset: 0x001595B3
	// (set) Token: 0x06004580 RID: 17792 RVA: 0x0015B3BB File Offset: 0x001595BB
	public float InitialSpeed
	{
		get
		{
			return this.initialSpeed;
		}
		set
		{
			this.initialSpeed = value;
		}
	}

	// Token: 0x1700068C RID: 1676
	// (get) Token: 0x06004581 RID: 17793 RVA: 0x0015B3C4 File Offset: 0x001595C4
	// (set) Token: 0x06004582 RID: 17794 RVA: 0x0015B3CC File Offset: 0x001595CC
	public float InitialRate
	{
		get
		{
			return this.initialRate;
		}
		set
		{
			this.initialRate = value;
		}
	}

	// Token: 0x1700068D RID: 1677
	// (get) Token: 0x06004583 RID: 17795 RVA: 0x0015B3D5 File Offset: 0x001595D5
	// (set) Token: 0x06004584 RID: 17796 RVA: 0x0015B3DD File Offset: 0x001595DD
	public float InitialSize
	{
		get
		{
			return this.initialSize;
		}
		set
		{
			this.initialSize = value;
		}
	}

	// Token: 0x04005071 RID: 20593
	public ParticleSystem particleSystem;

	// Token: 0x04005072 RID: 20594
	public bool UseSmoothedLoudness;

	// Token: 0x04005073 RID: 20595
	public float Scale = 1f;

	// Token: 0x04005074 RID: 20596
	private float initialSpeed;

	// Token: 0x04005075 RID: 20597
	private float initialRate;

	// Token: 0x04005076 RID: 20598
	private float initialSize;

	// Token: 0x04005077 RID: 20599
	public AnimationCurve speed;

	// Token: 0x04005078 RID: 20600
	public AnimationCurve rate;

	// Token: 0x04005079 RID: 20601
	public AnimationCurve size;

	// Token: 0x0400507A RID: 20602
	[HideInInspector]
	public ParticleSystem.MainModule Main;

	// Token: 0x0400507B RID: 20603
	[HideInInspector]
	public ParticleSystem.EmissionModule Emission;
}
