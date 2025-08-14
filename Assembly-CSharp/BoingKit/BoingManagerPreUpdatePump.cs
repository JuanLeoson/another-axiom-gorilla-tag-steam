using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FDC RID: 4060
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x06006579 RID: 25977 RVA: 0x00201F96 File Offset: 0x00200196
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x0600657A RID: 25978 RVA: 0x00201F96 File Offset: 0x00200196
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x0600657B RID: 25979 RVA: 0x00201F9E File Offset: 0x0020019E
		private void TryPump()
		{
			if (this.m_lastPumpedFrame >= Time.frameCount)
			{
				return;
			}
			if (this.m_lastPumpedFrame >= 0)
			{
				this.DoPump();
			}
			this.m_lastPumpedFrame = Time.frameCount;
		}

		// Token: 0x0600657C RID: 25980 RVA: 0x00201FC8 File Offset: 0x002001C8
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x04007030 RID: 28720
		private int m_lastPumpedFrame = -1;
	}
}
