using System;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class LaserPointer : OVRCursor
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x060012F0 RID: 4848 RVA: 0x00067AA7 File Offset: 0x00065CA7
	// (set) Token: 0x060012EF RID: 4847 RVA: 0x00067A73 File Offset: 0x00065C73
	public LaserPointer.LaserBeamBehavior laserBeamBehavior
	{
		get
		{
			return this._laserBeamBehavior;
		}
		set
		{
			this._laserBeamBehavior = value;
			if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off || this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.OnWhenHitTarget)
			{
				this.lineRenderer.enabled = false;
				return;
			}
			this.lineRenderer.enabled = true;
		}
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00067AAF File Offset: 0x00065CAF
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00067ABD File Offset: 0x00065CBD
	private void Start()
	{
		if (this.cursorVisual)
		{
			this.cursorVisual.SetActive(false);
		}
		OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
		OVRManager.InputFocusLost += this.OnInputFocusLost;
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00067AFA File Offset: 0x00065CFA
	public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
	{
		this._startPoint = start;
		this._endPoint = dest;
		this._hitTarget = true;
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00067B11 File Offset: 0x00065D11
	public override void SetCursorRay(Transform t)
	{
		this._startPoint = t.position;
		this._forward = t.forward;
		this._hitTarget = false;
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00067B34 File Offset: 0x00065D34
	private void LateUpdate()
	{
		this.lineRenderer.SetPosition(0, this._startPoint);
		if (this._hitTarget)
		{
			this.lineRenderer.SetPosition(1, this._endPoint);
			this.UpdateLaserBeam(this._startPoint, this._endPoint);
			if (this.cursorVisual)
			{
				this.cursorVisual.transform.position = this._endPoint;
				this.cursorVisual.SetActive(true);
				return;
			}
		}
		else
		{
			this.UpdateLaserBeam(this._startPoint, this._startPoint + this.maxLength * this._forward);
			this.lineRenderer.SetPosition(1, this._startPoint + this.maxLength * this._forward);
			if (this.cursorVisual)
			{
				this.cursorVisual.SetActive(false);
			}
		}
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00067C1C File Offset: 0x00065E1C
	private void UpdateLaserBeam(Vector3 start, Vector3 end)
	{
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off)
		{
			return;
		}
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.On)
		{
			this.lineRenderer.SetPosition(0, start);
			this.lineRenderer.SetPosition(1, end);
			return;
		}
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.OnWhenHitTarget)
		{
			if (this._hitTarget)
			{
				if (!this.lineRenderer.enabled)
				{
					this.lineRenderer.enabled = true;
					this.lineRenderer.SetPosition(0, start);
					this.lineRenderer.SetPosition(1, end);
					return;
				}
			}
			else if (this.lineRenderer.enabled)
			{
				this.lineRenderer.enabled = false;
			}
		}
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x00067CB4 File Offset: 0x00065EB4
	private void OnDisable()
	{
		if (this.cursorVisual)
		{
			this.cursorVisual.SetActive(false);
		}
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x00067CCF File Offset: 0x00065ECF
	public void OnInputFocusLost()
	{
		if (base.gameObject && base.gameObject.activeInHierarchy)
		{
			this.m_restoreOnInputAcquired = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x00067CFE File Offset: 0x00065EFE
	public void OnInputFocusAcquired()
	{
		if (this.m_restoreOnInputAcquired && base.gameObject)
		{
			this.m_restoreOnInputAcquired = false;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x00067D28 File Offset: 0x00065F28
	private void OnDestroy()
	{
		OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
		OVRManager.InputFocusLost -= this.OnInputFocusLost;
	}

	// Token: 0x04001A87 RID: 6791
	public GameObject cursorVisual;

	// Token: 0x04001A88 RID: 6792
	public float maxLength = 10f;

	// Token: 0x04001A89 RID: 6793
	private LaserPointer.LaserBeamBehavior _laserBeamBehavior;

	// Token: 0x04001A8A RID: 6794
	private bool m_restoreOnInputAcquired;

	// Token: 0x04001A8B RID: 6795
	private Vector3 _startPoint;

	// Token: 0x04001A8C RID: 6796
	private Vector3 _forward;

	// Token: 0x04001A8D RID: 6797
	private Vector3 _endPoint;

	// Token: 0x04001A8E RID: 6798
	private bool _hitTarget;

	// Token: 0x04001A8F RID: 6799
	private LineRenderer lineRenderer;

	// Token: 0x02000319 RID: 793
	public enum LaserBeamBehavior
	{
		// Token: 0x04001A91 RID: 6801
		On,
		// Token: 0x04001A92 RID: 6802
		Off,
		// Token: 0x04001A93 RID: 6803
		OnWhenHitTarget
	}
}
