using System;
using UnityEngine;

// Token: 0x02000B1E RID: 2846
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x0600448F RID: 17551 RVA: 0x00156928 File Offset: 0x00154B28
	public void Awake()
	{
		if (this.particleFX == null)
		{
			this.particleFX = base.GetComponent<ParticleSystem>();
		}
		if (this.particleFX == null)
		{
			Debug.LogError("SurfaceImpactFX: No ParticleSystem found! Disabling component.", this);
			base.enabled = false;
			return;
		}
		this.fxMainModule = this.particleFX.main;
	}

	// Token: 0x06004490 RID: 17552 RVA: 0x00156981 File Offset: 0x00154B81
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x04004EB4 RID: 20148
	public ParticleSystem particleFX;

	// Token: 0x04004EB5 RID: 20149
	public float startingGravityModifier;

	// Token: 0x04004EB6 RID: 20150
	public Vector3 startingScale = Vector3.one;

	// Token: 0x04004EB7 RID: 20151
	private ParticleSystem.MainModule fxMainModule;
}
