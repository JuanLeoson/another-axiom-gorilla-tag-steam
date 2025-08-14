using System;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class SpeedDrivenAnim : MonoBehaviour
{
	// Token: 0x06000B1F RID: 2847 RVA: 0x0003B410 File Offset: 0x00039610
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.animator = base.GetComponent<Animator>();
		this.keyHash = Animator.StringToHash(this.animKey);
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0003B43C File Offset: 0x0003963C
	private void Update()
	{
		float target = Mathf.InverseLerp(this.speed0, this.speed1, this.velocityEstimator.linearVelocity.magnitude);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, target, this.maxChangePerSecond * Time.deltaTime);
		this.animator.SetFloat(this.keyHash, this.currentBlend);
	}

	// Token: 0x04000D99 RID: 3481
	[SerializeField]
	private float speed0;

	// Token: 0x04000D9A RID: 3482
	[SerializeField]
	private float speed1 = 1f;

	// Token: 0x04000D9B RID: 3483
	[SerializeField]
	private float maxChangePerSecond = 1f;

	// Token: 0x04000D9C RID: 3484
	[SerializeField]
	private string animKey = "speed";

	// Token: 0x04000D9D RID: 3485
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000D9E RID: 3486
	private Animator animator;

	// Token: 0x04000D9F RID: 3487
	private int keyHash;

	// Token: 0x04000DA0 RID: 3488
	private float currentBlend;
}
