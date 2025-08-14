using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000B28 RID: 2856
public class ReportForwardHit : MonoBehaviour
{
	// Token: 0x060044B9 RID: 17593 RVA: 0x001572B9 File Offset: 0x001554B9
	private void Start()
	{
		this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x060044BA RID: 17594 RVA: 0x001572D7 File Offset: 0x001554D7
	private void OnEnable()
	{
		if (this.seekOnEnable)
		{
			this.seek();
		}
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x060044BB RID: 17595 RVA: 0x0015730C File Offset: 0x0015550C
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x060044BC RID: 17596 RVA: 0x00157333 File Offset: 0x00155533
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x060044BD RID: 17597 RVA: 0x0015733C File Offset: 0x0015553C
	private void Update()
	{
		if (this.nsRand != null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x060044BE RID: 17598 RVA: 0x001573A8 File Offset: 0x001555A8
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.maxRadias * num) && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, raycastHit.point);
		}
	}

	// Token: 0x04004EDE RID: 20190
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04004EDF RID: 20191
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04004EE0 RID: 20192
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04004EE1 RID: 20193
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04004EE2 RID: 20194
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04004EE3 RID: 20195
	[SerializeField]
	private RandomDispatcher nsRand;

	// Token: 0x04004EE4 RID: 20196
	private float timeSinceSeek;

	// Token: 0x04004EE5 RID: 20197
	private float seekFreq;

	// Token: 0x04004EE6 RID: 20198
	[SerializeField]
	private bool seekOnEnable;
}
