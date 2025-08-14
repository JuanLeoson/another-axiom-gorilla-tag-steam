using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class MetroManager : MonoBehaviour
{
	// Token: 0x060004F6 RID: 1270 RVA: 0x0001CF58 File Offset: 0x0001B158
	private void Update()
	{
		for (int i = 0; i < this._blimps.Length; i++)
		{
			this._blimps[i].Tick();
		}
		for (int j = 0; j < this._spotlights.Length; j++)
		{
			this._spotlights[j].Tick();
		}
	}

	// Token: 0x040005EB RID: 1515
	[SerializeField]
	private MetroBlimp[] _blimps = new MetroBlimp[0];

	// Token: 0x040005EC RID: 1516
	[SerializeField]
	private MetroSpotlight[] _spotlights = new MetroSpotlight[0];

	// Token: 0x040005ED RID: 1517
	[Space]
	[SerializeField]
	private Transform _blimpsRotationAnchor;
}
