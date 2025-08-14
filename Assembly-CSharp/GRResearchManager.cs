using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// Token: 0x0200065B RID: 1627
public class GRResearchManager : MonoBehaviour
{
	// Token: 0x060027CD RID: 10189 RVA: 0x000D6A1A File Offset: 0x000D4C1A
	public void Init(GhostReactor reactor)
	{
		this.researchTree.Assemble();
		this.reactor = reactor;
		this.GeneratePlayerData();
		this.LoadPlayerResearchData();
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000D6A3C File Offset: 0x000D4C3C
	public void CompletedLoadingPlayerResearch()
	{
		Debug.LogError("Completed Loading Research Data");
		foreach (GRResearchStation grresearchStation in this.researchStations)
		{
			grresearchStation.Init(this);
			this.researchManagerUpdated.AddListener(new UnityAction(grresearchStation.ResearchTreeUpdated));
		}
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x000D6AB0 File Offset: 0x000D4CB0
	public void ResearchCoresUpdated()
	{
		this.researchManagerUpdated.Invoke();
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000D6ABD File Offset: 0x000D4CBD
	public void CompletedResearch()
	{
		DateTime now = DateTime.Now;
		this._playerResearchData.researchPoints = this._playerResearchData.researchPoints + 1;
		this.researchManagerUpdated.Invoke();
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x000D6AE0 File Offset: 0x000D4CE0
	public void UpdateModStations()
	{
		for (int i = 0; i < this.researchStations.Count; i++)
		{
			this.researchStations[i].UpdateResearchPoints(this.playerResearchPoints);
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x060027D2 RID: 10194 RVA: 0x000D6B1A File Offset: 0x000D4D1A
	// (set) Token: 0x060027D3 RID: 10195 RVA: 0x000D6B27 File Offset: 0x000D4D27
	public int playerResearchPoints
	{
		get
		{
			return this._playerResearchData.researchPoints;
		}
		set
		{
			this._playerResearchData.researchPoints = value;
		}
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000D6B35 File Offset: 0x000D4D35
	public void GeneratePlayerData()
	{
		this._playerResearchData = default(GRResearchManager.PlayerResearchData);
		this._playerResearchData.researchPoints = 0;
		this._playerResearchData.coresInserted = 0;
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000D6B5B File Offset: 0x000D4D5B
	public void LoadPlayerResearchData()
	{
		base.StartCoroutine(this.LoadPlayerResearchTreeWebRequest());
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x000D6B6A File Offset: 0x000D4D6A
	public IEnumerator LoadPlayerResearchTreeWebRequest()
	{
		UnityWebRequest unityWebRequest = this.FormatLoadResearchTreeWebRequest();
		yield return new WaitForSeconds(0.2f);
		Debug.LogError("Loading Research Tree - " + PlayerPrefs.GetString("GhostReactorResearch"));
		GRResearchManager.ResearchTreeResponse response = JsonConvert.DeserializeObject<GRResearchManager.ResearchTreeResponse>(PlayerPrefs.GetString("GhostReactorResearch"));
		this.LoadResearchTree(response);
		this.CompletedLoadingPlayerResearch();
		yield break;
		this.LoadPlayerResearchData();
		yield break;
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000D6B7C File Offset: 0x000D4D7C
	private void SaveResearchTreeToPlayerPrefs()
	{
		GRResearchManager.ResearchTreeResponse researchTreeResponse = new GRResearchManager.ResearchTreeResponse();
		List<string> list = new List<string>();
		List<bool> list2 = new List<bool>();
		for (int i = 0; i < this.researchTree.tree.Length; i++)
		{
			if (!this.researchTree.tree[i].defaultUnlocked)
			{
				list.Add(this.researchTree.tree[i].id);
				list2.Add(this.researchTree.tree[i].unlocked);
			}
		}
		researchTreeResponse.researchIDs = list.ToArray();
		researchTreeResponse.researchUnlocked = list2.ToArray();
		PlayerPrefs.SetString("GhostReactorResearch", JsonConvert.SerializeObject(researchTreeResponse));
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000D6C20 File Offset: 0x000D4E20
	private void DebugResetResearchTree()
	{
		for (int i = 0; i < this.researchTree.tree.Length; i++)
		{
			if (!this.researchTree.tree[i].defaultUnlocked)
			{
				this.researchTree.tree[i].unlocked = false;
			}
		}
		this.SaveResearchTreeToPlayerPrefs();
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000D6C74 File Offset: 0x000D4E74
	private void LoadResearchTree(GRResearchManager.ResearchTreeResponse response)
	{
		if (response == null)
		{
			return;
		}
		try
		{
			for (int i = 0; i < response.researchIDs.Length; i++)
			{
				ResearchNode researchNode;
				if (this.researchTree.dictionary.TryGetValue(response.researchIDs[i], out researchNode))
				{
					researchNode.unlocked = response.researchUnlocked[i];
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x00058615 File Offset: 0x00056815
	public UnityWebRequest FormatLoadResearchTreeWebRequest()
	{
		return null;
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000D6CE4 File Offset: 0x000D4EE4
	public void RequestUnlockUpgrade(string ResearchID)
	{
		base.StartCoroutine(this.UnlockResearchWebRequest(ResearchID));
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000D6CF4 File Offset: 0x000D4EF4
	public void UnlockLocalResearch(string ResearchID)
	{
		ResearchNode researchNode;
		if (this.researchTree.dictionary.TryGetValue(ResearchID, out researchNode))
		{
			researchNode.unlocked = true;
		}
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x000D6D1D File Offset: 0x000D4F1D
	private IEnumerator UnlockResearchWebRequest(string ResearchID)
	{
		UnityWebRequest unityWebRequest = this.FormatUnlockResearchWebRequest(ResearchID);
		yield return new WaitForSeconds(0.2f);
		this.AttemptToUnlockLocally(ResearchID);
		this.researchManagerUpdated.Invoke();
		yield break;
		this.RequestUnlockUpgrade(ResearchID);
		yield break;
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x000023F5 File Offset: 0x000005F5
	public void FailedUnlock()
	{
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000D6D34 File Offset: 0x000D4F34
	private bool AttemptToUnlockLocally(string ResearchID)
	{
		ResearchNode researchNode;
		if (this.researchTree.dictionary.TryGetValue(ResearchID, out researchNode))
		{
			if (researchNode.unlocked)
			{
				Debug.Log("NodeID : " + ResearchID + " is unlocked returning");
				return false;
			}
			GRPlayer grplayer = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
			if (grplayer != null)
			{
				if (researchNode.requiredEmployeeLevel <= GhostReactorProgression.GetTitleLevel(grplayer.CurrentProgression.redeemedPoints) && this.playerResearchPoints >= researchNode.researchCost)
				{
					Debug.Log(string.Format("RP: {0}", this.playerResearchPoints));
					this.playerResearchPoints -= researchNode.researchCost;
					this.UnlockLocalResearch(ResearchID);
					this.SaveResearchTreeToPlayerPrefs();
					Debug.Log(string.Format("Tool {0} is unlocked - {1}", ResearchID, this.playerResearchPoints));
					return true;
				}
				Debug.Log(string.Format("{0} <= {1} && {2} >= {3}", new object[]
				{
					researchNode.requiredEmployeeLevel,
					GhostReactorProgression.GetTitleLevel(grplayer.CurrentProgression.redeemedPoints),
					this.playerResearchPoints,
					researchNode.researchCost
				}));
			}
			else
			{
				Debug.Log("Unlocking - Player is Null");
			}
		}
		else
		{
			Debug.Log("Could not find " + ResearchID + " in tree");
		}
		return false;
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x00058615 File Offset: 0x00056815
	private UnityWebRequest FormatUnlockResearchWebRequest(string ResearchID)
	{
		return null;
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000D6E8C File Offset: 0x000D508C
	public List<ResearchNode> GetResearchNodesForTool(string ToolID)
	{
		ResearchNode researchNode;
		if (this.researchTree.dictionary.TryGetValue(ToolID, out researchNode))
		{
			return researchNode.modNodes;
		}
		return null;
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000D6EB6 File Offset: 0x000D50B6
	public bool IsResearchUnlocked(string researchId)
	{
		return this.researchTree.IsNodeUnlocked(researchId);
	}

	// Token: 0x04003346 RID: 13126
	public List<GRResearchStation> researchStations;

	// Token: 0x04003347 RID: 13127
	private GhostReactor reactor;

	// Token: 0x04003348 RID: 13128
	public GRResearchTreeSO researchTree;

	// Token: 0x04003349 RID: 13129
	private int unlockResearchWebRequestRetryCount;

	// Token: 0x0400334A RID: 13130
	private int loadResearchWebRequestRetryCount;

	// Token: 0x0400334B RID: 13131
	private int maxRetriesOnFail = 3;

	// Token: 0x0400334C RID: 13132
	public UnityEvent researchManagerUpdated;

	// Token: 0x0400334D RID: 13133
	private GRResearchManager.PlayerResearchData _playerResearchData;

	// Token: 0x0200065C RID: 1628
	private struct PlayerResearchData
	{
		// Token: 0x0400334E RID: 13134
		public int researchPoints;

		// Token: 0x0400334F RID: 13135
		public DateTime researchStartTime;

		// Token: 0x04003350 RID: 13136
		public int coresInserted;
	}

	// Token: 0x0200065D RID: 1629
	[Serializable]
	public class ResearchTreeResponse
	{
		// Token: 0x04003351 RID: 13137
		public string[] researchIDs = Array.Empty<string>();

		// Token: 0x04003352 RID: 13138
		public bool[] researchUnlocked = Array.Empty<bool>();
	}

	// Token: 0x0200065E RID: 1630
	public class UnlockReserachResponse
	{
		// Token: 0x04003353 RID: 13139
		public string Track;

		// Token: 0x04003354 RID: 13140
		public string Name;

		// Token: 0x04003355 RID: 13141
		public string ID;

		// Token: 0x04003356 RID: 13142
		public string[] requiredEntitlementIDs;

		// Token: 0x04003357 RID: 13143
		public string[] RequiredLevels;

		// Token: 0x04003358 RID: 13144
		public string[] PrerequisiteNodes;

		// Token: 0x04003359 RID: 13145
		public int ResearchCost;

		// Token: 0x0400335A RID: 13146
		public bool Unlocked;
	}
}
