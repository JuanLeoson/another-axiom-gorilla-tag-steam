using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000B29 RID: 2857
public class ReportTargetHit : MonoBehaviour
{
	// Token: 0x060044C1 RID: 17601 RVA: 0x00157480 File Offset: 0x00155680
	private void Start()
	{
		this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x060044C2 RID: 17602 RVA: 0x0015749E File Offset: 0x0015569E
	private void OnEnable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x060044C3 RID: 17603 RVA: 0x001574C5 File Offset: 0x001556C5
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x060044C4 RID: 17604 RVA: 0x001574EC File Offset: 0x001556EC
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x060044C5 RID: 17605 RVA: 0x001574F4 File Offset: 0x001556F4
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
			this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x060044C6 RID: 17606 RVA: 0x00157560 File Offset: 0x00155760
	private void seek()
	{
		if (this.targets.Length != 0)
		{
			Vector3 direction = this.targets[ReportTargetHit.rand.NextInt(this.targets.Length)].position - base.transform.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, direction, out raycastHit) && this.colliderFound != null)
			{
				this.colliderFound.Invoke(base.transform.position, raycastHit.point);
			}
		}
	}

	// Token: 0x04004EE7 RID: 20199
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04004EE8 RID: 20200
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04004EE9 RID: 20201
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04004EEA RID: 20202
	[SerializeField]
	private Transform[] targets;

	// Token: 0x04004EEB RID: 20203
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04004EEC RID: 20204
	private float timeSinceSeek;

	// Token: 0x04004EED RID: 20205
	private float seekFreq;

	// Token: 0x04004EEE RID: 20206
	[SerializeField]
	private RandomDispatcher nsRand;
}
