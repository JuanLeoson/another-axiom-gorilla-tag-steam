using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200074F RID: 1871
public class HeldButton : MonoBehaviour
{
	// Token: 0x06002EDE RID: 11998 RVA: 0x000F843C File Offset: 0x000F663C
	private void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		if ((componentInParent.isLeftHand && !this.leftHandPressable) || (!componentInParent.isLeftHand && !this.rightHandPressable))
		{
			return;
		}
		if (!this.pendingPress || other != this.pendingPressCollider)
		{
			UnityEvent unityEvent = this.onStartPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.touchTime = Time.time;
			this.pendingPressCollider = other;
			this.pressingHand = componentInParent;
			this.pendingPress = true;
			this.SetOn(true);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x000F84FC File Offset: 0x000F66FC
	private void LateUpdate()
	{
		if (!this.pendingPress)
		{
			return;
		}
		if (this.touchTime < this.releaseTime && this.releaseTime + this.debounceTime < Time.time)
		{
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.SetOn(false);
			return;
		}
		if (this.touchTime + this.pressDuration < Time.time)
		{
			this.onPressButton.Invoke();
			if (this.pressingHand != null)
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, this.pressingHand.isLeftHand, 0.1f);
				GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			UnityEvent unityEvent2 = this.onStopPressingButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.releaseTime = Time.time;
			this.SetOn(false);
			return;
		}
		if (this.touchTime > this.releaseTime && this.pressingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, Time.fixedDeltaTime);
		}
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x000F865B File Offset: 0x000F685B
	private void OnTriggerExit(Collider other)
	{
		if (this.pendingPress && this.pendingPressCollider == other)
		{
			this.releaseTime = Time.time;
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x000F8690 File Offset: 0x000F6890
	public void SetOn(bool inOn)
	{
		if (inOn == this.isOn)
		{
			return;
		}
		this.isOn = inOn;
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x04003AD9 RID: 15065
	public Material pressedMaterial;

	// Token: 0x04003ADA RID: 15066
	public Material unpressedMaterial;

	// Token: 0x04003ADB RID: 15067
	public MeshRenderer buttonRenderer;

	// Token: 0x04003ADC RID: 15068
	private bool isOn;

	// Token: 0x04003ADD RID: 15069
	public float debounceTime = 0.25f;

	// Token: 0x04003ADE RID: 15070
	public bool leftHandPressable;

	// Token: 0x04003ADF RID: 15071
	public bool rightHandPressable = true;

	// Token: 0x04003AE0 RID: 15072
	public float pressDuration = 0.5f;

	// Token: 0x04003AE1 RID: 15073
	public UnityEvent onStartPressingButton;

	// Token: 0x04003AE2 RID: 15074
	public UnityEvent onStopPressingButton;

	// Token: 0x04003AE3 RID: 15075
	public UnityEvent onPressButton;

	// Token: 0x04003AE4 RID: 15076
	[TextArea]
	public string offText;

	// Token: 0x04003AE5 RID: 15077
	[TextArea]
	public string onText;

	// Token: 0x04003AE6 RID: 15078
	public Text myText;

	// Token: 0x04003AE7 RID: 15079
	private float touchTime;

	// Token: 0x04003AE8 RID: 15080
	private float releaseTime;

	// Token: 0x04003AE9 RID: 15081
	private bool pendingPress;

	// Token: 0x04003AEA RID: 15082
	private Collider pendingPressCollider;

	// Token: 0x04003AEB RID: 15083
	private GorillaTriggerColliderHandIndicator pressingHand;
}
