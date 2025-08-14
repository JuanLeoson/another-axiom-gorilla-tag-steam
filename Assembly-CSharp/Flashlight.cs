using System;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class Flashlight : MonoBehaviour
{
	// Token: 0x0600149E RID: 5278 RVA: 0x0006F1A8 File Offset: 0x0006D3A8
	private void LateUpdate()
	{
		for (int i = 0; i < this.lightVolume.transform.childCount; i++)
		{
			this.lightVolume.transform.GetChild(i).rotation = Quaternion.LookRotation((this.lightVolume.transform.GetChild(i).position - Camera.main.transform.position).normalized);
		}
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0006F220 File Offset: 0x0006D420
	public void ToggleFlashlight()
	{
		this.lightVolume.SetActive(!this.lightVolume.activeSelf);
		this.spotlight.enabled = !this.spotlight.enabled;
		this.bulbGlow.SetActive(this.lightVolume.activeSelf);
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x0006F275 File Offset: 0x0006D475
	public void EnableFlashlight(bool doEnable)
	{
		this.lightVolume.SetActive(doEnable);
		this.spotlight.enabled = doEnable;
		this.bulbGlow.SetActive(doEnable);
	}

	// Token: 0x04001C23 RID: 7203
	public GameObject lightVolume;

	// Token: 0x04001C24 RID: 7204
	public Light spotlight;

	// Token: 0x04001C25 RID: 7205
	public GameObject bulbGlow;
}
