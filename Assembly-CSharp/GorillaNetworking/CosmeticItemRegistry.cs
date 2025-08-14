using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D45 RID: 3397
	public class CosmeticItemRegistry
	{
		// Token: 0x0600540F RID: 21519 RVA: 0x0019F7E0 File Offset: 0x0019D9E0
		public void Initialize(GameObject[] cosmeticGObjs)
		{
			if (this._isInitialized)
			{
				return;
			}
			this._isInitialized = true;
			foreach (GameObject gameObject in cosmeticGObjs)
			{
				string text = gameObject.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
				CosmeticItemInstance cosmeticItemInstance;
				if (this.nameToCosmeticMap.ContainsKey(text))
				{
					cosmeticItemInstance = this.nameToCosmeticMap[text];
				}
				else
				{
					cosmeticItemInstance = new CosmeticItemInstance();
					cosmeticItemInstance.clippingOffsets = CosmeticsController.instance.GetClipOffsetsFromDisplayName(text);
					this.nameToCosmeticMap.Add(text, cosmeticItemInstance);
				}
				Object component = gameObject.GetComponent<HoldableObject>();
				bool flag = gameObject.name.Contains("LEFT.");
				bool flag2 = gameObject.name.Contains("RIGHT.");
				if (component != null)
				{
					cosmeticItemInstance.holdableObjects.Add(gameObject);
				}
				else if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(gameObject);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(gameObject);
				}
				else
				{
					cosmeticItemInstance.objects.Add(gameObject);
				}
				cosmeticItemInstance.dbgname = text;
			}
		}

		// Token: 0x06005410 RID: 21520 RVA: 0x0019F904 File Offset: 0x0019DB04
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (!this._isInitialized)
			{
				Debug.LogError("Tried to use CosmeticItemRegistry before it was initialized!");
				return null;
			}
			if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance result;
			if (!this.nameToCosmeticMap.TryGetValue(itemName, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x04005DAC RID: 23980
		private bool _isInitialized;

		// Token: 0x04005DAD RID: 23981
		private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x04005DAE RID: 23982
		private GameObject nullItem;
	}
}
