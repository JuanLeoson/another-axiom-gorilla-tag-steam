using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.Fusion;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020006B5 RID: 1717
[NetworkBehaviourWeaved(35)]
internal class VRRigSerializer : GorillaWrappedSerializer, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x06002A59 RID: 10841 RVA: 0x000E20AD File Offset: 0x000E02AD
	// (set) Token: 0x06002A5A RID: 10842 RVA: 0x000E20D7 File Offset: 0x000E02D7
	[Networked]
	[NetworkedWeaved(0, 17)]
	public unsafe NetworkString<_16> nickName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06002A5B RID: 10843 RVA: 0x000E2102 File Offset: 0x000E0302
	// (set) Token: 0x06002A5C RID: 10844 RVA: 0x000E2130 File Offset: 0x000E0330
	[Networked]
	[NetworkedWeaved(17, 17)]
	public unsafe NetworkString<_16> defaultName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 17);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 17) = value;
		}
	}

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x06002A5D RID: 10845 RVA: 0x000E215F File Offset: 0x000E035F
	// (set) Token: 0x06002A5E RID: 10846 RVA: 0x000E218D File Offset: 0x000E038D
	[Networked]
	[NetworkedWeaved(34, 1)]
	public bool tutorialComplete
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 34);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 34, value);
		}
	}

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x06002A5F RID: 10847 RVA: 0x000E21BC File Offset: 0x000E03BC
	private PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
	}

	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x06002A60 RID: 10848 RVA: 0x000E21C4 File Offset: 0x000E03C4
	public VRRig VRRig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06002A61 RID: 10849 RVA: 0x000E21CC File Offset: 0x000E03CC
	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000E21DC File Offset: 0x000E03DC
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (this.netView.IsRoomView)
		{
			if (player != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player.UserId, player.NickName);
			}
			return false;
		}
		if (NetworkSystem.Instance.IsObjectRoomObject(base.gameObject))
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
			if (player2 != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player2.UserId, player2.NickName);
			}
			return false;
		}
		if (player != this.netView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", player.UserId, player.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x000E22E4 File Offset: 0x000E04E4
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		bool initialized = this.rigContainer.Initialized;
		this.rigContainer.InitializeNetwork(this.netView, this.Voice, this);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.SetupLoudSpeakerNetwork(this.rigContainer);
		this.netView.GetView.AddCallbackTarget(this);
		if (!initialized)
		{
			object[] instantiationData = info.punInfo.photonView.InstantiationData;
			float red = 0f;
			float green = 0f;
			float blue = 0f;
			if (instantiationData != null && instantiationData.Length == 3)
			{
				object obj = instantiationData[0];
				if (obj is float)
				{
					float value = (float)obj;
					obj = instantiationData[1];
					if (obj is float)
					{
						float value2 = (float)obj;
						obj = instantiationData[2];
						if (obj is float)
						{
							float value3 = (float)obj;
							red = value.ClampSafe(0f, 1f);
							green = value2.ClampSafe(0f, 1f);
							blue = value3.ClampSafe(0f, 1f);
						}
					}
				}
			}
			this.vrrig.InitializeNoobMaterialLocal(red, green, blue);
		}
		NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000E242C File Offset: 0x000E062C
	protected override void OnBeforeDespawn()
	{
		this.CleanUp(true);
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x000E2438 File Offset: 0x000E0638
	private void CleanUp(bool netDestroy)
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		this.successfullInstantiate = false;
		if (this.vrrig != null)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.vrrig.isOfflineVRRig)
				{
					this.vrrig.ChangeMaterialLocal(0);
				}
			}
			else if (this.vrrig.isOfflineVRRig)
			{
				NetworkSystem.Instance.NetDestroy(base.gameObject);
			}
			if (this.vrrig.netView == this.netView)
			{
				this.vrrig.netView = null;
			}
			if (this.vrrig.rigSerializer == this)
			{
				this.vrrig.rigSerializer = null;
			}
		}
		if (this.networkSpeaker != null)
		{
			this.CleanupLoudSpeakerNetwork();
			if (netDestroy)
			{
				this.networkSpeaker.SetParent(base.transform, false);
			}
			else
			{
				this.networkSpeaker.SetParent(null);
			}
			this.networkSpeaker.gameObject.SetActive(false);
		}
		this.vrrig = null;
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x000E252F File Offset: 0x000E072F
	private void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.CleanUp(false);
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000E253E File Offset: 0x000E073E
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			UnityEngine.Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x000E257C File Offset: 0x000E077C
	[PunRPC]
	public void RPC_InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfo info)
	{
		this.InitializeNoobMaterialShared(red, green, blue, info);
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x000E258E File Offset: 0x000E078E
	[PunRPC]
	public void RPC_RequestCosmetics(PhotonMessageInfo info)
	{
		this.RequestCosmeticsShared(info);
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x000E259C File Offset: 0x000E079C
	[PunRPC]
	public void RPC_PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.PlayDrumShared(drumIndex, drumVolume, info);
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x000E25AC File Offset: 0x000E07AC
	[PunRPC]
	public void RPC_PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.PlaySelfOnlyInstrumentShared(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000E25BE File Offset: 0x000E07BE
	[PunRPC]
	public void RPC_PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.PlayHandTapShared(soundIndex, isLeftHand, tapVolume, info);
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x000E25D0 File Offset: 0x000E07D0
	public void RPC_UpdateNativeSize(float value, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.UpdateNativeSizeShared(value, info);
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000023F5 File Offset: 0x000005F5
	public void RPC_UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000023F5 File Offset: 0x000005F5
	public void RPC_UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x000E25DF File Offset: 0x000E07DF
	[PunRPC]
	public void RPC_UpdateCosmeticsWithTryonPacked(int[] currentItemsPacked, int[] tryOnItemsPacked, PhotonMessageInfo info)
	{
		this.UpdateCosmeticsWithTryonShared(currentItemsPacked, tryOnItemsPacked, info);
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x000E25EF File Offset: 0x000E07EF
	[PunRPC]
	public void RPC_HideAllCosmetics(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.HideAllCosmetics(info);
	}

	// Token: 0x06002A73 RID: 10867 RVA: 0x000E2602 File Offset: 0x000E0802
	[PunRPC]
	public void RPC_PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.PlaySplashEffectShared(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x000E261A File Offset: 0x000E081A
	[PunRPC]
	public void RPC_PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		this.PlayGeodeEffectShared(hitPosition, info);
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x000E2629 File Offset: 0x000E0829
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		this.EnableNonCosmeticHandItemShared(enable, isLeftHand, info);
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x000E2639 File Offset: 0x000E0839
	[PunRPC]
	public void OnHandTapRPC(int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfo info)
	{
		this.OnHandTapRPCShared(audioClipIndex, isDownTap, isLeftHand, handTapSpeed, packedDirFromHitToHand, info);
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x000E264F File Offset: 0x000E084F
	[PunRPC]
	public void RPC_UpdateQuestScore(int score, PhotonMessageInfo info)
	{
		this.UpdateQuestScore(score, info);
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x000E265E File Offset: 0x000E085E
	[PunRPC]
	public void RPC_UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfo info)
	{
		this.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x000E2670 File Offset: 0x000E0870
	private void SetupLoudSpeakerNetwork(RigContainer rigContainer)
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.AddSpeaker(component);
		}
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x000E26E8 File Offset: 0x000E08E8
	private void CleanupLoudSpeakerNetwork()
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in this.rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.RemoveSpeaker(component);
		}
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x000E2764 File Offset: 0x000E0964
	public void BroadcastLoudSpeakerNetwork(bool toggleBroadcast, int actorNumber)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
		{
			return;
		}
		bool isLocal = actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.BroadcastLoudSpeakerNetworkShared(toggleBroadcast, rigContainer, actorNumber, isLocal);
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x000E27A8 File Offset: 0x000E09A8
	private void BroadcastLoudSpeakerNetworkShared(bool toggleBroadcast, RigContainer rigContainer, int actorNumber, bool isLocal)
	{
		this.SetupLoudSpeakerNetwork(rigContainer);
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			if (toggleBroadcast)
			{
				loudSpeakerNetwork.BroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
			else
			{
				loudSpeakerNetwork.StopBroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
		}
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x000E2814 File Offset: 0x000E0A14
	[PunRPC]
	public void GrabbedByPlayer(bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand, PhotonMessageInfo info)
	{
		GorillaGuardianManager gorillaGuardianManager = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.IsPlayerGuardian(info.Sender))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.vrrig.GrabbedByPlayer(rigContainer.Rig, grabbedBody, grabbedLeftHand, grabbedWithLeftHand);
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x000E2870 File Offset: 0x000E0A70
	[PunRPC]
	public void DroppedByPlayer(Vector3 throwVelocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DroppedByPlayer");
		RigContainer rigContainer;
		if (this.vrrig.isOfflineVRRig && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			float num = 10000f;
			if (throwVelocity.IsValid(num))
			{
				this.vrrig.DroppedByPlayer(rigContainer.Rig, throwVelocity);
				return;
			}
		}
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x000E28CD File Offset: 0x000E0ACD
	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x000E28EC File Offset: 0x000E0AEC
	void IFXContextParems<GeoSoundArg>.OnPlayFX(GeoSoundArg parems)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(parems.position);
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000E2904 File Offset: 0x000E0B04
	private void OnHandTapRPCShared(int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnHandTapRPCShared");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		if (audioClipIndex < 0 || audioClipIndex >= GTPlayer.Instance.materialData.Count)
		{
			return;
		}
		HandLink handLink = isLeftHand ? this.vrrig.rightHandLink : this.vrrig.leftHandLink;
		NetPlayer grabbedPlayer = handLink.grabbedPlayer;
		if (grabbedPlayer != null && grabbedPlayer.IsLocal)
		{
			(handLink.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).PlayVicariousTapHaptic();
		}
		Vector3 tapDir = Utils.UnpackVector3FromLong(packedDirFromHitToHand);
		if (!Mathf.Approximately(tapDir.sqrMagnitude, 1f))
		{
			tapDir.Normalize();
		}
		float max = GorillaTagger.Instance.DefaultHandTapVolume;
		GorillaAmbushManager gorillaAmbushManager = GorillaGameModes.GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(this.rigContainer.Creator))
		{
			max = gorillaAmbushManager.crawlingSpeedForMaxVolume;
		}
		OnHandTapFX onHandTapFX = new OnHandTapFX
		{
			rig = this.vrrig,
			surfaceIndex = audioClipIndex,
			isDownTap = isDownTap,
			isLeftHand = isLeftHand,
			volume = handTapSpeed.ClampSafe(0f, max),
			speed = handTapSpeed,
			tapDir = tapDir
		};
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this.vrrig].IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.vrrig].rigActors[isLeftHand ? 0 : 2].actorSet;
			if (crittersLoudNoise.IsNotNull())
			{
				crittersLoudNoise.PlayHandTapRemote(info.SentServerTime, isLeftHand);
			}
		}
		FXSystem.PlayFXForRig<HandEffectContext>(FXType.OnHandTap, onHandTapFX, info);
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000E2ADC File Offset: 0x000E0CDC
	private void PlayHandTapShared(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		GorillaNot.IncrementRPCCall(info, "PlayHandTapShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && float.IsFinite(tapVolume))
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Clamp(tapVolume, 0f, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, info);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent hand tap", sender.UserId, sender.NickName);
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000E2B7C File Offset: 0x000E0D7C
	private void UpdateNativeSizeShared(float value, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		GorillaNot.IncrementRPCCall(info, "UpdateNativeSizeShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && RPCUtil.SafeValue(value, 0.1f, 10f) && RPCUtil.NotSpam("UpdateNativeSizeShared", info, 1f))
		{
			if (this.vrrig != null)
			{
				this.vrrig.NativeScale = value;
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent native size", sender.UserId, sender.NickName);
		}
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x000E2C0C File Offset: 0x000E0E0C
	private void PlayGeodeEffectShared(Vector3 hitPosition, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "PlayGeodeEffectShared");
		if (info.Sender == this.netView.Owner)
		{
			float num = 10000f;
			if (hitPosition.IsValid(num))
			{
				this.geoSoundArg.position = hitPosition;
				FXSystem.PlayFX<GeoSoundArg>(FXType.PlayHandTap, this, this.geoSoundArg, info);
				return;
			}
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x000E2C8D File Offset: 0x000E0E8D
	private void InitializeNoobMaterialShared(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, info);
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x000E2CA4 File Offset: 0x000E0EA4
	private void RequestMaterialColorShared(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayerID, info);
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x000E2CB8 File Offset: 0x000E0EB8
	private void RequestCosmeticsShared(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestCosmetics");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[9].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestCosmetics(info);
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x000E2D1B File Offset: 0x000E0F1B
	private void PlayDrumShared(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayDrum(drumIndex, drumVolume, info);
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x000E2D30 File Offset: 0x000E0F30
	private void PlaySelfOnlyInstrumentShared(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x000E2D47 File Offset: 0x000E0F47
	private void UpdateCosmeticsWithTryonShared(int[] currentItems, int[] tryOnItems, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithTryon(currentItems, tryOnItems, info);
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x000E2D5C File Offset: 0x000E0F5C
	private void PlaySplashEffectShared(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000E2D79 File Offset: 0x000E0F79
	private void EnableNonCosmeticHandItemShared(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.EnableNonCosmeticHandItemRPC(enable, isLeftHand, info);
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x000E2D8E File Offset: 0x000E0F8E
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateQuestScore(score, info);
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x000E2DA2 File Offset: 0x000E0FA2
	public void UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000E2DD7 File Offset: 0x000E0FD7
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.nickName = this._nickName;
		this.defaultName = this._defaultName;
		this.tutorialComplete = this._tutorialComplete;
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000E2E07 File Offset: 0x000E1007
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._nickName = this.nickName;
		this._defaultName = this.defaultName;
		this._tutorialComplete = this.tutorialComplete;
	}

	// Token: 0x040035F8 RID: 13816
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("nickName", 0, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _nickName;

	// Token: 0x040035F9 RID: 13817
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("defaultName", 17, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _defaultName;

	// Token: 0x040035FA RID: 13818
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("tutorialComplete", 34, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private bool _tutorialComplete;

	// Token: 0x040035FB RID: 13819
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x040035FC RID: 13820
	[SerializeField]
	private VoiceNetworkObject fusionVoiceView;

	// Token: 0x040035FD RID: 13821
	public Transform networkSpeaker;

	// Token: 0x040035FE RID: 13822
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x040035FF RID: 13823
	private RigContainer rigContainer;

	// Token: 0x04003600 RID: 13824
	private HandTapArgs handTapArgs = new HandTapArgs();

	// Token: 0x04003601 RID: 13825
	private GeoSoundArg geoSoundArg = new GeoSoundArg();
}
