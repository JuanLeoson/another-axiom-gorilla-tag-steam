using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Rendering;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x0200080E RID: 2062
public class CMSZoneShaderSettingsTrigger : MonoBehaviour
{
	// Token: 0x06003398 RID: 13208 RVA: 0x0010BF34 File Offset: 0x0010A134
	public void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x06003399 RID: 13209 RVA: 0x0010BF44 File Offset: 0x0010A144
	public void CopySettings(ZoneShaderTriggerSettings triggerSettings)
	{
		base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
		this.activateOnEnable = triggerSettings.activateOnEnable;
		if (triggerSettings.activationType == ZoneShaderTriggerSettings.ActivationType.ActivateCustomMapDefaults)
		{
			this.activateCustomMapDefaults = true;
			return;
		}
		GameObject zoneShaderSettingsObject = triggerSettings.zoneShaderSettingsObject;
		if (zoneShaderSettingsObject.IsNotNull())
		{
			this.shaderSettingsObject = zoneShaderSettingsObject;
		}
	}

	// Token: 0x0600339A RID: 13210 RVA: 0x0010BF96 File Offset: 0x0010A196
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.bodyCollider)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x0600339B RID: 13211 RVA: 0x0010BFB0 File Offset: 0x0010A1B0
	private void ActivateShaderSettings()
	{
		if (this.activateCustomMapDefaults)
		{
			CustomMapManager.ActivateDefaultZoneShaderSettings();
			return;
		}
		if (this.shaderSettingsObject.IsNotNull())
		{
			ZoneShaderSettings component = this.shaderSettingsObject.GetComponent<ZoneShaderSettings>();
			if (component.IsNotNull())
			{
				component.BecomeActiveInstance(false);
			}
		}
	}

	// Token: 0x04004076 RID: 16502
	public GameObject shaderSettingsObject;

	// Token: 0x04004077 RID: 16503
	public bool activateCustomMapDefaults;

	// Token: 0x04004078 RID: 16504
	public bool activateOnEnable;
}
