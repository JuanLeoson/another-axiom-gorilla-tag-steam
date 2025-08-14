using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000423 RID: 1059
[Obsolete("Replaced with bundlebutton")]
public class EarlyAccessButton : GorillaPressableButton
{
	// Token: 0x0600199E RID: 6558 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x00089D94 File Offset: 0x00087F94
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x00089DEE File Offset: 0x00087FEE
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x00089E0F File Offset: 0x0008800F
	public void AlreadyOwn()
	{
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x00089E45 File Offset: 0x00088045
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}
}
