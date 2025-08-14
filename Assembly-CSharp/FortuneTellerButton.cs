using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000598 RID: 1432
public class FortuneTellerButton : GorillaPressableButton
{
	// Token: 0x060022E9 RID: 8937 RVA: 0x000BC835 File Offset: 0x000BAA35
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000BC848 File Offset: 0x000BAA48
	public override void ButtonActivation()
	{
		this.PressButtonUpdate();
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000BC850 File Offset: 0x000BAA50
	public void PressButtonUpdate()
	{
		if (this.pressTime != 0f)
		{
			return;
		}
		base.transform.localPosition = this.startingPos + this.pressedOffset;
		this.buttonRenderer.material = this.pressedMaterial;
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonUpdate>g__ButtonColorUpdate_Local|6_0());
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000BC8DD File Offset: 0x000BAADD
	[CompilerGenerated]
	private IEnumerator <PressButtonUpdate>g__ButtonColorUpdate_Local|6_0()
	{
		yield return new WaitForSeconds(this.durationPressed);
		if (this.pressTime != 0f && Time.time > this.durationPressed + this.pressTime)
		{
			base.transform.localPosition = this.startingPos;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04002C96 RID: 11414
	[SerializeField]
	private float durationPressed = 0.25f;

	// Token: 0x04002C97 RID: 11415
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04002C98 RID: 11416
	private float pressTime;

	// Token: 0x04002C99 RID: 11417
	private Vector3 startingPos;
}
