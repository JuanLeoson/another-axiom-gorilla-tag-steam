using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000FB1 RID: 4017
	public struct Vector3Spring
	{
		// Token: 0x06006469 RID: 25705 RVA: 0x001FD823 File Offset: 0x001FBA23
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x001FD83B File Offset: 0x001FBA3B
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x0600646B RID: 25707 RVA: 0x001FD84F File Offset: 0x001FBA4F
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x0600646C RID: 25708 RVA: 0x001FD860 File Offset: 0x001FBA60
		public Vector3 TrackDampingRatio(Vector3 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				return this.Value;
			}
			Vector3 a = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float d = 1f / (num + num4);
			Vector3 a2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector3 a3 = this.Velocity + num3 * a;
			this.Velocity = a3 * d;
			this.Value = a2 * d;
			if (this.Velocity.magnitude < MathUtil.Epsilon && a.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x001FD95C File Offset: 0x001FBB5C
		public Vector3 TrackHalfLife(Vector3 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x0600646E RID: 25710 RVA: 0x001FD9A8 File Offset: 0x001FBBA8
		public Vector3 TrackExponential(Vector3 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x04006F26 RID: 28454
		public static readonly int Stride = 32;

		// Token: 0x04006F27 RID: 28455
		public Vector3 Value;

		// Token: 0x04006F28 RID: 28456
		private float m_padding0;

		// Token: 0x04006F29 RID: 28457
		public Vector3 Velocity;

		// Token: 0x04006F2A RID: 28458
		private float m_padding1;
	}
}
