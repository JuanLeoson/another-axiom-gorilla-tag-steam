using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020003E0 RID: 992
public class SlingshotLifeIndicator : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x17000284 RID: 644
	// (get) Token: 0x06001737 RID: 5943 RVA: 0x0007DCCC File Offset: 0x0007BECC
	// (set) Token: 0x06001738 RID: 5944 RVA: 0x0007DCD4 File Offset: 0x0007BED4
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x06001739 RID: 5945 RVA: 0x0007DCDD File Offset: 0x0007BEDD
	// (set) Token: 0x0600173A RID: 5946 RVA: 0x0007DCE5 File Offset: 0x0007BEE5
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x0600173B RID: 5947 RVA: 0x0007DCEE File Offset: 0x0007BEEE
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x000023F5 File Offset: 0x000005F5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x0007DCF7 File Offset: 0x0007BEF7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x0007DD1B File Offset: 0x0007BF1B
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.Reset();
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x0007DD45 File Offset: 0x0007BF45
	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(false);
		}
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x0007DD70 File Offset: 0x0007BF70
	public void SliceUpdate()
	{
		if (!NetworkSystem.Instance.InRoom || (this.checkedBattle && !this.inBattle))
		{
			if (this.indicator1.activeSelf)
			{
				this.indicator1.SetActive(false);
			}
			if (this.indicator2.activeSelf)
			{
				this.indicator2.SetActive(false);
			}
			if (this.indicator3.activeSelf)
			{
				this.indicator3.SetActive(false);
			}
			return;
		}
		if (this.bMgr == null)
		{
			this.checkedBattle = true;
			this.inBattle = true;
			if (GorillaGameManager.instance == null)
			{
				return;
			}
			this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (this.bMgr == null)
			{
				this.inBattle = false;
				return;
			}
		}
		VRRig vrrig = this.myRig;
		if (((vrrig != null) ? vrrig.creator : null) == null)
		{
			return;
		}
		int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
		this.SetActive(this.indicator1, playerLives >= 1);
		this.SetActive(this.indicator2, playerLives >= 2);
		this.SetActive(this.indicator3, playerLives >= 3);
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x0007DEA5 File Offset: 0x0007C0A5
	public void OnLeftRoom()
	{
		this.Reset();
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x0007DEAD File Offset: 0x0007C0AD
	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	// Token: 0x04001F1F RID: 7967
	private VRRig myRig;

	// Token: 0x04001F20 RID: 7968
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x04001F21 RID: 7969
	public bool checkedBattle;

	// Token: 0x04001F22 RID: 7970
	public bool inBattle;

	// Token: 0x04001F23 RID: 7971
	public GameObject indicator1;

	// Token: 0x04001F24 RID: 7972
	public GameObject indicator2;

	// Token: 0x04001F25 RID: 7973
	public GameObject indicator3;
}
