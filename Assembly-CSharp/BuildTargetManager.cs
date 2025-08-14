using System;
using UnityEngine;

// Token: 0x02000A7F RID: 2687
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x06004177 RID: 16759 RVA: 0x0014AD57 File Offset: 0x00148F57
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x04004CE9 RID: 19689
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x04004CEA RID: 19690
	public bool isBeta;

	// Token: 0x04004CEB RID: 19691
	public bool isQA;

	// Token: 0x04004CEC RID: 19692
	public bool spoofIDs;

	// Token: 0x04004CED RID: 19693
	public bool spoofChild;

	// Token: 0x04004CEE RID: 19694
	public bool enableAllCosmetics;

	// Token: 0x04004CEF RID: 19695
	public OVRManager ovrManager;

	// Token: 0x04004CF0 RID: 19696
	private string path = "Assets/csc.rsp";

	// Token: 0x04004CF1 RID: 19697
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x04004CF2 RID: 19698
	public GorillaTagger gorillaTagger;

	// Token: 0x04004CF3 RID: 19699
	public GameObject[] betaDisableObjects;

	// Token: 0x04004CF4 RID: 19700
	public GameObject[] betaEnableObjects;

	// Token: 0x04004CF5 RID: 19701
	public BuildTargetManager.NetworkBackend networkBackend;

	// Token: 0x02000A80 RID: 2688
	public enum BuildTowards
	{
		// Token: 0x04004CF7 RID: 19703
		Steam,
		// Token: 0x04004CF8 RID: 19704
		OculusPC,
		// Token: 0x04004CF9 RID: 19705
		Quest,
		// Token: 0x04004CFA RID: 19706
		Viveport
	}

	// Token: 0x02000A81 RID: 2689
	public enum NetworkBackend
	{
		// Token: 0x04004CFC RID: 19708
		Pun,
		// Token: 0x04004CFD RID: 19709
		Fusion
	}
}
