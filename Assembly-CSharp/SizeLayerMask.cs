using System;
using UnityEngine;

// Token: 0x02000778 RID: 1912
[Serializable]
public class SizeLayerMask
{
	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x06002FF0 RID: 12272 RVA: 0x000FC1EC File Offset: 0x000FA3EC
	public int Mask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x04003C01 RID: 15361
	[SerializeField]
	private bool affectLayerA = true;

	// Token: 0x04003C02 RID: 15362
	[SerializeField]
	private bool affectLayerB = true;

	// Token: 0x04003C03 RID: 15363
	[SerializeField]
	private bool affectLayerC = true;

	// Token: 0x04003C04 RID: 15364
	[SerializeField]
	private bool affectLayerD = true;
}
