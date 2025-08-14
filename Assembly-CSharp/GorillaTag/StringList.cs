using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E74 RID: 3700
	[CreateAssetMenu(fileName = "New String List", menuName = "String List")]
	public class StringList : ScriptableObject
	{
		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06005C7E RID: 23678 RVA: 0x001D19D2 File Offset: 0x001CFBD2
		public string[] Strings
		{
			get
			{
				return this.strings;
			}
		}

		// Token: 0x0400662F RID: 26159
		[SerializeField]
		private string[] strings;
	}
}
