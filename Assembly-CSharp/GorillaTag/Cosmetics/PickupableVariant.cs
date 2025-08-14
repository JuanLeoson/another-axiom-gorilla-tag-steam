using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F26 RID: 3878
	public class PickupableVariant : MonoBehaviour
	{
		// Token: 0x06006019 RID: 24601 RVA: 0x000023F5 File Offset: 0x000005F5
		protected internal virtual void Release(HoldableObject holdable, Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
		}

		// Token: 0x0600601A RID: 24602 RVA: 0x000023F5 File Offset: 0x000005F5
		protected internal virtual void Pickup()
		{
		}

		// Token: 0x0600601B RID: 24603 RVA: 0x000023F5 File Offset: 0x000005F5
		protected internal virtual void DelayedPickup()
		{
		}
	}
}
