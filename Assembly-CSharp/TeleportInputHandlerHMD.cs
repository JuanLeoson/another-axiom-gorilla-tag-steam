using System;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class TeleportInputHandlerHMD : TeleportInputHandler
{
	// Token: 0x1700022B RID: 555
	// (get) Token: 0x060013A1 RID: 5025 RVA: 0x00069BBE File Offset: 0x00067DBE
	// (set) Token: 0x060013A2 RID: 5026 RVA: 0x00069BC6 File Offset: 0x00067DC6
	public Transform Pointer { get; private set; }

	// Token: 0x060013A3 RID: 5027 RVA: 0x00069BD0 File Offset: 0x00067DD0
	public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!base.isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(this.TeleportButton, OVRInput.Controller.Active))
		{
			if (!this.FastTeleport)
			{
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
			}
			return LocomotionTeleport.TeleportIntentions.Teleport;
		}
		else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			if (OVRInput.GetUp(this.TeleportButton, OVRInput.Controller.Active))
			{
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			return LocomotionTeleport.TeleportIntentions.PreTeleport;
		}
		else
		{
			if (OVRInput.Get(this.AimButton, OVRInput.Controller.Active))
			{
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			if (this.AimButton == this.TeleportButton)
			{
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			return LocomotionTeleport.TeleportIntentions.None;
		}
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x00069C5C File Offset: 0x00067E5C
	public override void GetAimData(out Ray aimRay)
	{
		Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
		aimRay = new Ray(centerEyeAnchor.position, centerEyeAnchor.forward);
	}

	// Token: 0x04001B17 RID: 6935
	[Tooltip("The button used to begin aiming for a teleport.")]
	public OVRInput.RawButton AimButton;

	// Token: 0x04001B18 RID: 6936
	[Tooltip("The button used to trigger the teleport after aiming. It can be the same button as the AimButton, however you cannot abort a teleport if it is.")]
	public OVRInput.RawButton TeleportButton;

	// Token: 0x04001B19 RID: 6937
	[Tooltip("When true, the system will not use the PreTeleport intention which will allow a teleport to occur on a button downpress. When false, the button downpress will trigger the PreTeleport intention and the Teleport intention when the button is released.")]
	public bool FastTeleport;
}
