using System;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox
{
	// Token: 0x06001E71 RID: 7793 RVA: 0x000A1517 File Offset: 0x0009F717
	private void Awake()
	{
		if (this.sameSceneSettingsRef != null)
		{
			this.settings = this.sameSceneSettingsRef;
			return;
		}
		this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x000A1548 File Offset: 0x0009F748
	public override void OnBoxTriggered()
	{
		if (this.settings == null)
		{
			if (this.sameSceneSettingsRef != null)
			{
				this.settings = this.sameSceneSettingsRef;
			}
			else
			{
				this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
			}
		}
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance(false);
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	// Token: 0x0400271F RID: 10015
	[SerializeField]
	private XSceneRef settingsRef;

	// Token: 0x04002720 RID: 10016
	[SerializeField]
	private ZoneShaderSettings sameSceneSettingsRef;

	// Token: 0x04002721 RID: 10017
	private ZoneShaderSettings settings;
}
