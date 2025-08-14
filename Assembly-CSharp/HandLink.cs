using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.ModIO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200074A RID: 1866
public class HandLink : HoldableObject, IGorillaSliceableSimple
{
	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06002EAE RID: 11950 RVA: 0x000F7163 File Offset: 0x000F5363
	// (set) Token: 0x06002EAF RID: 11951 RVA: 0x000F716B File Offset: 0x000F536B
	public bool IsLocal { get; private set; }

	// Token: 0x06002EB0 RID: 11952 RVA: 0x000F7174 File Offset: 0x000F5374
	private void Start()
	{
		this.myOtherHandLink = (this.isLeftHand ? this.myRig.rightHandLink : this.myRig.leftHandLink);
		if (this.myRig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			this.IsLocal = true;
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	// Token: 0x06002EB1 RID: 11953 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002EB2 RID: 11954 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x000F71E4 File Offset: 0x000F53E4
	public void SliceUpdate()
	{
		this.interactionPoint.enabled = (this.canBeGrabbed && (this.myRig.transform.position - VRRig.LocalRig.transform.position).sqrMagnitude < 9f);
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x000F723C File Offset: 0x000F543C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.CanBeGrabbed())
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			(this.isLeftHand ? this.myRig.leftHolds : this.myRig.rightHolds).OnGrab(pointGrabbed, grabbingHand);
			return;
		}
		HandLink handLink = (grabbingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
		if (handLink.canBeGrabbed && Time.time - handLink.gripPressedAtTimestamp < 0.1f)
		{
			handLink.CreateLink(this);
		}
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000F72E8 File Offset: 0x000F54E8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.myRig.isOfflineVRRig)
		{
			HandLink handLink = (releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			bool flag = false;
			HandLinkAuthorityStatus selfHandLinkAuthority = GTPlayer.Instance.GetSelfHandLinkAuthority();
			int num;
			HandLinkAuthorityStatus chainAuthority = handLink.GetChainAuthority(out num);
			if (selfHandLinkAuthority.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < selfHandLinkAuthority.type)
			{
				flag = true;
			}
			else if (handLink.myOtherHandLink.grabbedLink != null)
			{
				int num2;
				HandLinkAuthorityStatus chainAuthority2 = handLink.myOtherHandLink.GetChainAuthority(out num2);
				if (chainAuthority2.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < chainAuthority2.type)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Vector3 averageVelocity = (handLink.isLeftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
				this.myRig.netView.SendRPC("DroppedByPlayer", this.myRig.OwningNetPlayer, new object[]
				{
					averageVelocity
				});
				this.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
			}
			handLink.BreakLink();
		}
		return true;
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x000F741D File Offset: 0x000F561D
	public override void DropItemCleanup()
	{
		if (this.grabbedLink != null)
		{
			this.grabbedLink.BreakLink();
		}
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x000F7438 File Offset: 0x000F5638
	public bool CanBeGrabbed()
	{
		return (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.IsLocalPlayerInVirtualStump() || CustomMapLoader.IsModLoaded(0L)) && Time.time >= this.rejectGrabsUntilTimestamp && this.canBeGrabbed && this.grabbedPlayer == null;
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x000F7487 File Offset: 0x000F5687
	public bool IsLinkActive()
	{
		return this.grabbedLink != null;
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x000F7498 File Offset: 0x000F5698
	private void CreateLink(HandLink remoteLink)
	{
		if (this.grabbedPlayer != null || !this.myRig.isLocal)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(remoteLink, this.isLeftHand);
		this.grabbedLink = remoteLink;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
		(this.isLeftHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer).GTPlayOneShot(this.audioOnGrab, 1f);
		Action onHandLinkChanged = HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x000F758E File Offset: 0x000F578E
	public void BreakLinkTo(HandLink targetLink)
	{
		if (this.grabbedLink == targetLink)
		{
			this.BreakLink();
		}
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x000F75A4 File Offset: 0x000F57A4
	public void BreakLink()
	{
		if (this.grabbedPlayer == null || this.grabbedLink == null)
		{
			return;
		}
		Vector3 velocity = this.myRig.LatestVelocity();
		GTPlayer.Instance.SetVelocity(velocity);
		this.grabbedLink = null;
		this.grabbedPlayer = null;
		this.grabbedHandIsLeft = false;
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isLeftHand);
		Action onHandLinkChanged = HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x000F7618 File Offset: 0x000F5818
	public static bool IsHandInChainWithOtherPlayer(HandLink startingLink, int targetPlayer)
	{
		HandLink handLink = startingLink;
		int num = 0;
		int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		while (handLink != null && num < roomPlayerCount)
		{
			if (handLink.myRig == null || handLink.myRig.creator == null)
			{
				return false;
			}
			if (handLink.myRig.creator.ActorNumber == targetPlayer)
			{
				return true;
			}
			HandLink handLink2 = null;
			RigContainer rigContainer;
			if (handLink.grabbedLink != null && handLink.grabbedLink.myOtherHandLink != null)
			{
				handLink2 = handLink.grabbedLink.myOtherHandLink;
			}
			else if (handLink.grabbedPlayer != null && VRRigCache.Instance.TryGetVrrig(handLink.grabbedPlayer, out rigContainer))
			{
				HandLink handLink3 = handLink.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
				if (handLink3 != null && handLink3.myOtherHandLink != null)
				{
					handLink2 = handLink3.myOtherHandLink;
				}
			}
			handLink = handLink2;
			num++;
		}
		return false;
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x000F7714 File Offset: 0x000F5914
	public void LocalUpdate(bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool canBeGrabbed)
	{
		if (isGripPressed && !this.wasGripPressed)
		{
			this.gripPressedAtTimestamp = Time.time;
		}
		this.wasGripPressed = isGripPressed;
		this.canBeGrabbed = canBeGrabbed;
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		if (this.grabbedLink != null)
		{
			if (!this.grabbedLink.canBeGrabbed && this.grabbedLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				this.BreakLink();
				return;
			}
			if (!isGripPressed || !this.grabbedLink.myRig.gameObject.activeSelf)
			{
				this.BreakLink();
				return;
			}
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(this.grabbedPlayer))
			{
				this.BreakLink();
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.grabbedLink.myRig);
			GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
			if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
			{
				this.BreakLink();
				return;
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.IsLocalPlayerInVirtualStump() && !CustomMapLoader.IsModLoaded(0L))
			{
				this.BreakLink();
				return;
			}
		}
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x000F7847 File Offset: 0x000F5A47
	public void RejectGrabsFor(float duration)
	{
		this.rejectGrabsUntilTimestamp = Mathf.Max(this.rejectGrabsUntilTimestamp, Time.time + duration);
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000F7861 File Offset: 0x000F5A61
	public void Write(out bool isGroundedHand, out bool isGroundedButt, out int grabbedPlayerActorNumber, out bool grabbedHandIsLeft)
	{
		isGroundedHand = this.isGroundedHand;
		isGroundedButt = this.isGroundedButt;
		if (this.grabbedPlayer != null)
		{
			grabbedPlayerActorNumber = this.grabbedPlayer.ActorNumber;
			grabbedHandIsLeft = this.grabbedHandIsLeft;
			return;
		}
		grabbedPlayerActorNumber = 0;
		grabbedHandIsLeft = false;
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000F789C File Offset: 0x000F5A9C
	public void Read(Vector3 remoteHandLocalPos, Quaternion remoteBodyWorldRot, Vector3 remoteBodyWorldPos, bool isGroundedHand, bool isGroundedButt, bool isGripReady, int grabbedPlayerActorNumber, bool grabbedHandIsLeft)
	{
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		this.canBeGrabbed = isGripReady;
		if (grabbedPlayerActorNumber == 0)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
			}
			bool flag = this.grabbedPlayer != null;
			this.grabbedPlayer = null;
			this.grabbedLink = null;
			if (flag)
			{
				Action onHandLinkChanged = HandLink.OnHandLinkChanged;
				if (onHandLinkChanged != null)
				{
					onHandLinkChanged();
				}
			}
		}
		else if (this.lastReadGrabbedPlayerActorNumber == grabbedPlayerActorNumber)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsValid && this.grabbedPlayer.ActorNumber == grabbedPlayerActorNumber && this.grabbedPlayer.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 7f))
			{
				if (this.grabbedHandIsLeft)
				{
					VRRig.LocalRig.leftHandLink.BreakLink();
				}
				else
				{
					VRRig.LocalRig.rightHandLink.BreakLink();
				}
			}
		}
		else
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this);
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(grabbedPlayerActorNumber);
			if (player != null)
			{
				if (player.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 0.25f))
				{
					bool flag2 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag2)
					{
						Action onHandLinkChanged2 = HandLink.OnHandLinkChanged;
						if (onHandLinkChanged2 != null)
						{
							onHandLinkChanged2();
						}
					}
				}
				else if (player == this.myRig.OwningNetPlayer)
				{
					bool flag3 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag3)
					{
						Action onHandLinkChanged3 = HandLink.OnHandLinkChanged;
						if (onHandLinkChanged3 != null)
						{
							onHandLinkChanged3();
						}
					}
				}
				else
				{
					this.grabbedPlayer = player;
					this.grabbedHandIsLeft = grabbedHandIsLeft;
					this.CheckFormLinkWithRemoteGrab();
					Action onHandLinkChanged4 = HandLink.OnHandLinkChanged;
					if (onHandLinkChanged4 != null)
					{
						onHandLinkChanged4();
					}
				}
			}
			else
			{
				bool flag4 = this.grabbedPlayer != null;
				this.grabbedPlayer = null;
				this.grabbedLink = null;
				if (flag4)
				{
					Action onHandLinkChanged5 = HandLink.OnHandLinkChanged;
					if (onHandLinkChanged5 != null)
					{
						onHandLinkChanged5();
					}
				}
			}
		}
		this.lastReadGrabbedPlayerActorNumber = grabbedPlayerActorNumber;
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x000F7AE3 File Offset: 0x000F5CE3
	private bool IsLocalGrabInRange(bool grabbedLeftHand, Vector3 handLocalPos, Quaternion bodyWorldRot, Vector3 bodyWorldPos, float tolerance)
	{
		return ((grabbedLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).transform.position - (bodyWorldPos + bodyWorldRot * handLocalPos)).IsShorterThan(tolerance);
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000F7B24 File Offset: 0x000F5D24
	private void CheckFormLinkWithRemoteGrab()
	{
		RigContainer rigContainer;
		if (this.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			HandLink handLink = this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			if (handLink.canBeGrabbed && Time.time > handLink.rejectGrabsUntilTimestamp)
			{
				handLink.CreateLink(this);
				return;
			}
		}
		else if (VRRigCache.Instance.TryGetVrrig(this.grabbedPlayer, out rigContainer))
		{
			HandLink handLink2 = this.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
			if (handLink2.grabbedPlayer == this.myRig.creator)
			{
				this.grabbedLink = handLink2;
				this.grabbedLink.grabbedLink = this;
			}
		}
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000F7BDC File Offset: 0x000F5DDC
	public HandLinkAuthorityStatus GetChainAuthority(out int stepsToAuth)
	{
		HandLink handLink = this.grabbedLink;
		int num = 1;
		HandLinkAuthorityStatus handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, -1f, -1);
		stepsToAuth = -1;
		while (handLink != null && num < 10 && !handLink.IsLocal)
		{
			if (handLink.isGroundedHand)
			{
				stepsToAuth = num;
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded, -1f, -1);
			}
			if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ResidualHandGrounded && (double)(handLink.myRig.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, handLink.myRig.LastHandTouchedGroundAtNetworkTime, handLink.myRig.OwningNetPlayer.ActorNumber);
			}
			else if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ButtGrounded && handLink.isGroundedButt)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded, -1f, -1);
			}
			else if (handLinkAuthorityStatus.type == HandLinkAuthorityType.None)
			{
				HandLinkAuthorityStatus handLinkAuthorityStatus2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, handLink.myRig.LastTouchedGroundAtNetworkTime, handLink.myRig.OwningNetPlayer.ActorNumber);
				if (handLinkAuthorityStatus2 > handLinkAuthorityStatus)
				{
					stepsToAuth = num;
					handLinkAuthorityStatus = handLinkAuthorityStatus2;
				}
			}
			num++;
			handLink = handLink.myOtherHandLink.grabbedLink;
		}
		return handLinkAuthorityStatus;
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000F7CF4 File Offset: 0x000F5EF4
	public void SnapHandsTogether()
	{
		if (this.grabbedLink == null)
		{
			return;
		}
		if (this.grabbedLink.snapPositionCalculatedAtFrame == Time.frameCount)
		{
			this.snapPositionCalculatedAtFrame = Time.frameCount;
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = this.grabbedLink.transform.position;
		Vector3 a = (position + position2) / 2f;
		Vector3 b = (this.isLeftHand ? this.myRig.leftHand.rigTarget : this.myRig.rightHand.rigTarget).position - position;
		Vector3 b2 = (this.grabbedLink.isLeftHand ? this.grabbedLink.myRig.leftHand.rigTarget : this.grabbedLink.myRig.rightHand.rigTarget).position - position2;
		Vector3 targetWorldPos = a + b;
		Vector3 targetWorldPos2 = a + b2;
		this.myIK.OverrideTargetPos(this.isLeftHand, targetWorldPos);
		this.grabbedLink.myIK.OverrideTargetPos(this.grabbedLink.isLeftHand, targetWorldPos2);
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x000F7E18 File Offset: 0x000F6018
	public void PlayVicariousTapHaptic()
	{
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnVicariousTap, this.hapticDurationOnVicariousTap);
	}

	// Token: 0x04003AA4 RID: 15012
	[FormerlySerializedAs("myPlayer")]
	[SerializeField]
	public VRRig myRig;

	// Token: 0x04003AA5 RID: 15013
	[FormerlySerializedAs("leftHand")]
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x04003AA6 RID: 15014
	[SerializeField]
	public GorillaIK myIK;

	// Token: 0x04003AA7 RID: 15015
	private HandLink myOtherHandLink;

	// Token: 0x04003AA8 RID: 15016
	private bool canBeGrabbed;

	// Token: 0x04003AA9 RID: 15017
	public bool isGroundedHand;

	// Token: 0x04003AAA RID: 15018
	public bool isGroundedButt;

	// Token: 0x04003AAB RID: 15019
	private bool wasGripPressed;

	// Token: 0x04003AAC RID: 15020
	private float gripPressedAtTimestamp;

	// Token: 0x04003AAD RID: 15021
	private float rejectGrabsUntilTimestamp;

	// Token: 0x04003AAE RID: 15022
	public HandLink grabbedLink;

	// Token: 0x04003AAF RID: 15023
	public NetPlayer grabbedPlayer;

	// Token: 0x04003AB0 RID: 15024
	public bool grabbedHandIsLeft;

	// Token: 0x04003AB2 RID: 15026
	private const bool DEBUG_GRAB_ANYONE = false;

	// Token: 0x04003AB3 RID: 15027
	[SerializeField]
	private float hapticStrengthOnGrab;

	// Token: 0x04003AB4 RID: 15028
	[SerializeField]
	private float hapticDurationOnGrab;

	// Token: 0x04003AB5 RID: 15029
	[SerializeField]
	private float hapticStrengthOnVicariousTap;

	// Token: 0x04003AB6 RID: 15030
	[SerializeField]
	private float hapticDurationOnVicariousTap;

	// Token: 0x04003AB7 RID: 15031
	[SerializeField]
	private AudioClip audioOnGrab;

	// Token: 0x04003AB8 RID: 15032
	public InteractionPoint interactionPoint;

	// Token: 0x04003AB9 RID: 15033
	public static Action OnHandLinkChanged;

	// Token: 0x04003ABA RID: 15034
	private int lastReadGrabbedPlayerActorNumber;

	// Token: 0x04003ABB RID: 15035
	private int snapPositionCalculatedAtFrame = -1;
}
