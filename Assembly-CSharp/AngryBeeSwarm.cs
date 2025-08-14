using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag.Rendering;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

// Token: 0x02000102 RID: 258
[NetworkBehaviourWeaved(3)]
public class AngryBeeSwarm : NetworkComponent
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000666 RID: 1638 RVA: 0x00024D70 File Offset: 0x00022F70
	public bool isDormant
	{
		get
		{
			return this.currentState == AngryBeeSwarm.ChaseState.Dormant;
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00024D7C File Offset: 0x00022F7C
	protected override void Awake()
	{
		base.Awake();
		AngryBeeSwarm.instance = this;
		this.targetPlayer = null;
		this.currentState = AngryBeeSwarm.ChaseState.Dormant;
		this.grabTimestamp = -this.minGrabCooldown;
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00024DCC File Offset: 0x00022FCC
	private void InitializeSwarm()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.beeAnimator.transform.localPosition = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00024E18 File Offset: 0x00023018
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = AngryBeeSwarm.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			AngryBeeSwarm.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case AngryBeeSwarm.ChaseState.Dormant:
				if (Application.isEditor && Keyboard.current[Key.Space].wasPressedThisFrame)
				{
					this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
				}
				break;
			case AngryBeeSwarm.ChaseState.InitialEmerge:
				if (Time.time > this.emergeStartedTimestamp + this.totalTimeToEmerge)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Chasing;
				}
				break;
			case (AngryBeeSwarm.ChaseState)3:
				break;
			case AngryBeeSwarm.ChaseState.Chasing:
				if (this.followTarget == null || this.targetPlayer == null || Time.time > this.NextRefreshClosestPlayerTimestamp)
				{
					this.ChooseClosestTarget();
					if (this.followTarget != null)
					{
						this.BoredToDeathAtTimestamp = -1f;
					}
					else if (this.BoredToDeathAtTimestamp < 0f)
					{
						this.BoredToDeathAtTimestamp = Time.time + this.boredAfterDuration;
					}
				}
				if (this.BoredToDeathAtTimestamp >= 0f && Time.time > this.BoredToDeathAtTimestamp)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Dormant;
				}
				else if (!(this.followTarget == null) && (this.followTarget.position - this.beeAnimator.transform.position).magnitude < this.catchDistance)
				{
					float num = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
					if (this.followTarget.position.y > num)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Grabbing;
					}
				}
				break;
			default:
				if (chaseState == AngryBeeSwarm.ChaseState.Grabbing)
				{
					if (Time.time > this.grabTimestamp + this.grabDuration)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Dormant;
					}
				}
				break;
			}
		}
		if (this.lastState != this.currentState)
		{
			this.OnChangeState(this.currentState);
			this.lastState = this.currentState;
		}
		this.UpdateState();
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00024FFC File Offset: 0x000231FC
	public void UpdateState()
	{
		AngryBeeSwarm.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			if (NetworkSystem.Instance.InRoom)
			{
				this.SwarmEmergeUpdateShared();
				return;
			}
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				return;
			}
			break;
		default:
			if (chaseState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.RiseGrabbedLocalPlayer();
				}
				this.GrabBodyShared();
			}
			break;
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0002508B File Offset: 0x0002328B
	public void Emerge(Vector3 fromPosition, Vector3 toPosition)
	{
		base.transform.position = fromPosition;
		this.emergeFromPosition = fromPosition;
		this.emergeToPosition = toPosition;
		this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
		this.emergeStartedTimestamp = Time.time;
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x000250BC File Offset: 0x000232BC
	private void OnChangeState(AngryBeeSwarm.ChaseState newState)
	{
		switch (newState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
			if (this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				base.transform.position = new Vector3(0f, -9999f, 0f);
				this.InitializeSwarm();
			}
			this.SetInitialRotations();
			return;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			this.emergeStartedTimestamp = Time.time;
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(0f);
			if (base.IsMine)
			{
				this.currentSpeed = 0f;
				this.ChooseClosestTarget();
			}
			this.SetInitialRotations();
			return;
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(1f);
			this.ResetPath();
			this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
			this.BoredToDeathAtTimestamp = -1f;
			return;
		default:
		{
			if (newState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.grabTimestamp = Time.time;
			this.beeAnimator.transform.localPosition = this.ghostOffsetGrabbingLocal;
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
			if (vrrig != null)
			{
				this.followTarget = vrrig.transform;
			}
			break;
		}
		}
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00025264 File Offset: 0x00023464
	private void ChooseClosestTarget()
	{
		float num = Mathf.Lerp(this.initialRangeLimit, this.finalRangeLimit, (Time.time + this.totalTimeToEmerge - this.emergeStartedTimestamp) / this.rangeLimitBlendDuration);
		float num2 = num * num;
		VRRig vrrig = null;
		float num3 = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
		foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
		{
			if (vrrig2.head != null && !(vrrig2.head.rigTarget == null) && vrrig2.head.rigTarget.position.y > num3)
			{
				float sqrMagnitude = (base.transform.position - vrrig2.head.rigTarget.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
		}
		if (vrrig != null)
		{
			this.targetPlayer = vrrig.creator;
			this.followTarget = vrrig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
		}
		else
		{
			this.targetPlayer = null;
			this.followTarget = null;
		}
		this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x000253D0 File Offset: 0x000235D0
	private void SetInitialRotations()
	{
		this.beeAnimator.transform.localPosition = Vector3.zero;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x000253E8 File Offset: 0x000235E8
	private void SwarmEmergeUpdateShared()
	{
		if (Time.time < this.emergeStartedTimestamp + this.totalTimeToEmerge)
		{
			float emergeFraction = (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge;
			if (base.IsMine)
			{
				base.transform.position = Vector3.Lerp(this.emergeFromPosition, this.emergeToPosition, (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge);
			}
			this.beeAnimator.SetEmergeFraction(emergeFraction);
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00025460 File Offset: 0x00023660
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTimestamp + this.minGrabCooldown)
		{
			this.grabTimestamp = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTimestamp + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.velocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00025510 File Offset: 0x00023710
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.pathPoints[this.pathPoints.Count - 1] = destination;
		Vector3 vector = this.pathPoints[this.currentPathPointIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentPathPointIdx + 1 < this.pathPoints.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentPathPointIdx++;
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00025620 File Offset: 0x00023820
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.pathPoints = new List<Vector3>();
		foreach (Vector3 a in this.path.corners)
		{
			this.pathPoints.Add(a + Vector3.up * this.heightAboveNavmesh);
		}
		this.pathPoints.Add(destination);
		this.currentPathPointIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x000256F4 File Offset: 0x000238F4
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00025700 File Offset: 0x00023900
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseInterval)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			float num = ZoneShaderSettings.GetWaterY() + this.MinHeightAboveWater;
			Vector3 position = this.followTarget.position;
			if (position.y < num)
			{
				position.y = num;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, position, this.currentSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x000257B8 File Offset: 0x000239B8
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.beeAnimator.transform.localPosition = this.noisyOffset;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00025825 File Offset: 0x00023A25
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000677 RID: 1655 RVA: 0x00025861 File Offset: 0x00023A61
	// (set) Token: 0x06000678 RID: 1656 RVA: 0x0002588B File Offset: 0x00023A8B
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe BeeSwarmData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BeeSwarmData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BeeSwarmData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x000258B6 File Offset: 0x00023AB6
	public override void WriteDataFusion()
	{
		this.Data = new BeeSwarmData(this.targetPlayer.ActorNumber, (int)this.currentState, this.currentSpeed);
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x000258DC File Offset: 0x00023ADC
	public override void ReadDataFusion()
	{
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(this.Data.TargetActorNumber);
		this.currentState = (AngryBeeSwarm.ChaseState)this.Data.CurrentState;
		if (float.IsFinite(this.Data.CurrentSpeed))
		{
			this.currentSpeed = this.Data.CurrentSpeed;
		}
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00025944 File Offset: 0x00023B44
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.Equals(PhotonNetwork.MasterClient))
		{
			return;
		}
		NetPlayer netPlayer = this.targetPlayer;
		stream.SendNext((netPlayer != null) ? netPlayer.ActorNumber : -1);
		stream.SendNext(this.currentState);
		stream.SendNext(this.currentSpeed);
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x000259AC File Offset: 0x00023BAC
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int playerID = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		this.currentState = (AngryBeeSwarm.ChaseState)stream.ReceiveNext();
		float f = (float)stream.ReceiveNext();
		if (float.IsFinite(f))
		{
			this.currentSpeed = f;
		}
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00025A10 File Offset: 0x00023C10
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00025A2E File Offset: 0x00023C2E
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeSwarm();
		}
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00025A42 File Offset: 0x00023C42
	private void TestEmerge()
	{
		this.Emerge(this.testEmergeFrom.transform.position, this.testEmergeTo.transform.position);
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00025AF8 File Offset: 0x00023CF8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00025B10 File Offset: 0x00023D10
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040007BC RID: 1980
	public static AngryBeeSwarm instance;

	// Token: 0x040007BD RID: 1981
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x040007BE RID: 1982
	public Transform followTarget;

	// Token: 0x040007BF RID: 1983
	[SerializeField]
	private float velocityStep = 1f;

	// Token: 0x040007C0 RID: 1984
	private float currentSpeed;

	// Token: 0x040007C1 RID: 1985
	[SerializeField]
	private float velocityIncreaseInterval = 20f;

	// Token: 0x040007C2 RID: 1986
	public Vector3 noisyOffset;

	// Token: 0x040007C3 RID: 1987
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x040007C4 RID: 1988
	private float emergeStartedTimestamp;

	// Token: 0x040007C5 RID: 1989
	private float grabTimestamp;

	// Token: 0x040007C6 RID: 1990
	private float lastSpeedIncreased;

	// Token: 0x040007C7 RID: 1991
	[SerializeField]
	private float totalTimeToEmerge;

	// Token: 0x040007C8 RID: 1992
	[SerializeField]
	private float catchDistance;

	// Token: 0x040007C9 RID: 1993
	[SerializeField]
	private float grabDuration;

	// Token: 0x040007CA RID: 1994
	[SerializeField]
	private float grabSpeed = 1f;

	// Token: 0x040007CB RID: 1995
	[SerializeField]
	private float minGrabCooldown;

	// Token: 0x040007CC RID: 1996
	[SerializeField]
	private float initialRangeLimit;

	// Token: 0x040007CD RID: 1997
	[SerializeField]
	private float finalRangeLimit;

	// Token: 0x040007CE RID: 1998
	[SerializeField]
	private float rangeLimitBlendDuration;

	// Token: 0x040007CF RID: 1999
	[SerializeField]
	private float boredAfterDuration;

	// Token: 0x040007D0 RID: 2000
	public NetPlayer targetPlayer;

	// Token: 0x040007D1 RID: 2001
	public AngryBeeAnimator beeAnimator;

	// Token: 0x040007D2 RID: 2002
	public AngryBeeSwarm.ChaseState currentState;

	// Token: 0x040007D3 RID: 2003
	public AngryBeeSwarm.ChaseState lastState;

	// Token: 0x040007D4 RID: 2004
	public NetPlayer grabbedPlayer;

	// Token: 0x040007D5 RID: 2005
	private bool targetIsOnNavMesh;

	// Token: 0x040007D6 RID: 2006
	private const float navMeshSampleRange = 5f;

	// Token: 0x040007D7 RID: 2007
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x040007D8 RID: 2008
	public float hapticDuration = 1.5f;

	// Token: 0x040007D9 RID: 2009
	public float MinHeightAboveWater = 0.5f;

	// Token: 0x040007DA RID: 2010
	public float PlayerMinHeightAboveWater = 0.5f;

	// Token: 0x040007DB RID: 2011
	public float RefreshClosestPlayerInterval = 1f;

	// Token: 0x040007DC RID: 2012
	private float NextRefreshClosestPlayerTimestamp = 1f;

	// Token: 0x040007DD RID: 2013
	private float BoredToDeathAtTimestamp = -1f;

	// Token: 0x040007DE RID: 2014
	[SerializeField]
	private Transform testEmergeFrom;

	// Token: 0x040007DF RID: 2015
	[SerializeField]
	private Transform testEmergeTo;

	// Token: 0x040007E0 RID: 2016
	private Vector3 emergeFromPosition;

	// Token: 0x040007E1 RID: 2017
	private Vector3 emergeToPosition;

	// Token: 0x040007E2 RID: 2018
	private NavMeshPath path;

	// Token: 0x040007E3 RID: 2019
	public List<Vector3> pathPoints;

	// Token: 0x040007E4 RID: 2020
	public int currentPathPointIdx;

	// Token: 0x040007E5 RID: 2021
	private float nextPathTimestamp;

	// Token: 0x040007E6 RID: 2022
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BeeSwarmData _Data;

	// Token: 0x02000103 RID: 259
	public enum ChaseState
	{
		// Token: 0x040007E8 RID: 2024
		Dormant = 1,
		// Token: 0x040007E9 RID: 2025
		InitialEmerge,
		// Token: 0x040007EA RID: 2026
		Chasing = 4,
		// Token: 0x040007EB RID: 2027
		Grabbing = 8
	}
}
