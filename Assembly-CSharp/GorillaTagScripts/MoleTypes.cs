using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BED RID: 3053
	public class MoleTypes : MonoBehaviour
	{
		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06004A1B RID: 18971 RVA: 0x0016820A File Offset: 0x0016640A
		// (set) Token: 0x06004A1C RID: 18972 RVA: 0x00168212 File Offset: 0x00166412
		public bool IsLeftSideMoleType { get; set; }

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06004A1D RID: 18973 RVA: 0x0016821B File Offset: 0x0016641B
		// (set) Token: 0x06004A1E RID: 18974 RVA: 0x00168223 File Offset: 0x00166423
		public Mole MoleContainerParent { get; set; }

		// Token: 0x06004A1F RID: 18975 RVA: 0x0016822C File Offset: 0x0016642C
		private void Start()
		{
			this.MoleContainerParent = base.GetComponentInParent<Mole>();
			if (this.MoleContainerParent)
			{
				this.IsLeftSideMoleType = this.MoleContainerParent.IsLeftSideMole;
			}
		}

		// Token: 0x040052EE RID: 21230
		public bool isHazard;

		// Token: 0x040052EF RID: 21231
		public int scorePoint = 1;

		// Token: 0x040052F0 RID: 21232
		public MeshRenderer MeshRenderer;

		// Token: 0x040052F1 RID: 21233
		public Material monkeMoleDefaultMaterial;

		// Token: 0x040052F2 RID: 21234
		public Material monkeMoleHitMaterial;
	}
}
