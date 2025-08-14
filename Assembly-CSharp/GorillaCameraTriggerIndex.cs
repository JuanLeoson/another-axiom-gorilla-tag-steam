using System;
using UnityEngine;

// Token: 0x020004D3 RID: 1235
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x06001E4F RID: 7759 RVA: 0x000A1232 File Offset: 0x0009F432
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x000A1240 File Offset: 0x0009F440
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x000A126C File Offset: 0x0009F46C
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x040026D6 RID: 9942
	public int sceneTriggerIndex;

	// Token: 0x040026D7 RID: 9943
	public GorillaCameraSceneTrigger parentTrigger;
}
