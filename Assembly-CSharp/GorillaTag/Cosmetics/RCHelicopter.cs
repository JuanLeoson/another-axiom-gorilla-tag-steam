using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F15 RID: 3861
	public class RCHelicopter : RCVehicle
	{
		// Token: 0x06005F90 RID: 24464 RVA: 0x001E46BC File Offset: 0x001E28BC
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.verticalPropeller.localRotation = this.verticalPropellerBaseRotation;
			this.turnPropeller.localRotation = this.turnPropellerBaseRotation;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005F91 RID: 24465 RVA: 0x001E4718 File Offset: 0x001E2918
		protected override void Awake()
		{
			base.Awake();
			this.verticalPropellerBaseRotation = this.verticalPropeller.localRotation;
			this.turnPropellerBaseRotation = this.turnPropeller.localRotation;
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
		}

		// Token: 0x06005F92 RID: 24466 RVA: 0x001E4788 File Offset: 0x001E2988
		protected override void SharedUpdate(float dt)
		{
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = Mathf.Lerp(this.mainPropellerSpinRateRange.x, this.mainPropellerSpinRateRange.y, this.activeInput.trigger);
				this.verticalPropeller.Rotate(new Vector3(0f, num * dt, 0f), Space.Self);
				this.turnPropeller.Rotate(new Vector3(this.activeInput.joystick.x * this.backPropellerSpinRate * dt, 0f, 0f), Space.Self);
			}
		}

		// Token: 0x06005F93 RID: 24467 RVA: 0x001E4818 File Offset: 0x001E2A18
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			float target = this.activeInput.joystick.x * this.maxTurnRate;
			this.turnRate = Mathf.MoveTowards(this.turnRate, target, this.turnAccel * fixedDeltaTime);
			float num = this.activeInput.joystick.y * this.maxHorizontalSpeed;
			float x = Mathf.Sign(this.activeInput.joystick.y) * Mathf.Lerp(0f, this.maxHorizontalTiltAngle, Mathf.Abs(this.activeInput.joystick.y));
			base.transform.rotation = Quaternion.Euler(new Vector3(x, this.turnAccel, 0f));
			float num2 = Mathf.Abs(num);
			Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			float num3 = Vector3.Dot(normalized, velocity);
			if (num2 > 0.01f && ((num > 0f && num > num3) || (num < 0f && num < num3)))
			{
				this.rb.AddForce(normalized * Mathf.Sign(num) * this.horizontalAccel * fixedDeltaTime, ForceMode.Acceleration);
			}
			float num4 = this.activeInput.trigger * this.maxAscendSpeed;
			if (num4 > 0.01f && velocity.y < num4)
			{
				this.rb.AddForce(Vector3.up * this.ascendAccel, ForceMode.Acceleration);
			}
			if (this.rb.useGravity)
			{
				this.rb.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
			}
		}

		// Token: 0x06005F94 RID: 24468 RVA: 0x001E49E6 File Offset: 0x001E2BE6
		private void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
			}
		}

		// Token: 0x04006A98 RID: 27288
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04006A99 RID: 27289
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04006A9A RID: 27290
		[SerializeField]
		private float gravityCompensation = 0.5f;

		// Token: 0x04006A9B RID: 27291
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04006A9C RID: 27292
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04006A9D RID: 27293
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04006A9E RID: 27294
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04006A9F RID: 27295
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04006AA0 RID: 27296
		[SerializeField]
		private Vector2 mainPropellerSpinRateRange = new Vector2(3f, 15f);

		// Token: 0x04006AA1 RID: 27297
		[SerializeField]
		private float backPropellerSpinRate = 5f;

		// Token: 0x04006AA2 RID: 27298
		[SerializeField]
		private Transform verticalPropeller;

		// Token: 0x04006AA3 RID: 27299
		[SerializeField]
		private Transform turnPropeller;

		// Token: 0x04006AA4 RID: 27300
		private Quaternion verticalPropellerBaseRotation;

		// Token: 0x04006AA5 RID: 27301
		private Quaternion turnPropellerBaseRotation;

		// Token: 0x04006AA6 RID: 27302
		private float turnRate;

		// Token: 0x04006AA7 RID: 27303
		private float ascendAccel;

		// Token: 0x04006AA8 RID: 27304
		private float turnAccel;

		// Token: 0x04006AA9 RID: 27305
		private float horizontalAccel;
	}
}
