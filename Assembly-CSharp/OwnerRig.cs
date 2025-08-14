using System;
using UnityEngine;

// Token: 0x0200079F RID: 1951
public class OwnerRig : MonoBehaviour, IVariable<VRRig>, IVariable, IRigAware
{
	// Token: 0x06003112 RID: 12562 RVA: 0x00100223 File Offset: 0x000FE423
	public void TryFindRig()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (this._rig != null)
		{
			return;
		}
		this._rig = base.GetComponentInChildren<VRRig>();
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x0010024C File Offset: 0x000FE44C
	public VRRig Get()
	{
		return this._rig;
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x00100254 File Offset: 0x000FE454
	public void Set(VRRig value)
	{
		this._rig = value;
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x0010025D File Offset: 0x000FE45D
	public void Set(GameObject obj)
	{
		this._rig = ((obj != null) ? obj.GetComponentInParent<VRRig>() : null);
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x00100254 File Offset: 0x000FE454
	void IRigAware.SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x00100277 File Offset: 0x000FE477
	public static implicit operator bool(OwnerRig or)
	{
		return or != null && !(or == null) && or._rig != null && !(or._rig == null);
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x001002A4 File Offset: 0x000FE4A4
	public static implicit operator VRRig(OwnerRig or)
	{
		if (!or)
		{
			return null;
		}
		return or._rig;
	}

	// Token: 0x04003CBA RID: 15546
	[SerializeField]
	private VRRig _rig;
}
