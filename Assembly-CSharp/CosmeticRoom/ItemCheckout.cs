using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmeticRoom
{
	// Token: 0x02000CDF RID: 3295
	public class ItemCheckout : MonoBehaviour
	{
		// Token: 0x060051E8 RID: 20968 RVA: 0x00198214 File Offset: 0x00196414
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene, bool useCustomCounterMesh = true)
		{
			GameObject gameObject = this.checkoutCounterMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomCounterMesh);
			}
			GameObject gameObject2 = this.purchaseScreenMesh;
			if (gameObject2 != null)
			{
				gameObject2.SetActive(useCustomCounterMesh);
			}
			this.originalScene = customMapScene;
			customMapTryOnArea.AddCollider(this.checkoutTryOnArea);
			CosmeticsController.instance.AddItemCheckout(this);
		}

		// Token: 0x060051E9 RID: 20969 RVA: 0x00198268 File Offset: 0x00196468
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (customMapTryOnArea.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.checkoutTryOnArea);
		}

		// Token: 0x060051EA RID: 20970 RVA: 0x00198280 File Offset: 0x00196480
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticItem itemToBuy)
		{
			this.iterator = 0;
			while (this.iterator < this.checkoutCartButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isCurrentItemToBuy = currentCart[this.iterator].itemName == itemToBuy.itemName;
					this.checkoutCartButtons[this.iterator].SetItem(currentCart[this.iterator], isCurrentItemToBuy);
				}
				else
				{
					this.checkoutCartButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x060051EB RID: 20971 RVA: 0x00198314 File Offset: 0x00196514
		public void UpdatePurchaseText(string newText, string leftPurchaseButtonText, string rightPurchaseButtonText, bool leftButtonOn, bool rightButtonOn)
		{
			if (this.purchaseText.IsNotNull())
			{
				this.purchaseText.text = newText;
			}
			if (this.purchaseTextTMP.IsNotNull())
			{
				this.purchaseTextTMP.text = newText;
			}
			if (!leftPurchaseButtonText.IsNullOrEmpty())
			{
				this.leftPurchaseButton.SetText(leftPurchaseButtonText);
				this.leftPurchaseButton.buttonRenderer.material = (leftButtonOn ? this.leftPurchaseButton.pressedMaterial : this.leftPurchaseButton.unpressedMaterial);
			}
			if (!rightPurchaseButtonText.IsNullOrEmpty())
			{
				this.rightPurchaseButton.SetText(rightPurchaseButtonText);
				this.rightPurchaseButton.buttonRenderer.material = (rightButtonOn ? this.rightPurchaseButton.pressedMaterial : this.rightPurchaseButton.unpressedMaterial);
			}
		}

		// Token: 0x060051EC RID: 20972 RVA: 0x001983D3 File Offset: 0x001965D3
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x04005B86 RID: 23430
		public CheckoutCartButton[] checkoutCartButtons;

		// Token: 0x04005B87 RID: 23431
		public PurchaseItemButton leftPurchaseButton;

		// Token: 0x04005B88 RID: 23432
		public PurchaseItemButton rightPurchaseButton;

		// Token: 0x04005B89 RID: 23433
		[HideInInspector]
		public Text purchaseText;

		// Token: 0x04005B8A RID: 23434
		public TMP_Text purchaseTextTMP;

		// Token: 0x04005B8B RID: 23435
		public HeadModel checkoutHeadModel;

		// Token: 0x04005B8C RID: 23436
		public Collider checkoutTryOnArea;

		// Token: 0x04005B8D RID: 23437
		public GameObject checkoutCounterMesh;

		// Token: 0x04005B8E RID: 23438
		public GameObject purchaseScreenMesh;

		// Token: 0x04005B8F RID: 23439
		private Scene originalScene;

		// Token: 0x04005B90 RID: 23440
		private int iterator;
	}
}
