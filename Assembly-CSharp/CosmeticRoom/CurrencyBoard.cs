using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000CDD RID: 3293
	public class CurrencyBoard : MonoBehaviour
	{
		// Token: 0x060051E1 RID: 20961 RVA: 0x00198094 File Offset: 0x00196294
		public void OnEnable()
		{
			CosmeticsController.instance.AddCurrencyBoard(this);
		}

		// Token: 0x060051E2 RID: 20962 RVA: 0x001980A3 File Offset: 0x001962A3
		public void OnDisable()
		{
			CosmeticsController.instance.RemoveCurrencyBoard(this);
		}

		// Token: 0x060051E3 RID: 20963 RVA: 0x001980B4 File Offset: 0x001962B4
		public void UpdateCurrencyBoard(bool checkedDaily, bool gotDaily, int currencyBalance, int secTilTomorrow)
		{
			if (this.dailyRocksTextTMP.IsNotNull())
			{
				this.dailyRocksTextTMP.text = (checkedDaily ? (gotDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...") : "CHECKING DAILY ROCKS...");
			}
			if (this.currencyBoardTextTMP.IsNotNull())
			{
				this.currencyBoardTextTMP.text = string.Concat(new string[]
				{
					currencyBalance.ToString(),
					"\n\n",
					(secTilTomorrow / 3600).ToString(),
					" HR, ",
					(secTilTomorrow % 3600 / 60).ToString(),
					"MIN"
				});
			}
		}

		// Token: 0x04005B81 RID: 23425
		public TMP_Text dailyRocksTextTMP;

		// Token: 0x04005B82 RID: 23426
		public TMP_Text currencyBoardTextTMP;
	}
}
