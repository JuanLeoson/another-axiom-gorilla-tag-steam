using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001001 RID: 4097
	public struct Vector3Spring
	{
		// Token: 0x06006667 RID: 26215 RVA: 0x00208D8B File Offset: 0x00206F8B
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06006668 RID: 26216 RVA: 0x00208DA3 File Offset: 0x00206FA3
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06006669 RID: 26217 RVA: 0x00208DB7 File Offset: 0x00206FB7
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x0600666A RID: 26218 RVA: 0x00208DC8 File Offset: 0x00206FC8
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

		// Token: 0x0600666B RID: 26219 RVA: 0x00208EC4 File Offset: 0x002070C4
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

		// Token: 0x0600666C RID: 26220 RVA: 0x00208F10 File Offset: 0x00207110
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

		// Token: 0x04007132 RID: 28978
		public static readonly int Stride = 32;

		// Token: 0x04007133 RID: 28979
		public Vector3 Value;

		// Token: 0x04007134 RID: 28980
		private float m_padding0;

		// Token: 0x04007135 RID: 28981
		public Vector3 Velocity;

		// Token: 0x04007136 RID: 28982
		private float m_padding1;
	}
}
