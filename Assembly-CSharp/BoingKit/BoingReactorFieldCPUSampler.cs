using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FE6 RID: 4070
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x060065AF RID: 26031 RVA: 0x00204B9A File Offset: 0x00202D9A
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060065B0 RID: 26032 RVA: 0x00204BA2 File Offset: 0x00202DA2
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060065B1 RID: 26033 RVA: 0x00204BAC File Offset: 0x00202DAC
		public void SampleFromField()
		{
			this.m_objPosition = base.transform.position;
			this.m_objRotation = base.transform.rotation;
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				return;
			}
			Vector3 a;
			Vector4 v;
			if (!component.SampleCpuGrid(base.transform.position, out a, out v))
			{
				return;
			}
			base.transform.position = this.m_objPosition + a * this.PositionSampleMultiplier;
			base.transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(v, true), this.RotationSampleMultiplier) * this.m_objRotation;
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x00204C6B File Offset: 0x00202E6B
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x0400709C RID: 28828
		public BoingReactorField ReactorField;

		// Token: 0x0400709D RID: 28829
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x0400709E RID: 28830
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x0400709F RID: 28831
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x040070A0 RID: 28832
		private Vector3 m_objPosition;

		// Token: 0x040070A1 RID: 28833
		private Quaternion m_objRotation;
	}
}
