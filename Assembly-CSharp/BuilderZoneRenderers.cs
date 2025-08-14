using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200051C RID: 1308
public class BuilderZoneRenderers : MonoBehaviour
{
	// Token: 0x06001FC8 RID: 8136 RVA: 0x000A7C38 File Offset: 0x000A5E38
	private void Start()
	{
		this.allRenderers.Clear();
		this.allRenderers.AddRange(this.renderers);
		foreach (GameObject gameObject in this.rootObjects)
		{
			this.allRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));
		}
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000A7CE8 File Offset: 0x000A5EE8
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x000A7D20 File Offset: 0x000A5F20
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer in this.allRenderers)
			{
				renderer.enabled = true;
			}
			using (List<Canvas>.Enumerator enumerator2 = this.canvases.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Canvas canvas = enumerator2.Current;
					canvas.enabled = true;
				}
				return;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer2 in this.allRenderers)
			{
				renderer2.enabled = false;
			}
			foreach (Canvas canvas2 in this.canvases)
			{
				canvas2.enabled = false;
			}
		}
	}

	// Token: 0x0400287B RID: 10363
	public List<Renderer> renderers;

	// Token: 0x0400287C RID: 10364
	public List<Canvas> canvases;

	// Token: 0x0400287D RID: 10365
	public List<GameObject> rootObjects;

	// Token: 0x0400287E RID: 10366
	private bool inBuilderZone;

	// Token: 0x0400287F RID: 10367
	private List<Renderer> allRenderers = new List<Renderer>(200);
}
