using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C40 RID: 3136
	public class LayerChanger : MonoBehaviour
	{
		// Token: 0x06004D82 RID: 19842 RVA: 0x00181051 File Offset: 0x0017F251
		public void InitializeLayers(Transform parent)
		{
			if (!this.layersStored)
			{
				this.StoreOriginalLayers(parent);
				this.layersStored = true;
			}
		}

		// Token: 0x06004D83 RID: 19843 RVA: 0x0018106C File Offset: 0x0017F26C
		private void StoreOriginalLayers(Transform parent)
		{
			if (!this.includeChildren)
			{
				this.StoreOriginalLayers(parent);
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				this.originalLayers[transform] = transform.gameObject.layer;
				this.StoreOriginalLayers(transform);
			}
		}

		// Token: 0x06004D84 RID: 19844 RVA: 0x001810E8 File Offset: 0x0017F2E8
		public void ChangeLayer(Transform parent, string newLayer)
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			this.ChangeLayers(parent, LayerMask.NameToLayer(newLayer));
		}

		// Token: 0x06004D85 RID: 19845 RVA: 0x0018110C File Offset: 0x0017F30C
		private void ChangeLayers(Transform parent, int newLayer)
		{
			if (!this.includeChildren)
			{
				if (!this.restrictedLayers.Contains(parent.gameObject.layer))
				{
					parent.gameObject.layer = newLayer;
				}
				return;
			}
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (!this.restrictedLayers.Contains(transform.gameObject.layer))
				{
					transform.gameObject.layer = newLayer;
					this.ChangeLayers(transform, newLayer);
				}
			}
		}

		// Token: 0x06004D86 RID: 19846 RVA: 0x001811B4 File Offset: 0x0017F3B4
		public void RestoreOriginalLayers()
		{
			if (!this.layersStored)
			{
				Debug.LogWarning("Layers have not been initialized. Call InitializeLayers first.");
				return;
			}
			foreach (KeyValuePair<Transform, int> keyValuePair in this.originalLayers)
			{
				keyValuePair.Key.gameObject.layer = keyValuePair.Value;
			}
		}

		// Token: 0x0400566F RID: 22127
		public LayerMask restrictedLayers;

		// Token: 0x04005670 RID: 22128
		public bool includeChildren = true;

		// Token: 0x04005671 RID: 22129
		private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();

		// Token: 0x04005672 RID: 22130
		private bool layersStored;
	}
}
