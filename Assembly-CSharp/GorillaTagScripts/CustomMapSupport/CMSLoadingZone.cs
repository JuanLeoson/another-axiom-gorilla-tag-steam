using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000C51 RID: 3153
	public class CMSLoadingZone : MonoBehaviour
	{
		// Token: 0x06004E00 RID: 19968 RVA: 0x00183960 File Offset: 0x00181B60
		private void Start()
		{
			base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
		}

		// Token: 0x06004E01 RID: 19969 RVA: 0x00183974 File Offset: 0x00181B74
		public void SetupLoadingZone(LoadZoneSettings settings, in string[] assetBundleSceneFilePaths)
		{
			this.scenesToLoad = this.GetSceneIndexes(settings.scenesToLoad, assetBundleSceneFilePaths);
			this.scenesToUnload = this.CleanSceneUnloadArray(settings.scenesToUnload, settings.scenesToLoad, assetBundleSceneFilePaths);
			this.useDynamicLighting = settings.useDynamicLighting;
			this.dynamicLightingAmbientColor = settings.UberShaderAmbientDynamicLight;
			this.triggeredBy = TriggerSource.Body;
			base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x06004E02 RID: 19970 RVA: 0x00183A04 File Offset: 0x00181C04
		private int[] GetSceneIndexes(List<string> sceneNames, in string[] assetBundleSceneFilePaths)
		{
			int[] array = new int[sceneNames.Count];
			for (int i = 0; i < sceneNames.Count; i++)
			{
				for (int j = 0; j < assetBundleSceneFilePaths.Length; j++)
				{
					if (string.Equals(sceneNames[i], this.GetSceneNameFromFilePath(assetBundleSceneFilePaths[j])))
					{
						array[i] = j;
						break;
					}
				}
			}
			return array;
		}

		// Token: 0x06004E03 RID: 19971 RVA: 0x00183A5C File Offset: 0x00181C5C
		private int[] CleanSceneUnloadArray(List<string> unload, List<string> load, in string[] assetBundleSceneFilePaths)
		{
			for (int i = 0; i < load.Count; i++)
			{
				if (unload.Contains(load[i]))
				{
					unload.Remove(load[i]);
				}
			}
			return this.GetSceneIndexes(unload, assetBundleSceneFilePaths);
		}

		// Token: 0x06004E04 RID: 19972 RVA: 0x00183AA0 File Offset: 0x00181CA0
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.bodyCollider)
			{
				if (this.useDynamicLighting)
				{
					GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
					GameLightingManager.instance.SetAmbientLightDynamic(this.dynamicLightingAmbientColor);
				}
				else
				{
					GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
					GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				}
				CustomMapManager.LoadZoneTriggered(this.scenesToLoad, this.scenesToUnload);
			}
		}

		// Token: 0x06004E05 RID: 19973 RVA: 0x00183B17 File Offset: 0x00181D17
		private string GetSceneNameFromFilePath(string filePath)
		{
			string[] array = filePath.Split("/", StringSplitOptions.None);
			return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
		}

		// Token: 0x040056FB RID: 22267
		private int[] scenesToLoad;

		// Token: 0x040056FC RID: 22268
		private int[] scenesToUnload;

		// Token: 0x040056FD RID: 22269
		private bool useDynamicLighting;

		// Token: 0x040056FE RID: 22270
		private Color dynamicLightingAmbientColor;

		// Token: 0x040056FF RID: 22271
		private TriggerSource triggeredBy = TriggerSource.HeadOrBody;
	}
}
