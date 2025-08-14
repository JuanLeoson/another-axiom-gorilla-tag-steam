using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000CFF RID: 3327
	public class DistanceGrabbable : OVRGrabbable
	{
		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06005257 RID: 21079 RVA: 0x00199253 File Offset: 0x00197453
		// (set) Token: 0x06005258 RID: 21080 RVA: 0x0019925B File Offset: 0x0019745B
		public bool InRange
		{
			get
			{
				return this.m_inRange;
			}
			set
			{
				this.m_inRange = value;
				this.RefreshCrosshair();
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06005259 RID: 21081 RVA: 0x0019926A File Offset: 0x0019746A
		// (set) Token: 0x0600525A RID: 21082 RVA: 0x00199272 File Offset: 0x00197472
		public bool Targeted
		{
			get
			{
				return this.m_targeted;
			}
			set
			{
				this.m_targeted = value;
				this.RefreshCrosshair();
			}
		}

		// Token: 0x0600525B RID: 21083 RVA: 0x00199284 File Offset: 0x00197484
		protected override void Start()
		{
			base.Start();
			this.m_crosshair = base.gameObject.GetComponentInChildren<GrabbableCrosshair>();
			this.m_renderer = base.gameObject.GetComponent<Renderer>();
			this.m_crosshairManager = Object.FindObjectOfType<GrabManager>();
			this.m_mpb = new MaterialPropertyBlock();
			this.RefreshCrosshair();
			this.m_renderer.SetPropertyBlock(this.m_mpb);
		}

		// Token: 0x0600525C RID: 21084 RVA: 0x001992E8 File Offset: 0x001974E8
		private void RefreshCrosshair()
		{
			if (this.m_crosshair)
			{
				if (base.isGrabbed)
				{
					this.m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
				}
				else if (!this.InRange)
				{
					this.m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
				}
				else
				{
					this.m_crosshair.SetState(this.Targeted ? GrabbableCrosshair.CrosshairState.Targeted : GrabbableCrosshair.CrosshairState.Enabled);
				}
			}
			if (this.m_materialColorField != null)
			{
				this.m_renderer.GetPropertyBlock(this.m_mpb);
				if (base.isGrabbed || !this.InRange)
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorOutOfRange);
				}
				else if (this.Targeted)
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorHighlighted);
				}
				else
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorInRange);
				}
				this.m_renderer.SetPropertyBlock(this.m_mpb);
			}
		}

		// Token: 0x04005BC3 RID: 23491
		public string m_materialColorField;

		// Token: 0x04005BC4 RID: 23492
		private GrabbableCrosshair m_crosshair;

		// Token: 0x04005BC5 RID: 23493
		private GrabManager m_crosshairManager;

		// Token: 0x04005BC6 RID: 23494
		private Renderer m_renderer;

		// Token: 0x04005BC7 RID: 23495
		private MaterialPropertyBlock m_mpb;

		// Token: 0x04005BC8 RID: 23496
		private bool m_inRange;

		// Token: 0x04005BC9 RID: 23497
		private bool m_targeted;
	}
}
