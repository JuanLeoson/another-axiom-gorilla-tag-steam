using System;

// Token: 0x02000162 RID: 354
internal class PropHuntPools_Callbacks
{
	// Token: 0x06000977 RID: 2423 RVA: 0x00034176 File Offset: 0x00032376
	internal void ListenForZoneChanged()
	{
		if (PropHuntPools_Callbacks._isListeningForZoneChanged)
		{
			return;
		}
		ZoneManagement.OnZoneChange += this._OnZoneChanged;
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x00034194 File Offset: 0x00032394
	private void _OnZoneChanged(ZoneData[] zoneDatas)
	{
		if (VRRigCache.Instance == null || VRRigCache.Instance.localRig == null || VRRigCache.Instance.localRig.Rig == null || VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone != GTZone.bayou)
		{
			return;
		}
		PropHuntPools_Callbacks._isListeningForZoneChanged = false;
		ZoneManagement.OnZoneChange -= this._OnZoneChanged;
		PropHuntPools.OnLocalPlayerEnteredBayou();
	}

	// Token: 0x04000B31 RID: 2865
	private const string preLog = "PropHuntPools_Callbacks: ";

	// Token: 0x04000B32 RID: 2866
	private const string preLogEd = "(editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x04000B33 RID: 2867
	private const string preLogBeta = "(beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x04000B34 RID: 2868
	private const string preErr = "ERROR!!!  PropHuntPools_Callbacks: ";

	// Token: 0x04000B35 RID: 2869
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x04000B36 RID: 2870
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x04000B37 RID: 2871
	internal static readonly PropHuntPools_Callbacks instance = new PropHuntPools_Callbacks();

	// Token: 0x04000B38 RID: 2872
	private static bool _isListeningForZoneChanged;
}
