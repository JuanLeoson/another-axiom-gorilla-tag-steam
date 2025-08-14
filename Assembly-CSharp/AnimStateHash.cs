using System;
using UnityEngine;

// Token: 0x0200085E RID: 2142
[Serializable]
public struct AnimStateHash
{
	// Token: 0x060035EC RID: 13804 RVA: 0x0011B890 File Offset: 0x00119A90
	public static implicit operator AnimStateHash(string s)
	{
		return new AnimStateHash
		{
			_hash = Animator.StringToHash(s)
		};
	}

	// Token: 0x060035ED RID: 13805 RVA: 0x0011B8B3 File Offset: 0x00119AB3
	public static implicit operator int(AnimStateHash ash)
	{
		return ash._hash;
	}

	// Token: 0x040042DB RID: 17115
	[SerializeField]
	private int _hash;
}
