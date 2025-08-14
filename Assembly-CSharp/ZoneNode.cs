using System;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000B61 RID: 2913
[Serializable]
public struct ZoneNode : IEquatable<ZoneNode>
{
	// Token: 0x1700069D RID: 1693
	// (get) Token: 0x060045CD RID: 17869 RVA: 0x0015C42F File Offset: 0x0015A62F
	public static ZoneNode Null { get; } = new ZoneNode
	{
		zoneId = GTZone.none,
		subZoneId = GTSubZone.none,
		isValid = false
	};

	// Token: 0x1700069E RID: 1694
	// (get) Token: 0x060045CE RID: 17870 RVA: 0x0015C436 File Offset: 0x0015A636
	public int zoneKey
	{
		get
		{
			return StaticHash.Compute((int)this.zoneId, (int)this.subZoneId);
		}
	}

	// Token: 0x060045CF RID: 17871 RVA: 0x0015C449 File Offset: 0x0015A649
	public bool ContainsPoint(Vector3 point)
	{
		return MathUtils.OrientedBoxContains(point, this.center, this.size, this.orientation);
	}

	// Token: 0x060045D0 RID: 17872 RVA: 0x0015C463 File Offset: 0x0015A663
	public int SphereOverlap(Vector3 position, float radius)
	{
		return MathUtils.OrientedBoxSphereOverlap(position, radius, this.center, this.size, this.orientation);
	}

	// Token: 0x060045D1 RID: 17873 RVA: 0x0015C47E File Offset: 0x0015A67E
	public override string ToString()
	{
		if (this.subZoneId != GTSubZone.none)
		{
			return ZString.Concat<GTZone, string, GTSubZone>(this.zoneId, ".", this.subZoneId);
		}
		return ZString.Concat<GTZone>(this.zoneId);
	}

	// Token: 0x060045D2 RID: 17874 RVA: 0x0015C4AC File Offset: 0x0015A6AC
	public override int GetHashCode()
	{
		int zoneKey = this.zoneKey;
		int hashCode = this.center.QuantizedId128().GetHashCode();
		int hashCode2 = this.size.QuantizedId128().GetHashCode();
		int hashCode3 = this.orientation.QuantizedId128().GetHashCode();
		return StaticHash.Compute(zoneKey, hashCode, hashCode2, hashCode3);
	}

	// Token: 0x060045D3 RID: 17875 RVA: 0x0015C515 File Offset: 0x0015A715
	public static bool operator ==(ZoneNode x, ZoneNode y)
	{
		return x.Equals(y);
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x0015C51F File Offset: 0x0015A71F
	public static bool operator !=(ZoneNode x, ZoneNode y)
	{
		return !x.Equals(y);
	}

	// Token: 0x060045D5 RID: 17877 RVA: 0x0015C52C File Offset: 0x0015A72C
	public bool Equals(ZoneNode other)
	{
		return this.zoneId == other.zoneId && this.subZoneId == other.subZoneId && this.center.Approx(other.center, 1E-05f) && this.size.Approx(other.size, 1E-05f) && this.orientation.Approx(other.orientation, 1E-06f);
	}

	// Token: 0x060045D6 RID: 17878 RVA: 0x0015C5A0 File Offset: 0x0015A7A0
	public override bool Equals(object obj)
	{
		if (obj is ZoneNode)
		{
			ZoneNode other = (ZoneNode)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x040050CA RID: 20682
	public GTZone zoneId;

	// Token: 0x040050CB RID: 20683
	public GTSubZone subZoneId;

	// Token: 0x040050CC RID: 20684
	public Vector3 center;

	// Token: 0x040050CD RID: 20685
	public Vector3 size;

	// Token: 0x040050CE RID: 20686
	public Quaternion orientation;

	// Token: 0x040050CF RID: 20687
	public Bounds AABB;

	// Token: 0x040050D0 RID: 20688
	public bool isValid;
}
