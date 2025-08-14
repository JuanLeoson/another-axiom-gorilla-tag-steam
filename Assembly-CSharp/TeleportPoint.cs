using System;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class TeleportPoint : MonoBehaviour
{
	// Token: 0x060013C2 RID: 5058 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Start()
	{
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0006A4CF File Offset: 0x000686CF
	public Transform GetDestTransform()
	{
		return this.destTransform;
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0006A4D8 File Offset: 0x000686D8
	private void Update()
	{
		float value = Mathf.SmoothStep(this.fullIntensity, this.lowIntensity, (Time.time - this.lastLookAtTime) * this.dimmingSpeed);
		base.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", value);
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0006A520 File Offset: 0x00068720
	public void OnLookAt()
	{
		this.lastLookAtTime = Time.time;
	}

	// Token: 0x04001B45 RID: 6981
	public float dimmingSpeed = 1f;

	// Token: 0x04001B46 RID: 6982
	public float fullIntensity = 1f;

	// Token: 0x04001B47 RID: 6983
	public float lowIntensity = 0.5f;

	// Token: 0x04001B48 RID: 6984
	public Transform destTransform;

	// Token: 0x04001B49 RID: 6985
	private float lastLookAtTime;
}
