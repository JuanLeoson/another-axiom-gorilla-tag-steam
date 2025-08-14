using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E65 RID: 3685
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x06005C47 RID: 23623 RVA: 0x001D0781 File Offset: 0x001CE981
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
