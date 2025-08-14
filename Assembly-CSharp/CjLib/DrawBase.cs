using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FA3 RID: 4003
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x060063F5 RID: 25589 RVA: 0x001F6F28 File Offset: 0x001F5128
		private void Update()
		{
			if (this.Style != DebugUtil.Style.Wireframe)
			{
				this.Draw(this.ShadededColor, this.Style, this.DepthTest);
			}
			if (this.Style == DebugUtil.Style.Wireframe || this.Wireframe)
			{
				this.Draw(this.WireframeColor, DebugUtil.Style.Wireframe, this.DepthTest);
			}
		}

		// Token: 0x060063F6 RID: 25590
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04006ECF RID: 28367
		public Color WireframeColor = Color.white;

		// Token: 0x04006ED0 RID: 28368
		public Color ShadededColor = Color.gray;

		// Token: 0x04006ED1 RID: 28369
		public bool Wireframe;

		// Token: 0x04006ED2 RID: 28370
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04006ED3 RID: 28371
		public bool DepthTest = true;
	}
}
