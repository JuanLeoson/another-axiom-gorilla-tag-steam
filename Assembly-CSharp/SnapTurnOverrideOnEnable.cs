using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000B09 RID: 2825
public class SnapTurnOverrideOnEnable : MonoBehaviour, ISnapTurnOverride
{
	// Token: 0x0600440D RID: 17421 RVA: 0x0015582C File Offset: 0x00153A2C
	private void OnEnable()
	{
		if (this.snapTurn == null && GorillaTagger.Instance != null)
		{
			this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
		}
		if (this.snapTurn != null)
		{
			this.snapTurnOverride = true;
			this.snapTurn.SetTurningOverride(this);
		}
	}

	// Token: 0x0600440E RID: 17422 RVA: 0x00155885 File Offset: 0x00153A85
	private void OnDisable()
	{
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
	}

	// Token: 0x0600440F RID: 17423 RVA: 0x001558A2 File Offset: 0x00153AA2
	bool ISnapTurnOverride.TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x04004E63 RID: 20067
	private GorillaSnapTurn snapTurn;

	// Token: 0x04004E64 RID: 20068
	private bool snapTurnOverride;
}
