using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class NonCosmeticItemProvider : MonoBehaviour
{
	// Token: 0x060012BE RID: 4798 RVA: 0x00066ECC File Offset: 0x000650CC
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
			{
				true,
				component.isLeftHand
			});
		}
	}

	// Token: 0x04001A59 RID: 6745
	public GTZone zone;

	// Token: 0x04001A5A RID: 6746
	[Tooltip("only for honeycomb")]
	public bool useCondition;

	// Token: 0x04001A5B RID: 6747
	public int conditionThreshold;

	// Token: 0x04001A5C RID: 6748
	public NonCosmeticItemProvider.ItemType itemType;

	// Token: 0x0200030C RID: 780
	public enum ItemType
	{
		// Token: 0x04001A5E RID: 6750
		honeycomb
	}
}
