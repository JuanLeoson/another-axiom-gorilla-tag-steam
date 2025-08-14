using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class RotatingQuestBadge : MonoBehaviour, ISpawnable
{
	// Token: 0x170000CE RID: 206
	// (get) Token: 0x0600088D RID: 2189 RVA: 0x0002ECAF File Offset: 0x0002CEAF
	// (set) Token: 0x0600088E RID: 2190 RVA: 0x0002ECB7 File Offset: 0x0002CEB7
	public bool IsSpawned { get; set; }

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x0600088F RID: 2191 RVA: 0x0002ECC0 File Offset: 0x0002CEC0
	// (set) Token: 0x06000890 RID: 2192 RVA: 0x0002ECC8 File Offset: 0x0002CEC8
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000891 RID: 2193 RVA: 0x0002ECD4 File Offset: 0x0002CED4
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
		this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002ED2D File Offset: 0x0002CF2D
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.SetBadgeLevel(-1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002ED53 File Offset: 0x0002CF53
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnQuestScoreChanged -= this.OnProgressScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0002ED88 File Offset: 0x0002CF88
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0002ED98 File Offset: 0x0002CF98
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
			this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
			return true;
		}
		return false;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0002EDF4 File Offset: 0x0002CFF4
	private void OnProgressScoreChanged(int score)
	{
		score = Mathf.Clamp(score, 0, 99999);
		this.displayField.text = score.ToString();
		this.UpdateBadge(score);
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0002EE20 File Offset: 0x0002D020
	private void UpdateBadge(int score)
	{
		int num = -1;
		int badgeLevel = -1;
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			if (this.badgeLevels[i].requiredPoints <= score && this.badgeLevels[i].requiredPoints > num)
			{
				num = this.badgeLevels[i].requiredPoints;
				badgeLevel = i;
			}
		}
		this.SetBadgeLevel(badgeLevel);
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0002EE88 File Offset: 0x0002D088
	private void SetBadgeLevel(int level)
	{
		level = Mathf.Clamp(level, 0, this.badgeLevels.Length - 1);
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			this.badgeLevels[i].badge.SetActive(i == level);
		}
	}

	// Token: 0x04000A29 RID: 2601
	[SerializeField]
	private TextMeshPro displayField;

	// Token: 0x04000A2A RID: 2602
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x04000A2B RID: 2603
	[SerializeField]
	private VRRig myRig;

	// Token: 0x04000A2C RID: 2604
	[SerializeField]
	private RotatingQuestBadge.BadgeLevel[] badgeLevels;

	// Token: 0x02000148 RID: 328
	[Serializable]
	public struct BadgeLevel
	{
		// Token: 0x04000A2F RID: 2607
		public GameObject badge;

		// Token: 0x04000A30 RID: 2608
		public int requiredPoints;
	}
}
