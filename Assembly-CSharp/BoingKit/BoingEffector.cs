using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FC6 RID: 4038
	public class BoingEffector : BoingBase
	{
		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x060064F2 RID: 25842 RVA: 0x00200EA9 File Offset: 0x001FF0A9
		public Vector3 LinearVelocity
		{
			get
			{
				return this.m_linearVelocity;
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x060064F3 RID: 25843 RVA: 0x00200EB1 File Offset: 0x001FF0B1
		public float LinearSpeed
		{
			get
			{
				return this.m_linearVelocity.magnitude;
			}
		}

		// Token: 0x060064F4 RID: 25844 RVA: 0x00200EBE File Offset: 0x001FF0BE
		public void OnEnable()
		{
			this.m_currPosition = base.transform.position;
			this.m_prevPosition = base.transform.position;
			this.m_linearVelocity = Vector3.zero;
			BoingManager.Register(this);
		}

		// Token: 0x060064F5 RID: 25845 RVA: 0x00200EF3 File Offset: 0x001FF0F3
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060064F6 RID: 25846 RVA: 0x00200EFC File Offset: 0x001FF0FC
		public void Update()
		{
			float deltaTime = Time.deltaTime;
			if (deltaTime < MathUtil.Epsilon)
			{
				return;
			}
			this.m_linearVelocity = (base.transform.position - this.m_prevPosition) / deltaTime;
			this.m_prevPosition = this.m_currPosition;
			this.m_currPosition = base.transform.position;
		}

		// Token: 0x060064F7 RID: 25847 RVA: 0x00200F58 File Offset: 0x001FF158
		public void OnDrawGizmosSelected()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.FullEffectRadiusRatio < 1f)
			{
				Gizmos.color = new Color(1f, 0.5f, 0.2f, 0.4f);
				Gizmos.DrawWireSphere(base.transform.position, this.Radius);
			}
			Gizmos.color = new Color(1f, 0.5f, 0.2f, 1f);
			Gizmos.DrawWireSphere(base.transform.position, this.Radius * this.FullEffectRadiusRatio);
		}

		// Token: 0x04006FEA RID: 28650
		[Header("Metrics")]
		[Range(0f, 20f)]
		[Tooltip("Maximum radius of influence.")]
		public float Radius = 3f;

		// Token: 0x04006FEB RID: 28651
		[Range(0f, 1f)]
		[Tooltip("Fraction of Radius past which influence begins decaying gradually to zero exactly at Radius.\n\ne.g. With a Radius of 10.0 and FullEffectRadiusRatio of 0.5, reactors within distance of 5.0 will be fully influenced, reactors at distance of 7.5 will experience 50% influence, and reactors past distance of 10.0 will not be influenced at all.")]
		public float FullEffectRadiusRatio = 0.5f;

		// Token: 0x04006FEC RID: 28652
		[Header("Dynamics")]
		[Range(0f, 100f)]
		[Tooltip("Speed of this effector at which impulse effects will be at maximum strength.\n\ne.g. With a MaxImpulseSpeed of 10.0 and an effector traveling at speed of 4.0, impulse effects will be at 40% maximum strength.")]
		public float MaxImpulseSpeed = 5f;

		// Token: 0x04006FED RID: 28653
		[Tooltip("This affects impulse-related effects.\n\nIf checked, continuous motion will be simulated between frames. This means even if an effector \"teleports\" by moving a huge distance between frames, the effector will still affect all reactors caught on the effector's path in between frames, not just the reactors around the effector's discrete positions at different frames.")]
		public bool ContinuousMotion;

		// Token: 0x04006FEE RID: 28654
		[Header("Position Effect")]
		[Range(-10f, 10f)]
		[Tooltip("Distance to push away reactors at maximum influence.\n\ne.g. With a MoveDistance of 2.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor will be pushed away to 50% of maximum influence, i.e. 50% of MoveDistance, which is a distance of 1.0 away from the effector.")]
		public float MoveDistance = 0.5f;

		// Token: 0x04006FEF RID: 28655
		[Range(-200f, 200f)]
		[Tooltip("Under maximum impulse influence (within distance of Radius * FullEffectRadiusRatio and with effector moving at speed faster or equal to MaxImpulaseSpeed), a reactor's movement speed will be maintained to be at least as fast as LinearImpulse (unit: distance per second) in the direction of effector's movement direction.\n\ne.g. With a LinearImpulse of 2.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor's movement speed in the direction of effector's movement direction will be maintained to be at least 50% of LinearImpulse, which is 1.0 per second.")]
		public float LinearImpulse = 5f;

		// Token: 0x04006FF0 RID: 28656
		[Header("Rotation Effect")]
		[Range(-180f, 180f)]
		[Tooltip("Angle (in degrees) to rotate reactors at maximum influence. The rotation will point reactors' up vectors (defined individually in the reactor component) away from the effector.\n\ne.g. With a RotationAngle of 20.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor will be rotated to 50% of maximum influence, i.e. 50% of RotationAngle, which is 10 degrees.")]
		public float RotationAngle = 20f;

		// Token: 0x04006FF1 RID: 28657
		[Range(-2000f, 2000f)]
		[Tooltip("Under maximum impulse influence (within distance of Radius * FullEffectRadiusRatio and with effector moving at speed faster or equal to MaxImpulaseSpeed), a reactor's rotation speed will be maintained to be at least as fast as AngularImpulse (unit: degrees per second) in the direction of effector's movement direction, i.e. the reactor's up vector will be pulled in the direction of effector's movement direction.\n\ne.g. With a AngularImpulse of 20.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor's rotation speed in the direction of effector's movement direction will be maintained to be at least 50% of AngularImpulse, which is 10.0 degrees per second.")]
		public float AngularImpulse = 400f;

		// Token: 0x04006FF2 RID: 28658
		[Header("Debug")]
		[Tooltip("If checked, gizmos of reactor fields affected by this effector will be drawn.")]
		public bool DrawAffectedReactorFieldGizmos;

		// Token: 0x04006FF3 RID: 28659
		private Vector3 m_currPosition;

		// Token: 0x04006FF4 RID: 28660
		private Vector3 m_prevPosition;

		// Token: 0x04006FF5 RID: 28661
		private Vector3 m_linearVelocity;

		// Token: 0x02000FC7 RID: 4039
		public struct Params
		{
			// Token: 0x060064F9 RID: 25849 RVA: 0x0020104C File Offset: 0x001FF24C
			public Params(BoingEffector effector)
			{
				this.Bits = default(Bits32);
				this.Bits.SetBit(0, effector.ContinuousMotion);
				float num = (effector.MaxImpulseSpeed > MathUtil.Epsilon) ? Mathf.Min(1f, effector.LinearSpeed / effector.MaxImpulseSpeed) : 1f;
				this.PrevPosition = effector.m_prevPosition;
				this.CurrPosition = effector.m_currPosition;
				this.LinearVelocityDir = VectorUtil.NormalizeSafe(effector.LinearVelocity, Vector3.zero);
				this.Radius = effector.Radius;
				this.FullEffectRadius = this.Radius * effector.FullEffectRadiusRatio;
				this.MoveDistance = effector.MoveDistance;
				this.LinearImpulse = num * effector.LinearImpulse;
				this.RotateAngle = effector.RotationAngle * MathUtil.Deg2Rad;
				this.AngularImpulse = num * effector.AngularImpulse * MathUtil.Deg2Rad;
				this.m_padding0 = 0f;
				this.m_padding1 = 0f;
				this.m_padding2 = 0f;
				this.m_padding3 = 0;
			}

			// Token: 0x060064FA RID: 25850 RVA: 0x00201167 File Offset: 0x001FF367
			public void Fill(BoingEffector effector)
			{
				this = new BoingEffector.Params(effector);
			}

			// Token: 0x060064FB RID: 25851 RVA: 0x00201178 File Offset: 0x001FF378
			private void SuppressWarnings()
			{
				this.m_padding0 = 0f;
				this.m_padding1 = 0f;
				this.m_padding2 = 0f;
				this.m_padding3 = 0;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding2;
				this.m_padding2 = (float)this.m_padding3;
				this.m_padding3 = (int)this.m_padding0;
			}

			// Token: 0x04006FF6 RID: 28662
			public static readonly int Stride = 80;

			// Token: 0x04006FF7 RID: 28663
			public Vector3 PrevPosition;

			// Token: 0x04006FF8 RID: 28664
			private float m_padding0;

			// Token: 0x04006FF9 RID: 28665
			public Vector3 CurrPosition;

			// Token: 0x04006FFA RID: 28666
			private float m_padding1;

			// Token: 0x04006FFB RID: 28667
			public Vector3 LinearVelocityDir;

			// Token: 0x04006FFC RID: 28668
			private float m_padding2;

			// Token: 0x04006FFD RID: 28669
			public float Radius;

			// Token: 0x04006FFE RID: 28670
			public float FullEffectRadius;

			// Token: 0x04006FFF RID: 28671
			public float MoveDistance;

			// Token: 0x04007000 RID: 28672
			public float LinearImpulse;

			// Token: 0x04007001 RID: 28673
			public float RotateAngle;

			// Token: 0x04007002 RID: 28674
			public float AngularImpulse;

			// Token: 0x04007003 RID: 28675
			public Bits32 Bits;

			// Token: 0x04007004 RID: 28676
			private int m_padding3;
		}
	}
}
