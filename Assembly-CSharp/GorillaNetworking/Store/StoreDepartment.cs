using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DD4 RID: 3540
	public class StoreDepartment : MonoBehaviour
	{
		// Token: 0x060057E3 RID: 22499 RVA: 0x001B4AC4 File Offset: 0x001B2CC4
		private void FindAllDisplays()
		{
			this.Displays = base.GetComponentsInChildren<StoreDisplay>();
			for (int i = this.Displays.Length - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(this.Displays[i].displayName))
				{
					this.Displays[i] = this.Displays[this.Displays.Length - 1];
					Array.Resize<StoreDisplay>(ref this.Displays, this.Displays.Length - 1);
				}
			}
		}

		// Token: 0x040061A5 RID: 24997
		public StoreDisplay[] Displays;

		// Token: 0x040061A6 RID: 24998
		public string departmentName = "";
	}
}
