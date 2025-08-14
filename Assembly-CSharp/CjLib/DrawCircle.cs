using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA5 RID: 4005
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x060063FB RID: 25595 RVA: 0x001F7074 File Offset: 0x001F5274
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x060063FC RID: 25596 RVA: 0x001F709E File Offset: 0x001F529E
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x04006ED8 RID: 28376
		public float Radius = 1f;

		// Token: 0x04006ED9 RID: 28377
		public int NumSegments = 64;
	}
}
