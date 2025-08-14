using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FB3 RID: 4019
	public struct QuaternionSpring
	{
		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x06006477 RID: 25719 RVA: 0x001FDBCB File Offset: 0x001FBDCB
		// (set) Token: 0x06006478 RID: 25720 RVA: 0x001FDBD9 File Offset: 0x001FBDD9
		public Quaternion ValueQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			set
			{
				this.ValueVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x06006479 RID: 25721 RVA: 0x001FDBE7 File Offset: 0x001FBDE7
		// (set) Token: 0x0600647A RID: 25722 RVA: 0x001FDBF5 File Offset: 0x001FBDF5
		public Quaternion VelocityQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.VelocityVec, false);
			}
			set
			{
				this.VelocityVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x001FDC03 File Offset: 0x001FBE03
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x001FDC20 File Offset: 0x001FBE20
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x0600647D RID: 25725 RVA: 0x001FDC34 File Offset: 0x001FBE34
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x0600647E RID: 25726 RVA: 0x001FDC44 File Offset: 0x001FBE44
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x0600647F RID: 25727 RVA: 0x001FDC5D File Offset: 0x001FBE5D
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x06006480 RID: 25728 RVA: 0x001FDC78 File Offset: 0x001FBE78
		public Quaternion TrackDampingRatio(Quaternion targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			Vector4 vector = QuaternionUtil.ToVector4(targetValue);
			if (Vector4.Dot(this.ValueVec, vector) < 0f)
			{
				vector = -vector;
			}
			Vector4 a = vector - this.ValueVec;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float d = 1f / (num + num4);
			Vector4 a2 = num * this.ValueVec + deltaTime * this.VelocityVec + num4 * vector;
			Vector4 a3 = this.VelocityVec + num3 * a;
			this.VelocityVec = a3 * d;
			this.ValueVec = a2 * d;
			if (this.VelocityVec.magnitude < MathUtil.Epsilon && a.magnitude < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = vector;
			}
			return QuaternionUtil.FromVector4(this.ValueVec, true);
		}

		// Token: 0x06006481 RID: 25729 RVA: 0x001FDDAC File Offset: 0x001FBFAC
		public Quaternion TrackHalfLife(Quaternion targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x001FDDFC File Offset: 0x001FBFFC
		public Quaternion TrackExponential(Quaternion targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x04006F2E RID: 28462
		public static readonly int Stride = 32;

		// Token: 0x04006F2F RID: 28463
		public Vector4 ValueVec;

		// Token: 0x04006F30 RID: 28464
		public Vector4 VelocityVec;
	}
}
