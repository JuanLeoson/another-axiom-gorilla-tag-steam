using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A57 RID: 2647
internal class GuardianRPCs : RPCNetworkBase
{
	// Token: 0x06004096 RID: 16534 RVA: 0x00146FB0 File Offset: 0x001451B0
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.guardianManager = (GorillaGuardianManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06004097 RID: 16535 RVA: 0x00146FCC File Offset: 0x001451CC
	[PunRPC]
	public void GuardianRequestEject(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GuardianRequestEject");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (photonMessageInfoWrapped.Sender != null)
		{
			this.guardianManager.EjectGuardian(photonMessageInfoWrapped.Sender);
		}
	}

	// Token: 0x06004098 RID: 16536 RVA: 0x00147008 File Offset: 0x00145208
	[PunRPC]
	public void GuardianLaunchPlayer(Vector3 velocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GuardianLaunchPlayer");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent LaunchPlayer when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (!velocity.IsValid(num))
		{
			return;
		}
		if (!this.launchCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.guardianManager.LaunchPlayer(photonMessageInfoWrapped.Sender, velocity);
	}

	// Token: 0x06004099 RID: 16537 RVA: 0x001470A0 File Offset: 0x001452A0
	[PunRPC]
	public void ShowSlapEffects(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ShowSlapEffects");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent ShowSlapEffects when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (location.IsValid(num))
		{
			float num2 = 10000f;
			if (direction.IsValid(num2))
			{
				if (!this.slapFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlapEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x0600409A RID: 16538 RVA: 0x00147144 File Offset: 0x00145344
	[PunRPC]
	public void ShowSlamEffect(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ShowSlamEffect");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			GorillaNot.instance.SendReport("Sent ShowSlamEffect when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (location.IsValid(num))
		{
			float num2 = 10000f;
			if (direction.IsValid(num2))
			{
				if (!this.slamFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlamEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x04004C40 RID: 19520
	private GameModeSerializer serializer;

	// Token: 0x04004C41 RID: 19521
	private GorillaGuardianManager guardianManager;

	// Token: 0x04004C42 RID: 19522
	private CallLimiter launchCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04004C43 RID: 19523
	private CallLimiter slapFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04004C44 RID: 19524
	private CallLimiter slamFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);
}
