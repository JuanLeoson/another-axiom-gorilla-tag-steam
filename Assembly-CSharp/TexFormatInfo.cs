using System;
using UnityEngine;

// Token: 0x0200023C RID: 572
public struct TexFormatInfo
{
	// Token: 0x06000D49 RID: 3401 RVA: 0x00052518 File Offset: 0x00050718
	public TexFormatInfo(Texture2D tex2d)
	{
		this.width = tex2d.width;
		this.height = tex2d.height;
		this.format = tex2d.format;
		this.filterMode = tex2d.filterMode;
		this.isLinearColor = !tex2d.isDataSRGB;
		this.mipmapCount = tex2d.mipmapCount;
		this.isValid = true;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00052578 File Offset: 0x00050778
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"TexFormatInfo(isValid: ",
			this.isValid.ToString(),
			", width: ",
			this.width.ToString(),
			", height: ",
			this.height.ToString(),
			", format: ",
			this.format.ToString(),
			", filterMode: ",
			this.filterMode.ToString(),
			", isLinearColor: ",
			this.isLinearColor.ToString(),
			", mipmapCount: ",
			this.mipmapCount.ToString(),
			")"
		});
	}

	// Token: 0x0400154E RID: 5454
	public bool isValid;

	// Token: 0x0400154F RID: 5455
	public int width;

	// Token: 0x04001550 RID: 5456
	public int height;

	// Token: 0x04001551 RID: 5457
	public TextureFormat format;

	// Token: 0x04001552 RID: 5458
	public FilterMode filterMode;

	// Token: 0x04001553 RID: 5459
	public int mipmapCount;

	// Token: 0x04001554 RID: 5460
	public bool isLinearColor;
}
