using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004E7 RID: 1255
public class PrimaryButtonWatcher : MonoBehaviour
{
	// Token: 0x06001E83 RID: 7811 RVA: 0x000A18D7 File Offset: 0x0009FAD7
	private void Awake()
	{
		if (this.primaryButtonPress == null)
		{
			this.primaryButtonPress = new PrimaryButtonEvent();
		}
		this.devicesWithPrimaryButton = new List<InputDevice>();
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000A18F8 File Offset: 0x0009FAF8
	private void OnEnable()
	{
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevices(list);
		foreach (InputDevice device in list)
		{
			this.InputDevices_deviceConnected(device);
		}
		InputDevices.deviceConnected += this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected += this.InputDevices_deviceDisconnected;
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000A1974 File Offset: 0x0009FB74
	private void OnDisable()
	{
		InputDevices.deviceConnected -= this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected -= this.InputDevices_deviceDisconnected;
		this.devicesWithPrimaryButton.Clear();
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000A19A4 File Offset: 0x0009FBA4
	private void InputDevices_deviceConnected(InputDevice device)
	{
		bool flag;
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, out flag))
		{
			this.devicesWithPrimaryButton.Add(device);
		}
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x000A19CD File Offset: 0x0009FBCD
	private void InputDevices_deviceDisconnected(InputDevice device)
	{
		if (this.devicesWithPrimaryButton.Contains(device))
		{
			this.devicesWithPrimaryButton.Remove(device);
		}
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x000A19EC File Offset: 0x0009FBEC
	private void Update()
	{
		bool flag = false;
		foreach (InputDevice inputDevice in this.devicesWithPrimaryButton)
		{
			bool flag2 = false;
			flag = ((inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out flag2) && flag2) || flag);
		}
		if (flag != this.lastButtonState)
		{
			this.primaryButtonPress.Invoke(flag);
			this.lastButtonState = flag;
		}
	}

	// Token: 0x04002735 RID: 10037
	public PrimaryButtonEvent primaryButtonPress;

	// Token: 0x04002736 RID: 10038
	private bool lastButtonState;

	// Token: 0x04002737 RID: 10039
	private List<InputDevice> devicesWithPrimaryButton;
}
