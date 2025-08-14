using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000740 RID: 1856
[NetworkBehaviourWeaved(5)]
public class HalloweenGhostChaser : NetworkComponent
{
	// Token: 0x06002E6F RID: 11887 RVA: 0x000F58AB File Offset: 0x000F3AAB
	protected override void Awake()
	{
		base.Awake();
		this.spawnIndex = 0;
		this.targetPlayer = null;
		this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
		this.grabTime = -this.minGrabCooldown;
		this.possibleTarget = new List<NetPlayer>();
	}

	// Token: 0x06002E70 RID: 11888 RVA: 0x000F58E0 File Offset: 0x000F3AE0
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x000F5910 File Offset: 0x000F3B10
	private void InitializeGhost()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.lastHeadAngleTime = 0f;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Random.value * this.maxTimeToNextHeadAngle;
			this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			this.ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x000F59B0 File Offset: 0x000F3BB0
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			HalloweenGhostChaser.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case HalloweenGhostChaser.ChaseState.Dormant:
				if (Time.time >= this.nextTimeToChasePlayer)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				if (Time.time >= this.lastSummonCheck + this.summoningDuration)
				{
					this.lastSummonCheck = Time.time;
					this.possibleTarget.Clear();
					int num = 0;
					int i = 0;
					while (i < this.spawnTransforms.Length)
					{
						int num2 = 0;
						for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
						{
							if ((GorillaParent.instance.vrrigs[j].transform.position - this.spawnTransforms[i].position).magnitude < this.summonDistance)
							{
								this.possibleTarget.Add(GorillaParent.instance.vrrigs[j].creator);
								num2++;
								if (num2 >= this.summonCount)
								{
									break;
								}
							}
						}
						if (num2 >= this.summonCount)
						{
							if (!this.wasSurroundedLastCheck)
							{
								this.wasSurroundedLastCheck = true;
								break;
							}
							this.wasSurroundedLastCheck = false;
							this.isSummoned = true;
							this.currentState = HalloweenGhostChaser.ChaseState.Gong;
							break;
						}
						else
						{
							num++;
							i++;
						}
					}
					if (num == this.spawnTransforms.Length)
					{
						this.wasSurroundedLastCheck = false;
					}
				}
				break;
			case HalloweenGhostChaser.ChaseState.InitialRise:
				if (Time.time > this.timeRiseStarted + this.totalTimeToRise)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.Chasing;
				}
				break;
			case (HalloweenGhostChaser.ChaseState)3:
				break;
			case HalloweenGhostChaser.ChaseState.Gong:
				if (Time.time > this.timeGongStarted + this.gongDuration)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				break;
			default:
				if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
				{
					if (chaseState == HalloweenGhostChaser.ChaseState.Grabbing)
					{
						if (Time.time > this.grabTime + this.grabDuration)
						{
							this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
						}
					}
				}
				else
				{
					if (this.followTarget == null || this.targetPlayer == null)
					{
						this.ChooseRandomTarget();
					}
					if (!(this.followTarget == null) && (this.followTarget.position - this.ghostBody.transform.position).magnitude < this.catchDistance)
					{
						this.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
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

	// Token: 0x06002E73 RID: 11891 RVA: 0x000F5C48 File Offset: 0x000F3E48
	public void UpdateState()
	{
		HalloweenGhostChaser.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			this.isSummoned = false;
			if (this.ghostMaterial.color == this.summonedColor)
			{
				this.ghostMaterial.color = this.defaultColor;
				return;
			}
			break;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.RiseHost();
				}
				this.MoveHead();
				return;
			}
			break;
		case (HalloweenGhostChaser.ChaseState)3:
		case HalloweenGhostChaser.ChaseState.Gong:
			break;
		default:
			if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (chaseState != HalloweenGhostChaser.ChaseState.Grabbing)
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
					this.MoveHead();
				}
			}
			else if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				this.MoveHead();
				return;
			}
			break;
		}
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x000F5D2C File Offset: 0x000F3F2C
	private void OnChangeState(HalloweenGhostChaser.ChaseState newState)
	{
		switch (newState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			if (this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				this.InitializeGhost();
			}
			else
			{
				this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			}
			this.SetInitialRotations();
			return;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			this.timeRiseStarted = Time.time;
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				if (!this.isSummoned)
				{
					this.currentSpeed = 0f;
					this.ChooseRandomTarget();
					this.SetInitialSpawnPoint();
				}
				else
				{
					this.currentSpeed = 3f;
				}
			}
			if (this.isSummoned)
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				this.ghostMaterial.color = this.summonedColor;
			}
			else
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlay();
				this.ghostMaterial.color = this.defaultColor;
			}
			this.SetInitialRotations();
			return;
		case (HalloweenGhostChaser.ChaseState)3:
			break;
		case HalloweenGhostChaser.ChaseState.Gong:
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				this.ChooseRandomTarget();
				this.SetInitialSpawnPoint();
				base.transform.position = this.spawnTransforms[this.spawnIndex].position;
			}
			this.timeGongStarted = Time.time;
			this.laugh.volume = 1f;
			this.laugh.GTPlayOneShot(this.gong, 1f);
			this.isSummoned = true;
			return;
		default:
			if (newState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (newState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.grabTime = Time.time;
				if (this.isSummoned)
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				}
				else
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlay();
				}
				this.leftArm.localEulerAngles = this.leftArmGrabbingLocal;
				this.rightArm.localEulerAngles = this.rightArmGrabbingLocal;
				this.leftHand.localEulerAngles = this.leftHandGrabbingLocal;
				this.rightHand.localEulerAngles = this.rightHandGrabbingLocal;
				this.ghostBody.transform.localPosition = this.ghostOffsetGrabbingLocal;
				this.ghostBody.transform.localEulerAngles = this.ghostGrabbingEulerRotation;
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				if (vrrig != null)
				{
					this.followTarget = vrrig.transform;
					return;
				}
			}
			else
			{
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.ResetPath();
			}
			break;
		}
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x000F6024 File Offset: 0x000F4224
	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		this.spawnIndex = 0;
		if (this.followTarget == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnTransforms.Length; i++)
		{
			float magnitude = (this.followTarget.position - this.spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				this.spawnIndex = i;
			}
		}
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x000F6094 File Offset: 0x000F4294
	private void ChooseRandomTarget()
	{
		int num = -1;
		if (this.possibleTarget.Count >= this.summonCount)
		{
			int randomTarget = Random.Range(0, this.possibleTarget.Count);
			num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == this.possibleTarget[randomTarget]);
			this.currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
		}
		this.possibleTarget.Clear();
		if (num < GorillaParent.instance.vrrigs.Count)
		{
			this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
			this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
			return;
		}
		this.targetPlayer = null;
		this.followTarget = null;
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x000F61AC File Offset: 0x000F43AC
	private void SetInitialRotations()
	{
		this.leftArm.localEulerAngles = Vector3.zero;
		this.rightArm.localEulerAngles = Vector3.zero;
		this.leftHand.localEulerAngles = this.leftHandStartingLocal;
		this.rightHand.localEulerAngles = this.rightHandStartingLocal;
		this.ghostBody.transform.localPosition = Vector3.zero;
		this.ghostBody.transform.localEulerAngles = this.ghostStartingEulerRotation;
	}

	// Token: 0x06002E78 RID: 11896 RVA: 0x000F6228 File Offset: 0x000F4428
	private void MoveHead()
	{
		if (Time.time > this.nextHeadAngleTime)
		{
			this.skullTransform.localEulerAngles = this.headEulerAngles[Random.Range(0, this.headEulerAngles.Length)];
			this.lastHeadAngleTime = Time.time;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Mathf.Max(Random.value * this.maxTimeToNextHeadAngle, 0.05f);
		}
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x000F6294 File Offset: 0x000F4494
	private void RiseHost()
	{
		if (Time.time < this.timeRiseStarted + this.totalTimeToRise)
		{
			if (this.spawnIndex == -1)
			{
				this.spawnIndex = 0;
			}
			base.transform.position = this.spawnTransforms[this.spawnIndex].position + Vector3.up * (Time.time - this.timeRiseStarted) / this.totalTimeToRise * this.riseDistance;
			base.transform.rotation = this.spawnTransforms[this.spawnIndex].rotation;
		}
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000F6330 File Offset: 0x000F4530
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTime + this.minGrabCooldown)
		{
			this.grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTime + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.velocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000F63E0 File Offset: 0x000F45E0
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.points[this.points.Count - 1] = destination;
		Vector3 vector = this.points[this.currentTargetIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentTargetIdx + 1 < this.points.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentTargetIdx++;
		}
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x000F64F0 File Offset: 0x000F46F0
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.points = new List<Vector3>();
		foreach (Vector3 a in this.path.corners)
		{
			this.points.Add(a + Vector3.up * this.heightAboveNavmesh);
		}
		this.points.Add(destination);
		this.currentTargetIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x000F65C4 File Offset: 0x000F47C4
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x000F65D0 File Offset: 0x000F47D0
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseTime)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(this.followTarget.position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.followTarget.position, this.currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(this.followTarget.position - base.transform.position, Vector3.up);
		}
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000F66A4 File Offset: 0x000F48A4
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.childGhost.localPosition = this.noisyOffset;
		this.leftArm.localEulerAngles = this.noisyOffset * 20f;
		this.rightArm.localEulerAngles = this.noisyOffset * -20f;
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000F6742 File Offset: 0x000F4942
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x06002E81 RID: 11905 RVA: 0x000F677E File Offset: 0x000F497E
	// (set) Token: 0x06002E82 RID: 11906 RVA: 0x000F67A8 File Offset: 0x000F49A8
	[Networked]
	[NetworkedWeaved(0, 5)]
	public unsafe HalloweenGhostChaser.GhostData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HalloweenGhostChaser.GhostData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HalloweenGhostChaser.GhostData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000F67D4 File Offset: 0x000F49D4
	public override void WriteDataFusion()
	{
		HalloweenGhostChaser.GhostData data = default(HalloweenGhostChaser.GhostData);
		NetPlayer netPlayer = this.targetPlayer;
		data.TargetActorNumber = ((netPlayer != null) ? netPlayer.ActorNumber : -1);
		data.CurrentState = (int)this.currentState;
		data.SpawnIndex = this.spawnIndex;
		data.CurrentSpeed = this.currentSpeed;
		data.IsSummoned = this.isSummoned;
		this.Data = data;
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000F6844 File Offset: 0x000F4A44
	public override void ReadDataFusion()
	{
		int targetActorNumber = this.Data.TargetActorNumber;
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		this.currentState = (HalloweenGhostChaser.ChaseState)this.Data.CurrentState;
		this.spawnIndex = this.Data.SpawnIndex;
		float f = this.Data.CurrentSpeed;
		this.isSummoned = this.Data.IsSummoned;
		if (float.IsFinite(f))
		{
			this.currentSpeed = f;
		}
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000F68C4 File Offset: 0x000F4AC4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (this.targetPlayer == null)
		{
			stream.SendNext(-1);
		}
		else
		{
			stream.SendNext(this.targetPlayer.ActorNumber);
		}
		stream.SendNext(this.currentState);
		stream.SendNext(this.spawnIndex);
		stream.SendNext(this.currentSpeed);
		stream.SendNext(this.isSummoned);
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000F6960 File Offset: 0x000F4B60
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		int playerID = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		this.currentState = (HalloweenGhostChaser.ChaseState)stream.ReceiveNext();
		this.spawnIndex = (int)stream.ReceiveNext();
		float f = (float)stream.ReceiveNext();
		this.isSummoned = (bool)stream.ReceiveNext();
		if (float.IsFinite(f))
		{
			this.currentSpeed = f;
		}
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000F69F5 File Offset: 0x000F4BF5
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x000F6A13 File Offset: 0x000F4C13
	public void OnJoinedRoom()
	{
		Debug.Log("Here");
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeGhost();
			return;
		}
		this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000F6AE3 File Offset: 0x000F4CE3
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x000F6AFB File Offset: 0x000F4CFB
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04003A32 RID: 14898
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x04003A33 RID: 14899
	public Transform followTarget;

	// Token: 0x04003A34 RID: 14900
	public Transform childGhost;

	// Token: 0x04003A35 RID: 14901
	public float velocityStep = 1f;

	// Token: 0x04003A36 RID: 14902
	public float currentSpeed;

	// Token: 0x04003A37 RID: 14903
	public float velocityIncreaseTime = 20f;

	// Token: 0x04003A38 RID: 14904
	public float riseDistance = 2f;

	// Token: 0x04003A39 RID: 14905
	public float summonDistance = 5f;

	// Token: 0x04003A3A RID: 14906
	public float timeEncircled;

	// Token: 0x04003A3B RID: 14907
	public float lastSummonCheck;

	// Token: 0x04003A3C RID: 14908
	public float timeGongStarted;

	// Token: 0x04003A3D RID: 14909
	public float summoningDuration = 30f;

	// Token: 0x04003A3E RID: 14910
	public float summoningCheckCountdown = 5f;

	// Token: 0x04003A3F RID: 14911
	public float gongDuration = 5f;

	// Token: 0x04003A40 RID: 14912
	public int summonCount = 5;

	// Token: 0x04003A41 RID: 14913
	public bool wasSurroundedLastCheck;

	// Token: 0x04003A42 RID: 14914
	public AudioSource laugh;

	// Token: 0x04003A43 RID: 14915
	public List<NetPlayer> possibleTarget;

	// Token: 0x04003A44 RID: 14916
	public AudioClip defaultLaugh;

	// Token: 0x04003A45 RID: 14917
	public AudioClip deepLaugh;

	// Token: 0x04003A46 RID: 14918
	public AudioClip gong;

	// Token: 0x04003A47 RID: 14919
	public Vector3 noisyOffset;

	// Token: 0x04003A48 RID: 14920
	public Vector3 leftArmGrabbingLocal;

	// Token: 0x04003A49 RID: 14921
	public Vector3 rightArmGrabbingLocal;

	// Token: 0x04003A4A RID: 14922
	public Vector3 leftHandGrabbingLocal;

	// Token: 0x04003A4B RID: 14923
	public Vector3 rightHandGrabbingLocal;

	// Token: 0x04003A4C RID: 14924
	public Vector3 leftHandStartingLocal;

	// Token: 0x04003A4D RID: 14925
	public Vector3 rightHandStartingLocal;

	// Token: 0x04003A4E RID: 14926
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04003A4F RID: 14927
	public Vector3 ghostStartingEulerRotation;

	// Token: 0x04003A50 RID: 14928
	public Vector3 ghostGrabbingEulerRotation;

	// Token: 0x04003A51 RID: 14929
	public float maxTimeToNextHeadAngle;

	// Token: 0x04003A52 RID: 14930
	public float lastHeadAngleTime;

	// Token: 0x04003A53 RID: 14931
	public float nextHeadAngleTime;

	// Token: 0x04003A54 RID: 14932
	public float nextTimeToChasePlayer;

	// Token: 0x04003A55 RID: 14933
	public float maxNextTimeToChasePlayer;

	// Token: 0x04003A56 RID: 14934
	public float timeRiseStarted;

	// Token: 0x04003A57 RID: 14935
	public float totalTimeToRise;

	// Token: 0x04003A58 RID: 14936
	public float catchDistance;

	// Token: 0x04003A59 RID: 14937
	public float grabTime;

	// Token: 0x04003A5A RID: 14938
	public float grabDuration;

	// Token: 0x04003A5B RID: 14939
	public float grabSpeed = 1f;

	// Token: 0x04003A5C RID: 14940
	public float minGrabCooldown;

	// Token: 0x04003A5D RID: 14941
	public float lastSpeedIncreased;

	// Token: 0x04003A5E RID: 14942
	public Vector3[] headEulerAngles;

	// Token: 0x04003A5F RID: 14943
	public Transform skullTransform;

	// Token: 0x04003A60 RID: 14944
	public Transform leftArm;

	// Token: 0x04003A61 RID: 14945
	public Transform rightArm;

	// Token: 0x04003A62 RID: 14946
	public Transform leftHand;

	// Token: 0x04003A63 RID: 14947
	public Transform rightHand;

	// Token: 0x04003A64 RID: 14948
	public Transform[] spawnTransforms;

	// Token: 0x04003A65 RID: 14949
	public Transform[] spawnTransformOffsets;

	// Token: 0x04003A66 RID: 14950
	public NetPlayer targetPlayer;

	// Token: 0x04003A67 RID: 14951
	public GameObject ghostBody;

	// Token: 0x04003A68 RID: 14952
	public HalloweenGhostChaser.ChaseState currentState;

	// Token: 0x04003A69 RID: 14953
	public HalloweenGhostChaser.ChaseState lastState;

	// Token: 0x04003A6A RID: 14954
	public int spawnIndex;

	// Token: 0x04003A6B RID: 14955
	public NetPlayer grabbedPlayer;

	// Token: 0x04003A6C RID: 14956
	public Material ghostMaterial;

	// Token: 0x04003A6D RID: 14957
	public Color defaultColor;

	// Token: 0x04003A6E RID: 14958
	public Color summonedColor;

	// Token: 0x04003A6F RID: 14959
	public bool isSummoned;

	// Token: 0x04003A70 RID: 14960
	private bool targetIsOnNavMesh;

	// Token: 0x04003A71 RID: 14961
	private const float navMeshSampleRange = 5f;

	// Token: 0x04003A72 RID: 14962
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04003A73 RID: 14963
	public float hapticDuration = 1.5f;

	// Token: 0x04003A74 RID: 14964
	private NavMeshPath path;

	// Token: 0x04003A75 RID: 14965
	public List<Vector3> points;

	// Token: 0x04003A76 RID: 14966
	public int currentTargetIdx;

	// Token: 0x04003A77 RID: 14967
	private float nextPathTimestamp;

	// Token: 0x04003A78 RID: 14968
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 5)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HalloweenGhostChaser.GhostData _Data;

	// Token: 0x02000741 RID: 1857
	public enum ChaseState
	{
		// Token: 0x04003A7A RID: 14970
		Dormant = 1,
		// Token: 0x04003A7B RID: 14971
		InitialRise,
		// Token: 0x04003A7C RID: 14972
		Gong = 4,
		// Token: 0x04003A7D RID: 14973
		Chasing = 8,
		// Token: 0x04003A7E RID: 14974
		Grabbing = 16
	}

	// Token: 0x02000742 RID: 1858
	[NetworkStructWeaved(5)]
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct GhostData : INetworkStruct
	{
		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06002E8C RID: 11916 RVA: 0x000F6B0F File Offset: 0x000F4D0F
		// (set) Token: 0x06002E8D RID: 11917 RVA: 0x000F6B1D File Offset: 0x000F4D1D
		[Networked]
		public unsafe float CurrentSpeed
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed) = value;
			}
		}

		// Token: 0x04003A7F RID: 14975
		[FieldOffset(0)]
		public int TargetActorNumber;

		// Token: 0x04003A80 RID: 14976
		[FieldOffset(4)]
		public int CurrentState;

		// Token: 0x04003A81 RID: 14977
		[FieldOffset(8)]
		public int SpawnIndex;

		// Token: 0x04003A82 RID: 14978
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(12)]
		private FixedStorage@1 _CurrentSpeed;

		// Token: 0x04003A83 RID: 14979
		[FieldOffset(16)]
		public NetworkBool IsSummoned;
	}
}
