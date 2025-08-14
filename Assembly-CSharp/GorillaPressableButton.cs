using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020007E5 RID: 2021
public class GorillaPressableButton : MonoBehaviour
{
	// Token: 0x14000063 RID: 99
	// (add) Token: 0x0600328B RID: 12939 RVA: 0x00107638 File Offset: 0x00105838
	// (remove) Token: 0x0600328C RID: 12940 RVA: 0x00107670 File Offset: 0x00105870
	public event Action<GorillaPressableButton, bool> onPressed;

	// Token: 0x0600328D RID: 12941 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void Start()
	{
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnEnable()
	{
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnDisable()
	{
	}

	// Token: 0x06003290 RID: 12944 RVA: 0x001076A8 File Offset: 0x001058A8
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touchTime = Time.time;
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		Action<GorillaPressableButton, bool> action = this.onPressed;
		if (action != null)
		{
			action(this, component.isLeftHand);
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
		if (component == null)
		{
			return;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				67,
				component.isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x06003291 RID: 12945 RVA: 0x001077E1 File Offset: 0x001059E1
	public virtual void UpdateColor()
	{
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x06003292 RID: 12946 RVA: 0x001077F0 File Offset: 0x001059F0
	protected void UpdateColorWithState(bool state)
	{
		if (state)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (!string.IsNullOrEmpty(this.onText) || !string.IsNullOrEmpty(this.offText))
			{
				if (this.myTmpText != null)
				{
					this.myTmpText.text = this.onText;
				}
				if (this.myTmpText2 != null)
				{
					this.myTmpText2.text = this.onText;
					return;
				}
				if (this.myText != null)
				{
					this.myText.text = this.onText;
					return;
				}
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (!string.IsNullOrEmpty(this.offText) || !string.IsNullOrEmpty(this.onText))
			{
				if (this.myTmpText != null)
				{
					this.myTmpText.text = this.offText;
				}
				if (this.myTmpText2 != null)
				{
					this.myTmpText2.text = this.offText;
					return;
				}
				if (this.myText != null)
				{
					this.myText.text = this.offText;
				}
			}
		}
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x0010791C File Offset: 0x00105B1C
	public virtual void ResetState()
	{
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x0010792C File Offset: 0x00105B2C
	public void SetText(string newText)
	{
		if (this.myTmpText != null)
		{
			this.myTmpText.text = newText;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.text = newText;
		}
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
	}

	// Token: 0x04003F76 RID: 16246
	public Material pressedMaterial;

	// Token: 0x04003F77 RID: 16247
	public Material unpressedMaterial;

	// Token: 0x04003F78 RID: 16248
	public MeshRenderer buttonRenderer;

	// Token: 0x04003F79 RID: 16249
	public int pressButtonSoundIndex = 67;

	// Token: 0x04003F7A RID: 16250
	public bool isOn;

	// Token: 0x04003F7B RID: 16251
	public float debounceTime = 0.25f;

	// Token: 0x04003F7C RID: 16252
	public float touchTime;

	// Token: 0x04003F7D RID: 16253
	public bool testPress;

	// Token: 0x04003F7E RID: 16254
	public bool testHandLeft;

	// Token: 0x04003F7F RID: 16255
	[TextArea]
	public string offText;

	// Token: 0x04003F80 RID: 16256
	[TextArea]
	public string onText;

	// Token: 0x04003F81 RID: 16257
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText;

	// Token: 0x04003F82 RID: 16258
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText2;

	// Token: 0x04003F83 RID: 16259
	public Text myText;

	// Token: 0x04003F84 RID: 16260
	[Space]
	public UnityEvent onPressButton;
}
