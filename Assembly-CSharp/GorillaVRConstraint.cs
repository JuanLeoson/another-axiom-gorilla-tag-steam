using System;
using UnityEngine;

// Token: 0x02000738 RID: 1848
public class GorillaVRConstraint : MonoBehaviour
{
	// Token: 0x06002E35 RID: 11829 RVA: 0x000F4F50 File Offset: 0x000F3150
	private void Update()
	{
		if (NetworkSystem.Instance.WrongVersion)
		{
			this.isConstrained = true;
		}
		if (this.isConstrained && Time.realtimeSinceStartup > this.angle)
		{
			GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
		}
	}

	// Token: 0x04003A12 RID: 14866
	public bool isConstrained;

	// Token: 0x04003A13 RID: 14867
	public float angle = 3600f;
}
