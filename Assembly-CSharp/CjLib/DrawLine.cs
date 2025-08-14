using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA6 RID: 4006
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x060063FE RID: 25598 RVA: 0x001F70EF File Offset: 0x001F52EF
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x060063FF RID: 25599 RVA: 0x001F70FF File Offset: 0x001F52FF
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x04006EDA RID: 28378
		public Vector3 LocalEndVector = Vector3.right;
	}
}
