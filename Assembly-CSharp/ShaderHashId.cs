using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000ADE RID: 2782
[Serializable]
public struct ShaderHashId : IEquatable<ShaderHashId>
{
	// Token: 0x17000659 RID: 1625
	// (get) Token: 0x060042F1 RID: 17137 RVA: 0x0014FF67 File Offset: 0x0014E167
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x060042F2 RID: 17138 RVA: 0x0014FF6F File Offset: 0x0014E16F
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x060042F3 RID: 17139 RVA: 0x0014FF77 File Offset: 0x0014E177
	public ShaderHashId(string text)
	{
		this._text = text;
		this._hash = Shader.PropertyToID(text);
	}

	// Token: 0x060042F4 RID: 17140 RVA: 0x0014FF67 File Offset: 0x0014E167
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x060042F5 RID: 17141 RVA: 0x0014FF6F File Offset: 0x0014E16F
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x060042F6 RID: 17142 RVA: 0x0014FF6F File Offset: 0x0014E16F
	public static implicit operator int(ShaderHashId h)
	{
		return h._hash;
	}

	// Token: 0x060042F7 RID: 17143 RVA: 0x0014FF8C File Offset: 0x0014E18C
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x060042F8 RID: 17144 RVA: 0x0014FF94 File Offset: 0x0014E194
	public bool Equals(ShaderHashId other)
	{
		return this._hash == other._hash;
	}

	// Token: 0x060042F9 RID: 17145 RVA: 0x0014FFA4 File Offset: 0x0014E1A4
	public override bool Equals(object obj)
	{
		if (obj is ShaderHashId)
		{
			ShaderHashId other = (ShaderHashId)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x060042FA RID: 17146 RVA: 0x0014FFC9 File Offset: 0x0014E1C9
	public static bool operator ==(ShaderHashId x, ShaderHashId y)
	{
		return x.Equals(y);
	}

	// Token: 0x060042FB RID: 17147 RVA: 0x0014FFD3 File Offset: 0x0014E1D3
	public static bool operator !=(ShaderHashId x, ShaderHashId y)
	{
		return !x.Equals(y);
	}

	// Token: 0x04004DC9 RID: 19913
	[FormerlySerializedAs("_hashText")]
	[SerializeField]
	private string _text;

	// Token: 0x04004DCA RID: 19914
	[NonSerialized]
	private int _hash;
}
