using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A9B RID: 2715
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x04004D3A RID: 19770
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000A9C RID: 2716
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x04004D3B RID: 19771
		public GameObject go;

		// Token: 0x04004D3C RID: 19772
		public Transform bakingTransform;

		// Token: 0x04004D3D RID: 19773
		public Transform gameTransform;
	}
}
