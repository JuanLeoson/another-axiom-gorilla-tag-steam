using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200064A RID: 1610
public class GRPatrolPath : MonoBehaviour
{
	// Token: 0x06002783 RID: 10115 RVA: 0x000D5184 File Offset: 0x000D3384
	private void Awake()
	{
		this.patrolNodes = new List<Transform>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.patrolNodes.Add(base.transform.GetChild(i));
		}
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000D51D4 File Offset: 0x000D33D4
	public void OnDrawGizmosSelected()
	{
		if (this.patrolNodes == null || base.transform.childCount != this.patrolNodes.Count)
		{
			this.patrolNodes = new List<Transform>(base.transform.childCount);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				this.patrolNodes.Add(base.transform.GetChild(i));
			}
		}
		if (this.patrolNodes != null)
		{
			for (int j = 0; j < this.patrolNodes.Count; j++)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(this.patrolNodes[j].transform.position, Vector3.one * 0.5f);
				if (j < this.patrolNodes.Count - 1)
				{
					Gizmos.DrawLine(this.patrolNodes[j].transform.position, this.patrolNodes[j + 1].transform.position);
				}
			}
		}
	}

	// Token: 0x040032BB RID: 12987
	[NonSerialized]
	public List<Transform> patrolNodes;

	// Token: 0x040032BC RID: 12988
	public int index;
}
