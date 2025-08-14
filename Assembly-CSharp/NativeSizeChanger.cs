using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020002AA RID: 682
public class NativeSizeChanger : MonoBehaviour
{
	// Token: 0x06000FD0 RID: 4048 RVA: 0x0005C1D4 File Offset: 0x0005A3D4
	public void Activate(NativeSizeChangerSettings settings)
	{
		settings.WorldPosition = base.transform.position;
		settings.ActivationTime = Time.time;
		GTPlayer.Instance.SetNativeScale(settings);
	}
}
