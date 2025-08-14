using System;
using UnityEngine;

// Token: 0x020007DC RID: 2012
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x06003258 RID: 12888 RVA: 0x00106461 File Offset: 0x00104661
	// (set) Token: 0x06003259 RID: 12889 RVA: 0x00106469 File Offset: 0x00104669
	public bool on
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				this.isOn = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x00106481 File Offset: 0x00104681
	public void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x04003F1C RID: 16156
	public DevButtonType Type;

	// Token: 0x04003F1D RID: 16157
	public LogType levelType;

	// Token: 0x04003F1E RID: 16158
	public DevConsoleInstance targetConsole;

	// Token: 0x04003F1F RID: 16159
	public int lineNumber;

	// Token: 0x04003F20 RID: 16160
	public bool repeatIfHeld;

	// Token: 0x04003F21 RID: 16161
	public float holdForSeconds;

	// Token: 0x04003F22 RID: 16162
	private Coroutine pressCoroutine;
}
