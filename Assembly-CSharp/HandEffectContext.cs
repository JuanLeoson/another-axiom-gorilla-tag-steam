using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000413 RID: 1043
[Serializable]
internal class HandEffectContext : IFXEffectContextObject
{
	// Token: 0x170002BE RID: 702
	// (get) Token: 0x0600193D RID: 6461 RVA: 0x00088683 File Offset: 0x00086883
	public int[] PrefabPoolIds
	{
		get
		{
			return this.prefabHashes;
		}
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x0600193E RID: 6462 RVA: 0x0008868B File Offset: 0x0008688B
	public Vector3 Positon
	{
		get
		{
			return this.position;
		}
	}

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x0600193F RID: 6463 RVA: 0x00088693 File Offset: 0x00086893
	public Quaternion Rotation
	{
		get
		{
			return this.rotation;
		}
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001940 RID: 6464 RVA: 0x0008869B File Offset: 0x0008689B
	public float Speed
	{
		get
		{
			return this.speed;
		}
	}

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001941 RID: 6465 RVA: 0x000886A3 File Offset: 0x000868A3
	public AudioSource SoundSource
	{
		get
		{
			return this.handSoundSource;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001942 RID: 6466 RVA: 0x000886AB File Offset: 0x000868AB
	public AudioClip Sound
	{
		get
		{
			return this.soundFX;
		}
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06001943 RID: 6467 RVA: 0x000886B3 File Offset: 0x000868B3
	public float Volume
	{
		get
		{
			return this.soundVolume;
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06001944 RID: 6468 RVA: 0x000886BB File Offset: 0x000868BB
	public float Pitch
	{
		get
		{
			return this.soundPitch;
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06001945 RID: 6469 RVA: 0x000886C3 File Offset: 0x000868C3
	// (set) Token: 0x06001946 RID: 6470 RVA: 0x000886CE File Offset: 0x000868CE
	public bool SeparateUpTapCooldown
	{
		get
		{
			return this.separateUpTapCooldownCount > 0;
		}
		set
		{
			this.separateUpTapCooldownCount = Mathf.Max(this.separateUpTapCooldownCount + (value ? 1 : -1), 0);
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06001947 RID: 6471 RVA: 0x000886EA File Offset: 0x000868EA
	// (set) Token: 0x06001948 RID: 6472 RVA: 0x000886FC File Offset: 0x000868FC
	public HandTapOverrides DownTapOverrides
	{
		get
		{
			return this.downTapOverrides ?? this.defaultDownTapOverrides;
		}
		set
		{
			this.downTapOverrides = value;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06001949 RID: 6473 RVA: 0x00088705 File Offset: 0x00086905
	// (set) Token: 0x0600194A RID: 6474 RVA: 0x00088717 File Offset: 0x00086917
	public HandTapOverrides UpTapOverrides
	{
		get
		{
			return this.upTapOverrides ?? this.defaultUpTapOverrides;
		}
		set
		{
			this.upTapOverrides = value;
		}
	}

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x0600194B RID: 6475 RVA: 0x00088720 File Offset: 0x00086920
	// (remove) Token: 0x0600194C RID: 6476 RVA: 0x00088758 File Offset: 0x00086958
	public event Action<HandEffectContext> handTapDown;

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x0600194D RID: 6477 RVA: 0x00088790 File Offset: 0x00086990
	// (remove) Token: 0x0600194E RID: 6478 RVA: 0x000887C8 File Offset: 0x000869C8
	public event Action<HandEffectContext> handTapUp;

	// Token: 0x0600194F RID: 6479 RVA: 0x000887FD File Offset: 0x000869FD
	public void OnTriggerActions()
	{
		if (this.isDownTap)
		{
			Action<HandEffectContext> action = this.handTapDown;
			if (action == null)
			{
				return;
			}
			action(this);
			return;
		}
		else
		{
			Action<HandEffectContext> action2 = this.handTapUp;
			if (action2 == null)
			{
				return;
			}
			action2(this);
			return;
		}
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x0008882C File Offset: 0x00086A2C
	public void OnPlayVisualFX(int fxID, GameObject fx)
	{
		FXModifier fxmodifier;
		if (fx.TryGetComponent<FXModifier>(out fxmodifier))
		{
			fxmodifier.UpdateScale(this.soundVolume * ((fxID == GorillaAmbushManager.HandEffectHash) ? GorillaAmbushManager.HandFXScaleModifier : 1f));
		}
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlaySoundFX(AudioSource audioSource)
	{
	}

	// Token: 0x0400219A RID: 8602
	internal int[] prefabHashes = new int[]
	{
		-1,
		-1
	};

	// Token: 0x0400219B RID: 8603
	internal Vector3 position;

	// Token: 0x0400219C RID: 8604
	internal Quaternion rotation;

	// Token: 0x0400219D RID: 8605
	internal float speed;

	// Token: 0x0400219E RID: 8606
	[SerializeField]
	internal AudioSource handSoundSource;

	// Token: 0x0400219F RID: 8607
	internal AudioClip soundFX;

	// Token: 0x040021A0 RID: 8608
	internal float soundVolume;

	// Token: 0x040021A1 RID: 8609
	internal float soundPitch;

	// Token: 0x040021A2 RID: 8610
	internal int separateUpTapCooldownCount;

	// Token: 0x040021A3 RID: 8611
	[SerializeField]
	internal HandTapOverrides defaultDownTapOverrides;

	// Token: 0x040021A4 RID: 8612
	internal HandTapOverrides downTapOverrides;

	// Token: 0x040021A5 RID: 8613
	[SerializeField]
	internal HandTapOverrides defaultUpTapOverrides;

	// Token: 0x040021A6 RID: 8614
	internal HandTapOverrides upTapOverrides;

	// Token: 0x040021A9 RID: 8617
	internal bool isDownTap;
}
