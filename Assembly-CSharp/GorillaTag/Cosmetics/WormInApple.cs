using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F6B RID: 3947
	public class WormInApple : MonoBehaviour
	{
		// Token: 0x060061B4 RID: 25012 RVA: 0x001F10D2 File Offset: 0x001EF2D2
		public void OnHandTap()
		{
			if (this.blendShapeCosmetic && this.blendShapeCosmetic.GetBlendValue() > 0.5f)
			{
				UnityEvent onHandTapped = this.OnHandTapped;
				if (onHandTapped == null)
				{
					return;
				}
				onHandTapped.Invoke();
			}
		}

		// Token: 0x04006DFD RID: 28157
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04006DFE RID: 28158
		public UnityEvent OnHandTapped;
	}
}
