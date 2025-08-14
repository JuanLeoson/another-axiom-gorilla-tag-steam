using System;
using UnityEngine;

namespace Docking
{
	// Token: 0x02000F88 RID: 3976
	public class Dockable : MonoBehaviour
	{
		// Token: 0x0600637B RID: 25467 RVA: 0x001F59B8 File Offset: 0x001F3BB8
		protected virtual void OnTriggerEnter(Collider other)
		{
			Dock dock;
			if (other.TryGetComponent<Dock>(out dock))
			{
				this.potentialDock = other.transform;
			}
		}

		// Token: 0x0600637C RID: 25468 RVA: 0x001F59DB File Offset: 0x001F3BDB
		protected virtual void OnTriggerExit(Collider other)
		{
			if (this.potentialDock == other.transform)
			{
				this.potentialDock = null;
			}
		}

		// Token: 0x0600637D RID: 25469 RVA: 0x001F59F8 File Offset: 0x001F3BF8
		public virtual void Dock()
		{
			if (this.potentialDock == null)
			{
				return;
			}
			base.transform.position = this.potentialDock.position;
			base.transform.rotation = this.potentialDock.rotation;
			this.potentialDock = null;
		}

		// Token: 0x04006E78 RID: 28280
		protected Transform potentialDock;
	}
}
