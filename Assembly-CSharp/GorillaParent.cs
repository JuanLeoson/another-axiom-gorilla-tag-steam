using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
public class GorillaParent : MonoBehaviour
{
	// Token: 0x06002C7C RID: 11388 RVA: 0x000EB600 File Offset: 0x000E9800
	public void Awake()
	{
		if (GorillaParent.instance == null)
		{
			GorillaParent.instance = this;
			GorillaParent.hasInstance = true;
			return;
		}
		if (GorillaParent.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x000EB63B File Offset: 0x000E983B
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x000EB65A File Offset: 0x000E985A
	public static void ReplicatedClientReady()
	{
		GorillaParent.replicatedClientReady = true;
		Action action = GorillaParent.onReplicatedClientReady;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x000EB671 File Offset: 0x000E9871
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x040037E1 RID: 14305
	public GameObject tagUI;

	// Token: 0x040037E2 RID: 14306
	public GameObject playerParent;

	// Token: 0x040037E3 RID: 14307
	public GameObject vrrigParent;

	// Token: 0x040037E4 RID: 14308
	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	// Token: 0x040037E5 RID: 14309
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040037E6 RID: 14310
	public List<VRRig> vrrigs;

	// Token: 0x040037E7 RID: 14311
	public Dictionary<NetPlayer, VRRig> vrrigDict = new Dictionary<NetPlayer, VRRig>();

	// Token: 0x040037E8 RID: 14312
	private int i;

	// Token: 0x040037E9 RID: 14313
	private static bool replicatedClientReady;

	// Token: 0x040037EA RID: 14314
	private static Action onReplicatedClientReady;
}
