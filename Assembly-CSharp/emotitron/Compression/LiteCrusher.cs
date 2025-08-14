using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000F7A RID: 3962
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x06006302 RID: 25346 RVA: 0x001F4008 File Offset: 0x001F2208
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x04006E29 RID: 28201
		[SerializeField]
		protected int bits;
	}
}
