using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005FA RID: 1530
[Serializable]
public class GRAbilityWander
{
	// Token: 0x06002599 RID: 9625 RVA: 0x000C9CC3 File Offset: 0x000C7EC3
	public void Setup(GameAgent agent, Animation anim, Transform root)
	{
		this.agent = agent;
		this.moveAbility.Setup(agent, anim, root);
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000C9CDC File Offset: 0x000C7EDC
	public void Start()
	{
		this.moveAbility.Start();
		Vector3 targetPos = this.PickRandomDestination();
		this.moveAbility.SetTargetPos(targetPos);
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000C9D07 File Offset: 0x000C7F07
	public void Stop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsDone()
	{
		return false;
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000C9D14 File Offset: 0x000C7F14
	public void Think(float dt)
	{
		if (this.moveAbility.IsDone())
		{
			Vector3 targetPos = this.PickRandomDestination();
			this.moveAbility.SetTargetPos(targetPos);
		}
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000C9D44 File Offset: 0x000C7F44
	private Vector3 PickRandomDestination()
	{
		Vector3 position = this.agent.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, -1))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 forward = this.agent.transform.forward;
			float num = 0f;
			Vector3 sourcePosition = position2;
			for (int i = 0; i < GRAbilityWander.rotations.Length; i++)
			{
				Vector3 a = GRAbilityWander.rotations[i] * forward;
				float num2 = 8f;
				if (NavMesh.Raycast(position2, position2 + a * num2, out navMeshHit, -1))
				{
					num2 = navMeshHit.distance * 0.95f;
				}
				float num3 = num2 * GRAbilityWander.rotationWeight[i];
				if (num3 > num)
				{
					num = num3;
					sourcePosition = position2 + a * num2;
				}
			}
			if (NavMesh.SamplePosition(sourcePosition, out navMeshHit, 1f, -1))
			{
				position = navMeshHit.position;
			}
		}
		return position;
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000C9E34 File Offset: 0x000C8034
	public void Update(float dt)
	{
		this.moveAbility.Update(dt);
	}

	// Token: 0x04002FAA RID: 12202
	private GameAgent agent;

	// Token: 0x04002FAB RID: 12203
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04002FAC RID: 12204
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 45f, 0f),
		Quaternion.Euler(0f, -45f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};

	// Token: 0x04002FAD RID: 12205
	private static float[] rotationWeight = new float[]
	{
		1f,
		0.75f,
		0.75f,
		0.5f,
		0.5f,
		0.2f,
		0.2f,
		0.2f
	};
}
