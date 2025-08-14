using System;
using UnityEngine;

// Token: 0x0200029D RID: 669
[RequireComponent(typeof(LineRenderer))]
public class LineRenderVelocityMapper : MonoBehaviour
{
	// Token: 0x06000F74 RID: 3956 RVA: 0x0005B34F File Offset: 0x0005954F
	private void Awake()
	{
		this._lr = base.GetComponent<LineRenderer>();
		this._lr.useWorldSpace = true;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0005B36C File Offset: 0x0005956C
	private void LateUpdate()
	{
		if (this.velocityEstimator == null)
		{
			return;
		}
		this._lr.SetPosition(0, this.velocityEstimator.transform.position);
		if (this.velocityEstimator.linearVelocity.sqrMagnitude > 0.1f)
		{
			this._lr.SetPosition(1, this.velocityEstimator.transform.position + this.velocityEstimator.linearVelocity.normalized * 0.2f);
			return;
		}
		this._lr.SetPosition(1, this.velocityEstimator.transform.position);
	}

	// Token: 0x04001824 RID: 6180
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001825 RID: 6181
	private LineRenderer _lr;
}
