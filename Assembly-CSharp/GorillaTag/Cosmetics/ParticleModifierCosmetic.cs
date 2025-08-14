using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F54 RID: 3924
	public class ParticleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06006120 RID: 24864 RVA: 0x001EE72B File Offset: 0x001EC92B
		private void Awake()
		{
			this.StoreOriginalValues();
			this.currentIndex = -1;
		}

		// Token: 0x06006121 RID: 24865 RVA: 0x001EE73A File Offset: 0x001EC93A
		private void OnValidate()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x06006122 RID: 24866 RVA: 0x001EE73A File Offset: 0x001EC93A
		private void OnEnable()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x06006123 RID: 24867 RVA: 0x001EE742 File Offset: 0x001EC942
		private void OnDisable()
		{
			this.ResetToOriginal();
		}

		// Token: 0x06006124 RID: 24868 RVA: 0x001EE74C File Offset: 0x001EC94C
		private void StoreOriginalValues()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			this.originalStartSize = main.startSize.constant;
			this.originalStartColor = main.startColor.color;
		}

		// Token: 0x06006125 RID: 24869 RVA: 0x001EE79E File Offset: 0x001EC99E
		public void ApplySetting(ParticleSettingsSO setting)
		{
			this.SetStartSize(setting.startSize);
			this.SetStartColor(setting.startColor);
		}

		// Token: 0x06006126 RID: 24870 RVA: 0x001EE7B8 File Offset: 0x001EC9B8
		public void ApplySettingLerp(ParticleSettingsSO setting)
		{
			this.LerpStartSize(setting.startSize);
			this.LerpStartColor(setting.startColor);
		}

		// Token: 0x06006127 RID: 24871 RVA: 0x001EE7D4 File Offset: 0x001EC9D4
		public void MoveToNextSetting()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[this.currentIndex];
				this.ApplySetting(setting);
			}
		}

		// Token: 0x06006128 RID: 24872 RVA: 0x001EE820 File Offset: 0x001ECA20
		public void MoveToNextSettingLerp()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[this.currentIndex];
				this.ApplySettingLerp(setting);
			}
		}

		// Token: 0x06006129 RID: 24873 RVA: 0x001EE869 File Offset: 0x001ECA69
		public void ResetSettings()
		{
			this.currentIndex = -1;
			this.ResetToOriginal();
		}

		// Token: 0x0600612A RID: 24874 RVA: 0x001EE878 File Offset: 0x001ECA78
		public void MoveToSettingIndex(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[index];
				this.ApplySetting(setting);
			}
		}

		// Token: 0x0600612B RID: 24875 RVA: 0x001EE8A4 File Offset: 0x001ECAA4
		public void MoveToSettingIndexLerp(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[index];
				this.ApplySettingLerp(setting);
			}
		}

		// Token: 0x0600612C RID: 24876 RVA: 0x001EE8D0 File Offset: 0x001ECAD0
		public void SetStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startSize = size;
			this.targetSize = null;
		}

		// Token: 0x0600612D RID: 24877 RVA: 0x001EE914 File Offset: 0x001ECB14
		public void IncreaseStartSize(float delta)
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			float constant = main.startSize.constant;
			main.startSize = constant + delta;
			this.targetSize = null;
		}

		// Token: 0x0600612E RID: 24878 RVA: 0x001EE968 File Offset: 0x001ECB68
		public void LerpStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			if (Mathf.Abs(this.ps.main.startSize.constant - size) < 0.01f)
			{
				return;
			}
			this.targetSize = new float?(size);
		}

		// Token: 0x0600612F RID: 24879 RVA: 0x001EE9BC File Offset: 0x001ECBBC
		public void SetStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startColor = color;
			this.targetColor = null;
		}

		// Token: 0x06006130 RID: 24880 RVA: 0x001EEA00 File Offset: 0x001ECC00
		public void LerpStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			Color color2 = this.ps.main.startColor.color;
			if (this.IsColorApproximatelyEqual(color2, color, 0.0001f))
			{
				return;
			}
			this.targetColor = new Color?(color);
		}

		// Token: 0x06006131 RID: 24881 RVA: 0x001EEA54 File Offset: 0x001ECC54
		public void SetStartValues(float size, Color color)
		{
			this.SetStartSize(size);
			this.SetStartColor(color);
		}

		// Token: 0x06006132 RID: 24882 RVA: 0x001EEA64 File Offset: 0x001ECC64
		public void LerpStartValues(float size, Color color)
		{
			this.LerpStartSize(size);
			this.LerpStartColor(color);
		}

		// Token: 0x06006133 RID: 24883 RVA: 0x001EEA74 File Offset: 0x001ECC74
		private void Update()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			if (this.targetSize != null)
			{
				float num = Mathf.Lerp(main.startSize.constant, this.targetSize.Value, Time.deltaTime * this.transitionSpeed);
				main.startSize = num;
				if (Mathf.Abs(num - this.targetSize.Value) < 0.01f)
				{
					main.startSize = this.targetSize.Value;
					this.targetSize = null;
				}
			}
			if (this.targetColor != null)
			{
				Color color = Color.Lerp(main.startColor.color, this.targetColor.Value, Time.deltaTime * this.transitionSpeed);
				main.startColor = color;
				if (this.IsColorApproximatelyEqual(color, this.targetColor.Value, 0.0001f))
				{
					main.startColor = this.targetColor.Value;
					this.targetColor = null;
				}
			}
		}

		// Token: 0x06006134 RID: 24884 RVA: 0x001EEBA4 File Offset: 0x001ECDA4
		[ContextMenu("Reset To Original")]
		public void ResetToOriginal()
		{
			if (this.ps == null)
			{
				return;
			}
			this.targetSize = null;
			this.targetColor = null;
			ParticleSystem.MainModule main = this.ps.main;
			main.startSize = this.originalStartSize;
			main.startColor = this.originalStartColor;
		}

		// Token: 0x06006135 RID: 24885 RVA: 0x001EEC08 File Offset: 0x001ECE08
		private bool IsColorApproximatelyEqual(Color a, Color b, float threshold = 0.0001f)
		{
			float num = a.r - b.r;
			float num2 = a.g - b.g;
			float num3 = a.b - b.b;
			float num4 = a.a - b.a;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4 < threshold;
		}

		// Token: 0x04006D1C RID: 27932
		[SerializeField]
		private ParticleSystem ps;

		// Token: 0x04006D1D RID: 27933
		[Tooltip("For calling gradual functions only")]
		[SerializeField]
		private float transitionSpeed = 5f;

		// Token: 0x04006D1E RID: 27934
		public ParticleSettingsSO[] particleSettings = new ParticleSettingsSO[0];

		// Token: 0x04006D1F RID: 27935
		private float originalStartSize;

		// Token: 0x04006D20 RID: 27936
		private Color originalStartColor;

		// Token: 0x04006D21 RID: 27937
		private float? targetSize;

		// Token: 0x04006D22 RID: 27938
		private Color? targetColor;

		// Token: 0x04006D23 RID: 27939
		private int currentIndex;
	}
}
