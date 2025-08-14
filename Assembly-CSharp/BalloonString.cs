using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003CE RID: 974
public class BalloonString : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060016AE RID: 5806 RVA: 0x0007B88C File Offset: 0x00079A8C
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.vertices = new List<Vector3>(this.numSegments + 1);
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.vertices.Add(this.startPositionXf.position);
			int num = this.vertices.Count - 2;
			for (int i = 0; i < num; i++)
			{
				float t = (float)((i + 1) / (this.vertices.Count - 1));
				Vector3 item = Vector3.Lerp(this.startPositionXf.position, this.endPositionXf.position, t);
				this.vertices.Add(item);
			}
			this.vertices.Add(this.endPositionXf.position);
		}
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x0007B95C File Offset: 0x00079B5C
	private void UpdateDynamics()
	{
		this.vertices[0] = this.startPositionXf.position;
		this.vertices[this.vertices.Count - 1] = this.endPositionXf.position;
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x0007B998 File Offset: 0x00079B98
	private void UpdateRenderPositions()
	{
		this.lineRenderer.SetPosition(0, this.startPositionXf.transform.position);
		this.lineRenderer.SetPosition(1, this.endPositionXf.transform.position);
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x0007B9D2 File Offset: 0x00079BD2
	public void SliceUpdate()
	{
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.UpdateDynamics();
			this.UpdateRenderPositions();
		}
	}

	// Token: 0x04001E7E RID: 7806
	public Transform startPositionXf;

	// Token: 0x04001E7F RID: 7807
	public Transform endPositionXf;

	// Token: 0x04001E80 RID: 7808
	private List<Vector3> vertices;

	// Token: 0x04001E81 RID: 7809
	public int numSegments = 1;

	// Token: 0x04001E82 RID: 7810
	private LineRenderer lineRenderer;
}
