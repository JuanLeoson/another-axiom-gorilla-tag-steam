using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class PartyGameModeWarning : MonoBehaviour
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000C7F RID: 3199 RVA: 0x0004377C File Offset: 0x0004197C
	public bool ShouldShowWarning
	{
		get
		{
			return FriendshipGroupDetection.Instance.IsInParty && FriendshipGroupDetection.Instance.AnyPartyMembersOutsideFriendCollider();
		}
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00043798 File Offset: 0x00041998
	private void Awake()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x000437C3 File Offset: 0x000419C3
	public void Show()
	{
		this.visibleUntilTimestamp = Time.time + this.visibleDuration;
		if (this.hideCoroutine == null)
		{
			this.hideCoroutine = base.StartCoroutine(this.HideCo());
		}
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x000437F1 File Offset: 0x000419F1
	private IEnumerator HideCo()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		float lastVisible;
		do
		{
			lastVisible = this.visibleUntilTimestamp;
			yield return new WaitForSeconds(this.visibleUntilTimestamp - Time.time);
		}
		while (lastVisible != this.visibleUntilTimestamp);
		array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x04000F7D RID: 3965
	[SerializeField]
	private GameObject[] showParts;

	// Token: 0x04000F7E RID: 3966
	[SerializeField]
	private GameObject[] hideParts;

	// Token: 0x04000F7F RID: 3967
	[SerializeField]
	private float visibleDuration;

	// Token: 0x04000F80 RID: 3968
	private float visibleUntilTimestamp;

	// Token: 0x04000F81 RID: 3969
	private Coroutine hideCoroutine;
}
