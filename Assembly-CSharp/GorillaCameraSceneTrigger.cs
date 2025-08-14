using System;
using UnityEngine;

// Token: 0x020004D2 RID: 1234
public class GorillaCameraSceneTrigger : MonoBehaviour
{
	// Token: 0x06001E4D RID: 7757 RVA: 0x000A11CC File Offset: 0x0009F3CC
	public void ChangeScene(GorillaCameraTriggerIndex triggerLeft)
	{
		if (triggerLeft == this.currentSceneTrigger || this.currentSceneTrigger == null)
		{
			if (this.mostRecentSceneTrigger != this.currentSceneTrigger)
			{
				this.sceneCamera.SetSceneCamera(this.mostRecentSceneTrigger.sceneTriggerIndex);
				this.currentSceneTrigger = this.mostRecentSceneTrigger;
				return;
			}
			this.currentSceneTrigger = null;
		}
	}

	// Token: 0x040026D3 RID: 9939
	public GorillaSceneCamera sceneCamera;

	// Token: 0x040026D4 RID: 9940
	public GorillaCameraTriggerIndex currentSceneTrigger;

	// Token: 0x040026D5 RID: 9941
	public GorillaCameraTriggerIndex mostRecentSceneTrigger;
}
