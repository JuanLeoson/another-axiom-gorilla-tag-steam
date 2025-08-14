using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200008A RID: 138
public class FixedScrollbarSize : MonoBehaviour
{
	// Token: 0x0600038F RID: 911 RVA: 0x00016350 File Offset: 0x00014550
	private void OnEnable()
	{
		this.EnforceScrollbarSize();
		CanvasUpdateRegistry.instance.Equals(null);
		Canvas.willRenderCanvases += this.EnforceScrollbarSize;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00016375 File Offset: 0x00014575
	private void OnDisable()
	{
		Canvas.willRenderCanvases -= this.EnforceScrollbarSize;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00016388 File Offset: 0x00014588
	private void EnforceScrollbarSize()
	{
		if (this.ScrollRect.horizontalScrollbar && this.ScrollRect.horizontalScrollbar.size != this.HorizontalBarSize)
		{
			this.ScrollRect.horizontalScrollbar.size = this.HorizontalBarSize;
		}
		if (this.ScrollRect.verticalScrollbar && this.ScrollRect.verticalScrollbar.size != this.VerticalBarSize)
		{
			this.ScrollRect.verticalScrollbar.size = this.VerticalBarSize;
		}
	}

	// Token: 0x0400040E RID: 1038
	public ScrollRect ScrollRect;

	// Token: 0x0400040F RID: 1039
	public float HorizontalBarSize = 0.2f;

	// Token: 0x04000410 RID: 1040
	public float VerticalBarSize = 0.2f;
}
