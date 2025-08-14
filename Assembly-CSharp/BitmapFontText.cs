using System;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public class BitmapFontText : MonoBehaviour
{
	// Token: 0x06001808 RID: 6152 RVA: 0x0008089E File Offset: 0x0007EA9E
	private void Awake()
	{
		this.Init();
		this.Render();
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x000808AC File Offset: 0x0007EAAC
	public void Render()
	{
		this.font.RenderToTexture(this.texture, this.uppercaseOnly ? this.text.ToUpperInvariant() : this.text);
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x000808DC File Offset: 0x0007EADC
	public void Init()
	{
		this.texture = new Texture2D(this.textArea.x, this.textArea.y, this.font.fontImage.format, false);
		this.texture.filterMode = FilterMode.Point;
		this.material = new Material(this.renderer.sharedMaterial);
		this.material.mainTexture = this.texture;
		this.renderer.sharedMaterial = this.material;
	}

	// Token: 0x04001FE2 RID: 8162
	public string text;

	// Token: 0x04001FE3 RID: 8163
	public bool uppercaseOnly;

	// Token: 0x04001FE4 RID: 8164
	public Vector2Int textArea;

	// Token: 0x04001FE5 RID: 8165
	[Space]
	public Renderer renderer;

	// Token: 0x04001FE6 RID: 8166
	public Texture2D texture;

	// Token: 0x04001FE7 RID: 8167
	public Material material;

	// Token: 0x04001FE8 RID: 8168
	public BitmapFont font;
}
