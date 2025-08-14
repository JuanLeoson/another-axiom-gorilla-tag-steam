using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E0 RID: 2016
public class GorillaPlayerLineButton : MonoBehaviour
{
	// Token: 0x0600326B RID: 12907 RVA: 0x001067AE File Offset: 0x001049AE
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x0600326C RID: 12908 RVA: 0x001067C4 File Offset: 0x001049C4
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x0600326D RID: 12909 RVA: 0x001067D3 File Offset: 0x001049D3
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
				{
					this.isOn = !this.isOn;
				}
				this.parentLine.PressButton(this.isOn, this.buttonType);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600326E RID: 12910 RVA: 0x001067E4 File Offset: 0x001049E4
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
			{
				if (this.isAutoOn)
				{
					this.isOn = false;
				}
				else
				{
					this.isOn = !this.isOn;
				}
			}
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute || this.buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech || this.buttonType == GorillaPlayerLineButton.ButtonType.Cheating || this.buttonType == GorillaPlayerLineButton.ButtonType.Cancel || this.parentLine.canPressNextReportButton)
			{
				this.parentLine.PressButton(this.isOn, this.buttonType);
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
						{
							67,
							component.isLeftHand,
							0.05f
						});
					}
				}
			}
		}
	}

	// Token: 0x0600326F RID: 12911 RVA: 0x00106954 File Offset: 0x00104B54
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x06003270 RID: 12912 RVA: 0x0010697C File Offset: 0x00104B7C
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		if (this.isAutoOn)
		{
			base.GetComponent<MeshRenderer>().material = this.autoOnMaterial;
			this.myText.text = this.autoOnText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x04003F33 RID: 16179
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04003F34 RID: 16180
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04003F35 RID: 16181
	public bool isOn;

	// Token: 0x04003F36 RID: 16182
	public bool isAutoOn;

	// Token: 0x04003F37 RID: 16183
	public Material offMaterial;

	// Token: 0x04003F38 RID: 16184
	public Material onMaterial;

	// Token: 0x04003F39 RID: 16185
	public Material autoOnMaterial;

	// Token: 0x04003F3A RID: 16186
	public string offText;

	// Token: 0x04003F3B RID: 16187
	public string onText;

	// Token: 0x04003F3C RID: 16188
	public string autoOnText;

	// Token: 0x04003F3D RID: 16189
	public Text myText;

	// Token: 0x04003F3E RID: 16190
	public float debounceTime = 0.25f;

	// Token: 0x04003F3F RID: 16191
	public float touchTime;

	// Token: 0x04003F40 RID: 16192
	public bool testPress;

	// Token: 0x020007E1 RID: 2017
	public enum ButtonType
	{
		// Token: 0x04003F42 RID: 16194
		HateSpeech,
		// Token: 0x04003F43 RID: 16195
		Cheating,
		// Token: 0x04003F44 RID: 16196
		Toxicity,
		// Token: 0x04003F45 RID: 16197
		Mute,
		// Token: 0x04003F46 RID: 16198
		Report,
		// Token: 0x04003F47 RID: 16199
		Cancel
	}
}
