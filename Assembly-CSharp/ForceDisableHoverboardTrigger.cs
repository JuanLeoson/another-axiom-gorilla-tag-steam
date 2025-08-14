using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000821 RID: 2081
public class ForceDisableHoverboardTrigger : MonoBehaviour
{
	// Token: 0x06003436 RID: 13366 RVA: 0x00110065 File Offset: 0x0010E265
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.wasEnabled = GTPlayer.Instance.isHoverAllowed;
			GTPlayer.Instance.SetHoverAllowed(false, true);
		}
	}

	// Token: 0x06003437 RID: 13367 RVA: 0x00110098 File Offset: 0x0010E298
	public void OnTriggerExit(Collider other)
	{
		if (!this.reEnableOnExit || !this.wasEnabled)
		{
			return;
		}
		if (this.reEnableOnlyInVStump && !GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			return;
		}
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x0400411B RID: 16667
	[Tooltip("If TRUE and the Hoverboard was enabled when the player entered this trigger, it will be re-enabled when they exit.")]
	public bool reEnableOnExit = true;

	// Token: 0x0400411C RID: 16668
	public bool reEnableOnlyInVStump = true;

	// Token: 0x0400411D RID: 16669
	private bool wasEnabled;
}
