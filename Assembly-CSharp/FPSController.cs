using System;
using UnityEngine;

// Token: 0x02000A83 RID: 2691
public class FPSController : MonoBehaviour
{
	// Token: 0x14000074 RID: 116
	// (add) Token: 0x0600417A RID: 16762 RVA: 0x0014AD74 File Offset: 0x00148F74
	// (remove) Token: 0x0600417B RID: 16763 RVA: 0x0014ADAC File Offset: 0x00148FAC
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x0600417C RID: 16764 RVA: 0x0014ADE4 File Offset: 0x00148FE4
	// (remove) Token: 0x0600417D RID: 16765 RVA: 0x0014AE1C File Offset: 0x0014901C
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x04004CFE RID: 19710
	public float baseMoveSpeed = 4f;

	// Token: 0x04004CFF RID: 19711
	public float shiftMoveSpeed = 8f;

	// Token: 0x04004D00 RID: 19712
	public float ctrlMoveSpeed = 1f;

	// Token: 0x04004D01 RID: 19713
	public float lookHorizontal = 0.4f;

	// Token: 0x04004D02 RID: 19714
	public float lookVertical = 0.25f;

	// Token: 0x04004D03 RID: 19715
	[SerializeField]
	private Vector3 leftControllerPosOffset = new Vector3(-0.2f, -0.25f, 0.3f);

	// Token: 0x04004D04 RID: 19716
	[SerializeField]
	private Vector3 leftControllerRotationOffset = new Vector3(265f, -82f, 28f);

	// Token: 0x04004D05 RID: 19717
	[SerializeField]
	private Vector3 rightControllerPosOffset = new Vector3(0.2f, -0.25f, 0.3f);

	// Token: 0x04004D06 RID: 19718
	[SerializeField]
	private Vector3 rightControllerRotationOffset = new Vector3(263f, 318f, 485f);

	// Token: 0x04004D07 RID: 19719
	[SerializeField]
	private bool toggleGrab;

	// Token: 0x04004D08 RID: 19720
	[SerializeField]
	private bool clampGrab;

	// Token: 0x04004D0B RID: 19723
	private bool controlRightHand;

	// Token: 0x04004D0C RID: 19724
	public LayerMask HandMask;

	// Token: 0x02000A84 RID: 2692
	// (Invoke) Token: 0x06004180 RID: 16768
	public delegate void OnStateChangeEventHandler();
}
