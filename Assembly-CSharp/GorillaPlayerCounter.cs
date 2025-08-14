using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200057B RID: 1403
public class GorillaPlayerCounter : MonoBehaviour
{
	// Token: 0x06002252 RID: 8786 RVA: 0x000B9694 File Offset: 0x000B7894
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000B96A8 File Offset: 0x000B78A8
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			int num = 0;
			foreach (KeyValuePair<int, Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
			{
				if ((bool)keyValuePair.Value.CustomProperties["isRedTeam"] == this.isRedTeam)
				{
					num++;
				}
			}
			this.text.text = num.ToString();
		}
	}

	// Token: 0x04002BBA RID: 11194
	public bool isRedTeam;

	// Token: 0x04002BBB RID: 11195
	public Text text;

	// Token: 0x04002BBC RID: 11196
	public string attribute;
}
