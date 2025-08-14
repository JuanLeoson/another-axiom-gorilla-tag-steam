using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D3 RID: 1747
[Obsolete("This class is obsolete and will be removed in a future version. (MattO 2024-02-26) It doesn't appear to be used anywhere.")]
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x06002B8A RID: 11146 RVA: 0x000E6170 File Offset: 0x000E4370
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			if (this.touchTime + this.debounceTime < Time.time)
			{
				this.touchTime = Time.time;
				this.isOn = !this.isOn;
				this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			}
		}
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x000E61D8 File Offset: 0x000E43D8
	private void OnTriggerEnter(Collider collider)
	{
		if (this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.isOn = !this.isOn;
			this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x000E6278 File Offset: 0x000E4478
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x040036D0 RID: 14032
	public GorillaHatButtonParent buttonParent;

	// Token: 0x040036D1 RID: 14033
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x040036D2 RID: 14034
	public bool isOn;

	// Token: 0x040036D3 RID: 14035
	public Material offMaterial;

	// Token: 0x040036D4 RID: 14036
	public Material onMaterial;

	// Token: 0x040036D5 RID: 14037
	public string offText;

	// Token: 0x040036D6 RID: 14038
	public string onText;

	// Token: 0x040036D7 RID: 14039
	public Text myText;

	// Token: 0x040036D8 RID: 14040
	public float debounceTime = 0.25f;

	// Token: 0x040036D9 RID: 14041
	public float touchTime;

	// Token: 0x040036DA RID: 14042
	public string cosmeticName;

	// Token: 0x040036DB RID: 14043
	public bool testPress;

	// Token: 0x020006D4 RID: 1748
	public enum HatButtonType
	{
		// Token: 0x040036DD RID: 14045
		Hat,
		// Token: 0x040036DE RID: 14046
		Face,
		// Token: 0x040036DF RID: 14047
		Badge
	}
}
