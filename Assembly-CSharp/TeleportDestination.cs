using System;
using UnityEngine;

// Token: 0x0200032D RID: 813
public class TeleportDestination : MonoBehaviour
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06001381 RID: 4993 RVA: 0x000697B5 File Offset: 0x000679B5
	// (set) Token: 0x06001382 RID: 4994 RVA: 0x000697BD File Offset: 0x000679BD
	public bool IsValidDestination { get; private set; }

	// Token: 0x06001383 RID: 4995 RVA: 0x000697C6 File Offset: 0x000679C6
	private TeleportDestination()
	{
		this._updateTeleportDestinationAction = new Action<bool, Vector3?, Quaternion?, Quaternion?>(this.UpdateTeleportDestination);
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x000697E4 File Offset: 0x000679E4
	public void OnEnable()
	{
		this.PositionIndicator.gameObject.SetActive(false);
		if (this.OrientationIndicator != null)
		{
			this.OrientationIndicator.gameObject.SetActive(false);
		}
		this.LocomotionTeleport.UpdateTeleportDestination += this._updateTeleportDestinationAction;
		this._eventsActive = true;
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x00069839 File Offset: 0x00067A39
	private void TryDisableEventHandlers()
	{
		if (!this._eventsActive)
		{
			return;
		}
		this.LocomotionTeleport.UpdateTeleportDestination -= this._updateTeleportDestinationAction;
		this._eventsActive = false;
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x0006985C File Offset: 0x00067A5C
	public void OnDisable()
	{
		this.TryDisableEventHandlers();
	}

	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06001387 RID: 4999 RVA: 0x00069864 File Offset: 0x00067A64
	// (remove) Token: 0x06001388 RID: 5000 RVA: 0x0006989C File Offset: 0x00067A9C
	public event Action<TeleportDestination> Deactivated;

	// Token: 0x06001389 RID: 5001 RVA: 0x000698D1 File Offset: 0x00067AD1
	public void OnDeactivated()
	{
		if (this.Deactivated != null)
		{
			this.Deactivated(this);
			return;
		}
		this.Recycle();
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x000698EE File Offset: 0x00067AEE
	public void Recycle()
	{
		this.LocomotionTeleport.RecycleTeleportDestination(this);
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x000698FC File Offset: 0x00067AFC
	public virtual void UpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		this.IsValidDestination = isValidDestination;
		this.LandingRotation = landingRotation.GetValueOrDefault();
		GameObject gameObject = this.PositionIndicator.gameObject;
		bool activeInHierarchy = gameObject.activeInHierarchy;
		if (position == null)
		{
			if (activeInHierarchy)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		if (!activeInHierarchy)
		{
			gameObject.SetActive(true);
		}
		base.transform.position = position.GetValueOrDefault();
		if (this.OrientationIndicator == null)
		{
			if (rotation != null)
			{
				base.transform.rotation = rotation.GetValueOrDefault();
			}
			return;
		}
		GameObject gameObject2 = this.OrientationIndicator.gameObject;
		bool activeInHierarchy2 = gameObject2.activeInHierarchy;
		if (rotation == null)
		{
			if (activeInHierarchy2)
			{
				gameObject2.SetActive(false);
			}
			return;
		}
		this.OrientationIndicator.rotation = rotation.GetValueOrDefault();
		if (!activeInHierarchy2)
		{
			gameObject2.SetActive(true);
		}
	}

	// Token: 0x04001B06 RID: 6918
	[Tooltip("If the target handler provides a target position, this transform will be moved to that position and it's game object enabled. A target position being provided does not mean the position is valid, only that the aim handler found something to test as a destination.")]
	public Transform PositionIndicator;

	// Token: 0x04001B07 RID: 6919
	[Tooltip("This transform will be rotated to match the rotation of the aiming target. Simple teleport destinations should assign this to the object containing this component. More complex teleport destinations might assign this to a sub-object that is used to indicate the landing orientation independently from the rest of the destination indicator, such as when world space effects are required. This will typically be a child of the PositionIndicator.")]
	public Transform OrientationIndicator;

	// Token: 0x04001B08 RID: 6920
	[Tooltip("After the player teleports, the character controller will have it's rotation set to this value. It is different from the OrientationIndicator transform.rotation in order to support both head-relative and forward-facing teleport modes (See TeleportOrientationHandlerThumbstick.cs).")]
	public Quaternion LandingRotation;

	// Token: 0x04001B09 RID: 6921
	[NonSerialized]
	public LocomotionTeleport LocomotionTeleport;

	// Token: 0x04001B0A RID: 6922
	[NonSerialized]
	public LocomotionTeleport.States TeleportState;

	// Token: 0x04001B0B RID: 6923
	private readonly Action<bool, Vector3?, Quaternion?, Quaternion?> _updateTeleportDestinationAction;

	// Token: 0x04001B0C RID: 6924
	private bool _eventsActive;
}
