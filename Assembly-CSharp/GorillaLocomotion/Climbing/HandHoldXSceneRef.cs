using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000E35 RID: 3637
	public class HandHoldXSceneRef : MonoBehaviour
	{
		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06005A78 RID: 23160 RVA: 0x001C949C File Offset: 0x001C769C
		public HandHold target
		{
			get
			{
				HandHold result;
				if (this.reference.TryResolve<HandHold>(out result))
				{
					return result;
				}
				return null;
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06005A79 RID: 23161 RVA: 0x001C94BC File Offset: 0x001C76BC
		public GameObject targetObject
		{
			get
			{
				GameObject result;
				if (this.reference.TryResolve(out result))
				{
					return result;
				}
				return null;
			}
		}

		// Token: 0x0400654B RID: 25931
		[SerializeField]
		public XSceneRef reference;
	}
}
