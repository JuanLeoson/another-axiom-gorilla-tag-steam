using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000335 RID: 821
public abstract class TeleportOrientationHandler : TeleportSupport
{
	// Token: 0x060013AA RID: 5034 RVA: 0x00069F70 File Offset: 0x00068170
	protected TeleportOrientationHandler()
	{
		this._updateOrientationAction = delegate()
		{
			base.StartCoroutine(this.UpdateOrientationCoroutine());
		};
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x00069F9C File Offset: 0x0006819C
	private void UpdateAimData(LocomotionTeleport.AimData aimData)
	{
		this.AimData = aimData;
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x00069FA5 File Offset: 0x000681A5
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x00069FCF File Offset: 0x000681CF
	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x00069FF9 File Offset: 0x000681F9
	private IEnumerator UpdateOrientationCoroutine()
	{
		this.InitializeTeleportDestination();
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport)
		{
			if (this.AimData != null)
			{
				this.UpdateTeleportDestination();
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060013AF RID: 5039
	protected abstract void InitializeTeleportDestination();

	// Token: 0x060013B0 RID: 5040
	protected abstract void UpdateTeleportDestination();

	// Token: 0x060013B1 RID: 5041 RVA: 0x0006A008 File Offset: 0x00068208
	protected Quaternion GetLandingOrientation(TeleportOrientationHandler.OrientationModes mode, Quaternion rotation)
	{
		if (mode != TeleportOrientationHandler.OrientationModes.HeadRelative)
		{
			return rotation * Quaternion.Euler(0f, -base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.localEulerAngles.y, 0f);
		}
		return rotation;
	}

	// Token: 0x04001B31 RID: 6961
	private readonly Action _updateOrientationAction;

	// Token: 0x04001B32 RID: 6962
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	// Token: 0x04001B33 RID: 6963
	protected LocomotionTeleport.AimData AimData;

	// Token: 0x02000336 RID: 822
	public enum OrientationModes
	{
		// Token: 0x04001B35 RID: 6965
		HeadRelative,
		// Token: 0x04001B36 RID: 6966
		ForwardFacing
	}
}
