using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004FF RID: 1279
public class MonkeBallBallKillZone : MonoBehaviour
{
	// Token: 0x06001F20 RID: 7968 RVA: 0x000A4874 File Offset: 0x000A2A74
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.RequestResetBall(component.id, -1);
				return;
			}
			GameBallManager.Instance.RequestSetBallPosition(component.id);
		}
	}
}
