using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E7 RID: 2023
public class GorillaScoreboardSpawner : MonoBehaviour
{
	// Token: 0x0600329E RID: 12958 RVA: 0x00107BE3 File Offset: 0x00105DE3
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x0600329F RID: 12959 RVA: 0x00107BF2 File Offset: 0x00105DF2
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x060032A0 RID: 12960 RVA: 0x00107C2A File Offset: 0x00105E2A
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x060032A1 RID: 12961 RVA: 0x00107C38 File Offset: 0x00105E38
	public void OnJoinedRoom()
	{
		Debug.Log("SCOREBOARD JOIN ROOM");
		if (this.IsCurrentScoreboard())
		{
			this.notInRoomText.SetActive(false);
			this.currentScoreboard = Object.Instantiate<GameObject>(this.scoreboardPrefab, base.transform).GetComponent<GorillaScoreBoard>();
			this.currentScoreboard.transform.rotation = base.transform.rotation;
			if (this.includeMMR)
			{
				this.currentScoreboard.GetComponent<GorillaScoreBoard>().includeMMR = true;
				this.currentScoreboard.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	// Token: 0x060032A2 RID: 12962 RVA: 0x00107CC8 File Offset: 0x00105EC8
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GTPlayer.Instance.inOverlay;
	}

	// Token: 0x060032A3 RID: 12963 RVA: 0x00107CE8 File Offset: 0x00105EE8
	private IEnumerator UpdateBoard()
	{
		for (;;)
		{
			try
			{
				if (this.currentScoreboard != null)
				{
					bool flag = this.IsVisible();
					foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
					{
						if (flag != gorillaPlayerScoreboardLine.lastVisible)
						{
							gorillaPlayerScoreboardLine.lastVisible = flag;
						}
					}
					if (this.currentScoreboard.boardText.enabled != flag)
					{
						this.currentScoreboard.boardText.enabled = flag;
					}
					if (this.currentScoreboard.buttonText.enabled != flag)
					{
						this.currentScoreboard.buttonText.enabled = flag;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x00107CF7 File Offset: 0x00105EF7
	public void OnLeftRoom()
	{
		this.Cleanup();
		if (this.notInRoomText)
		{
			this.notInRoomText.SetActive(true);
		}
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x00107D18 File Offset: 0x00105F18
	public void Cleanup()
	{
		if (this.currentScoreboard != null)
		{
			Object.Destroy(this.currentScoreboard.gameObject);
			this.currentScoreboard = null;
		}
	}

	// Token: 0x04003F88 RID: 16264
	public string gameType;

	// Token: 0x04003F89 RID: 16265
	public bool includeMMR;

	// Token: 0x04003F8A RID: 16266
	public GameObject scoreboardPrefab;

	// Token: 0x04003F8B RID: 16267
	public GameObject notInRoomText;

	// Token: 0x04003F8C RID: 16268
	public GameObject controllingParentGameObject;

	// Token: 0x04003F8D RID: 16269
	public bool isActive = true;

	// Token: 0x04003F8E RID: 16270
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x04003F8F RID: 16271
	public bool lastVisible;

	// Token: 0x04003F90 RID: 16272
	public bool forOverlay;
}
