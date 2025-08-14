using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200048C RID: 1164
public class WardrobeFunctionButton : GorillaPressableButton
{
	// Token: 0x06001CE4 RID: 7396 RVA: 0x0009BC6A File Offset: 0x00099E6A
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void UpdateColor()
	{
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x0009BC91 File Offset: 0x00099E91
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002551 RID: 9553
	public string function;

	// Token: 0x04002552 RID: 9554
	public float buttonFadeTime = 0.25f;
}
