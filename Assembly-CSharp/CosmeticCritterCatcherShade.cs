using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class CosmeticCritterCatcherShade : CosmeticCritterCatcher
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000438 RID: 1080 RVA: 0x00018C00 File Offset: 0x00016E00
	// (set) Token: 0x06000439 RID: 1081 RVA: 0x00018C08 File Offset: 0x00016E08
	public Vector3 LastTargetPosition { get; private set; }

	// Token: 0x0600043A RID: 1082 RVA: 0x00018C11 File Offset: 0x00016E11
	public float GetActionTimeFrac()
	{
		return this.targetHoldTime / this.maxHoldTime;
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00018C20 File Offset: 0x00016E20
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 0.25f, 0.5f);
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00018C34 File Offset: 0x00016E34
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (this.heartbeatCooldown > 0.5f || (this.currentTarget != null && this.currentTarget != critter))
		{
			return CosmeticCritterAction.None;
		}
		if (critter is CosmeticCritterShadeFleeing && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 0f))
		{
			if (this.targetHoldTime >= this.minSecondsLockedToCatch && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
		else
		{
			if (!(critter is CosmeticCritterShadeHidden) || !this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 0f))
			{
				return CosmeticCritterAction.None;
			}
			if (this.targetHoldTime >= this.secondsToReveal)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00018D00 File Offset: 0x00016F00
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		if (!base.ValidateRemoteCatchAction(critter, catchAction, serverTime))
		{
			return false;
		}
		if (critter is CosmeticCritterShadeFleeing)
		{
			if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius + 1f && this.targetHoldTime >= this.minSecondsLockedToCatch * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 2f))
			{
				return true;
			}
		}
		else if (critter is CosmeticCritterShadeHidden)
		{
			if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None && this.targetHoldTime >= this.secondsToReveal * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 2f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x00018DD4 File Offset: 0x00016FD4
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.currentTarget = critter;
		float num = PhotonNetwork.InRoom ? ((float)(PhotonNetwork.Time - serverTime)) : 0f;
		this.heartbeatCooldown = 1f + num;
		this.targetHoldTime += num;
		if (!(critter is CosmeticCritterShadeFleeing))
		{
			if (critter is CosmeticCritterShadeHidden)
			{
				this.maxHoldTime = this.secondsToReveal;
				if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None)
				{
					(this.optionalLinkedSpawner as CosmeticCritterSpawnerShadeFleeing).SetSpawnPosition(critter.transform.position);
					this.currentTarget = null;
					this.targetHoldTime = 0f;
				}
			}
			return;
		}
		this.maxHoldTime = this.minSecondsLockedToCatch;
		if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
		{
			this.shadeRevealer.ShadeCaught();
			this.currentTarget = null;
			this.targetHoldTime = 0f;
			return;
		}
		CosmeticCritterAction cosmeticCritterAction = catchAction & CosmeticCritterAction.ShadeHeartbeat;
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00018E9E File Offset: 0x0001709E
	protected override void Awake()
	{
		base.Awake();
		this.shadeRevealer = (this.transferrableObject as ShadeRevealer);
		this.maxHoldTime = Mathf.Max(this.secondsToReveal, this.minSecondsLockedToCatch);
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00018ED0 File Offset: 0x000170D0
	protected void LateUpdate()
	{
		if (this.heartbeatCooldown > 0f)
		{
			this.heartbeatCooldown -= Time.deltaTime;
			if (this.heartbeatCooldown < 0f)
			{
				this.heartbeatCooldown = 0f;
				this.currentTarget = null;
				return;
			}
			this.targetHoldTime = Mathf.Min(this.targetHoldTime + Time.deltaTime, this.maxHoldTime);
			if (this.currentTarget is CosmeticCritterShadeFleeing)
			{
				if (!base.IsLocal || this.heartbeatCooldown > 0.4f)
				{
					this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.LOCKED);
				}
				Vector3 normalized = (this.catchOrigin.position - this.currentTarget.transform.position).normalized;
				(this.currentTarget as CosmeticCritterShadeFleeing).pullVector += this.vacuumSpeed * Time.deltaTime * normalized;
				return;
			}
			if (this.currentTarget is CosmeticCritterShadeHidden && (!base.IsLocal || this.heartbeatCooldown > 0.4f))
			{
				this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.TRACKING);
				return;
			}
		}
		else if (this.targetHoldTime > 0f)
		{
			this.targetHoldTime = Mathf.Max(this.targetHoldTime - Time.deltaTime, 0f);
		}
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00019019 File Offset: 0x00017219
	protected override void OnEnable()
	{
		base.OnEnable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x0001903E File Offset: 0x0001723E
	protected override void OnDisable()
	{
		base.OnDisable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x040004CA RID: 1226
	[SerializeField]
	private float secondsToReveal = 1f;

	// Token: 0x040004CB RID: 1227
	[SerializeField]
	private float minSecondsLockedToCatch = 1f;

	// Token: 0x040004CC RID: 1228
	[SerializeField]
	private Transform catchOrigin;

	// Token: 0x040004CD RID: 1229
	[SerializeField]
	private float catchRadius = 1f;

	// Token: 0x040004CE RID: 1230
	[SerializeField]
	private float vacuumSpeed = 3f;

	// Token: 0x040004CF RID: 1231
	private ShadeRevealer shadeRevealer;

	// Token: 0x040004D0 RID: 1232
	private CosmeticCritter currentTarget;

	// Token: 0x040004D1 RID: 1233
	private float targetHoldTime;

	// Token: 0x040004D2 RID: 1234
	private float maxHoldTime;

	// Token: 0x040004D4 RID: 1236
	private const float HEARTBEAT_DELAY = 1f;

	// Token: 0x040004D5 RID: 1237
	private float heartbeatCooldown;
}
