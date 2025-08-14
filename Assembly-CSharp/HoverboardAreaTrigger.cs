using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000755 RID: 1877
public class HoverboardAreaTrigger : MonoBehaviour
{
	// Token: 0x06002F0D RID: 12045 RVA: 0x000F93F7 File Offset: 0x000F75F7
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x000F9417 File Offset: 0x000F7617
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(false, false);
		}
	}
}
