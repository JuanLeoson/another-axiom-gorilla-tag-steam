using System;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000F90 RID: 3984
	public class CrittersSpawnPoint : MonoBehaviour
	{
		// Token: 0x0600639B RID: 25499 RVA: 0x001F5F2C File Offset: 0x001F412C
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
