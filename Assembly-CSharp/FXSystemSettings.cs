using System;
using UnityEngine;

// Token: 0x02000A55 RID: 2645
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x06004091 RID: 16529 RVA: 0x00146C90 File Offset: 0x00144E90
	public void Awake()
	{
		int num = (this.callLimits != null) ? this.callLimits.Length : 0;
		int num2 = (this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0;
		for (int i = 0; i < num; i++)
		{
			FXType key = this.callLimits[i].Key;
			int num3 = (int)key;
			if (num3 < 0 || num3 >= 23)
			{
				string str = "NO_PATH_AT_RUNTIME";
				Debug.LogError("FXSystemSettings: (this should never happen) `callLimits.Key` is out of bounds of `callSettings`! Path=\"" + str + "\"", this);
			}
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping.");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType key = this.CallLimitsCooldown[i].Key;
			int num3 = (int)key;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
		for (int i = 0; i < this.callSettings.Length; i++)
		{
			if (this.callSettings[i] == null)
			{
				this.callSettings[i] = new LimiterType
				{
					CallLimitSettings = new CallLimiter(0, 0f, 0f),
					Key = (FXType)i
				};
			}
		}
	}

	// Token: 0x04004C38 RID: 19512
	private const string preLog = "FXSystemSettings: ";

	// Token: 0x04004C39 RID: 19513
	private const string preErr = "ERROR!!!  FXSystemSettings: ";

	// Token: 0x04004C3A RID: 19514
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x04004C3B RID: 19515
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x04004C3C RID: 19516
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x04004C3D RID: 19517
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[23];
}
