using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000BD3 RID: 3027
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UDateTime : ISerializationCallbackReceiver, IComparable<UDateTime>, IComparable<DateTime>
	{
		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x0600494A RID: 18762 RVA: 0x0016535B File Offset: 0x0016355B
		// (set) Token: 0x0600494B RID: 18763 RVA: 0x00165363 File Offset: 0x00163563
		[JsonProperty("DateTime")]
		public DateTime DateTime { get; set; }

		// Token: 0x0600494C RID: 18764 RVA: 0x0016536C File Offset: 0x0016356C
		[JsonConstructor]
		public UDateTime()
		{
			this.DateTime = DateTime.UnixEpoch;
		}

		// Token: 0x0600494D RID: 18765 RVA: 0x0016537F File Offset: 0x0016357F
		public UDateTime(DateTime dateTime)
		{
			this.DateTime = dateTime;
		}

		// Token: 0x0600494E RID: 18766 RVA: 0x0016538E File Offset: 0x0016358E
		public static implicit operator DateTime(UDateTime udt)
		{
			return udt.DateTime;
		}

		// Token: 0x0600494F RID: 18767 RVA: 0x00165396 File Offset: 0x00163596
		public static implicit operator UDateTime(DateTime dt)
		{
			return new UDateTime
			{
				DateTime = dt
			};
		}

		// Token: 0x06004950 RID: 18768 RVA: 0x001653A4 File Offset: 0x001635A4
		public int CompareTo(DateTime other)
		{
			return this.DateTime.CompareTo(other);
		}

		// Token: 0x06004951 RID: 18769 RVA: 0x001653C0 File Offset: 0x001635C0
		public int CompareTo(UDateTime other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.DateTime.CompareTo(other.DateTime);
		}

		// Token: 0x06004952 RID: 18770 RVA: 0x001653EC File Offset: 0x001635EC
		protected bool Equals(UDateTime other)
		{
			return this.DateTime.Equals(other.DateTime);
		}

		// Token: 0x06004953 RID: 18771 RVA: 0x0016540D File Offset: 0x0016360D
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UDateTime)obj)));
		}

		// Token: 0x06004954 RID: 18772 RVA: 0x0016543C File Offset: 0x0016363C
		public override int GetHashCode()
		{
			return this.DateTime.GetHashCode();
		}

		// Token: 0x06004955 RID: 18773 RVA: 0x00165458 File Offset: 0x00163658
		public override string ToString()
		{
			return this.DateTime.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06004956 RID: 18774 RVA: 0x00165478 File Offset: 0x00163678
		public void OnAfterDeserialize()
		{
			DateTime dateTime;
			this.DateTime = (DateTime.TryParse(this._DateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime) ? dateTime : DateTime.MinValue);
		}

		// Token: 0x06004957 RID: 18775 RVA: 0x001654AC File Offset: 0x001636AC
		public void OnBeforeSerialize()
		{
			this._DateTime = this.DateTime.ToString("o", CultureInfo.InvariantCulture);
		}

		// Token: 0x06004958 RID: 18776 RVA: 0x001654D7 File Offset: 0x001636D7
		[OnSerializing]
		internal void OnSerializing(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x06004959 RID: 18777 RVA: 0x001654DF File Offset: 0x001636DF
		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04005276 RID: 21110
		[HideInInspector]
		[SerializeField]
		private string _DateTime;
	}
}
