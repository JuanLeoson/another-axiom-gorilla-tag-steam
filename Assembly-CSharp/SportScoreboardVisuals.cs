using System;
using GorillaTag.Sports;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
public class SportScoreboardVisuals : MonoBehaviour
{
	// Token: 0x0600317D RID: 12669 RVA: 0x00101580 File Offset: 0x000FF780
	private void Awake()
	{
		SportScoreboard.Instance.RegisterTeamVisual(this.TeamIndex, this);
	}

	// Token: 0x04003D2A RID: 15658
	[SerializeField]
	public MaterialUVOffsetListSetter score1s;

	// Token: 0x04003D2B RID: 15659
	[SerializeField]
	public MaterialUVOffsetListSetter score10s;

	// Token: 0x04003D2C RID: 15660
	[SerializeField]
	private int TeamIndex;
}
