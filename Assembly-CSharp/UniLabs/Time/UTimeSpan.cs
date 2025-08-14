using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000BD4 RID: 3028
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpan : ISerializationCallbackReceiver, IComparable<UTimeSpan>, IComparable<TimeSpan>
	{
		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x0600495A RID: 18778 RVA: 0x001654E7 File Offset: 0x001636E7
		// (set) Token: 0x0600495B RID: 18779 RVA: 0x001654EF File Offset: 0x001636EF
		[JsonProperty("TimeSpan")]
		public TimeSpan TimeSpan { get; set; }

		// Token: 0x0600495C RID: 18780 RVA: 0x001654F8 File Offset: 0x001636F8
		[JsonConstructor]
		public UTimeSpan()
		{
			this.TimeSpan = TimeSpan.Zero;
		}

		// Token: 0x0600495D RID: 18781 RVA: 0x0016550B File Offset: 0x0016370B
		public UTimeSpan(TimeSpan timeSpan)
		{
			this.TimeSpan = timeSpan;
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x0016551A File Offset: 0x0016371A
		public UTimeSpan(long ticks) : this(new TimeSpan(ticks))
		{
		}

		// Token: 0x0600495F RID: 18783 RVA: 0x00165528 File Offset: 0x00163728
		public UTimeSpan(int hours, int minutes, int seconds) : this(new TimeSpan(hours, minutes, seconds))
		{
		}

		// Token: 0x06004960 RID: 18784 RVA: 0x00165538 File Offset: 0x00163738
		public UTimeSpan(int days, int hours, int minutes, int seconds) : this(new TimeSpan(days, hours, minutes, seconds))
		{
		}

		// Token: 0x06004961 RID: 18785 RVA: 0x0016554A File Offset: 0x0016374A
		public UTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) : this(new TimeSpan(days, hours, minutes, seconds, milliseconds))
		{
		}

		// Token: 0x06004962 RID: 18786 RVA: 0x0016555E File Offset: 0x0016375E
		public static implicit operator TimeSpan(UTimeSpan uTimeSpan)
		{
			if (uTimeSpan == null)
			{
				return TimeSpan.Zero;
			}
			return uTimeSpan.TimeSpan;
		}

		// Token: 0x06004963 RID: 18787 RVA: 0x0016556F File Offset: 0x0016376F
		public static implicit operator UTimeSpan(TimeSpan timeSpan)
		{
			return new UTimeSpan(timeSpan);
		}

		// Token: 0x06004964 RID: 18788 RVA: 0x00165578 File Offset: 0x00163778
		public int CompareTo(TimeSpan other)
		{
			return this.TimeSpan.CompareTo(other);
		}

		// Token: 0x06004965 RID: 18789 RVA: 0x00165594 File Offset: 0x00163794
		public int CompareTo(UTimeSpan other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return this.TimeSpan.CompareTo(other.TimeSpan);
		}

		// Token: 0x06004966 RID: 18790 RVA: 0x001655C0 File Offset: 0x001637C0
		protected bool Equals(UTimeSpan other)
		{
			return this.TimeSpan.Equals(other.TimeSpan);
		}

		// Token: 0x06004967 RID: 18791 RVA: 0x001655E1 File Offset: 0x001637E1
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UTimeSpan)obj)));
		}

		// Token: 0x06004968 RID: 18792 RVA: 0x00165610 File Offset: 0x00163810
		public override int GetHashCode()
		{
			return this.TimeSpan.GetHashCode();
		}

		// Token: 0x06004969 RID: 18793 RVA: 0x00165634 File Offset: 0x00163834
		public void OnAfterDeserialize()
		{
			TimeSpan timeSpan;
			this.TimeSpan = (TimeSpan.TryParse(this._TimeSpan, CultureInfo.InvariantCulture, out timeSpan) ? timeSpan : TimeSpan.Zero);
		}

		// Token: 0x0600496A RID: 18794 RVA: 0x00165664 File Offset: 0x00163864
		public void OnBeforeSerialize()
		{
			this._TimeSpan = this.TimeSpan.ToString();
		}

		// Token: 0x0600496B RID: 18795 RVA: 0x0016568B File Offset: 0x0016388B
		[OnSerializing]
		internal void OnSerializingMethod(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x0600496C RID: 18796 RVA: 0x00165693 File Offset: 0x00163893
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x04005278 RID: 21112
		[HideInInspector]
		[SerializeField]
		private string _TimeSpan;
	}
}
