using System;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000356 RID: 854
public class AppDeeplinkUI : MonoBehaviour
{
	// Token: 0x0600143E RID: 5182 RVA: 0x0006CDFC File Offset: 0x0006AFFC
	private void Start()
	{
		DebugUIBuilder instance = DebugUIBuilder.instance;
		this.uiLaunchType = instance.AddLabel("UnityDeeplinkSample", 0);
		instance.AddDivider(0);
		instance.AddButton("launch OtherApp", new DebugUIBuilder.OnClick(this.LaunchOtherApp), -1, 0, false);
		instance.AddButton("launch UnrealDeeplinkSample", new DebugUIBuilder.OnClick(this.LaunchUnrealDeeplinkSample), -1, 0, false);
		this.deeplinkAppId = CustomDebugUI.instance.AddTextField(3535750239844224UL.ToString(), 0);
		this.deeplinkMessage = CustomDebugUI.instance.AddTextField("MSG_UNITY_SAMPLE", 0);
		instance.AddButton("LaunchSelf", new DebugUIBuilder.OnClick(this.LaunchSelf), -1, 0, false);
		if (UnityEngine.Application.platform == RuntimePlatform.Android && !Core.IsInitialized())
		{
			Core.Initialize(null);
		}
		this.uiLaunchType = instance.AddLabel("LaunchType: ", 0);
		this.uiLaunchSource = instance.AddLabel("LaunchSource: ", 0);
		this.uiDeepLinkMessage = instance.AddLabel("DeeplinkMessage: ", 0);
		instance.ToggleLaserPointer(true);
		instance.Show();
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x0006CF0C File Offset: 0x0006B10C
	private void Update()
	{
		DebugUIBuilder instance = DebugUIBuilder.instance;
		if (UnityEngine.Application.platform == RuntimePlatform.Android)
		{
			LaunchDetails launchDetails = ApplicationLifecycle.GetLaunchDetails();
			this.uiLaunchType.GetComponentInChildren<Text>().text = "LaunchType: " + launchDetails.LaunchType.ToString();
			this.uiLaunchSource.GetComponentInChildren<Text>().text = "LaunchSource: " + launchDetails.LaunchSource;
			this.uiDeepLinkMessage.GetComponentInChildren<Text>().text = "DeeplinkMessage: " + launchDetails.DeeplinkMessage;
		}
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x0006CFE4 File Offset: 0x0006B1E4
	private void LaunchUnrealDeeplinkSample()
	{
		Debug.Log(string.Format("LaunchOtherApp({0})", 4055411724486843UL));
		ApplicationOptions applicationOptions = new ApplicationOptions();
		applicationOptions.SetDeeplinkMessage(this.deeplinkMessage.GetComponentInChildren<Text>().text);
		Oculus.Platform.Application.LaunchOtherApp(4055411724486843UL, applicationOptions);
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x0006D03C File Offset: 0x0006B23C
	private void LaunchSelf()
	{
		ulong num;
		if (ulong.TryParse(PlatformSettings.MobileAppID, out num))
		{
			Debug.Log(string.Format("LaunchSelf({0})", num));
			ApplicationOptions applicationOptions = new ApplicationOptions();
			applicationOptions.SetDeeplinkMessage(this.deeplinkMessage.GetComponentInChildren<Text>().text);
			Oculus.Platform.Application.LaunchOtherApp(num, applicationOptions);
		}
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x0006D090 File Offset: 0x0006B290
	private void LaunchOtherApp()
	{
		ulong num;
		if (ulong.TryParse(this.deeplinkAppId.GetComponentInChildren<Text>().text, out num))
		{
			Debug.Log(string.Format("LaunchOtherApp({0})", num));
			ApplicationOptions applicationOptions = new ApplicationOptions();
			applicationOptions.SetDeeplinkMessage(this.deeplinkMessage.GetComponentInChildren<Text>().text);
			Oculus.Platform.Application.LaunchOtherApp(num, applicationOptions);
		}
	}

	// Token: 0x04001BD1 RID: 7121
	private const ulong UNITY_COMPANION_APP_ID = 3535750239844224UL;

	// Token: 0x04001BD2 RID: 7122
	private const ulong UNREAL_COMPANION_APP_ID = 4055411724486843UL;

	// Token: 0x04001BD3 RID: 7123
	private RectTransform deeplinkAppId;

	// Token: 0x04001BD4 RID: 7124
	private RectTransform deeplinkMessage;

	// Token: 0x04001BD5 RID: 7125
	private RectTransform uiLaunchType;

	// Token: 0x04001BD6 RID: 7126
	private RectTransform uiLaunchSource;

	// Token: 0x04001BD7 RID: 7127
	private RectTransform uiDeepLinkMessage;

	// Token: 0x04001BD8 RID: 7128
	private bool inMenu = true;
}
