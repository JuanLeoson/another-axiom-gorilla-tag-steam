using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020007AA RID: 1962
public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x0600314A RID: 12618 RVA: 0x00100CB5 File Offset: 0x000FEEB5
	// (set) Token: 0x0600314B RID: 12619 RVA: 0x00100CBD File Offset: 0x000FEEBD
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x0600314C RID: 12620 RVA: 0x00100CC8 File Offset: 0x000FEEC8
	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x00100D17 File Offset: 0x000FEF17
	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x00100D37 File Offset: 0x000FEF37
	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x04003CF1 RID: 15601
	public ScienceExperimentElementID elementID;

	// Token: 0x04003CF2 RID: 15602
	private Transform followElement;
}
