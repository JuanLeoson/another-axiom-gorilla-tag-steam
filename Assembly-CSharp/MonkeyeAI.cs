using System;
using System.Collections.Generic;
using GorillaLocomotion;
using JetBrains.Annotations;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000BF RID: 191
[RequireComponent(typeof(NetworkView))]
public class MonkeyeAI : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060004A0 RID: 1184 RVA: 0x0001AA78 File Offset: 0x00018C78
	private string UserIdFromRig(VRRig rig)
	{
		if (rig == null)
		{
			return "";
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return "-1";
			}
			Debug.Log("Not in a room but not targeting offline rig");
			return null;
		}
		else
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return NetworkSystem.Instance.LocalPlayer.UserId;
			}
			if (rig.creator == null)
			{
				return "";
			}
			return rig.creator.UserId;
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0001AB00 File Offset: 0x00018D00
	private VRRig GetRig(string userId)
	{
		if (userId == "")
		{
			return null;
		}
		if (NetworkSystem.Instance.InRoom || !(userId != "-1"))
		{
			foreach (VRRig vrrig in this.GetValidChoosableRigs())
			{
				if (!(vrrig == null))
				{
					NetPlayer creator = vrrig.creator;
					if (creator != null && userId == creator.UserId)
					{
						return vrrig;
					}
				}
			}
			return null;
		}
		if (userId == "-1 " && GorillaTagger.Instance != null)
		{
			return GorillaTagger.Instance.offlineVRRig;
		}
		return null;
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001ABC4 File Offset: 0x00018DC4
	private float Distance2D(Vector3 a, Vector3 b)
	{
		Vector2 a2 = new Vector2(a.x, a.z);
		Vector2 b2 = new Vector2(b.x, b.z);
		return Vector2.Distance(a2, b2);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001ABFC File Offset: 0x00018DFC
	private Transform PickRandomPatrolPoint()
	{
		int num;
		do
		{
			num = Random.Range(0, this.patrolPts.Count);
		}
		while (num == this.patrolIdx);
		this.patrolIdx = num;
		return this.patrolPts[num];
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001AC3C File Offset: 0x00018E3C
	private void PickNewPath(bool pathFinished = false)
	{
		if (this.calculatingPath)
		{
			return;
		}
		this.currentWaypoint = 0;
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Patrolling:
			if (this.patrolCount == this.maxPatrols)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount = 0;
			}
			else
			{
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount++;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Chasing:
			if (!this.lockedOn)
			{
				Vector3 position = base.transform.position;
				VRRig vrrig;
				if (this.ClosestPlayer(position, out vrrig) && vrrig != this.targetRig)
				{
					this.SetTargetPlayer(vrrig);
				}
			}
			if (this.targetRig == null)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.sleepPt.position;
			}
			else
			{
				this.targetPosition = this.targetRig.transform.position;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
			this.targetPosition = this.sleepPt.position;
			break;
		}
		this.calculatingPath = true;
		this.seeker.StartPath(base.transform.position, this.targetPosition, new OnPathDelegate(this.OnPathComplete));
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001AD8C File Offset: 0x00018F8C
	private void Awake()
	{
		this.lazerFx = base.GetComponent<Monkeye_LazerFX>();
		this.animController = base.GetComponent<Animator>();
		this.layerBase = this.animController.GetLayerIndex("Base_Layer");
		this.layerForward = this.animController.GetLayerIndex("MoveFwdAddPose");
		this.layerLeft = this.animController.GetLayerIndex("TurnLAddPose");
		this.layerRight = this.animController.GetLayerIndex("TurnRAddPose");
		this.seeker = base.GetComponent<Seeker>();
		this.renderer = this.portalFx.GetComponent<Renderer>();
		this.portalMatPropBlock = new MaterialPropertyBlock();
		this.monkEyeMatPropBlock = new MaterialPropertyBlock();
		this.layerMask = (UnityLayer.Default.ToLayerMask() | UnityLayer.GorillaObject.ToLayerMask());
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.replStateRequestableOwnershipGaurd = this.replState.GetComponent<RequestableOwnershipGuard>();
		this.myRequestableOwnershipGaurd = base.GetComponent<RequestableOwnershipGuard>();
		if (this.monkEyeColor.a != 0f || this.monkEyeEyeColorNormal.a != 0f)
		{
			if (this.monkEyeColor.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.ColorShaderProp, this.monkEyeColor);
			}
			if (this.monkEyeEyeColorNormal.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, this.monkEyeEyeColorNormal);
			}
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
		base.InvokeRepeating("AntiOverlapAssurance", 0.2f, 0.5f);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001AF23 File Offset: 0x00019123
	private void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001AF38 File Offset: 0x00019138
	private void OnPathComplete(Path path_)
	{
		this.path = path_;
		this.currentWaypoint = 0;
		if (this.path.vectorPath.Count < 1)
		{
			base.transform.position = this.sleepPt.position;
			base.transform.rotation = this.sleepPt.rotation;
			this.path = null;
		}
		this.calculatingPath = false;
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001AFA0 File Offset: 0x000191A0
	private void FollowPath()
	{
		if (this.path == null || this.currentWaypoint >= this.path.vectorPath.Count || this.currentWaypoint < 0)
		{
			this.PickNewPath(false);
			if (this.path == null)
			{
				return;
			}
		}
		if (this.Distance2D(base.transform.position, this.path.vectorPath[this.currentWaypoint]) < 0.01f)
		{
			if (this.currentWaypoint + 1 == this.path.vectorPath.Count)
			{
				this.PickNewPath(true);
				return;
			}
			this.currentWaypoint++;
		}
		Vector3 normalized = (this.path.vectorPath[this.currentWaypoint] - base.transform.position).normalized;
		normalized.y = 0f;
		if (this.animController.GetCurrentAnimatorStateInfo(0).IsName("Move"))
		{
			Vector3 a = normalized * this.speed;
			base.transform.position += a * this.deltaTime;
		}
		Mathf.Clamp01(Vector3.Dot(base.transform.forward, normalized) / 1.5707964f);
		if (Mathf.Sign(Vector3.Cross(base.transform.forward, normalized).y) > 0f)
		{
			this.animController.SetLayerWeight(this.layerRight, 0f);
		}
		else
		{
			this.animController.SetLayerWeight(this.layerLeft, 0f);
		}
		this.animController.SetLayerWeight(this.layerForward, 0f);
		Vector3 forward = Vector3.RotateTowards(base.transform.forward, normalized, this.rotationSpeed * this.deltaTime, 0f);
		base.transform.rotation = Quaternion.LookRotation(forward);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001B184 File Offset: 0x00019384
	private bool PlayerNear(VRRig rig, float dist, out float playerDist)
	{
		if (rig == null)
		{
			playerDist = float.PositiveInfinity;
			return false;
		}
		playerDist = this.Distance2D(rig.transform.position, base.transform.position);
		return playerDist < dist && Physics.RaycastNonAlloc(new Ray(base.transform.position, rig.transform.position - base.transform.position), this.rayResults, playerDist, this.layerMask) <= 0;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001B214 File Offset: 0x00019414
	private void Sleeping()
	{
		this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
		if (this.audioSource.volume == this.sleepLoopVolume)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			this.PickNewPath(false);
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001B274 File Offset: 0x00019474
	private bool ClosestPlayer(in Vector3 myPos, out VRRig outRig)
	{
		float num = float.MaxValue;
		outRig = null;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num2 = 0f;
			if (this.PlayerNear(vrrig, this.chaseDistance, out num2) && num2 < num)
			{
				num = num2;
				outRig = vrrig;
			}
		}
		return num != float.MaxValue;
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001B2F4 File Offset: 0x000194F4
	private bool CheckForChase()
	{
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num = 0f;
			if (this.PlayerNear(vrrig, this.wakeDistance, out num))
			{
				this.SetTargetPlayer(vrrig);
				this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
				this.PickNewPath(false);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001B374 File Offset: 0x00019574
	public void SetChasePlayer(VRRig rig)
	{
		if (!this.GetValidChoosableRigs().Contains(rig))
		{
			return;
		}
		this.SetTargetPlayer(rig);
		this.lockedOn = true;
		this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
		this.PickNewPath(false);
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001B3A1 File Offset: 0x000195A1
	public void SetSleep()
	{
		if (this.replState.state == MonkeyeAI_ReplState.EStates.Patrolling || this.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001B3C8 File Offset: 0x000195C8
	private void Patrolling()
	{
		this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.audioSource.volume == this.patrolLoopVolume)
		{
			this.CheckForChase();
		}
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001B430 File Offset: 0x00019630
	private void Chasing()
	{
		this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
		this.PickNewPath(false);
		if (this.targetRig == null)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			return;
		}
		if (this.Distance2D(base.transform.position, this.targetRig.transform.position) < this.attackDistance)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.BeginAttack);
			return;
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001B4BC File Offset: 0x000196BC
	private void ReturnToSleepPt()
	{
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.CheckForChase())
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
			return;
		}
		if (this.Distance2D(base.transform.position, this.sleepPt.position) < 0.01f)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001B514 File Offset: 0x00019714
	private void UpdateClientState()
	{
		if (this.wasConnectedToRoom && !NetworkSystem.Instance.InRoom)
		{
			this.SetDefaultState();
			return;
		}
		if (ColliderEnabledManager.instance != null && !this.replState.floorEnabled)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.replState.userId == "-1")
				{
					ColliderEnabledManager.instance.DisableFloorForFrame();
				}
			}
			else if (this.replState.userId == NetworkSystem.Instance.LocalPlayer.UserId)
			{
				ColliderEnabledManager.instance.DisableFloorForFrame();
			}
		}
		if (this.portalFx.activeSelf != this.replState.portalEnabled)
		{
			this.portalFx.SetActive(this.replState.portalEnabled);
		}
		this.portalFx.transform.position = new Vector3(this.replState.attackPos.x, this.portalFx.transform.position.y, this.replState.attackPos.z);
		this.replState.timer -= this.deltaTime;
		if (this.replState.timer < 0f)
		{
			this.replState.timer = 0f;
		}
		VRRig rig = this.GetRig(this.replState.userId);
		if (this.replState.state >= MonkeyeAI_ReplState.EStates.BeginAttack)
		{
			if (rig == null)
			{
				this.lazerFx.DisableLazer();
			}
			else if (this.replState.state < MonkeyeAI_ReplState.EStates.DropPlayer)
			{
				this.lazerFx.EnableLazer(this.eyeBones, rig);
			}
			else
			{
				this.lazerFx.DisableLazer();
			}
		}
		else
		{
			this.lazerFx.DisableLazer();
		}
		if (this.replState.portalEnabled)
		{
			this.portalColor.a = this.replState.alpha;
			this.portalMatPropBlock.SetVector(MonkeyeAI.tintColorShaderProp, this.portalColor);
			this.renderer.SetPropertyBlock(this.portalMatPropBlock);
		}
		if (GorillaTagger.Instance.offlineVRRig == rig && this.replState.freezePlayer)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			Vector3 vector = rigidbody.velocity;
			rigidbody.velocity = new Vector3(vector.x * this.deltaTime * 4f, Mathf.Min(vector.y, 0f), vector.x * this.deltaTime * 4f);
		}
		if (!this.replState.IsMine)
		{
			this.SetClientState(this.replState.state);
		}
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001B7B9 File Offset: 0x000199B9
	private void SetDefaultState()
	{
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.SetDefaultAttackState();
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001B7C8 File Offset: 0x000199C8
	private void SetDefaultAttackState()
	{
		this.replState.floorEnabled = true;
		this.replState.timer = 0f;
		this.replState.userId = "";
		this.replState.attackPos = base.transform.position;
		this.replState.portalEnabled = false;
		this.replState.freezePlayer = false;
		this.replState.alpha = 0f;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001B83F File Offset: 0x00019A3F
	private void ExitAttackState()
	{
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001B850 File Offset: 0x00019A50
	private void BeginAttack()
	{
		this.path = null;
		this.replState.freezePlayer = true;
		if (this.replState.timer <= 0f)
		{
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlayOneShot(this.attackSound, this.attackVolume);
			}
			this.replState.timer = this.openFloorTime;
			this.replState.portalEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.OpenFloor);
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001B8CC File Offset: 0x00019ACC
	private void OpenFloor()
	{
		this.replState.alpha = Mathf.Lerp(0f, 1f, 1f - Mathf.Clamp01(this.replState.timer / this.openFloorTime));
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = false;
			this.SetState(MonkeyeAI_ReplState.EStates.DropPlayer);
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001B946 File Offset: 0x00019B46
	private void DropPlayer()
	{
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.CloseFloor);
		}
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001B97E File Offset: 0x00019B7E
	private void CloseFloor()
	{
		if (this.replState.timer <= 0f)
		{
			this.ExitAttackState();
		}
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001B998 File Offset: 0x00019B98
	private void ValidateChasingRig()
	{
		if (this.targetRig == null)
		{
			this.SetTargetPlayer(null);
			return;
		}
		bool flag = false;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			if (vrrig == this.targetRig)
			{
				flag = true;
				this.SetTargetPlayer(vrrig);
				break;
			}
		}
		if (!flag)
		{
			this.SetTargetPlayer(null);
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001BA20 File Offset: 0x00019C20
	public void SetState(MonkeyeAI_ReplState.EStates state_)
	{
		if (this.replState.IsMine)
		{
			this.replState.state = state_;
		}
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.sleepLoopSound;
			this.audioSource.volume = 0f;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.patrolLoopSound;
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
			}
			this.patrolCount = 0;
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			this.audioSource.clip = this.chaseLoopSound;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
		case MonkeyeAI_ReplState.EStates.GoToSleep:
			break;
		case MonkeyeAI_ReplState.EStates.BeginAttack:
			this.setEyeColor(this.monkEyeEyeColorAttacking);
			if (this.replState.IsMine)
			{
				this.replState.attackPos = ((this.targetRig != null) ? this.targetRig.transform.position : base.transform.position);
				this.replState.timer = this.beginAttackTime;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001BBF0 File Offset: 0x00019DF0
	public void SetClientState(MonkeyeAI_ReplState.EStates state_)
	{
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		if (this.previousState != this.replState.state)
		{
			this.previousState = this.replState.state;
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.sleepLoopSound;
				this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.patrolLoopSound;
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				this.patrolCount = 0;
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
				this.audioSource.clip = this.chaseLoopSound;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.setEyeColor(this.monkEyeEyeColorAttacking);
				break;
			}
		}
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
			return;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
			return;
		default:
			return;
		}
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001BE8D File Offset: 0x0001A08D
	private void setEyeColor(Color c)
	{
		if (c.a != 0f)
		{
			this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, c);
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001BEC4 File Offset: 0x0001A0C4
	public List<VRRig> GetValidChoosableRigs()
	{
		this.validRigs.Clear();
		foreach (VRRig vrrig in this.playerCollection.containedRigs)
		{
			if ((NetworkSystem.Instance.InRoom || vrrig.isOfflineVRRig) && !(vrrig == null))
			{
				this.validRigs.Add(vrrig);
			}
		}
		return this.validRigs;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001BF50 File Offset: 0x0001A150
	public void SliceUpdate()
	{
		this.wasConnectedToRoom = NetworkSystem.Instance.InRoom;
		this.deltaTime = Time.time - this.lastTime;
		this.lastTime = Time.time;
		this.UpdateClientState();
		if (NetworkSystem.Instance.InRoom && !this.replState.IsMine)
		{
			this.path = null;
			return;
		}
		if (!this.playerCollection.gameObject.activeInHierarchy)
		{
			NetPlayer netPlayer = null;
			float num = float.PositiveInfinity;
			foreach (VRRig vrrig in this.playersInRoomCollection.containedRigs)
			{
				if (!(vrrig == null))
				{
					float num2 = Vector3.Distance(base.transform.position, vrrig.transform.position);
					if (num2 < num)
					{
						netPlayer = vrrig.creator;
						num = num2;
					}
				}
			}
			if (num > 6f)
			{
				return;
			}
			this.path = null;
			if (netPlayer == null)
			{
				return;
			}
			this.replStateRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			this.myRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			return;
		}
		else
		{
			this.ValidateChasingRig();
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.Sleeping();
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.Patrolling();
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.Chasing();
				break;
			case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
				this.ReturnToSleepPt();
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.BeginAttack();
				break;
			case MonkeyeAI_ReplState.EStates.OpenFloor:
				this.OpenFloor();
				break;
			case MonkeyeAI_ReplState.EStates.DropPlayer:
				this.DropPlayer();
				break;
			case MonkeyeAI_ReplState.EStates.CloseFloor:
				this.CloseFloor();
				break;
			}
			if (this.path == null)
			{
				return;
			}
			this.FollowPath();
			this.velocity = base.transform.position - this.prevPosition;
			this.prevPosition = base.transform.position;
			return;
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001C13C File Offset: 0x0001A33C
	private void AntiOverlapAssurance()
	{
		try
		{
			if ((!NetworkSystem.Instance.InRoom || this.replState.IsMine) && this.playerCollection.gameObject.activeInHierarchy)
			{
				foreach (MonkeyeAI monkeyeAI in this.playerCollection.monkeyeAis)
				{
					if (!(monkeyeAI == this) && Vector3.Distance(base.transform.position, monkeyeAI.transform.position) < this.overlapRadius && (double)Vector3.Dot(base.transform.forward, monkeyeAI.transform.forward) > 0.2)
					{
						MonkeyeAI_ReplState.EStates state = this.replState.state;
						if (state != MonkeyeAI_ReplState.EStates.Patrolling)
						{
							if (state == MonkeyeAI_ReplState.EStates.Chasing)
							{
								if (monkeyeAI.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
								{
									this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
								}
							}
						}
						else
						{
							this.PickNewPath(false);
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
		}
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001C264 File Offset: 0x0001A464
	private void SetTargetPlayer([CanBeNull] VRRig rig)
	{
		if (rig == null)
		{
			this.replState.userId = "";
			this.replState.freezePlayer = false;
			this.replState.floorEnabled = true;
			this.replState.portalEnabled = false;
			this.targetRig = null;
			return;
		}
		this.replState.userId = this.UserIdFromRig(rig);
		this.targetRig = rig;
	}

	// Token: 0x0400056A RID: 1386
	public List<Transform> patrolPts;

	// Token: 0x0400056B RID: 1387
	public Transform sleepPt;

	// Token: 0x0400056C RID: 1388
	private int patrolIdx = -1;

	// Token: 0x0400056D RID: 1389
	private int patrolCount;

	// Token: 0x0400056E RID: 1390
	private Vector3 targetPosition;

	// Token: 0x0400056F RID: 1391
	private MaterialPropertyBlock portalMatPropBlock;

	// Token: 0x04000570 RID: 1392
	private MaterialPropertyBlock monkEyeMatPropBlock;

	// Token: 0x04000571 RID: 1393
	private Renderer renderer;

	// Token: 0x04000572 RID: 1394
	private AIDestinationSetter aiDest;

	// Token: 0x04000573 RID: 1395
	private AIPath aiPath;

	// Token: 0x04000574 RID: 1396
	private AILerp aiLerp;

	// Token: 0x04000575 RID: 1397
	private Seeker seeker;

	// Token: 0x04000576 RID: 1398
	private Path path;

	// Token: 0x04000577 RID: 1399
	private int currentWaypoint;

	// Token: 0x04000578 RID: 1400
	private bool calculatingPath;

	// Token: 0x04000579 RID: 1401
	private Monkeye_LazerFX lazerFx;

	// Token: 0x0400057A RID: 1402
	private Animator animController;

	// Token: 0x0400057B RID: 1403
	private RaycastHit[] rayResults = new RaycastHit[1];

	// Token: 0x0400057C RID: 1404
	private LayerMask layerMask;

	// Token: 0x0400057D RID: 1405
	private bool wasConnectedToRoom;

	// Token: 0x0400057E RID: 1406
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x0400057F RID: 1407
	public MazePlayerCollection playerCollection;

	// Token: 0x04000580 RID: 1408
	public PlayerCollection playersInRoomCollection;

	// Token: 0x04000581 RID: 1409
	private List<VRRig> validRigs = new List<VRRig>();

	// Token: 0x04000582 RID: 1410
	public GameObject portalFx;

	// Token: 0x04000583 RID: 1411
	public Transform[] eyeBones;

	// Token: 0x04000584 RID: 1412
	public float speed = 0.1f;

	// Token: 0x04000585 RID: 1413
	public float rotationSpeed = 1f;

	// Token: 0x04000586 RID: 1414
	public float wakeDistance = 1f;

	// Token: 0x04000587 RID: 1415
	public float chaseDistance = 3f;

	// Token: 0x04000588 RID: 1416
	public float sleepDuration = 3f;

	// Token: 0x04000589 RID: 1417
	public float attackDistance = 0.1f;

	// Token: 0x0400058A RID: 1418
	public float beginAttackTime = 1f;

	// Token: 0x0400058B RID: 1419
	public float openFloorTime = 3f;

	// Token: 0x0400058C RID: 1420
	public float dropPlayerTime = 1f;

	// Token: 0x0400058D RID: 1421
	public float closeFloorTime = 1f;

	// Token: 0x0400058E RID: 1422
	public Color portalColor;

	// Token: 0x0400058F RID: 1423
	public Color gorillaPortalColor;

	// Token: 0x04000590 RID: 1424
	public Color monkEyeColor;

	// Token: 0x04000591 RID: 1425
	public Color monkEyeEyeColorNormal;

	// Token: 0x04000592 RID: 1426
	public Color monkEyeEyeColorAttacking;

	// Token: 0x04000593 RID: 1427
	public int maxPatrols = 4;

	// Token: 0x04000594 RID: 1428
	private VRRig targetRig;

	// Token: 0x04000595 RID: 1429
	private float deltaTime;

	// Token: 0x04000596 RID: 1430
	private float lastTime;

	// Token: 0x04000597 RID: 1431
	public MonkeyeAI_ReplState replState;

	// Token: 0x04000598 RID: 1432
	private MonkeyeAI_ReplState.EStates previousState;

	// Token: 0x04000599 RID: 1433
	private RequestableOwnershipGuard replStateRequestableOwnershipGaurd;

	// Token: 0x0400059A RID: 1434
	private RequestableOwnershipGuard myRequestableOwnershipGaurd;

	// Token: 0x0400059B RID: 1435
	private int layerBase;

	// Token: 0x0400059C RID: 1436
	private int layerForward = 1;

	// Token: 0x0400059D RID: 1437
	private int layerLeft = 2;

	// Token: 0x0400059E RID: 1438
	private int layerRight = 3;

	// Token: 0x0400059F RID: 1439
	private static readonly int EmissionColorShaderProp = ShaderProps._EmissionColor;

	// Token: 0x040005A0 RID: 1440
	private static readonly int ColorShaderProp = ShaderProps._BaseColor;

	// Token: 0x040005A1 RID: 1441
	private static readonly int EyeColorShaderProp = ShaderProps._GChannelColor;

	// Token: 0x040005A2 RID: 1442
	private static readonly int tintColorShaderProp = ShaderProps._TintColor;

	// Token: 0x040005A3 RID: 1443
	private static readonly int animStateID = Animator.StringToHash("state");

	// Token: 0x040005A4 RID: 1444
	private Vector3 prevPosition;

	// Token: 0x040005A5 RID: 1445
	private Vector3 velocity;

	// Token: 0x040005A6 RID: 1446
	public AudioSource audioSource;

	// Token: 0x040005A7 RID: 1447
	public AudioClip sleepLoopSound;

	// Token: 0x040005A8 RID: 1448
	public float sleepLoopVolume = 0.5f;

	// Token: 0x040005A9 RID: 1449
	[FormerlySerializedAs("moveLoopSound")]
	public AudioClip patrolLoopSound;

	// Token: 0x040005AA RID: 1450
	public float patrolLoopVolume = 0.5f;

	// Token: 0x040005AB RID: 1451
	public float patrolLoopFadeInTime = 1f;

	// Token: 0x040005AC RID: 1452
	public AudioClip chaseLoopSound;

	// Token: 0x040005AD RID: 1453
	public float chaseLoopVolume = 0.5f;

	// Token: 0x040005AE RID: 1454
	public float chaseLoopFadeInTime = 0.05f;

	// Token: 0x040005AF RID: 1455
	public AudioClip attackSound;

	// Token: 0x040005B0 RID: 1456
	public float attackVolume = 0.5f;

	// Token: 0x040005B1 RID: 1457
	public float overlapRadius;

	// Token: 0x040005B2 RID: 1458
	private bool lockedOn;
}
