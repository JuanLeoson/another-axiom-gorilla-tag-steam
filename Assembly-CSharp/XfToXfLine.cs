using System;
using UnityEngine;

// Token: 0x02000780 RID: 1920
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x0600302F RID: 12335 RVA: 0x000FD406 File Offset: 0x000FB606
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x000FD414 File Offset: 0x000FB614
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04003C29 RID: 15401
	public Transform pt0;

	// Token: 0x04003C2A RID: 15402
	public Transform pt1;

	// Token: 0x04003C2B RID: 15403
	private LineRenderer lineRenderer;
}
