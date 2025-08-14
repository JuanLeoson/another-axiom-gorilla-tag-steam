using System;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class KiteDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x0600170E RID: 5902 RVA: 0x0007CFAC File Offset: 0x0007B1AC
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtPosition = this.grabPt.position;
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x0007D009 File Offset: 0x0007B209
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x0007D034 File Offset: 0x0007B234
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x0007D084 File Offset: 0x0007B284
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!enable)
			{
				this.rb.velocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x0007D0EE File Offset: 0x0007B2EE
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.rb.useGravity = !enable;
		this.balloonScale = scale;
		this.grabPtPosition = this.grabPt.position;
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001713 RID: 5907 RVA: 0x0007D117 File Offset: 0x0007B317
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x0007D134 File Offset: 0x0007B334
	private void FixedUpdate()
	{
		if (this.rb.isKinematic || this.rb.useGravity)
		{
			return;
		}
		if (this.enableDynamics)
		{
			Vector3 vector = (this.grabPt.position - this.grabPtPosition) * 100f;
			vector = Matrix4x4.Rotate(this.ctrlRotation).MultiplyVector(vector);
			this.rb.AddForce(vector, ForceMode.Force);
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			this.rb.velocity = velocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
			base.transform.LookAt(base.transform.position - this.rb.velocity);
		}
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x00002628 File Offset: 0x00000828
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x0007AABF File Offset: 0x00078CBF
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x0007D20E File Offset: 0x0007B40E
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		transferOwnership = false;
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x0007D214 File Offset: 0x0007B414
	public bool ReturnStep()
	{
		this.rb.isKinematic = true;
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.grabPt.position, Time.deltaTime * this.returnSpeed);
		return base.transform.position == this.grabPt.position;
	}

	// Token: 0x04001EDD RID: 7901
	private Rigidbody rb;

	// Token: 0x04001EDE RID: 7902
	private Collider balloonCollider;

	// Token: 0x04001EDF RID: 7903
	private Bounds bounds;

	// Token: 0x04001EE0 RID: 7904
	[SerializeField]
	private float bouyancyMinHeight = 10f;

	// Token: 0x04001EE1 RID: 7905
	[SerializeField]
	private float bouyancyMaxHeight = 20f;

	// Token: 0x04001EE2 RID: 7906
	private float bouyancyActualHeight = 20f;

	// Token: 0x04001EE3 RID: 7907
	[SerializeField]
	private float airResistance = 0.01f;

	// Token: 0x04001EE4 RID: 7908
	public GameObject knot;

	// Token: 0x04001EE5 RID: 7909
	private Rigidbody knotRb;

	// Token: 0x04001EE6 RID: 7910
	public Transform grabPt;

	// Token: 0x04001EE7 RID: 7911
	private Transform grabPtInitParent;

	// Token: 0x04001EE8 RID: 7912
	[SerializeField]
	private float maximumVelocity = 2f;

	// Token: 0x04001EE9 RID: 7913
	private bool enableDynamics;

	// Token: 0x04001EEA RID: 7914
	[SerializeField]
	private float balloonScale = 1f;

	// Token: 0x04001EEB RID: 7915
	private Vector3 grabPtPosition;

	// Token: 0x04001EEC RID: 7916
	[SerializeField]
	private Quaternion ctrlRotation;

	// Token: 0x04001EED RID: 7917
	[SerializeField]
	private float returnSpeed = 50f;
}
