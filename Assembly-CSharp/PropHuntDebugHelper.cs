using System;
using System.Collections;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class PropHuntDebugHelper : MonoBehaviour
{
	// Token: 0x06000931 RID: 2353 RVA: 0x000324A8 File Offset: 0x000306A8
	protected void Awake()
	{
		if (PropHuntDebugHelper.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		PropHuntDebugHelper.instance = this;
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x000324C4 File Offset: 0x000306C4
	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		this._propHuntManager = Object.FindObjectOfType<GorillaPropHuntGameManager>();
		if (this._propHuntManager != null)
		{
			Debug.Log("PropHuntDebugHelper :: Found number of props " + PropHuntPools.AllPropCosmeticIds.Length.ToString());
			this._cachedAllPropIDs = PropHuntPools.AllPropCosmeticIds;
			this._localPropHuntHandFollower = VRRig.LocalRig.GetComponent<PropHuntHandFollower>();
			this.UpdatePropsText();
		}
		yield break;
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x000324D4 File Offset: 0x000306D4
	public void UpdatePropsText()
	{
		string selectedPropID = this.GetSelectedPropID(this._selectedPropIndex);
		string text = string.Empty;
		if (this._selectedPropIndex != -1)
		{
			CosmeticSO cosmeticSO = this._allCosmetics.SearchForCosmeticSO(selectedPropID);
			if (cosmeticSO != null)
			{
				text = cosmeticSO.info.displayName;
			}
		}
		this._propsText.text = "Current Prop: " + this.GetCurrentPropInfo() + "\n" + string.Format("Selected Prop: {0} - {1} ({2}/{3})", new object[]
		{
			selectedPropID,
			text,
			this._selectedPropIndex,
			this._cachedAllPropIDs.Length
		});
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00032575 File Offset: 0x00030775
	private string GetCurrentPropInfo()
	{
		return string.Empty;
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0003257C File Offset: 0x0003077C
	private string GetSelectedPropID(int index)
	{
		if (index <= -1)
		{
			return "None";
		}
		return this._cachedAllPropIDs[index];
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00032590 File Offset: 0x00030790
	[ContextMenu("Prev Prop")]
	public void PrevProp()
	{
		this._selectedPropIndex--;
		if (this._selectedPropIndex < -1)
		{
			this._selectedPropIndex = this._cachedAllPropIDs.Length - 1;
		}
		string newPropId = (this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty;
		this.SendForcePropHandRPC(newPropId);
		this.UpdatePropsText();
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x000325F0 File Offset: 0x000307F0
	[ContextMenu("Next Prop")]
	public void NextProp()
	{
		this._selectedPropIndex++;
		if (this._selectedPropIndex >= this._cachedAllPropIDs.Length)
		{
			this._selectedPropIndex = -1;
		}
		string newPropId = (this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty;
		this.SendForcePropHandRPC(newPropId);
		this.UpdatePropsText();
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x000023F5 File Offset: 0x000005F5
	private void SendForcePropHandRPC(string newPropId)
	{
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x000023F5 File Offset: 0x000005F5
	[ContextMenu("Toggle Round")]
	public void ToggleRound()
	{
	}

	// Token: 0x04000AD8 RID: 2776
	[OnEnterPlay_SetNull]
	public static PropHuntDebugHelper instance;

	// Token: 0x04000AD9 RID: 2777
	[SerializeField]
	private GorillaPropHuntGameManager _propHuntManager;

	// Token: 0x04000ADA RID: 2778
	[SerializeField]
	private PropHuntHandFollower _localPropHuntHandFollower;

	// Token: 0x04000ADB RID: 2779
	[SerializeField]
	private TextMeshPro _propsText;

	// Token: 0x04000ADC RID: 2780
	[SerializeField]
	private AllCosmeticsArraySO _allCosmetics;

	// Token: 0x04000ADD RID: 2781
	private string[] _cachedAllPropIDs;

	// Token: 0x04000ADE RID: 2782
	private int _selectedPropIndex = -1;
}
