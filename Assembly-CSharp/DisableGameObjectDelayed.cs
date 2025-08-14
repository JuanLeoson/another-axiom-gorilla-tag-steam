using System;
using UnityEngine;

// Token: 0x02000AB3 RID: 2739
public class DisableGameObjectDelayed : MonoBehaviour
{
	// Token: 0x0600423B RID: 16955 RVA: 0x0014D96E File Offset: 0x0014BB6E
	private void OnEnable()
	{
		this.enabledTime = Time.time;
	}

	// Token: 0x0600423C RID: 16956 RVA: 0x0014D97B File Offset: 0x0014BB7B
	private void Update()
	{
		if (Time.time > this.enabledTime + this.delayTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04004D85 RID: 19845
	public float delayTime = 1f;

	// Token: 0x04004D86 RID: 19846
	public float enabledTime;
}
