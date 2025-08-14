using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CCC RID: 3276
	public class SharedBlocksVotingStation : MonoBehaviour
	{
		// Token: 0x06005163 RID: 20835 RVA: 0x00195B70 File Offset: 0x00193D70
		private void Start()
		{
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.table = builderTable;
				this.table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnLoadedMapChanged(this.table.GetCurrentMapID());
			}
			else
			{
				GTDev.LogWarning<string>("No Builder Table found for Voting Station", null);
			}
			base.GetComponentsInChildren<MeshRenderer>(false, this.meshes);
			this.upVoteButton.onPressButton.AddListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.AddListener(new UnityAction(this.OnDownVotePressed));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06005164 RID: 20836 RVA: 0x00195C5C File Offset: 0x00193E5C
		private void OnDestroy()
		{
			this.upVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnDownVotePressed));
			if (this.table != null)
			{
				this.table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06005165 RID: 20837 RVA: 0x00195D1C File Offset: 0x00193F1C
		private void OnZoneChanged()
		{
			bool enabled = ZoneManagement.instance.IsZoneActive(this.tableZone);
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				meshRenderer.enabled = enabled;
			}
		}

		// Token: 0x06005166 RID: 20838 RVA: 0x00195D80 File Offset: 0x00193F80
		private void OnUpVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.upVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, true, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.upVoteButton.pressedMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.upVoteButton.enabled = false;
				this.downVoteButton.enabled = true;
			}
		}

		// Token: 0x06005167 RID: 20839 RVA: 0x00195E40 File Offset: 0x00194040
		private void OnDownVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.downVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, false, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.downVoteButton.pressedMaterial;
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = false;
			}
		}

		// Token: 0x06005168 RID: 20840 RVA: 0x00195F00 File Offset: 0x00194100
		private void OnVoteResponse(bool success, string message)
		{
			this.voteInProgress = false;
			if (success)
			{
				this.statusText.text = "Successfully submitted vote";
				this.statusText.gameObject.SetActive(true);
			}
			else
			{
				this.statusText.text = message;
				this.statusText.gameObject.SetActive(true);
				if (!this.loadedMapID.IsNullOrEmpty())
				{
					this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.upVoteButton.enabled = true;
					this.downVoteButton.enabled = true;
				}
			}
			this.clearStatusTime = Time.time + this.clearStatusDelay;
			this.waitingToClearStatus = true;
		}

		// Token: 0x06005169 RID: 20841 RVA: 0x00195FC1 File Offset: 0x001941C1
		private void LateUpdate()
		{
			if (this.waitingToClearStatus && Time.time > this.clearStatusTime)
			{
				this.waitingToClearStatus = false;
				this.statusText.text = "";
				this.statusText.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600516A RID: 20842 RVA: 0x00196000 File Offset: 0x00194200
		private void OnLoadedMapChanged(string mapID)
		{
			this.loadedMapID = mapID;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x0600516B RID: 20843 RVA: 0x00196020 File Offset: 0x00194220
		private void OnMapCleared()
		{
			this.loadedMapID = null;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x0600516C RID: 20844 RVA: 0x00196040 File Offset: 0x00194240
		private void UpdateScreen()
		{
			if (!this.loadedMapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(this.loadedMapID))
			{
				this.screenText.text = "MAP: " + SharedBlocksTerminal.MapIDToDisplayedString(this.loadedMapID);
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = true;
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				return;
			}
			this.screenText.text = "MAP: NONE";
			this.upVoteButton.enabled = false;
			this.downVoteButton.enabled = false;
			this.upVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
			this.downVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
		}

		// Token: 0x04005AE2 RID: 23266
		[SerializeField]
		private TMP_Text screenText;

		// Token: 0x04005AE3 RID: 23267
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04005AE4 RID: 23268
		[SerializeField]
		private GorillaPressableButton upVoteButton;

		// Token: 0x04005AE5 RID: 23269
		[SerializeField]
		private GorillaPressableButton downVoteButton;

		// Token: 0x04005AE6 RID: 23270
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04005AE7 RID: 23271
		[SerializeField]
		private Material buttonDefaultMaterial;

		// Token: 0x04005AE8 RID: 23272
		[SerializeField]
		private Material buttonDisabledMaterial;

		// Token: 0x04005AE9 RID: 23273
		private BuilderTable table;

		// Token: 0x04005AEA RID: 23274
		private string loadedMapID = string.Empty;

		// Token: 0x04005AEB RID: 23275
		private bool voteInProgress;

		// Token: 0x04005AEC RID: 23276
		private bool waitingToClearStatus;

		// Token: 0x04005AED RID: 23277
		private float clearStatusTime;

		// Token: 0x04005AEE RID: 23278
		private float clearStatusDelay = 2f;

		// Token: 0x04005AEF RID: 23279
		private List<MeshRenderer> meshes = new List<MeshRenderer>(12);
	}
}
