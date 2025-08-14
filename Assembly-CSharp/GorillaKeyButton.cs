using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007DD RID: 2013
public abstract class GorillaKeyButton<TBinding> : MonoBehaviour where TBinding : Enum
{
	// Token: 0x0600325C RID: 12892 RVA: 0x00106489 File Offset: 0x00104689
	private void Awake()
	{
		if (this.ButtonRenderer == null)
		{
			this.ButtonRenderer = base.GetComponent<Renderer>();
		}
		this.propBlock = new MaterialPropertyBlock();
		this.pressTime = 0f;
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x001064BC File Offset: 0x001046BC
	private void OnEnable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(true);
			}
		}
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x001064FC File Offset: 0x001046FC
	private void OnDisable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(false);
			}
		}
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x0010653C File Offset: 0x0010473C
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.OnButtonPressedEvent();
			UnityEvent<TBinding> onKeyButtonPressed = this.OnKeyButtonPressed;
			if (onKeyButtonPressed != null)
			{
				onKeyButtonPressed.Invoke(this.Binding);
			}
			this.PressButtonColourUpdate();
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, component.isLeftHand, 0.1f);
				if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
					{
						66,
						component.isLeftHand,
						0.1f
					});
				}
			}
		}
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x00106634 File Offset: 0x00104834
	public virtual void PressButtonColourUpdate()
	{
		this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.PressedColor);
		this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.PressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|17_0());
	}

	// Token: 0x06003261 RID: 12897
	protected abstract void OnButtonPressedEvent();

	// Token: 0x06003263 RID: 12899 RVA: 0x001066BE File Offset: 0x001048BE
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|17_0()
	{
		yield return new WaitForSeconds(this.ButtonColorSettings.PressedTime);
		if (this.pressTime != 0f && Time.time > this.ButtonColorSettings.PressedTime + this.pressTime)
		{
			this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.UnpressedColor);
			this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.UnpressedColor);
			this.ButtonRenderer.SetPropertyBlock(this.propBlock);
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04003F23 RID: 16163
	public string characterString;

	// Token: 0x04003F24 RID: 16164
	public TBinding Binding;

	// Token: 0x04003F25 RID: 16165
	public bool functionKey;

	// Token: 0x04003F26 RID: 16166
	public Renderer ButtonRenderer;

	// Token: 0x04003F27 RID: 16167
	public ButtonColorSettings ButtonColorSettings;

	// Token: 0x04003F28 RID: 16168
	[Tooltip("These GameObjects will be Activated/Deactivated when this button is Activated/Deactivated")]
	public GameObject[] linkedObjects;

	// Token: 0x04003F29 RID: 16169
	[Tooltip("Intended for use with GorillaKeyWrapper")]
	public UnityEvent<TBinding> OnKeyButtonPressed = new UnityEvent<TBinding>();

	// Token: 0x04003F2A RID: 16170
	public bool testClick;

	// Token: 0x04003F2B RID: 16171
	public bool repeatTestClick;

	// Token: 0x04003F2C RID: 16172
	public float repeatCooldown = 2f;

	// Token: 0x04003F2D RID: 16173
	private float pressTime;

	// Token: 0x04003F2E RID: 16174
	private float lastTestClick;

	// Token: 0x04003F2F RID: 16175
	protected MaterialPropertyBlock propBlock;
}
