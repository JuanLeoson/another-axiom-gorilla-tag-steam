using System;
using UnityEngine;

// Token: 0x02000894 RID: 2196
public class VelocityHelperTest : MonoBehaviour
{
	// Token: 0x06003762 RID: 14178 RVA: 0x0011F615 File Offset: 0x0011D815
	private void Setup()
	{
		this.lastPosition = base.transform.position;
		this.lastVelocity = Vector3.zero;
		this.velocity = Vector3.zero;
		this.speed = 0f;
	}

	// Token: 0x06003763 RID: 14179 RVA: 0x0011F649 File Offset: 0x0011D849
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0011F654 File Offset: 0x0011D854
	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 b = (position - this.lastPosition) / deltaTime;
		this.velocity = Vector3.Lerp(this.lastVelocity, b, deltaTime);
		this.speed = this.velocity.magnitude;
		this.lastPosition = position;
		this.lastVelocity = b;
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x040043E1 RID: 17377
	public Vector3 velocity;

	// Token: 0x040043E2 RID: 17378
	public float speed;

	// Token: 0x040043E3 RID: 17379
	[Space]
	public Vector3 lastVelocity;

	// Token: 0x040043E4 RID: 17380
	public Vector3 lastPosition;

	// Token: 0x040043E5 RID: 17381
	[Space]
	[SerializeField]
	private float[] _deltaTimes = new float[5];
}
