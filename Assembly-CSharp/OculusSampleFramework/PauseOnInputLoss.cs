using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D04 RID: 3332
	public class PauseOnInputLoss : MonoBehaviour
	{
		// Token: 0x06005271 RID: 21105 RVA: 0x00199D23 File Offset: 0x00197F23
		private void Start()
		{
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x06005272 RID: 21106 RVA: 0x00199D47 File Offset: 0x00197F47
		private void OnInputFocusLost()
		{
			Time.timeScale = 0f;
		}

		// Token: 0x06005273 RID: 21107 RVA: 0x00199D53 File Offset: 0x00197F53
		private void OnInputFocusAcquired()
		{
			Time.timeScale = 1f;
		}
	}
}
