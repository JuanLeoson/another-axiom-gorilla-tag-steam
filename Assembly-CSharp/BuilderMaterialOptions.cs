using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200052E RID: 1326
[CreateAssetMenu(fileName = "BuilderMaterialOptions01a", menuName = "Gorilla Tag/Builder/Options", order = 0)]
public class BuilderMaterialOptions : ScriptableObject
{
	// Token: 0x06002051 RID: 8273 RVA: 0x000AB3B0 File Offset: 0x000A95B0
	public void GetMaterialFromType(int materialType, out Material material, out int soundIndex)
	{
		if (this.options == null)
		{
			material = null;
			soundIndex = -1;
			return;
		}
		foreach (BuilderMaterialOptions.Options options in this.options)
		{
			if (options.materialId.GetHashCode() == materialType)
			{
				material = options.material;
				soundIndex = options.soundIndex;
				return;
			}
		}
		material = null;
		soundIndex = -1;
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000AB434 File Offset: 0x000A9634
	public void GetDefaultMaterial(out int materialType, out Material material, out int soundIndex)
	{
		if (this.options.Count > 0)
		{
			materialType = this.options[0].materialId.GetHashCode();
			material = this.options[0].material;
			soundIndex = this.options[0].soundIndex;
			return;
		}
		materialType = -1;
		material = null;
		soundIndex = -1;
	}

	// Token: 0x0400291E RID: 10526
	public List<BuilderMaterialOptions.Options> options;

	// Token: 0x0200052F RID: 1327
	[Serializable]
	public class Options
	{
		// Token: 0x0400291F RID: 10527
		public string materialId;

		// Token: 0x04002920 RID: 10528
		public Material material;

		// Token: 0x04002921 RID: 10529
		[GorillaSoundLookup]
		public int soundIndex;

		// Token: 0x04002922 RID: 10530
		[NonSerialized]
		public int materialType;
	}
}
