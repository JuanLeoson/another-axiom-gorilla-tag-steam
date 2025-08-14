using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000CDE RID: 3294
	public class FittingRoom : MonoBehaviour
	{
		// Token: 0x060051E5 RID: 20965 RVA: 0x00198160 File Offset: 0x00196360
		public void InitializeForCustomMap(bool useCustomConsoleMesh = true)
		{
			GameObject gameObject = this.consoleMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomConsoleMesh);
			}
			CosmeticsController.instance.AddFittingRoom(this);
		}

		// Token: 0x060051E6 RID: 20966 RVA: 0x00198184 File Offset: 0x00196384
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticSet tryOnSet)
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isInTryOnSet = CosmeticsController.instance.AnyMatch(tryOnSet, currentCart[this.iterator]);
					this.fittingRoomButtons[this.iterator].SetItem(currentCart[this.iterator], isInTryOnSet);
				}
				else
				{
					this.fittingRoomButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x04005B83 RID: 23427
		public FittingRoomButton[] fittingRoomButtons;

		// Token: 0x04005B84 RID: 23428
		public GameObject consoleMesh;

		// Token: 0x04005B85 RID: 23429
		private int iterator;
	}
}
