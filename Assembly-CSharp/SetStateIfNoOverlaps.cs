using System;
using UnityEngine;

// Token: 0x020004EA RID: 1258
public class SetStateIfNoOverlaps : SetStateConditional
{
	// Token: 0x06001E92 RID: 7826 RVA: 0x000A1BF7 File Offset: 0x0009FDF7
	protected override void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this._volume = animator.GetComponent<VolumeCast>();
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x000A1C05 File Offset: 0x0009FE05
	protected override bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		bool flag = this._volume.CheckOverlaps();
		if (flag)
		{
			this._sinceEnter = 0f;
		}
		return !flag;
	}

	// Token: 0x04002742 RID: 10050
	public VolumeCast _volume;
}
