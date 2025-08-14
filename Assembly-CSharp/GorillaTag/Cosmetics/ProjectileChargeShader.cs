using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F57 RID: 3927
	public class ProjectileChargeShader : MonoBehaviour
	{
		// Token: 0x06006148 RID: 24904 RVA: 0x001EEE0A File Offset: 0x001ED00A
		private void Awake()
		{
			this.renderer = base.GetComponentInChildren<Renderer>();
			this.chargerMPB = new MaterialPropertyBlock();
		}

		// Token: 0x06006149 RID: 24905 RVA: 0x001EEE24 File Offset: 0x001ED024
		public void UpdateChargeProgress(float value)
		{
			if (this.chargerMPB == null)
			{
				this.chargerMPB = new MaterialPropertyBlock();
			}
			if (this.renderer)
			{
				this.renderer.GetPropertyBlock(this.chargerMPB, 1);
				this.chargerMPB.SetVector(ShaderProps._UvShiftOffset, new Vector2(value * (float)this.shaderAnimSteps, 0f));
				this.renderer.SetPropertyBlock(this.chargerMPB, 1);
			}
		}

		// Token: 0x04006D2C RID: 27948
		private Renderer renderer;

		// Token: 0x04006D2D RID: 27949
		private MaterialPropertyBlock chargerMPB;

		// Token: 0x04006D2E RID: 27950
		public int shaderAnimSteps = 4;
	}
}
