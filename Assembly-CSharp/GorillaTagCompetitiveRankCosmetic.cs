using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000703 RID: 1795
public class GorillaTagCompetitiveRankCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x06002D03 RID: 11523 RVA: 0x000ED806 File Offset: 0x000EBA06
	// (set) Token: 0x06002D04 RID: 11524 RVA: 0x000ED80E File Offset: 0x000EBA0E
	public bool IsSpawned { get; set; }

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x06002D05 RID: 11525 RVA: 0x000ED817 File Offset: 0x000EBA17
	// (set) Token: 0x06002D06 RID: 11526 RVA: 0x000ED81F File Offset: 0x000EBA1F
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06002D07 RID: 11527 RVA: 0x000ED828 File Offset: 0x000EBA28
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnRankedSubtierChanged += this.OnRankedScoreChanged;
		this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x000ED88E File Offset: 0x000EBA8E
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.UpdateDisplayedCosmetic(-1, -1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x000ED8B5 File Offset: 0x000EBAB5
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnRankedSubtierChanged -= this.OnRankedScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x000ED8EA File Offset: 0x000EBAEA
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x000ED8FC File Offset: 0x000EBAFC
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnRankedSubtierChanged += this.OnRankedScoreChanged;
			this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
			return true;
		}
		return false;
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x000ED965 File Offset: 0x000EBB65
	private void OnRankedScoreChanged(int questRank, int pcRank)
	{
		this.UpdateDisplayedCosmetic(questRank, pcRank);
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x000ED970 File Offset: 0x000EBB70
	private void UpdateDisplayedCosmetic(int questRank, int pcRank)
	{
		if (this.rankCosmetics == null)
		{
			return;
		}
		int num = this.usePCELO ? pcRank : questRank;
		if (num <= 0)
		{
			num = 0;
		}
		for (int i = 0; i < this.rankCosmetics.Length; i++)
		{
			this.rankCosmetics[i].SetActive(i == num);
		}
	}

	// Token: 0x04003850 RID: 14416
	[Tooltip("If enabled, display PC rank. Otherwise, display Quest rank")]
	[SerializeField]
	private bool usePCELO;

	// Token: 0x04003851 RID: 14417
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x04003852 RID: 14418
	[SerializeField]
	private VRRig myRig;

	// Token: 0x04003853 RID: 14419
	[SerializeField]
	private GameObject[] rankCosmetics;
}
