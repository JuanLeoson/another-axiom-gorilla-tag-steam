using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000820 RID: 2080
public class CustomMapTelemetryTrigger : MonoBehaviour
{
	// Token: 0x06003433 RID: 13363 RVA: 0x00110017 File Offset: 0x0010E217
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x06003434 RID: 13364 RVA: 0x00110037 File Offset: 0x0010E237
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.StartMapTracking();
		}
	}
}
