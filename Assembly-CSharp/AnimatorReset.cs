using System;
using UnityEngine;

// Token: 0x020004EB RID: 1259
public class AnimatorReset : MonoBehaviour
{
	// Token: 0x06001E95 RID: 7829 RVA: 0x000A1C30 File Offset: 0x0009FE30
	public void Reset()
	{
		if (!this.target)
		{
			return;
		}
		this.target.Rebind();
		this.target.Update(0f);
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000A1C5B File Offset: 0x0009FE5B
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Reset();
		}
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000A1C6B File Offset: 0x0009FE6B
	private void OnDisable()
	{
		if (this.onDisable)
		{
			this.Reset();
		}
	}

	// Token: 0x04002743 RID: 10051
	public Animator target;

	// Token: 0x04002744 RID: 10052
	public bool onEnable;

	// Token: 0x04002745 RID: 10053
	public bool onDisable = true;
}
