using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000063 RID: 99
public class CrittersPawn : CrittersActor, IEyeScannable
{
	// Token: 0x06000231 RID: 561 RVA: 0x0000DA58 File Offset: 0x0000BC58
	public override void Initialize()
	{
		base.Initialize();
		this.rB = base.GetComponentInChildren<Rigidbody>();
		this.soundsHeard = new Dictionary<int, CrittersActor>();
		base.transform.eulerAngles = new Vector3(0f, Random.value * 360f, 0f);
		this.raycastHits = new RaycastHit[20];
		this.wasSomethingInTheWay = false;
		this._spawnAnimationDuration = this.spawnInHeighMovement.keys.Last<Keyframe>().time;
		this._despawnAnimationDuration = this.despawnInHeighMovement.keys.Last<Keyframe>().time;
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000DAF8 File Offset: 0x0000BCF8
	private void InitializeTemplateValues()
	{
		this.sensoryRange *= this.sensoryRange;
		this.autoSeeFoodDistance *= this.autoSeeFoodDistance;
		this.currentSleepiness = Random.value * this.tiredThreshold;
		this.currentHunger = Random.value * this.hungryThreshold;
		this.currentFear = 0f;
		this.currentStruggle = 0f;
		this.currentAttraction = 0f;
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000DB70 File Offset: 0x0000BD70
	public float JumpVelocityForDistanceAtAngle(float horizontalDistance, float angle)
	{
		return Mathf.Min(this.maxJumpVel, Mathf.Sqrt(horizontalDistance * Physics.gravity.magnitude / Mathf.Sin(2f * angle)));
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0000DBA9 File Offset: 0x0000BDA9
	public override void OnEnable()
	{
		base.OnEnable();
		CrittersManager.RegisterCritter(this);
		this.lifeTimeStart = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		EyeScannerMono.Register(this);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0000DBD7 File Offset: 0x0000BDD7
	public override void OnDisable()
	{
		base.OnDisable();
		CrittersManager.DeregisterCritter(this);
		if (this.currentOngoingStateFX.IsNotNull())
		{
			this.currentOngoingStateFX.SetActive(false);
			this.currentOngoingStateFX = null;
		}
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000DC0B File Offset: 0x0000BE0B
	private float GetAdditiveJumpDelay()
	{
		if (this.currentState == CrittersPawn.CreatureState.Running)
		{
			return 0f;
		}
		return Mathf.Max(0f, this.jumpCooldown * Random.value * this.jumpVariabilityTime);
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000DC3C File Offset: 0x0000BE3C
	public void LocalJump(float maxVel, float jumpAngle)
	{
		maxVel *= this.slowSpeedMod;
		this.lastImpulsePosition = base.transform.position;
		this.lastImpulseVelocity = base.transform.forward * (Mathf.Sin(0.017453292f * jumpAngle) * maxVel) + Vector3.up * (Mathf.Cos(0.017453292f * jumpAngle) * maxVel);
		this.lastImpulseTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		this.lastImpulseTime += (double)this.GetAdditiveJumpDelay();
		this.lastImpulseQuaternion = base.transform.rotation;
		this.rB.velocity = this.lastImpulseVelocity;
		this.rb.angularVelocity = Vector3.zero;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000DD08 File Offset: 0x0000BF08
	private bool CanSeeActor(Vector3 actorPosition)
	{
		Vector3 to = actorPosition - base.transform.position;
		return to.sqrMagnitude < this.autoSeeFoodDistance || (to.sqrMagnitude < this.sensoryRange && Vector3.Angle(base.transform.forward, to) < this.visionConeAngle);
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000DD64 File Offset: 0x0000BF64
	private bool IsGrabPossible(CrittersGrabber actor)
	{
		return actor.grabbing && (base.transform.position - actor.grabPosition.position).magnitude < actor.grabDistance;
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000DDA8 File Offset: 0x0000BFA8
	private bool WithinCaptureDistance(CrittersCage actor)
	{
		return (this.bodyCollider.bounds.center - actor.grabPosition.position).magnitude < actor.grabDistance;
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000DDE8 File Offset: 0x0000BFE8
	public bool AwareOfActor(CrittersActor actor)
	{
		CrittersActor.CrittersActorType crittersActorType = actor.crittersActorType;
		switch (crittersActorType)
		{
		case CrittersActor.CrittersActorType.Creature:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.Food:
			return ((CrittersFood)actor).currentFood > 0f && this.CanSeeActor(((CrittersFood)actor).food.transform.position);
		case CrittersActor.CrittersActorType.LoudNoise:
			return (actor.transform.position - base.transform.position).sqrMagnitude < this.sensoryRange;
		case CrittersActor.CrittersActorType.BrightLight:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.Darkness:
		case CrittersActor.CrittersActorType.HidingArea:
		case CrittersActor.CrittersActorType.Disappear:
		case CrittersActor.CrittersActorType.Spawn:
		case CrittersActor.CrittersActorType.Player:
		case CrittersActor.CrittersActorType.AttachPoint:
			break;
		case CrittersActor.CrittersActorType.Grabber:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.Cage:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.FoodSpawner:
			return this.CanSeeActor(actor.transform.position);
		case CrittersActor.CrittersActorType.StunBomb:
			return this.CanSeeActor(actor.transform.position);
		default:
			if (crittersActorType == CrittersActor.CrittersActorType.StickyGoo)
			{
				return ((CrittersStickyGoo)actor).CanAffect(base.transform.position);
			}
			break;
		}
		return false;
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000DF24 File Offset: 0x0000C124
	public override bool ProcessLocal()
	{
		CrittersPawn.CreatureUpdateData creatureUpdateData = new CrittersPawn.CreatureUpdateData(this);
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasSomethingInTheWay = false;
		this.UpdateMoodSourceData();
		this.StuckCheck();
		switch (this.currentState)
		{
		case CrittersPawn.CreatureState.Idle:
			this.IdleStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Eating:
			this.EatingStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.AttractedTo:
			this.AttractedStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Running:
			this.RunningStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Grabbed:
			this.GrabbedStateUpdate();
			break;
		case CrittersPawn.CreatureState.Sleeping:
			this.SleepingStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.SeekingFood:
			this.SeekingFoodStateUpdate();
			this.DespawnCheck();
			break;
		case CrittersPawn.CreatureState.Captured:
			this.CapturedStateUpdate();
			break;
		case CrittersPawn.CreatureState.Stunned:
			this.StunnedStateUpdate();
			break;
		case CrittersPawn.CreatureState.WaitingToDespawn:
			this.WaitingToDespawnStateUpdate();
			break;
		case CrittersPawn.CreatureState.Despawning:
			this.DespawningStateUpdate();
			break;
		case CrittersPawn.CreatureState.Spawning:
			this.SpawningStateUpdate();
			break;
		}
		this.UpdateStateAnim();
		this.updatedSinceLastFrame = (flag || this.updatedSinceLastFrame || !creatureUpdateData.SameData(this));
		return this.updatedSinceLastFrame;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000E04C File Offset: 0x0000C24C
	private void StuckCheck()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (this._nextStuckCheck > (double)realtimeSinceStartup)
		{
			return;
		}
		this._nextStuckCheck = (double)(realtimeSinceStartup + 1f);
		if (!this.canJump && this.rb.IsSleeping())
		{
			this.canJump = true;
		}
		if (base.transform.position.y < this.killHeight)
		{
			this.SetState(CrittersPawn.CreatureState.Despawning);
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000E0B4 File Offset: 0x0000C2B4
	private void DespawnCheck()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (this._nextDespawnCheck > (double)realtimeSinceStartup)
		{
			return;
		}
		this._nextDespawnCheck = (double)(realtimeSinceStartup + 1f);
		bool flag;
		if (this.lifeTime <= 0.0)
		{
			flag = (this.creatureConfiguration != null && !this.creatureConfiguration.ShouldDespawn());
		}
		else
		{
			flag = ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.lifeTimeStart > this.lifeTime);
		}
		if (flag)
		{
			this.SetState(CrittersPawn.CreatureState.WaitingToDespawn);
			this.spawningStartingPosition = base.gameObject.transform.position;
			this.despawnStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000E16E File Offset: 0x0000C36E
	public void SetTemplate(int templateIndex)
	{
		this.TemplateIndex = templateIndex;
		this.UpdateTemplate();
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000E180 File Offset: 0x0000C380
	private void UpdateTemplate()
	{
		if (this.TemplateIndex != this.LastTemplateIndex)
		{
			this.creatureConfiguration = CrittersManager.instance.creatureIndex[this.TemplateIndex];
			if (this.creatureConfiguration != null)
			{
				this.creatureConfiguration.ApplyToCreature(this);
				this.InitializeAttractors();
			}
			this.LastTemplateIndex = this.TemplateIndex;
			this.InitializeTemplateValues();
		}
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000E1F8 File Offset: 0x0000C3F8
	private void InitializeAttractors()
	{
		this.attractedToTypes = new Dictionary<CrittersActor.CrittersActorType, float>();
		this.afraidOfTypes = new Dictionary<CrittersActor.CrittersActorType, float>();
		if (this.attractedToList != null)
		{
			for (int i = 0; i < this.attractedToList.Count; i++)
			{
				this.attractedToTypes.Add(this.attractedToList[i].type, this.attractedToList[i].multiplier);
			}
		}
		if (this.afraidOfList != null)
		{
			for (int j = 0; j < this.afraidOfList.Count; j++)
			{
				this.afraidOfTypes.Add(this.afraidOfList[j].type, this.afraidOfList[j].multiplier);
			}
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000E2B1 File Offset: 0x0000C4B1
	public override void ProcessRemote()
	{
		this.UpdateTemplate();
		base.ProcessRemote();
		this.UpdateStateAnim();
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000E2C8 File Offset: 0x0000C4C8
	public void SetState(CrittersPawn.CreatureState newState)
	{
		if (this.currentState == newState)
		{
			return;
		}
		if (this.currentState == CrittersPawn.CreatureState.Captured)
		{
			base.transform.localScale = Vector3.one;
		}
		this.ClearOngoingStateFX();
		this.currentState = newState;
		if (newState != CrittersPawn.CreatureState.Despawning)
		{
			if (newState == CrittersPawn.CreatureState.Spawning && CrittersManager.instance.LocalAuthority())
			{
				this.spawningStartingPosition = base.gameObject.transform.position;
				this.spawnStartTime = (double)(PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time);
			}
		}
		else if (CrittersManager.instance.LocalAuthority())
		{
			this.spawningStartingPosition = base.gameObject.transform.position;
			this.despawnStartTime = (double)(PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time);
		}
		this.StartOngoingStateFX(newState);
		GameObject valueOrDefault = this.StartStateFX.GetValueOrDefault(this.currentState);
		if (valueOrDefault.IsNotNull())
		{
			GameObject pooled = CrittersPool.GetPooled(valueOrDefault);
			if (pooled != null)
			{
				pooled.transform.position = base.transform.position;
			}
		}
		this.currentAnimTime = 0f;
		CrittersAnim crittersAnim;
		if (this.stateAnim.TryGetValue(this.currentState, out crittersAnim))
		{
			this.currentAnim = crittersAnim;
		}
		else
		{
			this.currentAnim = null;
			this.animTarget.localPosition = Vector3.zero;
			this.animTarget.localScale = Vector3.one;
		}
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0000E43C File Offset: 0x0000C63C
	private void ClearOngoingStateFX()
	{
		if (this.currentOngoingStateFX.IsNotNull())
		{
			CrittersPool.Return(this.currentOngoingStateFX);
			this.currentOngoingStateFX = null;
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000E460 File Offset: 0x0000C660
	private void StartOngoingStateFX(CrittersPawn.CreatureState state)
	{
		GameObject valueOrDefault = this.OngoingStateFX.GetValueOrDefault(state);
		if (valueOrDefault.IsNotNull())
		{
			this.currentOngoingStateFX = CrittersPool.GetPooled(valueOrDefault);
			if (this.currentOngoingStateFX.IsNotNull())
			{
				this.currentOngoingStateFX.transform.SetParent(base.transform, false);
				this.currentOngoingStateFX.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000E4C8 File Offset: 0x0000C6C8
	[Conditional("UNITY_EDITOR")]
	public void UpdateStateColor()
	{
		switch (this.currentState)
		{
		case CrittersPawn.CreatureState.Idle:
			this.debugStateIndicator.material.color = this.debugColorIdle;
			return;
		case CrittersPawn.CreatureState.Eating:
			this.debugStateIndicator.material.color = this.debugColorEating;
			return;
		case CrittersPawn.CreatureState.AttractedTo:
			this.debugStateIndicator.material.color = this.debugColorAttracted;
			return;
		case CrittersPawn.CreatureState.Running:
			this.debugStateIndicator.material.color = this.debugColorScared;
			return;
		case CrittersPawn.CreatureState.Grabbed:
			this.debugStateIndicator.material.color = this.debugColorCaught;
			return;
		case CrittersPawn.CreatureState.Sleeping:
			this.debugStateIndicator.material.color = this.debugColorSleeping;
			return;
		case CrittersPawn.CreatureState.SeekingFood:
			this.debugStateIndicator.material.color = this.debugColorSeekingFood;
			return;
		case CrittersPawn.CreatureState.Captured:
			this.debugStateIndicator.material.color = this.debugColorCaged;
			return;
		case CrittersPawn.CreatureState.Stunned:
			this.debugStateIndicator.material.color = this.debugColorStunned;
			return;
		default:
			this.debugStateIndicator.material.color = new Color(1f, 0f, 1f);
			return;
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000E600 File Offset: 0x0000C800
	public void UpdateStateAnim()
	{
		if (this.currentAnim != null)
		{
			this.currentAnimTime += Time.deltaTime * this.currentAnim.playSpeed;
			this.currentAnimTime %= 1f;
			float num = this.currentAnim.squashAmount.Evaluate(this.currentAnimTime);
			float z = this.currentAnim.forwardOffset.Evaluate(this.currentAnimTime);
			float x = this.currentAnim.horizontalOffset.Evaluate(this.currentAnimTime);
			float y = this.currentAnim.verticalOffset.Evaluate(this.currentAnimTime);
			this.animTarget.localPosition = new Vector3(x, y, z);
			float num2 = 1f - num;
			num2 *= 0.5f;
			num2 += 1f;
			this.animTarget.localScale = new Vector3(num2, num, num2);
		}
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000E6EC File Offset: 0x0000C8EC
	public void IdleStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.AboveAttractedThreshold() && (!this.AboveHungryThreshold() || !CrittersManager.AnyFoodNearby(this)))
		{
			this.SetState(CrittersPawn.CreatureState.AttractedTo);
			return;
		}
		if (this.AboveHungryThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.SeekingFood);
			return;
		}
		if (this.AboveSleepyThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Sleeping);
			return;
		}
		if (this.CanJump())
		{
			this.RandomJump();
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000E758 File Offset: 0x0000C958
	public void EatingStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.BelowNotHungryThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
			return;
		}
		if (!this.withinEatingRadius || this.eatingTarget.IsNull() || this.eatingTarget.currentFood <= 0f)
		{
			this.SetState(CrittersPawn.CreatureState.SeekingFood);
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000E7B3 File Offset: 0x0000C9B3
	public void SleepingStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.BelowNotSleepyThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000E7D4 File Offset: 0x0000C9D4
	public void AttractedStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.BelowUnAttractedThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
			return;
		}
		if (this.CanJump())
		{
			if (this.AboveHungryThreshold() && CrittersManager.AnyFoodNearby(this))
			{
				this.SetState(CrittersPawn.CreatureState.SeekingFood);
				return;
			}
			if (CrittersManager.instance.awareOfActors[this].Contains(this.attractionTarget))
			{
				this.lastSeenAttractionPosition = this.attractionTarget.transform.position;
			}
			this.JumpTowards(this.lastSeenAttractionPosition);
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0000E864 File Offset: 0x0000CA64
	public void RunningStateUpdate()
	{
		if (this.CanJump())
		{
			if (CrittersManager.instance.awareOfActors[this].Contains(this.fearTarget))
			{
				this.lastSeenFearPosition = this.fearTarget.transform.position;
			}
			this.JumpAwayFrom(this.lastSeenFearPosition);
		}
		if (this.BelowNotAfraidThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000E8CC File Offset: 0x0000CACC
	public void SeekingFoodStateUpdate()
	{
		if (this.AboveFearThreshold())
		{
			this.SetState(CrittersPawn.CreatureState.Running);
			return;
		}
		if (this.CanJump())
		{
			if (CrittersManager.CritterAwareOfAny(this))
			{
				this.eatingTarget = CrittersManager.ClosestFood(this);
				if (this.eatingTarget != null)
				{
					this.withinEatingRadius = ((this.eatingTarget.food.transform.position - base.transform.position).sqrMagnitude < this.eatingRadiusMaxSquared);
					if (!this.withinEatingRadius)
					{
						this.JumpTowards(this.eatingTarget.food.transform.position);
						return;
					}
					base.transform.forward = (this.eatingTarget.food.transform.position - base.transform.position).X_Z().normalized;
					this.SetState(CrittersPawn.CreatureState.Eating);
					this.debugStateIndicator.material.color = this.debugColorEating;
					return;
				}
				else
				{
					if (this.AboveAttractedThreshold())
					{
						this.SetState(CrittersPawn.CreatureState.AttractedTo);
						return;
					}
					this.RandomJump();
					return;
				}
			}
			else
			{
				this.RandomJump();
			}
		}
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000E9F4 File Offset: 0x0000CBF4
	public void GrabbedStateUpdate()
	{
		if (this.currentState == CrittersPawn.CreatureState.Grabbed && this.grabbedTarget != null)
		{
			if (this.currentStruggle >= this.escapeThreshold || !this.grabbedTarget.grabbing)
			{
				this.Released(true, default(Quaternion), default(Vector3), default(Vector3), default(Vector3));
				return;
			}
		}
		else if (this.grabbedTarget == null)
		{
			this.Released(true, default(Quaternion), default(Vector3), default(Vector3), default(Vector3));
		}
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000EA98 File Offset: 0x0000CC98
	protected override void HandleRemoteReleased()
	{
		base.HandleRemoteReleased();
		if (this.cageTarget.IsNotNull())
		{
			this.fearTarget = this.cageTarget;
			this.cageTarget.SetHasCritter(false);
			this.cageTarget = null;
		}
		if (this.grabbedTarget.IsNotNull())
		{
			this.fearTarget = this.grabbedTarget;
			this.grabbedTarget = null;
			if (this.OnReleasedFX)
			{
				CrittersPool.GetPooled(this.OnReleasedFX).transform.position = base.transform.position;
			}
		}
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000EB24 File Offset: 0x0000CD24
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulse = default(Vector3), Vector3 impulseRotation = default(Vector3))
	{
		base.Released(keepWorldPosition, rotation, position, impulse, impulseRotation);
		if (this.currentState != CrittersPawn.CreatureState.Grabbed && this.currentState != CrittersPawn.CreatureState.Captured)
		{
			return;
		}
		if (this.grabbedTarget.IsNotNull() && this.grabbedTarget.grabbedActors.Contains(this))
		{
			this.grabbedTarget.grabbedActors.Remove(this);
		}
		if (this.currentState == CrittersPawn.CreatureState.Grabbed)
		{
			this.fearTarget = this.grabbedTarget;
			this.grabbedTarget = null;
			if (this.OnReleasedFX)
			{
				CrittersPool.GetPooled(this.OnReleasedFX).transform.position = base.transform.position;
			}
		}
		else if (this.currentState == CrittersPawn.CreatureState.Captured)
		{
			base.transform.localScale = Vector3.one;
			this.fearTarget = this.cageTarget;
			this.cageTarget.SetHasCritter(false);
			this.cageTarget = null;
		}
		if (this.struggleGainedPerSecond > 0f)
		{
			this.currentFear = this.maxFear;
			this.SetState(CrittersPawn.CreatureState.Running);
			this.lastSeenFearPosition = this.fearTarget.transform.position;
			return;
		}
		this.currentFear = 0f;
		this.SetState(CrittersPawn.CreatureState.Idle);
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000EC50 File Offset: 0x0000CE50
	public void CapturedStateUpdate()
	{
		if (this.cageTarget.IsNull())
		{
			this.cageTarget = (CrittersCage)CrittersManager.instance.actorById[this.actorIdTarget];
			this.cageTarget.SetHasCritter(false);
		}
		if (this.cageTarget.inReleasingPosition && this.cageTarget.heldByPlayer)
		{
			this.Released(true, default(Quaternion), default(Vector3), default(Vector3), default(Vector3));
		}
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000ECDD File Offset: 0x0000CEDD
	public void StunnedStateUpdate()
	{
		this.remainingStunnedTime = Mathf.Max(0f, this.remainingStunnedTime - Time.deltaTime);
		if (this.remainingStunnedTime <= 0f)
		{
			this.currentFear = this.maxFear;
			this.SetState(CrittersPawn.CreatureState.Running);
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000ED1C File Offset: 0x0000CF1C
	public void WaitingToDespawnStateUpdate()
	{
		if (Mathf.FloorToInt(this.rb.velocity.magnitude * 10f) == 0)
		{
			this.SetState(CrittersPawn.CreatureState.Despawning);
		}
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000ED54 File Offset: 0x0000CF54
	public void DespawningStateUpdate()
	{
		this._despawnAnimTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.despawnStartTime;
		if (this._despawnAnimTime >= (double)this._despawnAnimationDuration)
		{
			base.gameObject.SetActive(false);
			this.TemplateIndex = -1;
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
	public void SpawningStateUpdate()
	{
		this._spawnAnimTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.spawnStartTime;
		base.MoveActor(this.spawningStartingPosition + new Vector3(0f, this.spawnInHeighMovement.Evaluate(Mathf.Clamp((float)this._spawnAnimTime, 0f, this._spawnAnimationDuration)), 0f), base.transform.rotation, false, true, true);
		if (this._spawnAnimTime >= (double)this._spawnAnimationDuration)
		{
			this.SetState(CrittersPawn.CreatureState.Idle);
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000EE38 File Offset: 0x0000D038
	public void UpdateMoodSourceData()
	{
		this.UpdateHunger();
		this.UpdateFearAndAttraction();
		this.UpdateSleepiness();
		this.UpdateStruggle();
		this.UpdateSlowed();
		this.UpdateGrabbed();
		this.UpdateCaged();
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000EE64 File Offset: 0x0000D064
	public void UpdateHunger()
	{
		if (this.currentState == CrittersPawn.CreatureState.Eating && !this.eatingTarget.IsNull())
		{
			this.eatingTarget.Feed(this.hungerLostPerSecond * Time.deltaTime);
			this.currentHunger = Mathf.Max(0f, this.currentHunger - this.hungerLostPerSecond * Time.deltaTime);
			return;
		}
		this.currentHunger = Mathf.Min(this.maxHunger, this.currentHunger + this.hungerGainedPerSecond * Time.deltaTime);
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000EEE8 File Offset: 0x0000D0E8
	public void UpdateFearAndAttraction()
	{
		if (this.currentState == CrittersPawn.CreatureState.Spawning)
		{
			return;
		}
		this.currentFear = Mathf.Max(0f, this.currentFear - this.fearLostPerSecond * Time.deltaTime);
		this.currentAttraction = Mathf.Max(0f, this.currentAttraction - this.attractionLostPerSecond * Time.deltaTime);
		for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
		{
			CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
			float multiplier;
			float multiplier2;
			if (this.afraidOfTypes != null && this.afraidOfTypes.TryGetValue(crittersActor.crittersActorType, out multiplier))
			{
				crittersActor.CalculateFear(this, multiplier);
			}
			else if (this.attractedToTypes != null && this.attractedToTypes.TryGetValue(crittersActor.crittersActorType, out multiplier2))
			{
				crittersActor.CalculateAttraction(this, multiplier2);
			}
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x0000EFD0 File Offset: 0x0000D1D0
	public void IncreaseFear(float fearAmount, CrittersActor actor)
	{
		if (fearAmount > 0f)
		{
			this.currentFear += fearAmount;
			this.currentFear = Mathf.Min(this.maxFear, this.currentFear);
			this.fearTarget = actor;
			this.lastSeenFearPosition = this.fearTarget.transform.position;
		}
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0000F028 File Offset: 0x0000D228
	public void IncreaseAttraction(float attractionAmount, CrittersActor actor)
	{
		if (attractionAmount > 0f)
		{
			this.currentAttraction += attractionAmount;
			this.currentAttraction = Mathf.Min(this.maxAttraction, this.currentAttraction);
			this.attractionTarget = actor;
			this.lastSeenAttractionPosition = this.attractionTarget.transform.position;
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000F080 File Offset: 0x0000D280
	public void UpdateSleepiness()
	{
		if (this.currentState == CrittersPawn.CreatureState.Sleeping)
		{
			this.currentSleepiness = Mathf.Max(0f, this.currentSleepiness - Time.deltaTime * this.sleepinessLostPerSecond);
			return;
		}
		this.currentSleepiness = Mathf.Min(this.maxSleepiness, this.currentSleepiness + Time.deltaTime * this.sleepinessGainedPerSecond);
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000F0E0 File Offset: 0x0000D2E0
	public void UpdateStruggle()
	{
		if (this.currentState == CrittersPawn.CreatureState.Grabbed)
		{
			this.currentStruggle = Mathf.Clamp(this.currentStruggle + this.struggleGainedPerSecond * Time.deltaTime, 0f, this.maxStruggle);
			return;
		}
		this.currentStruggle = Mathf.Max(0f, this.currentStruggle - this.struggleLostPerSecond * Time.deltaTime);
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000F144 File Offset: 0x0000D344
	private void UpdateSlowed()
	{
		if (this.remainingSlowedTime > 0f)
		{
			this.remainingSlowedTime -= Time.deltaTime;
			if (this.remainingSlowedTime < 0f)
			{
				this.slowSpeedMod = 1f;
				return;
			}
		}
		else if (this.currentState != CrittersPawn.CreatureState.Captured && this.currentState != CrittersPawn.CreatureState.Despawning && this.currentState != CrittersPawn.CreatureState.Grabbed && this.currentState != CrittersPawn.CreatureState.WaitingToDespawn && this.currentState != CrittersPawn.CreatureState.Spawning)
		{
			for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
			{
				CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
				if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.StickyGoo)
				{
					CrittersStickyGoo crittersStickyGoo = crittersActor as CrittersStickyGoo;
					this.slowSpeedMod = crittersStickyGoo.slowModifier;
					this.remainingSlowedTime = crittersStickyGoo.slowDuration;
					crittersStickyGoo.EffectApplied(this);
				}
			}
		}
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0000F230 File Offset: 0x0000D430
	public void UpdateGrabbed()
	{
		if (this.currentState == CrittersPawn.CreatureState.Grabbed || this.currentState == CrittersPawn.CreatureState.Captured)
		{
			return;
		}
		for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
		{
			CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber && !crittersActor.isOnPlayer && this.IsGrabPossible((CrittersGrabber)crittersActor))
			{
				this.GrabbedBy(crittersActor, true, default(Quaternion), default(Vector3), false);
			}
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000F2C8 File Offset: 0x0000D4C8
	public void UpdateCaged()
	{
		if (this.currentState == CrittersPawn.CreatureState.Captured)
		{
			return;
		}
		for (int i = 0; i < CrittersManager.instance.awareOfActors[this].Count; i++)
		{
			CrittersActor crittersActor = CrittersManager.instance.awareOfActors[this][i];
			CrittersCage crittersCage = crittersActor as CrittersCage;
			if (crittersActor.crittersActorType == CrittersActor.CrittersActorType.Cage && crittersCage.IsNotNull() && crittersCage.CanCatch && this.WithinCaptureDistance(crittersCage))
			{
				this.GrabbedBy(crittersActor, true, crittersCage.cagePosition.localRotation, crittersCage.cagePosition.localPosition, false);
			}
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000F364 File Offset: 0x0000D564
	public void RandomJump()
	{
		for (int i = 0; i < 5; i++)
		{
			base.transform.eulerAngles = new Vector3(0f, 360f * Random.value, 0f);
			if (!this.SomethingInTheWay(default(Vector3)))
			{
				break;
			}
		}
		this.LocalJump(this.maxJumpVel, 45f);
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0000F3C4 File Offset: 0x0000D5C4
	public void JumpTowards(Vector3 targetPos)
	{
		if (this.SomethingInTheWay((targetPos - base.transform.position).X_Z()))
		{
			this.RandomJump();
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetPos - base.transform.position, Vector3.up), Vector3.up);
		this.LocalJump(this.JumpVelocityForDistanceAtAngle(Vector3.ProjectOnPlane(targetPos - base.transform.position, Vector3.up).magnitude * this.fudge, 45f), 45f);
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0000F468 File Offset: 0x0000D668
	public void JumpAwayFrom(Vector3 targetPos)
	{
		Vector3 vector = (base.transform.position - targetPos).X_Z();
		if (vector == Vector3.zero)
		{
			vector = base.transform.forward;
		}
		Vector3 vector2 = Quaternion.Euler(0f, (float)Random.Range(-30, 30), 0f) * vector;
		if (this.SomethingInTheWay(vector2))
		{
			this.RandomJump();
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(vector2, Vector3.up);
		this.LocalJump(this.maxJumpVel, 45f);
	}

	// Token: 0x06000263 RID: 611 RVA: 0x0000F4FC File Offset: 0x0000D6FC
	public bool SomethingInTheWay(Vector3 direction = default(Vector3))
	{
		if (direction == default(Vector3))
		{
			direction = base.transform.forward;
		}
		bool flag = Physics.RaycastNonAlloc(this.bodyCollider.bounds.center, direction, this.raycastHits, this.obstacleSeeDistance, CrittersManager.instance.movementLayers) > 0;
		this.wasSomethingInTheWay = (this.wasSomethingInTheWay || flag);
		return flag;
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000F570 File Offset: 0x0000D770
	public override bool CanBeGrabbed(CrittersActor grabbedBy)
	{
		return this.currentState != CrittersPawn.CreatureState.Captured && base.CanBeGrabbed(grabbedBy);
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0000F584 File Offset: 0x0000D784
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		CrittersActor.CrittersActorType crittersActorType = grabbingActor.crittersActorType;
		if (crittersActorType == CrittersActor.CrittersActorType.Grabber)
		{
			this.SetState(CrittersPawn.CreatureState.Grabbed);
			this.grabbedTarget = (CrittersGrabber)grabbingActor;
			this.actorIdTarget = this.grabbedTarget.actorId;
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			return;
		}
		if (crittersActorType != CrittersActor.CrittersActorType.Cage)
		{
			return;
		}
		this.SetState(CrittersPawn.CreatureState.Captured);
		this.cageTarget = (CrittersCage)grabbingActor;
		this.cageTarget.SetHasCritter(true);
		this.actorIdTarget = this.cageTarget.actorId;
		if (CrittersManager.instance.LocalAuthority())
		{
			base.transform.localScale = this.cageTarget.critterScale;
		}
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000F638 File Offset: 0x0000D838
	protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		base.RemoteGrabbedBy(grabbingActor);
		CrittersActor.CrittersActorType crittersActorType = grabbingActor.crittersActorType;
		if (crittersActorType != CrittersActor.CrittersActorType.Grabber)
		{
			if (crittersActorType == CrittersActor.CrittersActorType.Cage)
			{
				this.cageTarget = (CrittersCage)grabbingActor;
				this.cageTarget.SetHasCritter(true);
				this.actorIdTarget = this.cageTarget.actorId;
				if (CrittersManager.instance.LocalAuthority())
				{
					base.transform.localScale = this.cageTarget.critterScale;
					return;
				}
			}
		}
		else
		{
			this.grabbedTarget = (CrittersGrabber)grabbingActor;
			this.actorIdTarget = this.grabbedTarget.actorId;
		}
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000F6C8 File Offset: 0x0000D8C8
	public void Stunned(float duration)
	{
		if (this.currentState == CrittersPawn.CreatureState.Captured || this.currentState == CrittersPawn.CreatureState.Grabbed || this.currentState == CrittersPawn.CreatureState.Despawning || this.currentState == CrittersPawn.CreatureState.WaitingToDespawn)
		{
			return;
		}
		this.remainingStunnedTime = duration;
		this.SetState(CrittersPawn.CreatureState.Stunned);
		this.updatedSinceLastFrame = true;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000F706 File Offset: 0x0000D906
	public bool AboveFearThreshold()
	{
		return this.currentFear >= this.scaredThreshold;
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000F719 File Offset: 0x0000D919
	public bool BelowNotAfraidThreshold()
	{
		return this.currentFear < this.calmThreshold;
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0000F729 File Offset: 0x0000D929
	public bool AboveAttractedThreshold()
	{
		return this.currentAttraction >= this.attractedThreshold;
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000F73C File Offset: 0x0000D93C
	public bool BelowUnAttractedThreshold()
	{
		return this.currentAttraction < this.unattractedThreshold;
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000F74C File Offset: 0x0000D94C
	public bool AboveHungryThreshold()
	{
		return this.currentHunger >= this.hungryThreshold;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000F75F File Offset: 0x0000D95F
	public bool BelowNotHungryThreshold()
	{
		return this.currentHunger < this.satiatedThreshold;
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0000F76F File Offset: 0x0000D96F
	public bool AboveSleepyThreshold()
	{
		return this.currentSleepiness >= this.tiredThreshold;
	}

	// Token: 0x0600026F RID: 623 RVA: 0x0000F782 File Offset: 0x0000D982
	public bool BelowNotSleepyThreshold()
	{
		return this.currentSleepiness < this.awakeThreshold;
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0000F794 File Offset: 0x0000D994
	public bool CanJump()
	{
		if (!this.canJump)
		{
			return false;
		}
		float num;
		if (this.currentState == CrittersPawn.CreatureState.Running)
		{
			num = this.scaredJumpCooldown;
		}
		else
		{
			num = this.jumpCooldown;
		}
		float num2 = PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time;
		if (this.lastImpulseTime > (double)(num2 + this.jumpCooldown + this.jumpVariabilityTime))
		{
			this.lastImpulseTime = (double)(num2 + this.GetAdditiveJumpDelay());
		}
		return (double)num2 > this.lastImpulseTime + (double)num;
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000F80D File Offset: 0x0000DA0D
	public void OnCollisionEnter(Collision collision)
	{
		this.canJump = true;
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000F816 File Offset: 0x0000DA16
	public void OnCollisionExit(Collision collision)
	{
		this.canJump = false;
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0000F81F File Offset: 0x0000DA1F
	public void SetVelocity(Vector3 linearVelocity)
	{
		this.rb.velocity = linearVelocity;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0000F830 File Offset: 0x0000DA30
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(Mathf.FloorToInt(this.currentFear));
		objList.Add(Mathf.FloorToInt(this.currentHunger));
		objList.Add(Mathf.FloorToInt(this.currentSleepiness));
		objList.Add(Mathf.FloorToInt(this.currentStruggle));
		objList.Add(this.currentState);
		objList.Add(this.actorIdTarget);
		objList.Add(this.lifeTimeStart);
		objList.Add(this.TemplateIndex);
		objList.Add(Mathf.FloorToInt(this.remainingStunnedTime));
		objList.Add(this.spawnStartTime);
		objList.Add(this.despawnStartTime);
		objList.AddRange(this.visuals.Appearance.WriteToRPCData());
		return this.TotalActorDataLength();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000F944 File Offset: 0x0000DB44
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 11 + CritterAppearance.DataLength();
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000F958 File Offset: 0x0000DB58
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		int num2;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 1], out num2))
		{
			return this.TotalActorDataLength();
		}
		int num3;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 2], out num3))
		{
			return this.TotalActorDataLength();
		}
		int num4;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 3], out num4))
		{
			return this.TotalActorDataLength();
		}
		int num5;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 4], out num5))
		{
			return this.TotalActorDataLength();
		}
		if (!Enum.IsDefined(typeof(CrittersPawn.CreatureState), (CrittersPawn.CreatureState)num5))
		{
			return this.TotalActorDataLength();
		}
		int num6;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 5], out num6))
		{
			return this.TotalActorDataLength();
		}
		double value;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 6], out value))
		{
			return this.TotalActorDataLength();
		}
		int templateIndex;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 7], out templateIndex))
		{
			return this.TotalActorDataLength();
		}
		int num7;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex + 8], out num7))
		{
			return this.TotalActorDataLength();
		}
		double value2;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 9], out value2))
		{
			return this.TotalActorDataLength();
		}
		double value3;
		if (!CrittersManager.ValidateDataType<double>(data[startingIndex + 10], out value3))
		{
			return this.TotalActorDataLength();
		}
		this.currentFear = (float)num;
		this.currentHunger = (float)num2;
		this.currentSleepiness = (float)num3;
		this.currentStruggle = (float)num4;
		this.SetState((CrittersPawn.CreatureState)num5);
		this.actorIdTarget = num6;
		this.lifeTimeStart = value.GetFinite();
		this.TemplateIndex = templateIndex;
		this.remainingStunnedTime = (float)num7;
		this.spawnStartTime = value2.GetFinite();
		this.despawnStartTime = value3.GetFinite();
		CrittersActor crittersActor = null;
		CrittersPawn.CreatureState creatureState = this.currentState;
		if (creatureState != CrittersPawn.CreatureState.Grabbed)
		{
			if (creatureState != CrittersPawn.CreatureState.Captured)
			{
				this.grabbedTarget = null;
				this.cageTarget = null;
			}
			else
			{
				if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
				{
					this.cageTarget = (CrittersCage)crittersActor;
					if (this.cageTarget != null)
					{
						base.transform.localScale = this.cageTarget.critterScale;
					}
				}
				this.grabbedTarget = null;
			}
		}
		else
		{
			if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
			{
				this.grabbedTarget = (CrittersGrabber)crittersActor;
			}
			this.cageTarget = null;
		}
		this.UpdateTemplate();
		this.visuals.SetAppearance(CritterAppearance.ReadFromRPCData(RuntimeHelpers.GetSubArray<object>(data, Range.StartAt(startingIndex + 11))));
		return this.TotalActorDataLength();
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000FBC0 File Offset: 0x0000DDC0
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		int num;
		int num2;
		int num3;
		int num4;
		int num5;
		int num6;
		double num7;
		int templateIndex;
		int num8;
		double num9;
		double num10;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num2) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num3) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num4) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num5) & Enum.IsDefined(typeof(CrittersPawn.CreatureState), (CrittersPawn.CreatureState)num5) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num6) & CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num7) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out templateIndex) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num8) & CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num9) & CrittersManager.ValidateDataType<double>(stream.ReceiveNext(), out num10)))
		{
			return false;
		}
		this.currentFear = (float)num;
		this.currentHunger = (float)num2;
		this.currentSleepiness = (float)num3;
		this.currentStruggle = (float)num4;
		this.SetState((CrittersPawn.CreatureState)num5);
		this.actorIdTarget = num6;
		this.lifeTimeStart = num7;
		this.TemplateIndex = templateIndex;
		this.remainingStunnedTime = (float)num8;
		this.spawnStartTime = num9;
		this.despawnStartTime = num10;
		this.UpdateTemplate();
		CrittersActor crittersActor = null;
		CrittersPawn.CreatureState creatureState = this.currentState;
		if (creatureState != CrittersPawn.CreatureState.Grabbed)
		{
			if (creatureState != CrittersPawn.CreatureState.Captured)
			{
				this.grabbedTarget = null;
				this.cageTarget = null;
			}
			else
			{
				if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
				{
					this.cageTarget = (CrittersCage)crittersActor;
					if (this.cageTarget != null)
					{
						base.transform.localScale = this.cageTarget.critterScale;
					}
				}
				this.grabbedTarget = null;
			}
		}
		else
		{
			if (CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
			{
				this.grabbedTarget = (CrittersGrabber)crittersActor;
			}
			this.cageTarget = null;
		}
		return true;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0000FD98 File Offset: 0x0000DF98
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(Mathf.FloorToInt(this.currentFear));
		stream.SendNext(Mathf.FloorToInt(this.currentHunger));
		stream.SendNext(Mathf.FloorToInt(this.currentSleepiness));
		stream.SendNext(Mathf.FloorToInt(this.currentStruggle));
		stream.SendNext(this.currentState);
		stream.SendNext(this.actorIdTarget);
		stream.SendNext(this.lifeTimeStart);
		stream.SendNext(this.TemplateIndex);
		stream.SendNext(Mathf.FloorToInt(this.remainingStunnedTime));
		stream.SendNext(this.spawnStartTime);
		stream.SendNext(this.despawnStartTime);
	}

	// Token: 0x06000279 RID: 633 RVA: 0x00002628 File Offset: 0x00000828
	public void SetConfiguration(CritterConfiguration getRandomConfiguration)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000FE80 File Offset: 0x0000E080
	public void SetSpawnData(object[] spawnData)
	{
		this.visuals.SetAppearance(CritterAppearance.ReadFromRPCData(spawnData));
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x0600027B RID: 635 RVA: 0x0000FE93 File Offset: 0x0000E093
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x0600027C RID: 636 RVA: 0x0000FEA0 File Offset: 0x0000E0A0
	Vector3 IEyeScannable.Position
	{
		get
		{
			return this.bodyCollider.bounds.center;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600027D RID: 637 RVA: 0x0000FEC0 File Offset: 0x0000E0C0
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this.bodyCollider.bounds;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x0600027E RID: 638 RVA: 0x0000FECD File Offset: 0x0000E0CD
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.BuildEyeScannerData();
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0000FED8 File Offset: 0x0000E0D8
	private IList<KeyValueStringPair> BuildEyeScannerData()
	{
		this.eyeScanData[0] = new KeyValueStringPair("Name", this.creatureConfiguration.critterName);
		this.eyeScanData[1] = new KeyValueStringPair("Type", this.creatureConfiguration.animalType.ToString());
		this.eyeScanData[2] = new KeyValueStringPair("Temperament", this.creatureConfiguration.behaviour.temperament);
		this.eyeScanData[3] = new KeyValueStringPair("Habitat", this.creatureConfiguration.biome.GetHabitatDescription());
		this.eyeScanData[4] = new KeyValueStringPair("Size", this.visuals.Appearance.size.ToString("0.00"));
		this.eyeScanData[5] = new KeyValueStringPair("State", this.GetCurrentStateName());
		return this.eyeScanData;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000FFD4 File Offset: 0x0000E1D4
	private string GetCurrentStateName()
	{
		string text;
		switch (this.currentState)
		{
		case CrittersPawn.CreatureState.Idle:
			text = "Adventuring";
			break;
		case CrittersPawn.CreatureState.Eating:
			text = "Eating";
			break;
		case CrittersPawn.CreatureState.AttractedTo:
			text = "Curious";
			break;
		case CrittersPawn.CreatureState.Running:
			text = "Scared";
			break;
		case CrittersPawn.CreatureState.Grabbed:
			text = ((this.struggleGainedPerSecond > 0f) ? "Struggling" : "Happy");
			break;
		case CrittersPawn.CreatureState.Sleeping:
			text = "Sleeping";
			break;
		case CrittersPawn.CreatureState.SeekingFood:
			text = "Foraging";
			break;
		case CrittersPawn.CreatureState.Captured:
			text = "Captured";
			break;
		case CrittersPawn.CreatureState.Stunned:
			text = "Stunned";
			break;
		default:
			text = "Contemplating Life";
			break;
		}
		string text2 = text;
		if (this.slowSpeedMod < 1f)
		{
			text2 = "Slowed, " + text2;
		}
		return text2;
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000281 RID: 641 RVA: 0x00010094 File Offset: 0x0000E294
	// (remove) Token: 0x06000282 RID: 642 RVA: 0x000100CC File Offset: 0x0000E2CC
	public event Action OnDataChange;

	// Token: 0x0400028E RID: 654
	[NonSerialized]
	public CritterConfiguration creatureConfiguration;

	// Token: 0x0400028F RID: 655
	public Collider bodyCollider;

	// Token: 0x04000290 RID: 656
	[HideInInspector]
	[NonSerialized]
	public float maxJumpVel;

	// Token: 0x04000291 RID: 657
	[HideInInspector]
	[NonSerialized]
	public float jumpCooldown;

	// Token: 0x04000292 RID: 658
	[HideInInspector]
	[NonSerialized]
	public float scaredJumpCooldown;

	// Token: 0x04000293 RID: 659
	[HideInInspector]
	[NonSerialized]
	public float jumpVariabilityTime;

	// Token: 0x04000294 RID: 660
	[HideInInspector]
	[NonSerialized]
	public float visionConeAngle;

	// Token: 0x04000295 RID: 661
	[HideInInspector]
	[NonSerialized]
	public float sensoryRange;

	// Token: 0x04000296 RID: 662
	[HideInInspector]
	[NonSerialized]
	public float maxHunger;

	// Token: 0x04000297 RID: 663
	[HideInInspector]
	[NonSerialized]
	public float hungryThreshold;

	// Token: 0x04000298 RID: 664
	[HideInInspector]
	[NonSerialized]
	public float satiatedThreshold;

	// Token: 0x04000299 RID: 665
	[HideInInspector]
	[NonSerialized]
	public float hungerLostPerSecond;

	// Token: 0x0400029A RID: 666
	[HideInInspector]
	[NonSerialized]
	public float hungerGainedPerSecond;

	// Token: 0x0400029B RID: 667
	[HideInInspector]
	[NonSerialized]
	public float maxFear;

	// Token: 0x0400029C RID: 668
	[HideInInspector]
	[NonSerialized]
	public float scaredThreshold;

	// Token: 0x0400029D RID: 669
	[HideInInspector]
	[NonSerialized]
	public float calmThreshold;

	// Token: 0x0400029E RID: 670
	[HideInInspector]
	[NonSerialized]
	public float fearLostPerSecond;

	// Token: 0x0400029F RID: 671
	[NonSerialized]
	public float maxAttraction;

	// Token: 0x040002A0 RID: 672
	[NonSerialized]
	public float attractedThreshold;

	// Token: 0x040002A1 RID: 673
	[NonSerialized]
	public float unattractedThreshold;

	// Token: 0x040002A2 RID: 674
	[NonSerialized]
	public float attractionLostPerSecond;

	// Token: 0x040002A3 RID: 675
	[HideInInspector]
	[NonSerialized]
	public float maxSleepiness;

	// Token: 0x040002A4 RID: 676
	[HideInInspector]
	[NonSerialized]
	public float tiredThreshold;

	// Token: 0x040002A5 RID: 677
	[HideInInspector]
	[NonSerialized]
	public float awakeThreshold;

	// Token: 0x040002A6 RID: 678
	[HideInInspector]
	[NonSerialized]
	public float sleepinessGainedPerSecond;

	// Token: 0x040002A7 RID: 679
	[HideInInspector]
	[NonSerialized]
	public float sleepinessLostPerSecond;

	// Token: 0x040002A8 RID: 680
	[HideInInspector]
	[NonSerialized]
	public float maxStruggle;

	// Token: 0x040002A9 RID: 681
	[HideInInspector]
	[NonSerialized]
	public float escapeThreshold;

	// Token: 0x040002AA RID: 682
	[HideInInspector]
	[NonSerialized]
	public float catchableThreshold;

	// Token: 0x040002AB RID: 683
	[HideInInspector]
	[NonSerialized]
	public float struggleGainedPerSecond;

	// Token: 0x040002AC RID: 684
	[HideInInspector]
	[NonSerialized]
	public float struggleLostPerSecond;

	// Token: 0x040002AD RID: 685
	public List<crittersAttractorStruct> attractedToList;

	// Token: 0x040002AE RID: 686
	public List<crittersAttractorStruct> afraidOfList;

	// Token: 0x040002AF RID: 687
	public Dictionary<CrittersActor.CrittersActorType, float> afraidOfTypes;

	// Token: 0x040002B0 RID: 688
	public Dictionary<CrittersActor.CrittersActorType, float> attractedToTypes;

	// Token: 0x040002B1 RID: 689
	private Rigidbody rB;

	// Token: 0x040002B2 RID: 690
	[NonSerialized]
	public CrittersPawn.CreatureState currentState;

	// Token: 0x040002B3 RID: 691
	[NonSerialized]
	public float currentHunger;

	// Token: 0x040002B4 RID: 692
	[NonSerialized]
	public float currentFear;

	// Token: 0x040002B5 RID: 693
	[NonSerialized]
	public float currentAttraction;

	// Token: 0x040002B6 RID: 694
	[NonSerialized]
	public float currentSleepiness;

	// Token: 0x040002B7 RID: 695
	[NonSerialized]
	public float currentStruggle;

	// Token: 0x040002B8 RID: 696
	public double lifeTime = 10.0;

	// Token: 0x040002B9 RID: 697
	public double lifeTimeStart;

	// Token: 0x040002BA RID: 698
	private CrittersFood eatingTarget;

	// Token: 0x040002BB RID: 699
	private CrittersActor fearTarget;

	// Token: 0x040002BC RID: 700
	private CrittersActor attractionTarget;

	// Token: 0x040002BD RID: 701
	private Vector3 lastSeenFearPosition;

	// Token: 0x040002BE RID: 702
	private Vector3 lastSeenAttractionPosition;

	// Token: 0x040002BF RID: 703
	private CrittersGrabber grabbedTarget;

	// Token: 0x040002C0 RID: 704
	private CrittersCage cageTarget;

	// Token: 0x040002C1 RID: 705
	private int actorIdTarget;

	// Token: 0x040002C2 RID: 706
	[FormerlySerializedAs("eatingRadiusMax")]
	public float eatingRadiusMaxSquared;

	// Token: 0x040002C3 RID: 707
	private bool withinEatingRadius;

	// Token: 0x040002C4 RID: 708
	public Transform animTarget;

	// Token: 0x040002C5 RID: 709
	public MeshRenderer myRenderer;

	// Token: 0x040002C6 RID: 710
	public float autoSeeFoodDistance;

	// Token: 0x040002C7 RID: 711
	public Dictionary<int, CrittersActor> soundsHeard;

	// Token: 0x040002C8 RID: 712
	public float fudge = 1.1f;

	// Token: 0x040002C9 RID: 713
	public float obstacleSeeDistance = 0.25f;

	// Token: 0x040002CA RID: 714
	private RaycastHit[] raycastHits;

	// Token: 0x040002CB RID: 715
	private bool canJump;

	// Token: 0x040002CC RID: 716
	private bool wasSomethingInTheWay;

	// Token: 0x040002CD RID: 717
	public Transform hat;

	// Token: 0x040002CE RID: 718
	private int LastTemplateIndex = -1;

	// Token: 0x040002CF RID: 719
	private int TemplateIndex = -1;

	// Token: 0x040002D0 RID: 720
	private double _nextDespawnCheck;

	// Token: 0x040002D1 RID: 721
	private double _nextStuckCheck;

	// Token: 0x040002D2 RID: 722
	public float killHeight = -500f;

	// Token: 0x040002D3 RID: 723
	private float remainingStunnedTime;

	// Token: 0x040002D4 RID: 724
	private float remainingSlowedTime;

	// Token: 0x040002D5 RID: 725
	private float slowSpeedMod = 1f;

	// Token: 0x040002D6 RID: 726
	[Header("Visuals")]
	public CritterVisuals visuals;

	// Token: 0x040002D7 RID: 727
	[HideInInspector]
	public Dictionary<CrittersPawn.CreatureState, GameObject> StartStateFX = new Dictionary<CrittersPawn.CreatureState, GameObject>();

	// Token: 0x040002D8 RID: 728
	[HideInInspector]
	public Dictionary<CrittersPawn.CreatureState, GameObject> OngoingStateFX = new Dictionary<CrittersPawn.CreatureState, GameObject>();

	// Token: 0x040002D9 RID: 729
	[NonSerialized]
	public GameObject OnReleasedFX;

	// Token: 0x040002DA RID: 730
	private GameObject currentOngoingStateFX;

	// Token: 0x040002DB RID: 731
	[HideInInspector]
	public Dictionary<CrittersPawn.CreatureState, CrittersAnim> stateAnim = new Dictionary<CrittersPawn.CreatureState, CrittersAnim>();

	// Token: 0x040002DC RID: 732
	private CrittersAnim currentAnim;

	// Token: 0x040002DD RID: 733
	private float currentAnimTime;

	// Token: 0x040002DE RID: 734
	public AudioClip grabbedHaptics;

	// Token: 0x040002DF RID: 735
	public float grabbedHapticsStrength;

	// Token: 0x040002E0 RID: 736
	public AnimationCurve spawnInHeighMovement;

	// Token: 0x040002E1 RID: 737
	public AnimationCurve despawnInHeighMovement;

	// Token: 0x040002E2 RID: 738
	private Vector3 spawningStartingPosition;

	// Token: 0x040002E3 RID: 739
	private double spawnStartTime;

	// Token: 0x040002E4 RID: 740
	private double despawnStartTime;

	// Token: 0x040002E5 RID: 741
	private float _spawnAnimationDuration;

	// Token: 0x040002E6 RID: 742
	private float _despawnAnimationDuration;

	// Token: 0x040002E7 RID: 743
	private double _spawnAnimTime;

	// Token: 0x040002E8 RID: 744
	private double _despawnAnimTime;

	// Token: 0x040002E9 RID: 745
	public MeshRenderer debugStateIndicator;

	// Token: 0x040002EA RID: 746
	public Color debugColorIdle;

	// Token: 0x040002EB RID: 747
	public Color debugColorSeekingFood;

	// Token: 0x040002EC RID: 748
	public Color debugColorEating;

	// Token: 0x040002ED RID: 749
	public Color debugColorScared;

	// Token: 0x040002EE RID: 750
	public Color debugColorSleeping;

	// Token: 0x040002EF RID: 751
	public Color debugColorCaught;

	// Token: 0x040002F0 RID: 752
	public Color debugColorCaged;

	// Token: 0x040002F1 RID: 753
	public Color debugColorStunned;

	// Token: 0x040002F2 RID: 754
	public Color debugColorAttracted;

	// Token: 0x040002F3 RID: 755
	[NonSerialized]
	public int regionId;

	// Token: 0x040002F4 RID: 756
	private KeyValueStringPair[] eyeScanData = new KeyValueStringPair[6];

	// Token: 0x02000064 RID: 100
	public enum CreatureState
	{
		// Token: 0x040002F7 RID: 759
		Idle,
		// Token: 0x040002F8 RID: 760
		Eating,
		// Token: 0x040002F9 RID: 761
		AttractedTo,
		// Token: 0x040002FA RID: 762
		Running,
		// Token: 0x040002FB RID: 763
		Grabbed,
		// Token: 0x040002FC RID: 764
		Sleeping,
		// Token: 0x040002FD RID: 765
		SeekingFood,
		// Token: 0x040002FE RID: 766
		Captured,
		// Token: 0x040002FF RID: 767
		Stunned,
		// Token: 0x04000300 RID: 768
		WaitingToDespawn,
		// Token: 0x04000301 RID: 769
		Despawning,
		// Token: 0x04000302 RID: 770
		Spawning
	}

	// Token: 0x02000065 RID: 101
	internal struct CreatureUpdateData
	{
		// Token: 0x06000284 RID: 644 RVA: 0x0001018D File Offset: 0x0000E38D
		internal CreatureUpdateData(CrittersPawn creature)
		{
			this.lastImpulseTime = creature.lastImpulseTime;
			this.state = creature.currentState;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000101A7 File Offset: 0x0000E3A7
		internal bool SameData(CrittersPawn creature)
		{
			return this.lastImpulseTime == creature.lastImpulseTime && this.state == creature.currentState;
		}

		// Token: 0x04000303 RID: 771
		private double lastImpulseTime;

		// Token: 0x04000304 RID: 772
		private CrittersPawn.CreatureState state;
	}
}
