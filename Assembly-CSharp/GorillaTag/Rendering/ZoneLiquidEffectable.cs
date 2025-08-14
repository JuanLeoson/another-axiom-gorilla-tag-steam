using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EF9 RID: 3833
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x06005F04 RID: 24324 RVA: 0x001DF1EB File Offset: 0x001DD3EB
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x06005F05 RID: 24325 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnEnable()
		{
		}

		// Token: 0x06005F06 RID: 24326 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnDisable()
		{
		}

		// Token: 0x04006969 RID: 26985
		public float radius = 1f;

		// Token: 0x0400696A RID: 26986
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x0400696B RID: 26987
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x0400696C RID: 26988
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
