using System;
using GorillaTagScripts.ModIO;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000C73 RID: 3187
	public class CustomMapsRoomMapDisplay : MonoBehaviour
	{
		// Token: 0x06004ED5 RID: 20181 RVA: 0x00188660 File Offset: 0x00186860
		public void Start()
		{
			this.loginToModioText.gameObject.SetActive(true);
			this.roomMapNameText.text = this.noRoomMapString;
			this.roomMapStatusText.text = this.notLoadedStatusString;
			this.roomMapLabelText.gameObject.SetActive(false);
			this.roomMapNameText.gameObject.SetActive(false);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnectedFromRoom;
			ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
			ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
			CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		// Token: 0x06004ED6 RID: 20182 RVA: 0x00188794 File Offset: 0x00186994
		public void OnDestroy()
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		}

		// Token: 0x06004ED7 RID: 20183 RVA: 0x001887FC File Offset: 0x001869FC
		private void OnModIOLoggedOut()
		{
			this.roomMapLabelText.gameObject.SetActive(false);
			this.roomMapNameText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.loginToModioText.gameObject.SetActive(true);
		}

		// Token: 0x06004ED8 RID: 20184 RVA: 0x0018885E File Offset: 0x00186A5E
		private void OnModIOLoggedIn()
		{
			this.loginToModioText.gameObject.SetActive(false);
			this.roomMapLabelText.gameObject.SetActive(true);
			this.roomMapNameText.gameObject.SetActive(true);
			this.UpdateRoomMap();
		}

		// Token: 0x06004ED9 RID: 20185 RVA: 0x00188899 File Offset: 0x00186A99
		private void OnJoinedRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06004EDA RID: 20186 RVA: 0x00188899 File Offset: 0x00186A99
		private void OnDisconnectedFromRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06004EDB RID: 20187 RVA: 0x00188899 File Offset: 0x00186A99
		private void OnRoomMapChanged(ModId roomMapModId)
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06004EDC RID: 20188 RVA: 0x001888A4 File Offset: 0x00186AA4
		private void UpdateRoomMap()
		{
			if (!ModIOManager.IsLoggedIn())
			{
				return;
			}
			ModId currentRoomMap = CustomMapManager.GetRoomMapId();
			if (currentRoomMap == ModId.Null)
			{
				this.roomMapNameText.text = this.noRoomMapString;
				this.roomMapStatusLabelText.gameObject.SetActive(false);
				this.roomMapStatusText.gameObject.SetActive(false);
				return;
			}
			ModIOManager.GetModProfile(currentRoomMap, delegate(ModIORequestResultAnd<ModProfile> result)
			{
				if (!ModIOManager.IsLoggedIn())
				{
					return;
				}
				if (!result.result.success)
				{
					this.roomMapNameText.text = "Failed to retrieve mod info.";
					return;
				}
				this.roomMapNameText.text = result.data.name;
				this.roomMapStatusLabelText.gameObject.SetActive(true);
				if (CustomMapLoader.IsModLoaded(currentRoomMap.id))
				{
					this.roomMapStatusText.text = this.readyToPlayStatusString;
					this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				}
				else if (CustomMapManager.IsLoading(currentRoomMap.id))
				{
					this.roomMapStatusText.text = this.loadingStatusString;
					this.roomMapStatusText.color = this.loadingStatusStringColor;
				}
				else
				{
					this.roomMapStatusText.text = this.notLoadedStatusString;
					this.roomMapStatusText.color = this.notLoadedStatusStringColor;
				}
				this.roomMapStatusText.gameObject.SetActive(true);
			});
		}

		// Token: 0x06004EDD RID: 20189 RVA: 0x00188930 File Offset: 0x00186B30
		private void OnMapLoadComplete(bool success)
		{
			if (!ModIOManager.IsLoggedIn())
			{
				return;
			}
			if (success)
			{
				this.roomMapStatusText.text = this.readyToPlayStatusString;
				this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				return;
			}
			this.roomMapStatusText.text = this.loadFailedStatusString;
			this.roomMapStatusText.color = this.loadFailedStatusStringColor;
		}

		// Token: 0x06004EDE RID: 20190 RVA: 0x0018898D File Offset: 0x00186B8D
		private void OnMapLoadProgress(MapLoadStatus status, int progress, string message)
		{
			if (!ModIOManager.IsLoggedIn())
			{
				return;
			}
			if (status - MapLoadStatus.Downloading <= 1)
			{
				this.roomMapStatusText.text = this.loadingStatusString;
				this.roomMapStatusText.color = this.loadingStatusStringColor;
			}
		}

		// Token: 0x040057D8 RID: 22488
		[SerializeField]
		private TMP_Text roomMapLabelText;

		// Token: 0x040057D9 RID: 22489
		[SerializeField]
		private TMP_Text roomMapNameText;

		// Token: 0x040057DA RID: 22490
		[SerializeField]
		private TMP_Text roomMapStatusLabelText;

		// Token: 0x040057DB RID: 22491
		[SerializeField]
		private TMP_Text roomMapStatusText;

		// Token: 0x040057DC RID: 22492
		[SerializeField]
		private TMP_Text loginToModioText;

		// Token: 0x040057DD RID: 22493
		[SerializeField]
		private string noRoomMapString = "NONE";

		// Token: 0x040057DE RID: 22494
		[SerializeField]
		private string notLoadedStatusString = "NOT LOADED";

		// Token: 0x040057DF RID: 22495
		[SerializeField]
		private string loadingStatusString = "LOADING...";

		// Token: 0x040057E0 RID: 22496
		[SerializeField]
		private string readyToPlayStatusString = "READY!";

		// Token: 0x040057E1 RID: 22497
		[SerializeField]
		private string loadFailedStatusString = "LOAD FAILED";

		// Token: 0x040057E2 RID: 22498
		[SerializeField]
		private Color notLoadedStatusStringColor = Color.red;

		// Token: 0x040057E3 RID: 22499
		[SerializeField]
		private Color loadingStatusStringColor = Color.yellow;

		// Token: 0x040057E4 RID: 22500
		[SerializeField]
		private Color readyToPlayStatusStringColor = Color.green;

		// Token: 0x040057E5 RID: 22501
		[SerializeField]
		private Color loadFailedStatusStringColor = Color.red;
	}
}
