using System;

// Token: 0x02000892 RID: 2194
public struct TimeSince
{
	// Token: 0x17000540 RID: 1344
	// (get) Token: 0x06003734 RID: 14132 RVA: 0x0011F200 File Offset: 0x0011D400
	public double secondsElapsed
	{
		get
		{
			double totalSeconds = (DateTime.UtcNow - this._dt).TotalSeconds;
			if (totalSeconds <= 2147483647.0)
			{
				return totalSeconds;
			}
			return 2147483647.0;
		}
	}

	// Token: 0x17000541 RID: 1345
	// (get) Token: 0x06003735 RID: 14133 RVA: 0x0011F23D File Offset: 0x0011D43D
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x17000542 RID: 1346
	// (get) Token: 0x06003736 RID: 14134 RVA: 0x0011F246 File Offset: 0x0011D446
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x17000543 RID: 1347
	// (get) Token: 0x06003737 RID: 14135 RVA: 0x0011F24F File Offset: 0x0011D44F
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x17000544 RID: 1348
	// (get) Token: 0x06003738 RID: 14136 RVA: 0x0011F258 File Offset: 0x0011D458
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x17000545 RID: 1349
	// (get) Token: 0x06003739 RID: 14137 RVA: 0x0011F261 File Offset: 0x0011D461
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x0011F26E File Offset: 0x0011D46E
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x0011F278 File Offset: 0x0011D478
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x0011F29C File Offset: 0x0011D49C
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x0600373D RID: 14141 RVA: 0x0011F2CC File Offset: 0x0011D4CC
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x0011F2F0 File Offset: 0x0011D4F0
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x0011F314 File Offset: 0x0011D514
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x0011F338 File Offset: 0x0011D538
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x0011F35E File Offset: 0x0011D55E
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x0011F36C File Offset: 0x0011D56C
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x0011F37A File Offset: 0x0011D57A
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x0011F388 File Offset: 0x0011D588
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x0011F396 File Offset: 0x0011D596
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x0011F3A4 File Offset: 0x0011D5A4
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x0011F3B2 File Offset: 0x0011D5B2
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x0011F3BF File Offset: 0x0011D5BF
	public bool HasElapsed(int seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedInt >= seconds;
		}
		if (this.secondsElapsedInt < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x0011F3E3 File Offset: 0x0011D5E3
	public bool HasElapsed(uint seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedUint >= seconds;
		}
		if (this.secondsElapsedUint < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x0011F407 File Offset: 0x0011D607
	public bool HasElapsed(float seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedFloat >= seconds;
		}
		if (this.secondsElapsedFloat < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600374B RID: 14155 RVA: 0x0011F42B File Offset: 0x0011D62B
	public bool HasElapsed(double seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsed >= seconds;
		}
		if (this.secondsElapsed < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600374C RID: 14156 RVA: 0x0011F44F File Offset: 0x0011D64F
	public bool HasElapsed(long seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedLong >= seconds;
		}
		if (this.secondsElapsedLong < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600374D RID: 14157 RVA: 0x0011F473 File Offset: 0x0011D673
	public bool HasElapsed(TimeSpan seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedSpan >= seconds;
		}
		if (this.secondsElapsedSpan < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x0011F49C File Offset: 0x0011D69C
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x0600374F RID: 14159 RVA: 0x0011F4BE File Offset: 0x0011D6BE
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x0011F4CB File Offset: 0x0011D6CB
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x0011F4D7 File Offset: 0x0011D6D7
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x0011F4E0 File Offset: 0x0011D6E0
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x0011F4E9 File Offset: 0x0011D6E9
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0011F4F2 File Offset: 0x0011D6F2
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x0011F4FB File Offset: 0x0011D6FB
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x0011F504 File Offset: 0x0011D704
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x0011F50D File Offset: 0x0011D70D
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x0011F515 File Offset: 0x0011D715
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x0011F51D File Offset: 0x0011D71D
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x0011F525 File Offset: 0x0011D725
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x0011F52D File Offset: 0x0011D72D
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x0011F535 File Offset: 0x0011D735
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x0011F53D File Offset: 0x0011D73D
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x040043DB RID: 17371
	private DateTime _dt;

	// Token: 0x040043DC RID: 17372
	private const double INT32_MAX = 2147483647.0;
}
