using System;
using UnityEngine;

// Token: 0x020004E9 RID: 1257
public class SetStateConditional : StateMachineBehaviour
{
	// Token: 0x06001E8C RID: 7820 RVA: 0x000A1B57 File Offset: 0x0009FD57
	private void OnValidate()
	{
		this._setToID = this.setToState;
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x000A1B6A File Offset: 0x0009FD6A
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this._didSetup)
		{
			this.parentAnimator = animator;
			this.Setup(animator, stateInfo, layerIndex);
			this._didSetup = true;
		}
		this._sinceEnter = TimeSince.Now();
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000A1B98 File Offset: 0x0009FD98
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.delay > 0f && !this._sinceEnter.HasElapsed(this.delay, true))
		{
			return;
		}
		if (!this.CanSetState(animator, stateInfo, layerIndex))
		{
			return;
		}
		animator.Play(this._setToID);
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x0001D558 File Offset: 0x0001B758
	protected virtual bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		return true;
	}

	// Token: 0x0400273C RID: 10044
	public Animator parentAnimator;

	// Token: 0x0400273D RID: 10045
	public string setToState;

	// Token: 0x0400273E RID: 10046
	[SerializeField]
	private AnimStateHash _setToID;

	// Token: 0x0400273F RID: 10047
	public float delay = 1f;

	// Token: 0x04002740 RID: 10048
	protected TimeSince _sinceEnter;

	// Token: 0x04002741 RID: 10049
	[NonSerialized]
	private bool _didSetup;
}
