using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000467 RID: 1127
public class UmbrellaItem : TransferrableObject
{
	// Token: 0x06001BFA RID: 7162 RVA: 0x000966D9 File Offset: 0x000948D9
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x000966E8 File Offset: 0x000948E8
	public override void OnActivate()
	{
		base.OnActivate();
		float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float soundVolume = 0.08f;
		int soundIndex;
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			soundIndex = this.SoundIdOpen;
			this.itemState = TransferrableObject.ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(this.umbrellaRainDestroyTrigger);
		}
		else
		{
			soundIndex = this.SoundIdClose;
			this.itemState = TransferrableObject.ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		}
		base.ActivateItemFX(hapticStrength, fixedDeltaTime, soundIndex, soundVolume);
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x00096780 File Offset: 0x00094980
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x0009678E File Offset: 0x0009498E
	internal override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x000967AE File Offset: 0x000949AE
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		this.itemState = TransferrableObject.ItemStates.State1;
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x000967DB File Offset: 0x000949DB
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.OnActivate();
		}
		return true;
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x00096804 File Offset: 0x00094A04
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaItem.UmbrellaStates itemState = (UmbrellaItem.UmbrellaStates)this.itemState;
		if (itemState != this.previousUmbrellaState)
		{
			this.OnUmbrellaStateChanged();
		}
		this.UpdateAngles((itemState == UmbrellaItem.UmbrellaStates.UmbrellaOpen) ? this.startingAngles : this.endingAngles, this.lerpValue);
		this.previousUmbrellaState = itemState;
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x00096854 File Offset: 0x00094A54
	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = this.itemState == TransferrableObject.ItemStates.State0;
		GameObject[] array = this.gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		ParticleSystem[] array2;
		if (flag)
		{
			array2 = this.particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			return;
		}
		array2 = this.particlesEmitOnOpen;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop();
		}
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x000968C8 File Offset: 0x00094AC8
	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < this.umbrellaBones.Length; i++)
		{
			this.umbrellaBones[i].localRotation = Quaternion.Lerp(this.umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	// Token: 0x06001C03 RID: 7171 RVA: 0x00096910 File Offset: 0x00094B10
	protected void GenerateAngles()
	{
		this.startingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int i = 0; i < this.endingAngles.Length; i++)
		{
			this.startingAngles[i] = this.umbrellaToCopy.startingAngles[i];
		}
		this.endingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int j = 0; j < this.endingAngles.Length; j++)
		{
			this.endingAngles[j] = this.umbrellaToCopy.endingAngles[j];
		}
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x0001D558 File Offset: 0x0001B758
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0400247D RID: 9341
	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	// Token: 0x0400247E RID: 9342
	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	// Token: 0x0400247F RID: 9343
	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	// Token: 0x04002480 RID: 9344
	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	private UmbrellaItem umbrellaToCopy;

	// Token: 0x04002481 RID: 9345
	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	// Token: 0x04002482 RID: 9346
	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	// Token: 0x04002483 RID: 9347
	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	// Token: 0x04002484 RID: 9348
	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	// Token: 0x04002485 RID: 9349
	[GorillaSoundLookup]
	public int SoundIdOpen = 64;

	// Token: 0x04002486 RID: 9350
	[GorillaSoundLookup]
	public int SoundIdClose = 65;

	// Token: 0x04002487 RID: 9351
	private UmbrellaItem.UmbrellaStates previousUmbrellaState = UmbrellaItem.UmbrellaStates.UmbrellaOpen;

	// Token: 0x02000468 RID: 1128
	private enum UmbrellaStates
	{
		// Token: 0x04002489 RID: 9353
		UmbrellaOpen = 1,
		// Token: 0x0400248A RID: 9354
		UmbrellaClosed
	}
}
