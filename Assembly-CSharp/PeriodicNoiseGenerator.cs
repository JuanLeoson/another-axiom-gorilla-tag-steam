using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class PeriodicNoiseGenerator : MonoBehaviour
{
	// Token: 0x06000344 RID: 836 RVA: 0x00013AF1 File Offset: 0x00011CF1
	private void Awake()
	{
		this.noiseActor = base.GetComponentInParent<CrittersLoudNoise>();
		this.lastTime = Time.time;
		this.mR = base.GetComponentInChildren<MeshRenderer>();
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00013B18 File Offset: 0x00011D18
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.time > this.lastTime + this.sleepDuration)
		{
			this.lastTime = Time.time + this.randomDuration * Random.value;
			this.noiseActor.SetTimeEnabled();
			this.noiseActor.soundEnabled = true;
			this.mR.sharedMaterial = this.solid;
		}
		if (!this.noiseActor.soundEnabled && this.mR.sharedMaterial != this.transparent)
		{
			this.mR.sharedMaterial = this.transparent;
		}
	}

	// Token: 0x040003D4 RID: 980
	public float sleepDuration;

	// Token: 0x040003D5 RID: 981
	public float randomDuration;

	// Token: 0x040003D6 RID: 982
	public float lastTime;

	// Token: 0x040003D7 RID: 983
	private CrittersLoudNoise noiseActor;

	// Token: 0x040003D8 RID: 984
	public Material transparent;

	// Token: 0x040003D9 RID: 985
	public Material solid;

	// Token: 0x040003DA RID: 986
	private MeshRenderer mR;
}
