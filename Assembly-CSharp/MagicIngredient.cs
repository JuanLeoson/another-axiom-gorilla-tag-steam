using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200076A RID: 1898
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x06002F83 RID: 12163 RVA: 0x000FADA0 File Offset: 0x000F8FA0
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x000FADCC File Offset: 0x000F8FCC
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x000FAE02 File Offset: 0x000F9002
	public void Disable()
	{
		this.DropItem();
		base.OnDisable();
		if (this.item)
		{
			this.item.OnDisable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04003B89 RID: 15241
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04003B8A RID: 15242
	public Transform rootParent;

	// Token: 0x04003B8B RID: 15243
	private WorldShareableItem item;

	// Token: 0x04003B8C RID: 15244
	private Transform grabPtInitParent;
}
