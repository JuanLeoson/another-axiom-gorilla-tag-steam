using System;
using UnityEngine;

// Token: 0x02000A9D RID: 2717
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x04004D3E RID: 19774
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x02000A9E RID: 2718
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x04004D3F RID: 19775
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x04004D40 RID: 19776
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
