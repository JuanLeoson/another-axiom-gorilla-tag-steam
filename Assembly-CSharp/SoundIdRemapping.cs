using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
[Serializable]
internal class SoundIdRemapping
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000C7 RID: 199 RVA: 0x00005525 File Offset: 0x00003725
	public int SoundIn
	{
		get
		{
			return this.soundIn;
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000C8 RID: 200 RVA: 0x0000552D File Offset: 0x0000372D
	public int SoundOut
	{
		get
		{
			return this.soundOut;
		}
	}

	// Token: 0x040000DF RID: 223
	[GorillaSoundLookup]
	[SerializeField]
	private int soundIn = 1;

	// Token: 0x040000E0 RID: 224
	[GorillaSoundLookup]
	[SerializeField]
	private int soundOut = 2;
}
