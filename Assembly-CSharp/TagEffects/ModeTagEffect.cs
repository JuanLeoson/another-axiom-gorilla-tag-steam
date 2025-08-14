using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000E03 RID: 3587
	[Serializable]
	public class ModeTagEffect
	{
		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x060058CF RID: 22735 RVA: 0x001B99A6 File Offset: 0x001B7BA6
		public HashSet<GameModeType> Modes
		{
			get
			{
				if (this.modesHash == null)
				{
					this.modesHash = new HashSet<GameModeType>(this.modes);
				}
				return this.modesHash;
			}
		}

		// Token: 0x0400629D RID: 25245
		[SerializeField]
		private GameModeType[] modes;

		// Token: 0x0400629E RID: 25246
		private HashSet<GameModeType> modesHash;

		// Token: 0x0400629F RID: 25247
		public TagEffectPack tagEffect;

		// Token: 0x040062A0 RID: 25248
		public bool blockTagOverride;

		// Token: 0x040062A1 RID: 25249
		public bool blockFistBumpOverride;

		// Token: 0x040062A2 RID: 25250
		public bool blockHiveFiveOverride;
	}
}
