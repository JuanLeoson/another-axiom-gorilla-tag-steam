using System;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class BuilderLaserSight : MonoBehaviour
{
	// Token: 0x0600204D RID: 8269 RVA: 0x000AB335 File Offset: 0x000A9535
	public void Awake()
	{
		if (this.lineRenderer == null)
		{
			this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
		}
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000AB36B File Offset: 0x000A956B
	public void SetPoints(Vector3 start, Vector3 end)
	{
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.SetPosition(0, start);
		this.lineRenderer.SetPosition(1, end);
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000AB393 File Offset: 0x000A9593
	public void Show(bool show)
	{
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = show;
		}
	}

	// Token: 0x0400291D RID: 10525
	public LineRenderer lineRenderer;
}
