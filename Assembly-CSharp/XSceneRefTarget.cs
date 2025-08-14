using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class XSceneRefTarget : MonoBehaviour
{
	// Token: 0x06000E69 RID: 3689 RVA: 0x00057E2D File Offset: 0x0005602D
	private void Awake()
	{
		this.Register(false);
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x00057E36 File Offset: 0x00056036
	private void Reset()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00057E43 File Offset: 0x00056043
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Register(false);
		}
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00057E54 File Offset: 0x00056054
	public void Register(bool force = false)
	{
		if (this.UniqueID == this.lastRegisteredID && !force)
		{
			return;
		}
		if (this.lastRegisteredID != -1)
		{
			XSceneRefGlobalHub.Unregister(this.lastRegisteredID, this);
		}
		XSceneRefGlobalHub.Register(this.UniqueID, this);
		this.lastRegisteredID = this.UniqueID;
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00057EA0 File Offset: 0x000560A0
	private void OnDestroy()
	{
		XSceneRefGlobalHub.Unregister(this.UniqueID, this);
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x00057EAE File Offset: 0x000560AE
	private void AssignNewID()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
		this.Register(false);
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00057EC4 File Offset: 0x000560C4
	public static int CreateNewID()
	{
		int num = (int)((DateTime.Now - XSceneRefTarget.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
		if (num <= XSceneRefTarget.lastAssignedID)
		{
			XSceneRefTarget.lastAssignedID++;
			return XSceneRefTarget.lastAssignedID;
		}
		XSceneRefTarget.lastAssignedID = num;
		return num;
	}

	// Token: 0x0400174B RID: 5963
	public int UniqueID;

	// Token: 0x0400174C RID: 5964
	[NonSerialized]
	private int lastRegisteredID = -1;

	// Token: 0x0400174D RID: 5965
	private static DateTime epoch = new DateTime(2024, 1, 1);

	// Token: 0x0400174E RID: 5966
	private static int lastAssignedID;
}
