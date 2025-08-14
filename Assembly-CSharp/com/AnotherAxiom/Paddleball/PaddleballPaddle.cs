using System;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000DF6 RID: 3574
	public class PaddleballPaddle : MonoBehaviour
	{
		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06005886 RID: 22662 RVA: 0x001B88F6 File Offset: 0x001B6AF6
		public bool Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04006264 RID: 25188
		[SerializeField]
		private bool right;
	}
}
