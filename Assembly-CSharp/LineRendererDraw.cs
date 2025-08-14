using System;
using UnityEngine;

// Token: 0x0200029C RID: 668
public class LineRendererDraw : MonoBehaviour
{
	// Token: 0x06000F70 RID: 3952 RVA: 0x0005B2EE File Offset: 0x000594EE
	public void SetUpLine(Transform[] points)
	{
		this.lr.positionCount = points.Length;
		this.points = points;
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0005B308 File Offset: 0x00059508
	private void LateUpdate()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.lr.SetPosition(i, this.points[i].position);
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0005B341 File Offset: 0x00059541
	public void Enable(bool enable)
	{
		this.lr.enabled = enable;
	}

	// Token: 0x04001822 RID: 6178
	public LineRenderer lr;

	// Token: 0x04001823 RID: 6179
	public Transform[] points;
}
