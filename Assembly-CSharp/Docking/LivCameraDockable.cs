using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

namespace Docking
{
	// Token: 0x02000F8A RID: 3978
	public class LivCameraDockable : Dockable
	{
		// Token: 0x06006382 RID: 25474 RVA: 0x001F5AB4 File Offset: 0x001F3CB4
		protected override void OnTriggerEnter(Collider other)
		{
			LivCameraDock livCameraDock;
			if (other.TryGetComponent<LivCameraDock>(out livCameraDock))
			{
				this.livDock = livCameraDock;
				this.potentialDock = other.transform;
			}
		}

		// Token: 0x06006383 RID: 25475 RVA: 0x001F5ADE File Offset: 0x001F3CDE
		protected override void OnTriggerExit(Collider other)
		{
			if (this.livDock != null && other.transform == this.potentialDock.transform)
			{
				this.potentialDock = null;
				this.livDock = null;
			}
		}

		// Token: 0x06006384 RID: 25476 RVA: 0x001F5B14 File Offset: 0x001F3D14
		public override void Dock()
		{
			base.Dock();
			if (this.livDock == null)
			{
				return;
			}
			GTLckController gtlckController = base.GetComponent<GTLckController>() ?? base.GetComponentInParent<GTLckController>();
			if (gtlckController != null)
			{
				gtlckController.ApplyCameraSettings(this.livDock.cameraSettings);
			}
			this.livDock = null;
		}

		// Token: 0x04006E7A RID: 28282
		private LivCameraDock livDock;
	}
}
