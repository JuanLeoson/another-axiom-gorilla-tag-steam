using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GorillaTag
{
	// Token: 0x02000E51 RID: 3665
	[Serializable]
	public class GTAssetRef<TObject> : AssetReferenceT<TObject> where TObject : Object
	{
		// Token: 0x06005C03 RID: 23555 RVA: 0x001CFE00 File Offset: 0x001CE000
		public GTAssetRef(string guid) : base(guid)
		{
		}
	}
}
