using System;
using System.Collections.Generic;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006CB RID: 1739
public class GorillaGuardianZoneManager : MonoBehaviourPunCallbacks, IPunObservable, IGorillaSliceableSimple
{
	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x06002B46 RID: 11078 RVA: 0x000E4F12 File Offset: 0x000E3112
	public NetPlayer CurrentGuardian
	{
		get
		{
			return this.guardianPlayer;
		}
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x000E4F1C File Offset: 0x000E311C
	public void Awake()
	{
		GorillaGuardianZoneManager.zoneManagers.Add(this);
		this.idol.gameObject.SetActive(false);
		foreach (Transform transform in this.idolPositions)
		{
			transform.gameObject.SetActive(false);
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && PhotonNetwork.IsMasterClient)
		{
			this.StartPlaying();
		}
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000E4FB4 File Offset: 0x000E31B4
	private void Start()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x000E4FDC File Offset: 0x000E31DC
	public void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		GorillaGuardianZoneManager.zoneManagers.Remove(this);
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x000E5010 File Offset: 0x000E3210
	public override void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x000E501F File Offset: 0x000E321F
	public override void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002B4C RID: 11084 RVA: 0x000E5030 File Offset: 0x000E3230
	public void SliceUpdate()
	{
		float idolActivationDisplay = this._idolActivationDisplay;
		float num = 0f;
		if (this._currentActivationTime < 0f)
		{
			this._idolActivationDisplay = 0f;
			this._progressing = false;
		}
		else
		{
			num = Mathf.Min(Time.time - this._lastTappedTime, this.activationTimePerTap);
			this._progressing = (num < this.activationTimePerTap);
			this._idolActivationDisplay = (this._currentActivationTime + num) / this.requiredActivationTime;
		}
		if (idolActivationDisplay != this._idolActivationDisplay)
		{
			this.idol.UpdateActivationProgress(this._currentActivationTime + num, this._progressing);
		}
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x000E50C7 File Offset: 0x000E32C7
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.StopPlaying();
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x000E50D5 File Offset: 0x000E32D5
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (this.guardianPlayer == null || this.guardianPlayer.GetPlayerRef() == otherPlayer)
		{
			this.SetGuardian(null);
		}
		NetPlayer previousGuardian = this._previousGuardian;
		if (((previousGuardian != null) ? previousGuardian.GetPlayerRef() : null) == otherPlayer)
		{
			this._previousGuardian = null;
		}
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000E5110 File Offset: 0x000E3310
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		if (flag != this._zoneIsActive || !this._zoneStateChanged)
		{
			GTDev.Log<string>(string.Format("{0} {1} Active: {2}->{3}", new object[]
			{
				this,
				this.zone,
				this._zoneIsActive,
				flag
			}), this, null);
			this._zoneIsActive = flag;
			this.idol.OnZoneActiveStateChanged(this._zoneIsActive);
			this._zoneStateChanged = true;
		}
		if (!this._zoneIsActive)
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer) && this.guardianPlayer != null && this.guardianPlayer != NetworkSystem.Instance.LocalPlayer)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000E51F4 File Offset: 0x000E33F4
	public void StartPlaying()
	{
		if (!this.IsZoneValid())
		{
			return;
		}
		this._currentActivationTime = -1f;
		if (this.guardianPlayer != null && !this.guardianPlayer.InRoom())
		{
			this.SetGuardian(null);
			this._previousGuardian = null;
		}
		this.idol.gameObject.SetActive(true);
		this.SelectNextIdol();
		this.SetIdolPosition(this.currentIdol);
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000E525C File Offset: 0x000E345C
	public void StopPlaying()
	{
		this._currentActivationTime = -1f;
		this.currentIdol = -1;
		this.idol.gameObject.SetActive(false);
		this._progressing = false;
		this._lastTappedTime = 0f;
		this.SetGuardian(null);
		this._previousGuardian = null;
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000E52AC File Offset: 0x000E34AC
	public void SetScaleCenterPoint(Transform scaleCenterPoint)
	{
		this.guardianSizeChanger.SetScaleCenterPoint(scaleCenterPoint);
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000E52BA File Offset: 0x000E34BA
	public void IdolWasTapped(NetPlayer tapper)
	{
		if (tapper != null && (!GameMode.ParticipatingPlayers.Contains(tapper) || tapper == this.guardianPlayer))
		{
			return;
		}
		if (!this.IsZoneValid())
		{
			return;
		}
		if (this.UpdateTapCount(tapper))
		{
			this.IdolActivated(tapper);
		}
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x000E52EF File Offset: 0x000E34EF
	public bool IsZoneValid()
	{
		return NetworkSystem.Instance.SessionIsPrivate || ZoneManagement.IsInZone(this.zone);
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000E530C File Offset: 0x000E350C
	private bool UpdateTapCount(NetPlayer tapper)
	{
		if (this.guardianPlayer == null && this._previousGuardian == null)
		{
			return true;
		}
		if (this._currentActivationTime < 0f)
		{
			this._currentActivationTime = 0f;
			this._lastTappedTime = Time.time;
		}
		if (!this._progressing)
		{
			float num = Mathf.Min(Time.time - this._lastTappedTime, this.activationTimePerTap);
			this._lastTappedTime = Time.time;
			if (num + this._currentActivationTime >= this.requiredActivationTime)
			{
				return true;
			}
			this._currentActivationTime += num;
		}
		return false;
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x000E539A File Offset: 0x000E359A
	private void IdolActivated(NetPlayer activater)
	{
		this._currentActivationTime = -1f;
		this.SetGuardian(activater);
		this.SelectNextIdol();
		this.MoveIdolPosition(this.currentIdol);
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000E53C4 File Offset: 0x000E35C4
	public void SetGuardian(NetPlayer newGuardian)
	{
		if (this.guardianPlayer == newGuardian)
		{
			return;
		}
		if (this.guardianPlayer != null)
		{
			if (NetworkSystem.Instance.LocalPlayer == this.guardianPlayer)
			{
				this.PlayerLostGuardianSFX.Play();
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.guardianPlayer, out rigContainer))
			{
				rigContainer.Rig.EnableGuardianEjectWatch(false);
				this.guardianSizeChanger.unacceptRig(rigContainer.Rig);
				int num = RoomSystem.JoinedRoom ? rigContainer.netView.ViewID : rigContainer.CachedNetViewID;
				if (GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex == num)
				{
					GorillaTagger.Instance.offlineVRRig.DroppedByPlayer(rigContainer.Rig, Vector3.zero);
					if (this.guardianPlayer == NetworkSystem.Instance.LocalPlayer)
					{
						bool forLeftHand = GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex == 1;
						EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
					}
				}
			}
		}
		this._previousGuardian = this.guardianPlayer;
		this.guardianPlayer = newGuardian;
		if (this.guardianPlayer != null)
		{
			if (NetworkSystem.Instance.LocalPlayer == this.guardianPlayer)
			{
				this.PlayerGainGuardianSFX.Play();
			}
			else
			{
				this.ObserverGainGuardianSFX.Play();
			}
			RigContainer rigContainer2;
			if (VRRigCache.Instance.TryGetVrrig(this.guardianPlayer, out rigContainer2))
			{
				rigContainer2.Rig.EnableGuardianEjectWatch(true);
				this.guardianSizeChanger.acceptRig(rigContainer2.Rig);
			}
			PlayerGameEvents.GameModeCompleteRound();
			if (NetworkSystem.Instance.LocalPlayer == this.guardianPlayer)
			{
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
		}
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000E5547 File Offset: 0x000E3747
	public bool IsPlayerGuardian(NetPlayer player)
	{
		return player == this.guardianPlayer;
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000E5552 File Offset: 0x000E3752
	private int SelectNextIdol()
	{
		if (this.idolPositions == null || this.idolPositions.Count == 0)
		{
			GTDev.Log<string>("No Guardian Idols possible to select.", null);
			return -1;
		}
		this.currentIdol = this.SelectRandomIdol();
		return this.currentIdol;
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x000E5588 File Offset: 0x000E3788
	private int SelectRandomIdol()
	{
		int result;
		if (this.currentIdol != -1 && this.idolPositions.Count > 1)
		{
			result = (this.currentIdol + Random.Range(1, this.idolPositions.Count)) % this.idolPositions.Count;
		}
		else
		{
			result = Random.Range(0, this.idolPositions.Count);
		}
		return result;
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x000E55E8 File Offset: 0x000E37E8
	private int SelectFarthestFromGuardian()
	{
		if (!(GorillaGameManager.instance is GorillaGuardianManager))
		{
			return this.SelectRandomIdol();
		}
		RigContainer rigContainer;
		if (this.guardianPlayer != null && VRRigCache.Instance.TryGetVrrig(this.guardianPlayer, out rigContainer))
		{
			Vector3 position = rigContainer.transform.position;
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < this.idolPositions.Count; i++)
			{
				float num3 = Vector3.SqrMagnitude(this.idolPositions[i].transform.position - position);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num != -1)
			{
				return num;
			}
		}
		return this.SelectRandomIdol();
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000E5690 File Offset: 0x000E3890
	private int SelectFarFromNearestPlayer()
	{
		List<Transform> list = this.SortByDistanceToNearestPlayer();
		if (list.Count > 1 && this.currentIdol >= 0 && this.currentIdol < list.Count)
		{
			list.Remove(this.idolPositions[this.currentIdol]);
		}
		int index = Random.Range(list.Count / 2, list.Count);
		Transform item = list[index];
		return this.idolPositions.IndexOf(item);
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000E5704 File Offset: 0x000E3904
	private List<Transform> SortByDistanceToNearestPlayer()
	{
		GorillaGuardianZoneManager.<>c__DisplayClass49_0 CS$<>8__locals1 = new GorillaGuardianZoneManager.<>c__DisplayClass49_0();
		CS$<>8__locals1.playerPositions = new List<Vector3>();
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (!(vrrig == null))
			{
				CS$<>8__locals1.playerPositions.Add(vrrig.transform.position);
			}
		}
		this._sortedIdolPositions.Clear();
		foreach (Transform item in this.idolPositions)
		{
			this._sortedIdolPositions.Add(item);
		}
		this._sortedIdolPositions.Sort(new Comparison<Transform>(CS$<>8__locals1.<SortByDistanceToNearestPlayer>g__CompareNearestPlayerDistance|0));
		return this._sortedIdolPositions;
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000E57F8 File Offset: 0x000E39F8
	public void TriggerIdolKnockback()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer;
			if ((this.knockbackIncludesGuardian || RoomSystem.PlayersInRoom[i] != this.guardianPlayer) && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer))
			{
				Vector3 vector = rigContainer.Rig.transform.position - this.idol.transform.position;
				if (Vector3.SqrMagnitude(vector) < this.idolKnockbackRadius * this.idolKnockbackRadius)
				{
					Vector3 velocity = (vector - Vector3.up * Vector3.Dot(Vector3.up, vector)).normalized * this.idolKnockbackStrengthHoriz + Vector3.up * this.idolKnockbackStrengthVert;
					RoomSystem.LaunchPlayer(RoomSystem.PlayersInRoom[i], velocity);
				}
			}
		}
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000E58F4 File Offset: 0x000E3AF4
	private void SetIdolPosition(int index)
	{
		if (index < 0 || index >= this.idolPositions.Count)
		{
			GTDev.Log<string>("Invalid index received", null);
			return;
		}
		this.idol.gameObject.SetActive(true);
		this.idol.SetPosition(this.idolPositions[index].position);
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x000E594C File Offset: 0x000E3B4C
	private void MoveIdolPosition(int index)
	{
		if (index < 0 || index >= this.idolPositions.Count)
		{
			GTDev.Log<string>("Invalid index received", null);
			return;
		}
		this.idol.gameObject.SetActive(true);
		this.idol.MovePositions(this.idolPositions[index].position);
		if (base.photonView.IsMine)
		{
			this.idolMoveCount++;
		}
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x000E59C0 File Offset: 0x000E3BC0
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.isPlaying || player != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext((this.guardianPlayer != null) ? this.guardianPlayer.ActorNumber : 0);
			stream.SendNext(this._currentActivationTime);
			stream.SendNext(this.currentIdol);
			stream.SendNext(this.idolMoveCount);
			return;
		}
		int num = (int)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		int num3 = (int)stream.ReceiveNext();
		int num4 = (int)stream.ReceiveNext();
		if (float.IsNaN(num2) || float.IsInfinity(num2))
		{
			return;
		}
		this.SetGuardian((num != 0) ? NetworkSystem.Instance.GetPlayer(num) : null);
		if (num2 != this._currentActivationTime)
		{
			this._currentActivationTime = num2;
			this._lastTappedTime = Time.time;
		}
		if (num3 != this.currentIdol || num4 != this.idolMoveCount)
		{
			if (this.currentIdol == -1)
			{
				this.SetIdolPosition(num3);
			}
			else
			{
				this.MoveIdolPosition(num3);
			}
			this.currentIdol = num3;
			this.idolMoveCount = num4;
		}
	}

	// Token: 0x040036A0 RID: 13984
	public static List<GorillaGuardianZoneManager> zoneManagers = new List<GorillaGuardianZoneManager>();

	// Token: 0x040036A1 RID: 13985
	[SerializeField]
	private GTZone zone;

	// Token: 0x040036A2 RID: 13986
	[SerializeField]
	private SizeChanger guardianSizeChanger;

	// Token: 0x040036A3 RID: 13987
	[SerializeField]
	private TappableGuardianIdol idol;

	// Token: 0x040036A4 RID: 13988
	[SerializeField]
	private List<Transform> idolPositions;

	// Token: 0x040036A5 RID: 13989
	[Space]
	[SerializeField]
	private float requiredActivationTime = 10f;

	// Token: 0x040036A6 RID: 13990
	[SerializeField]
	private float activationTimePerTap = 1f;

	// Token: 0x040036A7 RID: 13991
	[Space]
	[SerializeField]
	private bool knockbackIncludesGuardian = true;

	// Token: 0x040036A8 RID: 13992
	[SerializeField]
	private float idolKnockbackRadius = 6f;

	// Token: 0x040036A9 RID: 13993
	[SerializeField]
	private float idolKnockbackStrengthVert = 12f;

	// Token: 0x040036AA RID: 13994
	[SerializeField]
	private float idolKnockbackStrengthHoriz = 15f;

	// Token: 0x040036AB RID: 13995
	[Space]
	[SerializeField]
	private SoundBankPlayer PlayerGainGuardianSFX;

	// Token: 0x040036AC RID: 13996
	[SerializeField]
	private SoundBankPlayer PlayerLostGuardianSFX;

	// Token: 0x040036AD RID: 13997
	[SerializeField]
	private SoundBankPlayer ObserverGainGuardianSFX;

	// Token: 0x040036AE RID: 13998
	private NetPlayer guardianPlayer;

	// Token: 0x040036AF RID: 13999
	private NetPlayer _previousGuardian;

	// Token: 0x040036B0 RID: 14000
	private int currentIdol = -1;

	// Token: 0x040036B1 RID: 14001
	private int idolMoveCount;

	// Token: 0x040036B2 RID: 14002
	private List<Transform> _sortedIdolPositions = new List<Transform>();

	// Token: 0x040036B3 RID: 14003
	private float _currentActivationTime = -1f;

	// Token: 0x040036B4 RID: 14004
	private float _lastTappedTime;

	// Token: 0x040036B5 RID: 14005
	private bool _progressing;

	// Token: 0x040036B6 RID: 14006
	private float _idolActivationDisplay;

	// Token: 0x040036B7 RID: 14007
	private bool _zoneIsActive;

	// Token: 0x040036B8 RID: 14008
	private bool _zoneStateChanged;
}
