using System;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class TeleportOrientationHandlerHMD : TeleportOrientationHandler
{
	// Token: 0x060013BC RID: 5052 RVA: 0x0006A12C File Offset: 0x0006832C
	protected override void InitializeTeleportDestination()
	{
		this._initialRotation = Quaternion.identity;
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0006A13C File Offset: 0x0006833C
	protected override void UpdateTeleportDestination()
	{
		if (this.AimData.Destination != null && (this.UpdateOrientationDuringAim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport))
		{
			Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
			Vector3 valueOrDefault = this.AimData.Destination.GetValueOrDefault();
			Plane plane = new Plane(Vector3.up, valueOrDefault);
			float d;
			if (plane.Raycast(new Ray(centerEyeAnchor.position, centerEyeAnchor.forward), out d))
			{
				Vector3 vector = centerEyeAnchor.position + centerEyeAnchor.forward * d - valueOrDefault;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				if (magnitude > this.AimDistanceThreshold)
				{
					vector.Normalize();
					Quaternion quaternion = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z), Vector3.up);
					this._initialRotation = quaternion;
					if (this.AimDistanceMaxRange > 0f && magnitude > this.AimDistanceMaxRange)
					{
						this.AimData.TargetValid = false;
					}
					base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(quaternion), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, quaternion)));
					return;
				}
			}
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(this._initialRotation), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, this._initialRotation)));
	}

	// Token: 0x04001B3A RID: 6970
	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public TeleportOrientationHandler.OrientationModes OrientationMode;

	// Token: 0x04001B3B RID: 6971
	[Tooltip("Should the destination orientation be updated during the aim state in addition to the PreTeleport state?")]
	public bool UpdateOrientationDuringAim;

	// Token: 0x04001B3C RID: 6972
	[Tooltip("How far from the destination must the HMD be pointing before using it for orientation")]
	public float AimDistanceThreshold;

	// Token: 0x04001B3D RID: 6973
	[Tooltip("How far from the destination must the HMD be pointing before rejecting the teleport")]
	public float AimDistanceMaxRange;

	// Token: 0x04001B3E RID: 6974
	private Quaternion _initialRotation;
}
