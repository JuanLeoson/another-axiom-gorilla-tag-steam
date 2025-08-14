using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C24 RID: 3108
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x06004C72 RID: 19570 RVA: 0x0017B2E0 File Offset: 0x001794E0
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x000023F5 File Offset: 0x000005F5
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x04005590 RID: 21904
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x04005591 RID: 21905
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x02000C25 RID: 3109
		[Serializable]
		public struct VisualState
		{
			// Token: 0x06004C75 RID: 19573 RVA: 0x0017B30C File Offset: 0x0017950C
			public override int GetHashCode()
			{
				int item = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int item2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(item, item2).GetHashCode();
			}

			// Token: 0x06004C76 RID: 19574 RVA: 0x0017B344 File Offset: 0x00179544
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x04005592 RID: 21906
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x04005593 RID: 21907
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
