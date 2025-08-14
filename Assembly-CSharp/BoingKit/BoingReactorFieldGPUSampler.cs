using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FE7 RID: 4071
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x060065B4 RID: 26036 RVA: 0x00204CB4 File Offset: 0x00202EB4
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060065B5 RID: 26037 RVA: 0x00204CBC File Offset: 0x00202EBC
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x00204CC4 File Offset: 0x00202EC4
		public void Update()
		{
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			if (this.m_fieldResourceSetId != component.GpuResourceSetId)
			{
				if (this.m_matProps == null)
				{
					this.m_matProps = new MaterialPropertyBlock();
				}
				if (component.UpdateShaderConstants(this.m_matProps, this.PositionSampleMultiplier, this.RotationSampleMultiplier))
				{
					this.m_fieldResourceSetId = component.GpuResourceSetId;
					foreach (Renderer renderer in new Renderer[]
					{
						base.GetComponent<MeshRenderer>(),
						base.GetComponent<SkinnedMeshRenderer>()
					})
					{
						if (!(renderer == null))
						{
							renderer.SetPropertyBlock(this.m_matProps);
						}
					}
				}
			}
		}

		// Token: 0x040070A2 RID: 28834
		public BoingReactorField ReactorField;

		// Token: 0x040070A3 RID: 28835
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x040070A4 RID: 28836
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x040070A5 RID: 28837
		private MaterialPropertyBlock m_matProps;

		// Token: 0x040070A6 RID: 28838
		private int m_fieldResourceSetId = -1;
	}
}
