using System;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class WingsWearable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000624 RID: 1572 RVA: 0x00023AF3 File Offset: 0x00021CF3
	private void Awake()
	{
		this.xform = this.animator.transform;
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00023B06 File Offset: 0x00021D06
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.oldPos = this.xform.localPosition;
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00023B20 File Offset: 0x00021D20
	public void SliceUpdate()
	{
		Vector3 position = this.xform.position;
		float f = (position - this.oldPos).magnitude / Time.deltaTime;
		float value = this.flapSpeedCurve.Evaluate(Mathf.Abs(f));
		this.animator.SetFloat(this.flapSpeedParamID, value);
		this.oldPos = position;
	}

	// Token: 0x0400074B RID: 1867
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	// Token: 0x0400074C RID: 1868
	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	// Token: 0x0400074D RID: 1869
	private Transform xform;

	// Token: 0x0400074E RID: 1870
	private Vector3 oldPos;

	// Token: 0x0400074F RID: 1871
	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
