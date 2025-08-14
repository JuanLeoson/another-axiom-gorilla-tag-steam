using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class DevConsoleInstance : MonoBehaviour
{
	// Token: 0x06000B9C RID: 2972 RVA: 0x00020127 File Offset: 0x0001E327
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000E74 RID: 3700
	public GorillaDevButton[] buttons;

	// Token: 0x04000E75 RID: 3701
	public GameObject[] disableWhileActive;

	// Token: 0x04000E76 RID: 3702
	public GameObject[] enableWhileActive;

	// Token: 0x04000E77 RID: 3703
	public float maxHeight;

	// Token: 0x04000E78 RID: 3704
	public float lineHeight;

	// Token: 0x04000E79 RID: 3705
	public int targetLogIndex = -1;

	// Token: 0x04000E7A RID: 3706
	public int currentLogIndex;

	// Token: 0x04000E7B RID: 3707
	public int expandAmount = 20;

	// Token: 0x04000E7C RID: 3708
	public int expandedMessageIndex = -1;

	// Token: 0x04000E7D RID: 3709
	public bool canExpand = true;

	// Token: 0x04000E7E RID: 3710
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04000E7F RID: 3711
	public HashSet<LogType> selectedLogTypes = new HashSet<LogType>
	{
		LogType.Error,
		LogType.Exception,
		LogType.Log,
		LogType.Warning,
		LogType.Assert
	};

	// Token: 0x04000E80 RID: 3712
	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	// Token: 0x04000E81 RID: 3713
	[SerializeField]
	private GorillaDevButton BottomButton;

	// Token: 0x04000E82 RID: 3714
	public float lineStartHeight;

	// Token: 0x04000E83 RID: 3715
	public float lineStartZ;

	// Token: 0x04000E84 RID: 3716
	public float textStartHeight;

	// Token: 0x04000E85 RID: 3717
	public float lineStartTextWidth;

	// Token: 0x04000E86 RID: 3718
	public double textScale = 0.5;

	// Token: 0x04000E87 RID: 3719
	public bool isEnabled = true;

	// Token: 0x04000E88 RID: 3720
	[SerializeField]
	private GameObject ConsoleLineExample;
}
