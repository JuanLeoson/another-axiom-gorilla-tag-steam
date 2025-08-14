using System;
using UnityEngine;

// Token: 0x02000A8C RID: 2700
public class TrickTreatItem : RandomComponent<MeshRenderer>
{
	// Token: 0x0600418D RID: 16781 RVA: 0x0014AFE0 File Offset: 0x001491E0
	protected override void OnNextItem(MeshRenderer item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			MeshRenderer meshRenderer = this.items[i];
			meshRenderer.enabled = (meshRenderer == item);
		}
	}

	// Token: 0x0600418E RID: 16782 RVA: 0x0014B014 File Offset: 0x00149214
	public void Randomize()
	{
		this.NextItem();
	}
}
