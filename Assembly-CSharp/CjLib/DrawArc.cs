using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA1 RID: 4001
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x060063EF RID: 25583 RVA: 0x001F6D53 File Offset: 0x001F4F53
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x060063F0 RID: 25584 RVA: 0x001F6D8C File Offset: 0x001F4F8C
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04006EC6 RID: 28358
		public float Radius = 1f;

		// Token: 0x04006EC7 RID: 28359
		public int NumSegments = 64;

		// Token: 0x04006EC8 RID: 28360
		public float StartAngle;

		// Token: 0x04006EC9 RID: 28361
		public float ArcAngle = 60f;
	}
}
