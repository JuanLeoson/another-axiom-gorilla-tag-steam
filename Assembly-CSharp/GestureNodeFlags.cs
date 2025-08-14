using System;

// Token: 0x02000198 RID: 408
[Flags]
public enum GestureNodeFlags : uint
{
	// Token: 0x04000C6F RID: 3183
	None = 0U,
	// Token: 0x04000C70 RID: 3184
	HandLeft = 1U,
	// Token: 0x04000C71 RID: 3185
	HandRight = 2U,
	// Token: 0x04000C72 RID: 3186
	HandOpen = 4U,
	// Token: 0x04000C73 RID: 3187
	HandClosed = 8U,
	// Token: 0x04000C74 RID: 3188
	DigitOpen = 16U,
	// Token: 0x04000C75 RID: 3189
	DigitClosed = 32U,
	// Token: 0x04000C76 RID: 3190
	DigitBent = 64U,
	// Token: 0x04000C77 RID: 3191
	TowardFace = 128U,
	// Token: 0x04000C78 RID: 3192
	AwayFromFace = 256U,
	// Token: 0x04000C79 RID: 3193
	AxisWorldUp = 512U,
	// Token: 0x04000C7A RID: 3194
	AxisWorldDown = 1024U
}
