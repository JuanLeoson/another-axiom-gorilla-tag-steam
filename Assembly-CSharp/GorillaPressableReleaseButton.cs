using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007E6 RID: 2022
public class GorillaPressableReleaseButton : GorillaPressableButton
{
	// Token: 0x06003298 RID: 12952 RVA: 0x001079A4 File Offset: 0x00105BA4
	private new void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touchingCollider)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		this.touchTime = Time.time;
		this.touchingCollider = other;
		UnityEvent onPressButton = this.onPressButton;
		if (onPressButton != null)
		{
			onPressButton.Invoke();
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
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

	// Token: 0x06003299 RID: 12953 RVA: 0x00107ACC File Offset: 0x00105CCC
	private void OnTriggerExit(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (other != this.touchingCollider)
		{
			return;
		}
		this.touchingCollider = null;
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component == null)
		{
			return;
		}
		UnityEvent unityEvent = this.onReleaseButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this.ButtonDeactivation();
		this.ButtonDeactivationWithHand(component.isLeftHand);
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

	// Token: 0x0600329A RID: 12954 RVA: 0x00107BD4 File Offset: 0x00105DD4
	public override void ResetState()
	{
		base.ResetState();
		this.touchingCollider = null;
	}

	// Token: 0x0600329B RID: 12955 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void ButtonDeactivation()
	{
	}

	// Token: 0x0600329C RID: 12956 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void ButtonDeactivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x04003F86 RID: 16262
	public UnityEvent onReleaseButton;

	// Token: 0x04003F87 RID: 16263
	private Collider touchingCollider;
}
