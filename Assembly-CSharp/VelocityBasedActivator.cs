using System;
using UnityEngine;

// Token: 0x02000B50 RID: 2896
[RequireComponent(typeof(GorillaVelocityEstimator))]
public class VelocityBasedActivator : MonoBehaviour
{
	// Token: 0x0600456C RID: 17772 RVA: 0x0015A893 File Offset: 0x00158A93
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x0600456D RID: 17773 RVA: 0x0015A8A4 File Offset: 0x00158AA4
	private void Update()
	{
		this.k += this.velocityEstimator.linearVelocity.sqrMagnitude;
		this.k = Mathf.Max(this.k - Time.deltaTime * this.decay, 0f);
		if (!this.active && this.k > this.threshold)
		{
			this.activate(true);
		}
		if (this.active && this.k < this.threshold)
		{
			this.activate(false);
		}
	}

	// Token: 0x0600456E RID: 17774 RVA: 0x0015A930 File Offset: 0x00158B30
	private void activate(bool v)
	{
		this.active = v;
		for (int i = 0; i < this.activationTargets.Length; i++)
		{
			this.activationTargets[i].SetActive(v);
		}
	}

	// Token: 0x0600456F RID: 17775 RVA: 0x0015A965 File Offset: 0x00158B65
	private void OnDisable()
	{
		if (this.active)
		{
			this.activate(false);
		}
	}

	// Token: 0x04005047 RID: 20551
	[SerializeField]
	private GameObject[] activationTargets;

	// Token: 0x04005048 RID: 20552
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04005049 RID: 20553
	private float k;

	// Token: 0x0400504A RID: 20554
	private bool active;

	// Token: 0x0400504B RID: 20555
	[SerializeField]
	private float decay = 1f;

	// Token: 0x0400504C RID: 20556
	[SerializeField]
	private float threshold = 1f;
}
