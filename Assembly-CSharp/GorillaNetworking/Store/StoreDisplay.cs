using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DD5 RID: 3541
	public class StoreDisplay : MonoBehaviour
	{
		// Token: 0x060057E5 RID: 22501 RVA: 0x001B4B47 File Offset: 0x001B2D47
		private void GetAllDynamicCosmeticStands()
		{
			this.Stands = base.GetComponentsInChildren<DynamicCosmeticStand>();
		}

		// Token: 0x060057E6 RID: 22502 RVA: 0x001B4B58 File Offset: 0x001B2D58
		private void SetDisplayNameForAllStands()
		{
			DynamicCosmeticStand[] componentsInChildren = base.GetComponentsInChildren<DynamicCosmeticStand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CopyChildsName();
			}
		}

		// Token: 0x040061A7 RID: 24999
		public string displayName = "";

		// Token: 0x040061A8 RID: 25000
		public DynamicCosmeticStand[] Stands;
	}
}
