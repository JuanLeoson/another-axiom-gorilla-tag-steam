using System;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class RuntimeMaterialCombinerTargetMono : MonoBehaviour
{
	// Token: 0x06000D3A RID: 3386 RVA: 0x00047F8E File Offset: 0x0004618E
	protected void Awake()
	{
		throw new NotImplementedException("// TODO: get the material combiner manager to fingerprint and combine these materials.");
	}

	// Token: 0x040010CA RID: 4298
	[HideInInspector]
	public GTSerializableDict<string, string>[] m_matSlot_to_texProp_to_texGuid;
}
