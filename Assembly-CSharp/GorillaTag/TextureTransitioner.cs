using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E6A RID: 3690
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x06005C58 RID: 23640 RVA: 0x001D0EFB File Offset: 0x001CF0FB
		protected void Awake()
		{
			if (Application.isPlaying || this.editorPreview)
			{
				TextureTransitionerManager.EnsureInstanceIsAvailable();
			}
			this.RefreshShaderParams();
			this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
			this.ResetToDefaultState();
		}

		// Token: 0x06005C59 RID: 23641 RVA: 0x001D0F30 File Offset: 0x001CF130
		protected void OnEnable()
		{
			TextureTransitionerManager.Register(this);
			if (Application.isPlaying && !this.remapInfo.IsValid())
			{
				Debug.LogError("Bad min/max values for remapRanges: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.textures.Length == 0)
			{
				Debug.LogError("Textures array is empty: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.iDynamicFloat == null)
			{
				if (this.dynamicFloatComponent == null)
				{
					Debug.LogError("dynamicFloatComponent cannot be null: " + this.GetComponentPath(int.MaxValue), this);
				}
				this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
				if (this.iDynamicFloat == null)
				{
					Debug.LogError("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath(int.MaxValue), this);
					base.enabled = false;
				}
			}
		}

		// Token: 0x06005C5A RID: 23642 RVA: 0x001D101E File Offset: 0x001CF21E
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x06005C5B RID: 23643 RVA: 0x001D1026 File Offset: 0x001CF226
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x06005C5C RID: 23644 RVA: 0x001D105B File Offset: 0x001CF25B
		public void ResetToDefaultState()
		{
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x040065FD RID: 26109
		public bool editorPreview;

		// Token: 0x040065FE RID: 26110
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x040065FF RID: 26111
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x04006600 RID: 26112
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x04006601 RID: 26113
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x04006602 RID: 26114
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x04006603 RID: 26115
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x04006604 RID: 26116
		public Texture[] textures;

		// Token: 0x04006605 RID: 26117
		public Renderer[] renderers;

		// Token: 0x04006606 RID: 26118
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x04006607 RID: 26119
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x04006608 RID: 26120
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x04006609 RID: 26121
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x0400660A RID: 26122
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x0400660B RID: 26123
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x0400660C RID: 26124
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x0400660D RID: 26125
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02000E6B RID: 3691
		public enum DirectionRetentionMode
		{
			// Token: 0x0400660F RID: 26127
			None,
			// Token: 0x04006610 RID: 26128
			IncreaseOnly,
			// Token: 0x04006611 RID: 26129
			DecreaseOnly
		}
	}
}
