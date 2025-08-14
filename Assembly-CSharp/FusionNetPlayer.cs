using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class FusionNetPlayer : NetPlayer
{
	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06001016 RID: 4118 RVA: 0x0005D4FC File Offset: 0x0005B6FC
	// (set) Token: 0x06001017 RID: 4119 RVA: 0x0005D504 File Offset: 0x0005B704
	public PlayerRef PlayerRef { get; private set; }

	// Token: 0x06001018 RID: 4120 RVA: 0x0005D510 File Offset: 0x0005B710
	public FusionNetPlayer()
	{
		this.PlayerRef = default(PlayerRef);
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x0005D532 File Offset: 0x0005B732
	public FusionNetPlayer(PlayerRef playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x0600101A RID: 4122 RVA: 0x0005D541 File Offset: 0x0005B741
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x0600101B RID: 4123 RVA: 0x0005D554 File Offset: 0x0005B754
	public override bool IsValid
	{
		get
		{
			return this.validPlayer && this.PlayerRef.IsRealPlayer;
		}
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x0600101C RID: 4124 RVA: 0x0005D57C File Offset: 0x0005B77C
	public override int ActorNumber
	{
		get
		{
			return this.PlayerRef.PlayerId;
		}
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x0600101D RID: 4125 RVA: 0x0005D598 File Offset: 0x0005B798
	public override string UserId
	{
		get
		{
			return NetworkSystem.Instance.GetUserID(this.PlayerRef.PlayerId);
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x0600101E RID: 4126 RVA: 0x0005D5C0 File Offset: 0x0005B7C0
	public override bool IsMasterClient
	{
		get
		{
			if (!(this.runner == null))
			{
				return (this.IsLocal && this.runner.IsSharedModeMasterClient) || NetworkSystem.Instance.MasterClient == this;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x0600101F RID: 4127 RVA: 0x0005D614 File Offset: 0x0005B814
	public override bool IsLocal
	{
		get
		{
			if (!(this.runner == null))
			{
				return this.PlayerRef == this.runner.LocalPlayer;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06001020 RID: 4128 RVA: 0x0005D65A File Offset: 0x0005B85A
	public override bool IsNull
	{
		get
		{
			PlayerRef playerRef = this.PlayerRef;
			return false;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06001021 RID: 4129 RVA: 0x0005D664 File Offset: 0x0005B864
	public override string NickName
	{
		get
		{
			return NetworkSystem.Instance.GetNickName(this);
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06001022 RID: 4130 RVA: 0x0005D674 File Offset: 0x0005B874
	public override string DefaultName
	{
		get
		{
			if (string.IsNullOrEmpty(this._defaultName))
			{
				this._defaultName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			}
			return this._defaultName;
		}
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06001023 RID: 4131 RVA: 0x0005D6C0 File Offset: 0x0005B8C0
	public override bool InRoom
	{
		get
		{
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.PlayerRef)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x0005D720 File Offset: 0x0005B920
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((FusionNetPlayer)myPlayer).PlayerRef.Equals(((FusionNetPlayer)other).PlayerRef);
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x0005D753 File Offset: 0x0005B953
	public void InitPlayer(PlayerRef player)
	{
		this.PlayerRef = player;
		this.validPlayer = true;
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0005D764 File Offset: 0x0005B964
	public override void OnReturned()
	{
		base.OnReturned();
		this.PlayerRef = default(PlayerRef);
		if (this.PlayerRef.PlayerId != -1)
		{
			Debug.LogError("Returned Player to pool but isnt -1, broken");
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0005D7A1 File Offset: 0x0005B9A1
	public override void OnTaken()
	{
		base.OnTaken();
	}

	// Token: 0x0400187B RID: 6267
	private string _defaultName;

	// Token: 0x0400187C RID: 6268
	private bool validPlayer;
}
