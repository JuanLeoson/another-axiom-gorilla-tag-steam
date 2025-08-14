using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000402 RID: 1026
public class BitmapFont : ScriptableObject
{
	// Token: 0x06001801 RID: 6145 RVA: 0x000806F0 File Offset: 0x0007E8F0
	private void OnEnable()
	{
		this._charToSymbol = this.symbols.ToDictionary((BitmapFont.SymbolData s) => s.character, (BitmapFont.SymbolData s) => s);
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x0008074C File Offset: 0x0007E94C
	public void RenderToTexture(Texture2D target, string text)
	{
		if (text == null)
		{
			text = string.Empty;
		}
		int num = target.width * target.height;
		if (this._empty.Length != num)
		{
			this._empty = new Color[num];
			for (int i = 0; i < this._empty.Length; i++)
			{
				this._empty[i] = Color.black;
			}
		}
		target.SetPixels(this._empty);
		int length = text.Length;
		int num2 = 1;
		int width = this.fontImage.width;
		int height = this.fontImage.height;
		for (int j = 0; j < length; j++)
		{
			char key = text[j];
			BitmapFont.SymbolData symbolData = this._charToSymbol[key];
			int width2 = symbolData.width;
			int height2 = symbolData.height;
			int x = symbolData.x;
			int y = symbolData.y;
			Graphics.CopyTexture(this.fontImage, 0, 0, x, height - (y + height2), width2, height2, target, 0, 0, num2, 2 + symbolData.yoffset);
			num2 += width2 + 1;
		}
		target.Apply(false);
	}

	// Token: 0x04001FD0 RID: 8144
	public Texture2D fontImage;

	// Token: 0x04001FD1 RID: 8145
	public TextAsset fontJson;

	// Token: 0x04001FD2 RID: 8146
	public int symbolPixelsPerUnit = 1;

	// Token: 0x04001FD3 RID: 8147
	public string characterMap;

	// Token: 0x04001FD4 RID: 8148
	[Space]
	public BitmapFont.SymbolData[] symbols = new BitmapFont.SymbolData[0];

	// Token: 0x04001FD5 RID: 8149
	private Dictionary<char, BitmapFont.SymbolData> _charToSymbol;

	// Token: 0x04001FD6 RID: 8150
	private Color[] _empty = new Color[0];

	// Token: 0x02000403 RID: 1027
	[Serializable]
	public struct SymbolData
	{
		// Token: 0x04001FD7 RID: 8151
		public char character;

		// Token: 0x04001FD8 RID: 8152
		[Space]
		public int id;

		// Token: 0x04001FD9 RID: 8153
		public int width;

		// Token: 0x04001FDA RID: 8154
		public int height;

		// Token: 0x04001FDB RID: 8155
		public int x;

		// Token: 0x04001FDC RID: 8156
		public int y;

		// Token: 0x04001FDD RID: 8157
		public int xadvance;

		// Token: 0x04001FDE RID: 8158
		public int yoffset;
	}
}
