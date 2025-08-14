using System;

namespace BoingKit
{
	// Token: 0x02000FC9 RID: 4041
	public struct Version : IEquatable<Version>
	{
		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x060064FE RID: 25854 RVA: 0x002011F8 File Offset: 0x001FF3F8
		public readonly int MajorVersion { get; }

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x060064FF RID: 25855 RVA: 0x00201200 File Offset: 0x001FF400
		public readonly int MinorVersion { get; }

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x06006500 RID: 25856 RVA: 0x00201208 File Offset: 0x001FF408
		public readonly int Revision { get; }

		// Token: 0x06006501 RID: 25857 RVA: 0x00201210 File Offset: 0x001FF410
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.MajorVersion.ToString(),
				".",
				this.MinorVersion.ToString(),
				".",
				this.Revision.ToString()
			});
		}

		// Token: 0x06006502 RID: 25858 RVA: 0x0020126B File Offset: 0x001FF46B
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x06006503 RID: 25859 RVA: 0x0020128D File Offset: 0x001FF48D
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x06006504 RID: 25860 RVA: 0x002012A4 File Offset: 0x001FF4A4
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x06006505 RID: 25861 RVA: 0x002012F9 File Offset: 0x001FF4F9
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x06006506 RID: 25862 RVA: 0x00201305 File Offset: 0x001FF505
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x06006507 RID: 25863 RVA: 0x0020131D File Offset: 0x001FF51D
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x06006508 RID: 25864 RVA: 0x00201350 File Offset: 0x001FF550
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x04007006 RID: 28678
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x04007007 RID: 28679
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x04007008 RID: 28680
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
