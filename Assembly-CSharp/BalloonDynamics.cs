using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class BalloonDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x0600167C RID: 5756 RVA: 0x0007A690 File Offset: 0x00078890
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x0007A6DC File Offset: 0x000788DC
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x0007A708 File Offset: 0x00078908
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x0007A758 File Offset: 0x00078958
	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float y = this.bouyancyForce * num2 * this.balloonScale;
		this.rb.AddForce(new Vector3(0f, y, 0f), ForceMode.Acceleration);
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x0007A7C0 File Offset: 0x000789C0
	private void ApplyUpRightForce()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque * this.balloonScale;
		this.rb.AddTorque(torque);
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x0007A808 File Offset: 0x00078A08
	private void ApplyAntiSpinForce()
	{
		Vector3 vector = this.rb.transform.InverseTransformDirection(this.rb.angularVelocity);
		this.rb.AddRelativeTorque(0f, -vector.y * this.antiSpinTorque, 0f);
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x0007A854 File Offset: 0x00078A54
	private void ApplyAirResistance()
	{
		this.rb.velocity *= 1f - this.airResistance;
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x0007A878 File Offset: 0x00078A78
	private void ApplyDistanceConstraint()
	{
		this.knot.transform.position - base.transform.position;
		Vector3 vector = this.grabPt.transform.position - this.knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = this.stringLength * this.balloonScale;
		if (magnitude > num)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.velocity, normalized) * normalized;
			float num2 = magnitude - num;
			float num3 = num2 / Time.fixedDeltaTime;
			if (vector2.magnitude < num3)
			{
				float b = num3 - vector2.magnitude;
				float num4 = Mathf.Clamp01(num2 / this.stringStretch);
				Vector3 force = Mathf.Lerp(0f, b, num4 * num4) * normalized * this.stringStrength;
				this.rb.AddForceAtPosition(force, this.knot.transform.position, ForceMode.VelocityChange);
			}
		}
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x0007A984 File Offset: 0x00078B84
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		bool flag = !this.enableDynamics && enable;
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!kinematic && flag)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x0007A9FF File Offset: 0x00078BFF
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.enableDistanceConstraints = enable;
		this.balloonScale = scale;
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06001686 RID: 5766 RVA: 0x0007AA0F File Offset: 0x00078C0F
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x0007AA2C File Offset: 0x00078C2C
	private void FixedUpdate()
	{
		if (this.enableDynamics && !this.rb.isKinematic)
		{
			this.ApplyBouyancyForce();
			if (this.antiSpinTorque > 0f)
			{
				this.ApplyAntiSpinForce();
			}
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			this.rb.velocity = velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
		}
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x00002628 File Offset: 0x00000828
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x0007AABF File Offset: 0x00078CBF
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x0007AAC8 File Offset: 0x00078CC8
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		if (!other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
		{
			return;
		}
		if (!this.rb)
		{
			return;
		}
		transferOwnership = true;
		TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
		if (!component)
		{
			return;
		}
		Vector3 a = (component.transform.position - component.prevPos) / Time.deltaTime;
		force = a * this.bopSpeed;
		force = Mathf.Min(this.maximumVelocity, force.magnitude) * force.normalized * this.balloonScale;
		if (this.bopSpeedCap > 0f && force.IsLongerThan(this.bopSpeedCap))
		{
			force = force.normalized * this.bopSpeedCap;
		}
		collisionPt = other.ClosestPointOnBounds(base.transform.position);
		this.rb.AddForceAtPosition(force, collisionPt, ForceMode.VelocityChange);
		if (this.balloonBopSource != null)
		{
			this.balloonBopSource.GTPlay();
		}
		GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component2 != null)
		{
			float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, amplitude, fixedDeltaTime);
		}
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x0001D558 File Offset: 0x0001B758
	public bool ReturnStep()
	{
		return true;
	}

	// Token: 0x04001E41 RID: 7745
	private Rigidbody rb;

	// Token: 0x04001E42 RID: 7746
	private Collider balloonCollider;

	// Token: 0x04001E43 RID: 7747
	private Bounds bounds;

	// Token: 0x04001E44 RID: 7748
	public float bouyancyForce = 1f;

	// Token: 0x04001E45 RID: 7749
	public float bouyancyMinHeight = 10f;

	// Token: 0x04001E46 RID: 7750
	public float bouyancyMaxHeight = 20f;

	// Token: 0x04001E47 RID: 7751
	private float bouyancyActualHeight = 20f;

	// Token: 0x04001E48 RID: 7752
	public float varianceMaxheight = 5f;

	// Token: 0x04001E49 RID: 7753
	public float airResistance = 0.01f;

	// Token: 0x04001E4A RID: 7754
	public GameObject knot;

	// Token: 0x04001E4B RID: 7755
	private Rigidbody knotRb;

	// Token: 0x04001E4C RID: 7756
	public Transform grabPt;

	// Token: 0x04001E4D RID: 7757
	private Transform grabPtInitParent;

	// Token: 0x04001E4E RID: 7758
	public float stringLength = 2f;

	// Token: 0x04001E4F RID: 7759
	public float stringStrength = 0.9f;

	// Token: 0x04001E50 RID: 7760
	public float stringStretch = 0.1f;

	// Token: 0x04001E51 RID: 7761
	public float maximumVelocity = 2f;

	// Token: 0x04001E52 RID: 7762
	public float upRightTorque = 1f;

	// Token: 0x04001E53 RID: 7763
	public float antiSpinTorque;

	// Token: 0x04001E54 RID: 7764
	private bool enableDynamics;

	// Token: 0x04001E55 RID: 7765
	private bool enableDistanceConstraints;

	// Token: 0x04001E56 RID: 7766
	public float balloonScale = 1f;

	// Token: 0x04001E57 RID: 7767
	public float bopSpeed = 1f;

	// Token: 0x04001E58 RID: 7768
	public float bopSpeedCap;

	// Token: 0x04001E59 RID: 7769
	[SerializeField]
	private AudioSource balloonBopSource;
}
