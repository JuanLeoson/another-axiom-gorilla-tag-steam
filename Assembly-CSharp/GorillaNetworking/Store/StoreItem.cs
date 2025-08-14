using System;
using System.IO;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000DD9 RID: 3545
	[Serializable]
	public class StoreItem
	{
		// Token: 0x06005804 RID: 22532 RVA: 0x001B4F68 File Offset: 0x001B3168
		public static void SerializeItemsAsJSON(StoreItem[] items)
		{
			string text = "";
			foreach (StoreItem obj in items)
			{
				text = text + JsonUtility.ToJson(obj) + ";";
			}
			Debug.LogError(text);
			File.WriteAllText(Application.dataPath + "/Resources/StoreItems/FeaturedStoreItemsList.json", text);
		}

		// Token: 0x06005805 RID: 22533 RVA: 0x001B4FBC File Offset: 0x001B31BC
		public static void ConvertCosmeticItemToSToreItem(CosmeticsController.CosmeticItem cosmeticItem, ref StoreItem storeItem)
		{
			storeItem.itemName = cosmeticItem.itemName;
			storeItem.itemCategory = (int)cosmeticItem.itemCategory;
			storeItem.itemPictureResourceString = cosmeticItem.itemPictureResourceString;
			storeItem.displayName = cosmeticItem.displayName;
			storeItem.overrideDisplayName = cosmeticItem.overrideDisplayName;
			storeItem.bundledItems = cosmeticItem.bundledItems;
			storeItem.canTryOn = cosmeticItem.canTryOn;
			storeItem.bothHandsHoldable = cosmeticItem.bothHandsHoldable;
			storeItem.AssetBundleName = "";
			storeItem.bUsesMeshAtlas = cosmeticItem.bUsesMeshAtlas;
			storeItem.MeshResourceName = cosmeticItem.meshResourceString;
			storeItem.MeshAtlasResourceName = cosmeticItem.meshAtlasResourceString;
			storeItem.MaterialResrouceName = cosmeticItem.materialResourceString;
		}

		// Token: 0x040061B7 RID: 25015
		public string itemName = "";

		// Token: 0x040061B8 RID: 25016
		public int itemCategory;

		// Token: 0x040061B9 RID: 25017
		public string itemPictureResourceString = "";

		// Token: 0x040061BA RID: 25018
		public string displayName = "";

		// Token: 0x040061BB RID: 25019
		public string overrideDisplayName = "";

		// Token: 0x040061BC RID: 25020
		public string[] bundledItems = new string[0];

		// Token: 0x040061BD RID: 25021
		public bool canTryOn;

		// Token: 0x040061BE RID: 25022
		public bool bothHandsHoldable;

		// Token: 0x040061BF RID: 25023
		public string AssetBundleName = "";

		// Token: 0x040061C0 RID: 25024
		public bool bUsesMeshAtlas;

		// Token: 0x040061C1 RID: 25025
		public string MeshAtlasResourceName = "";

		// Token: 0x040061C2 RID: 25026
		public string MeshResourceName = "";

		// Token: 0x040061C3 RID: 25027
		public string MaterialResrouceName = "";

		// Token: 0x040061C4 RID: 25028
		public Vector3 translationOffset = Vector3.zero;

		// Token: 0x040061C5 RID: 25029
		public Vector3 rotationOffset = Vector3.zero;

		// Token: 0x040061C6 RID: 25030
		public Vector3 scale = Vector3.one;
	}
}
