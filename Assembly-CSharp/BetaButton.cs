using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000417 RID: 1047
public class BetaButton : GorillaPressableButton
{
	// Token: 0x06001978 RID: 6520 RVA: 0x00089444 File Offset: 0x00087644
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x0008949C File Offset: 0x0008769C
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x040021D7 RID: 8663
	public GameObject betaParent;

	// Token: 0x040021D8 RID: 8664
	public int count;

	// Token: 0x040021D9 RID: 8665
	public float buttonFadeTime = 0.25f;

	// Token: 0x040021DA RID: 8666
	public Text messageText;
}
