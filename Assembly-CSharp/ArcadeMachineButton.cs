using System;
using UnityEngine;

// Token: 0x020002A6 RID: 678
public class ArcadeMachineButton : GorillaPressableButton
{
	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06000FBA RID: 4026 RVA: 0x0005BF84 File Offset: 0x0005A184
	// (remove) Token: 0x06000FBB RID: 4027 RVA: 0x0005BFBC File Offset: 0x0005A1BC
	public event ArcadeMachineButton.ArcadeMachineButtonEvent OnStateChange;

	// Token: 0x06000FBC RID: 4028 RVA: 0x0005BFF1 File Offset: 0x0005A1F1
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.state)
		{
			this.state = true;
			if (this.OnStateChange != null)
			{
				this.OnStateChange(this.ButtonID, this.state);
			}
		}
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0005C028 File Offset: 0x0005A228
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled || !this.state)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.state = false;
		if (this.OnStateChange != null)
		{
			this.OnStateChange(this.ButtonID, this.state);
		}
	}

	// Token: 0x04001851 RID: 6225
	private bool state;

	// Token: 0x04001852 RID: 6226
	[SerializeField]
	private int ButtonID;

	// Token: 0x020002A7 RID: 679
	// (Invoke) Token: 0x06000FC0 RID: 4032
	public delegate void ArcadeMachineButtonEvent(int id, bool state);
}
