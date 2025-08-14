using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E54 RID: 3668
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x06005C15 RID: 23573 RVA: 0x001D0100 File Offset: 0x001CE300
		protected void Awake()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x06005C16 RID: 23574 RVA: 0x001D0130 File Offset: 0x001CE330
		protected void OnEnable()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x06005C17 RID: 23575 RVA: 0x001D0160 File Offset: 0x001CE360
		protected void Update()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}
	}
}
