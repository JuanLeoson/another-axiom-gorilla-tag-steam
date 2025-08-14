using System;

// Token: 0x02000959 RID: 2393
internal struct PlayerAgeGateWarningStatus
{
	// Token: 0x04004820 RID: 18464
	public string header;

	// Token: 0x04004821 RID: 18465
	public string body;

	// Token: 0x04004822 RID: 18466
	public string leftButtonText;

	// Token: 0x04004823 RID: 18467
	public string rightButtonText;

	// Token: 0x04004824 RID: 18468
	public WarningButtonResult leftButtonResult;

	// Token: 0x04004825 RID: 18469
	public WarningButtonResult rightButtonResult;

	// Token: 0x04004826 RID: 18470
	public EImageVisibility showImage;

	// Token: 0x04004827 RID: 18471
	public Action onLeftButtonPressedAction;

	// Token: 0x04004828 RID: 18472
	public Action onRightButtonPressedAction;
}
