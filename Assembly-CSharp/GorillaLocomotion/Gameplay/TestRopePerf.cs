using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E2F RID: 3631
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x06005A4E RID: 23118 RVA: 0x001C79DD File Offset: 0x001C5BDD
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x04006516 RID: 25878
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x04006517 RID: 25879
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x04006518 RID: 25880
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
