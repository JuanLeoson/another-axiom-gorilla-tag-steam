using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x0200028D RID: 653
public class LckCameraEvents : MonoBehaviour
{
	// Token: 0x06000EFA RID: 3834 RVA: 0x00059C41 File Offset: 0x00057E41
	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering += this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x00059C65 File Offset: 0x00057E65
	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering -= this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x00059C89 File Offset: 0x00057E89
	private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPreRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x00059CAA File Offset: 0x00057EAA
	private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPostRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x040017D5 RID: 6101
	[SerializeField]
	private Camera _camera;

	// Token: 0x040017D6 RID: 6102
	public UnityEvent onPreRender;

	// Token: 0x040017D7 RID: 6103
	public UnityEvent onPostRender;
}
