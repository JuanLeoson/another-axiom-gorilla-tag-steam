using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA4 RID: 4004
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x060063F8 RID: 25592 RVA: 0x001F6FA4 File Offset: 0x001F51A4
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x060063F9 RID: 25593 RVA: 0x001F6FD0 File Offset: 0x001F51D0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04006ED4 RID: 28372
		public float Radius = 1f;

		// Token: 0x04006ED5 RID: 28373
		public int NumSegments = 64;

		// Token: 0x04006ED6 RID: 28374
		public float StartAngle;

		// Token: 0x04006ED7 RID: 28375
		public float ArcAngle = 60f;
	}
}
