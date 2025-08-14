using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public sealed class GorillaGuardianManager : GorillaGameManager
{
	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x06002B27 RID: 11047 RVA: 0x000E45BF File Offset: 0x000E27BF
	// (set) Token: 0x06002B28 RID: 11048 RVA: 0x000E45C7 File Offset: 0x000E27C7
	public bool isPlaying { get; private set; }

	// Token: 0x06002B29 RID: 11049 RVA: 0x000E45D0 File Offset: 0x000E27D0
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.isPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StartPlaying();
			}
		}
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x000E4634 File Offset: 0x000E2834
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.isPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StopPlaying();
			}
		}
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x000E4698 File Offset: 0x000E2898
	public override void Reset()
	{
		base.Reset();
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x000E46A0 File Offset: 0x000E28A0
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GuardianRPCs>();
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
	}

	// Token: 0x06002B2E RID: 11054 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06002B2F RID: 11055 RVA: 0x00058615 File Offset: 0x00056815
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x000E46B0 File Offset: 0x000E28B0
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this.IsPlayerGuardian(myPlayer) && !this.IsHoldingPlayer();
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x000E46C6 File Offset: 0x000E28C6
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return player != null && !this.IsPlayerGuardian(player);
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x000E46D8 File Offset: 0x000E28D8
	public bool IsPlayerGuardian(NetPlayer player)
	{
		using (List<GorillaGuardianZoneManager>.Enumerator enumerator = GorillaGuardianZoneManager.zoneManagers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsPlayerGuardian(player))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x000E4734 File Offset: 0x000E2934
	public void RequestEjectGuardian(NetPlayer player)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.EjectGuardian(player);
			return;
		}
		GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianRequestEject", false, Array.Empty<object>());
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x000E475C File Offset: 0x000E295C
	public void EjectGuardian(NetPlayer player)
	{
		foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
		{
			if (gorillaGuardianZoneManager.IsPlayerGuardian(player))
			{
				gorillaGuardianZoneManager.SetGuardian(null);
			}
		}
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x000E47B8 File Offset: 0x000E29B8
	public void LaunchPlayer(NetPlayer launcher, Vector3 velocity)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(launcher, out rigContainer))
		{
			return;
		}
		if (Vector3.Magnitude(VRRigCache.Instance.localRig.Rig.transform.position - rigContainer.Rig.transform.position) > this.requiredGuardianDistance + Mathf.Epsilon)
		{
			return;
		}
		if (velocity.sqrMagnitude > this.maxLaunchVelocity * this.maxLaunchVelocity)
		{
			return;
		}
		GTPlayer.Instance.DoLaunch(velocity);
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x000E483C File Offset: 0x000E2A3C
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		base.LocalTag(taggedPlayer, taggingPlayer, bodyHit, leftHand);
		if (bodyHit)
		{
			return;
		}
		RigContainer rigContainer;
		Vector3 vector;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && this.CheckSlap(taggingPlayer, taggedPlayer, leftHand, out vector))
		{
			GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", taggedPlayer, new object[]
			{
				vector
			});
			rigContainer.Rig.ApplyLocalTrajectoryOverride(vector);
			GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlapEffects", true, new object[]
			{
				rigContainer.Rig.transform.position,
				vector.normalized
			});
			this.LocalPlaySlapEffect(rigContainer.Rig.transform.position, vector.normalized);
		}
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000E4900 File Offset: 0x000E2B00
	private bool CheckSlap(NetPlayer slapper, NetPlayer target, bool leftHand, out Vector3 velocity)
	{
		velocity = Vector3.zero;
		if (this.IsHoldingPlayer(leftHand))
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(slapper, out rigContainer))
		{
			return false;
		}
		Vector3 vector = (leftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
		Vector3 rhs = leftHand ? rigContainer.Rig.leftHandHoldsPlayer.transform.right : rigContainer.Rig.rightHandHoldsPlayer.transform.right;
		if (Vector3.Dot(vector.normalized, rhs) < this.slapFrontAlignmentThreshold && Vector3.Dot(vector.normalized, rhs) > this.slapBackAlignmentThreshold)
		{
			return false;
		}
		if (vector.magnitude < this.launchMinimumStrength)
		{
			return false;
		}
		vector = Vector3.ClampMagnitude(vector, this.maxLaunchVelocity);
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(target, out rigContainer2))
		{
			return false;
		}
		if (this.IsRigBeingHeld(rigContainer2.Rig) || rigContainer2.Rig.IsLocalTrajectoryOverrideActive())
		{
			return false;
		}
		if (!this.CheckLaunchRetriggerDelay(rigContainer2.Rig))
		{
			return false;
		}
		vector *= this.launchStrengthMultiplier;
		Vector3 vector2;
		if (rigContainer2.Rig.IsOnGround(this.launchGroundHeadCheckDist, this.launchGroundHandCheckDist, out vector2))
		{
			vector += vector2 * this.launchGroundKickup * Mathf.Clamp01(1f - Vector3.Dot(vector2, vector.normalized));
		}
		velocity = vector;
		return true;
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000E4A7C File Offset: 0x000E2C7C
	public override void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
		base.HandleHandTap(tappingPlayer, hitTappable, leftHand, handVelocity, tapSurfaceNormal);
		if (hitTappable != null)
		{
			TappableGuardianIdol tappableGuardianIdol = hitTappable as TappableGuardianIdol;
			if (tappableGuardianIdol != null && tappableGuardianIdol.isActivationReady)
			{
				tappableGuardianIdol.isActivationReady = false;
				GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tapHapticStrength * this.hapticStrength, GorillaTagger.Instance.tapHapticDuration * this.hapticDuration);
			}
		}
		if (!this.IsPlayerGuardian(tappingPlayer))
		{
			return;
		}
		if (this.IsHoldingPlayer(leftHand))
		{
			return;
		}
		float num = Vector3.Dot(Vector3.down, handVelocity);
		if (num < this.slamTriggerTapSpeed || Vector3.Dot(Vector3.down, handVelocity.normalized) < this.slamTriggerAngle)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(tappingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand;
		Vector3 b = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * rigContainer.Rig.scaleFactor;
		Vector3 vector = vrmap.rigTarget.position - b;
		float num2 = Mathf.Clamp01((num - this.slamTriggerTapSpeed) / (this.slamMaxTapSpeed - this.slamTriggerTapSpeed));
		num2 = Mathf.Lerp(this.slamMinStrengthMultiplier, this.slamMaxStrengthMultiplier, num2);
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer2;
			if (RoomSystem.PlayersInRoom[i] != tappingPlayer && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer2))
			{
				VRRig rig = rigContainer2.Rig;
				if (!this.IsRigBeingHeld(rig) && this.CheckLaunchRetriggerDelay(rig))
				{
					Vector3 position = rig.transform.position;
					if (Vector3.SqrMagnitude(position - vector) < this.slamRadius * this.slamRadius)
					{
						Vector3 vector2 = (position - vector).normalized * num2;
						vector2 = Vector3.ClampMagnitude(vector2, this.maxLaunchVelocity);
						GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", RoomSystem.PlayersInRoom[i], new object[]
						{
							vector2
						});
					}
				}
			}
		}
		this.LocalPlaySlamEffect(vector, Vector3.up);
		GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlamEffect", true, new object[]
		{
			vector,
			Vector3.up
		});
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x000E4CEE File Offset: 0x000E2EEE
	private bool CheckLaunchRetriggerDelay(VRRig launchedRig)
	{
		return launchedRig.fxSettings.callSettings[7].CallLimitSettings.CheckCallTime(Time.time);
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x000E4D0C File Offset: 0x000E2F0C
	private bool IsHoldingPlayer()
	{
		return this.IsHoldingPlayer(true) || this.IsHoldingPlayer(false);
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x000E4D20 File Offset: 0x000E2F20
	private bool IsHoldingPlayer(bool leftHand)
	{
		return (leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment is HoldableHand) || (!leftHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment is HoldableHand);
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x000E4D7C File Offset: 0x000E2F7C
	private bool IsRigBeingHeld(VRRig rig)
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment != null)
		{
			HoldableHand holdableHand = EquipmentInteractor.instance.leftHandHeldEquipment as HoldableHand;
			if (holdableHand != null && holdableHand.Rig == rig)
			{
				return true;
			}
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null)
		{
			HoldableHand holdableHand2 = EquipmentInteractor.instance.rightHandHeldEquipment as HoldableHand;
			if (holdableHand2 != null && holdableHand2.Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x000E4DF0 File Offset: 0x000E2FF0
	public override GameModeType GameType()
	{
		return GameModeType.Guardian;
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x000E4DF3 File Offset: 0x000E2FF3
	public override string GameModeName()
	{
		return "GUARDIAN";
	}

	// Token: 0x06002B41 RID: 11073 RVA: 0x000E4DFA File Offset: 0x000E2FFA
	public void PlaySlapEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlapEffect(location, direction);
	}

	// Token: 0x06002B42 RID: 11074 RVA: 0x000E4E04 File Offset: 0x000E3004
	private void LocalPlaySlapEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slapImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x000E4E1F File Offset: 0x000E301F
	public void PlaySlamEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlamEffect(location, direction);
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x000E4E29 File Offset: 0x000E3029
	private void LocalPlaySlamEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slamImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x0400368C RID: 13964
	[Space]
	[SerializeField]
	private float slapFrontAlignmentThreshold = 0.7f;

	// Token: 0x0400368D RID: 13965
	[SerializeField]
	private float slapBackAlignmentThreshold = 0.7f;

	// Token: 0x0400368E RID: 13966
	[SerializeField]
	private float launchMinimumStrength = 6f;

	// Token: 0x0400368F RID: 13967
	[SerializeField]
	private float launchStrengthMultiplier = 1f;

	// Token: 0x04003690 RID: 13968
	[SerializeField]
	private float launchGroundHeadCheckDist = 1.2f;

	// Token: 0x04003691 RID: 13969
	[SerializeField]
	private float launchGroundHandCheckDist = 0.4f;

	// Token: 0x04003692 RID: 13970
	[SerializeField]
	private float launchGroundKickup = 3f;

	// Token: 0x04003693 RID: 13971
	[Space]
	[SerializeField]
	private float slamTriggerTapSpeed = 7f;

	// Token: 0x04003694 RID: 13972
	[SerializeField]
	private float slamMaxTapSpeed = 16f;

	// Token: 0x04003695 RID: 13973
	[SerializeField]
	private float slamTriggerAngle = 0.7f;

	// Token: 0x04003696 RID: 13974
	[SerializeField]
	private float slamRadius = 2.4f;

	// Token: 0x04003697 RID: 13975
	[SerializeField]
	private float slamMinStrengthMultiplier = 3f;

	// Token: 0x04003698 RID: 13976
	[SerializeField]
	private float slamMaxStrengthMultiplier = 10f;

	// Token: 0x04003699 RID: 13977
	[Space]
	[SerializeField]
	private GameObject slapImpactPrefab;

	// Token: 0x0400369A RID: 13978
	[SerializeField]
	private GameObject slamImpactPrefab;

	// Token: 0x0400369B RID: 13979
	[Space]
	[SerializeField]
	private float hapticStrength = 1f;

	// Token: 0x0400369C RID: 13980
	[SerializeField]
	private float hapticDuration = 1f;

	// Token: 0x0400369E RID: 13982
	private float requiredGuardianDistance = 10f;

	// Token: 0x0400369F RID: 13983
	private float maxLaunchVelocity = 20f;
}
