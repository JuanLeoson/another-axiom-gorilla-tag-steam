using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020007B5 RID: 1973
public class StageMicrophone : MonoBehaviour
{
	// Token: 0x0600317F RID: 12671 RVA: 0x00101593 File Offset: 0x000FF793
	private void Awake()
	{
		StageMicrophone.Instance = this;
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x0010159B File Offset: 0x000FF79B
	public bool IsPlayerAmplified(VRRig player)
	{
		return (player.GetMouthPosition() - base.transform.position).IsShorterThan(this.PickupRadius);
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x001015BE File Offset: 0x000FF7BE
	public float GetPlayerSpatialBlend(VRRig player)
	{
		if (!this.IsPlayerAmplified(player))
		{
			return 0.9f;
		}
		return this.AmplifiedSpatialBlend;
	}

	// Token: 0x04003D2D RID: 15661
	public static StageMicrophone Instance;

	// Token: 0x04003D2E RID: 15662
	[SerializeField]
	private float PickupRadius;

	// Token: 0x04003D2F RID: 15663
	[SerializeField]
	private float AmplifiedSpatialBlend;
}
