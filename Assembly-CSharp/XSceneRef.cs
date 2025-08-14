using System;
using UnityEngine;

// Token: 0x0200026E RID: 622
[Serializable]
public struct XSceneRef
{
	// Token: 0x06000E5E RID: 3678 RVA: 0x00057B34 File Offset: 0x00055D34
	public bool TryResolve(out XSceneRefTarget result)
	{
		if (this.TargetID == 0)
		{
			result = null;
			return true;
		}
		if (this.didCache && this.cached != null)
		{
			result = this.cached;
			return true;
		}
		XSceneRefTarget xsceneRefTarget;
		if (!XSceneRefGlobalHub.TryResolve(this.TargetScene, this.TargetID, out xsceneRefTarget))
		{
			result = null;
			return false;
		}
		this.cached = xsceneRefTarget;
		this.didCache = true;
		result = xsceneRefTarget;
		return true;
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00057B9C File Offset: 0x00055D9C
	public bool TryResolve(out GameObject result)
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? null : xsceneRefTarget.gameObject);
			return true;
		}
		result = null;
		return false;
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00057BD0 File Offset: 0x00055DD0
	public bool TryResolve<T>(out T result) where T : Component
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? default(T) : xsceneRefTarget.GetComponent<T>());
			return true;
		}
		result = default(T);
		return false;
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x00057C11 File Offset: 0x00055E11
	public void AddCallbackOnLoad(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneLoad(callback);
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00057C1F File Offset: 0x00055E1F
	public void RemoveCallbackOnLoad(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneLoad(callback);
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00057C2D File Offset: 0x00055E2D
	public void AddCallbackOnUnload(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneUnload(callback);
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00057C3B File Offset: 0x00055E3B
	public void RemoveCallbackOnUnload(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneUnload(callback);
	}

	// Token: 0x04001746 RID: 5958
	public SceneIndex TargetScene;

	// Token: 0x04001747 RID: 5959
	public int TargetID;

	// Token: 0x04001748 RID: 5960
	private XSceneRefTarget cached;

	// Token: 0x04001749 RID: 5961
	private bool didCache;
}
