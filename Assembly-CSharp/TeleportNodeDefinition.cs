using System;
using UnityEngine;

// Token: 0x02000B20 RID: 2848
[CreateAssetMenu(fileName = "New TeleportNode Definition", menuName = "Teleportation/TeleportNode Definition", order = 1)]
public class TeleportNodeDefinition : ScriptableObject
{
	// Token: 0x1700067B RID: 1659
	// (get) Token: 0x06004494 RID: 17556 RVA: 0x00156A41 File Offset: 0x00154C41
	public TeleportNode Forward
	{
		get
		{
			return this.forward;
		}
	}

	// Token: 0x1700067C RID: 1660
	// (get) Token: 0x06004495 RID: 17557 RVA: 0x00156A49 File Offset: 0x00154C49
	public TeleportNode Backward
	{
		get
		{
			return this.backward;
		}
	}

	// Token: 0x06004496 RID: 17558 RVA: 0x00156A51 File Offset: 0x00154C51
	public void SetForward(TeleportNode node)
	{
		Debug.Log("registered fwd node " + node.name);
		this.forward = node;
	}

	// Token: 0x06004497 RID: 17559 RVA: 0x00156A6F File Offset: 0x00154C6F
	public void SetBackward(TeleportNode node)
	{
		Debug.Log("registered bkwd node " + node.name);
		this.backward = node;
	}

	// Token: 0x04004EBB RID: 20155
	[SerializeField]
	private TeleportNode forward;

	// Token: 0x04004EBC RID: 20156
	[SerializeField]
	private TeleportNode backward;
}
