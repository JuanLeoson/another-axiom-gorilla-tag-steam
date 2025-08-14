using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E6C RID: 3692
	[ExecuteAlways]
	public class TextureTransitionerManager : MonoBehaviour
	{
		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06005C5E RID: 23646 RVA: 0x001D10A6 File Offset: 0x001CF2A6
		// (set) Token: 0x06005C5F RID: 23647 RVA: 0x001D10AD File Offset: 0x001CF2AD
		public static TextureTransitionerManager instance { get; private set; }

		// Token: 0x06005C60 RID: 23648 RVA: 0x001D10B5 File Offset: 0x001CF2B5
		protected void Awake()
		{
			if (TextureTransitionerManager.instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TextureTransitionerManager.instance = this;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			this.matPropBlock = new MaterialPropertyBlock();
		}

		// Token: 0x06005C61 RID: 23649 RVA: 0x001D10F4 File Offset: 0x001CF2F4
		protected void LateUpdate()
		{
			foreach (TextureTransitioner textureTransitioner in TextureTransitionerManager.components)
			{
				int num = textureTransitioner.textures.Length;
				float num2 = Mathf.Clamp01(textureTransitioner.remapInfo.Remap(textureTransitioner.iDynamicFloat.floatValue));
				TextureTransitioner.DirectionRetentionMode directionRetentionMode = textureTransitioner.directionRetentionMode;
				if (directionRetentionMode != TextureTransitioner.DirectionRetentionMode.IncreaseOnly)
				{
					if (directionRetentionMode == TextureTransitioner.DirectionRetentionMode.DecreaseOnly)
					{
						num2 = Mathf.Min(num2, textureTransitioner.normalizedValue);
					}
				}
				else
				{
					num2 = Mathf.Max(num2, textureTransitioner.normalizedValue);
				}
				float num3 = num2 * (float)(num - 1);
				float num4 = num3 % 1f;
				int num5 = (int)(num4 * 1000f);
				int num6 = (int)num3;
				int num7 = Mathf.Min(num - 1, num6 + 1);
				if (num5 != textureTransitioner.transitionPercent || num6 != textureTransitioner.tex1Index || num7 != textureTransitioner.tex2Index)
				{
					this.matPropBlock.SetFloat(textureTransitioner.texTransitionShaderParam, num4);
					this.matPropBlock.SetTexture(textureTransitioner.tex1ShaderParam, textureTransitioner.textures[num6]);
					this.matPropBlock.SetTexture(textureTransitioner.tex2ShaderParam, textureTransitioner.textures[num7]);
					Renderer[] renderers = textureTransitioner.renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].SetPropertyBlock(this.matPropBlock);
					}
					textureTransitioner.normalizedValue = num2;
					textureTransitioner.transitionPercent = num5;
					textureTransitioner.tex1Index = num6;
					textureTransitioner.tex2Index = num7;
				}
			}
		}

		// Token: 0x06005C62 RID: 23650 RVA: 0x001D1284 File Offset: 0x001CF484
		public static void EnsureInstanceIsAvailable()
		{
			if (TextureTransitionerManager.instance != null)
			{
				return;
			}
			GameObject gameObject = new GameObject();
			TextureTransitionerManager.instance = gameObject.AddComponent<TextureTransitionerManager>();
			gameObject.name = "TextureTransitionerManager (Singleton)";
		}

		// Token: 0x06005C63 RID: 23651 RVA: 0x001D12AE File Offset: 0x001CF4AE
		public static void Register(TextureTransitioner component)
		{
			TextureTransitionerManager.components.Add(component);
		}

		// Token: 0x06005C64 RID: 23652 RVA: 0x001D12BB File Offset: 0x001CF4BB
		public static void Unregister(TextureTransitioner component)
		{
			TextureTransitionerManager.components.Remove(component);
		}

		// Token: 0x04006613 RID: 26131
		public static readonly List<TextureTransitioner> components = new List<TextureTransitioner>(256);

		// Token: 0x04006614 RID: 26132
		private MaterialPropertyBlock matPropBlock;
	}
}
