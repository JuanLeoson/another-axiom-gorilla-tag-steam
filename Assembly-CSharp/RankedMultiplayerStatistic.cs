using System;

// Token: 0x02000787 RID: 1927
public abstract class RankedMultiplayerStatistic
{
	// Token: 0x06003058 RID: 12376 RVA: 0x00032575 File Offset: 0x00030775
	public override string ToString()
	{
		return string.Empty;
	}

	// Token: 0x06003059 RID: 12377
	public abstract void Load();

	// Token: 0x0600305A RID: 12378
	protected abstract void Save();

	// Token: 0x0600305B RID: 12379
	public abstract bool TrySetValue(string valAsString);

	// Token: 0x0600305C RID: 12380 RVA: 0x000FE1DB File Offset: 0x000FC3DB
	public virtual string WriteToJson()
	{
		return string.Format("{{{0}:\"{1}\"}}", this.name, this.ToString());
	}

	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x0600305D RID: 12381 RVA: 0x000FE1F3 File Offset: 0x000FC3F3
	// (set) Token: 0x0600305E RID: 12382 RVA: 0x000FE1FB File Offset: 0x000FC3FB
	public bool IsValid { get; protected set; }

	// Token: 0x0600305F RID: 12383 RVA: 0x000FE204 File Offset: 0x000FC404
	public RankedMultiplayerStatistic(string n, RankedMultiplayerStatistic.SerializationType sType = RankedMultiplayerStatistic.SerializationType.Mothership)
	{
		this.serializationType = sType;
		this.name = n;
		this.IsValid = (this.serializationType != RankedMultiplayerStatistic.SerializationType.Mothership);
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x000FE23C File Offset: 0x000FC43C
	protected virtual void HandleUserDataSetSuccess(string keyName)
	{
		if (keyName == this.name)
		{
			this.IsValid = true;
		}
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x000FE253 File Offset: 0x000FC453
	protected virtual void HandleUserDataGetSuccess(string keyName, string value)
	{
		if (keyName == this.name)
		{
			if (this.TrySetValue(value))
			{
				this.IsValid = true;
				return;
			}
			this.Save();
		}
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x000FE27A File Offset: 0x000FC47A
	protected void HandleUserDataGetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
			this.IsValid = true;
		}
	}

	// Token: 0x06003063 RID: 12387 RVA: 0x000FE297 File Offset: 0x000FC497
	protected void HandleUserDataSetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
		}
	}

	// Token: 0x04003C55 RID: 15445
	protected RankedMultiplayerStatistic.SerializationType serializationType = RankedMultiplayerStatistic.SerializationType.Mothership;

	// Token: 0x04003C56 RID: 15446
	public string name;

	// Token: 0x02000788 RID: 1928
	public enum SerializationType
	{
		// Token: 0x04003C59 RID: 15449
		None,
		// Token: 0x04003C5A RID: 15450
		Mothership,
		// Token: 0x04003C5B RID: 15451
		PlayerPrefs
	}
}
