using System;
using UnityEngine;

// Token: 0x02000506 RID: 1286
public class MonkeBallPlayer : MonoBehaviour
{
	// Token: 0x06001F68 RID: 8040 RVA: 0x000A64E7 File Offset: 0x000A46E7
	private void Awake()
	{
		if (this.gamePlayer == null)
		{
			this.gamePlayer = base.GetComponent<GameBallPlayer>();
		}
	}

	// Token: 0x040027F6 RID: 10230
	public GameBallPlayer gamePlayer;

	// Token: 0x040027F7 RID: 10231
	public MonkeBallGoalZone currGoalZone;
}
