using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000DE4 RID: 3556
	public class CombineInRuntimeDemo : MonoBehaviour
	{
		// Token: 0x06005850 RID: 22608 RVA: 0x001B64C8 File Offset: 0x001B46C8
		private void Update()
		{
			if (!this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(true);
				this.undoButton.SetActive(false);
			}
			if (this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(false);
				this.undoButton.SetActive(true);
			}
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x001B651F File Offset: 0x001B471F
		public void CombineMeshes()
		{
			this.runtimeCombiner.CombineMeshes();
		}

		// Token: 0x06005852 RID: 22610 RVA: 0x001B652D File Offset: 0x001B472D
		public void UndoMerge()
		{
			this.runtimeCombiner.UndoMerge();
		}

		// Token: 0x040061EE RID: 25070
		public GameObject combineButton;

		// Token: 0x040061EF RID: 25071
		public GameObject undoButton;

		// Token: 0x040061F0 RID: 25072
		public RuntimeMeshCombiner runtimeCombiner;
	}
}
