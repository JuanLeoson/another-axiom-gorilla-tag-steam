using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;

// Token: 0x020006EA RID: 1770
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x06002BFB RID: 11259 RVA: 0x000E9178 File Offset: 0x000E7378
	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			double num;
			if (double.TryParse(s, out num))
			{
				this.oneOverChance = num;
			}
		}, delegate(PlayFabError e)
		{
		});
		for (;;)
		{
			yield return new WaitForSeconds(this.interval);
			if ((double)Random.Range(0f, 1f) < 1.0 / this.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				this.UploadGorillanalytics();
			}
		}
		yield break;
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x000E9188 File Offset: 0x000E7388
	private void UploadGorillanalytics()
	{
		try
		{
			string map;
			string mode;
			string queue;
			this.GetMapModeQueue(out map, out mode, out queue);
			Vector3 position = GTPlayer.Instance.headCollider.transform.position;
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			this.uploadData.version = NetworkSystemConfig.AppVersion;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = map;
			this.uploadData.mode = mode;
			this.uploadData.queue = queue;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = averagedVelocity.x;
			this.uploadData.vel_y = averagedVelocity.y;
			this.uploadData.vel_z = averagedVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", from c in CosmeticsController.instance.unlockedCosmetics
			select c.itemName);
			this.uploadData.cosmetics_worn = string.Join(";", from c in CosmeticsController.instance.currentWornSet.items
			select c.itemName);
			GorillaServer.Instance.UploadGorillanalytics(this.uploadData);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06002BFD RID: 11261 RVA: 0x000E9358 File Offset: 0x000E7558
	private void GetMapModeQueue(out string map, out string mode, out string queue)
	{
		if (!PhotonNetwork.InRoom)
		{
			map = "none";
			mode = "none";
			queue = "none";
			return;
		}
		object obj = null;
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null)
		{
			currentRoom.CustomProperties.TryGetValue("gameMode", out obj);
		}
		string gameMode = ((obj != null) ? obj.ToString() : null) ?? "";
		GTZone gtzone = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone;
		if (gtzone == GTZone.cityNoBuildings || gtzone == GTZone.cityWithSkyJungle || gtzone == GTZone.mall)
		{
			gtzone = GTZone.city;
		}
		if (gtzone == GTZone.tutorial)
		{
			gtzone = GTZone.forest;
		}
		if (gtzone == GTZone.ghostReactorTunnel)
		{
			gtzone = GTZone.ghostReactor;
		}
		map = gtzone.ToString().ToLower();
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			map += "private";
		}
		mode = (this.modes.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown");
		queue = (this.queues.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown");
	}

	// Token: 0x0400376F RID: 14191
	public float interval = 60f;

	// Token: 0x04003770 RID: 14192
	public double oneOverChance = 4320.0;

	// Token: 0x04003771 RID: 14193
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04003772 RID: 14194
	public GameModeZoneMapping gameModeData;

	// Token: 0x04003773 RID: 14195
	public List<string> maps;

	// Token: 0x04003774 RID: 14196
	public List<string> modes;

	// Token: 0x04003775 RID: 14197
	public List<string> queues;

	// Token: 0x04003776 RID: 14198
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x020006EB RID: 1771
	private class UploadData
	{
		// Token: 0x04003777 RID: 14199
		public string version;

		// Token: 0x04003778 RID: 14200
		public double upload_chance;

		// Token: 0x04003779 RID: 14201
		public string map;

		// Token: 0x0400377A RID: 14202
		public string mode;

		// Token: 0x0400377B RID: 14203
		public string queue;

		// Token: 0x0400377C RID: 14204
		public int player_count;

		// Token: 0x0400377D RID: 14205
		public float pos_x;

		// Token: 0x0400377E RID: 14206
		public float pos_y;

		// Token: 0x0400377F RID: 14207
		public float pos_z;

		// Token: 0x04003780 RID: 14208
		public float vel_x;

		// Token: 0x04003781 RID: 14209
		public float vel_y;

		// Token: 0x04003782 RID: 14210
		public float vel_z;

		// Token: 0x04003783 RID: 14211
		public string cosmetics_owned;

		// Token: 0x04003784 RID: 14212
		public string cosmetics_worn;
	}
}
