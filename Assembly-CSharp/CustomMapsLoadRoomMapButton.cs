using System;
using System.Collections;
using GorillaTagScripts.ModIO;
using UnityEngine;

// Token: 0x0200080B RID: 2059
public class CustomMapsLoadRoomMapButton : GorillaPressableButton
{
	// Token: 0x06003389 RID: 13193 RVA: 0x0010BD62 File Offset: 0x00109F62
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (CustomMapManager.CanLoadRoomMap())
		{
			CustomMapManager.ApproveAndLoadRoomMap();
		}
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x0010BD83 File Offset: 0x00109F83
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x0400406F RID: 16495
	[SerializeField]
	private float pressedTime = 0.2f;
}
