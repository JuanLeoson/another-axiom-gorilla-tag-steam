using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA2 RID: 4002
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x060063F2 RID: 25586 RVA: 0x001F6E30 File Offset: 0x001F5030
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x001F6E94 File Offset: 0x001F5094
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x04006ECA RID: 28362
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x04006ECB RID: 28363
		public float ConeRadius = 0.05f;

		// Token: 0x04006ECC RID: 28364
		public float ConeHeight = 0.1f;

		// Token: 0x04006ECD RID: 28365
		public float StemThickness = 0.05f;

		// Token: 0x04006ECE RID: 28366
		public int NumSegments = 8;
	}
}
