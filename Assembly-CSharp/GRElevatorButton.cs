using System;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class GRElevatorButton : MonoBehaviour
{
	// Token: 0x06002666 RID: 9830 RVA: 0x000CD100 File Offset: 0x000CB300
	private void Awake()
	{
		if (this.disableDelayed == null)
		{
			this.disableDelayed = this.buttonLit.GetComponent<DisableGameObjectDelayed>();
		}
		if (this.tempLight)
		{
			this.disableDelayed.enabled = false;
			return;
		}
		this.disableDelayed.delayTime = this.litUpTime;
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000CD152 File Offset: 0x000CB352
	public void Pressed()
	{
		this.buttonLit.SetActive(true);
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000CD160 File Offset: 0x000CB360
	public void Depressed()
	{
		this.buttonLit.SetActive(false);
	}

	// Token: 0x040030CA RID: 12490
	public GRElevator.ButtonType buttonType;

	// Token: 0x040030CB RID: 12491
	public GameObject buttonLit;

	// Token: 0x040030CC RID: 12492
	public float litUpTime;

	// Token: 0x040030CD RID: 12493
	public DisableGameObjectDelayed disableDelayed;

	// Token: 0x040030CE RID: 12494
	public bool tempLight;
}
