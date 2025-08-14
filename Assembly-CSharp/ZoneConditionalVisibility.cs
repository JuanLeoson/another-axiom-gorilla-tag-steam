using System;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class ZoneConditionalVisibility : MonoBehaviour
{
	// Token: 0x06000E7A RID: 3706 RVA: 0x0005814B File Offset: 0x0005634B
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x00058179 File Offset: 0x00056379
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x000581A1 File Offset: 0x000563A1
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			base.gameObject.SetActive(!ZoneManagement.IsInZone(this.zone));
			return;
		}
		base.gameObject.SetActive(ZoneManagement.IsInZone(this.zone));
	}

	// Token: 0x04001757 RID: 5975
	[SerializeField]
	private GTZone zone;

	// Token: 0x04001758 RID: 5976
	[SerializeField]
	private bool invisibleWhileLoaded;
}
