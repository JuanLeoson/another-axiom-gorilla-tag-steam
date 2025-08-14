using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F2B RID: 3883
	public class TrashcanCosmetic : MonoBehaviour
	{
		// Token: 0x0600604A RID: 24650 RVA: 0x001E984C File Offset: 0x001E7A4C
		public void OnBasket(bool isLeftHand, Collider other)
		{
			SlingshotProjectile slingshotProjectile;
			if (other.TryGetComponent<SlingshotProjectile>(out slingshotProjectile) && slingshotProjectile.GetDistanceTraveled() >= this.minScoringDistance)
			{
				UnityEvent onScored = this.OnScored;
				if (onScored != null)
				{
					onScored.Invoke();
				}
				slingshotProjectile.DestroyAfterRelease();
			}
		}

		// Token: 0x04006BA6 RID: 27558
		public float minScoringDistance = 2f;

		// Token: 0x04006BA7 RID: 27559
		public UnityEvent OnScored;
	}
}
