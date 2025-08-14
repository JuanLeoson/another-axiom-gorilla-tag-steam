using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E4 RID: 1764
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x06002BE5 RID: 11237 RVA: 0x000E8CB2 File Offset: 0x000E6EB2
	private void Awake()
	{
		if (this.myText != null)
		{
			this.startingText = this.myText.text;
		}
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x000E8CD4 File Offset: 0x000E6ED4
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x0400374B RID: 14155
	public string startingText;

	// Token: 0x0400374C RID: 14156
	public Material goodMaterial;

	// Token: 0x0400374D RID: 14157
	public Material badMaterial;

	// Token: 0x0400374E RID: 14158
	public Text myText;
}
