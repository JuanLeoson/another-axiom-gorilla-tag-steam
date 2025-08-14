using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200063F RID: 1599
public class GRGuide : MonoBehaviour
{
	// Token: 0x0600275F RID: 10079 RVA: 0x000D45D8 File Offset: 0x000D27D8
	private void Awake()
	{
		this.path = new NavMeshPath();
		this.showing = false;
		for (int i = 0; i < this.show.Count; i++)
		{
			this.show[i].SetActive(false);
		}
		this.hasPath = false;
		this.numPathCorners = 0;
		this.pathCorners = new Vector3[512];
		this.connectorCorners = new List<Vector3>(64);
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000D464C File Offset: 0x000D284C
	private void Update()
	{
		bool flag = GRPlayer.Get(VRRig.LocalRig).State == GRPlayer.GRPlayerState.Ghost;
		Vector3 position = VRRig.LocalRig.transform.position;
		float sqrMagnitude = (position - base.transform.position).sqrMagnitude;
		if (flag && (!this.hasPath || sqrMagnitude > 36f))
		{
			this.hasPath = false;
			Vector3 sourcePosition;
			Quaternion quaternion;
			NavMeshHit navMeshHit;
			NavMeshHit navMeshHit2;
			if (GhostReactor.instance.levelGenerator.GetExitFromCurrentSection(position, out sourcePosition, out quaternion, this.connectorCorners) && NavMesh.SamplePosition(position, out navMeshHit, 5f, -1) && NavMesh.SamplePosition(sourcePosition, out navMeshHit2, 5f, -1) && NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path) && this.path.status == NavMeshPathStatus.PathComplete)
			{
				this.numPathCorners = this.path.GetCornersNonAlloc(this.pathCorners);
				for (int i = this.connectorCorners.Count - 1; i >= 0; i--)
				{
					this.pathCorners[this.numPathCorners] = this.connectorCorners[i];
					this.numPathCorners++;
				}
				if (this.numPathCorners > 0)
				{
					base.transform.position = this.pathCorners[0];
					this.hasPath = true;
				}
			}
		}
		if (!flag)
		{
			this.hasPath = false;
		}
		if (this.showing != this.hasPath)
		{
			this.showing = this.hasPath;
			for (int j = 0; j < this.show.Count; j++)
			{
				this.show[j].SetActive(this.showing);
			}
			if (this.audioSource != null)
			{
				if (this.showing)
				{
					this.audioSource.Play();
				}
				else
				{
					this.audioSource.Stop();
				}
			}
		}
		if (this.hasPath)
		{
			int num;
			Vector3 closestPointOnPath = GRGuide.GetClosestPointOnPath(position, this.pathCorners, this.numPathCorners, out num);
			float num2 = 2.5f;
			Vector3 vector = closestPointOnPath;
			for (int k = num; k < this.numPathCorners; k++)
			{
				Vector3 a = this.pathCorners[k] - vector;
				float magnitude = a.magnitude;
				if (num2 <= magnitude)
				{
					vector += a * (num2 / magnitude);
					break;
				}
				num2 -= magnitude;
				vector = this.pathCorners[k];
			}
			base.transform.position = vector;
		}
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000D48D8 File Offset: 0x000D2AD8
	private static Vector3 GetClosestPointOnPath(Vector3 pos, Vector3[] pathCorners, int numPathCorners, out int nextCorner)
	{
		nextCorner = 0;
		if (numPathCorners == 0)
		{
			return pos;
		}
		if (numPathCorners == 1)
		{
			return pathCorners[0];
		}
		float num = float.MaxValue;
		Vector3 result = Vector3.zero;
		for (int i = 0; i < numPathCorners - 1; i++)
		{
			Vector3 vA = pathCorners[i];
			Vector3 vB = pathCorners[i + 1];
			Vector3 vector = GRGuide.ClosestPointOnLine(vA, vB, pos);
			float sqrMagnitude = (vector - pos).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = vector;
				nextCorner = i + 1;
			}
		}
		return result;
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000D4954 File Offset: 0x000D2B54
	public static Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
	{
		Vector3 rhs = vPoint - vA;
		Vector3 normalized = (vB - vA).normalized;
		float num = Vector3.Distance(vA, vB);
		float num2 = Vector3.Dot(normalized, rhs);
		if (num2 <= 0f)
		{
			return vA;
		}
		if (num2 >= num)
		{
			return vB;
		}
		Vector3 b = normalized * num2;
		return vA + b;
	}

	// Token: 0x04003289 RID: 12937
	public Transform tempTarget;

	// Token: 0x0400328A RID: 12938
	public List<GameObject> show;

	// Token: 0x0400328B RID: 12939
	public AudioSource audioSource;

	// Token: 0x0400328C RID: 12940
	private bool showing;

	// Token: 0x0400328D RID: 12941
	private bool hasPath;

	// Token: 0x0400328E RID: 12942
	private NavMeshPath path;

	// Token: 0x0400328F RID: 12943
	private int numPathCorners;

	// Token: 0x04003290 RID: 12944
	private Vector3[] pathCorners;

	// Token: 0x04003291 RID: 12945
	private List<Vector3> connectorCorners;
}
