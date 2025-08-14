using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200048E RID: 1166
public class WardrobeInstance : MonoBehaviour
{
	// Token: 0x06001CEE RID: 7406 RVA: 0x0009BD3B File Offset: 0x00099F3B
	public void Start()
	{
		CosmeticsController.instance.AddWardrobeInstance(this);
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x0009BD4A File Offset: 0x00099F4A
	public void OnDestroy()
	{
		CosmeticsController.instance.RemoveWardrobeInstance(this);
	}

	// Token: 0x04002556 RID: 9558
	public WardrobeItemButton[] wardrobeItemButtons;

	// Token: 0x04002557 RID: 9559
	public HeadModel selfDoll;
}
