using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EFA RID: 3834
	public class ZoneLiquidEffectableManager : MonoBehaviour
	{
		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06005F08 RID: 24328 RVA: 0x001DF20D File Offset: 0x001DD40D
		// (set) Token: 0x06005F09 RID: 24329 RVA: 0x001DF214 File Offset: 0x001DD414
		public static ZoneLiquidEffectableManager instance { get; private set; }

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06005F0A RID: 24330 RVA: 0x001DF21C File Offset: 0x001DD41C
		// (set) Token: 0x06005F0B RID: 24331 RVA: 0x001DF223 File Offset: 0x001DD423
		public static bool hasInstance { get; private set; }

		// Token: 0x06005F0C RID: 24332 RVA: 0x001DF22B File Offset: 0x001DD42B
		protected void Awake()
		{
			if (ZoneLiquidEffectableManager.hasInstance && ZoneLiquidEffectableManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			ZoneLiquidEffectableManager.SetInstance(this);
		}

		// Token: 0x06005F0D RID: 24333 RVA: 0x001DF253 File Offset: 0x001DD453
		protected void OnDestroy()
		{
			if (ZoneLiquidEffectableManager.instance == this)
			{
				ZoneLiquidEffectableManager.hasInstance = false;
				ZoneLiquidEffectableManager.instance = null;
			}
		}

		// Token: 0x06005F0E RID: 24334 RVA: 0x001DF270 File Offset: 0x001DD470
		protected void LateUpdate()
		{
			int layerMask = UnityLayer.Water.ToLayerMask();
			foreach (ZoneLiquidEffectable zoneLiquidEffectable in this.zoneLiquidEffectables)
			{
				Transform transform = zoneLiquidEffectable.transform;
				zoneLiquidEffectable.inLiquidVolume = Physics.CheckSphere(transform.position, zoneLiquidEffectable.radius * transform.lossyScale.x, layerMask);
				if (zoneLiquidEffectable.inLiquidVolume != zoneLiquidEffectable.wasInLiquidVolume)
				{
					for (int i = 0; i < zoneLiquidEffectable.childRenderers.Length; i++)
					{
						if (zoneLiquidEffectable.inLiquidVolume)
						{
							zoneLiquidEffectable.childRenderers[i].material.EnableKeyword("_WATER_EFFECT");
							zoneLiquidEffectable.childRenderers[i].material.EnableKeyword("_HEIGHT_BASED_WATER_EFFECT");
						}
						else
						{
							zoneLiquidEffectable.childRenderers[i].material.DisableKeyword("_WATER_EFFECT");
							zoneLiquidEffectable.childRenderers[i].material.DisableKeyword("_HEIGHT_BASED_WATER_EFFECT");
						}
					}
				}
				zoneLiquidEffectable.wasInLiquidVolume = zoneLiquidEffectable.inLiquidVolume;
			}
		}

		// Token: 0x06005F0F RID: 24335 RVA: 0x001DF394 File Offset: 0x001DD594
		private static void CreateManager()
		{
			ZoneLiquidEffectableManager.SetInstance(new GameObject("ZoneLiquidEffectableManager").AddComponent<ZoneLiquidEffectableManager>());
		}

		// Token: 0x06005F10 RID: 24336 RVA: 0x001DF3AA File Offset: 0x001DD5AA
		private static void SetInstance(ZoneLiquidEffectableManager manager)
		{
			ZoneLiquidEffectableManager.instance = manager;
			ZoneLiquidEffectableManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06005F11 RID: 24337 RVA: 0x001DF3C8 File Offset: 0x001DD5C8
		public static void Register(ZoneLiquidEffectable effect)
		{
			if (!ZoneLiquidEffectableManager.hasInstance)
			{
				ZoneLiquidEffectableManager.CreateManager();
			}
			if (effect == null)
			{
				return;
			}
			if (ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Contains(effect))
			{
				return;
			}
			ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Add(effect);
			effect.inLiquidVolume = false;
			for (int i = 0; i < effect.childRenderers.Length; i++)
			{
				if (!(effect.childRenderers[i] == null))
				{
					Material sharedMaterial = effect.childRenderers[i].sharedMaterial;
					if (!(sharedMaterial == null) || sharedMaterial.shader.keywordSpace.FindKeyword("_WATER_EFFECT").isValid)
					{
						effect.inLiquidVolume = (sharedMaterial.IsKeywordEnabled("_WATER_EFFECT") && sharedMaterial.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT"));
						return;
					}
				}
			}
		}

		// Token: 0x06005F12 RID: 24338 RVA: 0x001DF493 File Offset: 0x001DD693
		public static void Unregister(ZoneLiquidEffectable effect)
		{
			ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Remove(effect);
		}

		// Token: 0x0400696F RID: 26991
		private readonly List<ZoneLiquidEffectable> zoneLiquidEffectables = new List<ZoneLiquidEffectable>(32);
	}
}
