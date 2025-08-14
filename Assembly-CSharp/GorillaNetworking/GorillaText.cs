using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x02000D76 RID: 3446
	[Serializable]
	public class GorillaText
	{
		// Token: 0x060055EB RID: 21995 RVA: 0x001AAEFC File Offset: 0x001A90FC
		public void Initialize(Material[] originalMaterials, Material failureMaterial, UnityEvent<string> callback = null, UnityEvent<Material[]> materialCallback = null)
		{
			this.failureMaterial = failureMaterial;
			this.originalMaterials = originalMaterials;
			this.currentMaterials = originalMaterials;
			Debug.Log("Original text = " + this.originalText);
			this.updateTextCallback = callback;
			this.updateMaterialCallback = materialCallback;
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x060055EC RID: 21996 RVA: 0x001AAF37 File Offset: 0x001A9137
		// (set) Token: 0x060055ED RID: 21997 RVA: 0x001AAF3F File Offset: 0x001A913F
		public string Text
		{
			get
			{
				return this.originalText;
			}
			set
			{
				if (this.originalText == value)
				{
					return;
				}
				this.originalText = value;
				if (!this.failedState)
				{
					UnityEvent<string> unityEvent = this.updateTextCallback;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke(value);
				}
			}
		}

		// Token: 0x060055EE RID: 21998 RVA: 0x001AAF70 File Offset: 0x001A9170
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.failureText = failText;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(failText);
			}
			this.currentMaterials = (Material[])this.originalMaterials.Clone();
			this.currentMaterials[0] = this.failureMaterial;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x060055EF RID: 21999 RVA: 0x001AAFD8 File Offset: 0x001A91D8
		public void DisableFailedState()
		{
			this.failedState = true;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.originalText);
			}
			this.failureText = "";
			this.currentMaterials = this.originalMaterials;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x04005F9E RID: 24478
		private string failureText;

		// Token: 0x04005F9F RID: 24479
		private string originalText = string.Empty;

		// Token: 0x04005FA0 RID: 24480
		private bool failedState;

		// Token: 0x04005FA1 RID: 24481
		private Material[] originalMaterials;

		// Token: 0x04005FA2 RID: 24482
		private Material failureMaterial;

		// Token: 0x04005FA3 RID: 24483
		internal Material[] currentMaterials;

		// Token: 0x04005FA4 RID: 24484
		private UnityEvent<string> updateTextCallback;

		// Token: 0x04005FA5 RID: 24485
		private UnityEvent<Material[]> updateMaterialCallback;
	}
}
