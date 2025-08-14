using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class GreyZoneManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000719 RID: 1817 RVA: 0x00028D72 File Offset: 0x00026F72
	public bool GreyZoneActive
	{
		get
		{
			return this.greyZoneActive;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x0600071A RID: 1818 RVA: 0x00028D7C File Offset: 0x00026F7C
	public bool GreyZoneAvailable
	{
		get
		{
			bool result = false;
			if (GorillaComputer.instance != null)
			{
				result = (GorillaComputer.instance.GetServerTime().DayOfYear >= this.greyZoneAvailableDayOfYear);
			}
			return result;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600071B RID: 1819 RVA: 0x00028DBB File Offset: 0x00026FBB
	public int GravityFactorSelection
	{
		get
		{
			return this.gravityFactorOptionSelection;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600071C RID: 1820 RVA: 0x00028DC3 File Offset: 0x00026FC3
	// (set) Token: 0x0600071D RID: 1821 RVA: 0x00028DCB File Offset: 0x00026FCB
	public bool TickRunning
	{
		get
		{
			return this._tickRunning;
		}
		set
		{
			this._tickRunning = value;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x0600071E RID: 1822 RVA: 0x00028DD4 File Offset: 0x00026FD4
	public bool HasAuthority
	{
		get
		{
			return !PhotonNetwork.InRoom || base.photonView.IsMine;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x0600071F RID: 1823 RVA: 0x00028DEA File Offset: 0x00026FEA
	public float SummoningProgress
	{
		get
		{
			return this.summoningProgress;
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00028DF2 File Offset: 0x00026FF2
	public void RegisterSummoner(GreyZoneSummoner summoner)
	{
		if (!this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Add(summoner);
		}
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00028E0E File Offset: 0x0002700E
	public void DeregisterSummoner(GreyZoneSummoner summoner)
	{
		if (this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Remove(summoner);
		}
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00028E2B File Offset: 0x0002702B
	public void RegisterMoon(MoonController moon)
	{
		this.moonController = moon;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00028E34 File Offset: 0x00027034
	public void UnregisterMoon(MoonController moon)
	{
		if (this.moonController == moon)
		{
			this.moonController = null;
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00028E4B File Offset: 0x0002704B
	public void ActivateGreyZoneAuthority()
	{
		this.greyZoneActive = true;
		this.photonConnectedDuringActivation = PhotonNetwork.InRoom;
		this.greyZoneActivationTime = (this.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
		this.ActivateGreyZoneLocal();
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00028E80 File Offset: 0x00027080
	private void ActivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 1);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.gravityOverrideSet = true;
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutMusic(2f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioIn(this.greyZoneAmbience, this.greyZoneAmbienceVolume, this.ambienceFadeTime));
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.GTPlay();
		}
		this.greyZoneParticles.gameObject.SetActive(true);
		this.summoningProgress = 1f;
		this.UpdateSummonerVisuals();
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].OnGreyZoneActivated();
		}
		if (this.OnGreyZoneActivated != null)
		{
			this.OnGreyZoneActivated();
		}
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00028F94 File Offset: 0x00027194
	public void DeactivateGreyZoneAuthority()
	{
		this.greyZoneActive = false;
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			this.summoningPlayerProgress[keyValuePair.Key] = 0f;
		}
		this.DeactivateGreyZoneLocal();
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00029004 File Offset: 0x00027204
	private void DeactivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(4f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioOut(this.greyZoneAmbience, this.ambienceFadeTime));
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x000290AC File Offset: 0x000272AC
	public void ForceStopGreyZone()
	{
		this.greyZoneActive = false;
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = false;
		if (this.moonController != null)
		{
			this.moonController.UpdateDistance(1f);
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(0f);
		}
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.volume = 0f;
			this.greyZoneAmbience.GTStop();
		}
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0002918C File Offset: 0x0002738C
	public void GravityOverrideFunction(GTPlayer player)
	{
		this.gravityReductionAmount = 0f;
		if (this.moonController != null)
		{
			this.gravityReductionAmount = Mathf.InverseLerp(1f - this.skyMonsterDistGravityRampBuffer, this.skyMonsterDistGravityRampBuffer, this.moonController.Distance);
		}
		float d = Mathf.Lerp(1f, this.gravityFactorOptions[this.gravityFactorOptionSelection], this.gravityReductionAmount);
		player.AddForce(Physics.gravity * d * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00029215 File Offset: 0x00027415
	private IEnumerator FadeAudioIn(AudioSource source, float maxVolume, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			source.GTPlay();
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, maxVolume, num);
				yield return null;
			}
			source.volume = maxVolume;
		}
		yield break;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00029232 File Offset: 0x00027432
	private IEnumerator FadeAudioOut(AudioSource source, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, 0f, num);
				yield return null;
			}
			source.volume = 0f;
			source.Stop();
		}
		yield break;
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00029248 File Offset: 0x00027448
	public void VRRigEnteredSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (!this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Add(rig.Creator.ActorNumber, new ValueTuple<VRRig, GreyZoneSummoner>(rig, summoner));
			this.summoningPlayerProgress.Add(rig.Creator.ActorNumber, 0f);
		}
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x000292A8 File Offset: 0x000274A8
	public void VRRigExitedSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Remove(rig.Creator.ActorNumber);
			this.summoningPlayerProgress.Remove(rig.Creator.ActorNumber);
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x000292FC File Offset: 0x000274FC
	private void UpdateSummonerVisuals()
	{
		bool greyZoneAvailable = this.GreyZoneAvailable;
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].UpdateProgressFeedback(greyZoneAvailable);
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00029338 File Offset: 0x00027538
	private void ValidateSummoningPlayers()
	{
		this.invalidSummoners.Clear();
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			VRRig item = keyValuePair.Value.Item1;
			GreyZoneSummoner item2 = keyValuePair.Value.Item2;
			if (item.Creator.ActorNumber != keyValuePair.Key || (item.head.rigTarget.position - item2.SummoningFocusPoint).sqrMagnitude > item2.SummonerMaxDistance * item2.SummonerMaxDistance)
			{
				this.invalidSummoners.Add(keyValuePair.Key);
			}
		}
		foreach (int key in this.invalidSummoners)
		{
			this.summoningPlayers.Remove(key);
			this.summoningPlayerProgress.Remove(key);
		}
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00029460 File Offset: 0x00027660
	private int DayNightOverrideFunction(int inputIndex)
	{
		int num = 0;
		int num2 = 8;
		int num3 = inputIndex - num;
		int num4 = num2 - inputIndex;
		if (num3 <= 0 || num4 <= 0)
		{
			return inputIndex;
		}
		if (num4 > num3)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0002948A File Offset: 0x0002768A
	private void Awake()
	{
		if (GreyZoneManager.Instance == null)
		{
			GreyZoneManager.Instance = this;
			this.greyZoneAmbienceVolume = this.greyZoneAmbience.volume;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000294C0 File Offset: 0x000276C0
	private void OnEnable()
	{
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.SetTimeIndexOverrideFunction(new Func<int, int>(this.DayNightOverrideFunction));
			}
		}
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x000294F8 File Offset: 0x000276F8
	private void OnDisable()
	{
		this.ForceStopGreyZone();
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.UnsetTimeIndexOverrideFunction();
			}
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0002952A File Offset: 0x0002772A
	private void Update()
	{
		if (this.HasAuthority)
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00029540 File Offset: 0x00027740
	private void AuthorityUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (this.greyZoneActive)
		{
			this.summoningProgress = 1f;
			double num;
			if (this.photonConnectedDuringActivation && PhotonNetwork.InRoom)
			{
				num = PhotonNetwork.Time;
			}
			else if (!this.photonConnectedDuringActivation && !PhotonNetwork.InRoom)
			{
				num = (double)Time.time;
			}
			else
			{
				num = -100.0;
			}
			if (num > this.greyZoneActivationTime + (double)this.greyZoneActiveDuration || num < this.greyZoneActivationTime - 10.0)
			{
				this.DeactivateGreyZoneAuthority();
				return;
			}
		}
		else if (this.GreyZoneAvailable)
		{
			this.roomPlayerList = PhotonNetwork.PlayerList;
			int num2 = 1;
			if (this.roomPlayerList != null && this.roomPlayerList.Length != 0)
			{
				num2 = Mathf.Max((this.roomPlayerList.Length + 1) / 2, 1);
			}
			float num3 = 0f;
			float num4 = 1f / this.summoningActivationTime;
			foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
			{
				VRRig item = keyValuePair.Value.Item1;
				GreyZoneSummoner item2 = keyValuePair.Value.Item2;
				float num5 = this.summoningPlayerProgress[keyValuePair.Key];
				Vector3 lhs = item2.SummoningFocusPoint - item.leftHand.rigTarget.position;
				Vector3 rhs = -item.leftHand.rigTarget.right;
				bool flag = Vector3.Dot(lhs, rhs) > 0f;
				Vector3 lhs2 = item2.SummoningFocusPoint - item.rightHand.rigTarget.position;
				Vector3 right = item.rightHand.rigTarget.right;
				bool flag2 = Vector3.Dot(lhs2, right) > 0f;
				if (flag && flag2)
				{
					num5 = Mathf.MoveTowards(num5, 1f, num4 * deltaTime);
				}
				else
				{
					num5 = Mathf.MoveTowards(num5, 0f, num4 * deltaTime);
				}
				num3 += num5;
				this.summoningPlayerProgress[keyValuePair.Key] = num5;
			}
			float num6 = 0.95f;
			this.summoningProgress = Mathf.Clamp01(num3 / num6 / (float)num2);
			this.UpdateSummonerVisuals();
			if (this.summoningProgress > 0.99f)
			{
				this.ActivateGreyZoneAuthority();
			}
		}
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00029798 File Offset: 0x00027998
	private void SharedUpdate()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.greyZoneActive)
		{
			Vector3 b = Vector3.ClampMagnitude(instance.InstantaneousVelocity * this.particlePredictiveSpawnVelocityFactor, this.particlePredictiveSpawnMaxDist);
			this.greyZoneParticles.transform.position = instance.HeadCenterPosition + Vector3.down * 0.5f + b;
		}
		else if (this.gravityOverrideSet && this.gravityReductionAmount < 0.01f)
		{
			instance.UnsetGravityOverride(this);
			this.gravityOverrideSet = false;
		}
		float num = this.greyZoneActive ? 0f : 1f;
		float smoothTime = this.greyZoneActive ? this.skyMonsterMovementEnterTime : this.skyMonsterMovementExitTime;
		if (this.moonController != null && this.moonController.Distance != num)
		{
			float num2 = Mathf.SmoothDamp(this.moonController.Distance, num, ref this.skyMonsterMovementVelocity, smoothTime);
			if ((double)Mathf.Abs(num2 - num) < 0.001)
			{
				num2 = num;
			}
			this.moonController.UpdateDistance(num2);
		}
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x000298AC File Offset: 0x00027AAC
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.greyZoneActive);
			stream.SendNext(this.greyZoneActivationTime);
			stream.SendNext(this.photonConnectedDuringActivation);
			stream.SendNext(this.gravityFactorOptionSelection);
			stream.SendNext(this.summoningProgress);
			return;
		}
		if (stream.IsReading && info.Sender.IsMasterClient)
		{
			bool flag = this.greyZoneActive;
			this.greyZoneActive = (bool)stream.ReceiveNext();
			this.greyZoneActivationTime = ((double)stream.ReceiveNext()).GetFinite();
			this.photonConnectedDuringActivation = (bool)stream.ReceiveNext();
			this.gravityFactorOptionSelection = (int)stream.ReceiveNext();
			this.summoningProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.UpdateSummonerVisuals();
			if (this.greyZoneActive && !flag)
			{
				this.ActivateGreyZoneLocal();
				return;
			}
			if (!this.greyZoneActive && flag)
			{
				this.DeactivateGreyZoneLocal();
			}
		}
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x000299CD File Offset: 0x00027BCD
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x000299CD File Offset: 0x00027BCD
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x040008AB RID: 2219
	[OnEnterPlay_SetNull]
	public static volatile GreyZoneManager Instance;

	// Token: 0x040008AC RID: 2220
	[SerializeField]
	private float greyZoneActiveDuration = 90f;

	// Token: 0x040008AD RID: 2221
	[SerializeField]
	private float[] gravityFactorOptions = new float[]
	{
		0.25f,
		0.5f,
		0.75f
	};

	// Token: 0x040008AE RID: 2222
	[SerializeField]
	private int gravityFactorOptionSelection = 1;

	// Token: 0x040008AF RID: 2223
	[SerializeField]
	private float summoningActivationTime = 3f;

	// Token: 0x040008B0 RID: 2224
	[SerializeField]
	private AudioSource greyZoneAmbience;

	// Token: 0x040008B1 RID: 2225
	[SerializeField]
	private float ambienceFadeTime = 4f;

	// Token: 0x040008B2 RID: 2226
	[SerializeField]
	private bool forceTimeOfDayToNight;

	// Token: 0x040008B3 RID: 2227
	[SerializeField]
	private float skyMonsterMovementEnterTime = 4.5f;

	// Token: 0x040008B4 RID: 2228
	[SerializeField]
	private float skyMonsterMovementExitTime = 3.2f;

	// Token: 0x040008B5 RID: 2229
	[SerializeField]
	private float skyMonsterDistGravityRampBuffer = 0.15f;

	// Token: 0x040008B6 RID: 2230
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityReductionAmount = 1f;

	// Token: 0x040008B7 RID: 2231
	[SerializeField]
	private ParticleSystem greyZoneParticles;

	// Token: 0x040008B8 RID: 2232
	[SerializeField]
	private float particlePredictiveSpawnMaxDist = 4f;

	// Token: 0x040008B9 RID: 2233
	[SerializeField]
	private float particlePredictiveSpawnVelocityFactor = 0.5f;

	// Token: 0x040008BA RID: 2234
	private bool photonConnectedDuringActivation;

	// Token: 0x040008BB RID: 2235
	private double greyZoneActivationTime;

	// Token: 0x040008BC RID: 2236
	private bool greyZoneActive;

	// Token: 0x040008BD RID: 2237
	private bool _tickRunning;

	// Token: 0x040008BE RID: 2238
	private float summoningProgress;

	// Token: 0x040008BF RID: 2239
	private List<GreyZoneSummoner> activeSummoners = new List<GreyZoneSummoner>();

	// Token: 0x040008C0 RID: 2240
	private Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>> summoningPlayers = new Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>>();

	// Token: 0x040008C1 RID: 2241
	private Dictionary<int, float> summoningPlayerProgress = new Dictionary<int, float>();

	// Token: 0x040008C2 RID: 2242
	private HashSet<int> invalidSummoners = new HashSet<int>();

	// Token: 0x040008C3 RID: 2243
	private Coroutine audioFadeCoroutine;

	// Token: 0x040008C4 RID: 2244
	private Player[] roomPlayerList;

	// Token: 0x040008C5 RID: 2245
	private ShaderHashId _GreyZoneActive = new ShaderHashId("_GreyZoneActive");

	// Token: 0x040008C6 RID: 2246
	private MoonController moonController;

	// Token: 0x040008C7 RID: 2247
	private float skyMonsterMovementVelocity;

	// Token: 0x040008C8 RID: 2248
	private bool gravityOverrideSet;

	// Token: 0x040008C9 RID: 2249
	private float greyZoneAmbienceVolume = 0.15f;

	// Token: 0x040008CA RID: 2250
	private int greyZoneAvailableDayOfYear = new DateTime(2024, 10, 25).DayOfYear;

	// Token: 0x040008CB RID: 2251
	public Action OnGreyZoneActivated;

	// Token: 0x040008CC RID: 2252
	public Action OnGreyZoneDeactivated;
}
