using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005FF RID: 1535
[Serializable]
public class GRAbilityKeepDistance
{
	// Token: 0x060025BB RID: 9659 RVA: 0x000CA498 File Offset: 0x000C8698
	public void Setup(GameAgent agent, Animation animation)
	{
		this.agent = agent;
		this.anim = animation;
		this.entity = agent.GetComponent<GameEntity>();
		this.root = agent.transform;
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.moveAbility.Setup(agent, this.anim, this.root);
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x000CA4F0 File Offset: 0x000C86F0
	public void Start()
	{
		this.startTime = Time.timeAsDouble;
		this.moveAbility.Start();
		this.agent.SetIsPathing(true, true);
		Vector3 targetPos = this.PickBackupDestination();
		this.moveAbility.SetTargetPos(targetPos);
		this.defaultUpdateRotation = this.navMeshAgent.updateRotation;
		this.navMeshAgent.updateRotation = false;
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x000CA550 File Offset: 0x000C8750
	public void Stop()
	{
		this.moveAbility.Stop();
		this.navMeshAgent.updateRotation = this.defaultUpdateRotation;
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsDone()
	{
		return false;
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x000CA570 File Offset: 0x000C8770
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.moveAbility.SetLookAtTarget(this.target);
			}
		}
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000CA5C4 File Offset: 0x000C87C4
	public void Think(float dt)
	{
		if (this.moveAbility.IsDone())
		{
			Vector3 targetPos = this.PickBackupDestination();
			this.moveAbility.SetTargetPos(targetPos);
		}
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x000CA5F4 File Offset: 0x000C87F4
	private Vector3 PickBackupDestination()
	{
		Vector3 position = this.agent.transform.position;
		if (this.target == null)
		{
			return position;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, -1))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 vector = this.agent.transform.position - this.target.position;
			vector.y = 0f;
			Vector3 normalized = vector.normalized;
			for (int i = 0; i < GRAbilityKeepDistance.rotations.Length; i++)
			{
				Vector3 a = GRAbilityKeepDistance.rotations[i] * normalized;
				float d = 2f;
				NavMeshHit navMeshHit2;
				NavMeshHit navMeshHit3;
				if ((!NavMesh.Raycast(position2, position2 + a * d, out navMeshHit2, -1) || navMeshHit2.distance >= this.minBackupSpaceRequired) && NavMesh.SamplePosition(navMeshHit2.position, out navMeshHit3, 1f, -1))
				{
					return navMeshHit3.position;
				}
			}
		}
		return position;
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000CA6EF File Offset: 0x000C88EF
	public void Update(float dt)
	{
		this.moveAbility.Update(dt);
	}

	// Token: 0x04002FCD RID: 12237
	private GameEntity entity;

	// Token: 0x04002FCE RID: 12238
	private GameAgent agent;

	// Token: 0x04002FCF RID: 12239
	private NavMeshAgent navMeshAgent;

	// Token: 0x04002FD0 RID: 12240
	private Transform root;

	// Token: 0x04002FD1 RID: 12241
	private double startTime;

	// Token: 0x04002FD2 RID: 12242
	private Transform target;

	// Token: 0x04002FD3 RID: 12243
	private Animation anim;

	// Token: 0x04002FD4 RID: 12244
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04002FD5 RID: 12245
	public float minBackupSpaceRequired = 0.5f;

	// Token: 0x04002FD6 RID: 12246
	private bool defaultUpdateRotation;

	// Token: 0x04002FD7 RID: 12247
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 30f, 0f),
		Quaternion.Euler(0f, -30f, 0f),
		Quaternion.Euler(0f, 60f, 0f),
		Quaternion.Euler(0f, -60f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};
}
