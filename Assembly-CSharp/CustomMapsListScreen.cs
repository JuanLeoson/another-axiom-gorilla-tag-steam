using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using GorillaTagScripts.UI;
using ModIO;
using PlayFab;
using TMPro;
using UnityEngine;

// Token: 0x02000843 RID: 2115
public class CustomMapsListScreen : CustomMapsTerminalScreen
{
	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x060034F0 RID: 13552 RVA: 0x0011476F File Offset: 0x0011296F
	public int CurrentModPage
	{
		get
		{
			return this.currentModPage;
		}
	}

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x060034F1 RID: 13553 RVA: 0x00114777 File Offset: 0x00112977
	public int SelectedModIndex
	{
		get
		{
			return this.selectedModIndex;
		}
	}

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x060034F2 RID: 13554 RVA: 0x0011477F File Offset: 0x0011297F
	public int ModsPerPage
	{
		get
		{
			return this.modsPerPage;
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x060034F3 RID: 13555 RVA: 0x00114787 File Offset: 0x00112987
	// (set) Token: 0x060034F4 RID: 13556 RVA: 0x00114790 File Offset: 0x00112990
	public SortModsBy SortType
	{
		get
		{
			return this.sortType;
		}
		set
		{
			if (this.sortType != value)
			{
				this.currentAvailableModsRequestPage = 0;
			}
			this.sortType = value;
			switch (this.sortType)
			{
			case SortModsBy.Name:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Price:
				break;
			case SortModsBy.Rating:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Popular:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Downloads:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Subscribers:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.DateSubmitted:
				this.isAscendingOrder = true;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x000023F5 File Offset: 0x000005F5
	public override void Initialize()
	{
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x00114810 File Offset: 0x00112A10
	public override void Show()
	{
		base.Show();
		if (this.featuredMods.IsNullOrEmpty<ModProfile>())
		{
			this.RetrieveFeaturedMods();
		}
		if (this.availableMods.IsNullOrEmpty<ModProfile>())
		{
			this.RetrieveAvailableMods();
		}
		if (this.subscribedMods.IsNullOrEmpty<SubscribedMod>())
		{
			this.RetrieveSubscribedMods();
		}
		this.RefreshScreenState();
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x00114864 File Offset: 0x00112A64
	protected override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			return;
		}
		if (this.loadingText.gameObject.activeSelf)
		{
			return;
		}
		if (this.CheckTags(buttonPressed))
		{
			this.Refresh(null);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option3)
		{
			ModIOManager.Refresh(delegate(bool result)
			{
				if (result)
				{
					this.Refresh(null);
				}
			}, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.up)
		{
			this.selectedModIndex--;
			if ((this.IsOnFirstPage() && this.selectedModIndex < 0) || (!this.IsOnFirstPage() && this.selectedModIndex < -1))
			{
				this.selectedModIndex = (this.IsOnLastPage() ? (this.displayedModProfiles.Count - 1) : this.displayedModProfiles.Count);
			}
			this.UpdateModListSelection();
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.down)
		{
			this.selectedModIndex++;
			if ((this.IsOnLastPage() && this.selectedModIndex >= this.displayedModProfiles.Count) || (!this.IsOnLastPage() && this.selectedModIndex > this.displayedModProfiles.Count))
			{
				this.selectedModIndex = (this.IsOnFirstPage() ? 0 : -1);
			}
			this.UpdateModListSelection();
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.enter)
		{
			if (!this.IsOnFirstPage() && this.selectedModIndex == -1)
			{
				this.currentModPage--;
				this.selectedModIndex = 0;
				CustomMapsTerminal.SendTerminalStatus(false, false);
				this.RefreshScreenState();
				return;
			}
			if (!this.IsOnLastPage() && this.selectedModIndex == this.displayedModProfiles.Count)
			{
				this.currentModPage++;
				this.selectedModIndex = 0;
				CustomMapsTerminal.SendTerminalStatus(false, false);
				this.RefreshScreenState();
				return;
			}
			if (this.selectedModIndex >= 0 && this.selectedModIndex < this.displayedModProfiles.Count)
			{
				CustomMapsTerminal.ShowDetailsScreen(this.displayedModProfiles[this.selectedModIndex]);
				return;
			}
		}
		else
		{
			if (buttonPressed == CustomMapKeyboardBinding.sub)
			{
				this.SwapListDisplay();
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.sort)
			{
				this.SetSortType();
				this.Refresh(null);
			}
		}
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x00114A5A File Offset: 0x00112C5A
	public void ClearTags(bool clearModLists = false)
	{
		this.searchTags.Clear();
		if (clearModLists)
		{
			this.featuredMods.Clear();
			this.availableMods.Clear();
		}
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x00114A80 File Offset: 0x00112C80
	public void UpdateTagsFromDriver(List<string> tags)
	{
		this.currentAvailableModsRequestPage = 0;
		this.searchTags.Clear();
		if (tags.IsNullOrEmpty<string>())
		{
			return;
		}
		this.searchTags.AddRange(tags);
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x00114AAC File Offset: 0x00112CAC
	private bool CheckTags(CustomMapKeyboardBinding buttonPressed)
	{
		bool result = false;
		short num = (short)buttonPressed;
		if (num > 0 && num < 10)
		{
			result = true;
			string item;
			if (CustomMapsTerminal.SetTagButtonStatus(num, out item))
			{
				if (!this.searchTags.Contains(item))
				{
					this.searchTags.Add(item);
				}
			}
			else if (this.searchTags.Contains(item))
			{
				this.searchTags.Remove(item);
			}
		}
		return result;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x00114B0C File Offset: 0x00112D0C
	private void SetSortType()
	{
		this.currentAvailableModsRequestPage = 0;
		this.sortTypeIndex++;
		if (this.sortTypeIndex >= 6)
		{
			this.sortTypeIndex = 0;
		}
		switch (this.sortTypeIndex)
		{
		case 0:
			this.sortType = SortModsBy.Popular;
			this.isAscendingOrder = false;
			return;
		case 1:
			this.sortType = SortModsBy.Name;
			this.isAscendingOrder = true;
			return;
		case 2:
			this.sortType = SortModsBy.Rating;
			this.isAscendingOrder = true;
			return;
		case 3:
			this.sortType = SortModsBy.Downloads;
			this.isAscendingOrder = true;
			return;
		case 4:
			this.sortType = SortModsBy.Subscribers;
			this.isAscendingOrder = true;
			return;
		case 5:
			this.sortType = SortModsBy.DateSubmitted;
			this.isAscendingOrder = true;
			return;
		default:
			this.sortTypeIndex = 0;
			this.sortType = SortModsBy.Popular;
			this.isAscendingOrder = false;
			return;
		}
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x00114BD4 File Offset: 0x00112DD4
	private void SwapListDisplay()
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			this.currentState = CustomMapsListScreen.ListScreenState.SubscribedMods;
		}
		else if (this.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods)
		{
			this.currentState = CustomMapsListScreen.ListScreenState.AvailableMods;
		}
		this.selectedModIndex = 0;
		this.currentModPage = 0;
		CustomMapsTerminal.UpdateListScreenState(this.currentState);
		CustomMapsTerminal.SendTerminalStatus(this.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods, false);
		this.RefreshScreenState();
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x00114C30 File Offset: 0x00112E30
	public void Refresh(long[] customModListIds = null)
	{
		if (this.loadingAvailableMods || this.loadingFeaturedMods || this.loadingCustomModList)
		{
			return;
		}
		this.currentModPage = 0;
		this.selectedModIndex = 0;
		CustomMapsTerminal.SendTerminalStatus(false, true);
		this.featuredMods.Clear();
		this.availableMods.Clear();
		this.filteredAvailableMods.Clear();
		this.currentAvailableModsRequestPage = 0;
		this.errorLoadingAvailableMods = false;
		this.totalAvailableMods = 0;
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
		if (customModListIds != null && customModListIds.Length != 0)
		{
			this.customModListModIds.Clear();
			this.customModListModIds.AddRange(customModListIds);
		}
		this.customModList.Clear();
		this.RetrieveFeaturedMods();
		this.RetrieveAvailableMods();
		this.RetrieveSubscribedMods();
		this.RetrieveCustomModList();
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x00114CFC File Offset: 0x00112EFC
	private void RetrieveFeaturedMods()
	{
		if (this.loadingFeaturedMods || this.featuredMods.Count > 0)
		{
			return;
		}
		this.loadingFeaturedMods = true;
		PlayFabTitleDataCache.Instance.GetTitleData(this.featuredModsPlayFabKey, new Action<string>(this.OnGetFeaturedModsTitleData), delegate(PlayFabError error)
		{
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		});
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x00114D50 File Offset: 0x00112F50
	private void OnGetFeaturedModsTitleData(string data)
	{
		CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__76 <OnGetFeaturedModsTitleData>d__;
		<OnGetFeaturedModsTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGetFeaturedModsTitleData>d__.<>4__this = this;
		<OnGetFeaturedModsTitleData>d__.data = data;
		<OnGetFeaturedModsTitleData>d__.<>1__state = -1;
		<OnGetFeaturedModsTitleData>d__.<>t__builder.Start<CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__76>(ref <OnGetFeaturedModsTitleData>d__);
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x00114D90 File Offset: 0x00112F90
	private void RetrieveAvailableMods()
	{
		if (this.loadingAvailableMods)
		{
			return;
		}
		this.loadingAvailableMods = true;
		int num = this.currentAvailableModsRequestPage;
		this.currentAvailableModsRequestPage = num + 1;
		SearchFilter searchFilter = new SearchFilter(num, this.numModsPerRequest);
		searchFilter.SetSortBy(this.sortType);
		if (!this.searchTags.IsNullOrEmpty<string>())
		{
			searchFilter.AddTags(this.searchTags);
		}
		searchFilter.SetToAscending(this.isAscendingOrder);
		ModIOUnity.GetMods(searchFilter, new Action<ResultAnd<ModPage>>(this.OnAvailableModsRetrieved));
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x00114E10 File Offset: 0x00113010
	private void OnAvailableModsRetrieved(ResultAnd<ModPage> result)
	{
		if (result.result.Succeeded())
		{
			this.totalAvailableMods = (int)result.value.totalSearchResultsFound;
			this.availableMods.AddRange(result.value.modProfiles);
			this.FilterAvailableMods();
		}
		else
		{
			this.errorLoadingAvailableMods = true;
		}
		this.loadingAvailableMods = false;
		this.RefreshScreenState();
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x00114E70 File Offset: 0x00113070
	private void FilterAvailableMods()
	{
		if (this.availableMods.IsNullOrEmpty<ModProfile>())
		{
			return;
		}
		this.filteredAvailableMods.Clear();
		if (this.searchTags.IsNullOrEmpty<string>())
		{
			this.totalAvailableMods = Mathf.Max(0, this.totalAvailableMods - 1);
		}
		foreach (ModProfile modProfile in this.availableMods)
		{
			if (!(modProfile.id == ModIOManager.GetNewMapsModId()) && (this.featuredModIds.IsNullOrEmpty<long>() || !this.featuredModIds.Contains(modProfile.id.id)))
			{
				this.filteredAvailableMods.Add(modProfile);
			}
		}
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x00114F3C File Offset: 0x0011313C
	private void RetrieveSubscribedMods()
	{
		this.subscribedMods = ModIOManager.GetSubscribedMods();
		this.FilterSubscribedMods();
		this.totalSubscribedMods = this.filteredSubscribedMods.Count;
		this.RefreshScreenState();
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x00114F68 File Offset: 0x00113168
	private void FilterSubscribedMods()
	{
		if (this.subscribedMods.IsNullOrEmpty<SubscribedMod>())
		{
			return;
		}
		this.filteredSubscribedMods.Clear();
		foreach (SubscribedMod subscribedMod in this.subscribedMods)
		{
			if (!(subscribedMod.modProfile.id == ModIOManager.GetNewMapsModId()))
			{
				this.filteredSubscribedMods.Add(subscribedMod);
			}
		}
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x00114FD0 File Offset: 0x001131D0
	public void ShowCustomModList(long[] modIds)
	{
		if (modIds == null || modIds.Length == 0)
		{
			return;
		}
		if (this.customModListModIds.Count > 0 && this.customModListModIds.Count == modIds.Length)
		{
			bool flag = true;
			foreach (long value in this.customModListModIds)
			{
				if (!modIds.Contains(value))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.currentState = CustomMapsListScreen.ListScreenState.CustomModList;
				this.currentModPage = 0;
				this.selectedModIndex = 0;
				this.RefreshScreenState();
				return;
			}
		}
		this.customModListModIds.Clear();
		this.customModList.Clear();
		this.customModListModIds.AddRange(modIds);
		if (this.loadingCustomModList)
		{
			this.restartCustomModListRetrieval = true;
		}
		this.currentState = CustomMapsListScreen.ListScreenState.CustomModList;
		this.loadingCustomModList = true;
		this.RefreshScreenState();
		this.RetrieveCustomModList();
	}

	// Token: 0x06003506 RID: 13574 RVA: 0x001150BC File Offset: 0x001132BC
	private void RetrieveCustomModList()
	{
		if (this.restartCustomModListRetrieval || this.customModListModIds.Count == 0)
		{
			return;
		}
		ModIOManager.GetModProfile(new ModId(this.customModListModIds[this.customModList.Count]), new Action<ModIORequestResultAnd<ModProfile>>(this.OnModProfileReceived));
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x0011510C File Offset: 0x0011330C
	private void OnModProfileReceived(ModIORequestResultAnd<ModProfile> requestResult)
	{
		if (this.restartCustomModListRetrieval)
		{
			this.restartCustomModListRetrieval = false;
			this.RetrieveCustomModList();
			return;
		}
		if (!requestResult.result.success)
		{
			this.loadingCustomModList = false;
			this.errorLoadingCustomModList = true;
			this.RefreshScreenState();
			return;
		}
		this.customModList.Add(requestResult.data);
		if (this.customModList.Count < this.customModListModIds.Count)
		{
			ModIOManager.GetModProfile(new ModId(this.customModListModIds[this.customModList.Count]), new Action<ModIORequestResultAnd<ModProfile>>(this.OnModProfileReceived));
			return;
		}
		this.loadingCustomModList = false;
		this.RefreshScreenState();
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x001151B4 File Offset: 0x001133B4
	public bool DoesModListMatchDisplay(long[] modIds)
	{
		if (this.displayedModProfiles.IsNullOrEmpty<ModProfile>() || this.displayedModProfiles.Count != modIds.Length)
		{
			return false;
		}
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			if (modIds[i] != this.displayedModProfiles[i].id.id)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x00115214 File Offset: 0x00113414
	public ModProfile GetProfile()
	{
		if (this.displayedModProfiles == null)
		{
			return default(ModProfile);
		}
		if (this.selectedModIndex < 0 || this.selectedModIndex >= this.displayedModProfiles.Count)
		{
			return default(ModProfile);
		}
		return this.displayedModProfiles[this.selectedModIndex];
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x0011526C File Offset: 0x0011346C
	public void GetModList(out long[] modList)
	{
		if (this.displayedModProfiles == null)
		{
			modList = Array.Empty<long>();
			return;
		}
		modList = new long[this.displayedModProfiles.Count];
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			modList[i] = this.displayedModProfiles[i].id.id;
		}
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x001152CC File Offset: 0x001134CC
	private void RefreshScreenState()
	{
		this.displayedModProfiles.Clear();
		this.modListText.text = "";
		this.modListText.gameObject.SetActive(false);
		this.modPageLabel.gameObject.SetActive(false);
		this.modPageText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.createdByText.gameObject.SetActive(false);
		this.tagText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		for (int i = 0; i < this.buttonsToShow.Length; i++)
		{
			this.buttonsToShow[i].SetActive(true);
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
		{
			this.titleText.text = this.browseModsTitle + string.Format(" (SORTED BY {0})", this.sortType);
			for (int j = 0; j < this.buttonsToHideForAvailableMaps.Length; j++)
			{
				this.buttonsToHideForAvailableMaps[j].SetActive(false);
			}
			if (this.loadingFeaturedMods || this.loadingAvailableMods)
			{
				return;
			}
			if (this.errorLoadingAvailableMods)
			{
				this.modListText.text = this.failedToRetrieveModsString;
				this.loadingText.gameObject.SetActive(false);
				this.modListText.gameObject.SetActive(true);
				return;
			}
			this.UpdatePageCount(this.totalAvailableMods);
			int num = 0;
			int num2 = this.modsPerPage - 1;
			if (this.IsOnFirstPage())
			{
				this.displayedModProfiles.AddRange(this.featuredMods);
				num2 -= this.totalFeaturedMods;
			}
			else
			{
				num = this.currentModPage * this.modsPerPage - this.totalFeaturedMods;
				num2 = num + this.modsPerPage - 1;
			}
			if (this.filteredAvailableMods.Count <= num2 && this.totalAvailableMods > this.availableMods.Count)
			{
				this.displayedModProfiles.Clear();
				this.RetrieveAvailableMods();
				return;
			}
			int num3 = num;
			while (num3 <= num2 && this.filteredAvailableMods.Count > num3)
			{
				this.displayedModProfiles.Add(this.filteredAvailableMods[num3]);
				num3++;
			}
			this.UpdateModListSelection();
			this.loadingText.gameObject.SetActive(false);
			this.modListText.gameObject.SetActive(true);
			return;
		}
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
		{
			this.titleText.text = this.subscribedOnlyTitle;
			for (int k = 0; k < this.buttonsToHideForSubscribedMaps.Length; k++)
			{
				this.buttonsToHideForSubscribedMaps[k].SetActive(false);
			}
			this.UpdatePageCount(this.totalSubscribedMods);
			int num4 = this.currentModPage * this.modsPerPage;
			int num5 = num4;
			while (num5 < num4 + this.modsPerPage && this.filteredSubscribedMods.Count > num5)
			{
				this.displayedModProfiles.Add(this.filteredSubscribedMods[num5].modProfile);
				num5++;
			}
			this.UpdateModListSelection();
			this.loadingText.gameObject.SetActive(false);
			this.modListText.gameObject.SetActive(true);
			CustomMapsTerminal.SendTerminalStatus(true, false);
			return;
		}
		case CustomMapsListScreen.ListScreenState.CustomModList:
			this.titleText.text = CustomMapsTerminal.GetDriverNickname() + "'s " + this.subscribedOnlyTitle;
			for (int l = 0; l < this.buttonsToHideForSubscribedMaps.Length; l++)
			{
				this.buttonsToHideForSubscribedMaps[l].SetActive(false);
			}
			if (this.loadingCustomModList)
			{
				return;
			}
			if (this.errorLoadingCustomModList)
			{
				this.modPageLabel.gameObject.SetActive(false);
				this.modPageText.gameObject.SetActive(false);
				if (CustomMapsTerminal.IsDriver)
				{
					this.currentModPage = -1;
				}
				this.modListText.text = this.failedToRetrieveModsString;
				this.loadingText.gameObject.SetActive(false);
				this.modListText.gameObject.SetActive(true);
				return;
			}
			this.UpdatePageCount(this.customModList.Count);
			this.displayedModProfiles.AddRange(this.customModList);
			this.UpdateModListSelection();
			this.loadingText.gameObject.SetActive(false);
			this.modListText.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x00115700 File Offset: 0x00113900
	public void UpdateModListSelection()
	{
		if (this.displayedModProfiles.IsNullOrEmpty<ModProfile>())
		{
			return;
		}
		if (this.selectedModIndex < -1 || this.selectedModIndex > this.displayedModProfiles.Count)
		{
			this.mapScreenshotImage.gameObject.SetActive(false);
			this.createdByText.gameObject.SetActive(false);
			this.tagText.gameObject.SetActive(false);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (this.selectedModIndex == -1 || this.selectedModIndex == this.displayedModProfiles.Count)
		{
			this.mapScreenshotImage.gameObject.SetActive(false);
			this.createdByText.gameObject.SetActive(false);
			this.tagText.gameObject.SetActive(false);
		}
		else
		{
			this.DownloadThumbnail(this.displayedModProfiles[this.selectedModIndex].logoImage320x180);
			this.createdByText.gameObject.SetActive(true);
			this.tagText.gameObject.SetActive(true);
			this.createdByText.text = this.creatorString + this.displayedModProfiles[this.selectedModIndex].creator.username;
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < this.displayedModProfiles[this.selectedModIndex].tags.Length; i++)
			{
				stringBuilder2.Append((i == 0) ? (this.displayedModProfiles[this.selectedModIndex].tags[i] ?? "") : (", " + this.displayedModProfiles[this.selectedModIndex].tags[i]));
			}
			this.tagText.text = this.tagString + stringBuilder2.ToString();
		}
		if (!this.IsOnFirstPage())
		{
			stringBuilder.Append((this.selectedModIndex == -1) ? ("> " + this.prevPageString + "\n") : ("  " + this.prevPageString + "\n"));
		}
		long num = (long)(this.currentModPage * this.modsPerPage + 1);
		bool flag = false;
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			if (this.IsOnFirstPage())
			{
				for (int j = 0; j < this.displayedModProfiles.Count; j++)
				{
					if (j < this.totalFeaturedMods)
					{
						stringBuilder.Append((j == this.selectedModIndex) ? this.TruncateModName("> FEATURED: " + this.displayedModProfiles[j].name) : this.TruncateModName("  FEATURED: " + this.displayedModProfiles[j].name));
					}
					else
					{
						StringBuilder stringBuilder3 = stringBuilder;
						string value;
						if (j != this.selectedModIndex)
						{
							string format = "  {0}. {1}";
							long num2 = num;
							num = num2 + 1L;
							value = this.TruncateModName(string.Format(format, num2, this.displayedModProfiles[j].name));
						}
						else
						{
							string format2 = "> {0}. {1}";
							long num3 = num;
							num = num3 + 1L;
							value = this.TruncateModName(string.Format(format2, num3, this.displayedModProfiles[j].name));
						}
						stringBuilder3.Append(value);
					}
				}
				flag = true;
			}
			else
			{
				num -= (long)this.totalFeaturedMods;
			}
		}
		if (!flag)
		{
			for (int k = 0; k < this.displayedModProfiles.Count; k++)
			{
				stringBuilder.Append((k == this.selectedModIndex) ? this.TruncateModName(string.Format("> {0}. {1}", num + (long)k, this.displayedModProfiles[k].name)) : this.TruncateModName(string.Format("  {0}. {1}", num + (long)k, this.displayedModProfiles[k].name)));
			}
		}
		if (!this.IsOnLastPage())
		{
			stringBuilder.Append((this.selectedModIndex == this.displayedModProfiles.Count) ? ("> " + this.nextPageString + "\n") : ("  " + this.nextPageString + "\n"));
		}
		this.modListText.text = stringBuilder.ToString();
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x00115B24 File Offset: 0x00113D24
	private string TruncateModName(string modname)
	{
		if (modname.Length <= this.maxModListItemLength)
		{
			return modname + "\n";
		}
		return modname.Substring(0, this.maxModListItemLength) + "\n";
	}

	// Token: 0x0600350E RID: 13582 RVA: 0x00115B58 File Offset: 0x00113D58
	private void DownloadThumbnail(DownloadReference thumbnail)
	{
		this.mapScreenshotImage.gameObject.SetActive(false);
		if (this.isDownloadingThumbnail)
		{
			this.newDownloadRequest = true;
			this.currentThumbnail = thumbnail;
			return;
		}
		this.isDownloadingThumbnail = true;
		ModIOUnity.DownloadTexture(thumbnail, new Action<ResultAnd<Texture2D>>(this.OnTextureDownloaded));
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x00115BA8 File Offset: 0x00113DA8
	private void OnTextureDownloaded(ResultAnd<Texture2D> resultAnd)
	{
		this.isDownloadingThumbnail = false;
		if (this.newDownloadRequest)
		{
			this.newDownloadRequest = false;
			this.mapScreenshotImage.gameObject.SetActive(false);
			ModIOUnity.DownloadTexture(this.currentThumbnail, new Action<ResultAnd<Texture2D>>(this.OnTextureDownloaded));
			return;
		}
		if (!resultAnd.result.Succeeded())
		{
			return;
		}
		Texture2D value = resultAnd.value;
		this.mapScreenshotImage.sprite = Sprite.Create(value, new Rect(0f, 0f, (float)value.width, (float)value.height), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x00115C58 File Offset: 0x00113E58
	private void UpdatePageCount(int totalMods)
	{
		this.totalModCount = totalMods;
		this.modPageText.gameObject.SetActive(false);
		this.modPageLabel.gameObject.SetActive(false);
		if (this.totalModCount == 0)
		{
			this.modListText.text = ((this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods) ? this.noModsAvailableString : this.noSubscribedModsString);
			return;
		}
		int numPages = this.GetNumPages();
		if (numPages > 1)
		{
			this.modPageText.text = string.Format("{0} / {1}", this.currentModPage + 1, numPages);
			this.modPageText.gameObject.SetActive(true);
			this.modPageLabel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003511 RID: 13585 RVA: 0x00115D10 File Offset: 0x00113F10
	public int GetNumPages()
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			return this.customModListPageCount;
		}
		int num = this.totalModCount % this.modsPerPage;
		int num2 = this.totalModCount / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x00115D50 File Offset: 0x00113F50
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x00115D5C File Offset: 0x00113F5C
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x06003514 RID: 13588 RVA: 0x00115D80 File Offset: 0x00113F80
	public void UpdateFromTerminalStatus(CustomMapsTerminal.TerminalStatus localStatus)
	{
		switch (localStatus.currentScreen)
		{
		case CustomMapsTerminal.ScreenType.AvailableMods:
			this.currentState = CustomMapsListScreen.ListScreenState.AvailableMods;
			break;
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList && CustomMapsTerminal.IsDriver)
			{
				this.currentState = CustomMapsListScreen.ListScreenState.SubscribedMods;
				this.currentModPage = 0;
				this.selectedModIndex = 0;
				this.customModListPageCount = -1;
				return;
			}
			this.currentState = (CustomMapsTerminal.IsDriver ? CustomMapsListScreen.ListScreenState.SubscribedMods : CustomMapsListScreen.ListScreenState.CustomModList);
			break;
		}
		this.currentModPage = localStatus.pageIndex;
		this.selectedModIndex = localStatus.modIndex;
		this.customModListPageCount = localStatus.numModPages;
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x00115E21 File Offset: 0x00114021
	public void RefreshDriverNickname(string driverNickname)
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			this.titleText.text = driverNickname + "'s " + this.subscribedOnlyTitle;
		}
	}

	// Token: 0x040041D7 RID: 16855
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040041D8 RID: 16856
	[SerializeField]
	private TMP_Text modListText;

	// Token: 0x040041D9 RID: 16857
	[SerializeField]
	private TMP_Text modPageLabel;

	// Token: 0x040041DA RID: 16858
	[SerializeField]
	private TMP_Text modPageText;

	// Token: 0x040041DB RID: 16859
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x040041DC RID: 16860
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x040041DD RID: 16861
	[SerializeField]
	private TMP_Text createdByText;

	// Token: 0x040041DE RID: 16862
	[SerializeField]
	private TMP_Text tagText;

	// Token: 0x040041DF RID: 16863
	[SerializeField]
	private GameObject[] buttonsToHideForAvailableMaps;

	// Token: 0x040041E0 RID: 16864
	[SerializeField]
	private GameObject[] buttonsToShow;

	// Token: 0x040041E1 RID: 16865
	[SerializeField]
	private GameObject[] buttonsToHideForSubscribedMaps;

	// Token: 0x040041E2 RID: 16866
	[SerializeField]
	private string browseModsTitle = "AVAILABLE MODS";

	// Token: 0x040041E3 RID: 16867
	[SerializeField]
	private string subscribedOnlyTitle = "SUBSCRIBED MODS";

	// Token: 0x040041E4 RID: 16868
	[SerializeField]
	private string nextPageString = "NEXT PAGE";

	// Token: 0x040041E5 RID: 16869
	[SerializeField]
	private string prevPageString = "PREVIOUS PAGE";

	// Token: 0x040041E6 RID: 16870
	[SerializeField]
	private string noModsAvailableString = "NO MODS AVAILABLE";

	// Token: 0x040041E7 RID: 16871
	[SerializeField]
	private string noSubscribedModsString = "NOT SUBSCRIBED TO ANY MODS";

	// Token: 0x040041E8 RID: 16872
	[SerializeField]
	private string failedToRetrieveModsString = "FAILED TO RETRIEVE MODS FROM MOD.IO \nPRESS THE 'REFRESH' BUTTON TO RETRY";

	// Token: 0x040041E9 RID: 16873
	[SerializeField]
	private string creatorString = "CREATED BY:\n";

	// Token: 0x040041EA RID: 16874
	[SerializeField]
	private string tagString = "MAP TAGS:\n";

	// Token: 0x040041EB RID: 16875
	[SerializeField]
	private int modsPerPage = 10;

	// Token: 0x040041EC RID: 16876
	[SerializeField]
	private int numModsPerRequest = 50;

	// Token: 0x040041ED RID: 16877
	[SerializeField]
	private int maxModListItemLength = 25;

	// Token: 0x040041EE RID: 16878
	[SerializeField]
	private string featuredModsPlayFabKey = "VStumpFeaturedMaps";

	// Token: 0x040041EF RID: 16879
	private bool loadingFeaturedMods;

	// Token: 0x040041F0 RID: 16880
	private int totalFeaturedMods;

	// Token: 0x040041F1 RID: 16881
	private List<long> featuredModIds = new List<long>();

	// Token: 0x040041F2 RID: 16882
	private List<ModProfile> featuredMods = new List<ModProfile>();

	// Token: 0x040041F3 RID: 16883
	private int currentAvailableModsRequestPage;

	// Token: 0x040041F4 RID: 16884
	private bool loadingAvailableMods;

	// Token: 0x040041F5 RID: 16885
	private int totalAvailableMods;

	// Token: 0x040041F6 RID: 16886
	private bool errorLoadingAvailableMods;

	// Token: 0x040041F7 RID: 16887
	private List<ModProfile> availableMods = new List<ModProfile>();

	// Token: 0x040041F8 RID: 16888
	private List<ModProfile> filteredAvailableMods = new List<ModProfile>();

	// Token: 0x040041F9 RID: 16889
	private int totalSubscribedMods;

	// Token: 0x040041FA RID: 16890
	private SubscribedMod[] subscribedMods;

	// Token: 0x040041FB RID: 16891
	private List<SubscribedMod> filteredSubscribedMods = new List<SubscribedMod>();

	// Token: 0x040041FC RID: 16892
	private bool loadingCustomModList;

	// Token: 0x040041FD RID: 16893
	private bool errorLoadingCustomModList;

	// Token: 0x040041FE RID: 16894
	private int customModListPageCount = -1;

	// Token: 0x040041FF RID: 16895
	private List<long> customModListModIds = new List<long>();

	// Token: 0x04004200 RID: 16896
	private List<ModProfile> customModList = new List<ModProfile>();

	// Token: 0x04004201 RID: 16897
	private int selectedModIndex;

	// Token: 0x04004202 RID: 16898
	private int currentModPage;

	// Token: 0x04004203 RID: 16899
	private int totalModCount;

	// Token: 0x04004204 RID: 16900
	private List<ModProfile> displayedModProfiles = new List<ModProfile>();

	// Token: 0x04004205 RID: 16901
	private int sortTypeIndex;

	// Token: 0x04004206 RID: 16902
	private SortModsBy sortType = SortModsBy.Popular;

	// Token: 0x04004207 RID: 16903
	private const int MAX_SORT_TYPES = 6;

	// Token: 0x04004208 RID: 16904
	private List<string> searchTags = new List<string>();

	// Token: 0x04004209 RID: 16905
	private bool isAscendingOrder = true;

	// Token: 0x0400420A RID: 16906
	private bool isDownloadingThumbnail;

	// Token: 0x0400420B RID: 16907
	private bool newDownloadRequest;

	// Token: 0x0400420C RID: 16908
	private DownloadReference currentThumbnail;

	// Token: 0x0400420D RID: 16909
	private bool restartCustomModListRetrieval;

	// Token: 0x0400420E RID: 16910
	public CustomMapsListScreen.ListScreenState currentState;

	// Token: 0x02000844 RID: 2116
	public enum ListScreenState
	{
		// Token: 0x04004210 RID: 16912
		AvailableMods,
		// Token: 0x04004211 RID: 16913
		SubscribedMods,
		// Token: 0x04004212 RID: 16914
		CustomModList
	}
}
