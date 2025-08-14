using System;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class GhostLabButton : GorillaPressableButton, IBuildValidation
{
	// Token: 0x0600051E RID: 1310 RVA: 0x0001D900 File Offset: 0x0001BB00
	public bool BuildValidationCheck()
	{
		if (this.ghostLab == null)
		{
			Debug.LogError("ghostlab is missing", this);
			return false;
		}
		return true;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001D91E File Offset: 0x0001BB1E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.ghostLab.DoorButtonPress(this.buttonIndex, this.forSingleDoor);
	}

	// Token: 0x0400061D RID: 1565
	public GhostLab ghostLab;

	// Token: 0x0400061E RID: 1566
	public int buttonIndex;

	// Token: 0x0400061F RID: 1567
	public bool forSingleDoor;
}
