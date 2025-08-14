using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007FF RID: 2047
public class CustomMapsChaseBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06003333 RID: 13107 RVA: 0x0010AB1C File Offset: 0x00108D1C
	public CustomMapsChaseBehaviour(CustomMapsAIBehaviourController AIController, Vector3 agentSightOffset, float agentSightDist, float agentLoseSightDist, float agentStopDist)
	{
		this.sightOffset = agentSightOffset;
		this.sightDist = agentSightDist;
		this.loseSightDist = agentLoseSightDist;
		this.stopDist = agentStopDist;
		this.controller = AIController;
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x0010AB7D File Offset: 0x00108D7D
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull();
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x0010ABA4 File Offset: 0x00108DA4
	public override void Execute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.controller.TargetPlayer.IsNull())
		{
			return;
		}
		float num = this.loseSightDist * this.loseSightDist;
		Vector3 position = this.controller.TargetPlayer.transform.position;
		Vector3 position2 = this.controller.transform.position;
		float sqrMagnitude = (position - position2).sqrMagnitude;
		if (sqrMagnitude > num)
		{
			this.controller.TargetPlayer = null;
			return;
		}
		Vector3 vector = position2 + this.sightOffset;
		if (Physics.RaycastNonAlloc(new Ray(vector, position - vector), CustomMapsChaseBehaviour.visibilityHits, Mathf.Min(Vector3.Distance(position, vector), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) >= 1)
		{
			this.controller.TargetPlayer = null;
			return;
		}
		if (sqrMagnitude < this.stopDist)
		{
			return;
		}
		this.controller.agent.RequestDestination(position);
	}

	// Token: 0x04004042 RID: 16450
	private NavMeshAgent navMeshAgent;

	// Token: 0x04004043 RID: 16451
	private CustomMapsAIBehaviourController controller;

	// Token: 0x04004044 RID: 16452
	private float loseSightDist;

	// Token: 0x04004045 RID: 16453
	private float sightDist;

	// Token: 0x04004046 RID: 16454
	private Vector3 sightOffset;

	// Token: 0x04004047 RID: 16455
	private float stopDist = 4f;

	// Token: 0x04004048 RID: 16456
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x04004049 RID: 16457
	private LayerMask visibilityLayerMask;
}
