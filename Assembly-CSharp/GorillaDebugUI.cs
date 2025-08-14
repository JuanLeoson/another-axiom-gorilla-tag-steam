using System;
using TMPro;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class GorillaDebugUI : MonoBehaviour
{
	// Token: 0x04002676 RID: 9846
	private readonly float Delay = 0.5f;

	// Token: 0x04002677 RID: 9847
	public GameObject parentCanvas;

	// Token: 0x04002678 RID: 9848
	public GameObject rayInteractorLeft;

	// Token: 0x04002679 RID: 9849
	public GameObject rayInteractorRight;

	// Token: 0x0400267A RID: 9850
	[SerializeField]
	private TMP_Dropdown playfabIdDropdown;

	// Token: 0x0400267B RID: 9851
	[SerializeField]
	private TMP_Dropdown roomIdDropdown;

	// Token: 0x0400267C RID: 9852
	[SerializeField]
	private TMP_Dropdown locationDropdown;

	// Token: 0x0400267D RID: 9853
	[SerializeField]
	private TMP_Dropdown playerNameDropdown;

	// Token: 0x0400267E RID: 9854
	[SerializeField]
	private TMP_Dropdown gameModeDropdown;

	// Token: 0x0400267F RID: 9855
	[SerializeField]
	private TMP_Dropdown timeOfDayDropdown;

	// Token: 0x04002680 RID: 9856
	[SerializeField]
	private TMP_Text networkStateTextBox;

	// Token: 0x04002681 RID: 9857
	[SerializeField]
	private TMP_Text gameModeTextBox;

	// Token: 0x04002682 RID: 9858
	[SerializeField]
	private TMP_Text currentRoomTextBox;

	// Token: 0x04002683 RID: 9859
	[SerializeField]
	private TMP_Text playerCountTextBox;

	// Token: 0x04002684 RID: 9860
	[SerializeField]
	private TMP_Text roomVisibilityTextBox;

	// Token: 0x04002685 RID: 9861
	[SerializeField]
	private TMP_Text timeMultiplierTextBox;

	// Token: 0x04002686 RID: 9862
	[SerializeField]
	private TMP_Text versionTextBox;
}
