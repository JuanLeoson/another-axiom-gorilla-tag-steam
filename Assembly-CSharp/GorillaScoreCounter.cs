using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057C RID: 1404
public class GorillaScoreCounter : MonoBehaviour
{
	// Token: 0x06002255 RID: 8789 RVA: 0x000B973C File Offset: 0x000B793C
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
		if (this.isRedTeam)
		{
			this.attribute = "redScore";
			return;
		}
		this.attribute = "blueScore";
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000B9770 File Offset: 0x000B7970
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties[this.attribute] != null)
		{
			this.text.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties[this.attribute]).ToString();
		}
	}

	// Token: 0x04002BBD RID: 11197
	public bool isRedTeam;

	// Token: 0x04002BBE RID: 11198
	public Text text;

	// Token: 0x04002BBF RID: 11199
	public string attribute;
}
