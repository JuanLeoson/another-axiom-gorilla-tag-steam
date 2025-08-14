using System;
using UnityEngine;

// Token: 0x0200012E RID: 302
public class RockPiles : MonoBehaviour
{
	// Token: 0x060007E1 RID: 2017 RVA: 0x0002C490 File Offset: 0x0002A690
	public void Show(int visiblePercentage)
	{
		if (visiblePercentage <= 0)
		{
			this.ShowRock(-1);
			return;
		}
		int rockToShow = -1;
		int num = -1;
		for (int i = 0; i < this._rocks.Length; i++)
		{
			RockPiles.RockPile rockPile = this._rocks[i];
			if (visiblePercentage >= rockPile.threshold && num < rockPile.threshold)
			{
				rockToShow = i;
				num = rockPile.threshold;
			}
		}
		this.ShowRock(rockToShow);
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002C4F0 File Offset: 0x0002A6F0
	private void ShowRock(int rockToShow)
	{
		for (int i = 0; i < this._rocks.Length; i++)
		{
			this._rocks[i].visual.SetActive(i == rockToShow);
		}
	}

	// Token: 0x04000982 RID: 2434
	[SerializeField]
	private RockPiles.RockPile[] _rocks;

	// Token: 0x0200012F RID: 303
	[Serializable]
	public struct RockPile
	{
		// Token: 0x04000983 RID: 2435
		public GameObject visual;

		// Token: 0x04000984 RID: 2436
		public int threshold;
	}
}
