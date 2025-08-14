using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA7 RID: 4007
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x06006401 RID: 25601 RVA: 0x001F7147 File Offset: 0x001F5347
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06006402 RID: 25602 RVA: 0x001F7174 File Offset: 0x001F5374
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04006EDB RID: 28379
		public float Radius = 1f;

		// Token: 0x04006EDC RID: 28380
		public int NumSegments = 64;

		// Token: 0x04006EDD RID: 28381
		public float StartAngle;

		// Token: 0x04006EDE RID: 28382
		public float ArcAngle = 60f;
	}
}
