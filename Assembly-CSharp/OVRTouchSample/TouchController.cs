using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000D43 RID: 3395
	public class TouchController : MonoBehaviour
	{
		// Token: 0x0600540A RID: 21514 RVA: 0x0019F5F8 File Offset: 0x0019D7F8
		private void Update()
		{
			this.m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).x);
			this.m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).y);
			this.m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
			this.m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x0600540B RID: 21515 RVA: 0x0019F6F5 File Offset: 0x0019D8F5
		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		// Token: 0x0600540C RID: 21516 RVA: 0x0019F717 File Offset: 0x0019D917
		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				base.gameObject.SetActive(true);
				this.m_restoreOnInputAcquired = false;
			}
		}

		// Token: 0x04005D8D RID: 23949
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04005D8E RID: 23950
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04005D8F RID: 23951
		private bool m_restoreOnInputAcquired;
	}
}
