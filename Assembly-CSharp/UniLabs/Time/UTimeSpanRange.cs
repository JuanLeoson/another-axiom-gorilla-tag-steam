using System;
using Newtonsoft.Json;
using UnityEngine;

namespace UniLabs.Time
{
	// Token: 0x02000BD5 RID: 3029
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public class UTimeSpanRange
	{
		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x0600496D RID: 18797 RVA: 0x0016569B File Offset: 0x0016389B
		// (set) Token: 0x0600496E RID: 18798 RVA: 0x001656A8 File Offset: 0x001638A8
		public TimeSpan Start
		{
			get
			{
				return this._Start;
			}
			set
			{
				this._Start = value;
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x0600496F RID: 18799 RVA: 0x001656B6 File Offset: 0x001638B6
		// (set) Token: 0x06004970 RID: 18800 RVA: 0x001656C3 File Offset: 0x001638C3
		public TimeSpan End
		{
			get
			{
				return this._End;
			}
			set
			{
				this._End = value;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06004971 RID: 18801 RVA: 0x001656D1 File Offset: 0x001638D1
		public TimeSpan Duration
		{
			get
			{
				return this.End - this.Start;
			}
		}

		// Token: 0x06004972 RID: 18802 RVA: 0x001656E4 File Offset: 0x001638E4
		public bool IsInRange(TimeSpan time)
		{
			return time >= this.Start && time <= this.End;
		}

		// Token: 0x06004973 RID: 18803 RVA: 0x00002050 File Offset: 0x00000250
		[JsonConstructor]
		public UTimeSpanRange()
		{
		}

		// Token: 0x06004974 RID: 18804 RVA: 0x00165702 File Offset: 0x00163902
		public UTimeSpanRange(TimeSpan start)
		{
			this._Start = start;
			this._End = start;
		}

		// Token: 0x06004975 RID: 18805 RVA: 0x00165722 File Offset: 0x00163922
		public UTimeSpanRange(TimeSpan start, TimeSpan end)
		{
			this._Start = start;
			this._End = end;
		}

		// Token: 0x06004976 RID: 18806 RVA: 0x00165742 File Offset: 0x00163942
		private void OnStartChanged()
		{
			if (this._Start.CompareTo(this._End) > 0)
			{
				this._End.TimeSpan = this._Start.TimeSpan;
			}
		}

		// Token: 0x06004977 RID: 18807 RVA: 0x0016576E File Offset: 0x0016396E
		private void OnEndChanged()
		{
			if (this._End.CompareTo(this._Start) < 0)
			{
				this._Start.TimeSpan = this._End.TimeSpan;
			}
		}

		// Token: 0x04005279 RID: 21113
		[JsonProperty("Start")]
		[SerializeField]
		private UTimeSpan _Start;

		// Token: 0x0400527A RID: 21114
		[JsonProperty("End")]
		[SerializeField]
		private UTimeSpan _End;
	}
}
