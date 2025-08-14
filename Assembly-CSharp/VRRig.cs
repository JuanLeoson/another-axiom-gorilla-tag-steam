using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using TagEffects;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x0200040A RID: 1034
public class VRRig : MonoBehaviour, IWrappedSerializable, INetworkStruct, IPreDisable, IUserCosmeticsCallback, IEyeScannable
{
	// Token: 0x06001822 RID: 6178 RVA: 0x00080DCC File Offset: 0x0007EFCC
	private void CosmeticsV2_Awake()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
			return;
		}
		if (!this._isListeningFor_OnPostInstantiateAllPrefabs)
		{
			this._isListeningFor_OnPostInstantiateAllPrefabs = true;
			CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		}
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x00080E0B File Offset: 0x0007F00B
	private void CosmeticsV2_OnDestroy()
	{
		if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
			return;
		}
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x00080E3B File Offset: 0x0007F03B
	internal void Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		this.CheckForEarlyAccess();
		this.BuildInitialize_AfterCosmeticsV2Instantiated();
		this.SetCosmeticsActive();
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06001825 RID: 6181 RVA: 0x00080E6F File Offset: 0x0007F06F
	// (set) Token: 0x06001826 RID: 6182 RVA: 0x00080E7C File Offset: 0x0007F07C
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06001827 RID: 6183 RVA: 0x00080E8A File Offset: 0x0007F08A
	// (set) Token: 0x06001828 RID: 6184 RVA: 0x00080E92 File Offset: 0x0007F092
	public GameObject[] cosmetics
	{
		get
		{
			return this._cosmetics;
		}
		set
		{
			this._cosmetics = value;
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06001829 RID: 6185 RVA: 0x00080E9B File Offset: 0x0007F09B
	// (set) Token: 0x0600182A RID: 6186 RVA: 0x00080EA3 File Offset: 0x0007F0A3
	public GameObject[] overrideCosmetics
	{
		get
		{
			return this._overrideCosmetics;
		}
		set
		{
			this._overrideCosmetics = value;
		}
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x00080EAC File Offset: 0x0007F0AC
	internal void SetTaggedBy(VRRig taggingRig)
	{
		this.taggedById = taggingRig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x0600182C RID: 6188 RVA: 0x00080EBF File Offset: 0x0007F0BF
	// (set) Token: 0x0600182D RID: 6189 RVA: 0x00080EC7 File Offset: 0x0007F0C7
	internal bool InitializedCosmetics
	{
		get
		{
			return this.initializedCosmetics;
		}
		set
		{
			this.initializedCosmetics = value;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x0600182E RID: 6190 RVA: 0x00080ED0 File Offset: 0x0007F0D0
	// (set) Token: 0x0600182F RID: 6191 RVA: 0x00080ED8 File Offset: 0x0007F0D8
	public CosmeticRefRegistry cosmeticReferences { get; private set; }

	// Token: 0x06001830 RID: 6192 RVA: 0x00080EE1 File Offset: 0x0007F0E1
	public void BreakHandLinks()
	{
		this.leftHandLink.BreakLink();
		this.rightHandLink.BreakLink();
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x00080EF9 File Offset: 0x0007F0F9
	public bool IsInHandHoldChainWithOtherPlayer(int otherPlayer)
	{
		return HandLink.IsHandInChainWithOtherPlayer(this.leftHandLink, otherPlayer) || HandLink.IsHandInChainWithOtherPlayer(this.rightHandLink, otherPlayer);
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06001832 RID: 6194 RVA: 0x00080F17 File Offset: 0x0007F117
	// (set) Token: 0x06001833 RID: 6195 RVA: 0x00080F1F File Offset: 0x0007F11F
	public float LastTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06001834 RID: 6196 RVA: 0x00080F28 File Offset: 0x0007F128
	// (set) Token: 0x06001835 RID: 6197 RVA: 0x00080F30 File Offset: 0x0007F130
	public float LastHandTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001836 RID: 6198 RVA: 0x00080F39 File Offset: 0x0007F139
	public bool HasBracelet
	{
		get
		{
			return this.reliableState.HasBracelet;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001837 RID: 6199 RVA: 0x00080F46 File Offset: 0x0007F146
	public int CosmeticStepIndex
	{
		get
		{
			return this.newSwappedCosmetics.Count;
		}
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06001838 RID: 6200 RVA: 0x00080F53 File Offset: 0x0007F153
	// (set) Token: 0x06001839 RID: 6201 RVA: 0x00080F5B File Offset: 0x0007F15B
	public float LastCosmeticSwapTime { get; private set; } = float.PositiveInfinity;

	// Token: 0x0600183A RID: 6202 RVA: 0x00080F64 File Offset: 0x0007F164
	public void SetCosmeticSwapper(CosmeticSwapper swapper, float timeout)
	{
		this.cosmeticSwapper = swapper;
		this.cosmeticStepsDuration = timeout;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x00080F74 File Offset: 0x0007F174
	public void AddNewSwappedCosmetic(CosmeticSwapper.CosmeticState state)
	{
		this.newSwappedCosmetics.Push(state);
		this.LastCosmeticSwapTime = Time.time;
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x00080F8D File Offset: 0x0007F18D
	public void MarkFinalCosmeticStep()
	{
		this.isAtFinalCosmeticStep = true;
		this.LastCosmeticSwapTime = Time.time;
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x00080FA1 File Offset: 0x0007F1A1
	public void UnmarkFinalCosmeticStep()
	{
		this.isAtFinalCosmeticStep = false;
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x00080FAA File Offset: 0x0007F1AA
	public Vector3 GetMouthPosition()
	{
		return this.MouthPosition.position;
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x0600183F RID: 6207 RVA: 0x00080FB7 File Offset: 0x0007F1B7
	// (set) Token: 0x06001840 RID: 6208 RVA: 0x00080FBF File Offset: 0x0007F1BF
	public GorillaSkin CurrentCosmeticSkin { get; set; }

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06001841 RID: 6209 RVA: 0x00080FC8 File Offset: 0x0007F1C8
	// (set) Token: 0x06001842 RID: 6210 RVA: 0x00080FD0 File Offset: 0x0007F1D0
	public GorillaSkin CurrentModeSkin { get; set; }

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06001843 RID: 6211 RVA: 0x00080FD9 File Offset: 0x0007F1D9
	// (set) Token: 0x06001844 RID: 6212 RVA: 0x00080FE1 File Offset: 0x0007F1E1
	public GorillaSkin TemporaryEffectSkin { get; set; }

	// Token: 0x06001845 RID: 6213 RVA: 0x00080FEA File Offset: 0x0007F1EA
	public VRRig.PartyMemberStatus GetPartyMemberStatus()
	{
		if (this.partyMemberStatus == VRRig.PartyMemberStatus.NeedsUpdate)
		{
			this.partyMemberStatus = (FriendshipGroupDetection.Instance.IsInMyGroup(this.creator.UserId) ? VRRig.PartyMemberStatus.InLocalParty : VRRig.PartyMemberStatus.NotInLocalParty);
		}
		return this.partyMemberStatus;
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06001846 RID: 6214 RVA: 0x0008101B File Offset: 0x0007F21B
	public bool IsLocalPartyMember
	{
		get
		{
			return this.GetPartyMemberStatus() != VRRig.PartyMemberStatus.NotInLocalParty;
		}
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x00081029 File Offset: 0x0007F229
	public void ClearPartyMemberStatus()
	{
		this.partyMemberStatus = VRRig.PartyMemberStatus.NeedsUpdate;
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x00081032 File Offset: 0x0007F232
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x00081041 File Offset: 0x0007F241
	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x00081050 File Offset: 0x0007F250
	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x0008107B File Offset: 0x0007F27B
	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x0008108A File Offset: 0x0007F28A
	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x000810B5 File Offset: 0x0007F2B5
	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x000810C4 File Offset: 0x0007F2C4
	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000810EF File Offset: 0x0007F2EF
	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x0008111A File Offset: 0x0007F31A
	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x06001851 RID: 6225 RVA: 0x00081129 File Offset: 0x0007F329
	// (set) Token: 0x06001852 RID: 6226 RVA: 0x00081136 File Offset: 0x0007F336
	public int WearablePackedStates
	{
		get
		{
			return this.reliableState.wearablesPackedStates;
		}
		set
		{
			if (this.reliableState.wearablesPackedStates != value)
			{
				this.reliableState.wearablesPackedStates = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06001853 RID: 6227 RVA: 0x0008115D File Offset: 0x0007F35D
	// (set) Token: 0x06001854 RID: 6228 RVA: 0x0008116A File Offset: 0x0007F36A
	public int LeftThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.lThrowableProjectileIndex != value)
			{
				this.reliableState.lThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06001855 RID: 6229 RVA: 0x00081191 File Offset: 0x0007F391
	// (set) Token: 0x06001856 RID: 6230 RVA: 0x0008119E File Offset: 0x0007F39E
	public int RightThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.rThrowableProjectileIndex != value)
			{
				this.reliableState.rThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06001857 RID: 6231 RVA: 0x000811C5 File Offset: 0x0007F3C5
	// (set) Token: 0x06001858 RID: 6232 RVA: 0x000811D2 File Offset: 0x0007F3D2
	public Color32 LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.Equals(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06001859 RID: 6233 RVA: 0x00081209 File Offset: 0x0007F409
	// (set) Token: 0x0600185A RID: 6234 RVA: 0x00081216 File Offset: 0x0007F416
	public Color32 RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.Equals(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x0008124D File Offset: 0x0007F44D
	public Color32 GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x0008125F File Offset: 0x0007F45F
	public void SetThrowableProjectileColor(bool isLeftHand, Color32 color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x00081273 File Offset: 0x0007F473
	public void SetRandomThrowableModelIndex(int randModelIndex)
	{
		this.RandomThrowableIndex = randModelIndex;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x0008127C File Offset: 0x0007F47C
	public int GetRandomThrowableModelIndex()
	{
		return this.RandomThrowableIndex;
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x0600185F RID: 6239 RVA: 0x00081284 File Offset: 0x0007F484
	// (set) Token: 0x06001860 RID: 6240 RVA: 0x00081291 File Offset: 0x0007F491
	private int RandomThrowableIndex
	{
		get
		{
			return this.reliableState.randomThrowableIndex;
		}
		set
		{
			if (this.reliableState.randomThrowableIndex != value)
			{
				this.reliableState.randomThrowableIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06001861 RID: 6241 RVA: 0x000812B8 File Offset: 0x0007F4B8
	// (set) Token: 0x06001862 RID: 6242 RVA: 0x000812C5 File Offset: 0x0007F4C5
	public bool IsMicEnabled
	{
		get
		{
			return this.reliableState.isMicEnabled;
		}
		set
		{
			if (this.reliableState.isMicEnabled != value)
			{
				this.reliableState.isMicEnabled = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06001863 RID: 6243 RVA: 0x000812EC File Offset: 0x0007F4EC
	// (set) Token: 0x06001864 RID: 6244 RVA: 0x000812F9 File Offset: 0x0007F4F9
	public int SizeLayerMask
	{
		get
		{
			return this.reliableState.sizeLayerMask;
		}
		set
		{
			if (this.reliableState.sizeLayerMask != value)
			{
				this.reliableState.sizeLayerMask = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06001865 RID: 6245 RVA: 0x00081320 File Offset: 0x0007F520
	public float scaleFactor
	{
		get
		{
			return this.scaleMultiplier * this.nativeScale;
		}
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06001866 RID: 6246 RVA: 0x0008132F File Offset: 0x0007F52F
	// (set) Token: 0x06001867 RID: 6247 RVA: 0x00081337 File Offset: 0x0007F537
	public float ScaleMultiplier
	{
		get
		{
			return this.scaleMultiplier;
		}
		set
		{
			this.scaleMultiplier = value;
		}
	}

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06001868 RID: 6248 RVA: 0x00081340 File Offset: 0x0007F540
	// (set) Token: 0x06001869 RID: 6249 RVA: 0x00081348 File Offset: 0x0007F548
	public float NativeScale
	{
		get
		{
			return this.nativeScale;
		}
		set
		{
			this.nativeScale = value;
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x0600186A RID: 6250 RVA: 0x00081351 File Offset: 0x0007F551
	public NetPlayer Creator
	{
		get
		{
			return this.creator;
		}
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x0600186B RID: 6251 RVA: 0x00081359 File Offset: 0x0007F559
	internal bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x0600186C RID: 6252 RVA: 0x00081361 File Offset: 0x0007F561
	// (set) Token: 0x0600186D RID: 6253 RVA: 0x00081369 File Offset: 0x0007F569
	public float SpeakingLoudness
	{
		get
		{
			return this.speakingLoudness;
		}
		set
		{
			this.speakingLoudness = value;
		}
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x0600186E RID: 6254 RVA: 0x00081372 File Offset: 0x0007F572
	internal HandEffectContext LeftHandEffect
	{
		get
		{
			return this._leftHandEffect;
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x0600186F RID: 6255 RVA: 0x0008137A File Offset: 0x0007F57A
	internal HandEffectContext RightHandEffect
	{
		get
		{
			return this._rightHandEffect;
		}
	}

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06001870 RID: 6256 RVA: 0x00081382 File Offset: 0x0007F582
	public bool RigBuildFullyInitialized
	{
		get
		{
			return this._rigBuildFullyInitialized;
		}
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06001871 RID: 6257 RVA: 0x0008138A File Offset: 0x0007F58A
	public GamePlayer GamePlayerRef
	{
		get
		{
			if (this._gamePlayerRef == null)
			{
				this._gamePlayerRef = base.GetComponent<GamePlayer>();
			}
			return this._gamePlayerRef;
		}
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x000813AC File Offset: 0x0007F5AC
	public void BuildInitialize()
	{
		this.fxSettings = Object.Instantiate<FXSystemSettings>(this.sharedFXSettings);
		this.fxSettings.forLocalRig = this.isOfflineVRRig;
		this.lastPosition = base.transform.position;
		if (!this.isOfflineVRRig)
		{
			base.transform.parent = null;
		}
		SizeManager component = base.GetComponent<SizeManager>();
		if (component != null)
		{
			component.BuildInitialize();
		}
		this.myMouthFlap = base.GetComponent<GorillaMouthFlap>();
		this.mySpeakerLoudness = base.GetComponent<GorillaSpeakerLoudness>();
		if (this.myReplacementVoice == null)
		{
			this.myReplacementVoice = base.GetComponentInChildren<ReplacementVoice>();
		}
		this.myEyeExpressions = base.GetComponent<GorillaEyeExpressions>();
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x00081450 File Offset: 0x0007F650
	public void BuildInitialize_AfterCosmeticsV2Instantiated()
	{
		if (!this._rigBuildFullyInitialized)
		{
			Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
			foreach (GameObject gameObject in this.cosmetics)
			{
				GameObject gameObject2;
				if (!dictionary.TryGetValue(gameObject.name, out gameObject2))
				{
					dictionary.Add(gameObject.name, gameObject);
				}
			}
			foreach (GameObject gameObject3 in this.overrideCosmetics)
			{
				GameObject gameObject2;
				if (dictionary.TryGetValue(gameObject3.name, out gameObject2) && gameObject2.name == gameObject3.name)
				{
					gameObject2.name = "OVERRIDDEN";
				}
			}
			this.cosmetics = this.cosmetics.Concat(this.overrideCosmetics).ToArray<GameObject>();
		}
		this.cosmeticsObjectRegistry.Initialize(this.cosmetics);
		this._rigBuildFullyInitialized = true;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00081528 File Offset: 0x0007F728
	private void Awake()
	{
		this.CosmeticsV2_Awake();
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.UpdateName));
		if (this.isOfflineVRRig)
		{
			VRRig.gLocalRig = this;
			this.BuildInitialize();
		}
		this.SharedStart();
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x0008157D File Offset: 0x0007F77D
	private void EnsureInstantiatedMaterial()
	{
		if (this.myDefaultSkinMaterialInstance == null)
		{
			this.myDefaultSkinMaterialInstance = Object.Instantiate<Material>(this.materialsToChangeTo[0]);
			this.materialsToChangeTo[0] = this.myDefaultSkinMaterialInstance;
		}
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x000815B0 File Offset: 0x0007F7B0
	private void ApplyColorCode()
	{
		float defaultValue = 0f;
		float @float = PlayerPrefs.GetFloat("redValue", defaultValue);
		float float2 = PlayerPrefs.GetFloat("greenValue", defaultValue);
		float float3 = PlayerPrefs.GetFloat("blueValue", defaultValue);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x000815F4 File Offset: 0x0007F7F4
	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.lastScaleFactor = this.scaleFactor;
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		this.concatStringOfCosmeticsAllowed = "";
		this.EnsureInstantiatedMaterial();
		this.initialized = false;
		if (this.isOfflineVRRig)
		{
			if (CosmeticsController.hasInstance && CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			}
			if (Application.platform == RuntimePlatform.Android && this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.initialized = true;
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
		}
		GorillaSkin.ShowActiveSkin(this);
		base.Invoke("ApplyColorCode", 1f);
		List<Material> m = new List<Material>();
		this.mainSkin.GetSharedMaterials(m);
		this.layerChanger = base.GetComponent<LayerChanger>();
		if (this.layerChanger != null)
		{
			this.layerChanger.InitializeLayers(base.transform);
		}
		this.frozenEffectMinY = this.frozenEffect.transform.localScale.y;
		this.frozenEffectMinHorizontalScale = this.frozenEffect.transform.localScale.x;
		this.rightIndex.Initialize();
		this.rightMiddle.Initialize();
		this.rightThumb.Initialize();
		this.leftIndex.Initialize();
		this.leftMiddle.Initialize();
		this.leftThumb.Initialize();
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x000817BC File Offset: 0x0007F9BC
	private void Update()
	{
		float time = Time.time;
		if (this._nextUpdateTime < 0f)
		{
			this._nextUpdateTime = time + 1f;
			return;
		}
		if (time < this._nextUpdateTime)
		{
			return;
		}
		this._nextUpdateTime = time + 1f;
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.IsMasterClient && GorillaGameModes.GameMode.ActiveNetworkHandler.IsNull())
		{
			GorillaGameModes.GameMode.LoadGameModeFromProperty();
		}
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x00081828 File Offset: 0x0007FA28
	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot")
		{
			return NetworkSystem.Instance.InRoom && GorillaGameManager.instance is GorillaPaintbrawlManager;
		}
		if (BuilderSetManager.instance.GetStarterSetsConcat().Contains(itemName))
		{
			return true;
		}
		if (this.concatStringOfCosmeticsAllowed == null)
		{
			return false;
		}
		if (this.concatStringOfCosmeticsAllowed.Contains(itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		return this.inTryOnRoom && canTryOn;
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x000818AE File Offset: 0x0007FAAE
	public void ApplyLocalTrajectoryOverride(Vector3 overrideVelocity)
	{
		this.LocalTrajectoryOverrideBlend = 1f;
		this.LocalTrajectoryOverridePosition = base.transform.position;
		this.LocalTrajectoryOverrideVelocity = overrideVelocity;
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x000818D3 File Offset: 0x0007FAD3
	public bool IsLocalTrajectoryOverrideActive()
	{
		return this.LocalTrajectoryOverrideBlend > 0f;
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x000818E2 File Offset: 0x0007FAE2
	public void ApplyLocalGrabOverride(bool isBody, bool isLeftHand, Transform grabbingHand)
	{
		this.localOverrideIsBody = isBody;
		this.localOverrideIsLeftHand = isLeftHand;
		this.localOverrideGrabbingHand = grabbingHand;
		this.localGrabOverrideBlend = 1f;
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00081904 File Offset: 0x0007FB04
	public void ClearLocalGrabOverride()
	{
		this.localGrabOverrideBlend = -1f;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x00081914 File Offset: 0x0007FB14
	public void RemoteRigUpdate()
	{
		if (this.scaleFactor != this.lastScaleFactor)
		{
			this.ScaleUpdate();
		}
		if (this.voiceAudio != null)
		{
			float time = GorillaTagger.Instance.offlineVRRig.scaleFactor / this.scaleFactor;
			float num = this.voicePitchForRelativeScale.Evaluate(time);
			if (float.IsNaN(num) || num <= 0f)
			{
				Debug.LogError("Voice pitch curve is invalid, please fix!");
			}
			float num2 = this.UsingHauntedRing ? this.HauntedRingVoicePitch : num;
			num2 = (this.IsHaunted ? this.HauntedVoicePitch : num2);
			if (!Mathf.Approximately(this.voiceAudio.pitch, num2))
			{
				this.voiceAudio.pitch = num2;
			}
		}
		this.jobPos = base.transform.position;
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobPos = Vector3.Lerp(base.transform.position, this.SanitizeVector3(this.syncPos), this.lerpValueBody * 0.66f);
			if (this.currentRopeSwing && this.currentRopeSwingTarget)
			{
				Vector3 b;
				if (this.grabbedRopeIsLeft)
				{
					b = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
				}
				else
				{
					b = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
				}
				if (this.shouldLerpToRope)
				{
					this.jobPos += Vector3.Lerp(Vector3.zero, b, this.lastRopeGrabTimer * 4f);
					if (this.lastRopeGrabTimer < 1f)
					{
						this.lastRopeGrabTimer += Time.deltaTime;
					}
				}
				else
				{
					this.jobPos += b;
				}
			}
			else if (this.currentHoldParent)
			{
				Transform transform;
				if (this.grabbedRopeIsBody)
				{
					transform = this.bodyTransform;
				}
				else if (this.grabbedRopeIsLeft)
				{
					transform = this.leftHandTransform;
				}
				else
				{
					transform = this.rightHandTransform;
				}
				this.jobPos += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - transform.position;
			}
			else if (this.mountedMonkeBlock || this.mountedMovingSurface)
			{
				Transform transform2 = this.movingSurfaceIsMonkeBlock ? this.mountedMonkeBlock.transform : this.mountedMovingSurface.transform;
				Vector3 b2 = Vector3.zero;
				Vector3 b3 = this.jobPos - base.transform.position;
				Transform transform3;
				if (this.mountedMovingSurfaceIsBody)
				{
					transform3 = this.bodyTransform;
				}
				else if (this.mountedMovingSurfaceIsLeft)
				{
					transform3 = this.leftHandTransform;
				}
				else
				{
					transform3 = this.rightHandTransform;
				}
				b2 = transform2.TransformPoint(this.mountedMonkeBlockOffset) - (transform3.position + b3);
				if (this.shouldLerpToMovingSurface)
				{
					this.lastMountedSurfaceTimer += Time.deltaTime;
					this.jobPos += Vector3.Lerp(Vector3.zero, b2, this.lastMountedSurfaceTimer * 4f);
					if (this.lastMountedSurfaceTimer * 4f >= 1f)
					{
						this.shouldLerpToMovingSurface = false;
					}
				}
				else
				{
					this.jobPos += b2;
				}
			}
		}
		else
		{
			this.jobPos = this.SanitizeVector3(this.syncPos);
		}
		if (this.LocalTrajectoryOverrideBlend > 0f)
		{
			this.LocalTrajectoryOverrideBlend -= Time.deltaTime / this.LocalTrajectoryOverrideDuration;
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			Vector3 localTrajectoryOverrideVelocity;
			Vector3 localTrajectoryOverridePosition;
			if (this.LocalTestMovementCollision(this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideVelocity, out localTrajectoryOverrideVelocity, out localTrajectoryOverridePosition))
			{
				this.LocalTrajectoryOverrideVelocity = localTrajectoryOverrideVelocity;
				this.LocalTrajectoryOverridePosition = localTrajectoryOverridePosition;
			}
			else
			{
				this.LocalTrajectoryOverridePosition += this.LocalTrajectoryOverrideVelocity * Time.deltaTime;
			}
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			this.jobPos = Vector3.Lerp(this.jobPos, this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideBlend);
		}
		else if (this.localGrabOverrideBlend > 0f)
		{
			this.localGrabOverrideBlend -= Time.deltaTime / this.LocalGrabOverrideDuration;
			if (this.localOverrideGrabbingHand != null)
			{
				Transform transform4;
				if (this.localOverrideIsBody)
				{
					transform4 = this.bodyTransform;
				}
				else if (this.localOverrideIsLeftHand)
				{
					transform4 = this.leftHandTransform;
				}
				else
				{
					transform4 = this.rightHandTransform;
				}
				this.jobPos += this.localOverrideGrabbingHand.TransformPoint(this.grabbedRopeOffset) - transform4.position;
			}
		}
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobRotation = Quaternion.Lerp(base.transform.rotation, this.SanitizeQuaternion(this.syncRotation), this.lerpValueBody);
		}
		else
		{
			this.jobRotation = this.SanitizeQuaternion(this.syncRotation);
		}
		this.head.syncPos = base.transform.rotation * -this.headBodyOffset * this.scaleFactor;
		this.head.MapOther(this.lerpValueBody);
		this.rightHand.MapOther(this.lerpValueBody);
		this.leftHand.MapOther(this.lerpValueBody);
		this.rightIndex.MapOtherFinger((float)(this.handSync % 10) / 10f, this.lerpValueFingers);
		this.rightMiddle.MapOtherFinger((float)(this.handSync % 100) / 100f, this.lerpValueFingers);
		this.rightThumb.MapOtherFinger((float)(this.handSync % 1000) / 1000f, this.lerpValueFingers);
		this.leftIndex.MapOtherFinger((float)(this.handSync % 10000) / 10000f, this.lerpValueFingers);
		this.leftMiddle.MapOtherFinger((float)(this.handSync % 100000) / 100000f, this.lerpValueFingers);
		this.leftThumb.MapOtherFinger((float)(this.handSync % 1000000) / 1000000f, this.lerpValueFingers);
		this.leftHandHoldableStatus = this.handSync % 10000000 / 1000000;
		this.rightHandHoldableStatus = this.handSync % 100000000 / 10000000;
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00081FBC File Offset: 0x000801BC
	private void ScaleUpdate()
	{
		this.frameScale = Mathf.MoveTowards(this.lastScaleFactor, this.scaleFactor, Time.deltaTime * 4f);
		base.transform.localScale = Vector3.one * this.frameScale;
		this.lastScaleFactor = this.frameScale;
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x00082012 File Offset: 0x00080212
	public void AddLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Add(action);
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x00082021 File Offset: 0x00080221
	public void RemoveLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Remove(action);
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x00082030 File Offset: 0x00080230
	private void LateUpdate()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				this.speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				instance.jumpMultiplier = this.speedArray[1];
				instance.maxJumpSpeed = this.speedArray[0];
			}
			else
			{
				instance.jumpMultiplier = 1.1f;
				instance.maxJumpSpeed = 6.5f;
			}
			this.nativeScale = instance.NativeScale;
			this.scaleMultiplier = instance.ScaleMultiplier;
			if (this.scaleFactor != this.lastScaleFactor)
			{
				this.ScaleUpdate();
			}
			base.transform.eulerAngles = new Vector3(0f, this.mainCamera.transform.rotation.eulerAngles.y, 0f);
			this.syncPos = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.lastScaleFactor + base.transform.rotation * this.headBodyOffset * this.lastScaleFactor;
			base.transform.position = this.syncPos;
			this.head.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.leftHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightIndex.MapMyFinger(this.lerpValueFingers);
			this.rightMiddle.MapMyFinger(this.lerpValueFingers);
			this.rightThumb.MapMyFinger(this.lerpValueFingers);
			this.leftIndex.MapMyFinger(this.lerpValueFingers);
			this.leftMiddle.MapMyFinger(this.lerpValueFingers);
			this.leftThumb.MapMyFinger(this.lerpValueFingers);
			bool isGroundedHand = instance.IsGroundedHand;
			bool isGroundedButt = instance.IsGroundedButt;
			bool isLeftGrabbing = EquipmentInteractor.instance.isLeftGrabbing;
			bool canBeGrabbed = isLeftGrabbing && EquipmentInteractor.instance.CanGrabLeft();
			bool isRightGrabbing = EquipmentInteractor.instance.isRightGrabbing;
			bool canBeGrabbed2 = isRightGrabbing && EquipmentInteractor.instance.CanGrabRight();
			this.LastTouchedGroundAtNetworkTime = instance.LastTouchedGroundAtNetworkTime;
			this.LastHandTouchedGroundAtNetworkTime = instance.LastHandTouchedGroundAtNetworkTime;
			HandLink handLink = this.leftHandLink;
			if (handLink != null)
			{
				handLink.LocalUpdate(isGroundedHand, isGroundedButt, isLeftGrabbing, canBeGrabbed);
			}
			HandLink handLink2 = this.rightHandLink;
			if (handLink2 != null)
			{
				handLink2.LocalUpdate(isGroundedHand, isGroundedButt, isRightGrabbing, canBeGrabbed2);
			}
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				this.mainSkin.enabled = OVRManager.hasInputFocus;
			}
			this.bodyRenderer.ActiveBody.enabled = !instance.inOverlay;
			int i = this.loudnessCheckFrame - 1;
			this.loudnessCheckFrame = i;
			if (i < 0)
			{
				this.SpeakingLoudness = 0f;
				if (this.shouldSendSpeakingLoudness && this.netView)
				{
					PhotonVoiceView component = this.netView.GetComponent<PhotonVoiceView>();
					if (component && component.RecorderInUse)
					{
						MicWrapper micWrapper = component.RecorderInUse.InputSource as MicWrapper;
						if (micWrapper != null)
						{
							int num = this.replacementVoiceDetectionDelay;
							if (num > this.voiceSampleBuffer.Length)
							{
								Array.Resize<float>(ref this.voiceSampleBuffer, num);
							}
							float[] array = this.voiceSampleBuffer;
							if (micWrapper != null && micWrapper.Mic != null && micWrapper.Mic.samples >= num && micWrapper.Mic.GetData(array, micWrapper.Mic.samples - num))
							{
								float num2 = 0f;
								for (int j = 0; j < num; j++)
								{
									float num3 = Mathf.Sqrt(array[j]);
									if (num3 > num2)
									{
										num2 = num3;
									}
								}
								this.SpeakingLoudness = num2;
							}
						}
					}
				}
				this.loudnessCheckFrame = 10;
			}
			if (PhotonNetwork.InRoom && Time.time > this.nextLocalVelocityStoreTimestamp)
			{
				this.AddVelocityToQueue(base.transform.position, PhotonNetwork.Time);
				this.nextLocalVelocityStoreTimestamp = Time.time + 0.1f;
			}
		}
		if (this.leftHandLink.IsLinkActive())
		{
			VRRig myRig = this.leftHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig.inDuplicationZone && myRig.duplicationZone.IsApplyingDisplacement)
			{
				this.leftHandLink.BreakLink();
			}
			else
			{
				this.leftHandLink.SnapHandsTogether();
			}
		}
		if (this.rightHandLink.IsLinkActive())
		{
			VRRig myRig2 = this.rightHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig2.inDuplicationZone && myRig2.duplicationZone.IsApplyingDisplacement)
			{
				this.rightHandLink.BreakLink();
			}
			else
			{
				this.rightHandLink.SnapHandsTogether();
			}
		}
		if (this.creator != null)
		{
			if (GorillaGameManager.instance != null)
			{
				GorillaGameManager.instance.UpdatePlayerAppearance(this);
			}
			else if (this.setMatIndex != 0)
			{
				this.ChangeMaterialLocal(0);
				this.ForceResetFrozenEffect();
			}
		}
		if (this.inDuplicationZone)
		{
			this.renderTransform.position = base.transform.position + this.duplicationZone.VisualOffsetForRigs;
		}
		if (this.frozenEffect.activeSelf)
		{
			GorillaFreezeTagManager gorillaFreezeTagManager = GorillaGameManager.instance as GorillaFreezeTagManager;
			if (gorillaFreezeTagManager != null)
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				Vector3 vector = localScale;
				vector.y = Mathf.Lerp(this.frozenEffectMinY, this.frozenEffectMaxY, this.frozenTimeElapsed / gorillaFreezeTagManager.freezeDuration);
				localScale = new Vector3(localScale.x, vector.y, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
				this.frozenTimeElapsed += Time.deltaTime;
			}
		}
		if (this.TemporaryCosmeticEffects.Count > 0)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
			{
				if (Time.time - effect.Value.EffectStartedTime >= effect.Value.EffectDuration)
				{
					this.RemoveTemporaryCosmeticEffects(effect);
				}
			}
		}
		if (this.cosmeticSwapper != null && this.newSwappedCosmetics.Count > 0)
		{
			if (this.cosmeticSwapper.GetCurrentMode() == CosmeticSwapper.SwapMode.StepByStep)
			{
				if (this.isAtFinalCosmeticStep && this.cosmeticSwapper.ShouldHoldFinalStep())
				{
					if (Time.time - this.LastCosmeticSwapTime <= this.cosmeticStepsDuration)
					{
						return;
					}
					this.isAtFinalCosmeticStep = false;
				}
				if (Time.time - this.LastCosmeticSwapTime > this.cosmeticStepsDuration)
				{
					CosmeticSwapper.CosmeticState state = this.newSwappedCosmetics.Pop();
					this.cosmeticSwapper.RestorePreviousCosmetic(state, this);
					this.LastCosmeticSwapTime = Time.time;
					if (this.newSwappedCosmetics.Count == 0)
					{
						this.isAtFinalCosmeticStep = false;
					}
				}
			}
			else if (this.cosmeticSwapper.GetCurrentMode() == CosmeticSwapper.SwapMode.AllAtOnce && Time.time - this.LastCosmeticSwapTime > this.cosmeticStepsDuration)
			{
				while (this.newSwappedCosmetics.Count > 0)
				{
					CosmeticSwapper.CosmeticState state2 = this.newSwappedCosmetics.Pop();
					this.cosmeticSwapper.RestorePreviousCosmetic(state2, this);
				}
				this.LastCosmeticSwapTime = float.PositiveInfinity;
				this.isAtFinalCosmeticStep = false;
			}
		}
		this.lateUpdateCallbacks.TryRunCallbacks();
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x00082798 File Offset: 0x00080998
	private void RemoveTemporaryCosmeticEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin)
		{
			bool flag;
			if (effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.FPV)
		{
			if (this.FPVEffectsParent != null)
			{
				this.SpawnFPVEffects(effect, false);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
		{
			this.DisableHitWithKnockBack(effect);
		}
		this.TemporaryCosmeticEffects.Remove(effect.Key);
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x0008282D File Offset: 0x00080A2D
	public void SpawnSkinEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GorillaSkin.ApplyToRig(this, effect.Value.newSkin, GorillaSkin.SkinType.temporaryEffect);
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0008285C File Offset: 0x00080A5C
	public void SpawnFPVEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, bool enable)
	{
		if (this.FPVEffectsParent == null)
		{
			return;
		}
		if (enable)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.FPVEffect, this.FPVEffectsParent.transform.position, this.FPVEffectsParent.transform.rotation, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(this.FPVEffectsParent.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
			}
			this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
			return;
		}
		foreach (object obj in this.FPVEffectsParent.transform)
		{
			Transform transform = (Transform)obj;
			ObjectPools.instance.Destroy(transform.gameObject);
		}
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key))
		{
			this.TemporaryCosmeticEffects.Remove(effect.Key);
		}
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x0008298C File Offset: 0x00080B8C
	public void EnableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x000829A8 File Offset: 0x00080BA8
	private void DisableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key) && effect.Value.knockbackVFX)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.knockbackVFX, base.transform.position, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(base.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x00082A34 File Offset: 0x00080C34
	public void DisableHitWithKnockBack()
	{
		foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
		{
			bool flag;
			if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
			{
				this.DisableHitWithKnockBack(effect);
				this.TemporaryCosmeticEffects.Remove(effect.Key);
			}
			else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin && effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
				this.TemporaryCosmeticEffects.Remove(effect.Key);
			}
		}
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x0008298C File Offset: 0x00080B8C
	public void ApplyInstanceKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x0008298C File Offset: 0x00080B8C
	public void ActivateVOEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x00082AE2 File Offset: 0x00080CE2
	public bool TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE key, out CosmeticEffectsOnPlayers.CosmeticEffect value)
	{
		if (this.TemporaryCosmeticEffects == null)
		{
			value = null;
			return false;
		}
		return this.TemporaryCosmeticEffects.TryGetValue(key, out value);
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x00082B00 File Offset: 0x00080D00
	public void PlayCosmeticEffectSFX(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
		int index = UnityEngine.Random.Range(0, effect.Value.sfxAudioClip.Count);
		this.tagSound.PlayOneShot(effect.Value.sfxAudioClip[index]);
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x0600188D RID: 6285 RVA: 0x00082B5C File Offset: 0x00080D5C
	public bool IsPlayerMeshHidden
	{
		get
		{
			return !this.mainSkin.enabled;
		}
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00082B6C File Offset: 0x00080D6C
	public void SetPlayerMeshHidden(bool hide)
	{
		this.mainSkin.enabled = !hide;
		this.faceSkin.enabled = !hide;
		this.nameTagAnchor.SetActive(!hide);
		this.UpdateMatParticles(-1);
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x00082BA2 File Offset: 0x00080DA2
	public void SetInvisibleToLocalPlayer(bool invisible)
	{
		if (this.IsInvisibleToLocalPlayer == invisible)
		{
			return;
		}
		this.IsInvisibleToLocalPlayer = invisible;
		this.nameTagAnchor.SetActive(!invisible);
		this.UpdateFriendshipBracelet();
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x00082BCA File Offset: 0x00080DCA
	public void ChangeLayer(string layerName)
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.ChangeLayer(base.transform.parent, layerName);
		}
		GTPlayer.Instance.ChangeLayer(layerName);
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x00082BFC File Offset: 0x00080DFC
	public void RestoreLayer()
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.RestoreOriginalLayers();
		}
		GTPlayer.Instance.RestoreLayer();
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x000023F5 File Offset: 0x000005F5
	public void SetHeadBodyOffset()
	{
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x00082C21 File Offset: 0x00080E21
	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x00082C34 File Offset: 0x00080E34
	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x00082CFE File Offset: 0x00080EFE
	public void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		this.ClearRopeData();
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x00082D40 File Offset: 0x00080F40
	private InputStruct SerializeWriteShared()
	{
		InputStruct result = default(InputStruct);
		result.headRotation = BitPackUtils.PackQuaternionForNetwork(this.head.rigTarget.localRotation);
		result.rightHandLong = BitPackUtils.PackHandPosRotForNetwork(this.rightHand.rigTarget.localPosition, this.rightHand.rigTarget.localRotation);
		result.leftHandLong = BitPackUtils.PackHandPosRotForNetwork(this.leftHand.rigTarget.localPosition, this.leftHand.rigTarget.localRotation);
		result.position = BitPackUtils.PackWorldPosForNetwork(base.transform.position);
		result.handPosition = this.ReturnHandPosition();
		result.taggedById = (short)this.taggedById;
		int num = Mathf.Clamp(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y + 360f) % 360, 0, 360);
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(this.SpeakingLoudness) * 255f);
		bool flag = this.leftHandLink.IsLinkActive() || this.rightHandLink.IsLinkActive();
		GorillaGameManager activeGameMode = GorillaGameModes.GameMode.ActiveGameMode;
		bool flag2 = activeGameMode != null && activeGameMode.GameType() == GameModeType.PropHunt;
		int packedFields = num + (this.remoteUseReplacementVoice ? 512 : 0) + ((this.grabbedRopeIndex != -1) ? 1024 : 0) + (this.grabbedRopeIsPhotonView ? 2048 : 0) + (flag ? 4096 : 0) + (this.hoverboardVisual.IsHeld ? 8192 : 0) + (this.hoverboardVisual.IsLeftHanded ? 16384 : 0) + ((this.mountedMovingSurfaceId != -1) ? 32768 : 0) + (flag2 ? 65536 : 0) + (this.propHuntHandFollower.IsLeftHand ? 131072 : 0) + (this.leftHandLink.CanBeGrabbed() ? 262144 : 0) + (this.rightHandLink.CanBeGrabbed() ? 524288 : 0) + (num2 << 24);
		result.packedFields = packedFields;
		result.packedCompetitiveData = this.PackCompetitiveData();
		if (this.grabbedRopeIndex != -1)
		{
			result.grabbedRopeIndex = this.grabbedRopeIndex;
			result.ropeBoneIndex = this.grabbedRopeBoneIndex;
			result.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			result.ropeGrabIsBody = this.grabbedRopeIsBody;
			result.ropeGrabOffset = this.grabbedRopeOffset;
		}
		if (this.grabbedRopeIndex == -1 && this.mountedMovingSurfaceId != -1)
		{
			result.grabbedRopeIndex = this.mountedMovingSurfaceId;
			result.ropeGrabIsLeft = this.mountedMovingSurfaceIsLeft;
			result.ropeGrabIsBody = this.mountedMovingSurfaceIsBody;
			result.ropeGrabOffset = this.mountedMonkeBlockOffset;
		}
		if (this.hoverboardVisual.IsHeld)
		{
			result.hoverboardPosRot = BitPackUtils.PackHandPosRotForNetwork(this.hoverboardVisual.NominalLocalPosition, this.hoverboardVisual.NominalLocalRotation);
			result.hoverboardColor = BitPackUtils.PackColorForNetwork(this.hoverboardVisual.boardColor);
		}
		if (flag2)
		{
			result.propHuntPosRot = this.propHuntHandFollower.GetRelativePosRotLong();
		}
		if (flag)
		{
			this.leftHandLink.Write(out result.isGroundedHand, out result.isGroundedButt, out result.leftHandGrabbedActorNumber, out result.leftGrabbedHandIsLeft);
			this.rightHandLink.Write(out result.isGroundedHand, out result.isGroundedButt, out result.rightHandGrabbedActorNumber, out result.rightGrabbedHandIsLeft);
			result.lastTouchedGroundAtTime = this.LastTouchedGroundAtNetworkTime;
			result.lastHandTouchedGroundAtTime = this.LastHandTouchedGroundAtNetworkTime;
		}
		return result;
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x000830BC File Offset: 0x000812BC
	private void SerializeReadShared(InputStruct data)
	{
		VRMap vrmap = this.head;
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.headRotation);
		ref vrmap.syncRotation.SetValueSafe(quaternion);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.rightHandLong, out this.tempVec, out this.tempQuat);
		this.rightHand.syncPos = this.tempVec;
		ref this.rightHand.syncRotation.SetValueSafe(this.tempQuat);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.leftHandLong, out this.tempVec, out this.tempQuat);
		this.leftHand.syncPos = this.tempVec;
		ref this.leftHand.syncRotation.SetValueSafe(this.tempQuat);
		this.syncPos = BitPackUtils.UnpackWorldPosFromNetwork(data.position);
		this.handSync = data.handPosition;
		int packedFields = data.packedFields;
		int num = packedFields & 511;
		this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)num, 0f));
		this.remoteUseReplacementVoice = ((packedFields & 512) != 0);
		int num2 = packedFields >> 24 & 255;
		this.SpeakingLoudness = (float)num2 / 255f;
		this.UpdateReplacementVoice();
		this.UnpackCompetitiveData(data.packedCompetitiveData);
		this.taggedById = (int)data.taggedById;
		bool flag = (packedFields & 1024) != 0;
		this.grabbedRopeIsPhotonView = ((packedFields & 2048) != 0);
		if (flag)
		{
			this.grabbedRopeIndex = data.grabbedRopeIndex;
			this.grabbedRopeBoneIndex = data.ropeBoneIndex;
			this.grabbedRopeIsLeft = data.ropeGrabIsLeft;
			this.grabbedRopeIsBody = data.ropeGrabIsBody;
			ref this.grabbedRopeOffset.SetValueSafe(data.ropeGrabOffset);
		}
		else
		{
			this.grabbedRopeIndex = -1;
		}
		bool flag2 = (packedFields & 32768) != 0;
		if (!flag && flag2)
		{
			this.mountedMovingSurfaceId = data.grabbedRopeIndex;
			this.mountedMovingSurfaceIsLeft = data.ropeGrabIsLeft;
			this.mountedMovingSurfaceIsBody = data.ropeGrabIsBody;
			ref this.mountedMonkeBlockOffset.SetValueSafe(data.ropeGrabOffset);
			this.movingSurfaceIsMonkeBlock = data.movingSurfaceIsMonkeBlock;
		}
		else
		{
			this.mountedMovingSurfaceId = -1;
		}
		bool flag3 = (packedFields & 8192) != 0;
		bool isHeldLeftHanded = (packedFields & 16384) != 0;
		if (flag3)
		{
			Vector3 v;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.hoverboardPosRot, out v, out localRotation);
			Color boardColor = BitPackUtils.UnpackColorFromNetwork(data.hoverboardColor);
			if (localRotation.IsValid())
			{
				this.hoverboardVisual.SetIsHeld(isHeldLeftHanded, v.ClampMagnitudeSafe(1f), localRotation, boardColor);
			}
		}
		else if (this.hoverboardVisual.gameObject.activeSelf)
		{
			this.hoverboardVisual.SetNotHeld();
		}
		if ((packedFields & 65536) != 0)
		{
			bool isLeftHand = (packedFields & 131072) != 0;
			Vector3 propPos;
			Quaternion propRot;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.propHuntPosRot, out propPos, out propRot);
			this.propHuntHandFollower.SetProp(isLeftHand, propPos, propRot);
		}
		if (this.grabbedRopeIsPhotonView)
		{
			this.localGrabOverrideBlend = -1f;
		}
		Vector3 position = base.transform.position;
		this.leftHandLink.Read(this.leftHand.syncPos, this.syncRotation, position, data.isGroundedHand, data.isGroundedButt, (packedFields & 262144) != 0, data.leftHandGrabbedActorNumber, data.leftGrabbedHandIsLeft);
		this.rightHandLink.Read(this.rightHand.syncPos, this.syncRotation, position, data.isGroundedHand, data.isGroundedButt, (packedFields & 524288) != 0, data.rightHandGrabbedActorNumber, data.rightGrabbedHandIsLeft);
		this.LastTouchedGroundAtNetworkTime = data.lastTouchedGroundAtTime;
		this.LastHandTouchedGroundAtNetworkTime = data.lastHandTouchedGroundAtTime;
		this.UpdateRopeData();
		this.UpdateMovingMonkeBlockData();
		this.AddVelocityToQueue(this.syncPos, data.serverTimeStamp);
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x00083444 File Offset: 0x00081644
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		stream.SendNext(inputStruct.headRotation);
		stream.SendNext(inputStruct.rightHandLong);
		stream.SendNext(inputStruct.leftHandLong);
		stream.SendNext(inputStruct.position);
		stream.SendNext(inputStruct.handPosition);
		stream.SendNext(inputStruct.packedFields);
		stream.SendNext(inputStruct.packedCompetitiveData);
		if (this.grabbedRopeIndex != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeBoneIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
		}
		else if (this.mountedMovingSurfaceId != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
			stream.SendNext(inputStruct.movingSurfaceIsMonkeBlock);
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			stream.SendNext(inputStruct.hoverboardPosRot);
			stream.SendNext(inputStruct.hoverboardColor);
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			stream.SendNext(inputStruct.isGroundedHand);
			stream.SendNext(inputStruct.isGroundedButt);
			stream.SendNext(inputStruct.leftHandGrabbedActorNumber);
			stream.SendNext(inputStruct.leftGrabbedHandIsLeft);
			stream.SendNext(inputStruct.rightHandGrabbedActorNumber);
			stream.SendNext(inputStruct.rightGrabbedHandIsLeft);
			stream.SendNext(inputStruct.lastTouchedGroundAtTime);
			stream.SendNext(inputStruct.lastHandTouchedGroundAtTime);
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			stream.SendNext(inputStruct.propHuntPosRot);
		}
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x00083678 File Offset: 0x00081878
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!Utils.ValidateServerTime(info.SentServerTime, 60.0))
		{
			return;
		}
		InputStruct inputStruct = new InputStruct
		{
			headRotation = (int)stream.ReceiveNext(),
			rightHandLong = (long)stream.ReceiveNext(),
			leftHandLong = (long)stream.ReceiveNext(),
			position = (long)stream.ReceiveNext(),
			handPosition = (int)stream.ReceiveNext(),
			packedFields = (int)stream.ReceiveNext(),
			packedCompetitiveData = (short)stream.ReceiveNext()
		};
		bool flag = (inputStruct.packedFields & 1024) != 0;
		bool flag2 = (inputStruct.packedFields & 32768) != 0;
		if (flag)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeBoneIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		else if (flag2)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			inputStruct.hoverboardPosRot = (long)stream.ReceiveNext();
			inputStruct.hoverboardColor = (short)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			inputStruct.isGroundedHand = (bool)stream.ReceiveNext();
			inputStruct.isGroundedButt = (bool)stream.ReceiveNext();
			inputStruct.leftHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.leftGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.rightHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.rightGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.lastTouchedGroundAtTime = (float)stream.ReceiveNext();
			inputStruct.lastHandTouchedGroundAtTime = (float)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			inputStruct.propHuntPosRot = (long)stream.ReceiveNext();
		}
		inputStruct.serverTimeStamp = info.SentServerTime;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x000838F8 File Offset: 0x00081AF8
	public object OnSerializeWrite()
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		double serverTimeStamp = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = serverTimeStamp;
		return inputStruct;
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x00083934 File Offset: 0x00081B34
	public void OnSerializeRead(object objectData)
	{
		InputStruct data = (InputStruct)objectData;
		this.SerializeReadShared(data);
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x00083950 File Offset: 0x00081B50
	private void UpdateExtrapolationTarget()
	{
		float num = (float)(NetworkSystem.Instance.SimTime - this.remoteLatestTimestamp);
		num -= 0.15f;
		num = Mathf.Clamp(num, -0.5f, 0.5f);
		this.syncPos += this.remoteVelocity * num;
		this.remoteCorrectionNeeded = this.syncPos - base.transform.position;
		if (this.remoteCorrectionNeeded.magnitude > 1.5f && this.grabbedRopeIndex <= 0)
		{
			base.transform.position = this.syncPos;
			this.remoteCorrectionNeeded = Vector3.zero;
		}
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x000839FC File Offset: 0x00081BFC
	private void UpdateRopeData()
	{
		if (this.previousGrabbedRope == this.grabbedRopeIndex && this.previousGrabbedRopeBoneIndex == this.grabbedRopeBoneIndex && this.previousGrabbedRopeWasLeft == this.grabbedRopeIsLeft && this.previousGrabbedRopeWasBody == this.grabbedRopeIsBody)
		{
			return;
		}
		this.ClearRopeData();
		if (this.grabbedRopeIndex != -1)
		{
			GorillaRopeSwing gorillaRopeSwing;
			if (this.grabbedRopeIsPhotonView)
			{
				PhotonView photonView = PhotonView.Find(this.grabbedRopeIndex);
				GorillaClimbable gorillaClimbable;
				HandHoldXSceneRef handHoldXSceneRef;
				VRRigSerializer vrrigSerializer;
				if (photonView.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
				{
					this.currentHoldParent = photonView.transform;
				}
				else if (photonView.TryGetComponent<HandHoldXSceneRef>(out handHoldXSceneRef))
				{
					GameObject targetObject = handHoldXSceneRef.targetObject;
					this.currentHoldParent = ((targetObject != null) ? targetObject.transform : null);
				}
				else if (photonView && photonView.TryGetComponent<VRRigSerializer>(out vrrigSerializer))
				{
					this.currentHoldParent = ((this.grabbedRopeBoneIndex == 1) ? vrrigSerializer.VRRig.leftHandHoldsPlayer.transform : vrrigSerializer.VRRig.rightHandHoldsPlayer.transform);
				}
			}
			else if (RopeSwingManager.instance.TryGetRope(this.grabbedRopeIndex, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				if (this.currentRopeSwingTarget == null || this.currentRopeSwingTarget.gameObject == null)
				{
					this.currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (gorillaRopeSwing.AttachRemotePlayer(this.creator.ActorNumber, this.grabbedRopeBoneIndex, this.currentRopeSwingTarget, this.grabbedRopeOffset))
				{
					this.currentRopeSwing = gorillaRopeSwing;
				}
				this.lastRopeGrabTimer = 0f;
			}
		}
		else if (this.previousGrabbedRope != -1)
		{
			PhotonView photonView2 = PhotonView.Find(this.previousGrabbedRope);
			VRRigSerializer vrrigSerializer2;
			if (photonView2 && photonView2.TryGetComponent<VRRigSerializer>(out vrrigSerializer2) && vrrigSerializer2.VRRig == VRRig.LocalRig)
			{
				EquipmentInteractor.instance.ForceDropEquipment(this.bodyHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.leftHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.rightHolds);
			}
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
		this.previousGrabbedRopeWasBody = this.grabbedRopeIsBody;
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x00083C3C File Offset: 0x00081E3C
	private void UpdateMovingMonkeBlockData()
	{
		if (this.mountedMonkeBlockOffset.sqrMagnitude > 2f)
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.prevMovingSurfaceID == this.mountedMovingSurfaceId && this.movingSurfaceWasBody == this.mountedMovingSurfaceIsBody && this.movingSurfaceWasLeft == this.mountedMovingSurfaceIsLeft && this.movingSurfaceWasMonkeBlock == this.movingSurfaceIsMonkeBlock)
		{
			return;
		}
		if (this.mountedMovingSurfaceId == -1)
		{
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		else if (this.movingSurfaceIsMonkeBlock)
		{
			this.mountedMonkeBlock = null;
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.zoneEntity.currentZone, out builderTable))
			{
				this.mountedMonkeBlock = builderTable.GetPiece(this.mountedMovingSurfaceId);
			}
			if (this.mountedMonkeBlock == null)
			{
				this.mountedMovingSurfaceId = -1;
				this.mountedMovingSurfaceIsLeft = false;
				this.mountedMovingSurfaceIsBody = false;
				this.mountedMonkeBlock = null;
				this.mountedMovingSurface = null;
			}
		}
		else if (MovingSurfaceManager.instance == null || !MovingSurfaceManager.instance.TryGetMovingSurface(this.mountedMovingSurfaceId, out this.mountedMovingSurface))
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.mountedMovingSurfaceId != -1 && this.prevMovingSurfaceID == -1)
		{
			this.shouldLerpToMovingSurface = true;
			this.lastMountedSurfaceTimer = 0f;
		}
		this.prevMovingSurfaceID = this.mountedMovingSurfaceId;
		this.movingSurfaceWasLeft = this.mountedMovingSurfaceIsLeft;
		this.movingSurfaceWasBody = this.mountedMovingSurfaceIsBody;
		this.movingSurfaceWasMonkeBlock = this.movingSurfaceIsMonkeBlock;
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x00083DE8 File Offset: 0x00081FE8
	public static void AttachLocalPlayerToMovingSurface(int blockId, bool isLeft, bool isBody, Vector3 offset, bool isMonkeBlock)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = blockId;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsLeft = isLeft;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsBody = isBody;
			GorillaTagger.Instance.offlineVRRig.movingSurfaceIsMonkeBlock = isMonkeBlock;
			GorillaTagger.Instance.offlineVRRig.mountedMonkeBlockOffset = offset;
		}
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x00083E5E File Offset: 0x0008205E
	public static void DetachLocalPlayerFromMovingSurface()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = -1;
		}
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x00083E88 File Offset: 0x00082088
	public static void AttachLocalPlayerToPhotonView(PhotonView view, XRNode xrNode, Vector3 offset, Vector3 velocity)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = view.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = true;
		}
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x00083EF5 File Offset: 0x000820F5
	public static void DetachLocalPlayerFromPhotonView()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x00083F20 File Offset: 0x00082120
	private void ClearRopeData()
	{
		if (this.currentRopeSwing)
		{
			this.currentRopeSwing.DetachRemotePlayer(this.creator.ActorNumber);
		}
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		this.currentRopeSwing = null;
		this.currentHoldParent = null;
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x00083F77 File Offset: 0x00082177
	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x00083F90 File Offset: 0x00082190
	public void UpdateFrozenEffect(bool enable)
	{
		if (this.frozenEffect != null && ((!this.frozenEffect.activeSelf && enable) || (this.frozenEffect.activeSelf && !enable)))
		{
			this.frozenEffect.SetActive(enable);
			if (enable)
			{
				this.frozenTimeElapsed = 0f;
			}
			else
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				localScale = new Vector3(localScale.x, this.frozenEffectMinY, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
			}
		}
		if (this.iceCubeLeft != null && ((!this.iceCubeLeft.activeSelf && enable) || (this.iceCubeLeft.activeSelf && !enable)))
		{
			this.iceCubeLeft.SetActive(enable);
		}
		if (this.iceCubeRight != null && ((!this.iceCubeRight.activeSelf && enable) || (this.iceCubeRight.activeSelf && !enable)))
		{
			this.iceCubeRight.SetActive(enable);
		}
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x0008409C File Offset: 0x0008229C
	public void ForceResetFrozenEffect()
	{
		this.frozenEffect.SetActive(false);
		this.iceCubeRight.SetActive(false);
		this.iceCubeLeft.SetActive(false);
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x000840C4 File Offset: 0x000822C4
	public void ChangeMaterialLocal(int materialIndex)
	{
		if (this.setMatIndex == materialIndex)
		{
			return;
		}
		this.setMatIndex = materialIndex;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.bodyRenderer.SetMaterialIndex(materialIndex);
		}
		this.UpdateMatParticles(materialIndex);
		if (materialIndex > 0 && VRRig.LocalRig != this)
		{
			this.PlayTaggedEffect();
		}
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x00084128 File Offset: 0x00082328
	public void PlayTaggedEffect()
	{
		TagEffectPack tagEffectPack = null;
		quaternion q = base.transform.rotation;
		TagEffectsLibrary.EffectType effectType = (VRRig.LocalRig == this) ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON;
		if (GorillaGameManager.instance != null && this.OwningNetPlayer == null)
		{
			GorillaGameManager.instance.lastTaggedActorNr.TryGetValue(this.OwningNetPlayer.ActorNumber, out this.taggedById);
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.taggedById);
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			tagEffectPack = rigContainer.Rig.CosmeticEffectPack;
			if (tagEffectPack && tagEffectPack.shouldFaceTagger && effectType == TagEffectsLibrary.EffectType.THIRD_PERSON)
			{
				q = Quaternion.LookRotation((rigContainer.Rig.transform.position - base.transform.position).normalized);
			}
		}
		TagEffectsLibrary.PlayEffect(base.transform, false, this.scaleFactor, effectType, this.CosmeticEffectPack, tagEffectPack, q);
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x0008422C File Offset: 0x0008242C
	public void UpdateMatParticles(int materialIndex)
	{
		if (this.lavaParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 2 && this.lavaParticleSystem.isStopped)
			{
				this.lavaParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.lavaParticleSystem.isPlaying)
			{
				this.lavaParticleSystem.Stop();
			}
		}
		if (this.rockParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 1 && this.rockParticleSystem.isStopped)
			{
				this.rockParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.rockParticleSystem.isPlaying)
			{
				this.rockParticleSystem.Stop();
			}
		}
		if (this.iceParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 3 && this.rockParticleSystem.isStopped)
			{
				this.iceParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.iceParticleSystem.isPlaying)
			{
				this.iceParticleSystem.Stop();
			}
		}
		if (this.snowFlakeParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 14 && this.snowFlakeParticleSystem.isStopped)
			{
				this.snowFlakeParticleSystem.Play();
				return;
			}
			if (!this.isOfflineVRRig && this.snowFlakeParticleSystem.isPlaying)
			{
				this.snowFlakeParticleSystem.Stop();
			}
		}
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x0008438C File Offset: 0x0008258C
	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_InitializeNoobMaterial");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID)) || (this.initialized && CosmeticWardrobeProximityDetector.IsUserNearWardrobe(userID))))
		{
			this.initialized = true;
			blue = blue.ClampSafe(0f, 1f);
			red = red.ClampSafe(0f, 1f);
			green = green.ClampSafe(0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue);
		}
	}

	// Token: 0x060018AB RID: 6315 RVA: 0x00084468 File Offset: 0x00082668
	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color = new Color(red, green, blue);
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			color.r = Mathf.Clamp(color.r, 0f, 1f);
			color.g = Mathf.Clamp(color.g, 0f, 1f);
			color.b = Mathf.Clamp(color.b, 0f, 1f);
			this.skeleton.UpdateColor(color);
			this.myDefaultSkinMaterialInstance.color = color;
		}
		this.SetColor(color);
		bool isNamePermissionEnabled = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		this.UpdateName(isNamePermissionEnabled);
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x00084514 File Offset: 0x00082714
	public void UpdateName(bool isNamePermissionEnabled)
	{
		if (!this.isOfflineVRRig && this.creator != null)
		{
			string text = (isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? this.creator.NickName : this.creator.DefaultName;
			this.playerNameVisible = this.NormalizeName(true, text);
		}
		else if (this.showName && NetworkSystem.Instance != null)
		{
			this.playerNameVisible = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? NetworkSystem.Instance.GetMyNickName() : NetworkSystem.Instance.GetMyDefaultName());
		}
		this.SetNameTagText(this.playerNameVisible);
		if (this.creator != null)
		{
			this.creator.SanitizedNickName = this.playerNameVisible;
		}
		Action onPlayerNameVisibleChanged = this.OnPlayerNameVisibleChanged;
		if (onPlayerNameVisibleChanged == null)
		{
			return;
		}
		onPlayerNameVisibleChanged();
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x000845E2 File Offset: 0x000827E2
	public void SetNameTagText(string name)
	{
		this.playerNameVisible = name;
		this.playerText1.text = name;
		this.playerText2.text = name;
		Action<RigContainer> onNameChanged = this.OnNameChanged;
		if (onNameChanged == null)
		{
			return;
		}
		onNameChanged(this.rigContainer);
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x0008461C File Offset: 0x0008281C
	public void UpdateName()
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
		bool isNamePermissionEnabled = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
		this.UpdateName(isNamePermissionEnabled);
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x00084658 File Offset: 0x00082858
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x000846E5 File Offset: 0x000828E5
	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GTPlayer.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x000846F2 File Offset: 0x000828F2
	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GTPlayer.Instance.jumpMultiplier = jumpMultiplier;
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x00084700 File Offset: 0x00082900
	public void RequestMaterialColor(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RequestMaterialColor");
		Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).PlayerRef;
		if (this.netView.IsMine)
		{
			this.netView.GetView.RPC("RPC_InitializeNoobMaterial", playerRef, new object[]
			{
				this.myDefaultSkinMaterialInstance.color.r,
				this.myDefaultSkinMaterialInstance.color.g,
				this.myDefaultSkinMaterialInstance.color.b
			});
		}
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x000847A8 File Offset: 0x000829A8
	public void RequestCosmetics(PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (this.netView.IsMine && CosmeticsController.hasInstance)
		{
			if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
			{
				this.netView.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
				return;
			}
			int[] array = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
			int[] array2 = CosmeticsController.instance.tryOnSet.ToPackedIDArray();
			this.netView.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", player, new object[]
			{
				array,
				array2
			});
		}
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00084848 File Offset: 0x00082A48
	public void PlayTagSoundLocal(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		if (soundIndex < 0 || soundIndex >= this.clipToPlay.Length)
		{
			return;
		}
		this.tagSound.volume = Mathf.Min(0.25f, soundVolume);
		if (stopCurrentAudio)
		{
			this.tagSound.Stop();
		}
		this.tagSound.GTPlayOneShot(this.clipToPlay[soundIndex], 1f);
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x000848A1 File Offset: 0x00082AA1
	public void AssignDrumToMusicDrums(int drumIndex, AudioSource drum)
	{
		if (drumIndex >= 0 && drumIndex < this.musicDrums.Length && drum != null)
		{
			this.musicDrums[drumIndex] = drum;
		}
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x000848C4 File Offset: 0x00082AC4
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlayDrum");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.senderRig = rigContainer.Rig;
		}
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f || !float.IsFinite(drumVolume))
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent drum", player.UserId, player.NickName);
			return;
		}
		AudioSource audioSource = this.netView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex];
		if (!audioSource.gameObject.activeSelf)
		{
			return;
		}
		float instrumentVolume = GorillaComputer.instance.instrumentVolume;
		audioSource.time = 0f;
		audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
		audioSource.GTPlay();
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x000849F8 File Offset: 0x00082BF8
	public int AssignInstrumentToInstrumentSelfOnly(TransferrableObject instrument)
	{
		if (instrument == null)
		{
			return -1;
		}
		if (!this.instrumentSelfOnly.Contains(instrument))
		{
			this.instrumentSelfOnly.Add(instrument);
		}
		return this.instrumentSelfOnly.IndexOf(instrument);
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x00084A2C File Offset: 0x00082C2C
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySelfOnlyInstrument");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Count && float.IsFinite(instrumentVol))
			{
				if (this.instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
				{
					this.instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
					return;
				}
			}
			else
			{
				GorillaNot.instance.SendReport("inappropriate tag data being sent self only instrument", player.UserId, player.NickName);
			}
		}
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x00084B08 File Offset: 0x00082D08
	public void PlayHandTapLocal(int audioClipIndex, bool isLeftHand, float tapVolume)
	{
		if (audioClipIndex > -1 && audioClipIndex < GTPlayer.Instance.materialData.Count)
		{
			GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[audioClipIndex];
			AudioSource audioSource = isLeftHand ? this.leftHandPlayer : this.rightHandPlayer;
			audioSource.volume = tapVolume;
			AudioClip clip = materialData.overrideAudio ? materialData.audio : GTPlayer.Instance.materialData[0].audio;
			audioSource.GTPlayOneShot(clip, 1f);
		}
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x00084B85 File Offset: 0x00082D85
	internal HandEffectContext GetHandEffect(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightHandEffect;
		}
		return this.LeftHandEffect;
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x00084B98 File Offset: 0x00082D98
	internal void SetHandEffectData(HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
	{
		VRMap vrmap = isLeftHand ? this.leftHand : this.rightHand;
		Vector3 b = dirFromHitToHand * this.tapPointDistance * this.scaleFactor;
		if (this.isOfflineVRRig)
		{
			Vector3 b2 = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * this.scaleFactor;
			effectContext.position = vrmap.rigTarget.position - b2 + b;
		}
		else
		{
			Quaternion rotation = vrmap.rigTarget.parent.rotation * vrmap.syncRotation;
			Vector3 b3 = this.netSyncPos.GetPredictedFuture() - base.transform.position;
			Vector3 b2 = rotation * vrmap.trackingPositionOffset * this.scaleFactor;
			effectContext.position = vrmap.rigTarget.parent.TransformPoint(vrmap.netSyncPos.GetPredictedFuture()) - b2 + b + b3;
		}
		GTPlayer.MaterialData handSurfaceData = this.GetHandSurfaceData(audioClipIndex);
		HandTapOverrides handTapOverrides = isDownTap ? effectContext.DownTapOverrides : effectContext.UpTapOverrides;
		int[] prefabHashes = effectContext.prefabHashes;
		int num = 0;
		HashWrapper hashWrapper = handTapOverrides.overrideSurfacePrefab ? handTapOverrides.surfaceTapPrefab : GTPlayer.Instance.materialDatasSO.surfaceEffects[handSurfaceData.surfaceEffectIndex];
		prefabHashes[num] = hashWrapper;
		effectContext.prefabHashes[1] = (ref handTapOverrides.overrideGamemodePrefab ? handTapOverrides.gamemodeTapPrefab : ((RoomSystem.JoinedRoom && GorillaGameModes.GameMode.ActiveGameMode.IsNotNull()) ? GorillaGameModes.GameMode.ActiveGameMode.SpecialHandFX(this.creator, this.rigContainer) : -1));
		effectContext.soundFX = (handTapOverrides.overrideSound ? handTapOverrides.tapSound : handSurfaceData.audio);
		effectContext.isDownTap = isDownTap;
		effectContext.soundVolume = handTapVolume * this.handSpeedToVolumeModifier;
		effectContext.soundPitch = 1f;
		effectContext.speed = handTapSpeed;
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x00084D8C File Offset: 0x00082F8C
	internal GTPlayer.MaterialData GetHandSurfaceData(int index)
	{
		GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[index];
		if (!materialData.overrideAudio)
		{
			materialData = GTPlayer.Instance.materialData[0];
		}
		return materialData;
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x00084DC4 File Offset: 0x00082FC4
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySplashEffect");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner)
		{
			float num = 10000f;
			if (splashPosition.IsValid(num) && splashRotation.IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
			{
				if ((base.transform.position - splashPosition).sqrMagnitude >= 9f)
				{
					return;
				}
				float time = Time.time;
				int num2 = -1;
				float num3 = time + 10f;
				for (int i = 0; i < this.splashEffectTimes.Length; i++)
				{
					if (this.splashEffectTimes[i] < num3)
					{
						num3 = this.splashEffectTimes[i];
						num2 = i;
					}
				}
				if (time - 0.5f > num3)
				{
					this.splashEffectTimes[num2] = time;
					boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
					splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);
					return;
				}
				return;
			}
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent splash effect", player.UserId, player.NickName);
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x00084F60 File Offset: 0x00083160
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_EnableNonCosmeticHandItem(bool enable, bool isLeftHand, RpcInfo info = default(RpcInfo))
	{
		PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
		this.IncrementRPC(info2, "EnableNonCosmeticHandItem");
		if (info2.Sender == this.creator)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(info2.Sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info2.Sender.UserId, info2.Sender.NickName);
		}
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x00085020 File Offset: 0x00083220
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		NetPlayer sender = info.Sender;
		this.IncrementRPC(info, "EnableNonCosmeticHandItem");
		if (sender == this.netView.Owner)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x000850D8 File Offset: 0x000832D8
	public bool IsMakingFistLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) > 0.25f;
		}
		return this.leftIndex.calcT > 0.25f && this.leftMiddle.calcT > 0.25f;
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x00085130 File Offset: 0x00083330
	public bool IsMakingFistRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.25f;
		}
		return this.rightIndex.calcT > 0.25f && this.rightMiddle.calcT > 0.25f;
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x00085188 File Offset: 0x00083388
	public bool IsMakingFiveLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) < 0.25f;
		}
		return this.leftIndex.calcT < 0.25f && this.leftMiddle.calcT < 0.25f;
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x000851E0 File Offset: 0x000833E0
	public bool IsMakingFiveRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) < 0.25f;
		}
		return this.rightIndex.calcT < 0.25f && this.rightMiddle.calcT < 0.25f;
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x00085238 File Offset: 0x00083438
	public VRMap GetMakingFist(bool debug, out bool isLeftHand)
	{
		if (this.IsMakingFistRight())
		{
			isLeftHand = false;
			return this.rightHand;
		}
		if (this.IsMakingFistLeft())
		{
			isLeftHand = true;
			return this.leftHand;
		}
		isLeftHand = false;
		return null;
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x00085264 File Offset: 0x00083464
	public void PlayGeodeEffect(Vector3 hitPosition)
	{
		if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
		{
			this.geodeCrackingSound.GTPlay();
		}
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x000852AC File Offset: 0x000834AC
	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.leftHandPlayer.volume = 0.1f;
			this.leftHandPlayer.clip = clip;
			this.leftHandPlayer.GTPlayOneShot(this.leftHandPlayer.clip, 1f);
			return;
		}
		this.rightHandPlayer.volume = 0.1f;
		this.rightHandPlayer.clip = clip;
		this.rightHandPlayer.GTPlayOneShot(this.rightHandPlayer.clip, 1f);
	}

	// Token: 0x060018C7 RID: 6343 RVA: 0x0008532C File Offset: 0x0008352C
	public void HideAllCosmetics(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "HideAllCosmetics");
		if (NetworkSystem.Instance.GetPlayer(info.Sender) == this.netView.Owner)
		{
			this.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x0008539C File Offset: 0x0008359C
	public void UpdateCosmetics(string[] currentItems, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmetics");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length <= 16)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			this.LocalUpdateCosmetics(newSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics", player.UserId, player.NickName);
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x00085414 File Offset: 0x00083614
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length == 16 && tryOnItems.Length == 16)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x000854A4 File Offset: 0x000836A4
	public void UpdateCosmeticsWithTryon(int[] currentItemsPacked, int[] tryOnItemsPacked, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && CosmeticsController.instance.ValidatePackedItems(currentItemsPacked) && CosmeticsController.instance.ValidatePackedItems(tryOnItemsPacked))
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItemsPacked, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItemsPacked, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x00085542 File Offset: 0x00083742
	public void LocalUpdateCosmetics(CosmeticsController.CosmeticSet newSet)
	{
		this.cosmeticSet = newSet;
		if (this.InitializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x00085559 File Offset: 0x00083759
	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive();
		}
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x00085577 File Offset: 0x00083777
	private void CheckForEarlyAccess()
	{
		if (this.concatStringOfCosmeticsAllowed.Contains("Early Access Supporter Pack"))
		{
			this.concatStringOfCosmeticsAllowed += "LBAAE.LFAAM.LFAAN.LHAAA.LHAAK.LHAAL.LHAAM.LHAAN.LHAAO.LHAAP.LHABA.LHABB.";
		}
		this.InitializedCosmetics = true;
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x000855A8 File Offset: 0x000837A8
	public void SetCosmeticsActive()
	{
		if (CosmeticsController.instance == null || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		this.prevSet.CopyItems(this.mergedSet);
		this.mergedSet.MergeSets(this.inTryOnRoom ? this.tryOnSet : null, this.cosmeticSet);
		BodyDockPositions component = base.GetComponent<BodyDockPositions>();
		this.mergedSet.ActivateCosmetics(this.prevSet, this, component, this.cosmeticsObjectRegistry);
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x00085620 File Offset: 0x00083820
	public void GetCosmeticsPlayFabCatalogData()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (ItemInstance itemInstance in result.Inventory)
				{
					if (!dictionary.ContainsKey(itemInstance.ItemId))
					{
						dictionary[itemInstance.ItemId] = itemInstance.ItemId;
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
						{
							this.concatStringOfCosmeticsAllowed += itemInstance.ItemId;
						}
					}
				}
				if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics();
				}
			}, delegate(PlayFabError error)
			{
				this.initializedCosmetics = true;
				if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
				{
					this.SetCosmeticsActive();
				}
			}, null, null);
		}
		this.concatStringOfCosmeticsAllowed += "Slingshot";
		this.concatStringOfCosmeticsAllowed += BuilderSetManager.instance.GetStarterSetsConcat();
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x00085694 File Offset: 0x00083894
	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000856EC File Offset: 0x000838EC
	private void GenerateTableThumb(ref VRMapThumb thumb)
	{
		thumb.angle1Table = new Quaternion[11];
		thumb.angle2Table = new Quaternion[11];
		for (int i = 0; i < thumb.angle1Table.Length; i++)
		{
			thumb.angle1Table[i] = Quaternion.Lerp(thumb.startingAngle1Quat, thumb.closedAngle1Quat, (float)i / 10f);
			thumb.angle2Table[i] = Quaternion.Lerp(thumb.startingAngle2Quat, thumb.closedAngle2Quat, (float)i / 10f);
		}
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x0008577C File Offset: 0x0008397C
	private void GenerateTableIndex(ref VRMapIndex index)
	{
		index.angle1Table = new Quaternion[11];
		index.angle2Table = new Quaternion[11];
		index.angle3Table = new Quaternion[11];
		for (int i = 0; i < index.angle1Table.Length; i++)
		{
			index.angle1Table[i] = Quaternion.Lerp(index.startingAngle1Quat, index.closedAngle1Quat, (float)i / 10f);
			index.angle2Table[i] = Quaternion.Lerp(index.startingAngle2Quat, index.closedAngle2Quat, (float)i / 10f);
			index.angle3Table[i] = Quaternion.Lerp(index.startingAngle3Quat, index.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x00085844 File Offset: 0x00083A44
	private void GenerateTableMiddle(ref VRMapMiddle middle)
	{
		middle.angle1Table = new Quaternion[11];
		middle.angle2Table = new Quaternion[11];
		middle.angle3Table = new Quaternion[11];
		for (int i = 0; i < middle.angle1Table.Length; i++)
		{
			middle.angle1Table[i] = Quaternion.Lerp(middle.startingAngle1Quat, middle.closedAngle1Quat, (float)i / 10f);
			middle.angle2Table[i] = Quaternion.Lerp(middle.startingAngle2Quat, middle.closedAngle2Quat, (float)i / 10f);
			middle.angle3Table[i] = Quaternion.Lerp(middle.startingAngle3Quat, middle.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x0008590C File Offset: 0x00083B0C
	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x00085988 File Offset: 0x00083B88
	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 5000f);
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x000859F4 File Offset: 0x00083BF4
	private void IncrementRPC(PhotonMessageInfoWrapped info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x00085A0A File Offset: 0x00083C0A
	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			GorillaNot.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x00085A20 File Offset: 0x00083C20
	private void AddVelocityToQueue(Vector3 position, double serverTime)
	{
		Vector3 velocity = Vector3.zero;
		if (this.velocityHistoryList.Count > 0)
		{
			double num = Utils.CalculateNetworkDeltaTime(this.velocityHistoryList[0].time, serverTime);
			if (num == 0.0)
			{
				return;
			}
			velocity = (position - this.lastPosition) / (float)num;
		}
		this.velocityHistoryList.Add(new VRRig.VelocityTime(velocity, serverTime));
		this.lastPosition = position;
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x00085A94 File Offset: 0x00083C94
	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (this.velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = this.velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return this.velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (this.velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(this.velocityHistoryList[num].time - timeToReturn);
		double num6 = this.velocityHistoryList[num].time - this.velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(this.velocityHistoryList[num].vel, this.velocityHistoryList[num2].vel, num5);
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x00085BA6 File Offset: 0x00083DA6
	public Vector3 LatestVelocity()
	{
		if (this.velocityHistoryList.Count > 0)
		{
			return this.velocityHistoryList[0].vel;
		}
		return Vector3.zero;
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x00085BCD File Offset: 0x00083DCD
	public bool IsPositionInRange(Vector3 position, float range)
	{
		return (this.syncPos - position).IsShorterThan(range * this.scaleFactor);
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x00085BE8 File Offset: 0x00083DE8
	public bool CheckTagDistanceRollback(VRRig otherRig, float max, float timeInterval)
	{
		Vector3 a;
		Vector3 b;
		GorillaMath.LineSegClosestPoints(this.syncPos, -this.LatestVelocity() * timeInterval, otherRig.syncPos, -otherRig.LatestVelocity() * timeInterval, out a, out b);
		return Vector3.SqrMagnitude(a - b) < max * max * this.scaleFactor;
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x00085C44 File Offset: 0x00083E44
	public Vector3 ClampVelocityRelativeToPlayerSafe(Vector3 inVel, float max)
	{
		max *= this.scaleFactor;
		Vector3 vector = Vector3.zero;
		ref vector.SetValueSafe(inVel);
		Vector3 vector2 = (this.velocityHistoryList.Count > 0) ? this.velocityHistoryList[0].vel : Vector3.zero;
		Vector3 vector3 = vector - vector2;
		vector3 = Vector3.ClampMagnitude(vector3, max);
		vector = vector2 + vector3;
		return vector;
	}

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x060018DE RID: 6366 RVA: 0x00085CAC File Offset: 0x00083EAC
	// (remove) Token: 0x060018DF RID: 6367 RVA: 0x00085CE4 File Offset: 0x00083EE4
	public event Action<Color> OnColorChanged;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x060018E0 RID: 6368 RVA: 0x00085D1C File Offset: 0x00083F1C
	// (remove) Token: 0x060018E1 RID: 6369 RVA: 0x00085D54 File Offset: 0x00083F54
	public event Action OnPlayerNameVisibleChanged;

	// Token: 0x060018E2 RID: 6370 RVA: 0x00085D8C File Offset: 0x00083F8C
	public void SetColor(Color color)
	{
		this.skeleton.UpdateColor(color);
		Action<Color> onColorChanged = this.OnColorChanged;
		if (onColorChanged != null)
		{
			onColorChanged(color);
		}
		Action<Color> action = this.onColorInitialized;
		if (action != null)
		{
			action(color);
		}
		this.onColorInitialized = delegate(Color color1)
		{
		};
		this.colorInitialized = true;
		this.playerColor = color;
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x00085E0F File Offset: 0x0008400F
	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x00085E3D File Offset: 0x0008403D
	private void SendScoresToRoom()
	{
		if (this.netView != null && this._scoreUpdated)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.Others, new object[]
			{
				this.currentQuestScore
			});
		}
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x00085E7C File Offset: 0x0008407C
	private void SendScoresToGameModeRoom(GameModeType newGameModeType)
	{
		if (this.netView != null && this._rankedInfoUpdated && newGameModeType != GameModeType.InfectionCompetitive && !this.m_sentRankedScore)
		{
			this.m_sentRankedScore = true;
			this.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[]
			{
				this.currentRankedELO,
				this.currentRankedSubTierQuest,
				this.currentRankedSubTierPC
			});
		}
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x00085EF4 File Offset: 0x000840F4
	private void SendScoresToNewPlayer(NetPlayer player)
	{
		if (this.netView != null)
		{
			if (this._scoreUpdated)
			{
				this.netView.SendRPC("RPC_UpdateQuestScore", player, new object[]
				{
					this.currentQuestScore
				});
			}
			if (this._rankedInfoUpdated && !this.IsInRankedMode())
			{
				this.netView.SendRPC("RPC_UpdateRankedInfo", player, new object[]
				{
					this.currentRankedELO,
					this.currentRankedSubTierQuest,
					this.currentRankedSubTierPC
				});
			}
		}
	}

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x060018E7 RID: 6375 RVA: 0x00085F90 File Offset: 0x00084190
	// (remove) Token: 0x060018E8 RID: 6376 RVA: 0x00085FC8 File Offset: 0x000841C8
	public event Action<int> OnQuestScoreChanged;

	// Token: 0x060018E9 RID: 6377 RVA: 0x00086000 File Offset: 0x00084200
	public void SetQuestScore(int score)
	{
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged != null)
		{
			onQuestScoreChanged(this.currentQuestScore);
		}
		if (this.netView != null)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.Others, new object[]
			{
				this.currentQuestScore
			});
		}
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x0008605E File Offset: 0x0008425E
	public int GetCurrentQuestScore()
	{
		if (!this._scoreUpdated)
		{
			this.SetQuestScoreLocal(ProgressionController.TotalPoints);
		}
		return this.currentQuestScore;
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x00086079 File Offset: 0x00084279
	private void SetQuestScoreLocal(int score)
	{
		this.currentQuestScore = score;
		this._scoreUpdated = true;
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x0008608C File Offset: 0x0008428C
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateQuestScore");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.senderID != this.creator.ActorNumber)
		{
			return;
		}
		if (!this.updateQuestCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		if (score < this.currentQuestScore)
		{
			return;
		}
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged == null)
		{
			return;
		}
		onQuestScoreChanged(this.currentQuestScore);
	}

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x060018ED RID: 6381 RVA: 0x00086104 File Offset: 0x00084304
	// (remove) Token: 0x060018EE RID: 6382 RVA: 0x0008613C File Offset: 0x0008433C
	public event Action<int, int> OnRankedSubtierChanged;

	// Token: 0x060018EF RID: 6383 RVA: 0x00086174 File Offset: 0x00084374
	public void SetRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, bool broadcastToOtherClients = true)
	{
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged(rankedSubtierQuest, rankedSubtierPC);
		}
		if (this.netView != null && broadcastToOtherClients)
		{
			this.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[]
			{
				this.currentRankedELO,
				this.currentRankedSubTierQuest,
				this.currentRankedSubTierPC
			});
		}
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x000861EF File Offset: 0x000843EF
	public int GetCurrentRankedSubTier(bool getPC)
	{
		if (!this._rankedInfoUpdated)
		{
			return -1;
		}
		if (!getPC)
		{
			return this.currentRankedSubTierQuest;
		}
		return this.currentRankedSubTierPC;
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x0008620B File Offset: 0x0008440B
	private void SetRankedInfoLocal(float rankedELO, int rankedSubTierQuest, int rankedSubTierPC)
	{
		this.currentRankedELO = rankedELO;
		this.currentRankedSubTierQuest = rankedSubTierQuest;
		this.currentRankedSubTierPC = rankedSubTierPC;
		this._rankedInfoUpdated = true;
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x00086229 File Offset: 0x00084429
	private bool IsInRankedMode()
	{
		return GorillaGameModes.GameMode.ActiveGameMode != null && GorillaGameModes.GameMode.ActiveGameMode.GameType() == GameModeType.InfectionCompetitive;
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x00086248 File Offset: 0x00084448
	public void UpdateRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateRankedInfo");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		if (!rigContainer.Rig.updateRankedInfoCallLimit.CheckCallTime(Time.time) || info.senderID != this.creator.ActorNumber || !float.IsFinite(rankedELO))
		{
			return;
		}
		if (this.IsInRankedMode())
		{
			return;
		}
		if (RankedProgressionManager.Instance == null || !RankedProgressionManager.Instance.AreValuesValid(rankedELO, rankedSubtierQuest, rankedSubtierPC))
		{
			return;
		}
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged(rankedSubtierQuest, rankedSubtierPC);
		}
		RankedProgressionManager.Instance.HandlePlayerRankedInfoReceived(this.creator.ActorNumber, rankedELO, rankedSubtierPC);
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x00086318 File Offset: 0x00084518
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
		GorillaComputer.RegisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
		if (this.currentRopeSwingTarget != null)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RegisterCosmeticCallback(this.creator.ActorNumber, this);
		}
		this.bodyRenderer.SetDefaults();
		this.SetInvisibleToLocalPlayer(false);
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride += this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride += this.HandHold_HandPositionReleaseOverride;
			GorillaGameModes.GameMode.OnStartGameMode += this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent += new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.SendScoresToNewPlayer);
			return;
		}
		VRRigJobManager.Instance.RegisterVRRig(this);
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x00086400 File Offset: 0x00084600
	void IPreDisable.PreDisable()
	{
		try
		{
			this.ClearRopeData();
			if (this.currentRopeSwingTarget)
			{
				this.currentRopeSwingTarget.SetParent(base.transform);
			}
			this.EnableHuntWatch(false);
			this.EnablePaintbrawlCosmetics(false);
			this.ClearPartyMemberStatus();
			this.concatStringOfCosmeticsAllowed = "";
			this.rawCosmeticString = "";
			if (this.cosmeticSet != null)
			{
				this.mergedSet.DeactivateAllCosmetcs(this.myBodyDockPositions, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
				this.mergedSet.ClearSet(CosmeticsController.instance.nullItem);
				this.prevSet.ClearSet(CosmeticsController.instance.nullItem);
				this.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
				this.cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
			}
			if (!this.isOfflineVRRig)
			{
				PlayerCosmeticsSystem.RemoveCosmeticCallback(this.creator.ActorNumber);
				this.pendingCosmeticUpdate = true;
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.rightHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.rightHandLink);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x00086578 File Offset: 0x00084778
	public void OnDisable()
	{
		try
		{
			GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.gameMode);
			this.ChangeMaterialLocal(0);
			GorillaComputer.UnregisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
			this.netView = null;
			this.voiceAudio = null;
			this.muted = false;
			this.initialized = false;
			this.initializedCosmetics = false;
			this.inTryOnRoom = false;
			this.timeSpawned = 0f;
			this.setMatIndex = 0;
			this.currentCosmeticTries = 0;
			this.velocityHistoryList.Clear();
			this.netSyncPos.Reset();
			this.rightHand.netSyncPos.Reset();
			this.leftHand.netSyncPos.Reset();
			this.ForceResetFrozenEffect();
			this.nativeScale = (this.frameScale = (this.lastScaleFactor = 1f));
			base.transform.localScale = Vector3.one;
			this.currentQuestScore = 0;
			this._scoreUpdated = false;
			this.currentRankedELO = 0f;
			this.currentRankedSubTierQuest = 0;
			this.currentRankedSubTierPC = 0;
			this._rankedInfoUpdated = false;
			this.TemporaryCosmeticEffects.Clear();
			this.m_sentRankedScore = false;
			try
			{
				CallLimitType<CallLimiter>[] callSettings = this.fxSettings.callSettings;
				for (int i = 0; i < callSettings.Length; i++)
				{
					callSettings[i].CallLimitSettings.Reset();
				}
			}
			catch
			{
				Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
			}
		}
		catch (Exception)
		{
		}
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride -= this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride -= this.HandHold_HandPositionReleaseOverride;
			GorillaGameModes.GameMode.OnStartGameMode -= this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent -= new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.DeregisterVRRig(this);
		}
		EyeScannerMono.Unregister(this);
		this.creator = null;
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x0008678C File Offset: 0x0008498C
	private void HandHold_HandPositionReleaseOverride(HandHold hh, bool rightHand)
	{
		if (rightHand)
		{
			this.rightHand.handholdOverrideTarget = null;
			return;
		}
		this.leftHand.handholdOverrideTarget = null;
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x000867AA File Offset: 0x000849AA
	private void HandHold_HandPositionRequestOverride(HandHold hh, bool rightHand, Vector3 pos)
	{
		if (rightHand)
		{
			this.rightHand.handholdOverrideTarget = hh.transform;
			this.rightHand.handholdOverrideTargetOffset = pos;
			return;
		}
		this.leftHand.handholdOverrideTarget = hh.transform;
		this.leftHand.handholdOverrideTargetOffset = pos;
	}

	// Token: 0x060018F9 RID: 6393 RVA: 0x000867EC File Offset: 0x000849EC
	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameModeName() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaPaintbrawlManager || instance.GameModeName() == "PAINTBRAWL")
				{
					this.EnablePaintbrawlCosmetics(true);
				}
			}
			else
			{
				string gameModeString = NetworkSystem.Instance.GameModeString;
				if (!gameModeString.IsNullOrEmpty())
				{
					string text = gameModeString;
					if (text.Contains("HUNT"))
					{
						this.EnableHuntWatch(true);
					}
					else if (text.Contains("PAINTBRAWL"))
					{
						this.EnablePaintbrawlCosmetics(true);
					}
				}
			}
			this.UpdateFriendshipBracelet();
			if (this.IsLocalPartyMember && !this.isOfflineVRRig)
			{
				FriendshipGroupDetection.Instance.SendVerifyPartyMember(this.creator);
			}
		}
		if (this.netView != null)
		{
			base.transform.position = this.netView.gameObject.transform.position;
			base.transform.rotation = this.netView.gameObject.transform.rotation;
		}
		try
		{
			Action action = VRRig.newPlayerJoined;
			if (action != null)
			{
				action();
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x00086948 File Offset: 0x00084B48
	public void GrabbedByPlayer(VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
	{
		GorillaClimbable climbable = grabbedWithLeftHand ? grabbedByRig.leftHandHoldsPlayer : grabbedByRig.rightHandHoldsPlayer;
		GorillaHandClimber gorillaHandClimber;
		if (grabbedBody)
		{
			gorillaHandClimber = EquipmentInteractor.instance.BodyClimber;
		}
		else if (grabbedLeftHand)
		{
			gorillaHandClimber = EquipmentInteractor.instance.LeftClimber;
		}
		else
		{
			gorillaHandClimber = EquipmentInteractor.instance.RightClimber;
		}
		gorillaHandClimber.SetCanRelease(false);
		GTPlayer.Instance.BeginClimbing(climbable, gorillaHandClimber, null);
		this.grabbedRopeIsBody = grabbedBody;
		this.grabbedRopeIsLeft = grabbedLeftHand;
		this.grabbedRopeIndex = grabbedByRig.netView.ViewID;
		this.grabbedRopeBoneIndex = (grabbedWithLeftHand ? 1 : 0);
		this.grabbedRopeOffset = Vector3.zero;
		this.grabbedRopeIsPhotonView = true;
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x000869EC File Offset: 0x00084BEC
	public void DroppedByPlayer(VRRig grabbedByRig, Vector3 throwVelocity)
	{
		GorillaClimbable currentClimbable = GTPlayer.Instance.CurrentClimbable;
		if (GTPlayer.Instance.isClimbing && (currentClimbable == grabbedByRig.leftHandHoldsPlayer || currentClimbable == grabbedByRig.rightHandHoldsPlayer))
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 20f);
			GorillaHandClimber currentClimber = GTPlayer.Instance.CurrentClimber;
			GTPlayer.Instance.EndClimbing(currentClimber, false, false);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			this.grabbedRopeIsBody = false;
			this.grabbedRopeIsLeft = false;
			this.grabbedRopeIndex = -1;
			this.grabbedRopeBoneIndex = 0;
			this.grabbedRopeOffset = Vector3.zero;
			this.grabbedRopeIsPhotonView = false;
			return;
		}
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() && VRRig.LocalRig.leftHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.leftHandLink.BreakLink();
			VRRig.LocalRig.leftHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			return;
		}
		if (VRRig.LocalRig.rightHandLink.IsLinkActive() && VRRig.LocalRig.rightHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.rightHandLink.BreakLink();
			VRRig.LocalRig.rightHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
		}
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x00086B5C File Offset: 0x00084D5C
	public bool IsOnGround(float headCheckDistance, float handCheckDistance, out Vector3 groundNormal)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(position, Vector3.down * headCheckDistance * this.scaleFactor, instance.headCollider.radius * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		if (this.LocalCheckCollision(position2, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		if (this.LocalCheckCollision(position3, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		groundNormal = Vector3.up;
		return false;
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x00086C70 File Offset: 0x00084E70
	private bool LocalTestMovementCollision(Vector3 startPosition, Vector3 startVelocity, out Vector3 modifiedVelocity, out Vector3 finalPosition)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = startVelocity * Time.deltaTime;
		finalPosition = startPosition + vector;
		modifiedVelocity = startVelocity;
		Vector3 a;
		RaycastHit raycastHit;
		bool flag = this.LocalCheckCollision(startPosition, vector, instance.headCollider.radius * this.scaleFactor, out a, out raycastHit);
		if (flag)
		{
			finalPosition = a - vector.normalized * 0.01f;
			modifiedVelocity = startVelocity - raycastHit.normal * Vector3.Dot(raycastHit.normal, startVelocity);
		}
		Vector3 position = this.leftHand.rigTarget.position;
		Vector3 a2;
		RaycastHit raycastHit2;
		bool flag2 = this.LocalCheckCollision(position, vector, instance.minimumRaycastDistance * this.scaleFactor, out a2, out raycastHit2);
		if (flag2)
		{
			finalPosition = a2 - (this.leftHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		Vector3 position2 = this.rightHand.rigTarget.position;
		Vector3 a3;
		RaycastHit raycastHit3;
		bool flag3 = this.LocalCheckCollision(position2, vector, instance.minimumRaycastDistance * this.scaleFactor, out a3, out raycastHit3);
		if (flag3)
		{
			finalPosition = a3 - (this.rightHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		return flag || flag2 || flag3;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x00086E00 File Offset: 0x00085000
	public void TrySweptMoveTo(Vector3 targetPosition, out bool handCollided, out bool buttCollided)
	{
		Vector3 position = base.transform.position;
		this.TrySweptOffsetMove(targetPosition - position, out handCollided, out buttCollided);
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x00086E28 File Offset: 0x00085028
	public void TrySweptOffsetMove(Vector3 movement, out bool handCollided, out bool buttCollided)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector = position + movement;
		Vector3 startPosition = position;
		handCollided = false;
		buttCollided = false;
		Vector3 a;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(startPosition, movement, instance.headCollider.radius * this.scaleFactor, out a, out raycastHit))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = a - movement.normalized * 0.01f;
			}
			movement = vector - position;
			buttCollided = true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		Vector3 a2;
		RaycastHit raycastHit2;
		if (this.LocalCheckCollision(position2, movement, instance.minimumRaycastDistance * this.scaleFactor, out a2, out raycastHit2))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = a2 - (this.leftHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		Vector3 a3;
		RaycastHit raycastHit3;
		if (this.LocalCheckCollision(position3, movement, instance.minimumRaycastDistance * this.scaleFactor, out a3, out raycastHit3))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = a3 - (this.rightHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		base.transform.position = vector;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x00086FB8 File Offset: 0x000851B8
	private bool LocalCheckCollision(Vector3 startPosition, Vector3 movement, float radius, out Vector3 finalPosition, out RaycastHit hit)
	{
		GTPlayer instance = GTPlayer.Instance;
		finalPosition = startPosition + movement;
		RaycastHit raycastHit = default(RaycastHit);
		bool flag = false;
		Vector3 normalized = movement.normalized;
		int num = Physics.SphereCastNonAlloc(startPosition, radius, normalized, this.rayCastNonAllocColliders, movement.magnitude, instance.locomotionEnabledLayers.value);
		if (num > 0)
		{
			raycastHit = this.rayCastNonAllocColliders[0];
			for (int i = 0; i < num; i++)
			{
				if (raycastHit.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < raycastHit.distance))
				{
					flag = true;
					raycastHit = this.rayCastNonAllocColliders[i];
				}
			}
		}
		hit = raycastHit;
		if (flag)
		{
			finalPosition = startPosition + normalized * (raycastHit.distance - 0.01f);
			return true;
		}
		return false;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x0008709C File Offset: 0x0008529C
	public void UpdateFriendshipBracelet()
	{
		bool flag = false;
		if (this.isOfflineVRRig)
		{
			bool flag2 = false;
			VRRig.PartyMemberStatus partyMemberStatus = this.GetPartyMemberStatus();
			if (partyMemberStatus != VRRig.PartyMemberStatus.InLocalParty)
			{
				if (partyMemberStatus == VRRig.PartyMemberStatus.NotInLocalParty)
				{
					flag2 = false;
					this.reliableState.isBraceletLeftHanded = false;
				}
			}
			else
			{
				flag2 = true;
				this.reliableState.isBraceletLeftHanded = (FriendshipGroupDetection.Instance.DidJoinLeftHanded && !this.huntComputer.activeSelf);
			}
			if (this.reliableState.HasBracelet != flag2 || this.reliableState.braceletBeadColors.Count != FriendshipGroupDetection.Instance.myBeadColors.Count)
			{
				this.reliableState.SetIsDirty();
				flag = (this.reliableState.HasBracelet == flag2);
			}
			this.reliableState.braceletBeadColors.Clear();
			if (flag2)
			{
				this.reliableState.braceletBeadColors.AddRange(FriendshipGroupDetection.Instance.myBeadColors);
			}
			this.reliableState.braceletSelfIndex = FriendshipGroupDetection.Instance.MyBraceletSelfIndex;
		}
		if (this.nonCosmeticLeftHandItem != null)
		{
			bool flag3 = this.reliableState.HasBracelet && this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticLeftHandItem.EnableItem(flag3);
			if (flag3)
			{
				this.friendshipBraceletLeftHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletLeftHand.PlayAppearEffects();
				}
			}
		}
		if (this.nonCosmeticRightHandItem != null)
		{
			bool flag4 = this.reliableState.HasBracelet && !this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticRightHandItem.EnableItem(flag4);
			if (flag4)
			{
				this.friendshipBraceletRightHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletRightHand.PlayAppearEffects();
				}
			}
		}
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x0008726C File Offset: 0x0008546C
	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
		if (this.builderResizeWatch != null)
		{
			MeshRenderer component = this.builderResizeWatch.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = !on;
			}
		}
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000872B2 File Offset: 0x000854B2
	public void EnablePaintbrawlCosmetics(bool on)
	{
		this.paintbrawlBalloons.gameObject.SetActive(on);
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x000872C8 File Offset: 0x000854C8
	public void EnableBuilderResizeWatch(bool on)
	{
		if (this.builderResizeWatch != null && this.builderResizeWatch.activeSelf != on)
		{
			this.builderResizeWatch.SetActive(on);
			if (this.builderArmShelfLeft != null)
			{
				this.builderArmShelfLeft.gameObject.SetActive(on);
			}
			if (this.builderArmShelfRight != null)
			{
				this.builderArmShelfRight.gameObject.SetActive(on);
			}
		}
		if (this.isOfflineVRRig)
		{
			bool flag = this.reliableState.isBuilderWatchEnabled != on;
			this.reliableState.isBuilderWatchEnabled = on;
			if (flag)
			{
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x0008736D File Offset: 0x0008556D
	public void EnableGuardianEjectWatch(bool on)
	{
		if (this.guardianEjectWatch != null && this.guardianEjectWatch.activeSelf != on)
		{
			this.guardianEjectWatch.SetActive(on);
		}
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x00087397 File Offset: 0x00085597
	public void EnableVStumpReturnWatch(bool on)
	{
		if (this.vStumpReturnWatch != null && this.vStumpReturnWatch.activeSelf != on)
		{
			this.vStumpReturnWatch.SetActive(on);
		}
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x000873C1 File Offset: 0x000855C1
	public void EnableRankedTimerWatch(bool on)
	{
		if (this.rankedTimerWatch != null && this.rankedTimerWatch.activeSelf != on)
		{
			this.rankedTimerWatch.SetActive(on);
		}
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x000873EC File Offset: 0x000855EC
	private void UpdateReplacementVoice()
	{
		if (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn != "TRUE")
		{
			this.voiceAudio.mute = true;
			return;
		}
		this.voiceAudio.mute = false;
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x0008743C File Offset: 0x0008563C
	public bool ShouldPlayReplacementVoice()
	{
		return this.netView && !this.netView.IsMine && !(GorillaComputer.instance.voiceChatOn == "OFF") && (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && this.SpeakingLoudness > this.replacementVoiceLoudnessThreshold;
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x000874B7 File Offset: 0x000856B7
	public void SetDuplicationZone(RigDuplicationZone duplicationZone)
	{
		this.duplicationZone = duplicationZone;
		this.inDuplicationZone = (duplicationZone != null);
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x000874CD File Offset: 0x000856CD
	public void ClearDuplicationZone(RigDuplicationZone duplicationZone)
	{
		if (this.duplicationZone == duplicationZone)
		{
			this.SetDuplicationZone(null);
			this.renderTransform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000874F4 File Offset: 0x000856F4
	public void ResetTimeSpawned()
	{
		this.timeSpawned = Time.time;
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x0600190D RID: 6413 RVA: 0x00087501 File Offset: 0x00085701
	// (set) Token: 0x0600190E RID: 6414 RVA: 0x00087509 File Offset: 0x00085709
	bool IUserCosmeticsCallback.PendingUpdate
	{
		get
		{
			return this.pendingCosmeticUpdate;
		}
		set
		{
			this.pendingCosmeticUpdate = value;
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x0600190F RID: 6415 RVA: 0x00087512 File Offset: 0x00085712
	// (set) Token: 0x06001910 RID: 6416 RVA: 0x0008751A File Offset: 0x0008571A
	public bool IsFrozen { get; set; }

	// Token: 0x06001911 RID: 6417 RVA: 0x00087524 File Offset: 0x00085724
	bool IUserCosmeticsCallback.OnGetUserCosmetics(string cosmetics)
	{
		if (cosmetics == this.rawCosmeticString && this.currentCosmeticTries < this.cosmeticRetries)
		{
			this.currentCosmeticTries++;
			return false;
		}
		this.rawCosmeticString = (cosmetics ?? "");
		this.concatStringOfCosmeticsAllowed = this.rawCosmeticString;
		this.concatStringOfCosmeticsAllowed += "LHAJJ.LHAJK.LHAJL.";
		this.InitializedCosmetics = true;
		this.currentCosmeticTries = 0;
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive();
		this.myBodyDockPositions.RefreshTransferrableItems();
		NetworkView networkView = this.netView;
		if (networkView != null)
		{
			networkView.SendRPC("RPC_RequestCosmetics", this.creator, Array.Empty<object>());
		}
		return true;
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x000875D8 File Offset: 0x000857D8
	private short PackCompetitiveData()
	{
		if (!this.turningCompInitialized)
		{
			this.GorillaSnapTurningComp = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			this.turningCompInitialized = true;
		}
		this.fps = Mathf.Min(Mathf.RoundToInt(1f / Time.smoothDeltaTime), 255);
		int num = 0;
		if (this.GorillaSnapTurningComp != null)
		{
			this.turnFactor = this.GorillaSnapTurningComp.turnFactor;
			this.turnType = this.GorillaSnapTurningComp.turnType;
			string a = this.turnType;
			if (!(a == "SNAP"))
			{
				if (a == "SMOOTH")
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
			num *= 10;
			num += this.turnFactor;
		}
		return (short)(this.fps + (num << 8));
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00087698 File Offset: 0x00085898
	private void UnpackCompetitiveData(short packed)
	{
		int num = 255;
		this.fps = ((int)packed & num);
		int num2 = 31;
		int num3 = packed >> 8 & num2;
		this.turnFactor = num3 % 10;
		int num4 = num3 / 10;
		if (num4 == 1)
		{
			this.turnType = "SNAP";
			return;
		}
		if (num4 != 2)
		{
			this.turnType = "NONE";
			return;
		}
		this.turnType = "SMOOTH";
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000876FC File Offset: 0x000858FC
	private void OnKIDSessionUpdated(bool showCustomNames, Permission.ManagedByEnum managedBy)
	{
		bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
		GorillaComputer.instance.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
		{
			GorillaComputer.ComputerState.Name
		}, false);
		bool flag2 = PlayerPrefs.GetInt("nameTagsOn", -1) > 0;
		switch (managedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			flag = GorillaComputer.instance.NametagsEnabled;
			break;
		case Permission.ManagedByEnum.GUARDIAN:
			flag = (showCustomNames && flag2);
			break;
		case Permission.ManagedByEnum.PROHIBITED:
			flag = false;
			break;
		}
		this.UpdateName(flag);
		Debug.Log("[KID] On Session Update - Custom Names Permission changed - Has enabled customNames? [" + flag.ToString() + "]");
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06001915 RID: 6421 RVA: 0x00087798 File Offset: 0x00085998
	public static VRRig LocalRig
	{
		get
		{
			return VRRig.gLocalRig;
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06001916 RID: 6422 RVA: 0x0008779F File Offset: 0x0008599F
	public bool isLocal
	{
		get
		{
			return VRRig.gLocalRig == this;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06001917 RID: 6423 RVA: 0x0000FE93 File Offset: 0x0000E093
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06001918 RID: 6424 RVA: 0x000877AC File Offset: 0x000859AC
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001919 RID: 6425 RVA: 0x000877BC File Offset: 0x000859BC
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return default(Bounds);
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x0600191A RID: 6426 RVA: 0x000877D2 File Offset: 0x000859D2
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.buildEntries();
		}
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x000877DC File Offset: 0x000859DC
	private IList<KeyValueStringPair> buildEntries()
	{
		return new KeyValueStringPair[]
		{
			new KeyValueStringPair("Name", this.playerNameVisible),
			new KeyValueStringPair("Color", string.Format("{0}, {1}, {2}", Mathf.RoundToInt(this.playerColor.r * 9f), Mathf.RoundToInt(this.playerColor.g * 9f), Mathf.RoundToInt(this.playerColor.b * 9f)))
		};
	}

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x0600191C RID: 6428 RVA: 0x00087874 File Offset: 0x00085A74
	// (remove) Token: 0x0600191D RID: 6429 RVA: 0x000878AC File Offset: 0x00085AAC
	public event Action OnDataChange;

	// Token: 0x04002001 RID: 8193
	private bool _isListeningFor_OnPostInstantiateAllPrefabs;

	// Token: 0x04002002 RID: 8194
	public static Action newPlayerJoined;

	// Token: 0x04002003 RID: 8195
	public VRMap head;

	// Token: 0x04002004 RID: 8196
	public VRMap rightHand;

	// Token: 0x04002005 RID: 8197
	public VRMap leftHand;

	// Token: 0x04002006 RID: 8198
	public VRMapThumb leftThumb;

	// Token: 0x04002007 RID: 8199
	public VRMapIndex leftIndex;

	// Token: 0x04002008 RID: 8200
	public VRMapMiddle leftMiddle;

	// Token: 0x04002009 RID: 8201
	public VRMapThumb rightThumb;

	// Token: 0x0400200A RID: 8202
	public VRMapIndex rightIndex;

	// Token: 0x0400200B RID: 8203
	public VRMapMiddle rightMiddle;

	// Token: 0x0400200C RID: 8204
	public CrittersLoudNoise leftHandNoise;

	// Token: 0x0400200D RID: 8205
	public CrittersLoudNoise rightHandNoise;

	// Token: 0x0400200E RID: 8206
	public CrittersLoudNoise speakingNoise;

	// Token: 0x0400200F RID: 8207
	private int previousGrabbedRope = -1;

	// Token: 0x04002010 RID: 8208
	private int previousGrabbedRopeBoneIndex;

	// Token: 0x04002011 RID: 8209
	private bool previousGrabbedRopeWasLeft;

	// Token: 0x04002012 RID: 8210
	private bool previousGrabbedRopeWasBody;

	// Token: 0x04002013 RID: 8211
	private GorillaRopeSwing currentRopeSwing;

	// Token: 0x04002014 RID: 8212
	private Transform currentHoldParent;

	// Token: 0x04002015 RID: 8213
	private Transform currentRopeSwingTarget;

	// Token: 0x04002016 RID: 8214
	private float lastRopeGrabTimer;

	// Token: 0x04002017 RID: 8215
	private bool shouldLerpToRope;

	// Token: 0x04002018 RID: 8216
	[NonSerialized]
	public int grabbedRopeIndex = -1;

	// Token: 0x04002019 RID: 8217
	[NonSerialized]
	public int grabbedRopeBoneIndex;

	// Token: 0x0400201A RID: 8218
	[NonSerialized]
	public bool grabbedRopeIsLeft;

	// Token: 0x0400201B RID: 8219
	[NonSerialized]
	public bool grabbedRopeIsBody;

	// Token: 0x0400201C RID: 8220
	[NonSerialized]
	public bool grabbedRopeIsPhotonView;

	// Token: 0x0400201D RID: 8221
	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	// Token: 0x0400201E RID: 8222
	private int prevMovingSurfaceID = -1;

	// Token: 0x0400201F RID: 8223
	private bool movingSurfaceWasLeft;

	// Token: 0x04002020 RID: 8224
	private bool movingSurfaceWasBody;

	// Token: 0x04002021 RID: 8225
	private bool movingSurfaceWasMonkeBlock;

	// Token: 0x04002022 RID: 8226
	[NonSerialized]
	public int mountedMovingSurfaceId = -1;

	// Token: 0x04002023 RID: 8227
	[NonSerialized]
	private BuilderPiece mountedMonkeBlock;

	// Token: 0x04002024 RID: 8228
	[NonSerialized]
	private MovingSurface mountedMovingSurface;

	// Token: 0x04002025 RID: 8229
	[NonSerialized]
	public bool mountedMovingSurfaceIsLeft;

	// Token: 0x04002026 RID: 8230
	[NonSerialized]
	public bool mountedMovingSurfaceIsBody;

	// Token: 0x04002027 RID: 8231
	[NonSerialized]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x04002028 RID: 8232
	[NonSerialized]
	public Vector3 mountedMonkeBlockOffset = Vector3.zero;

	// Token: 0x04002029 RID: 8233
	private float lastMountedSurfaceTimer;

	// Token: 0x0400202A RID: 8234
	private bool shouldLerpToMovingSurface;

	// Token: 0x0400202B RID: 8235
	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	// Token: 0x0400202C RID: 8236
	public GameObject mainCamera;

	// Token: 0x0400202D RID: 8237
	public Transform playerOffsetTransform;

	// Token: 0x0400202E RID: 8238
	public int SDKIndex;

	// Token: 0x0400202F RID: 8239
	public bool isMyPlayer;

	// Token: 0x04002030 RID: 8240
	public AudioSource leftHandPlayer;

	// Token: 0x04002031 RID: 8241
	public AudioSource rightHandPlayer;

	// Token: 0x04002032 RID: 8242
	public AudioSource tagSound;

	// Token: 0x04002033 RID: 8243
	[SerializeField]
	private float ratio;

	// Token: 0x04002034 RID: 8244
	public Transform headConstraint;

	// Token: 0x04002035 RID: 8245
	public Vector3 headBodyOffset = Vector3.zero;

	// Token: 0x04002036 RID: 8246
	public GameObject headMesh;

	// Token: 0x04002037 RID: 8247
	private NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04002038 RID: 8248
	public Vector3 jobPos;

	// Token: 0x04002039 RID: 8249
	public Quaternion syncRotation;

	// Token: 0x0400203A RID: 8250
	public Quaternion jobRotation;

	// Token: 0x0400203B RID: 8251
	public AudioClip[] clipToPlay;

	// Token: 0x0400203C RID: 8252
	public AudioClip[] handTapSound;

	// Token: 0x0400203D RID: 8253
	public int currentMatIndex;

	// Token: 0x0400203E RID: 8254
	public int setMatIndex;

	// Token: 0x0400203F RID: 8255
	public float lerpValueFingers;

	// Token: 0x04002040 RID: 8256
	public float lerpValueBody;

	// Token: 0x04002041 RID: 8257
	public GameObject backpack;

	// Token: 0x04002042 RID: 8258
	public Transform leftHandTransform;

	// Token: 0x04002043 RID: 8259
	public Transform rightHandTransform;

	// Token: 0x04002044 RID: 8260
	public Transform bodyTransform;

	// Token: 0x04002045 RID: 8261
	public SkinnedMeshRenderer mainSkin;

	// Token: 0x04002046 RID: 8262
	public GorillaSkin defaultSkin;

	// Token: 0x04002047 RID: 8263
	public MeshRenderer faceSkin;

	// Token: 0x04002048 RID: 8264
	public XRaySkeleton skeleton;

	// Token: 0x04002049 RID: 8265
	public GorillaBodyRenderer bodyRenderer;

	// Token: 0x0400204A RID: 8266
	public ZoneEntity zoneEntity;

	// Token: 0x0400204B RID: 8267
	public Material myDefaultSkinMaterialInstance;

	// Token: 0x0400204C RID: 8268
	public Material scoreboardMaterial;

	// Token: 0x0400204D RID: 8269
	public GameObject spectatorSkin;

	// Token: 0x0400204E RID: 8270
	public int handSync;

	// Token: 0x0400204F RID: 8271
	public Material[] materialsToChangeTo;

	// Token: 0x04002050 RID: 8272
	public float red;

	// Token: 0x04002051 RID: 8273
	public float green;

	// Token: 0x04002052 RID: 8274
	public float blue;

	// Token: 0x04002053 RID: 8275
	public TextMeshPro playerText1;

	// Token: 0x04002054 RID: 8276
	public TextMeshPro playerText2;

	// Token: 0x04002055 RID: 8277
	public string playerNameVisible;

	// Token: 0x04002056 RID: 8278
	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	// Token: 0x04002057 RID: 8279
	public CosmeticItemRegistry cosmeticsObjectRegistry = new CosmeticItemRegistry();

	// Token: 0x04002058 RID: 8280
	[NonSerialized]
	public PropHuntHandFollower propHuntHandFollower;

	// Token: 0x04002059 RID: 8281
	[FormerlySerializedAs("cosmetics")]
	public GameObject[] _cosmetics;

	// Token: 0x0400205A RID: 8282
	[FormerlySerializedAs("overrideCosmetics")]
	public GameObject[] _overrideCosmetics;

	// Token: 0x0400205B RID: 8283
	private int taggedById;

	// Token: 0x0400205C RID: 8284
	public string concatStringOfCosmeticsAllowed = "";

	// Token: 0x0400205D RID: 8285
	private bool initializedCosmetics;

	// Token: 0x0400205E RID: 8286
	public CosmeticsController.CosmeticSet cosmeticSet;

	// Token: 0x0400205F RID: 8287
	public CosmeticsController.CosmeticSet tryOnSet;

	// Token: 0x04002060 RID: 8288
	public CosmeticsController.CosmeticSet mergedSet;

	// Token: 0x04002061 RID: 8289
	public CosmeticsController.CosmeticSet prevSet;

	// Token: 0x04002062 RID: 8290
	[NonSerialized]
	public readonly List<GameObject> activeCosmetics = new List<GameObject>(16);

	// Token: 0x04002063 RID: 8291
	private int cosmeticRetries = 2;

	// Token: 0x04002064 RID: 8292
	private int currentCosmeticTries;

	// Token: 0x04002066 RID: 8294
	public SizeManager sizeManager;

	// Token: 0x04002067 RID: 8295
	public float pitchScale = 0.3f;

	// Token: 0x04002068 RID: 8296
	public float pitchOffset = 1f;

	// Token: 0x04002069 RID: 8297
	[NonSerialized]
	public bool IsHaunted;

	// Token: 0x0400206A RID: 8298
	public float HauntedVoicePitch = 0.5f;

	// Token: 0x0400206B RID: 8299
	public float HauntedHearingVolume = 0.15f;

	// Token: 0x0400206C RID: 8300
	[NonSerialized]
	public bool UsingHauntedRing;

	// Token: 0x0400206D RID: 8301
	[NonSerialized]
	public float HauntedRingVoicePitch;

	// Token: 0x0400206E RID: 8302
	public FriendshipBracelet friendshipBraceletLeftHand;

	// Token: 0x0400206F RID: 8303
	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	// Token: 0x04002070 RID: 8304
	public FriendshipBracelet friendshipBraceletRightHand;

	// Token: 0x04002071 RID: 8305
	public NonCosmeticHandItem nonCosmeticRightHandItem;

	// Token: 0x04002072 RID: 8306
	public HoverboardVisual hoverboardVisual;

	// Token: 0x04002073 RID: 8307
	private int hoverboardEnabledCount;

	// Token: 0x04002074 RID: 8308
	public HoldableHand bodyHolds;

	// Token: 0x04002075 RID: 8309
	public HoldableHand leftHolds;

	// Token: 0x04002076 RID: 8310
	public HoldableHand rightHolds;

	// Token: 0x04002077 RID: 8311
	public GorillaClimbable leftHandHoldsPlayer;

	// Token: 0x04002078 RID: 8312
	public GorillaClimbable rightHandHoldsPlayer;

	// Token: 0x04002079 RID: 8313
	public HandLink leftHandLink;

	// Token: 0x0400207A RID: 8314
	public HandLink rightHandLink;

	// Token: 0x0400207D RID: 8317
	public GameObject nameTagAnchor;

	// Token: 0x0400207E RID: 8318
	public GameObject frozenEffect;

	// Token: 0x0400207F RID: 8319
	public GameObject iceCubeLeft;

	// Token: 0x04002080 RID: 8320
	public GameObject iceCubeRight;

	// Token: 0x04002081 RID: 8321
	public float frozenEffectMaxY;

	// Token: 0x04002082 RID: 8322
	public float frozenEffectMaxHorizontalScale = 0.8f;

	// Token: 0x04002083 RID: 8323
	public GameObject FPVEffectsParent;

	// Token: 0x04002084 RID: 8324
	public Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> TemporaryCosmeticEffects = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

	// Token: 0x04002085 RID: 8325
	private float cosmeticStepsDuration;

	// Token: 0x04002086 RID: 8326
	private CosmeticSwapper cosmeticSwapper;

	// Token: 0x04002087 RID: 8327
	private Stack<CosmeticSwapper.CosmeticState> newSwappedCosmetics = new Stack<CosmeticSwapper.CosmeticState>();

	// Token: 0x04002089 RID: 8329
	private bool isAtFinalCosmeticStep;

	// Token: 0x0400208A RID: 8330
	private float _nextUpdateTime = -1f;

	// Token: 0x0400208B RID: 8331
	public VRRigReliableState reliableState;

	// Token: 0x0400208C RID: 8332
	[SerializeField]
	private Transform MouthPosition;

	// Token: 0x04002090 RID: 8336
	internal RigContainer rigContainer;

	// Token: 0x04002091 RID: 8337
	public Action<RigContainer> OnNameChanged;

	// Token: 0x04002092 RID: 8338
	private Vector3 remoteVelocity;

	// Token: 0x04002093 RID: 8339
	private double remoteLatestTimestamp;

	// Token: 0x04002094 RID: 8340
	private Vector3 remoteCorrectionNeeded;

	// Token: 0x04002095 RID: 8341
	private const float REMOTE_CORRECTION_RATE = 5f;

	// Token: 0x04002096 RID: 8342
	private const bool USE_NEW_NETCODE = false;

	// Token: 0x04002097 RID: 8343
	private float stealthTimer;

	// Token: 0x04002098 RID: 8344
	private GorillaAmbushManager stealthManager;

	// Token: 0x04002099 RID: 8345
	private LayerChanger layerChanger;

	// Token: 0x0400209A RID: 8346
	private float frozenEffectMinY;

	// Token: 0x0400209B RID: 8347
	private float frozenEffectMinHorizontalScale;

	// Token: 0x0400209C RID: 8348
	private float frozenTimeElapsed;

	// Token: 0x0400209D RID: 8349
	public TagEffectPack CosmeticEffectPack;

	// Token: 0x0400209E RID: 8350
	private GorillaSnapTurn GorillaSnapTurningComp;

	// Token: 0x0400209F RID: 8351
	private bool turningCompInitialized;

	// Token: 0x040020A0 RID: 8352
	private string turnType = "NONE";

	// Token: 0x040020A1 RID: 8353
	private int turnFactor;

	// Token: 0x040020A2 RID: 8354
	private int fps;

	// Token: 0x040020A3 RID: 8355
	private VRRig.PartyMemberStatus partyMemberStatus;

	// Token: 0x040020A4 RID: 8356
	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2),
		new GTBitOps.BitWriteInfo(5, 2),
		new GTBitOps.BitWriteInfo(7, 2),
		new GTBitOps.BitWriteInfo(9, 2)
	};

	// Token: 0x040020A5 RID: 8357
	public bool inTryOnRoom;

	// Token: 0x040020A6 RID: 8358
	public bool muted;

	// Token: 0x040020A7 RID: 8359
	private float lastScaleFactor = 1f;

	// Token: 0x040020A8 RID: 8360
	private float scaleMultiplier = 1f;

	// Token: 0x040020A9 RID: 8361
	private float nativeScale = 1f;

	// Token: 0x040020AA RID: 8362
	private float timeSpawned;

	// Token: 0x040020AB RID: 8363
	public float doNotLerpConstant = 1f;

	// Token: 0x040020AC RID: 8364
	public string tempString;

	// Token: 0x040020AD RID: 8365
	private Player tempPlayer;

	// Token: 0x040020AE RID: 8366
	internal NetPlayer creator;

	// Token: 0x040020AF RID: 8367
	private float[] speedArray;

	// Token: 0x040020B0 RID: 8368
	private double handLerpValues;

	// Token: 0x040020B1 RID: 8369
	private bool initialized;

	// Token: 0x040020B2 RID: 8370
	[FormerlySerializedAs("battleBalloons")]
	public PaintbrawlBalloons paintbrawlBalloons;

	// Token: 0x040020B3 RID: 8371
	private int tempInt;

	// Token: 0x040020B4 RID: 8372
	public BodyDockPositions myBodyDockPositions;

	// Token: 0x040020B5 RID: 8373
	public ParticleSystem lavaParticleSystem;

	// Token: 0x040020B6 RID: 8374
	public ParticleSystem rockParticleSystem;

	// Token: 0x040020B7 RID: 8375
	public ParticleSystem iceParticleSystem;

	// Token: 0x040020B8 RID: 8376
	public ParticleSystem snowFlakeParticleSystem;

	// Token: 0x040020B9 RID: 8377
	public string tempItemName;

	// Token: 0x040020BA RID: 8378
	public CosmeticsController.CosmeticItem tempItem;

	// Token: 0x040020BB RID: 8379
	public string tempItemId;

	// Token: 0x040020BC RID: 8380
	public int tempItemCost;

	// Token: 0x040020BD RID: 8381
	public int leftHandHoldableStatus;

	// Token: 0x040020BE RID: 8382
	public int rightHandHoldableStatus;

	// Token: 0x040020BF RID: 8383
	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	// Token: 0x040020C0 RID: 8384
	private List<TransferrableObject> instrumentSelfOnly = new List<TransferrableObject>();

	// Token: 0x040020C1 RID: 8385
	public AudioSource geodeCrackingSound;

	// Token: 0x040020C2 RID: 8386
	public float bonkTime;

	// Token: 0x040020C3 RID: 8387
	public float bonkCooldown = 2f;

	// Token: 0x040020C4 RID: 8388
	private VRRig tempVRRig;

	// Token: 0x040020C5 RID: 8389
	public GameObject huntComputer;

	// Token: 0x040020C6 RID: 8390
	public GameObject builderResizeWatch;

	// Token: 0x040020C7 RID: 8391
	public BuilderArmShelf builderArmShelfLeft;

	// Token: 0x040020C8 RID: 8392
	public BuilderArmShelf builderArmShelfRight;

	// Token: 0x040020C9 RID: 8393
	public GameObject guardianEjectWatch;

	// Token: 0x040020CA RID: 8394
	public GameObject vStumpReturnWatch;

	// Token: 0x040020CB RID: 8395
	public GameObject rankedTimerWatch;

	// Token: 0x040020CC RID: 8396
	public ProjectileWeapon projectileWeapon;

	// Token: 0x040020CD RID: 8397
	private PhotonVoiceView myPhotonVoiceView;

	// Token: 0x040020CE RID: 8398
	private VRRig senderRig;

	// Token: 0x040020CF RID: 8399
	private bool isInitialized;

	// Token: 0x040020D0 RID: 8400
	private CircularBuffer<VRRig.VelocityTime> velocityHistoryList = new CircularBuffer<VRRig.VelocityTime>(200);

	// Token: 0x040020D1 RID: 8401
	public int velocityHistoryMaxLength = 200;

	// Token: 0x040020D2 RID: 8402
	private Vector3 lastPosition;

	// Token: 0x040020D3 RID: 8403
	public const int splashLimitCount = 4;

	// Token: 0x040020D4 RID: 8404
	public const float splashLimitCooldown = 0.5f;

	// Token: 0x040020D5 RID: 8405
	private float[] splashEffectTimes = new float[4];

	// Token: 0x040020D6 RID: 8406
	internal AudioSource voiceAudio;

	// Token: 0x040020D7 RID: 8407
	public bool remoteUseReplacementVoice;

	// Token: 0x040020D8 RID: 8408
	public bool localUseReplacementVoice;

	// Token: 0x040020D9 RID: 8409
	private MicWrapper currentMicWrapper;

	// Token: 0x040020DA RID: 8410
	private IAudioDesc audioDesc;

	// Token: 0x040020DB RID: 8411
	private float speakingLoudness;

	// Token: 0x040020DC RID: 8412
	public bool shouldSendSpeakingLoudness = true;

	// Token: 0x040020DD RID: 8413
	public float replacementVoiceLoudnessThreshold = 0.05f;

	// Token: 0x040020DE RID: 8414
	public int replacementVoiceDetectionDelay = 128;

	// Token: 0x040020DF RID: 8415
	private GorillaMouthFlap myMouthFlap;

	// Token: 0x040020E0 RID: 8416
	private GorillaSpeakerLoudness mySpeakerLoudness;

	// Token: 0x040020E1 RID: 8417
	public ReplacementVoice myReplacementVoice;

	// Token: 0x040020E2 RID: 8418
	private GorillaEyeExpressions myEyeExpressions;

	// Token: 0x040020E3 RID: 8419
	[SerializeField]
	internal NetworkView netView;

	// Token: 0x040020E4 RID: 8420
	[SerializeField]
	internal VRRigSerializer rigSerializer;

	// Token: 0x040020E5 RID: 8421
	public NetPlayer OwningNetPlayer;

	// Token: 0x040020E6 RID: 8422
	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	// Token: 0x040020E7 RID: 8423
	[NonSerialized]
	public FXSystemSettings fxSettings;

	// Token: 0x040020E8 RID: 8424
	[SerializeField]
	private float tapPointDistance = 0.035f;

	// Token: 0x040020E9 RID: 8425
	[SerializeField]
	private float handSpeedToVolumeModifier = 0.05f;

	// Token: 0x040020EA RID: 8426
	[SerializeField]
	private HandEffectContext _leftHandEffect;

	// Token: 0x040020EB RID: 8427
	[SerializeField]
	private HandEffectContext _rightHandEffect;

	// Token: 0x040020EC RID: 8428
	private bool _rigBuildFullyInitialized;

	// Token: 0x040020ED RID: 8429
	[SerializeField]
	private Transform renderTransform;

	// Token: 0x040020EE RID: 8430
	private GamePlayer _gamePlayerRef;

	// Token: 0x040020EF RID: 8431
	private bool playerWasHaunted;

	// Token: 0x040020F0 RID: 8432
	private float nonHauntedVolume;

	// Token: 0x040020F1 RID: 8433
	[SerializeField]
	private AnimationCurve voicePitchForRelativeScale;

	// Token: 0x040020F2 RID: 8434
	private Vector3 LocalTrajectoryOverridePosition;

	// Token: 0x040020F3 RID: 8435
	private Vector3 LocalTrajectoryOverrideVelocity;

	// Token: 0x040020F4 RID: 8436
	private float LocalTrajectoryOverrideBlend;

	// Token: 0x040020F5 RID: 8437
	[SerializeField]
	private float LocalTrajectoryOverrideDuration = 1f;

	// Token: 0x040020F6 RID: 8438
	private bool localOverrideIsBody;

	// Token: 0x040020F7 RID: 8439
	private bool localOverrideIsLeftHand;

	// Token: 0x040020F8 RID: 8440
	private Transform localOverrideGrabbingHand;

	// Token: 0x040020F9 RID: 8441
	private float localGrabOverrideBlend;

	// Token: 0x040020FA RID: 8442
	[SerializeField]
	private float LocalGrabOverrideDuration = 0.25f;

	// Token: 0x040020FB RID: 8443
	private float[] voiceSampleBuffer = new float[128];

	// Token: 0x040020FC RID: 8444
	private const int CHECK_LOUDNESS_FREQ_FRAMES = 10;

	// Token: 0x040020FD RID: 8445
	private CallbackContainer<ICallBack> lateUpdateCallbacks = new CallbackContainer<ICallBack>(5);

	// Token: 0x040020FE RID: 8446
	private float nextLocalVelocityStoreTimestamp;

	// Token: 0x040020FF RID: 8447
	private bool IsInvisibleToLocalPlayer;

	// Token: 0x04002100 RID: 8448
	private const int remoteUseReplacementVoice_BIT = 512;

	// Token: 0x04002101 RID: 8449
	private const int grabbedRope_BIT = 1024;

	// Token: 0x04002102 RID: 8450
	private const int grabbedRopeIsPhotonView_BIT = 2048;

	// Token: 0x04002103 RID: 8451
	private const int isHoldingHandsWithPlayer_BIT = 4096;

	// Token: 0x04002104 RID: 8452
	private const int isHoldingHoverboard_BIT = 8192;

	// Token: 0x04002105 RID: 8453
	private const int isHoverboardLeftHanded_BIT = 16384;

	// Token: 0x04002106 RID: 8454
	private const int isOnMovingSurface_BIT = 32768;

	// Token: 0x04002107 RID: 8455
	private const int isPropHunt_BIT = 65536;

	// Token: 0x04002108 RID: 8456
	private const int propHuntLeftHand_BIT = 131072;

	// Token: 0x04002109 RID: 8457
	private const int isLeftHandGrabbable_BIT = 262144;

	// Token: 0x0400210A RID: 8458
	private const int isRightHandGrabbable_BIT = 524288;

	// Token: 0x0400210B RID: 8459
	private const int speakingLoudnessVal_BITSHIFT = 24;

	// Token: 0x0400210C RID: 8460
	private Vector3 tempVec;

	// Token: 0x0400210D RID: 8461
	private Quaternion tempQuat;

	// Token: 0x0400210E RID: 8462
	public Color playerColor;

	// Token: 0x0400210F RID: 8463
	public bool colorInitialized;

	// Token: 0x04002110 RID: 8464
	private Action<Color> onColorInitialized;

	// Token: 0x04002113 RID: 8467
	private bool m_sentRankedScore;

	// Token: 0x04002115 RID: 8469
	private int currentQuestScore;

	// Token: 0x04002116 RID: 8470
	private bool _scoreUpdated;

	// Token: 0x04002117 RID: 8471
	private CallLimiter updateQuestCallLimit = new CallLimiter(1, 0.5f, 0.5f);

	// Token: 0x04002119 RID: 8473
	private float currentRankedELO;

	// Token: 0x0400211A RID: 8474
	private int currentRankedSubTierQuest;

	// Token: 0x0400211B RID: 8475
	private int currentRankedSubTierPC;

	// Token: 0x0400211C RID: 8476
	private bool _rankedInfoUpdated;

	// Token: 0x0400211D RID: 8477
	internal CallLimiter updateRankedInfoCallLimit = new CallLimiter(2, 60f, 0.5f);

	// Token: 0x0400211E RID: 8478
	public const float maxGuardianThrowVelocity = 20f;

	// Token: 0x0400211F RID: 8479
	public const float maxRegularThrowVelocity = 3f;

	// Token: 0x04002120 RID: 8480
	private RaycastHit[] rayCastNonAllocColliders = new RaycastHit[5];

	// Token: 0x04002121 RID: 8481
	private bool inDuplicationZone;

	// Token: 0x04002122 RID: 8482
	private RigDuplicationZone duplicationZone;

	// Token: 0x04002123 RID: 8483
	private bool pendingCosmeticUpdate = true;

	// Token: 0x04002124 RID: 8484
	private string rawCosmeticString = "";

	// Token: 0x04002126 RID: 8486
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Right = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002127 RID: 8487
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Left = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002128 RID: 8488
	private int loudnessCheckFrame;

	// Token: 0x04002129 RID: 8489
	private float frameScale;

	// Token: 0x0400212A RID: 8490
	private const bool SHOW_SCREENS = false;

	// Token: 0x0400212B RID: 8491
	private static VRRig gLocalRig;

	// Token: 0x0200040B RID: 1035
	public enum PartyMemberStatus
	{
		// Token: 0x0400212E RID: 8494
		NeedsUpdate,
		// Token: 0x0400212F RID: 8495
		InLocalParty,
		// Token: 0x04002130 RID: 8496
		NotInLocalParty
	}

	// Token: 0x0200040C RID: 1036
	public enum WearablePackedStateSlots
	{
		// Token: 0x04002132 RID: 8498
		Hat,
		// Token: 0x04002133 RID: 8499
		LeftHand,
		// Token: 0x04002134 RID: 8500
		RightHand,
		// Token: 0x04002135 RID: 8501
		Face,
		// Token: 0x04002136 RID: 8502
		Pants1,
		// Token: 0x04002137 RID: 8503
		Pants2
	}

	// Token: 0x0200040D RID: 1037
	public struct VelocityTime
	{
		// Token: 0x06001922 RID: 6434 RVA: 0x00087C3E File Offset: 0x00085E3E
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		// Token: 0x04002138 RID: 8504
		public Vector3 vel;

		// Token: 0x04002139 RID: 8505
		public double time;
	}
}
