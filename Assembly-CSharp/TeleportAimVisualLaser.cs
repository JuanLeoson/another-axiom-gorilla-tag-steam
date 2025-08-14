using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class TeleportAimVisualLaser : TeleportSupport
{
	// Token: 0x0600137A RID: 4986 RVA: 0x00069647 File Offset: 0x00067847
	public TeleportAimVisualLaser()
	{
		this._enterAimStateAction = new Action(this.EnterAimState);
		this._exitAimStateAction = new Action(this.ExitAimState);
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x00069685 File Offset: 0x00067885
	private void EnterAimState()
	{
		this._lineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x00069698 File Offset: 0x00067898
	private void ExitAimState()
	{
		this._lineRenderer.gameObject.SetActive(false);
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x000696AB File Offset: 0x000678AB
	private void Awake()
	{
		this.LaserPrefab.gameObject.SetActive(false);
		this._lineRenderer = Object.Instantiate<LineRenderer>(this.LaserPrefab);
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x000696CF File Offset: 0x000678CF
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim += this._exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x0006970A File Offset: 0x0006790A
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateAim -= this._enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim -= this._exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
		base.RemoveEventHandlers();
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x00069748 File Offset: 0x00067948
	private void UpdateAimData(LocomotionTeleport.AimData obj)
	{
		this._lineRenderer.sharedMaterial.color = (obj.TargetValid ? Color.green : Color.red);
		List<Vector3> points = obj.Points;
		this._lineRenderer.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++)
		{
			this._lineRenderer.SetPosition(i, points[i]);
		}
	}

	// Token: 0x04001AFF RID: 6911
	[Tooltip("This prefab will be instantiated when the aim visual is awakened, and will be set active when the user is aiming, and deactivated when they are done aiming.")]
	public LineRenderer LaserPrefab;

	// Token: 0x04001B00 RID: 6912
	private readonly Action _enterAimStateAction;

	// Token: 0x04001B01 RID: 6913
	private readonly Action _exitAimStateAction;

	// Token: 0x04001B02 RID: 6914
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	// Token: 0x04001B03 RID: 6915
	private LineRenderer _lineRenderer;

	// Token: 0x04001B04 RID: 6916
	private Vector3[] _linePoints;
}
