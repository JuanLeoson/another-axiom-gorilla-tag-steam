using System;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000647 RID: 1607 RVA: 0x000244F4 File Offset: 0x000226F4
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00024504 File Offset: 0x00022704
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = (this.audioSource != null && this.audioSource.clip != null);
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0002453C File Offset: 0x0002273C
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00024590 File Offset: 0x00022790
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x000245DC File Offset: 0x000227DC
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x000245EC File Offset: 0x000227EC
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.particleFX.isPlaying)
			{
				this.particleFX.Stop();
			}
			if (this.hasAudioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				return;
			}
		}
		else
		{
			if (!this.particleFX.isEmitting)
			{
				this.particleFX.Play();
			}
			if (this.hasAudioSource && !this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
			}
			if (this.IsMyItem() && Time.time > this.activationStartTime + this.activationVibrationStartDuration)
			{
				GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationLoopStrength, Time.deltaTime);
			}
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x000246E0 File Offset: 0x000228E0
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
		if (this.IsMyItem())
		{
			this.activationStartTime = Time.time;
			GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationStartStrength, this.activationVibrationStartDuration);
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0002472C File Offset: 0x0002292C
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0400078A RID: 1930
	[Tooltip("Emission rate will be increase when the trigger button is pressed.")]
	public ParticleSystem particleFX;

	// Token: 0x0400078B RID: 1931
	[Tooltip("Sound will loop and fade in/out volume when trigger pressed.")]
	public AudioSource audioSource;

	// Token: 0x0400078C RID: 1932
	private float activationVibrationStartStrength = 0.8f;

	// Token: 0x0400078D RID: 1933
	private float activationVibrationStartDuration = 0.05f;

	// Token: 0x0400078E RID: 1934
	private float activationVibrationLoopStrength = 0.005f;

	// Token: 0x0400078F RID: 1935
	private float activationStartTime;

	// Token: 0x04000790 RID: 1936
	private bool hasAudioSource;

	// Token: 0x020000FE RID: 254
	private enum VacuumState
	{
		// Token: 0x04000792 RID: 1938
		None = 1,
		// Token: 0x04000793 RID: 1939
		Active
	}
}
