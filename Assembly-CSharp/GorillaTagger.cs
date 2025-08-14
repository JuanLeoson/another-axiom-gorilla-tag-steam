using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CjLib;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaNetworking;
using GorillaTag.Cosmetics;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using Photon.Pun;
using Photon.Voice.Unity;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x02000720 RID: 1824
public class GorillaTagger : MonoBehaviour, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x06002D88 RID: 11656 RVA: 0x000EFCBC File Offset: 0x000EDEBC
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x06002D89 RID: 11657 RVA: 0x000EFCC3 File Offset: 0x000EDEC3
	public NetworkView myVRRig
	{
		get
		{
			return this.offlineVRRig.netView;
		}
	}

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06002D8A RID: 11658 RVA: 0x000EFCD0 File Offset: 0x000EDED0
	internal VRRigSerializer rigSerializer
	{
		get
		{
			return this.offlineVRRig.rigSerializer;
		}
	}

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x06002D8B RID: 11659 RVA: 0x000EFCDD File Offset: 0x000EDEDD
	// (set) Token: 0x06002D8C RID: 11660 RVA: 0x000EFCE5 File Offset: 0x000EDEE5
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x06002D8D RID: 11661 RVA: 0x000EFCEE File Offset: 0x000EDEEE
	public float DefaultHandTapVolume
	{
		get
		{
			return this.cacheHandTapVolume;
		}
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x06002D8E RID: 11662 RVA: 0x000EFCF6 File Offset: 0x000EDEF6
	// (set) Token: 0x06002D8F RID: 11663 RVA: 0x000EFCFE File Offset: 0x000EDEFE
	public Recorder myRecorder { get; private set; }

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x06002D90 RID: 11664 RVA: 0x000EFD07 File Offset: 0x000EDF07
	public float sphereCastRadius
	{
		get
		{
			if (this.tagRadiusOverride == null)
			{
				return 0.03f;
			}
			return this.tagRadiusOverride.Value;
		}
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x000EFD27 File Offset: 0x000EDF27
	public void SetTagRadiusOverrideThisFrame(float radius)
	{
		this.tagRadiusOverride = new float?(radius);
		this.tagRadiusOverrideFrame = Time.frameCount;
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x000EFD40 File Offset: 0x000EDF40
	protected void Awake()
	{
		this.GuidedRefInitialize();
		this.RecoverMissingRefs();
		this.MirrorCameraCullingMask = new Watchable<int>(this.BaseMirrorCameraCullingMask);
		if (GorillaTagger._instance != null && GorillaTagger._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GorillaTagger._instance = this;
			GorillaTagger.hasInstance = true;
			Action action = GorillaTagger.onPlayerSpawnedRootCallback;
			if (action != null)
			{
				action();
			}
		}
		GRFirstTimeUserExperience grfirstTimeUserExperience = Object.FindObjectOfType<GRFirstTimeUserExperience>(true);
		GameObject gameObject = (grfirstTimeUserExperience != null) ? grfirstTimeUserExperience.gameObject : null;
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PlayerPrefs.GetString("didTutorial") != "done" && NetworkSystemConfig.AppVersion != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GTPlayer.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", true);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
			bool flag = true;
			if (gameObject != null && PlayerPrefs.GetString("spawnInWrongStump") == "flagged" && flag)
			{
				gameObject.SetActive(true);
				GRFirstTimeUserExperience grfirstTimeUserExperience2;
				if (gameObject.TryGetComponent<GRFirstTimeUserExperience>(out grfirstTimeUserExperience2) && grfirstTimeUserExperience2.spawnPoint != null)
				{
					GTPlayer.Instance.TeleportTo(grfirstTimeUserExperience2.spawnPoint.position, grfirstTimeUserExperience2.spawnPoint.rotation, false);
					GTPlayer.Instance.InitializeValues();
					PlayerPrefs.DeleteKey("spawnInWrongStump");
					PlayerPrefs.Save();
				}
			}
		}
		this.thirdPersonCamera.SetActive(Application.platform != RuntimePlatform.Android);
		this.inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		this.wasInOverlay = false;
		this.baseSlideControl = GTPlayer.Instance.slideControl;
		this.gorillaTagColliderLayerMask = UnityLayer.GorillaTagCollider.ToLayerMask();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.cacheHandTapVolume = this.handTapVolume;
		OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.Medium;
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x000EFFCE File Offset: 0x000EE1CE
	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x000EFFEC File Offset: 0x000EE1EC
	private void IsXRSubsystemActive()
	{
		this.loadedDeviceName = XRSettings.loadedDeviceName;
		List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
		SubsystemManager.GetInstances<XRDisplaySubsystem>(list);
		using (List<XRDisplaySubsystem>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.running)
				{
					this.xrSubsystemIsActive = true;
					return;
				}
			}
		}
		this.xrSubsystemIsActive = false;
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x000F0060 File Offset: 0x000EE260
	protected void Start()
	{
		this.IsXRSubsystemActive();
		if (this.loadedDeviceName == "OpenVR Display")
		{
			GTPlayer.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
			GTPlayer.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
			Quaternion rotation = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion rotation2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion lhs = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion lhs2 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GTPlayer.Instance.leftHandRotOffset = lhs * Quaternion.Inverse(rotation);
			GTPlayer.Instance.rightHandRotOffset = lhs2 * Quaternion.Inverse(rotation2);
		}
		this.bodyVector = new Vector3(0f, this.bodyCollider.height / 2f - this.bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			this.gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x000F01A8 File Offset: 0x000EE3A8
	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = (pCallback.m_bActive > 0);
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x000F01BC File Offset: 0x000EE3BC
	protected void LateUpdate()
	{
		GorillaTagger.<>c__DisplayClass116_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.isGameOverlayActive)
		{
			if (this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(false);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = true;
		}
		else
		{
			if (!this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(true);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = false;
		}
		if (this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android)
		{
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / XRDevice.refreshRate) > 0.0001f)
			{
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" refresh rate         :\t" + XRDevice.refreshRate.ToString());
				Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				GTPlayer.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(XRDevice.refreshRate * 0.083333336f), 10), 6);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
			}
		}
		else if (Application.platform != RuntimePlatform.Android && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			Object.Destroy(OVRManager.instance.gameObject);
		}
		if (!this.frameRateUpdated && Application.platform == RuntimePlatform.Android && OVRManager.instance.gameObject.activeSelf)
		{
			InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
			int num = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num2 = OVRManager.display.displayFrequenciesAvailable[num];
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			if (systemDisplayFrequency != 60f)
			{
				if (systemDisplayFrequency == 71f)
				{
					num2 = 72f;
				}
			}
			else
			{
				num2 = 60f;
			}
			while (num2 > 90f)
			{
				num--;
				if (num < 0)
				{
					break;
				}
				num2 = OVRManager.display.displayFrequenciesAvailable[num];
			}
			float num3 = 1f;
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / num2 * num3) > 0.0001f)
			{
				float num4 = Time.fixedDeltaTime - 1f / num2 * num3;
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log("!!!!Time.fixedDeltaTime - (1f / newRefreshRate) * " + num3.ToString() + ")" + num4.ToString());
				Debug.Log("Old Refresh rate: " + systemDisplayFrequency.ToString());
				Debug.Log("New Refresh rate: " + num2.ToString());
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" fixedDeltaTime after :\t" + (1f / num2).ToString());
				Time.fixedDeltaTime = 1f / num2 * num3;
				OVRPlugin.systemDisplayFrequency = num2;
				GTPlayer.Instance.velocityHistorySize = Mathf.FloorToInt(num2 * 0.083333336f);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(false);
				this.frameRateUpdated = true;
			}
		}
		if (!this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android && Mathf.Abs(Time.fixedDeltaTime - 0.0069444445f) > 0.0001f)
		{
			Debug.Log("updating delta time. was: " + Time.fixedDeltaTime.ToString() + ". now it's " + 0.0069444445f.ToString());
			Application.targetFrameRate = 144;
			Time.fixedDeltaTime = 0.0069444445f;
			GTPlayer.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(12f), 10);
			if (GTPlayer.Instance.velocityHistorySize > 9)
			{
				GTPlayer.Instance.velocityHistorySize--;
			}
			Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
			GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(144f);
			GTPlayer.Instance.InitializeValues();
		}
		this.leftRaycastSweep = this.leftHandTransform.position - this.lastLeftHandPositionForTag;
		this.leftHeadRaycastSweep = this.leftHandTransform.position - this.headCollider.transform.position;
		this.rightRaycastSweep = this.rightHandTransform.position - this.lastRightHandPositionForTag;
		this.rightHeadRaycastSweep = this.rightHandTransform.position - this.headCollider.transform.position;
		this.headRaycastSweep = this.headCollider.transform.position - this.lastHeadPositionForTag;
		this.bodyRaycastSweep = this.bodyCollider.transform.position - this.lastBodyPositionForTag;
		this.otherPlayer = null;
		this.touchedPlayer = null;
		CS$<>8__locals1.otherTouchedPlayer = null;
		if (this.tagRadiusOverrideFrame < Time.frameCount)
		{
			this.tagRadiusOverride = null;
		}
		float num5 = this.sphereCastRadius * GTPlayer.Instance.scale;
		CS$<>8__locals1.bodyHit = false;
		CS$<>8__locals1.leftHandHit = false;
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastLeftHandPositionForTag, num5, this.leftRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|116_0(false, true, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num5, this.leftHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|116_0(false, true, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.lastRightHandPositionForTag, num5, this.rightRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|116_0(false, false, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, num5, this.rightHeadRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|116_0(false, false, ref CS$<>8__locals1);
		this.nonAllocHits = Physics.SphereCastNonAlloc(this.headCollider.transform.position, this.headCollider.radius * this.headCollider.transform.localScale.x * GTPlayer.Instance.scale, this.headRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.headRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|116_0(true, false, ref CS$<>8__locals1);
		this.topVector = this.lastBodyPositionForTag + this.bodyVector;
		this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
		this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GTPlayer.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
		this.<LateUpdate>g__TryTaggingAllHits|116_0(true, false, ref CS$<>8__locals1);
		if (this.otherPlayer != null)
		{
			GameMode.ActiveGameMode.LocalTag(this.otherPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.bodyHit, CS$<>8__locals1.leftHandHit);
			GameMode.ReportTag(this.otherPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null && GorillaGameManager.instance != null)
		{
			CustomGameMode.TouchPlayer(CS$<>8__locals1.otherTouchedPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null)
		{
			this.HitWithKnockBack(CS$<>8__locals1.otherTouchedPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.leftHandHit);
		}
		GTPlayer instance = GTPlayer.Instance;
		bool flag = true;
		bool flag2;
		this.ProcessHandTapping(flag, ref this.lastLeftTap, ref this.lastLeftUpTap, out flag2, this.leftHandTouching, instance.leftHandMaterialTouchIndex, instance.leftHandSurfaceOverride, instance.leftHandHitInfo, instance.leftHandFollower, this.leftHandSlideSource, instance.leftHandCenterVelocityTracker);
		this.leftHandTouching = flag2;
		flag = false;
		this.ProcessHandTapping(flag, ref this.lastRightTap, ref this.lastLeftUpTap, out flag2, this.rightHandTouching, instance.rightHandMaterialTouchIndex, instance.rightHandSurfaceOverride, instance.rightHandHitInfo, instance.rightHandFollower, this.rightHandSlideSource, instance.rightHandCenterVelocityTracker);
		this.rightHandTouching = flag2;
		this.CheckEndStatusEffect();
		this.lastLeftHandPositionForTag = this.leftHandTransform.position;
		this.lastRightHandPositionForTag = this.rightHandTransform.position;
		this.lastBodyPositionForTag = this.bodyCollider.transform.position;
		this.lastHeadPositionForTag = this.headCollider.transform.position;
		if (GTPlayer.Instance.IsBodySliding && (double)GTPlayer.Instance.RigidbodyVelocity.magnitude >= 0.15)
		{
			if (!this.bodySlideSource.isPlaying)
			{
				this.bodySlideSource.Play();
			}
		}
		else
		{
			this.bodySlideSource.Stop();
		}
		if (GorillaComputer.instance == null || NetworkSystem.Instance.LocalRecorder == null)
		{
			return;
		}
		if (float.IsFinite(GorillaTagger.moderationMutedTime) && GorillaTagger.moderationMutedTime >= 0f)
		{
			GorillaTagger.moderationMutedTime -= Time.deltaTime;
		}
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (GorillaTagger.moderationMutedTime > 0f)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
						{
							this.myRecorder.TransmitEnabled = true;
							return;
						}
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = false;
					bool transmitEnabled2 = this.myRecorder.TransmitEnabled;
					this.myRecorder.TransmitEnabled = false;
					return;
				}
			}
			else
			{
				if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
				{
					this.myRecorder.TransmitEnabled = true;
				}
				if (!this.offlineVRRig.shouldSendSpeakingLoudness)
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					return;
				}
			}
		}
		else if (GorillaComputer.instance.voiceChatOn == "FALSE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (!this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = true;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "ALL CHAT")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
				}
			}
			else if (!this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = true;
				return;
			}
		}
		else
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = false;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
		}
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x000F10C8 File Offset: 0x000EF2C8
	private bool TryToTag(VRRig rig, Vector3 hitObjectPos, bool isBodyTag, out NetPlayer taggedPlayer, out NetPlayer touchedPlayer)
	{
		taggedPlayer = null;
		touchedPlayer = null;
		if (NetworkSystem.Instance.InRoom)
		{
			this.tempCreator = ((rig != null) ? rig.creator : null);
			if (this.tempCreator != null && NetworkSystem.Instance.LocalPlayer != this.tempCreator)
			{
				touchedPlayer = this.tempCreator;
				if (GorillaGameManager.instance != null && Time.time > this.taggedTime + this.tagCooldown && GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.tempCreator) && (this.headCollider.transform.position - hitObjectPos).sqrMagnitude < this.maxTagDistance * this.maxTagDistance * GTPlayer.Instance.scale)
				{
					if (!isBodyTag)
					{
						this.StartVibration((this.leftHandTransform.position - hitObjectPos).magnitude < (this.rightHandTransform.position - hitObjectPos).magnitude, this.tagHapticStrength, this.tagHapticDuration);
					}
					else
					{
						this.StartVibration(true, this.tagHapticStrength, this.tagHapticDuration);
						this.StartVibration(false, this.tagHapticStrength, this.tagHapticDuration);
					}
					taggedPlayer = this.tempCreator;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002D99 RID: 11673 RVA: 0x000F122C File Offset: 0x000EF42C
	private bool TryToTag(RaycastHit hit, bool isBodyTag, out NetPlayer taggedPlayer, out NetPlayer touchedNetPlayer)
	{
		VRRig vrrig = hit.collider.GetComponentInParent<VRRig>();
		if (vrrig == null)
		{
			PropHuntTaggableProp componentInParent = hit.collider.GetComponentInParent<PropHuntTaggableProp>();
			if (!(componentInParent != null))
			{
				taggedPlayer = null;
				touchedNetPlayer = null;
				return false;
			}
			vrrig = componentInParent.ownerRig;
		}
		else if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
		{
			taggedPlayer = null;
			touchedNetPlayer = null;
			return false;
		}
		return this.TryToTag(vrrig, hit.collider.transform.position, isBodyTag, out taggedPlayer, out touchedNetPlayer);
	}

	// Token: 0x06002D9A RID: 11674 RVA: 0x000F12B8 File Offset: 0x000EF4B8
	private void HitWithKnockBack(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool leftHand)
	{
		Vector3 averageVelocity = (leftHand ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand;
		Vector3 vector = leftHand ? (-vrmap.rigTarget.right) : vrmap.rigTarget.right;
		RigContainer rigContainer2;
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer2) && rigContainer2.Rig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, out cosmeticEffect))
		{
			RoomSystem.HitPlayer(taggedPlayer, vector.normalized, averageVelocity.magnitude);
		}
	}

	// Token: 0x06002D9B RID: 11675 RVA: 0x000F1371 File Offset: 0x000EF571
	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x000F1383 File Offset: 0x000EF583
	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0U;
		UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		}
		else
		{
			device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, this.hapticWaitSeconds);
			yield return new WaitForSeconds(this.hapticWaitSeconds * 0.9f);
		}
		yield break;
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x000F13A8 File Offset: 0x000EF5A8
	public void PlayHapticClip(bool forLeftController, AudioClip clip, float strength)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
			}
			this.leftHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
			return;
		}
		if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
		}
		this.rightHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x000F140B File Offset: 0x000EF60B
	public void StopHapticClip(bool forLeftController)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
				this.leftHapticsRoutine = null;
				return;
			}
		}
		else if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
			this.rightHapticsRoutine = null;
		}
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x000F1447 File Offset: 0x000EF647
	private IEnumerator AudioClipHapticPulses(bool forLeftController, AudioClip clip, float strength)
	{
		uint channel = 0U;
		int bufferSize = 8192;
		int sampleWindowSize = 256;
		float[] audioData;
		UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			float[] array;
			if ((array = this.leftHapticsBuffer) == null)
			{
				array = (this.leftHapticsBuffer = new float[bufferSize]);
			}
			audioData = array;
			device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		}
		else
		{
			float[] array2;
			if ((array2 = this.rightHapticsBuffer) == null)
			{
				array2 = (this.rightHapticsBuffer = new float[bufferSize]);
			}
			audioData = array2;
			device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		int sampleOffset = -bufferSize;
		float startTime = Time.time;
		float length = clip.length;
		float endTime = Time.time + length;
		float sampleRate = (float)clip.samples;
		while (Time.time <= endTime)
		{
			float num = (Time.time - startTime) / length;
			int num2 = (int)(sampleRate * num);
			if (Mathf.Max(num2 + sampleWindowSize - 1, audioData.Length - 1) >= sampleOffset + bufferSize)
			{
				clip.GetData(audioData, num2);
				sampleOffset = num2;
			}
			float num3 = 0f;
			int num4 = Mathf.Min(clip.samples - num2, sampleWindowSize);
			for (int i = 0; i < num4; i++)
			{
				float num5 = audioData[num2 - sampleOffset + i];
				num3 += num5 * num5;
			}
			float amplitude = Mathf.Clamp01(((num4 > 0) ? Mathf.Sqrt(num3 / (float)num4) : 0f) * strength);
			device.SendHapticImpulse(channel, amplitude, Time.fixedDeltaTime);
			yield return null;
		}
		if (forLeftController)
		{
			this.leftHapticsRoutine = null;
		}
		else
		{
			this.rightHapticsRoutine = null;
		}
		yield break;
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x000F146C File Offset: 0x000EF66C
	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		UnityEngine.XR.InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x000F1494 File Offset: 0x000EF694
	public void UpdateColor(float red, float green, float blue)
	{
		this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue);
		if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom)
		{
			this.offlineVRRig.mainSkin.sharedMaterial = this.offlineVRRig.materialsToChangeTo[0];
		}
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x000F14E8 File Offset: 0x000EF6E8
	protected void OnTriggerEnter(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxTriggered();
		}
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x000F1508 File Offset: 0x000EF708
	protected void OnTriggerExit(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxExited();
		}
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x000F1528 File Offset: 0x000EF728
	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			this.MirrorCameraCullingMask.value |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
		this.MirrorCameraCullingMask.value &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x000F15A9 File Offset: 0x000EF7A9
	public void ApplyStatusEffect(GorillaTagger.StatusEffect newStatus, float duration)
	{
		this.EndStatusEffect(this.currentStatus);
		this.currentStatus = newStatus;
		this.statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case GorillaTagger.StatusEffect.None:
		case GorillaTagger.StatusEffect.Slowed:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x000F15E9 File Offset: 0x000EF7E9
	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x000F1604 File Offset: 0x000EF804
	private void EndStatusEffect(GorillaTagger.StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case GorillaTagger.StatusEffect.None:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = false;
			this.currentStatus = GorillaTagger.StatusEffect.None;
			return;
		case GorillaTagger.StatusEffect.Slowed:
			this.currentStatus = GorillaTagger.StatusEffect.None;
			break;
		default:
			return;
		}
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x000F1633 File Offset: 0x000EF833
	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x000F1657 File Offset: 0x000EF857
	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000F1684 File Offset: 0x000EF884
	private void ProcessHandTapping(in bool isLeftHand, ref float lastTapTime, ref float lastTapUpTime, out bool isHandTouching, in bool wasHandTouching, in int handMatIndex, in GorillaSurfaceOverride surfaceOverride, in RaycastHit handHitInfo, in Transform handFollower, in AudioSource handSlideSource, in GorillaVelocityTracker handVelocityTracker)
	{
		isHandTouching = GTPlayer.Instance.IsHandTouching(isLeftHand);
		if (GTPlayer.Instance.inOverlay)
		{
			handSlideSource.GTStop();
			return;
		}
		if (GTPlayer.Instance.IsHandSliding(isLeftHand))
		{
			this.StartVibration(isLeftHand, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!handSlideSource.isPlaying)
			{
				handSlideSource.GTPlay();
			}
			return;
		}
		handSlideSource.GTStop();
		bool flag = !wasHandTouching & isHandTouching;
		bool flag2 = wasHandTouching && !isHandTouching;
		if (!flag2 && !flag)
		{
			return;
		}
		Tappable tappable = null;
		bool flag3 = surfaceOverride != null && surfaceOverride.TryGetComponent<Tappable>(out tappable);
		HandEffectContext handEffect = this.offlineVRRig.GetHandEffect(isLeftHand);
		if ((!flag3 || !tappable.overrideTapCooldown) && (!handEffect.SeparateUpTapCooldown || !flag2 || Time.time <= lastTapUpTime + this.tapCoolDown) && Time.time <= lastTapTime + this.tapCoolDown)
		{
			return;
		}
		float sqrMagnitude = (handVelocityTracker.GetAverageVelocity(true, 0.03f, false) / GTPlayer.Instance.scale).sqrMagnitude;
		float sqrMagnitude2 = handVelocityTracker.GetAverageVelocity(false, 0.03f, false).sqrMagnitude;
		this.handTapSpeed = Mathf.Sqrt(Mathf.Max(sqrMagnitude, sqrMagnitude2));
		if (handEffect.SeparateUpTapCooldown && flag2)
		{
			lastTapUpTime = Time.time;
		}
		else
		{
			lastTapTime = Time.time;
		}
		RaycastHit raycastHit = handHitInfo;
		this.dirFromHitToHand = Vector3.Normalize(raycastHit.point - handFollower.position);
		GorillaAmbushManager gorillaAmbushManager = GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(NetworkSystem.Instance.LocalPlayer))
		{
			this.handTapVolume = Mathf.Clamp(this.handTapSpeed, 0f, gorillaAmbushManager.crawlingSpeedForMaxVolume);
		}
		else
		{
			this.handTapVolume = this.cacheHandTapVolume;
		}
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null && gorillaFreezeTagManager.IsFrozen(NetworkSystem.Instance.LocalPlayer))
		{
			this.audioClipIndex = gorillaFreezeTagManager.GetFrozenHandTapAudioIndex();
		}
		else if (surfaceOverride != null)
		{
			this.audioClipIndex = surfaceOverride.overrideIndex;
		}
		else
		{
			this.audioClipIndex = handMatIndex;
		}
		if (surfaceOverride != null)
		{
			if (surfaceOverride.sendOnTapEvent)
			{
				BuilderPieceTappable builderPieceTappable;
				if (flag3)
				{
					tappable.OnTap(this.handTapVolume);
				}
				else if (surfaceOverride.TryGetComponent<BuilderPieceTappable>(out builderPieceTappable))
				{
					builderPieceTappable.OnTapLocal(this.handTapVolume);
				}
			}
			PlayerGameEvents.TapObject(surfaceOverride.name);
		}
		if (GameMode.ActiveGameMode != null)
		{
			GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
			NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
			Tappable hitTappable = tappable;
			bool leftHand = isLeftHand;
			Vector3 averageVelocity = handVelocityTracker.GetAverageVelocity(true, 0.03f, false);
			raycastHit = handHitInfo;
			activeGameMode.HandleHandTap(localPlayer, hitTappable, leftHand, averageVelocity, raycastHit.normal);
		}
		this.StartVibration(isLeftHand, this.tapHapticStrength, this.tapHapticDuration);
		this.offlineVRRig.SetHandEffectData(handEffect, this.audioClipIndex, flag, isLeftHand, this.handTapVolume, this.handTapSpeed, this.dirFromHitToHand);
		FXSystem.PlayFX(handEffect);
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority())
		{
			CrittersRigActorSetup crittersRigActorSetup = CrittersManager.instance.rigSetupByRig[this.offlineVRRig];
			if (crittersRigActorSetup.IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)crittersRigActorSetup.rigActors[isLeftHand ? 0 : 2].actorSet;
				if (crittersLoudNoise.IsNotNull())
				{
					crittersLoudNoise.PlayHandTapLocal(isLeftHand);
				}
			}
		}
		if (NetworkSystem.Instance.InRoom && this.myVRRig.IsNotNull() && this.myVRRig != null)
		{
			this.myVRRig.GetView.RPC("OnHandTapRPC", RpcTarget.Others, new object[]
			{
				this.audioClipIndex,
				flag,
				isLeftHand,
				this.handTapSpeed,
				Utils.PackVector3ToLong(this.dirFromHitToHand)
			});
		}
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000F1A84 File Offset: 0x000EFC84
	public void DebugDrawTagCasts(Color color)
	{
		float num = this.sphereCastRadius * GTPlayer.Instance.scale;
		this.DrawSphereCast(this.lastLeftHandPositionForTag, this.leftRaycastSweep.normalized, num, Mathf.Max(this.leftRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.leftHeadRaycastSweep.normalized, num, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.lastRightHandPositionForTag, this.rightRaycastSweep.normalized, num, Mathf.Max(this.rightRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.rightHeadRaycastSweep.normalized, num, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num), color);
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x000F1B5F File Offset: 0x000EFD5F
	private void DrawSphereCast(Vector3 start, Vector3 dir, float radius, float dist, Color color)
	{
		DebugUtil.DrawCapsule(start, start + dir * dist, radius, 16, 16, color, true, DebugUtil.Style.Wireframe);
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x000F1B7E File Offset: 0x000EFD7E
	private void RecoverMissingRefs()
	{
		if (!this.offlineVRRig)
		{
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.leftHandSlideSource, "leftHandSlideSource", "./**/Left Arm IK/SlideAudio");
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.rightHandSlideSource, "rightHandSlideSource", "./**/Right Arm IK/SlideAudio");
		}
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x000F1BBC File Offset: 0x000EFDBC
	private void RecoverMissingRefs_Asdf<T>(ref T objRef, string objFieldName, string recoveryPath) where T : Object
	{
		if (objRef)
		{
			return;
		}
		Transform transform;
		if (!this.offlineVRRig.transform.TryFindByPath(recoveryPath, out transform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"`",
				objFieldName,
				"` reference missing and could not find by path: \"",
				recoveryPath,
				"\""
			}), this);
		}
		objRef = transform.GetComponentInChildren<T>();
		if (!objRef)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"`",
				objFieldName,
				"` reference is missing. Found transform with recover path, but did not find the component. Recover path: \"",
				recoveryPath,
				"\""
			}), this);
		}
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x000F1C72 File Offset: 0x000EFE72
	public void GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTagger>(this, "offlineVRRig", ref this.offlineVRRig_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTagger>(this);
	}

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x06002DB0 RID: 11696 RVA: 0x000F1C8B File Offset: 0x000EFE8B
	// (set) Token: 0x06002DB1 RID: 11697 RVA: 0x000F1C93 File Offset: 0x000EFE93
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000F1C9C File Offset: 0x000EFE9C
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		if (this.offlineVRRig_gRef.fieldId == target.fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = (target.targetMono.GuidedRefTargetObject as VRRig);
			return this.offlineVRRig != null;
		}
		return false;
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x000023F5 File Offset: 0x000005F5
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x000023F5 File Offset: 0x000005F5
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x0005860D File Offset: 0x0005680D
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x0001745D File Offset: 0x0001565D
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x000F1DB4 File Offset: 0x000EFFB4
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHits|116_0(bool isBodyTag, bool isLeftHand, ref GorillaTagger.<>c__DisplayClass116_0 A_3)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			if (this.nonAllocRaycastHits[i].collider.gameObject.activeSelf)
			{
				if (this.TryToTag(this.nonAllocRaycastHits[i], isBodyTag, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_3.bodyHit = isBodyTag;
					A_3.leftHandHit = isLeftHand;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_3.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x040038EB RID: 14571
	[OnEnterPlay_SetNull]
	private static GorillaTagger _instance;

	// Token: 0x040038EC RID: 14572
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040038ED RID: 14573
	public static float moderationMutedTime = -1f;

	// Token: 0x040038EE RID: 14574
	public bool inCosmeticsRoom;

	// Token: 0x040038EF RID: 14575
	public SphereCollider headCollider;

	// Token: 0x040038F0 RID: 14576
	public CapsuleCollider bodyCollider;

	// Token: 0x040038F1 RID: 14577
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x040038F2 RID: 14578
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x040038F3 RID: 14579
	private Vector3 lastBodyPositionForTag;

	// Token: 0x040038F4 RID: 14580
	private Vector3 lastHeadPositionForTag;

	// Token: 0x040038F5 RID: 14581
	public Transform rightHandTransform;

	// Token: 0x040038F6 RID: 14582
	public Transform leftHandTransform;

	// Token: 0x040038F7 RID: 14583
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x040038F8 RID: 14584
	public float handTapVolume = 0.1f;

	// Token: 0x040038F9 RID: 14585
	public float handTapSpeed;

	// Token: 0x040038FA RID: 14586
	public float tapCoolDown = 0.15f;

	// Token: 0x040038FB RID: 14587
	public float lastLeftTap;

	// Token: 0x040038FC RID: 14588
	public float lastLeftUpTap;

	// Token: 0x040038FD RID: 14589
	public float lastRightTap;

	// Token: 0x040038FE RID: 14590
	public float lastRightUpTap;

	// Token: 0x040038FF RID: 14591
	public float tapHapticDuration = 0.05f;

	// Token: 0x04003900 RID: 14592
	public float tapHapticStrength = 0.5f;

	// Token: 0x04003901 RID: 14593
	public float tagHapticDuration = 0.15f;

	// Token: 0x04003902 RID: 14594
	public float tagHapticStrength = 1f;

	// Token: 0x04003903 RID: 14595
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04003904 RID: 14596
	public float taggedHapticStrength = 1f;

	// Token: 0x04003905 RID: 14597
	private bool leftHandTouching;

	// Token: 0x04003906 RID: 14598
	private bool rightHandTouching;

	// Token: 0x04003907 RID: 14599
	public float taggedTime;

	// Token: 0x04003908 RID: 14600
	public float tagCooldown;

	// Token: 0x04003909 RID: 14601
	public float slowCooldown = 3f;

	// Token: 0x0400390A RID: 14602
	public float maxTagDistance = 1.2f;

	// Token: 0x0400390B RID: 14603
	public VRRig offlineVRRig;

	// Token: 0x0400390C RID: 14604
	[FormerlySerializedAs("offlineVRRig_guidedRef")]
	public GuidedRefReceiverFieldInfo offlineVRRig_gRef = new GuidedRefReceiverFieldInfo(false);

	// Token: 0x0400390D RID: 14605
	public GameObject thirdPersonCamera;

	// Token: 0x0400390E RID: 14606
	public GameObject mainCamera;

	// Token: 0x0400390F RID: 14607
	public bool testTutorial;

	// Token: 0x04003910 RID: 14608
	public bool disableTutorial;

	// Token: 0x04003911 RID: 14609
	public bool frameRateUpdated;

	// Token: 0x04003912 RID: 14610
	public GameObject leftHandTriggerCollider;

	// Token: 0x04003913 RID: 14611
	public GameObject rightHandTriggerCollider;

	// Token: 0x04003914 RID: 14612
	public AudioSource leftHandSlideSource;

	// Token: 0x04003915 RID: 14613
	public AudioSource rightHandSlideSource;

	// Token: 0x04003916 RID: 14614
	public AudioSource bodySlideSource;

	// Token: 0x04003917 RID: 14615
	public bool overrideNotInFocus;

	// Token: 0x04003919 RID: 14617
	private Vector3 leftRaycastSweep;

	// Token: 0x0400391A RID: 14618
	private Vector3 leftHeadRaycastSweep;

	// Token: 0x0400391B RID: 14619
	private Vector3 rightRaycastSweep;

	// Token: 0x0400391C RID: 14620
	private Vector3 rightHeadRaycastSweep;

	// Token: 0x0400391D RID: 14621
	private Vector3 headRaycastSweep;

	// Token: 0x0400391E RID: 14622
	private Vector3 bodyRaycastSweep;

	// Token: 0x0400391F RID: 14623
	private UnityEngine.XR.InputDevice rightDevice;

	// Token: 0x04003920 RID: 14624
	private UnityEngine.XR.InputDevice leftDevice;

	// Token: 0x04003921 RID: 14625
	private bool primaryButtonPressRight;

	// Token: 0x04003922 RID: 14626
	private bool secondaryButtonPressRight;

	// Token: 0x04003923 RID: 14627
	private bool primaryButtonPressLeft;

	// Token: 0x04003924 RID: 14628
	private bool secondaryButtonPressLeft;

	// Token: 0x04003925 RID: 14629
	private RaycastHit hitInfo;

	// Token: 0x04003926 RID: 14630
	public NetPlayer otherPlayer;

	// Token: 0x04003927 RID: 14631
	private NetPlayer tryPlayer;

	// Token: 0x04003928 RID: 14632
	private NetPlayer touchedPlayer;

	// Token: 0x04003929 RID: 14633
	private Vector3 topVector;

	// Token: 0x0400392A RID: 14634
	private Vector3 bottomVector;

	// Token: 0x0400392B RID: 14635
	private Vector3 bodyVector;

	// Token: 0x0400392C RID: 14636
	private Vector3 dirFromHitToHand;

	// Token: 0x0400392D RID: 14637
	private int audioClipIndex;

	// Token: 0x0400392E RID: 14638
	private UnityEngine.XR.InputDevice inputDevice;

	// Token: 0x0400392F RID: 14639
	private bool wasInOverlay;

	// Token: 0x04003930 RID: 14640
	private PhotonView tempView;

	// Token: 0x04003931 RID: 14641
	private NetPlayer tempCreator;

	// Token: 0x04003932 RID: 14642
	private float cacheHandTapVolume;

	// Token: 0x04003933 RID: 14643
	public GorillaTagger.StatusEffect currentStatus;

	// Token: 0x04003934 RID: 14644
	public float statusStartTime;

	// Token: 0x04003935 RID: 14645
	public float statusEndTime;

	// Token: 0x04003936 RID: 14646
	private float refreshRate;

	// Token: 0x04003937 RID: 14647
	private float baseSlideControl;

	// Token: 0x04003938 RID: 14648
	private int gorillaTagColliderLayerMask;

	// Token: 0x04003939 RID: 14649
	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	// Token: 0x0400393A RID: 14650
	private int nonAllocHits;

	// Token: 0x0400393C RID: 14652
	private bool xrSubsystemIsActive;

	// Token: 0x0400393D RID: 14653
	public string loadedDeviceName = "";

	// Token: 0x0400393E RID: 14654
	[SerializeField]
	private LayerMask BaseMirrorCameraCullingMask;

	// Token: 0x0400393F RID: 14655
	public Watchable<int> MirrorCameraCullingMask;

	// Token: 0x04003940 RID: 14656
	private float[] leftHapticsBuffer;

	// Token: 0x04003941 RID: 14657
	private float[] rightHapticsBuffer;

	// Token: 0x04003942 RID: 14658
	private Coroutine leftHapticsRoutine;

	// Token: 0x04003943 RID: 14659
	private Coroutine rightHapticsRoutine;

	// Token: 0x04003944 RID: 14660
	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	// Token: 0x04003945 RID: 14661
	private bool isGameOverlayActive;

	// Token: 0x04003946 RID: 14662
	private float? tagRadiusOverride;

	// Token: 0x04003947 RID: 14663
	private int tagRadiusOverrideFrame = -1;

	// Token: 0x04003948 RID: 14664
	private static Action onPlayerSpawnedRootCallback;

	// Token: 0x02000721 RID: 1825
	public enum StatusEffect
	{
		// Token: 0x0400394B RID: 14667
		None,
		// Token: 0x0400394C RID: 14668
		Frozen,
		// Token: 0x0400394D RID: 14669
		Slowed,
		// Token: 0x0400394E RID: 14670
		Dead,
		// Token: 0x0400394F RID: 14671
		Infected,
		// Token: 0x04003950 RID: 14672
		It
	}
}
