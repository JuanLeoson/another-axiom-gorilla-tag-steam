using System;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
public class ProjectileChargeHelper : MonoBehaviour
{
	// Token: 0x06001D5C RID: 7516 RVA: 0x0009D6BC File Offset: 0x0009B8BC
	public void SetAll(float f)
	{
		for (int i = 0; i < this.properties.Length; i++)
		{
			this.properties[i].Set(f);
		}
	}

	// Token: 0x040025D3 RID: 9683
	[SerializeField]
	private ProjectileChargeHelper.Property[] properties;

	// Token: 0x020004A3 RID: 1187
	[Serializable]
	private class Property
	{
		// Token: 0x06001D5E RID: 7518 RVA: 0x0009D6EA File Offset: 0x0009B8EA
		private bool IsColor()
		{
			return this.type == ProjectileChargeHelper.Property.Type.Color;
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0009D6F5 File Offset: 0x0009B8F5
		private bool IsCurve()
		{
			return this.type == ProjectileChargeHelper.Property.Type.Scale;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x0009D700 File Offset: 0x0009B900
		public void Set(float f)
		{
			ProjectileChargeHelper.Property.Type type = this.type;
			if (type != ProjectileChargeHelper.Property.Type.Color)
			{
				if (type != ProjectileChargeHelper.Property.Type.Scale)
				{
					return;
				}
				Vector3 localScale = this.curve.Evaluate(f) * Vector3.one;
				Transform transform = this.component as Transform;
				if (transform != null)
				{
					transform.localScale = localScale;
				}
			}
			else
			{
				Color color = this.color.Evaluate(f);
				ParticleSystem particleSystem = this.component as ParticleSystem;
				if (particleSystem != null)
				{
					particleSystem.main.startColor = color;
					return;
				}
			}
		}

		// Token: 0x040025D4 RID: 9684
		[SerializeField]
		private ProjectileChargeHelper.Property.Type type;

		// Token: 0x040025D5 RID: 9685
		[SerializeField]
		protected Component component;

		// Token: 0x040025D6 RID: 9686
		[SerializeField]
		private Gradient color;

		// Token: 0x040025D7 RID: 9687
		[SerializeField]
		private AnimationCurve curve;

		// Token: 0x020004A4 RID: 1188
		private enum Type
		{
			// Token: 0x040025D9 RID: 9689
			Color,
			// Token: 0x040025DA RID: 9690
			Scale
		}
	}
}
