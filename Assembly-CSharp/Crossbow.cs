using System;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class Crossbow : ProjectileWeapon
{
	// Token: 0x060009EA RID: 2538 RVA: 0x00036628 File Offset: 0x00034828
	protected override void Awake()
	{
		base.Awake();
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCrank));
		}
		this.SetReloadFraction(0f);
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00036670 File Offset: 0x00034870
	public void SetReloadFraction(float newFraction)
	{
		this.loadFraction = Mathf.Clamp01(newFraction);
		this.animator.SetFloat(this.ReloadFractionHashID, this.loadFraction);
		if (this.loadFraction == 1f && !this.dummyProjectile.enabled)
		{
			this.shootSfx.GTPlayOneShot(this.reloadComplete_audioClip, 1f);
			this.dummyProjectile.enabled = true;
			return;
		}
		if (this.loadFraction < 1f && this.dummyProjectile.enabled)
		{
			this.dummyProjectile.enabled = false;
		}
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00036708 File Offset: 0x00034908
	private void OnCrank(float degrees)
	{
		if (this.loadFraction == 1f)
		{
			return;
		}
		this.totalCrankDegrees += degrees;
		this.crankSoundDegrees += degrees;
		if (Mathf.Abs(this.crankSoundDegrees) > this.crankSoundDegreesThreshold)
		{
			this.playingCrankSoundUntilTimestamp = Time.time + this.crankSoundContinueDuration;
			this.crankSoundDegrees = 0f;
		}
		if (!this.reloadAudio.isPlaying && Time.time < this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTPlay();
		}
		this.SetReloadFraction(Mathf.Abs(this.totalCrankDegrees / this.crankTotalDegreesToReload));
		if (this.loadFraction >= 1f)
		{
			this.totalCrankDegrees = 0f;
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x000367C4 File Offset: 0x000349C4
	protected override Vector3 GetLaunchPosition()
	{
		return this.launchPosition.position;
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x000367D1 File Offset: 0x000349D1
	protected override Vector3 GetLaunchVelocity()
	{
		return this.launchPosition.forward * this.launchSpeed * base.myRig.scaleFactor;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x000367FC File Offset: 0x000349FC
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			this.wasPressingTrigger = false;
			return;
		}
		if ((base.InLeftHand() ? base.myRig.leftIndex.calcT : base.myRig.rightIndex.calcT) > 0.5f)
		{
			if (this.loadFraction == 1f && !this.wasPressingTrigger)
			{
				this.SetReloadFraction(0f);
				this.animator.SetTrigger(this.FireHashID);
				base.LaunchProjectile();
			}
			this.wasPressingTrigger = true;
		}
		else
		{
			this.wasPressingTrigger = false;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (this.loadFraction < 1f)
			{
				this.itemState &= (TransferrableObject.ItemStates)(-2);
				return;
			}
		}
		else if (this.loadFraction == 1f)
		{
			this.itemState |= TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x000368EC File Offset: 0x00034AEC
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			this.SetReloadFraction(1f);
			return;
		}
		if (this.loadFraction == 1f)
		{
			this.SetReloadFraction(0f);
		}
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00036944 File Offset: 0x00034B44
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.reloadAudio.isPlaying && Time.time > this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTStop();
		}
	}

	// Token: 0x04000BEF RID: 3055
	[SerializeField]
	private Transform launchPosition;

	// Token: 0x04000BF0 RID: 3056
	[SerializeField]
	private float launchSpeed;

	// Token: 0x04000BF1 RID: 3057
	[SerializeField]
	private Animator animator;

	// Token: 0x04000BF2 RID: 3058
	[SerializeField]
	private float crankTotalDegreesToReload;

	// Token: 0x04000BF3 RID: 3059
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x04000BF4 RID: 3060
	[SerializeField]
	private MeshRenderer dummyProjectile;

	// Token: 0x04000BF5 RID: 3061
	[SerializeField]
	private AudioSource reloadAudio;

	// Token: 0x04000BF6 RID: 3062
	[SerializeField]
	private AudioClip reloadComplete_audioClip;

	// Token: 0x04000BF7 RID: 3063
	[SerializeField]
	private float crankSoundContinueDuration = 0.1f;

	// Token: 0x04000BF8 RID: 3064
	[SerializeField]
	private float crankSoundDegreesThreshold = 0.1f;

	// Token: 0x04000BF9 RID: 3065
	private AnimHashId FireHashID = "Fire";

	// Token: 0x04000BFA RID: 3066
	private AnimHashId ReloadFractionHashID = "ReloadFraction";

	// Token: 0x04000BFB RID: 3067
	private float totalCrankDegrees;

	// Token: 0x04000BFC RID: 3068
	private float loadFraction;

	// Token: 0x04000BFD RID: 3069
	private float playingCrankSoundUntilTimestamp;

	// Token: 0x04000BFE RID: 3070
	private float crankSoundDegrees;

	// Token: 0x04000BFF RID: 3071
	private bool wasPressingTrigger;
}
