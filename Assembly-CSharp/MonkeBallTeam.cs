using System;
using UnityEngine;

// Token: 0x02000501 RID: 1281
[Serializable]
public class MonkeBallTeam
{
	// Token: 0x040027C0 RID: 10176
	public Color color;

	// Token: 0x040027C1 RID: 10177
	public int score;

	// Token: 0x040027C2 RID: 10178
	public Transform ballStartLocation;

	// Token: 0x040027C3 RID: 10179
	public Transform ballLaunchPosition;

	// Token: 0x040027C4 RID: 10180
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLaunchVelocityRange = new Vector2(8f, 15f);

	// Token: 0x040027C5 RID: 10181
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x040027C6 RID: 10182
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);
}
