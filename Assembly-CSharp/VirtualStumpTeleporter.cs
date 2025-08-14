using System;
using GorillaExtensions;
using GorillaTagScripts.ModIO;
using TMPro;
using UnityEngine;

// Token: 0x0200084F RID: 2127
public class VirtualStumpTeleporter : MonoBehaviour, IBuildValidation
{
	// Token: 0x06003578 RID: 13688 RVA: 0x001185F5 File Offset: 0x001167F5
	public bool BuildValidationCheck()
	{
		if (this.mySerializer == null && base.transform.parent.parent.GetComponentInChildren<VirtualStumpTeleporterSerializer>() == null)
		{
			Debug.LogError("This teleporter needs a reference to a VirtualStumpTeleporterSerializer, or to be placed alongside a VirtualStumpTeleporterSerializer. Check out the arcade or the stump", this);
			return false;
		}
		return true;
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x00118630 File Offset: 0x00116830
	public void OnEnable()
	{
		if (this.mySerializer == null)
		{
			this.mySerializer = base.transform.parent.parent.GetComponentInChildren<VirtualStumpTeleporterSerializer>();
		}
		if (UGCPermissionManager.IsUGCDisabled)
		{
			this.HideHandHolds();
		}
		else
		{
			this.ShowHandHolds();
		}
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x0011869D File Offset: 0x0011689D
	public void OnDisable()
	{
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x001186C1 File Offset: 0x001168C1
	private void OnUGCEnabled()
	{
		this.ShowHandHolds();
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x001186C9 File Offset: 0x001168C9
	private void OnUGCDisabled()
	{
		this.HideHandHolds();
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x001186D4 File Offset: 0x001168D4
	public void OnTriggerEnter(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.teleporting || CustomMapManager.WaitingForRoomJoin || CustomMapManager.WaitingForDisconnect)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = Time.time;
			this.ShowCountdownText();
		}
	}

	// Token: 0x0600357E RID: 13694 RVA: 0x0011872C File Offset: 0x0011692C
	public void OnTriggerStay(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject && this.triggerEntryTime >= 0f)
		{
			this.UpdateCountdownText();
			if (!this.teleporting && this.triggerEntryTime + this.stayInTriggerDuration <= Time.time)
			{
				this.TeleportPlayer();
				this.HideCountdownText();
			}
		}
	}

	// Token: 0x0600357F RID: 13695 RVA: 0x00118798 File Offset: 0x00116998
	public void OnTriggerExit(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = -1f;
			this.HideCountdownText();
		}
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x001187D0 File Offset: 0x001169D0
	private void ShowCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			int num = 1 + Mathf.FloorToInt(this.stayInTriggerDuration);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
					this.countdownTexts[i].gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x0011884C File Offset: 0x00116A4C
	private void HideCountdownText()
	{
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = "";
					this.countdownTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x001188B0 File Offset: 0x00116AB0
	private void UpdateCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			float f = this.stayInTriggerDuration - (Time.time - this.triggerEntryTime);
			int num = 1 + Mathf.FloorToInt(f);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
				}
			}
		}
	}

	// Token: 0x06003583 RID: 13699 RVA: 0x00118928 File Offset: 0x00116B28
	public void TeleportPlayer()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		if (!this.teleporting)
		{
			this.teleporting = true;
			base.StartCoroutine(CustomMapManager.TeleportToVirtualStump(this.teleporterIndex, new Action<bool>(this.FinishTeleport), this.entrancePoint, this.mySerializer));
		}
	}

	// Token: 0x06003584 RID: 13700 RVA: 0x00118976 File Offset: 0x00116B76
	private void FinishTeleport(bool success = true)
	{
		if (this.teleporting)
		{
			this.teleporting = false;
			this.triggerEntryTime = -1f;
		}
	}

	// Token: 0x06003585 RID: 13701 RVA: 0x00118994 File Offset: 0x00116B94
	private void HideHandHolds()
	{
		foreach (GameObject gameObject in this.handHoldObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06003586 RID: 13702 RVA: 0x001189CC File Offset: 0x00116BCC
	private void ShowHandHolds()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		foreach (GameObject gameObject in this.handHoldObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x04004263 RID: 16995
	[SerializeField]
	private short teleporterIndex;

	// Token: 0x04004264 RID: 16996
	[SerializeField]
	private float stayInTriggerDuration = 3f;

	// Token: 0x04004265 RID: 16997
	[SerializeField]
	private TMP_Text[] countdownTexts;

	// Token: 0x04004266 RID: 16998
	[SerializeField]
	private GameObject[] handHoldObjects;

	// Token: 0x04004267 RID: 16999
	private VirtualStumpTeleporterSerializer mySerializer;

	// Token: 0x04004268 RID: 17000
	private bool teleporting;

	// Token: 0x04004269 RID: 17001
	private float triggerEntryTime = -1f;

	// Token: 0x0400426A RID: 17002
	public GTZone entrancePoint = GTZone.arcade;
}
