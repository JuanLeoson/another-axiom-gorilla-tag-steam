using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000419 RID: 1049
public class BetaChecker : MonoBehaviour
{
	// Token: 0x06001981 RID: 6529 RVA: 0x00089547 File Offset: 0x00087747
	private void Start()
	{
		if (PlayerPrefs.GetString("CheckedBox2") == "true")
		{
			this.doNotEnable = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x00089574 File Offset: 0x00087774
	private void Update()
	{
		if (!this.doNotEnable)
		{
			if (CosmeticsController.instance.confirmedDidntPlayInBeta)
			{
				PlayerPrefs.SetString("CheckedBox2", "true");
				PlayerPrefs.Save();
				base.gameObject.SetActive(false);
				return;
			}
			if (CosmeticsController.instance.playedInBeta)
			{
				GameObject[] array = this.objectsToEnable;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				this.doNotEnable = true;
			}
		}
	}

	// Token: 0x040021DE RID: 8670
	public GameObject[] objectsToEnable;

	// Token: 0x040021DF RID: 8671
	public bool doNotEnable;
}
