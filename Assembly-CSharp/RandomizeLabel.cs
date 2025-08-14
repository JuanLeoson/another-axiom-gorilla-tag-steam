using System;
using TMPro;
using UnityEngine;

// Token: 0x020004A5 RID: 1189
public class RandomizeLabel : MonoBehaviour
{
	// Token: 0x06001D62 RID: 7522 RVA: 0x0009D780 File Offset: 0x0009B980
	public void Randomize()
	{
		this.strings.distinct = this.distinct;
		this.label.text = this.strings.NextItem();
	}

	// Token: 0x040025DB RID: 9691
	public TMP_Text label;

	// Token: 0x040025DC RID: 9692
	public RandomStrings strings;

	// Token: 0x040025DD RID: 9693
	public bool distinct;
}
