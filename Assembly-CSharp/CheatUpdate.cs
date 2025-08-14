using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000563 RID: 1379
public class CheatUpdate : MonoBehaviour
{
	// Token: 0x0600219E RID: 8606 RVA: 0x000B71E5 File Offset: 0x000B53E5
	private void Start()
	{
		base.StartCoroutine(this.UpdateNumberOfPlayers());
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000B71F4 File Offset: 0x000B53F4
	public IEnumerator UpdateNumberOfPlayers()
	{
		for (;;)
		{
			base.StartCoroutine(this.UpdatePlayerCount());
			yield return new WaitForSeconds(10f);
		}
		yield break;
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000B7203 File Offset: 0x000B5403
	private IEnumerator UpdatePlayerCount()
	{
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("player_count", PhotonNetwork.CountOfPlayers - 1);
		wwwform.AddField("game_version", "live");
		wwwform.AddField("game_name", Application.productName);
		Debug.Log(PhotonNetwork.CountOfPlayers - 1);
		using (UnityWebRequest www = UnityWebRequest.Post("http://ntsfranz.crabdance.com/update_monke_count", wwwform))
		{
			yield return www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.Log(www.error);
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}
}
