using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000910 RID: 2320
public class AnimationPauser : StateMachineBehaviour
{
	// Token: 0x06003949 RID: 14665 RVA: 0x00129228 File Offset: 0x00127428
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AnimationPauser.<OnStateEnter>d__4 <OnStateEnter>d__;
		<OnStateEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnStateEnter>d__.<>4__this = this;
		<OnStateEnter>d__.animator = animator;
		<OnStateEnter>d__.stateInfo = stateInfo;
		<OnStateEnter>d__.layerIndex = layerIndex;
		<OnStateEnter>d__.<>1__state = -1;
		<OnStateEnter>d__.<>t__builder.Start<AnimationPauser.<OnStateEnter>d__4>(ref <OnStateEnter>d__);
	}

	// Token: 0x04004668 RID: 18024
	[SerializeField]
	private int _maxTimeBetweenAnims = 5;

	// Token: 0x04004669 RID: 18025
	[SerializeField]
	private int _minTimeBetweenAnims = 1;

	// Token: 0x0400466A RID: 18026
	private int _animPauseDuration;

	// Token: 0x0400466B RID: 18027
	private static readonly string Restart_Anim_Name = "RestartAnim";
}
