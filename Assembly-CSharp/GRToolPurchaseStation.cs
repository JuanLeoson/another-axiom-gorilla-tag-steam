using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000682 RID: 1666
public class GRToolPurchaseStation : MonoBehaviour
{
	// Token: 0x170003BF RID: 959
	// (get) Token: 0x060028CE RID: 10446 RVA: 0x000DB704 File Offset: 0x000D9904
	public int ActiveEntryIndex
	{
		get
		{
			return this.activeEntryIndex;
		}
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x000DB70C File Offset: 0x000D990C
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x000DB71C File Offset: 0x000D991C
	public void RequestPurchaseButton(int actorNumber)
	{
		if (actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
		}
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000DB742 File Offset: 0x000D9942
	public void ShiftRightButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftRight);
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000DB756 File Offset: 0x000D9956
	public void ShiftLeftButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftLeft);
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x000DB76A File Offset: 0x000D996A
	public void ShiftRightAuthority()
	{
		this.activeEntryIndex = (this.activeEntryIndex + 1) % this.toolEntries.Count;
	}

	// Token: 0x060028D4 RID: 10452 RVA: 0x000DB786 File Offset: 0x000D9986
	public void ShiftLeftAuthority()
	{
		this.activeEntryIndex = ((this.activeEntryIndex > 0) ? (this.activeEntryIndex - 1) : (this.toolEntries.Count - 1));
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x000DB7B0 File Offset: 0x000D99B0
	public void DebugPurchase()
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
		Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
		Quaternion rotation = this.depositTransform.rotation * localRotation;
		Vector3 position = this.depositTransform.position + this.depositTransform.rotation * localPosition;
		this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, position, rotation, 0L);
		this.OnPurchaseSucceeded();
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x000DB870 File Offset: 0x000D9A70
	public bool TryPurchaseAuthority(GRPlayer player, out int itemCost)
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugIgnoreToolCost || player.currency >= itemCost)
		{
			Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
			Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
			Quaternion rotation = this.depositTransform.rotation * localRotation;
			Vector3 position = this.depositTransform.position + this.depositTransform.rotation * localPosition;
			this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, position, rotation, 0L);
			return true;
		}
		return false;
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000DB950 File Offset: 0x000D9B50
	public void OnSelectionUpdate(int newSelectedIndex)
	{
		this.activeEntryIndex = Mathf.Clamp(newSelectedIndex % this.toolEntries.Count, 0, this.toolEntries.Count - 1);
		this.audioSource.PlayOneShot(this.nextItemAudio, this.nextItemVolume);
		this.displayItemNameText.text = this.toolEntries[this.activeEntryIndex].toolName;
		this.displayItemCostText.text = this.toolEntries[this.activeEntryIndex].toolCost.ToString();
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000DB9E4 File Offset: 0x000D9BE4
	public void OnPurchaseSucceeded()
	{
		this.animatingDeposit = true;
		this.animationStartTime = Time.time;
		this.audioSource.PlayOneShot(this.purchaseAudio, this.purchaseVolume);
		UnityEvent onSucceeded = this.idCardScanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		if (this.displayedEntryIndex < 0 || this.displayedEntryIndex >= this.toolEntries.Count)
		{
			this.displayedEntryIndex = this.activeEntryIndex;
		}
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000DBA58 File Offset: 0x000D9C58
	public void OnPurchaseFailed()
	{
		this.audioSource.PlayOneShot(this.purchaseFailedAudio, this.purchaseFailedVolume);
		UnityEvent onFailed = this.idCardScanner.onFailed;
		if (onFailed == null)
		{
			return;
		}
		onFailed.Invoke();
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x000DBA86 File Offset: 0x000D9C86
	public Transform GetSpawnMarker()
	{
		return this.toolSpawnLocation;
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x000DBA8E File Offset: 0x000D9C8E
	public string GetCurrentToolName()
	{
		return this.toolEntries[this.activeEntryIndex].toolName;
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x000DBAA6 File Offset: 0x000D9CA6
	private void Awake()
	{
		this.depositLidOpenRot = Quaternion.Euler(this.depositLidOpenEuler);
		this.toolEntryRot = Quaternion.Euler(this.toolEntryRotEuler);
		this.toolExitRot = Quaternion.Euler(this.toolExitRotEuler);
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000DBADC File Offset: 0x000D9CDC
	private void Update()
	{
		if (!this.animatingSwap && !this.animatingDeposit && this.activeEntryIndex != this.displayedEntryIndex)
		{
			this.animatingSwap = true;
			this.animationStartTime = Time.time;
			this.animPrevToolIndex = this.displayedEntryIndex;
			this.animNextToolIndex = this.activeEntryIndex;
			this.toolEntryRot = Quaternion.AngleAxis(this.toolEntryRotDegrees, Random.onUnitSphere);
		}
		if (this.animatingSwap)
		{
			float num = (Time.time - this.animationStartTime) / this.nextToolAnimationTime;
			Transform transform = null;
			if (this.animPrevToolIndex >= 0 && this.animPrevToolIndex < this.toolEntries.Count)
			{
				transform = this.toolEntries[this.animPrevToolIndex].displayToolParent;
				transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.toolExitRot, this.toolExitRotTimingCurve.Evaluate(num));
				transform.localPosition = Vector3.Lerp(Vector3.zero, this.toolExitPosOffset, this.toolExitPosTimingCurve.Evaluate(num));
			}
			Transform displayToolParent = this.toolEntries[this.animNextToolIndex].displayToolParent;
			displayToolParent.localRotation = Quaternion.Slerp(this.toolEntryRot, Quaternion.identity, this.toolEntryRotTimingCurve.Evaluate(num));
			displayToolParent.localPosition = Vector3.Lerp(this.toolEntryPosOffset, Vector3.zero, this.toolEntryPosTimingCurve.Evaluate(num));
			displayToolParent.gameObject.SetActive(true);
			if (num >= 1f)
			{
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
				this.displayedEntryIndex = this.animNextToolIndex;
				this.animatingSwap = false;
				return;
			}
		}
		else if (this.animatingDeposit)
		{
			float num2 = (Time.time - this.animationStartTime) / this.toolDepositAnimationTime;
			Transform displayToolParent2 = this.toolEntries[this.displayedEntryIndex].displayToolParent;
			Vector3 localPosition = displayToolParent2.localPosition;
			localPosition.y = Mathf.Lerp(0f, this.depositTransform.localPosition.y, this.toolDepositMotionCurveY.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			localPosition.z = Mathf.Lerp(0f, this.depositTransform.localPosition.z, this.toolDepositMotionCurveZ.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			displayToolParent2.localPosition = localPosition;
			this.depositLidTransform.localRotation = Quaternion.Slerp(Quaternion.identity, this.depositLidOpenRot, this.depositLidTimingCurve.Evaluate(num2));
			if (num2 >= 1f)
			{
				this.depositLidTransform.localRotation = Quaternion.identity;
				displayToolParent2.gameObject.SetActive(false);
				this.displayedEntryIndex = -1;
				this.animatingDeposit = false;
			}
		}
	}

	// Token: 0x0400346D RID: 13421
	[SerializeField]
	private List<GRToolPurchaseStation.ToolEntry> toolEntries = new List<GRToolPurchaseStation.ToolEntry>();

	// Token: 0x0400346E RID: 13422
	[SerializeField]
	private Transform displayTransform;

	// Token: 0x0400346F RID: 13423
	[SerializeField]
	private Transform depositTransform;

	// Token: 0x04003470 RID: 13424
	[SerializeField]
	private Transform toolSpawnLocation;

	// Token: 0x04003471 RID: 13425
	[SerializeField]
	private TMP_Text displayItemNameText;

	// Token: 0x04003472 RID: 13426
	[SerializeField]
	private TMP_Text displayItemCostText;

	// Token: 0x04003473 RID: 13427
	[SerializeField]
	private float nextToolAnimationTime = 0.5f;

	// Token: 0x04003474 RID: 13428
	[SerializeField]
	private float toolDepositAnimationTime = 1f;

	// Token: 0x04003475 RID: 13429
	[SerializeField]
	private Vector3 toolEntryPosOffset = new Vector3(0f, 0.25f, 0f);

	// Token: 0x04003476 RID: 13430
	[SerializeField]
	private Vector3 toolEntryRotEuler = new Vector3(0f, 0f, 15f);

	// Token: 0x04003477 RID: 13431
	[SerializeField]
	private float toolEntryRotDegrees = 15f;

	// Token: 0x04003478 RID: 13432
	[SerializeField]
	private Vector3 toolExitPosOffset = new Vector3(0f, 0f, -0.25f);

	// Token: 0x04003479 RID: 13433
	[SerializeField]
	private Vector3 toolExitRotEuler = new Vector3(180f, 0f, 0f);

	// Token: 0x0400347A RID: 13434
	[SerializeField]
	private AnimationCurve toolEntryPosTimingCurve;

	// Token: 0x0400347B RID: 13435
	[SerializeField]
	private AnimationCurve toolEntryRotTimingCurve;

	// Token: 0x0400347C RID: 13436
	[SerializeField]
	private AnimationCurve toolExitPosTimingCurve;

	// Token: 0x0400347D RID: 13437
	[SerializeField]
	private AnimationCurve toolExitRotTimingCurve;

	// Token: 0x0400347E RID: 13438
	[SerializeField]
	private AnimationCurve toolDepositTimingCurve;

	// Token: 0x0400347F RID: 13439
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveY;

	// Token: 0x04003480 RID: 13440
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveZ;

	// Token: 0x04003481 RID: 13441
	[SerializeField]
	private Transform depositLidTransform;

	// Token: 0x04003482 RID: 13442
	[SerializeField]
	private Vector3 depositLidOpenEuler = new Vector3(65f, 0f, 0f);

	// Token: 0x04003483 RID: 13443
	[SerializeField]
	private AnimationCurve depositLidTimingCurve;

	// Token: 0x04003484 RID: 13444
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003485 RID: 13445
	[SerializeField]
	private AudioClip nextItemAudio;

	// Token: 0x04003486 RID: 13446
	[SerializeField]
	private float nextItemVolume = 0.5f;

	// Token: 0x04003487 RID: 13447
	[SerializeField]
	private AudioClip purchaseAudio;

	// Token: 0x04003488 RID: 13448
	[SerializeField]
	private float purchaseVolume = 0.5f;

	// Token: 0x04003489 RID: 13449
	[SerializeField]
	private AudioClip purchaseFailedAudio;

	// Token: 0x0400348A RID: 13450
	[SerializeField]
	private float purchaseFailedVolume = 0.5f;

	// Token: 0x0400348B RID: 13451
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x0400348C RID: 13452
	private int activeEntryIndex = 1;

	// Token: 0x0400348D RID: 13453
	private int displayedEntryIndex = -1;

	// Token: 0x0400348E RID: 13454
	private float animationStartTime;

	// Token: 0x0400348F RID: 13455
	private bool animatingDeposit;

	// Token: 0x04003490 RID: 13456
	private bool animatingSwap;

	// Token: 0x04003491 RID: 13457
	private int animPrevToolIndex;

	// Token: 0x04003492 RID: 13458
	private int animNextToolIndex;

	// Token: 0x04003493 RID: 13459
	private Quaternion depositLidOpenRot = Quaternion.identity;

	// Token: 0x04003494 RID: 13460
	private Quaternion toolEntryRot = Quaternion.identity;

	// Token: 0x04003495 RID: 13461
	private Quaternion toolExitRot = Quaternion.identity;

	// Token: 0x04003496 RID: 13462
	private Coroutine vendingCoroutine;

	// Token: 0x04003497 RID: 13463
	private bool debugIgnoreToolCost;

	// Token: 0x04003498 RID: 13464
	[HideInInspector]
	public int PurchaseStationId;

	// Token: 0x04003499 RID: 13465
	private GhostReactorManager grManager;

	// Token: 0x0400349A RID: 13466
	private GhostReactor reactor;

	// Token: 0x02000683 RID: 1667
	[Serializable]
	public struct ToolEntry
	{
		// Token: 0x060028DF RID: 10463 RVA: 0x000DBE99 File Offset: 0x000DA099
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x0400349B RID: 13467
		public Transform displayToolParent;

		// Token: 0x0400349C RID: 13468
		public GameEntity entityPrefab;

		// Token: 0x0400349D RID: 13469
		public string toolName;

		// Token: 0x0400349E RID: 13470
		public int toolCost;

		// Token: 0x0400349F RID: 13471
		private int entityTypeId;

		// Token: 0x040034A0 RID: 13472
		private bool entityTypeIdSet;
	}
}
