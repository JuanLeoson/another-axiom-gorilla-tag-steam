using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class ZoneConditionalComponentEnabling : MonoBehaviour
{
	// Token: 0x06000E72 RID: 3698 RVA: 0x00057F43 File Offset: 0x00056143
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x00057F71 File Offset: 0x00056171
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x00057F9C File Offset: 0x0005619C
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		bool enabled = this.invisibleWhileLoaded ? (!flag) : flag;
		if (this.components != null)
		{
			for (int i = 0; i < this.components.Length; i++)
			{
				if (this.components[i] != null)
				{
					this.components[i].enabled = enabled;
				}
			}
		}
		if (this.m_renderers != null)
		{
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null)
				{
					this.m_renderers[j].enabled = enabled;
				}
			}
		}
		if (this.m_colliders != null)
		{
			for (int k = 0; k < this.m_colliders.Length; k++)
			{
				if (this.m_colliders[k] != null)
				{
					this.m_colliders[k].enabled = enabled;
				}
			}
		}
	}

	// Token: 0x0400174F RID: 5967
	[SerializeField]
	private GTZone zone;

	// Token: 0x04001750 RID: 5968
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04001751 RID: 5969
	[SerializeField]
	private Behaviour[] components;

	// Token: 0x04001752 RID: 5970
	[SerializeField]
	private Renderer[] m_renderers;

	// Token: 0x04001753 RID: 5971
	[SerializeField]
	private Collider[] m_colliders;
}
