using System;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class SkeletonPathingNode : MonoBehaviour
{
	// Token: 0x0600057F RID: 1407 RVA: 0x00020127 File Offset: 0x0001E327
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400067D RID: 1661
	public bool ejectionPoint;

	// Token: 0x0400067E RID: 1662
	public SkeletonPathingNode[] connectedNodes;

	// Token: 0x0400067F RID: 1663
	public float distanceToExitNode;
}
