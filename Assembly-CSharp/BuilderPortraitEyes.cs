using System;
using UnityEngine;

// Token: 0x02000515 RID: 1301
public class BuilderPortraitEyes : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001FB3 RID: 8115 RVA: 0x000A77D6 File Offset: 0x000A59D6
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.scale = base.transform.lossyScale.x;
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000A77F5 File Offset: 0x000A59F5
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.eyes.transform.position = this.eyeCenter.transform.position;
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000A7820 File Offset: 0x000A5A20
	public void SliceUpdate()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		Vector3 b = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(GorillaTagger.Instance.headCollider.transform.position - this.eyeCenter.position, this.eyeCenter.forward), this.moveRadius * this.scale);
		this.eyes.transform.position = this.eyeCenter.position + b;
	}

	// Token: 0x04002858 RID: 10328
	[SerializeField]
	private Transform eyeCenter;

	// Token: 0x04002859 RID: 10329
	[SerializeField]
	private GameObject eyes;

	// Token: 0x0400285A RID: 10330
	[SerializeField]
	private float moveRadius = 0.5f;

	// Token: 0x0400285B RID: 10331
	private float scale = 1f;
}
