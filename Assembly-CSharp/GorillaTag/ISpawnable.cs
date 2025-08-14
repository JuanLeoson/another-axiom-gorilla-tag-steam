using System;
using GorillaTag.CosmeticSystem;

namespace GorillaTag
{
	// Token: 0x02000E52 RID: 3666
	public interface ISpawnable
	{
		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06005C04 RID: 23556
		// (set) Token: 0x06005C05 RID: 23557
		bool IsSpawned { get; set; }

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06005C06 RID: 23558
		// (set) Token: 0x06005C07 RID: 23559
		ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06005C08 RID: 23560
		void OnSpawn(VRRig rig);

		// Token: 0x06005C09 RID: 23561
		void OnDespawn();
	}
}
