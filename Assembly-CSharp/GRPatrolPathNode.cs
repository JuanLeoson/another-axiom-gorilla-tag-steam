using System;
using UnityEngine;

// Token: 0x0200064B RID: 1611
public class GRPatrolPathNode : MonoBehaviour
{
	// Token: 0x06002786 RID: 10118 RVA: 0x000D52E0 File Offset: 0x000D34E0
	public void OnDrawGizmosSelected()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		GRPatrolPath component = base.transform.parent.GetComponent<GRPatrolPath>();
		if (component == null)
		{
			return;
		}
		component.OnDrawGizmosSelected();
	}
}
