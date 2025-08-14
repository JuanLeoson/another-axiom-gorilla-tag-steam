using System;

namespace GTMathUtil
{
	// Token: 0x02000BE9 RID: 3049
	internal class CriticalSpringDamper
	{
		// Token: 0x06004A04 RID: 18948 RVA: 0x00165AA6 File Offset: 0x00163CA6
		private static float halflife_to_damping(float halflife, float eps = 1E-05f)
		{
			return 2.7725887f / (halflife + eps);
		}

		// Token: 0x06004A05 RID: 18949 RVA: 0x001657CD File Offset: 0x001639CD
		private static float fast_negexp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}

		// Token: 0x06004A06 RID: 18950 RVA: 0x00167C84 File Offset: 0x00165E84
		public float Update(float dt)
		{
			float num = CriticalSpringDamper.halflife_to_damping(this.halfLife, 1E-05f) / 2f;
			float num2 = this.x - this.xGoal;
			float num3 = this.curVel + num2 * num;
			float num4 = CriticalSpringDamper.fast_negexp(num * dt);
			this.x = num4 * (num2 + num3 * dt) + this.xGoal;
			this.curVel = num4 * (this.curVel - num3 * num * dt);
			return this.x;
		}

		// Token: 0x040052CC RID: 21196
		public float x;

		// Token: 0x040052CD RID: 21197
		public float xGoal;

		// Token: 0x040052CE RID: 21198
		public float halfLife = 0.1f;

		// Token: 0x040052CF RID: 21199
		private float curVel;
	}
}
