using System;
using UnityEngine;

// Token: 0x0200066D RID: 1645
public class GRShiftSirenLight : MonoBehaviour
{
	// Token: 0x06002844 RID: 10308 RVA: 0x000D8F68 File Offset: 0x000D7168
	public void Update()
	{
		if (this.shiftManager == null)
		{
			this.shiftManager = GhostReactor.instance.shiftManager;
			return;
		}
		this.redLight.SetActive(this.shiftManager.ShiftActive);
		this.greenLight.SetActive(!this.shiftManager.ShiftActive);
		if (this.readyRoomLight != null)
		{
			this.readyRoomLight.intensity = (this.shiftManager.ShiftActive ? this.dimLight : this.brightLight);
		}
		this.greenLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
		this.redLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
	}

	// Token: 0x040033BB RID: 13243
	public float rotationRate = 1.25f;

	// Token: 0x040033BC RID: 13244
	public Transform greenLightParent;

	// Token: 0x040033BD RID: 13245
	public Transform redLightParent;

	// Token: 0x040033BE RID: 13246
	public GameObject redLight;

	// Token: 0x040033BF RID: 13247
	public GameObject greenLight;

	// Token: 0x040033C0 RID: 13248
	public GhostReactorShiftManager shiftManager;

	// Token: 0x040033C1 RID: 13249
	public float dimLight;

	// Token: 0x040033C2 RID: 13250
	public float brightLight;

	// Token: 0x040033C3 RID: 13251
	public Light readyRoomLight;
}
