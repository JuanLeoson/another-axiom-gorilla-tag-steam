using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200063C RID: 1596
public class GRFirstTimeUserExperience : MonoBehaviour
{
	// Token: 0x06002755 RID: 10069 RVA: 0x000CBFC5 File Offset: 0x000CA1C5
	[ContextMenu("Set Player Pref")]
	private void RemovePlayerPref()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000D40C8 File Offset: 0x000D22C8
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.flickerSphere.SetActive(false);
		this.logoQuad.SetActive(false);
		this.flickerSphereOrigParent = this.flickerSphere.transform.parent;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		this.playerLight = GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true);
		this.playerLight.gameObject.SetActive(true);
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Waiting);
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000D414C File Offset: 0x000D234C
	public void ChangeState(GRFirstTimeUserExperience.TransitionState state)
	{
		this.transitionState = state;
		switch (state)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
			this.transitionState = GRFirstTimeUserExperience.TransitionState.Flicker;
			this.flickerSphere.transform.SetParent(GTPlayer.Instance.headCollider.transform, false);
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(false);
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Logo:
			this.stateStartTime = Time.time;
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(true);
			return;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
			ZoneManagement.SetActiveZone(this.teleportZone);
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.joinRoomTrigger, JoinType.Solo, null);
			GTPlayer.Instance.TeleportTo(this.teleportLocation.position, this.teleportLocation.rotation, false);
			GTPlayer.Instance.InitializeValues();
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Exit:
			this.flickerSphere.transform.SetParent(this.flickerSphereOrigParent, false);
			this.flickerSphere.SetActive(false);
			this.logoQuad.SetActive(false);
			this.rootObject.SetActive(false);
			GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true).gameObject.SetActive(false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000D42CD File Offset: 0x000D24CD
	private void OnZoneLoadComplete()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Teleport);
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000D42FC File Offset: 0x000D24FC
	public void InterruptWaitingTimer()
	{
		this.stateStartTime = -1f;
		for (int i = 0; i < this.delayObjects.Count; i++)
		{
			this.delayObjects[i].enabledTime = this.stateStartTime;
		}
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000D4344 File Offset: 0x000D2544
	private void Update()
	{
		switch (this.transitionState)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			if (PrivateUIRoom.GetInOverlay())
			{
				if (this.stateStartTime >= 0f)
				{
					this.InterruptWaitingTimer();
				}
			}
			else if (this.stateStartTime < 0f)
			{
				this.stateStartTime = Time.time;
			}
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.transitionDelay)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
		{
			float num = Time.time - this.stateStartTime;
			if (this.stateStartTime >= 0f && num >= this.flickerDuration)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Logo);
				return;
			}
			bool flag = this.flickerTimeline.Evaluate(num / this.flickerDuration) < 0f;
			this.flickerSphere.SetActive(flag);
			if (flag && !this.flickerLightWasOff)
			{
				if (this.audioSource != null && this.flickerAudioCount < this.flickerAudio.Count && this.flickerAudio[this.flickerAudioCount] != null)
				{
					this.audioSource.PlayOneShot(this.flickerAudio[this.flickerAudioCount]);
				}
				this.flickerAudioCount++;
			}
			this.flickerLightWasOff = flag;
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Logo:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.logoDisplayTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.ZoneLoad);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
			break;
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.teleportSettleTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Exit);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0400326A RID: 12906
	public Transform spawnPoint;

	// Token: 0x0400326B RID: 12907
	public GameObject rootObject;

	// Token: 0x0400326C RID: 12908
	public GameObject flickerSphere;

	// Token: 0x0400326D RID: 12909
	public GameObject logoQuad;

	// Token: 0x0400326E RID: 12910
	public AnimationCurve flickerTimeline;

	// Token: 0x0400326F RID: 12911
	public float flickerDuration = 3f;

	// Token: 0x04003270 RID: 12912
	public GTZone teleportZone = GTZone.none;

	// Token: 0x04003271 RID: 12913
	public Transform teleportLocation;

	// Token: 0x04003272 RID: 12914
	public float transitionDelay = 60f;

	// Token: 0x04003273 RID: 12915
	public float logoDisplayTime = 4f;

	// Token: 0x04003274 RID: 12916
	public float teleportSettleTime = 1f;

	// Token: 0x04003275 RID: 12917
	public GorillaNetworkJoinTrigger joinRoomTrigger;

	// Token: 0x04003276 RID: 12918
	public List<AudioClip> flickerAudio = new List<AudioClip>();

	// Token: 0x04003277 RID: 12919
	public List<DisableGameObjectDelayed> delayObjects;

	// Token: 0x04003278 RID: 12920
	private Transform flickerSphereOrigParent;

	// Token: 0x04003279 RID: 12921
	private float stateStartTime = -1f;

	// Token: 0x0400327A RID: 12922
	private bool flickerLightWasOff;

	// Token: 0x0400327B RID: 12923
	private int flickerAudioCount;

	// Token: 0x0400327C RID: 12924
	private AudioSource audioSource;

	// Token: 0x0400327D RID: 12925
	private GRFirstTimeUserExperience.TransitionState transitionState;

	// Token: 0x0400327E RID: 12926
	public GameLight playerLight;

	// Token: 0x0200063D RID: 1597
	public enum TransitionState
	{
		// Token: 0x04003280 RID: 12928
		Waiting,
		// Token: 0x04003281 RID: 12929
		Flicker,
		// Token: 0x04003282 RID: 12930
		Logo,
		// Token: 0x04003283 RID: 12931
		ZoneLoad,
		// Token: 0x04003284 RID: 12932
		Teleport,
		// Token: 0x04003285 RID: 12933
		Exit
	}
}
