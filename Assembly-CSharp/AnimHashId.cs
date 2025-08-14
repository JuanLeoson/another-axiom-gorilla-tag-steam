using System;
using UnityEngine;

// Token: 0x02000A8F RID: 2703
[Serializable]
public struct AnimHashId
{
	// Token: 0x17000640 RID: 1600
	// (get) Token: 0x06004196 RID: 16790 RVA: 0x0014B115 File Offset: 0x00149315
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x17000641 RID: 1601
	// (get) Token: 0x06004197 RID: 16791 RVA: 0x0014B11D File Offset: 0x0014931D
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x06004198 RID: 16792 RVA: 0x0014B125 File Offset: 0x00149325
	public AnimHashId(string text)
	{
		this._text = text;
		this._hash = Animator.StringToHash(text);
	}

	// Token: 0x06004199 RID: 16793 RVA: 0x0014B115 File Offset: 0x00149315
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x0600419A RID: 16794 RVA: 0x0014B11D File Offset: 0x0014931D
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x0600419B RID: 16795 RVA: 0x0014B11D File Offset: 0x0014931D
	public static implicit operator int(AnimHashId h)
	{
		return h._hash;
	}

	// Token: 0x0600419C RID: 16796 RVA: 0x0014B13A File Offset: 0x0014933A
	public static implicit operator AnimHashId(string s)
	{
		return new AnimHashId(s);
	}

	// Token: 0x04004D21 RID: 19745
	[SerializeField]
	private string _text;

	// Token: 0x04004D22 RID: 19746
	[NonSerialized]
	private int _hash;
}
