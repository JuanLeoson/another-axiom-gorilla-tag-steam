using System;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class RaceConsoleVisual : MonoBehaviour
{
	// Token: 0x06000CC3 RID: 3267 RVA: 0x0004477C File Offset: 0x0004297C
	public void ShowRaceInProgress(int laps)
	{
		this.button1.sharedMaterial = this.inactiveButton;
		this.button3.sharedMaterial = this.inactiveButton;
		this.button5.sharedMaterial = this.inactiveButton;
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		switch (laps)
		{
		default:
			this.button1.sharedMaterial = this.selectedButton;
			this.button1.transform.localPosition = this.buttonPressedOffset;
			return;
		case 3:
			this.button3.sharedMaterial = this.selectedButton;
			this.button3.transform.localPosition = this.buttonPressedOffset;
			return;
		case 5:
			this.button5.sharedMaterial = this.selectedButton;
			this.button5.transform.localPosition = this.buttonPressedOffset;
			return;
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00044890 File Offset: 0x00042A90
	public void ShowCanStartRace()
	{
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		this.button1.sharedMaterial = this.pressableButton;
		this.button3.sharedMaterial = this.pressableButton;
		this.button5.sharedMaterial = this.pressableButton;
	}

	// Token: 0x04000FB3 RID: 4019
	[SerializeField]
	private MeshRenderer button1;

	// Token: 0x04000FB4 RID: 4020
	[SerializeField]
	private MeshRenderer button3;

	// Token: 0x04000FB5 RID: 4021
	[SerializeField]
	private MeshRenderer button5;

	// Token: 0x04000FB6 RID: 4022
	[SerializeField]
	private Vector3 buttonPressedOffset;

	// Token: 0x04000FB7 RID: 4023
	[SerializeField]
	private Material pressableButton;

	// Token: 0x04000FB8 RID: 4024
	[SerializeField]
	private Material selectedButton;

	// Token: 0x04000FB9 RID: 4025
	[SerializeField]
	private Material inactiveButton;
}
