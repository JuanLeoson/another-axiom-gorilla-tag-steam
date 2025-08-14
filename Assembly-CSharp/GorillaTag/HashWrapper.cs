using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E62 RID: 3682
	[Serializable]
	public struct HashWrapper : IEquatable<int>
	{
		// Token: 0x06005C38 RID: 23608 RVA: 0x001D045B File Offset: 0x001CE65B
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		// Token: 0x06005C39 RID: 23609 RVA: 0x001D0463 File Offset: 0x001CE663
		public override bool Equals(object obj)
		{
			return this.hashCode.Equals(obj);
		}

		// Token: 0x06005C3A RID: 23610 RVA: 0x001D0471 File Offset: 0x001CE671
		public bool Equals(int i)
		{
			return this.hashCode.Equals(i);
		}

		// Token: 0x06005C3B RID: 23611 RVA: 0x001D045B File Offset: 0x001CE65B
		public static implicit operator int(in HashWrapper hash)
		{
			return hash.hashCode;
		}

		// Token: 0x040065DD RID: 26077
		[SerializeField]
		private int hashCode;
	}
}
