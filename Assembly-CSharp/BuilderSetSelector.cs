using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200055D RID: 1373
public class BuilderSetSelector : MonoBehaviour
{
	// Token: 0x0600217B RID: 8571 RVA: 0x000B59BC File Offset: 0x000B3BBC
	private void Start()
	{
		this.zoneRenderers.Clear();
		foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
		{
			this.zoneRenderers.Add(gorillaPressableButton.buttonRenderer);
			TMP_Text myTmpText = gorillaPressableButton.myTmpText;
			Renderer renderer = (myTmpText != null) ? myTmpText.GetComponent<Renderer>() : null;
			if (renderer != null)
			{
				this.zoneRenderers.Add(renderer);
			}
		}
		this.zoneRenderers.Add(this.previousPageButton.buttonRenderer);
		this.zoneRenderers.Add(this.nextPageButton.buttonRenderer);
		TMP_Text myTmpText2 = this.previousPageButton.myTmpText;
		Renderer renderer2 = (myTmpText2 != null) ? myTmpText2.GetComponent<Renderer>() : null;
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		TMP_Text myTmpText3 = this.nextPageButton.myTmpText;
		renderer2 = ((myTmpText3 != null) ? myTmpText3.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		this.inBuilderZone = true;
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000B5AE4 File Offset: 0x000B3CE4
	public void Setup(List<BuilderPieceSet.BuilderPieceCategory> categories)
	{
		List<BuilderPieceSet> livePieceSets = BuilderSetManager.instance.GetLivePieceSets();
		this.numLivePieceSets = livePieceSets.Count;
		this.pieceSets = new List<BuilderPieceSet>(livePieceSets.Count);
		this._includedCategories = categories;
		foreach (BuilderPieceSet builderPieceSet in livePieceSets)
		{
			if (!builderPieceSet.setName.Equals("HIDDEN") && this.DoesSetHaveIncludedCategories(builderPieceSet))
			{
				this.pieceSets.Add(builderPieceSet);
			}
		}
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedSets));
		BuilderSetManager.instance.OnLiveSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedSets));
		this.setsPerPage = this.setButtons.Length;
		this.totalPages = this.pieceSets.Count / this.setsPerPage;
		if (this.pieceSets.Count % this.setsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.pageIndex = 0;
		this.currentSet = this.pieceSets[this.setIndex];
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.setButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += this.OnSetButtonPressed;
		}
		this.UpdateLabels();
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000B5CF4 File Offset: 0x000B3EF4
	private void OnDestroy()
	{
		if (this.previousPageButton != null)
		{
			this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
		}
		if (this.nextPageButton != null)
		{
			this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedSets));
			BuilderSetManager.instance.OnLiveSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedSets));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnSetButtonPressed;
			}
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000B5E08 File Offset: 0x000B4008
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Renderer renderer = enumerator.Current;
					renderer.enabled = true;
				}
				goto IL_8B;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			foreach (Renderer renderer2 in this.zoneRenderers)
			{
				renderer2.enabled = false;
			}
		}
		IL_8B:
		this.inBuilderZone = flag;
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000B5EC4 File Offset: 0x000B40C4
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		int num = 0;
		for (int i = 0; i < this.setButtons.Length; i++)
		{
			if (button.Equals(this.setButtons[i]))
			{
				num = i;
				break;
			}
		}
		int num2 = this.pageIndex * this.setsPerPage + num;
		if (num2 < this.pieceSets.Count)
		{
			BuilderPieceSet builderPieceSet = this.pieceSets[num2];
			if (this.currentSet == null || builderPieceSet.setName != this.currentSet.setName)
			{
				UnityEvent<int> onSelectedSet = this.OnSelectedSet;
				if (onSelectedSet == null)
				{
					return;
				}
				onSelectedSet.Invoke(builderPieceSet.GetIntIdentifier());
			}
		}
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000B5F64 File Offset: 0x000B4164
	private void RefreshUnlockedSets()
	{
		List<BuilderPieceSet> livePieceSets = BuilderSetManager.instance.GetLivePieceSets();
		if (livePieceSets.Count != this.numLivePieceSets)
		{
			string value = (this.currentSet != null) ? this.currentSet.setName : "";
			this.numLivePieceSets = livePieceSets.Count;
			this.pieceSets.EnsureCapacity(this.numLivePieceSets);
			this.pieceSets.Clear();
			int num = 0;
			foreach (BuilderPieceSet builderPieceSet in livePieceSets)
			{
				if (!builderPieceSet.setName.Equals("HIDDEN") && this.DoesSetHaveIncludedCategories(builderPieceSet))
				{
					if (builderPieceSet.setName.Equals(value))
					{
						num = this.pieceSets.Count;
					}
					this.pieceSets.Add(builderPieceSet);
				}
			}
			if (this.pieceSets.Count < 1)
			{
				this.currentSet = null;
			}
			else
			{
				this.setIndex = num;
				this.currentSet = this.pieceSets[this.setIndex];
			}
			this.totalPages = this.pieceSets.Count / this.setsPerPage;
			if (this.pieceSets.Count % this.setsPerPage > 0)
			{
				this.totalPages++;
			}
			this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
			this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
			this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
			this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		}
		this.UpdateLabels();
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x000B6134 File Offset: 0x000B4334
	private void OnPreviousPageClicked()
	{
		this.RefreshUnlockedSets();
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000B6174 File Offset: 0x000B4374
	private void OnNextPageClicked()
	{
		this.RefreshUnlockedSets();
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000B61B4 File Offset: 0x000B43B4
	public void SetSelection(int setID)
	{
		if (BuilderSetManager.instance == null)
		{
			return;
		}
		BuilderPieceSet pieceSetFromID = BuilderSetManager.instance.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return;
		}
		this.currentSet = pieceSetFromID;
		this.UpdateLabels();
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000B61F8 File Offset: 0x000B43F8
	private void UpdateLabels()
	{
		for (int i = 0; i < this.setLabels.Length; i++)
		{
			int num = this.pageIndex * this.setsPerPage + i;
			if (num < this.pieceSets.Count && this.pieceSets[num] != null)
			{
				if (!this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(true);
					this.setButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.setButtons[i].myTmpText.text != this.pieceSets[num].setName.ToUpper())
				{
					this.setButtons[i].myTmpText.text = this.pieceSets[num].setName.ToUpper();
				}
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.pieceSets[num].GetIntIdentifier()))
				{
					bool flag = this.currentSet != null && this.pieceSets[num].setName == this.currentSet.setName;
					if (flag != this.setButtons[i].isOn || !this.setButtons[i].enabled)
					{
						this.setButtons[i].isOn = flag;
						this.setButtons[i].buttonRenderer.material = (flag ? this.setButtons[i].pressedMaterial : this.setButtons[i].unpressedMaterial);
					}
					this.setButtons[i].enabled = true;
				}
				else
				{
					if (this.setButtons[i].enabled)
					{
						this.setButtons[i].buttonRenderer.material = this.disabledMaterial;
					}
					this.setButtons[i].enabled = false;
				}
			}
			else
			{
				if (this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(false);
					this.setButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.setButtons[i].isOn || this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = false;
					this.setButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000B64E4 File Offset: 0x000B46E4
	public bool DoesSetHaveIncludedCategories(BuilderPieceSet set)
	{
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in set.subsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000B654C File Offset: 0x000B474C
	public BuilderPieceSet GetSelectedSet()
	{
		return this.currentSet;
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000B6554 File Offset: 0x000B4754
	public int GetDefaultSetID()
	{
		if (this.pieceSets == null || this.pieceSets.Count < 1)
		{
			return -1;
		}
		BuilderPieceSet builderPieceSet = this.pieceSets[0];
		if (!BuilderSetManager.instance.IsPieceSetOwnedLocally(builderPieceSet.GetIntIdentifier()))
		{
			foreach (BuilderPieceSet builderPieceSet2 in this.pieceSets)
			{
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(builderPieceSet2.GetIntIdentifier()))
				{
					return builderPieceSet2.GetIntIdentifier();
				}
			}
			Debug.LogWarning("No default set available for shelf");
			return -1;
		}
		return builderPieceSet.GetIntIdentifier();
	}

	// Token: 0x04002ACF RID: 10959
	private List<BuilderPieceSet> pieceSets;

	// Token: 0x04002AD0 RID: 10960
	private int numLivePieceSets;

	// Token: 0x04002AD1 RID: 10961
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x04002AD2 RID: 10962
	[Header("UI")]
	[SerializeField]
	private Text[] setLabels;

	// Token: 0x04002AD3 RID: 10963
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableButton[] setButtons;

	// Token: 0x04002AD4 RID: 10964
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x04002AD5 RID: 10965
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x04002AD6 RID: 10966
	private List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x04002AD7 RID: 10967
	private int setIndex;

	// Token: 0x04002AD8 RID: 10968
	private BuilderPieceSet currentSet;

	// Token: 0x04002AD9 RID: 10969
	private int pageIndex;

	// Token: 0x04002ADA RID: 10970
	private int setsPerPage = 3;

	// Token: 0x04002ADB RID: 10971
	private int totalPages = 1;

	// Token: 0x04002ADC RID: 10972
	private List<Renderer> zoneRenderers = new List<Renderer>(10);

	// Token: 0x04002ADD RID: 10973
	private bool inBuilderZone;

	// Token: 0x04002ADE RID: 10974
	[HideInInspector]
	public UnityEvent<int> OnSelectedSet;
}
