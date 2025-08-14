using System;
using UnityEngine;

// Token: 0x0200080D RID: 2061
public class CustomMapsTerminalControlButton : GorillaPressableButton
{
	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x06003392 RID: 13202 RVA: 0x00106461 File Offset: 0x00104661
	// (set) Token: 0x06003393 RID: 13203 RVA: 0x0010BE23 File Offset: 0x0010A023
	public bool IsLocked
	{
		get
		{
			return this.isOn;
		}
		set
		{
			this.isOn = value;
		}
	}

	// Token: 0x06003394 RID: 13204 RVA: 0x0010BE2C File Offset: 0x0010A02C
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.mapsTerminal == null)
		{
			return;
		}
		this.mapsTerminal.HandleTerminalControlButtonPressed();
	}

	// Token: 0x06003395 RID: 13205 RVA: 0x0010BE50 File Offset: 0x0010A050
	public void LockTerminalControl()
	{
		if (this.IsLocked)
		{
			return;
		}
		this.IsLocked = true;
		this.UpdateColor();
		if (this.myText != null)
		{
			this.myText.color = this.lockedTextColor;
			return;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.color = this.lockedTextColor;
		}
	}

	// Token: 0x06003396 RID: 13206 RVA: 0x0010BEB4 File Offset: 0x0010A0B4
	public void UnlockTerminalControl()
	{
		if (!this.IsLocked)
		{
			return;
		}
		this.IsLocked = false;
		this.UpdateColor();
		if (this.myText != null)
		{
			this.myText.color = this.unlockedTextColor;
			return;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.color = this.unlockedTextColor;
		}
	}

	// Token: 0x04004073 RID: 16499
	[SerializeField]
	private Color unlockedTextColor = Color.black;

	// Token: 0x04004074 RID: 16500
	[SerializeField]
	private Color lockedTextColor = Color.white;

	// Token: 0x04004075 RID: 16501
	[SerializeField]
	private CustomMapsTerminal mapsTerminal;
}
