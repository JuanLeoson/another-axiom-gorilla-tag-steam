using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E87 RID: 3719
	[CreateAssetMenu(fileName = "GorillaButtonColorSettings", menuName = "ScriptableObjects/GorillaButtonColorSettings", order = 0)]
	public class ButtonColorSettings : ScriptableObject
	{
		// Token: 0x0400674A RID: 26442
		public Color UnpressedColor;

		// Token: 0x0400674B RID: 26443
		public Color PressedColor;

		// Token: 0x0400674C RID: 26444
		[Tooltip("Optional\nThe time the change will be in effect")]
		public float PressedTime;
	}
}
