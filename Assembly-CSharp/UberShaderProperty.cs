using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007CA RID: 1994
[Serializable]
public class UberShaderProperty
{
	// Token: 0x060031F6 RID: 12790 RVA: 0x00104408 File Offset: 0x00102608
	public T GetValue<T>(Material target)
	{
		switch (this.type)
		{
		case ShaderPropertyType.Color:
			return UberShaderProperty.ValueAs<Color, T>(target.GetColor(this.nameID));
		case ShaderPropertyType.Vector:
			return UberShaderProperty.ValueAs<Vector4, T>(target.GetVector(this.nameID));
		case ShaderPropertyType.Float:
		case ShaderPropertyType.Range:
			return UberShaderProperty.ValueAs<float, T>(target.GetFloat(this.nameID));
		case ShaderPropertyType.Texture:
			return UberShaderProperty.ValueAs<Texture, T>(target.GetTexture(this.nameID));
		case ShaderPropertyType.Int:
			return UberShaderProperty.ValueAs<int, T>(target.GetInt(this.nameID));
		default:
			return default(T);
		}
	}

	// Token: 0x060031F7 RID: 12791 RVA: 0x001044A0 File Offset: 0x001026A0
	public void SetValue<T>(Material target, T value)
	{
		switch (this.type)
		{
		case ShaderPropertyType.Color:
			target.SetColor(this.nameID, UberShaderProperty.ValueAs<T, Color>(value));
			break;
		case ShaderPropertyType.Vector:
			target.SetVector(this.nameID, UberShaderProperty.ValueAs<T, Vector4>(value));
			break;
		case ShaderPropertyType.Float:
		case ShaderPropertyType.Range:
			target.SetFloat(this.nameID, UberShaderProperty.ValueAs<T, float>(value));
			break;
		case ShaderPropertyType.Texture:
			target.SetTexture(this.nameID, UberShaderProperty.ValueAs<T, Texture>(value));
			break;
		case ShaderPropertyType.Int:
			target.SetInt(this.nameID, UberShaderProperty.ValueAs<T, int>(value));
			break;
		}
		if (!this.isKeywordToggle)
		{
			return;
		}
		bool flag = false;
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != ShaderPropertyType.Float)
		{
			if (shaderPropertyType == ShaderPropertyType.Int)
			{
				flag = (UberShaderProperty.ValueAs<T, int>(value) >= 1);
			}
		}
		else
		{
			flag = (UberShaderProperty.ValueAs<T, float>(value) >= 0.5f);
		}
		if (flag)
		{
			target.EnableKeyword(this.keyword);
			return;
		}
		target.DisableKeyword(this.keyword);
	}

	// Token: 0x060031F8 RID: 12792 RVA: 0x0010458C File Offset: 0x0010278C
	public void Enable(Material target)
	{
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != ShaderPropertyType.Float)
		{
			if (shaderPropertyType == ShaderPropertyType.Int)
			{
				target.SetInt(this.nameID, 1);
			}
		}
		else
		{
			target.SetFloat(this.nameID, 1f);
		}
		if (this.isKeywordToggle)
		{
			target.EnableKeyword(this.keyword);
		}
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x001045DC File Offset: 0x001027DC
	public void Disable(Material target)
	{
		ShaderPropertyType shaderPropertyType = this.type;
		if (shaderPropertyType != ShaderPropertyType.Float)
		{
			if (shaderPropertyType == ShaderPropertyType.Int)
			{
				target.SetInt(this.nameID, 0);
			}
		}
		else
		{
			target.SetFloat(this.nameID, 0f);
		}
		if (this.isKeywordToggle)
		{
			target.DisableKeyword(this.keyword);
		}
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x0010462C File Offset: 0x0010282C
	public bool TryGetKeywordState(Material target, out bool enabled)
	{
		enabled = false;
		if (!this.isKeywordToggle)
		{
			return false;
		}
		enabled = target.IsKeywordEnabled(this.keyword);
		return true;
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x0010464A File Offset: 0x0010284A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static TOut ValueAs<TIn, TOut>(TIn value)
	{
		return *Unsafe.As<TIn, TOut>(ref value);
	}

	// Token: 0x04003E61 RID: 15969
	public int index;

	// Token: 0x04003E62 RID: 15970
	public int nameID;

	// Token: 0x04003E63 RID: 15971
	public string name;

	// Token: 0x04003E64 RID: 15972
	public ShaderPropertyType type;

	// Token: 0x04003E65 RID: 15973
	public ShaderPropertyFlags flags;

	// Token: 0x04003E66 RID: 15974
	public Vector2 rangeLimits;

	// Token: 0x04003E67 RID: 15975
	public string[] attributes;

	// Token: 0x04003E68 RID: 15976
	public bool isKeywordToggle;

	// Token: 0x04003E69 RID: 15977
	public string keyword;
}
