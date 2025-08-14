using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F67 RID: 3943
	public class UpdateBlendShapeCosmetic : MonoBehaviour
	{
		// Token: 0x0600619D RID: 24989 RVA: 0x001F0847 File Offset: 0x001EEA47
		private void Awake()
		{
			this.targetWeight = this.blendStartWeight;
			this.currentWeight = 0f;
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x001F0860 File Offset: 0x001EEA60
		private void Update()
		{
			this.currentWeight = Mathf.Lerp(this.currentWeight, this.targetWeight, Time.deltaTime * this.blendSpeed);
			this.skinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeIndex, this.currentWeight);
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x001F089C File Offset: 0x001EEA9C
		public void SetBlendValue(bool leftHand, float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x001F08C2 File Offset: 0x001EEAC2
		public void SetBlendValue(float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x001F08E8 File Offset: 0x001EEAE8
		public void FullyBlend()
		{
			this.targetWeight = this.maxBlendShapeWeight;
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x001F08F6 File Offset: 0x001EEAF6
		public void ResetBlend()
		{
			this.targetWeight = 0f;
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x001F0903 File Offset: 0x001EEB03
		public float GetBlendValue()
		{
			return this.skinnedMeshRenderer.GetBlendShapeWeight(this.blendShapeIndex);
		}

		// Token: 0x04006DD1 RID: 28113
		[SerializeField]
		private SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04006DD2 RID: 28114
		public float maxBlendShapeWeight = 100f;

		// Token: 0x04006DD3 RID: 28115
		[SerializeField]
		private int blendShapeIndex;

		// Token: 0x04006DD4 RID: 28116
		[SerializeField]
		private float blendSpeed = 10f;

		// Token: 0x04006DD5 RID: 28117
		[SerializeField]
		private float blendStartWeight;

		// Token: 0x04006DD6 RID: 28118
		[SerializeField]
		private bool invertPassedBlend;

		// Token: 0x04006DD7 RID: 28119
		[Tooltip("If enabled, inverts the passed blend value (0 becomes 1, .2 becomes .8, .65 becomes .45, etc)")]
		private float targetWeight;

		// Token: 0x04006DD8 RID: 28120
		private float currentWeight;
	}
}
