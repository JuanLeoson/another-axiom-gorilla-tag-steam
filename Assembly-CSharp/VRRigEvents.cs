using System;
using UnityEngine;

// Token: 0x02000414 RID: 1044
[RequireComponent(typeof(RigContainer))]
public class VRRigEvents : MonoBehaviour, IPreDisable
{
	// Token: 0x06001953 RID: 6483 RVA: 0x00088880 File Offset: 0x00086A80
	public void PreDisable()
	{
		Action<RigContainer> action = this.disableEvent;
		if (action == null)
		{
			return;
		}
		action(this.rigRef);
	}

	// Token: 0x040021AA RID: 8618
	[SerializeField]
	private RigContainer rigRef;

	// Token: 0x040021AB RID: 8619
	public Action<RigContainer> disableEvent;
}
