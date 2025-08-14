using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200023D RID: 573
public class ReparentOnAwakeWithRenderer : MonoBehaviour, IBuildValidation
{
	// Token: 0x06000D4B RID: 3403 RVA: 0x00052645 File Offset: 0x00050845
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() != null && this.myRenderer == null)
		{
			Debug.Log(base.name + " needs a reference to its renderer since it has one - ");
			return false;
		}
		return true;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x0005267C File Offset: 0x0005087C
	private void OnEnable()
	{
		base.transform.SetParent(this.newParent, true);
		if (this.sortLast)
		{
			base.transform.SetAsLastSibling();
		}
		else
		{
			base.transform.SetAsFirstSibling();
		}
		if (this.myRenderer != null)
		{
			this.myRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			this.myRenderer.lightProbeUsage = LightProbeUsage.CustomProvided;
			this.myRenderer.probeAnchor = this.newParent;
		}
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x000526F2 File Offset: 0x000508F2
	[ContextMenu("Set Renderer")]
	public void SetMyRenderer()
	{
		this.myRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x04001555 RID: 5461
	public Transform newParent;

	// Token: 0x04001556 RID: 5462
	public MeshRenderer myRenderer;

	// Token: 0x04001557 RID: 5463
	[Tooltip("We're mostly using this for UI elements like text and images, so this will help you separate these in whatever target parent object.Keep images and texts together, otherwise you'll get extra draw calls. Put images above text or they'll overlap weird tho lol")]
	public bool sortLast;
}
