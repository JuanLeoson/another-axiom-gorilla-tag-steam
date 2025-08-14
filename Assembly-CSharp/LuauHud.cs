using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020009D5 RID: 2517
public class LuauHud : MonoBehaviour
{
	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x06003D3C RID: 15676 RVA: 0x00137DFC File Offset: 0x00135FFC
	public static LuauHud Instance
	{
		get
		{
			return LuauHud._instance;
		}
	}

	// Token: 0x06003D3D RID: 15677 RVA: 0x00137E04 File Offset: 0x00136004
	private void Awake()
	{
		if (LuauHud._instance != null && LuauHud._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			LuauHud._instance = this;
		}
		this.path = Path.Combine(Application.persistentDataPath, "script.luau");
	}

	// Token: 0x06003D3E RID: 15678 RVA: 0x00137E53 File Offset: 0x00136053
	private void OnDestroy()
	{
		if (LuauHud._instance == this)
		{
			LuauHud._instance = null;
		}
	}

	// Token: 0x06003D3F RID: 15679 RVA: 0x00137E68 File Offset: 0x00136068
	private void Start()
	{
		this.useLuauHud = true;
		DebugHudStats instance = DebugHudStats.Instance;
		instance.enabled = false;
		this.debugHud = instance.gameObject;
		this.text = instance.text;
		this.text.gameObject.SetActive(false);
		this.builder = new StringBuilder(50);
	}

	// Token: 0x06003D40 RID: 15680 RVA: 0x00137EC0 File Offset: 0x001360C0
	private void Update()
	{
		MapDescriptor loadedMapDescriptor = CustomMapLoader.LoadedMapDescriptor;
		if (loadedMapDescriptor == null || !loadedMapDescriptor.DevMode)
		{
			if (this.showLog && this.useLuauHud)
			{
				this.showLog = false;
				DebugHudStats instance = DebugHudStats.Instance;
				if (instance != null)
				{
					instance.gameObject.SetActive(false);
				}
				this.text.gameObject.SetActive(false);
			}
			return;
		}
		GorillaGameManager instance2 = GorillaGameManager.instance;
		if (instance2 == null || instance2.GameType() != GameModeType.Custom)
		{
			return;
		}
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
		if (flag != this.buttonDown && this.useLuauHud)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				if (!this.text.gameObject.activeInHierarchy)
				{
					DebugHudStats instance3 = DebugHudStats.Instance;
					if (instance3 != null)
					{
						instance3.gameObject.SetActive(true);
					}
					this.text.gameObject.SetActive(true);
					this.showLog = true;
				}
				else
				{
					DebugHudStats instance4 = DebugHudStats.Instance;
					if (instance4 != null)
					{
						instance4.gameObject.SetActive(false);
					}
					this.text.gameObject.SetActive(false);
					this.showLog = false;
				}
			}
		}
		if (!flag || !flag2)
		{
			this.resetTimer = Time.time;
		}
		if (Time.time - this.resetTimer > 2f && CustomGameMode.GameModeInitialized)
		{
			this.RestartLuauScript();
			this.resetTimer = Time.time;
		}
		if (this.useLuauHud && this.showLog)
		{
			this.builder.AppendLine();
			for (int i = 0; i < this.luauLogs.Count; i++)
			{
				this.builder.AppendLine(this.luauLogs[i]);
			}
		}
	}

	// Token: 0x06003D41 RID: 15681 RVA: 0x00138064 File Offset: 0x00136264
	public void RestartLuauScript()
	{
		this.LuauLog("Restarting Luau Script");
		LuauScriptRunner gameScriptRunner = CustomGameMode.gameScriptRunner;
		if (gameScriptRunner != null && gameScriptRunner.ShouldTick)
		{
			CustomGameMode.StopScript();
		}
		this.script = this.LoadLocalScript();
		if (this.script != "")
		{
			this.LuauLog("Loaded script from: " + this.path);
			this.LuauLog("Loaded Script Text: \n" + this.script);
			CustomGameMode.LuaScript = this.script;
		}
		CustomGameMode.LuaStart();
	}

	// Token: 0x06003D42 RID: 15682 RVA: 0x001380F0 File Offset: 0x001362F0
	public string LoadLocalScript()
	{
		string result = "";
		if (File.Exists(this.path))
		{
			result = File.ReadAllText(this.path);
		}
		return result;
	}

	// Token: 0x06003D43 RID: 15683 RVA: 0x0013811D File Offset: 0x0013631D
	public void LuauLog(string log)
	{
		Debug.Log(log);
		this.luauLogs.Add(log);
		if (this.luauLogs.Count > 6)
		{
			this.luauLogs.RemoveAt(0);
		}
	}

	// Token: 0x0400496E RID: 18798
	private bool useLuauHud;

	// Token: 0x0400496F RID: 18799
	private bool buttonDown;

	// Token: 0x04004970 RID: 18800
	private bool showLog;

	// Token: 0x04004971 RID: 18801
	private GameObject debugHud;

	// Token: 0x04004972 RID: 18802
	private TMP_Text text;

	// Token: 0x04004973 RID: 18803
	private StringBuilder builder;

	// Token: 0x04004974 RID: 18804
	private float resetTimer;

	// Token: 0x04004975 RID: 18805
	private string path = "";

	// Token: 0x04004976 RID: 18806
	private string script = "";

	// Token: 0x04004977 RID: 18807
	private static LuauHud _instance;

	// Token: 0x04004978 RID: 18808
	private List<string> luauLogs = new List<string>();
}
