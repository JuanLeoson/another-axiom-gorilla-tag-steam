using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000500 RID: 1280
public class MonkeBallBallResetTrigger : MonoBehaviour
{
	// Token: 0x06001F22 RID: 7970 RVA: 0x000A48C4 File Offset: 0x000A2AC4
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			GameBallPlayer gameBallPlayer = (component.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.heldByActorNumber);
			if (gameBallPlayer == null)
			{
				gameBallPlayer = ((component.lastHeldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.lastHeldByActorNumber));
				if (gameBallPlayer == null)
				{
					return;
				}
			}
			this._lastBall = component;
			int num = gameBallPlayer.teamId;
			if (num == -1)
			{
				num = component.lastHeldByTeamId;
			}
			if (num >= 0 && num < this.teamMaterials.Length)
			{
				this.trigger.sharedMaterial = this.teamMaterials[num];
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(true, num);
			}
		}
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000A497C File Offset: 0x000A2B7C
	private void OnTriggerExit(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (component == this._lastBall)
			{
				this.trigger.sharedMaterial = this.neutralMaterial;
				this._lastBall = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(false, -1);
			}
		}
	}

	// Token: 0x040027BC RID: 10172
	public Renderer trigger;

	// Token: 0x040027BD RID: 10173
	public Material[] teamMaterials;

	// Token: 0x040027BE RID: 10174
	public Material neutralMaterial;

	// Token: 0x040027BF RID: 10175
	private GameBall _lastBall;
}
