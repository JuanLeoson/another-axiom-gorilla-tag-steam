using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000453 RID: 1107
[Serializable]
public class AdvancedItemState
{
	// Token: 0x06001B43 RID: 6979 RVA: 0x00092242 File Offset: 0x00090442
	public void Encode()
	{
		this._encodedValue = this.EncodeData();
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x00092250 File Offset: 0x00090450
	public void Decode()
	{
		AdvancedItemState advancedItemState = this.DecodeData(this._encodedValue);
		this.index = advancedItemState.index;
		this.preData = advancedItemState.preData;
		this.limitAxis = advancedItemState.limitAxis;
		this.reverseGrip = advancedItemState.reverseGrip;
		this.angle = advancedItemState.angle;
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x000922A8 File Offset: 0x000904A8
	public Quaternion GetQuaternion()
	{
		Vector3 one = Vector3.one;
		if (this.reverseGrip)
		{
			switch (this.limitAxis)
			{
			case LimitAxis.NoMovement:
				return Quaternion.identity;
			case LimitAxis.YAxis:
				return Quaternion.identity;
			case LimitAxis.XAxis:
			case LimitAxis.ZAxis:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Quaternion.identity;
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x000922FC File Offset: 0x000904FC
	[return: TupleElementNames(new string[]
	{
		"grabPointIndex",
		"YRotation",
		"XRotation",
		"ZRotation"
	})]
	public ValueTuple<int, float, float, float> DecodeAdvancedItemState(int encodedValue)
	{
		int item = encodedValue >> 21 & 255;
		float item2 = (float)(encodedValue >> 14 & 127) / 128f * 360f;
		float item3 = (float)(encodedValue >> 7 & 127) / 128f * 360f;
		float item4 = (float)(encodedValue & 127) / 128f * 360f;
		return new ValueTuple<int, float, float, float>(item, item2, item3, item4);
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001B47 RID: 6983 RVA: 0x00092356 File Offset: 0x00090556
	private float EncodedDeltaRotation
	{
		get
		{
			return this.GetEncodedDeltaRotation();
		}
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x0009235E File Offset: 0x0009055E
	public float GetEncodedDeltaRotation()
	{
		return Mathf.Abs(Mathf.Atan2(this.angleVectorWhereUpIsStandard.x, this.angleVectorWhereUpIsStandard.y)) / 3.1415927f;
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x00092388 File Offset: 0x00090588
	public void DecodeDeltaRotation(float encodedDelta, bool isFlipped)
	{
		float f = encodedDelta * 3.1415927f;
		if (isFlipped)
		{
			this.angleVectorWhereUpIsStandard = new Vector2(-Mathf.Sin(f), Mathf.Cos(f));
		}
		else
		{
			this.angleVectorWhereUpIsStandard = new Vector2(Mathf.Sin(f), Mathf.Cos(f));
		}
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
		case LimitAxis.XAxis:
		case LimitAxis.ZAxis:
			return;
		case LimitAxis.YAxis:
		{
			Vector3 forward = new Vector3(this.angleVectorWhereUpIsStandard.x, 0f, this.angleVectorWhereUpIsStandard.y);
			Vector3 upwards = this.reverseGrip ? Vector3.down : Vector3.up;
			this.deltaRotation = Quaternion.LookRotation(forward, upwards);
			return;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x0009243C File Offset: 0x0009063C
	public int EncodeData()
	{
		int num = 0;
		if (this.index >= 32 | this.index < 0)
		{
			throw new ArgumentOutOfRangeException(string.Format("Index is invalid {0}", this.index));
		}
		num |= this.index << 25;
		AdvancedItemState.PointType pointType = this.preData.pointType;
		num |= (int)((int)(pointType & (AdvancedItemState.PointType)7) << 22);
		num |= (int)((int)this.limitAxis << 19);
		num |= (this.reverseGrip ? 1 : 0) << 18;
		bool flag = this.angleVectorWhereUpIsStandard.x < 0f;
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num2 = (int)(this.GetEncodedDeltaRotation() * 512f) & 511;
			num |= (flag ? 1 : 0) << 17;
			num |= num2 << 9;
			int num3 = (int)(this.preData.distAlongLine * 256f) & 255;
			num |= num3;
		}
		else
		{
			int num4 = (int)(this.GetEncodedDeltaRotation() * 65536f) & 65535;
			num |= (flag ? 1 : 0) << 17;
			num |= num4 << 1;
		}
		return num;
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x00092558 File Offset: 0x00090758
	public AdvancedItemState DecodeData(int encoded)
	{
		AdvancedItemState advancedItemState = new AdvancedItemState();
		advancedItemState.index = (encoded >> 25 & 31);
		advancedItemState.limitAxis = (LimitAxis)(encoded >> 19 & 7);
		advancedItemState.reverseGrip = ((encoded >> 18 & 1) == 1);
		AdvancedItemState.PointType pointType = (AdvancedItemState.PointType)(encoded >> 22 & 7);
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType,
				distAlongLine = (float)(encoded & 255) / 256f
			};
			this.DecodeDeltaRotation((float)(encoded >> 9 & 511) / 512f, (encoded >> 17 & 1) > 0);
		}
		else
		{
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType
			};
			this.DecodeDeltaRotation((float)(encoded >> 1 & 65535) / 65536f, (encoded >> 17 & 1) > 0);
		}
		return advancedItemState;
	}

	// Token: 0x040023BE RID: 9150
	private int _encodedValue;

	// Token: 0x040023BF RID: 9151
	public Vector2 angleVectorWhereUpIsStandard;

	// Token: 0x040023C0 RID: 9152
	public Quaternion deltaRotation;

	// Token: 0x040023C1 RID: 9153
	public int index;

	// Token: 0x040023C2 RID: 9154
	public AdvancedItemState.PreData preData;

	// Token: 0x040023C3 RID: 9155
	public LimitAxis limitAxis;

	// Token: 0x040023C4 RID: 9156
	public bool reverseGrip;

	// Token: 0x040023C5 RID: 9157
	public float angle;

	// Token: 0x02000454 RID: 1108
	[Serializable]
	public class PreData
	{
		// Token: 0x040023C6 RID: 9158
		public float distAlongLine;

		// Token: 0x040023C7 RID: 9159
		public AdvancedItemState.PointType pointType;
	}

	// Token: 0x02000455 RID: 1109
	public enum PointType
	{
		// Token: 0x040023C9 RID: 9161
		Standard,
		// Token: 0x040023CA RID: 9162
		DistanceBased
	}
}
