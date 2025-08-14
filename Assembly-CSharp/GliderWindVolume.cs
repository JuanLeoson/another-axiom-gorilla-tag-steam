using System;
using UnityEngine;

// Token: 0x02000A21 RID: 2593
public class GliderWindVolume : MonoBehaviour
{
	// Token: 0x06003F46 RID: 16198 RVA: 0x001434D7 File Offset: 0x001416D7
	public void SetProperties(float speed, float accel, AnimationCurve svaCurve, Vector3 windDirection)
	{
		this.maxSpeed = speed;
		this.maxAccel = accel;
		this.speedVsAccelCurve.CopyFrom(svaCurve);
		this.localWindDirection = windDirection;
	}

	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x06003F47 RID: 16199 RVA: 0x001434FB File Offset: 0x001416FB
	public Vector3 WindDirection
	{
		get
		{
			return base.transform.TransformDirection(this.localWindDirection);
		}
	}

	// Token: 0x06003F48 RID: 16200 RVA: 0x00143510 File Offset: 0x00141710
	public Vector3 GetAccelFromVelocity(Vector3 velocity)
	{
		Vector3 windDirection = this.WindDirection;
		float time = Mathf.Clamp(Vector3.Dot(velocity, windDirection), -this.maxSpeed, this.maxSpeed) / this.maxSpeed;
		float d = this.speedVsAccelCurve.Evaluate(time) * this.maxAccel;
		return windDirection * d;
	}

	// Token: 0x04004BA0 RID: 19360
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x04004BA1 RID: 19361
	[SerializeField]
	private float maxAccel = 15f;

	// Token: 0x04004BA2 RID: 19362
	[SerializeField]
	private AnimationCurve speedVsAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04004BA3 RID: 19363
	[SerializeField]
	private Vector3 localWindDirection = Vector3.up;
}
