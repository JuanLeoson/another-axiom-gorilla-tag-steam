using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000A6D RID: 2669
[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x06004126 RID: 16678 RVA: 0x0014A497 File Offset: 0x00148697
	public ExpectedUsersDecayTimer ExpectedUsersTimer
	{
		get
		{
			return this.expectedUsersTimer;
		}
	}

	// Token: 0x1700062E RID: 1582
	// (get) Token: 0x06004127 RID: 16679 RVA: 0x0014A49F File Offset: 0x0014869F
	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	// Token: 0x1700062F RID: 1583
	// (get) Token: 0x06004128 RID: 16680 RVA: 0x0014A4A7 File Offset: 0x001486A7
	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	// Token: 0x17000630 RID: 1584
	// (get) Token: 0x06004129 RID: 16681 RVA: 0x0014A4AF File Offset: 0x001486AF
	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	// Token: 0x17000631 RID: 1585
	// (get) Token: 0x0600412A RID: 16682 RVA: 0x0014A4B7 File Offset: 0x001486B7
	public CallLimiterWithCooldown PlayerEffectLimiter
	{
		get
		{
			return this.playerEffectLimiter;
		}
	}

	// Token: 0x17000632 RID: 1586
	// (get) Token: 0x0600412B RID: 16683 RVA: 0x0014A4BF File Offset: 0x001486BF
	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	// Token: 0x17000633 RID: 1587
	// (get) Token: 0x0600412C RID: 16684 RVA: 0x0014A4C7 File Offset: 0x001486C7
	public List<RoomSystem.PlayerEffectConfig> PlayerEffects
	{
		get
		{
			return this.playerEffects;
		}
	}

	// Token: 0x17000634 RID: 1588
	// (get) Token: 0x0600412D RID: 16685 RVA: 0x0014A4CF File Offset: 0x001486CF
	public int PausedDCTimer
	{
		get
		{
			return this.pausedDCTimer;
		}
	}

	// Token: 0x04004CC1 RID: 19649
	[SerializeField]
	private ExpectedUsersDecayTimer expectedUsersTimer;

	// Token: 0x04004CC2 RID: 19650
	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	// Token: 0x04004CC3 RID: 19651
	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	// Token: 0x04004CC4 RID: 19652
	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	// Token: 0x04004CC5 RID: 19653
	[SerializeField]
	private CallLimiterWithCooldown playerEffectLimiter;

	// Token: 0x04004CC6 RID: 19654
	[SerializeField]
	private GameObject playerImpactEffect;

	// Token: 0x04004CC7 RID: 19655
	[SerializeField]
	private List<RoomSystem.PlayerEffectConfig> playerEffects = new List<RoomSystem.PlayerEffectConfig>();

	// Token: 0x04004CC8 RID: 19656
	[SerializeField]
	private int pausedDCTimer;
}
