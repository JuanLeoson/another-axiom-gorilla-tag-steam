using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020007B3 RID: 1971
public class SpoonClacker : MonoBehaviour
{
	// Token: 0x06003179 RID: 12665 RVA: 0x001013CE File Offset: 0x000FF5CE
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x001013D8 File Offset: 0x000FF5D8
	private void Setup()
	{
		JointLimits limits = this.hingeJoint.limits;
		this.hingeMin = limits.min;
		this.hingeMax = limits.max;
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x0010140C File Offset: 0x000FF60C
	private void Update()
	{
		if (!this.transferObject)
		{
			return;
		}
		TransferrableObject.PositionState currentState = this.transferObject.currentState;
		if (currentState != TransferrableObject.PositionState.InLeftHand && currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		float num = MathUtils.Linear(this.hingeJoint.angle, this.hingeMin, this.hingeMax, 0f, 1f);
		float value = (this.invertOut ? (1f - num) : num) * 100f;
		this.skinnedMesh.SetBlendShapeWeight(this.targetBlendShape, value);
		if (!this._lockMin && num <= this.minThreshold)
		{
			this.OnHitMin.Invoke();
			this._lockMin = true;
		}
		else if (!this._lockMax && num >= 1f - this.maxThreshold)
		{
			this.OnHitMax.Invoke();
			this._lockMax = true;
			if (this._sincelastHit.HasElapsed(this.multiHitCutoff, true))
			{
				this.soundsSingle.Play();
			}
			else
			{
				this.soundsMulti.Play();
			}
		}
		if (this._lockMin && num > this.minThreshold * this.hysterisisFactor)
		{
			this._lockMin = false;
		}
		if (this._lockMax && num < 1f - this.maxThreshold * this.hysterisisFactor)
		{
			this._lockMax = false;
		}
	}

	// Token: 0x04003D18 RID: 15640
	public TransferrableObject transferObject;

	// Token: 0x04003D19 RID: 15641
	public SkinnedMeshRenderer skinnedMesh;

	// Token: 0x04003D1A RID: 15642
	public HingeJoint hingeJoint;

	// Token: 0x04003D1B RID: 15643
	public int targetBlendShape;

	// Token: 0x04003D1C RID: 15644
	public float hingeMin;

	// Token: 0x04003D1D RID: 15645
	public float hingeMax;

	// Token: 0x04003D1E RID: 15646
	public bool invertOut;

	// Token: 0x04003D1F RID: 15647
	public float minThreshold = 0.01f;

	// Token: 0x04003D20 RID: 15648
	public float maxThreshold = 0.01f;

	// Token: 0x04003D21 RID: 15649
	public float hysterisisFactor = 4f;

	// Token: 0x04003D22 RID: 15650
	public UnityEvent OnHitMin;

	// Token: 0x04003D23 RID: 15651
	public UnityEvent OnHitMax;

	// Token: 0x04003D24 RID: 15652
	private bool _lockMin;

	// Token: 0x04003D25 RID: 15653
	private bool _lockMax;

	// Token: 0x04003D26 RID: 15654
	public SoundBankPlayer soundsSingle;

	// Token: 0x04003D27 RID: 15655
	public SoundBankPlayer soundsMulti;

	// Token: 0x04003D28 RID: 15656
	private TimeSince _sincelastHit;

	// Token: 0x04003D29 RID: 15657
	[FormerlySerializedAs("multiHitInterval")]
	public float multiHitCutoff = 0.1f;
}
