using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200059E RID: 1438
public class GameAgent : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x1400004A RID: 74
	// (add) Token: 0x060022FE RID: 8958 RVA: 0x000BCB60 File Offset: 0x000BAD60
	// (remove) Token: 0x060022FF RID: 8959 RVA: 0x000BCB98 File Offset: 0x000BAD98
	public event GameAgent.StateChangedEvent onBodyStateChanged;

	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06002300 RID: 8960 RVA: 0x000BCBD0 File Offset: 0x000BADD0
	// (remove) Token: 0x06002301 RID: 8961 RVA: 0x000BCC08 File Offset: 0x000BAE08
	public event GameAgent.StateChangedEvent onBehaviorStateChanged;

	// Token: 0x06002302 RID: 8962 RVA: 0x000BCC3D File Offset: 0x000BAE3D
	public GameAgentManager GetGameAgentManager()
	{
		return this.entity.manager.gameAgentManager;
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000BCC4F File Offset: 0x000BAE4F
	private void Awake()
	{
		this.agentComponents = new List<IGameAgentComponent>(1);
		base.GetComponentsInChildren<IGameAgentComponent>(this.agentComponents);
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000BCC69 File Offset: 0x000BAE69
	public void OnEntityInit()
	{
		this.GetGameAgentManager().AddGameAgent(this);
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000BCC77 File Offset: 0x000BAE77
	public void OnEntityDestroy()
	{
		this.GetGameAgentManager().RemoveGameAgent(this);
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000BCC85 File Offset: 0x000BAE85
	public void OnBehaviorStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBehaviorStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000BCC98 File Offset: 0x000BAE98
	public void OnBodyStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBodyStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000BCCAC File Offset: 0x000BAEAC
	public void OnThink(float deltaTime)
	{
		for (int i = 0; i < this.agentComponents.Count; i++)
		{
			this.agentComponents[i].OnEntityThink(deltaTime);
		}
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000BCCE1 File Offset: 0x000BAEE1
	public void OnUpdate()
	{
		if (this.navAgent.isOnNavMesh)
		{
			this.lastPosOnNavMesh = this.navAgent.transform.position;
		}
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000BCD06 File Offset: 0x000BAF06
	public bool IsOnNavMesh()
	{
		return this.navAgent != null && this.navAgent.isOnNavMesh;
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000BCD23 File Offset: 0x000BAF23
	public Vector3 GetLastPosOnNavMesh()
	{
		return this.lastPosOnNavMesh;
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000BCD2C File Offset: 0x000BAF2C
	public void RequestDestination(Vector3 dest)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!this.IsOnNavMesh())
		{
			dest = this.lastPosOnNavMesh;
		}
		if (Vector3.Distance(this.lastRequestedDest, dest) < 0.5f)
		{
			return;
		}
		this.lastRequestedDest = dest;
		if (this.entity.IsAuthority())
		{
			this.GetGameAgentManager().RequestDestination(this, dest);
		}
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000BCD8C File Offset: 0x000BAF8C
	public void RequestBehaviorChange(byte behavior)
	{
		this.GetGameAgentManager().RequestBehavior(this, behavior);
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000BCD9B File Offset: 0x000BAF9B
	public void RequestStateChange(byte state)
	{
		this.GetGameAgentManager().RequestState(this, state);
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000BCDAA File Offset: 0x000BAFAA
	public void RequestTarget(NetPlayer targetPlayer)
	{
		this.GetGameAgentManager().RequestTarget(this, targetPlayer);
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000BCDBC File Offset: 0x000BAFBC
	public void ApplyDestination(Vector3 dest)
	{
		NavMeshHit navMeshHit;
		if (!NavMesh.SamplePosition(dest, out navMeshHit, 1.5f, -1))
		{
			return;
		}
		dest = navMeshHit.position;
		this.lastReceivedDest = dest;
		if (this.navAgent.isOnNavMesh)
		{
			this.navAgent.destination = dest;
		}
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000BCE03 File Offset: 0x000BB003
	public void SetDisableNetworkSync(bool disable)
	{
		this.disableNetworkSync = disable;
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000BCE0C File Offset: 0x000BB00C
	public void SetIsPathing(bool isPathing, bool ignoreRigiBody = false)
	{
		this.navAgent.enabled = isPathing;
		if (!ignoreRigiBody && this.rigidBody != null)
		{
			this.rigidBody.isKinematic = isPathing;
		}
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000BCE37 File Offset: 0x000BB037
	public void SetSpeed(float speed)
	{
		this.navAgent.speed = speed;
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000BCE48 File Offset: 0x000BB048
	public void ApplyNetworkUpdate(Vector3 position, Quaternion rotation)
	{
		if (this.disableNetworkSync)
		{
			return;
		}
		if ((base.transform.position - position).sqrMagnitude > this.networkPositionCorrectionDist * this.networkPositionCorrectionDist)
		{
			this.navAgent.Warp(position);
			this.navAgent.destination = this.lastReceivedDest;
		}
		base.transform.rotation = rotation;
		if (this.rigidBody != null)
		{
			this.rigidBody.rotation = rotation;
		}
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x000BCECC File Offset: 0x000BB0CC
	public static void UpdateFacing(Transform transform, NavMeshAgent navAgent, NetPlayer targetPlayer, float turnspeed = 3600f)
	{
		Transform target = null;
		Vector3 forward = transform.forward;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				target = grplayer.transform;
			}
		}
		GameAgent.UpdateFacingTarget(transform, navAgent, target, turnspeed);
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000BCF14 File Offset: 0x000BB114
	public static void UpdateFacingTarget(Transform transform, NavMeshAgent navAgent, Transform target, float turnspeed = 3600f)
	{
		Vector3 forward = transform.forward;
		if (target != null)
		{
			Vector3 position = target.position;
			Vector3 position2 = transform.position;
			Vector3 a = position - position2;
			a.y = 0f;
			float magnitude = a.magnitude;
			if (magnitude > 0f)
			{
				forward = a / magnitude;
			}
		}
		else
		{
			Vector3 desiredVelocity = navAgent.desiredVelocity;
			desiredVelocity.y = 0f;
			float magnitude2 = desiredVelocity.magnitude;
			if (magnitude2 > 0f)
			{
				forward = desiredVelocity / magnitude2;
			}
		}
		Quaternion b = Quaternion.LookRotation(forward);
		transform.rotation = Quaternion.Lerp(transform.rotation, b, Mathf.Clamp(turnspeed * navAgent.speed / Quaternion.Angle(transform.rotation, b) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x000BCFE4 File Offset: 0x000BB1E4
	public static void UpdateFacingForward(Transform transform, NavMeshAgent navAgent, float turnspeed = 3600f)
	{
		Vector3 desiredVelocity = navAgent.desiredVelocity;
		desiredVelocity.y = 0f;
		float magnitude = desiredVelocity.magnitude;
		if (magnitude <= 0f)
		{
			return;
		}
		Vector3 facingDir = desiredVelocity / magnitude;
		GameAgent.UpdateFacingDir(transform, navAgent, facingDir, turnspeed);
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000BD028 File Offset: 0x000BB228
	public static void UpdateFacingPos(Transform transform, NavMeshAgent navAgent, Vector3 facingPos, float turnspeed = 3600f)
	{
		Vector3 facingDir = facingPos - transform.position;
		facingDir.y = 0f;
		facingDir.Normalize();
		GameAgent.UpdateFacingDir(transform, navAgent, facingDir, turnspeed);
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000BD060 File Offset: 0x000BB260
	public static void UpdateFacingDir(Transform transform, NavMeshAgent navAgent, Vector3 facingDir, float turnspeed = 3600f)
	{
		Quaternion b = Quaternion.LookRotation(facingDir);
		transform.rotation = Quaternion.Lerp(transform.rotation, b, Mathf.Clamp(turnspeed * navAgent.speed / Quaternion.Angle(transform.rotation, b) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x04002CC9 RID: 11465
	public GameEntity entity;

	// Token: 0x04002CCA RID: 11466
	public NavMeshAgent navAgent;

	// Token: 0x04002CCB RID: 11467
	public Rigidbody rigidBody;

	// Token: 0x04002CCC RID: 11468
	public float networkPositionCorrectionDist = 2.5f;

	// Token: 0x04002CCD RID: 11469
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04002CCE RID: 11470
	private bool disableNetworkSync;

	// Token: 0x04002CCF RID: 11471
	private Vector3 lastPosOnNavMesh;

	// Token: 0x04002CD0 RID: 11472
	private Vector3 lastRequestedDest;

	// Token: 0x04002CD1 RID: 11473
	private Vector3 lastReceivedDest;

	// Token: 0x04002CD4 RID: 11476
	private List<IGameAgentComponent> agentComponents;

	// Token: 0x0200059F RID: 1439
	// (Invoke) Token: 0x0600231D RID: 8989
	public delegate void StateChangedEvent(byte newState);
}
