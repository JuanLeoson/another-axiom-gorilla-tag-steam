using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DCA RID: 3530
	public class StoreBundleData : ScriptableObject
	{
		// Token: 0x0600579B RID: 22427 RVA: 0x001B2628 File Offset: 0x001B0828
		public void OnValidate()
		{
			if (this.playfabBundleID.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE PLAYFAB BUNDLE ID " + base.name);
			}
			if (this.bundleSKU.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE BUNDLE SKU " + base.name);
			}
		}

		// Token: 0x04006160 RID: 24928
		public string playfabBundleID = "NULL";

		// Token: 0x04006161 RID: 24929
		public string bundleSKU = "NULL SKU";

		// Token: 0x04006162 RID: 24930
		public Sprite bundleImage;

		// Token: 0x04006163 RID: 24931
		public string bundleDescriptionText = "THE NULL_BUNDLE PACK WITH 10,000 SHINY ROCKS IN THIS LIMITED TIME DLC!";
	}
}
