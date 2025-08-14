using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D01 RID: 3329
	public class GrabbableCrosshair : MonoBehaviour
	{
		// Token: 0x0600526A RID: 21098 RVA: 0x00199C3F File Offset: 0x00197E3F
		private void Start()
		{
			this.m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
		}

		// Token: 0x0600526B RID: 21099 RVA: 0x00199C58 File Offset: 0x00197E58
		public void SetState(GrabbableCrosshair.CrosshairState cs)
		{
			this.m_state = cs;
			if (cs == GrabbableCrosshair.CrosshairState.Disabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(false);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Enabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(true);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Targeted)
			{
				this.m_targetedCrosshair.SetActive(true);
				this.m_enabledCrosshair.SetActive(false);
			}
		}

		// Token: 0x0600526C RID: 21100 RVA: 0x00199CC1 File Offset: 0x00197EC1
		private void Update()
		{
			if (this.m_state != GrabbableCrosshair.CrosshairState.Disabled)
			{
				base.transform.LookAt(this.m_centerEyeAnchor);
			}
		}

		// Token: 0x04005BD7 RID: 23511
		private GrabbableCrosshair.CrosshairState m_state;

		// Token: 0x04005BD8 RID: 23512
		private Transform m_centerEyeAnchor;

		// Token: 0x04005BD9 RID: 23513
		[SerializeField]
		private GameObject m_targetedCrosshair;

		// Token: 0x04005BDA RID: 23514
		[SerializeField]
		private GameObject m_enabledCrosshair;

		// Token: 0x02000D02 RID: 3330
		public enum CrosshairState
		{
			// Token: 0x04005BDC RID: 23516
			Disabled,
			// Token: 0x04005BDD RID: 23517
			Enabled,
			// Token: 0x04005BDE RID: 23518
			Targeted
		}
	}
}
