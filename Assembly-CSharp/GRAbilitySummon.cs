using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005FD RID: 1533
[Serializable]
public class GRAbilitySummon
{
	// Token: 0x060025AE RID: 9646 RVA: 0x000CA016 File Offset: 0x000C8216
	public void Setup(GameAgent agent, Animation animation, Transform root)
	{
		this.agent = agent;
		this.root = root;
		this.anim = animation;
		this.audioSource = agent.GetComponent<AudioSource>();
		this.entity = agent.GetComponent<GameEntity>();
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000CA048 File Offset: 0x000C8248
	public void Start()
	{
		int index = Random.Range(0, this.animData.Count);
		this.duration = this.animData[index].duration;
		this.chargeTime = this.animData[index].eventTime;
		this.PlayAnim(this.animData[index].animName, 0.1f, this.animSpeed);
		this.startTime = Time.timeAsDouble;
		this.state = GRAbilitySummon.State.Charge;
		this.summonSound.Play(this.audioSource);
		this.spawnedCount = 0;
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.agent.navAgent.speed = 1f;
		if (this.fxStartSummon != null)
		{
			this.fxStartSummon.SetActive(false);
			this.fxStartSummon.SetActive(true);
		}
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000CA135 File Offset: 0x000C8335
	public void Stop()
	{
		this.lookAtTarget = null;
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000CA157 File Offset: 0x000C8357
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000CA160 File Offset: 0x000C8360
	public void Think(float dt)
	{
		this.UpdateState(dt);
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000CA169 File Offset: 0x000C8369
	public void Update(float dt)
	{
		if (this.lookAtTarget != null)
		{
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.lookAtTarget, 360f);
		}
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000CA19C File Offset: 0x000C839C
	private void UpdateState(float dt)
	{
		double num = Time.timeAsDouble - this.startTime;
		switch (this.state)
		{
		case GRAbilitySummon.State.Charge:
			if (num > (double)this.chargeTime)
			{
				this.SetState(GRAbilitySummon.State.Spawn);
				return;
			}
			break;
		case GRAbilitySummon.State.Spawn:
			if (!this.spawned)
			{
				this.spawned = this.DoSpawn();
			}
			if (this.spawned && num > (double)this.duration)
			{
				this.SetState(GRAbilitySummon.State.Done);
				this.spawned = false;
			}
			break;
		case GRAbilitySummon.State.Done:
			break;
		default:
			return;
		}
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000CA216 File Offset: 0x000C8416
	private void SetState(GRAbilitySummon.State newState)
	{
		GRAbilitySummon.State state = this.state;
		this.state = newState;
		switch (newState)
		{
		default:
			return;
		}
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000CA238 File Offset: 0x000C8438
	private Vector3? GetSpawnLocation()
	{
		Vector3 position = this.root.position;
		float num = Random.Range(0f, this.summonConeAngle);
		int i = 0;
		while (i < 5)
		{
			Vector3 a = Quaternion.Euler(0f, num, 0f) * this.root.forward;
			Vector3 vector = position + a * this.desiredSpawnDistance;
			NavMeshHit navMeshHit;
			if (NavMesh.Raycast(position, vector, out navMeshHit, -1))
			{
				if (navMeshHit.distance < this.minSpawnDistance)
				{
					num += 15f;
					i++;
					continue;
				}
				vector = navMeshHit.position + Vector3.up * this.spawnHeight;
			}
			return new Vector3?(vector);
		}
		return null;
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000CA300 File Offset: 0x000C8500
	private bool DoSpawn()
	{
		Vector3? spawnLocation = this.GetSpawnLocation();
		if (spawnLocation != null)
		{
			if (this.entity.IsAuthority())
			{
				Quaternion identity = Quaternion.identity;
				GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
				Debug.Log(string.Format("summon ability spawning for summoner id: {0} name: {1}", this.entity.GetNetId(), this.entity.ToString()), this.entity);
				gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), spawnLocation.Value, identity, (long)this.entity.GetNetId());
				this.spawnedCount++;
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.summonSpawnAudioClip);
			}
			if (this.fxOnSpawn != null)
			{
				this.fxOnSpawn.SetActive(false);
				this.fxOnSpawn.SetActive(true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000CA3F3 File Offset: 0x000C85F3
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x000CA42A File Offset: 0x000C862A
	public bool IsDone()
	{
		return this.state == GRAbilitySummon.State.Done;
	}

	// Token: 0x04002FB2 RID: 12210
	private GameEntity entity;

	// Token: 0x04002FB3 RID: 12211
	private GameAgent agent;

	// Token: 0x04002FB4 RID: 12212
	private Transform root;

	// Token: 0x04002FB5 RID: 12213
	private double startTime;

	// Token: 0x04002FB6 RID: 12214
	public GameEntity entityPrefabToSpawn;

	// Token: 0x04002FB7 RID: 12215
	private Animation anim;

	// Token: 0x04002FB8 RID: 12216
	public List<AnimationData> animData;

	// Token: 0x04002FB9 RID: 12217
	private float animSpeed = 1f;

	// Token: 0x04002FBA RID: 12218
	public float chargeTime = 3f;

	// Token: 0x04002FBB RID: 12219
	public float duration = 3f;

	// Token: 0x04002FBC RID: 12220
	public float desiredSpawnDistance = 3f;

	// Token: 0x04002FBD RID: 12221
	public float minSpawnDistance = 1f;

	// Token: 0x04002FBE RID: 12222
	public float spawnHeight = 1f;

	// Token: 0x04002FBF RID: 12223
	public float summonConeAngle = 120f;

	// Token: 0x04002FC0 RID: 12224
	private bool spawned;

	// Token: 0x04002FC1 RID: 12225
	public AudioClip summonSpawnAudioClip;

	// Token: 0x04002FC2 RID: 12226
	public GameObject fxStartSummon;

	// Token: 0x04002FC3 RID: 12227
	public GameObject fxOnSpawn;

	// Token: 0x04002FC4 RID: 12228
	private AudioSource audioSource;

	// Token: 0x04002FC5 RID: 12229
	public AbilitySound summonSound;

	// Token: 0x04002FC6 RID: 12230
	private int spawnedCount;

	// Token: 0x04002FC7 RID: 12231
	public Transform lookAtTarget;

	// Token: 0x04002FC8 RID: 12232
	private GRAbilitySummon.State state;

	// Token: 0x020005FE RID: 1534
	private enum State
	{
		// Token: 0x04002FCA RID: 12234
		Charge,
		// Token: 0x04002FCB RID: 12235
		Spawn,
		// Token: 0x04002FCC RID: 12236
		Done
	}
}
