using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class PeriodicFoodTopUpper : MonoBehaviour
{
	// Token: 0x06000341 RID: 833 RVA: 0x00013A58 File Offset: 0x00011C58
	private void Awake()
	{
		this.food = base.GetComponentInParent<CrittersFood>();
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00013A68 File Offset: 0x00011C68
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.waitingToRefill && this.food.currentFood == 0f)
		{
			this.waitingToRefill = true;
			this.timeFoodEmpty = Time.time;
		}
		if (this.waitingToRefill && Time.time > this.timeFoodEmpty + this.waitToRefill)
		{
			this.waitingToRefill = false;
			this.food.Initialize();
		}
	}

	// Token: 0x040003CF RID: 975
	private CrittersFood food;

	// Token: 0x040003D0 RID: 976
	private float timeFoodEmpty;

	// Token: 0x040003D1 RID: 977
	private bool waitingToRefill;

	// Token: 0x040003D2 RID: 978
	public float waitToRefill = 10f;

	// Token: 0x040003D3 RID: 979
	public GameObject foodObject;
}
