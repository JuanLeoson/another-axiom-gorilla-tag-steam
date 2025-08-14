using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000691 RID: 1681
public class GRUIEmployeeTerminal : MonoBehaviour
{
	// Token: 0x0600292C RID: 10540 RVA: 0x000DD778 File Offset: 0x000DB978
	public void Setup()
	{
		this.signupButton.onPressButton.AddListener(new UnityAction(this.OnSignup));
		PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new PlayFab.ClientModels.GetUserDataRequest();
		getUserDataRequest.PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		getUserDataRequest.Keys = new List<string>
		{
			"GRData"
		};
		this.isSigningUp = true;
		PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetUserDataInitialState), new Action<PlayFabError>(this.OnGetUserDataInitialStateFail), null, null);
		this.Refresh();
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000DD7FC File Offset: 0x000DB9FC
	public void OnSignup()
	{
		if (this.isSigningUp || this.isEmployee)
		{
			return;
		}
		UpdateUserDataRequest request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string>
			{
				{
					"GRData",
					"Now we have data"
				}
			}
		};
		if (!PlayFabClientAPI.IsClientLoggedIn())
		{
			if (PlayFabAuthenticator.instance != null)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			return;
		}
		this.isSigningUp = true;
		PlayFabClientAPI.UpdateUserData(request, new Action<UpdateUserDataResult>(this.OnSaveTableSuccess), new Action<PlayFabError>(this.OnSaveTableFailure), null, null);
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000DD885 File Offset: 0x000DBA85
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x000DD890 File Offset: 0x000DBA90
	public void Refresh()
	{
		if (this.isSigningUp)
		{
			this.signupButtonText.text = "APPLYING";
			return;
		}
		if (this.isEmployee)
		{
			this.signupButtonText.text = "HIRED";
			return;
		}
		this.signupButtonText.text = "APPLY";
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x000DD8E0 File Offset: 0x000DBAE0
	private void OnGetUserDataInitialState(GetUserDataResult result)
	{
		UserDataRecord userDataRecord;
		if (result.Data.TryGetValue("GRData", out userDataRecord))
		{
			string value = userDataRecord.Value;
			this.isEmployee = true;
		}
		else
		{
			this.isEmployee = false;
		}
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x000DD925 File Offset: 0x000DBB25
	private void OnGetUserDataInitialStateFail(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002932 RID: 10546 RVA: 0x000DD93B File Offset: 0x000DBB3B
	private void OnSaveTableSuccess(UpdateUserDataResult result)
	{
		this.isEmployee = true;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002933 RID: 10547 RVA: 0x000DD925 File Offset: 0x000DBB25
	private void OnSaveTableFailure(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x0400351C RID: 13596
	[SerializeField]
	private GorillaPressableButton signupButton;

	// Token: 0x0400351D RID: 13597
	[SerializeField]
	private TMP_Text signupButtonText;

	// Token: 0x0400351E RID: 13598
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x0400351F RID: 13599
	[SerializeField]
	private GRUIStationEmployeeBadges badgeStation;

	// Token: 0x04003520 RID: 13600
	private int entityTypeId;

	// Token: 0x04003521 RID: 13601
	private bool isEmployee;

	// Token: 0x04003522 RID: 13602
	private bool isSigningUp;

	// Token: 0x04003523 RID: 13603
	private const string GR_DATA_KEY = "GRData";
}
