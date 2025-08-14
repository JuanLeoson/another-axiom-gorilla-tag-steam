using System;
using GorillaNetworking;

// Token: 0x020007FB RID: 2043
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x0600331C RID: 13084 RVA: 0x0010A23F File Offset: 0x0010843F
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x0010A248 File Offset: 0x00108448
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			this.ButtonActivation();
		}
		if (!this.initialized && GorillaComputer.instance != null)
		{
			this.isOn = GorillaComputer.instance.allowedInCompetitive;
			this.UpdateColor();
			this.initialized = true;
		}
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x0010A2A0 File Offset: 0x001084A0
	public override void ButtonActivation()
	{
		if (!this.isOn)
		{
			base.ButtonActivation();
			GorillaComputer.instance.CompQueueUnlockButtonPress();
			this.isOn = true;
			this.UpdateColor();
		}
	}

	// Token: 0x04004022 RID: 16418
	public string gameMode;

	// Token: 0x04004023 RID: 16419
	private bool initialized;
}
