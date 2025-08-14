using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B27 RID: 2855
public class RandomLocalColliders : MonoBehaviour
{
	// Token: 0x060044B4 RID: 17588 RVA: 0x001570E3 File Offset: 0x001552E3
	private void Start()
	{
		this.colliders = new List<Collider>();
		this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x060044B5 RID: 17589 RVA: 0x0015710C File Offset: 0x0015530C
	private void Update()
	{
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x060044B6 RID: 17590 RVA: 0x00157168 File Offset: 0x00155368
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		this.colliders.Clear();
		this.colliders.AddRange(Physics.OverlapSphere(base.transform.position, this.maxRadias * num));
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.minRadias * num);
		for (int i = 0; i < array.Length; i++)
		{
			this.colliders.Remove(array[i]);
		}
		if (this.colliders.Count > 0 && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, this.colliders[RandomLocalColliders.rand.NextInt(this.colliders.Count)].transform.position);
		}
	}

	// Token: 0x04004ED5 RID: 20181
	private static SRand rand = new SRand("RandomLocalColliders");

	// Token: 0x04004ED6 RID: 20182
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04004ED7 RID: 20183
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04004ED8 RID: 20184
	[SerializeField]
	private float minRadias = 1f;

	// Token: 0x04004ED9 RID: 20185
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04004EDA RID: 20186
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04004EDB RID: 20187
	private List<Collider> colliders;

	// Token: 0x04004EDC RID: 20188
	private float timeSinceSeek;

	// Token: 0x04004EDD RID: 20189
	private float seekFreq;
}
