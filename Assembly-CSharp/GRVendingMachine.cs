using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000698 RID: 1688
public class GRVendingMachine : MonoBehaviour
{
	// Token: 0x06002956 RID: 10582 RVA: 0x000DE6CC File Offset: 0x000DC8CC
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000DE6D5 File Offset: 0x000DC8D5
	public Transform GetSpawnMarker()
	{
		return this.itemSpawnLocation;
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000DE6DD File Offset: 0x000DC8DD
	public void NavButtonPressedLeft()
	{
		this.hIndex = Mathf.Max(0, this.hIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000DE6F9 File Offset: 0x000DC8F9
	public void NavButtonPressedRight()
	{
		this.hIndex = Mathf.Min(this.hIndex + 1, this.horizontalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x000DE71C File Offset: 0x000DC91C
	public void NavButtonPressedUp()
	{
		this.vIndex = Mathf.Max(0, this.vIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000DE738 File Offset: 0x000DC938
	public void NavButtonPressedDown()
	{
		this.vIndex = Mathf.Min(this.vIndex + 1, this.verticalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000DE75C File Offset: 0x000DC95C
	public void RequestPurchase()
	{
		if (!this.currentlyVending)
		{
			int num = this.vIndex * this.horizontalSteps + this.hIndex;
			if (num >= 0 && num < this.vendingEntries.Count)
			{
				this.vendingIndex = num;
				if (this.vendingCoroutine != null)
				{
					base.StopCoroutine(this.vendingCoroutine);
				}
				this.vendingCoroutine = base.StartCoroutine(this.VendingCoroutine());
			}
		}
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000DE7C8 File Offset: 0x000DC9C8
	private void RefreshCardReaderDisplay()
	{
		int num = this.vIndex * this.horizontalSteps + this.hIndex;
		if (num >= 0 && num < this.vendingEntries.Count)
		{
			int entityTypeId = this.vendingEntries[num].GetEntityTypeId();
			int itemCost = this.reactor.GetItemCost(entityTypeId);
			this.cardDisplayText.text = this.vendingEntries[num].itemName + "\n" + itemCost.ToString();
		}
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000DE84B File Offset: 0x000DCA4B
	private void Update()
	{
		if (!this.currentlyVending)
		{
			this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime);
		}
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000DE888 File Offset: 0x000DCA88
	private bool MoveTransportToSlot(int x, int y, int rows, int cols, float xSpeed, float ySpeed, float dt)
	{
		Vector3 vector = Vector3.Lerp(this.horizontalMin.position, this.horizontalMax.position, (float)x / (float)(rows - 1));
		Vector3 vector2 = Vector3.Lerp(this.verticalMin.position, this.verticalMax.position, (float)y / (float)(cols - 1));
		this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, vector, xSpeed * dt);
		this.verticalTransport.position = Vector3.MoveTowards(this.verticalTransport.position, vector2, ySpeed * dt);
		float sqrMagnitude = (this.horizontalTransport.position - vector).sqrMagnitude;
		float sqrMagnitude2 = (this.verticalTransport.position - vector2).sqrMagnitude;
		return sqrMagnitude > 0.001f || sqrMagnitude2 > 0.001f;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000DE962 File Offset: 0x000DCB62
	private IEnumerator VendingCoroutine()
	{
		this.currentlyVending = true;
		while (this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
		{
			yield return null;
		}
		int entityTypeId = this.vendingEntries[this.vendingIndex].GetEntityTypeId();
		int itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugUnlimitedPurchasing || VRRig.LocalRig.GetComponent<GRPlayer>().currency >= itemCost)
		{
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(true);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
			float depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
			while (depositPosSqDist > 0.001f)
			{
				this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, this.depositLocation.position, this.horizontalSpeed * Time.deltaTime);
				depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
				yield return null;
			}
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(false);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
		}
		this.currentlyVending = false;
		yield break;
	}

	// Token: 0x04003555 RID: 13653
	[SerializeField]
	private Transform horizontalTransport;

	// Token: 0x04003556 RID: 13654
	[SerializeField]
	private Transform verticalTransport;

	// Token: 0x04003557 RID: 13655
	[SerializeField]
	private Transform horizontalMin;

	// Token: 0x04003558 RID: 13656
	[SerializeField]
	private Transform horizontalMax;

	// Token: 0x04003559 RID: 13657
	[SerializeField]
	private Transform verticalMin;

	// Token: 0x0400355A RID: 13658
	[SerializeField]
	private Transform verticalMax;

	// Token: 0x0400355B RID: 13659
	[SerializeField]
	private Transform depositLocation;

	// Token: 0x0400355C RID: 13660
	[SerializeField]
	private Transform itemSpawnLocation;

	// Token: 0x0400355D RID: 13661
	[SerializeField]
	private TMP_Text cardDisplayText;

	// Token: 0x0400355E RID: 13662
	[SerializeField]
	private int horizontalSteps = 4;

	// Token: 0x0400355F RID: 13663
	[SerializeField]
	private int verticalSteps = 3;

	// Token: 0x04003560 RID: 13664
	[SerializeField]
	private float horizontalSpeed = 0.25f;

	// Token: 0x04003561 RID: 13665
	[SerializeField]
	private float verticalSpeed = 0.25f;

	// Token: 0x04003562 RID: 13666
	[SerializeField]
	private bool debugUnlimitedPurchasing;

	// Token: 0x04003563 RID: 13667
	[SerializeField]
	private List<GRVendingMachine.VendingEntry> vendingEntries = new List<GRVendingMachine.VendingEntry>();

	// Token: 0x04003564 RID: 13668
	private int hIndex;

	// Token: 0x04003565 RID: 13669
	private int vIndex;

	// Token: 0x04003566 RID: 13670
	private bool currentlyVending;

	// Token: 0x04003567 RID: 13671
	private int vendingIndex;

	// Token: 0x04003568 RID: 13672
	private Coroutine vendingCoroutine;

	// Token: 0x04003569 RID: 13673
	public int VendingMachineId;

	// Token: 0x0400356A RID: 13674
	private GhostReactor reactor;

	// Token: 0x02000699 RID: 1689
	[Serializable]
	public struct VendingEntry
	{
		// Token: 0x06002962 RID: 10594 RVA: 0x000DE9A8 File Offset: 0x000DCBA8
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x0400356B RID: 13675
		public Transform transportVisual;

		// Token: 0x0400356C RID: 13676
		public GameEntity entityPrefab;

		// Token: 0x0400356D RID: 13677
		public string itemName;

		// Token: 0x0400356E RID: 13678
		private int entityTypeId;

		// Token: 0x0400356F RID: 13679
		private bool entityTypeIdSet;
	}
}
