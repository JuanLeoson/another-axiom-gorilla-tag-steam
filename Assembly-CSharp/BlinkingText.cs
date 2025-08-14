using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CD RID: 205
public class BlinkingText : MonoBehaviour
{
	// Token: 0x06000508 RID: 1288 RVA: 0x0001D3A8 File Offset: 0x0001B5A8
	private void Awake()
	{
		this.textComponent = base.GetComponent<Text>();
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001D3B8 File Offset: 0x0001B5B8
	private void Update()
	{
		if (this.isOn && Time.time > this.lastTime + this.cycleTime * this.dutyCycle)
		{
			this.isOn = false;
			this.textComponent.enabled = false;
			return;
		}
		if (!this.isOn && Time.time > this.lastTime + this.cycleTime)
		{
			this.lastTime = Time.time;
			this.isOn = true;
			this.textComponent.enabled = true;
		}
	}

	// Token: 0x04000607 RID: 1543
	public float cycleTime;

	// Token: 0x04000608 RID: 1544
	public float dutyCycle;

	// Token: 0x04000609 RID: 1545
	private bool isOn;

	// Token: 0x0400060A RID: 1546
	private float lastTime;

	// Token: 0x0400060B RID: 1547
	private Text textComponent;
}
