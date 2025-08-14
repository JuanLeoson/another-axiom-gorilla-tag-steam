using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000B3C RID: 2876
public class TransformOscilation : MonoBehaviour
{
	// Token: 0x06004533 RID: 17715 RVA: 0x00158DB0 File Offset: 0x00156FB0
	private void Start()
	{
		this.rootPos = base.transform.localPosition;
		this.rootRot = base.transform.localRotation.eulerAngles;
	}

	// Token: 0x06004534 RID: 17716 RVA: 0x00158DE8 File Offset: 0x00156FE8
	private void Update()
	{
		if (this.useServerTime && GorillaComputer.instance == null)
		{
			return;
		}
		float num = Time.timeSinceLevelLoad;
		if (this.useServerTime)
		{
			this.dt = GorillaComputer.instance.GetServerTime();
			num = (float)this.dt.Minute * 60f + (float)this.dt.Second + (float)this.dt.Millisecond / 1000f;
		}
		this.offsPos.x = this.PosAmp.x * Mathf.Sin(num * this.PosFreq.x);
		this.offsPos.y = this.PosAmp.y * Mathf.Sin(num * this.PosFreq.y);
		this.offsPos.z = this.PosAmp.z * Mathf.Sin(num * this.PosFreq.z);
		this.offsRot.x = this.RotAmp.x * Mathf.Sin(num * this.RotFreq.x);
		this.offsRot.y = this.RotAmp.y * Mathf.Sin(num * this.RotFreq.y);
		this.offsRot.z = this.RotAmp.z * Mathf.Sin(num * this.RotFreq.z);
		base.transform.localPosition = this.rootPos + this.offsPos;
		base.transform.localRotation = Quaternion.Euler(this.rootRot + this.offsRot);
	}

	// Token: 0x04004F65 RID: 20325
	[SerializeField]
	private Vector3 PosAmp;

	// Token: 0x04004F66 RID: 20326
	[SerializeField]
	private Vector3 PosFreq;

	// Token: 0x04004F67 RID: 20327
	[SerializeField]
	private Vector3 RotAmp;

	// Token: 0x04004F68 RID: 20328
	[SerializeField]
	private Vector3 RotFreq;

	// Token: 0x04004F69 RID: 20329
	private Vector3 rootPos;

	// Token: 0x04004F6A RID: 20330
	private Vector3 rootRot;

	// Token: 0x04004F6B RID: 20331
	private Vector3 offsPos = Vector3.zero;

	// Token: 0x04004F6C RID: 20332
	private Vector3 offsRot = Vector3.zero;

	// Token: 0x04004F6D RID: 20333
	private DateTime dt;

	// Token: 0x04004F6E RID: 20334
	[SerializeField]
	private bool useServerTime;
}
