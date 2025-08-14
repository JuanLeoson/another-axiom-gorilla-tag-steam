using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000DFA RID: 3578
	public interface IHandEffectsTrigger
	{
		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x060058A7 RID: 22695
		IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x060058A8 RID: 22696
		Transform Transform { get; }

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x060058A9 RID: 22697
		VRRig Rig { get; }

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x060058AA RID: 22698
		bool FingersDown { get; }

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x060058AB RID: 22699
		bool FingersUp { get; }

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x060058AC RID: 22700
		Vector3 Velocity { get; }

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x060058AD RID: 22701
		bool RightHand { get; }

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x060058AE RID: 22702
		TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x060058AF RID: 22703
		bool Static { get; }

		// Token: 0x060058B0 RID: 22704
		void OnTriggerEntered(IHandEffectsTrigger other);

		// Token: 0x060058B1 RID: 22705
		bool InTriggerZone(IHandEffectsTrigger t);

		// Token: 0x02000DFB RID: 3579
		public enum Mode
		{
			// Token: 0x04006273 RID: 25203
			HighFive,
			// Token: 0x04006274 RID: 25204
			FistBump,
			// Token: 0x04006275 RID: 25205
			Tag3P,
			// Token: 0x04006276 RID: 25206
			Tag1P,
			// Token: 0x04006277 RID: 25207
			HighFive_And_FistBump
		}
	}
}
