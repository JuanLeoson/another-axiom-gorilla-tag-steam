using System;

// Token: 0x0200073D RID: 1853
[Serializable]
public struct GTSignalID : IEquatable<GTSignalID>, IEquatable<int>
{
	// Token: 0x06002E52 RID: 11858 RVA: 0x000F5544 File Offset: 0x000F3744
	public override bool Equals(object obj)
	{
		if (obj is GTSignalID)
		{
			GTSignalID other = (GTSignalID)obj;
			return this.Equals(other);
		}
		if (obj is int)
		{
			int other2 = (int)obj;
			return this.Equals(other2);
		}
		return false;
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000F5580 File Offset: 0x000F3780
	public bool Equals(GTSignalID other)
	{
		return this._id == other._id;
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000F5590 File Offset: 0x000F3790
	public bool Equals(int other)
	{
		return this._id == other;
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x000F559B File Offset: 0x000F379B
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x000F55A3 File Offset: 0x000F37A3
	public static bool operator ==(GTSignalID x, GTSignalID y)
	{
		return x.Equals(y);
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x000F55AD File Offset: 0x000F37AD
	public static bool operator !=(GTSignalID x, GTSignalID y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000F559B File Offset: 0x000F379B
	public static implicit operator int(GTSignalID sid)
	{
		return sid._id;
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000F55BC File Offset: 0x000F37BC
	public static implicit operator GTSignalID(string s)
	{
		return new GTSignalID
		{
			_id = GTSignal.ComputeID(s)
		};
	}

	// Token: 0x04003A25 RID: 14885
	private int _id;
}
