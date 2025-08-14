using System;
using UnityEngine;

namespace CosmeticRoom.ItemScripts
{
	// Token: 0x02000CE0 RID: 3296
	public class TrickTreatHoldable : TransferrableObject
	{
		// Token: 0x060051EE RID: 20974 RVA: 0x001983E1 File Offset: 0x001965E1
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.candyCollider)
			{
				this.candyCollider.enabled = (this.IsMyItem() && this.IsHeld());
			}
		}

		// Token: 0x04005B91 RID: 23441
		public MeshCollider candyCollider;
	}
}
