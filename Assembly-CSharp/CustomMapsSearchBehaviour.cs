using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000800 RID: 2048
public class CustomMapsSearchBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06003337 RID: 13111 RVA: 0x0010ACAC File Offset: 0x00108EAC
	public CustomMapsSearchBehaviour(CustomMapsAIBehaviourController AIcontroller, Vector3 agentSightOffset, float agentSightDist, float fieldOfView)
	{
		this.sightOffset = agentSightOffset;
		this.sightDist = agentSightDist;
		this.sightFOV = fieldOfView;
		this.controller = AIcontroller;
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Gorilla Object"
		});
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x0010AD07 File Offset: 0x00108F07
	public override bool CanExecute()
	{
		return !this.controller.IsNull();
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x0010AD1C File Offset: 0x00108F1C
	public override void Execute()
	{
		float num = float.MaxValue;
		this.controller.TargetPlayer = null;
		this.tempRigs.Clear();
		this.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.tempRigs);
		Vector3 position = this.controller.transform.position;
		Vector3 rhs = this.controller.transform.rotation * Vector3.forward;
		float num2 = this.sightDist * this.sightDist;
		float num3 = Mathf.Cos(this.sightFOV);
		Vector3 vector = position + this.controller.transform.TransformVector(this.sightOffset);
		for (int i = 0; i < this.tempRigs.Count; i++)
		{
			VRRig vrrig = this.tempRigs[i];
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			Vector3 mouthPosition = vrrig.GetMouthPosition();
			Vector3 a = mouthPosition - vector;
			float sqrMagnitude = a.sqrMagnitude;
			if (sqrMagnitude <= num2)
			{
				float num4 = 0f;
				if (sqrMagnitude > 0f)
				{
					num4 = Mathf.Sqrt(sqrMagnitude);
					if (Vector3.Dot(a / num4, rhs) < num3)
					{
						goto IL_15B;
					}
				}
				if (num4 < num && Physics.RaycastNonAlloc(new Ray(vector, mouthPosition - vector), CustomMapsSearchBehaviour.visibilityHits, Mathf.Min(Vector3.Distance(mouthPosition, vector), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1)
				{
					num = num4;
					this.controller.TargetPlayer = component;
				}
			}
			IL_15B:;
		}
	}

	// Token: 0x0400404A RID: 16458
	private CustomMapsAIBehaviourController controller;

	// Token: 0x0400404B RID: 16459
	private List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400404C RID: 16460
	private float sightDist;

	// Token: 0x0400404D RID: 16461
	private Vector3 sightOffset;

	// Token: 0x0400404E RID: 16462
	private float sightFOV;

	// Token: 0x0400404F RID: 16463
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x04004050 RID: 16464
	private LayerMask visibilityLayerMask;
}
