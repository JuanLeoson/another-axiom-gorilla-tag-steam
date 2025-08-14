using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class BeeAvoidPoint : MonoBehaviour
{
	// Token: 0x06000697 RID: 1687 RVA: 0x00026ABB File Offset: 0x00024CBB
	private void Start()
	{
		BeeSwarmManager.RegisterAvoidPoint(base.gameObject);
		FlockingManager.RegisterAvoidPoint(base.gameObject);
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00026AD3 File Offset: 0x00024CD3
	private void OnDestroy()
	{
		BeeSwarmManager.UnregisterAvoidPoint(base.gameObject);
		FlockingManager.UnregisterAvoidPoint(base.gameObject);
	}
}
