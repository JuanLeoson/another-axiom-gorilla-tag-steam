using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000C58 RID: 3160
	public class CMSTryOnArea : MonoBehaviour
	{
		// Token: 0x06004E35 RID: 20021 RVA: 0x00184EEE File Offset: 0x001830EE
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene)
		{
			this.originalScene = customMapScene;
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.AddCollider(this.tryOnAreaCollider);
		}

		// Token: 0x06004E36 RID: 20022 RVA: 0x00184F11 File Offset: 0x00183111
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.tryOnAreaCollider);
		}

		// Token: 0x06004E37 RID: 20023 RVA: 0x00184F2D File Offset: 0x0018312D
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x04005723 RID: 22307
		private Scene originalScene;

		// Token: 0x04005724 RID: 22308
		public BoxCollider tryOnAreaCollider;
	}
}
