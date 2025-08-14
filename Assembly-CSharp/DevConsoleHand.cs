using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001DD RID: 477
public class DevConsoleHand : DevConsoleInstance
{
	// Token: 0x04000E63 RID: 3683
	public List<GameObject> otherButtonsList;

	// Token: 0x04000E64 RID: 3684
	public bool isStillEnabled = true;

	// Token: 0x04000E65 RID: 3685
	public bool isLeftHand;

	// Token: 0x04000E66 RID: 3686
	public ConsoleMode mode;

	// Token: 0x04000E67 RID: 3687
	public double debugScale;

	// Token: 0x04000E68 RID: 3688
	public double inspectorScale;

	// Token: 0x04000E69 RID: 3689
	public double componentInspectorScale;

	// Token: 0x04000E6A RID: 3690
	public List<GameObject> consoleButtons;

	// Token: 0x04000E6B RID: 3691
	public List<GameObject> inspectorButtons;

	// Token: 0x04000E6C RID: 3692
	public List<GameObject> componentInspectorButtons;

	// Token: 0x04000E6D RID: 3693
	public GorillaDevButton consoleButton;

	// Token: 0x04000E6E RID: 3694
	public GorillaDevButton inspectorButton;

	// Token: 0x04000E6F RID: 3695
	public GorillaDevButton componentInspectorButton;

	// Token: 0x04000E70 RID: 3696
	public GorillaDevButton showNonStarItems;

	// Token: 0x04000E71 RID: 3697
	public GorillaDevButton showPrivateItems;

	// Token: 0x04000E72 RID: 3698
	public Text componentInspectionText;

	// Token: 0x04000E73 RID: 3699
	public DevInspector selectedInspector;
}
