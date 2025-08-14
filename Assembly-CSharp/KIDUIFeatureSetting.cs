using System;
using KID.Model;
using TMPro;
using UnityEngine;

// Token: 0x0200091B RID: 2331
public class KIDUIFeatureSetting : MonoBehaviour
{
	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x06003983 RID: 14723 RVA: 0x0012A2C1 File Offset: 0x001284C1
	// (set) Token: 0x06003984 RID: 14724 RVA: 0x0012A2C9 File Offset: 0x001284C9
	public bool AlwaysCheckFeatureSetting { get; private set; }

	// Token: 0x06003985 RID: 14725 RVA: 0x0012A2D2 File Offset: 0x001284D2
	public void CreateNewFeatureSettingGuardianManaged(KIDUI_MainScreen.FeatureToggleSetup feature, bool isEnabled)
	{
		this.CreateNewFeatureSettingWithoutToggle(feature, false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
	}

	// Token: 0x06003986 RID: 14726 RVA: 0x0012A2F7 File Offset: 0x001284F7
	public KIDUIToggle CreateNewFeatureSettingWithToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool initialState = false, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, true);
		this._featureToggle.SetValue(initialState);
		KIDUIToggle featureToggle = this._featureToggle;
		if (featureToggle != null)
		{
			featureToggle.RegisterOnChangeEvent(new Action(this.SetFeatureName));
		}
		return this._featureToggle;
	}

	// Token: 0x06003987 RID: 14727 RVA: 0x0012A331 File Offset: 0x00128531
	public void CreateNewFeatureSettingWithoutToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, false);
	}

	// Token: 0x06003988 RID: 14728 RVA: 0x0012A33C File Offset: 0x0012853C
	private void SetFeatureData(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting, bool featureToggleEnabled)
	{
		this._enabledTextStr = feature.enabledText;
		this._disabledTextStr = feature.disabledText;
		this._hasToggle = featureToggleEnabled;
		this._featureType = feature.linkedFeature;
		this._featureName = feature.featureName;
		this.SetFeatureName();
		GameObject gameObject = base.gameObject;
		gameObject.name = gameObject.name + "_" + feature.featureName;
		this._permissionName = feature.permissionName;
		this._featureToggle.gameObject.SetActive(featureToggleEnabled);
		this.AlwaysCheckFeatureSetting = alwaysCheckFeatureSetting;
	}

	// Token: 0x06003989 RID: 14729 RVA: 0x0012A3CB File Offset: 0x001285CB
	public void UnregisterOnToggleChangeEvent(Action action)
	{
		this._featureToggle.UnregisterOnChangeEvent(action);
	}

	// Token: 0x0600398A RID: 14730 RVA: 0x0012A3D9 File Offset: 0x001285D9
	public void RegisterToggleOnEvent(Action action)
	{
		this._featureToggle.RegisterToggleOnEvent(action);
	}

	// Token: 0x0600398B RID: 14731 RVA: 0x0012A3E7 File Offset: 0x001285E7
	public void UnregisterToggleOnEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOnEvent(action);
	}

	// Token: 0x0600398C RID: 14732 RVA: 0x0012A3F5 File Offset: 0x001285F5
	public void RegisterToggleOffEvent(Action action)
	{
		this._featureToggle.RegisterToggleOffEvent(action);
	}

	// Token: 0x0600398D RID: 14733 RVA: 0x0012A403 File Offset: 0x00128603
	public void UnregisterToggleOffEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOffEvent(action);
	}

	// Token: 0x0600398E RID: 14734 RVA: 0x0012A411 File Offset: 0x00128611
	public bool GetFeatureToggleState()
	{
		if (this._hasToggle)
		{
			return this._featureToggle.IsOn;
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._featureType);
		if (permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogError("[KID::FeatureSetting] GetToggleState: feature has no toggle AND is not managed by Guardian");
		}
		return permissionDataByFeature.Enabled;
	}

	// Token: 0x0600398F RID: 14735 RVA: 0x0012A44A File Offset: 0x0012864A
	public bool GetHasToggle()
	{
		return this._hasToggle;
	}

	// Token: 0x06003990 RID: 14736 RVA: 0x0012A452 File Offset: 0x00128652
	public void SetFeatureSettingVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	// Token: 0x06003991 RID: 14737 RVA: 0x0012A460 File Offset: 0x00128660
	public void SetFeatureToggle(bool enableToggle)
	{
		this._featureToggle.interactable = enableToggle;
	}

	// Token: 0x06003992 RID: 14738 RVA: 0x0012A46E File Offset: 0x0012866E
	public void SetGuardianManagedState(bool isEnabled)
	{
		this._featureToggle.gameObject.SetActive(false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
		this.SetFeatureName();
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x0012A4A4 File Offset: 0x001286A4
	public void SetPlayerManagedState(bool isInteractable, bool isOptedIn)
	{
		this._featureToggle.gameObject.SetActive(true);
		this._guardianManagedEnabled.SetActive(false);
		this._guardianManagedLocked.SetActive(false);
		this._featureToggle.interactable = isInteractable;
		this._featureToggle.SetValue(isOptedIn);
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x0012A4F4 File Offset: 0x001286F4
	private void SetFeatureName()
	{
		string text = this.GetFeatureToggleState() ? ("<b>(" + this._enabledTextStr + ")</b>") : ("<b>(" + this._disabledTextStr + ")</b>");
		this._featureNameTxt.text = "<b>" + this._featureName + "</b>";
		this._featureStatusTxt.text = (text ?? "");
	}

	// Token: 0x040046B4 RID: 18100
	[SerializeField]
	private TMP_Text _featureNameTxt;

	// Token: 0x040046B5 RID: 18101
	[SerializeField]
	private TMP_Text _featureStatusTxt;

	// Token: 0x040046B6 RID: 18102
	[SerializeField]
	private KIDUIToggle _featureToggle;

	// Token: 0x040046B7 RID: 18103
	[SerializeField]
	private GameObject _tickIcon;

	// Token: 0x040046B8 RID: 18104
	[SerializeField]
	private GameObject _crossIcon;

	// Token: 0x040046B9 RID: 18105
	[SerializeField]
	private GameObject _guardianManagedLocked;

	// Token: 0x040046BA RID: 18106
	[SerializeField]
	private GameObject _guardianManagedEnabled;

	// Token: 0x040046BB RID: 18107
	private bool _hasToggle;

	// Token: 0x040046BC RID: 18108
	private string _featureName;

	// Token: 0x040046BD RID: 18109
	private string _permissionName;

	// Token: 0x040046BE RID: 18110
	private string _enabledTextStr;

	// Token: 0x040046BF RID: 18111
	private string _disabledTextStr;

	// Token: 0x040046C0 RID: 18112
	private EKIDFeatures _featureType;

	// Token: 0x040046C1 RID: 18113
	private Action<EKIDFeatures> _onChangeCallback;
}
