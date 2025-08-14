using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class CosmeticFan : MonoBehaviour
{
	// Token: 0x060009D1 RID: 2513 RVA: 0x00035DBF File Offset: 0x00033FBF
	private void Start()
	{
		this.spinUpRate = this.maxSpeed / this.spinUpDuration;
		this.spinDownRate = this.maxSpeed / this.spinDownDuration;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00035DE8 File Offset: 0x00033FE8
	public void Run()
	{
		this.targetSpeed = this.maxSpeed;
		if (this.spinUpDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinUpRate;
		}
		else
		{
			this.currentSpeed = this.maxSpeed;
		}
		base.enabled = true;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00035E36 File Offset: 0x00034036
	public void Stop()
	{
		this.targetSpeed = 0f;
		if (this.spinDownDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinDownRate;
			return;
		}
		this.currentSpeed = 0f;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00035E6F File Offset: 0x0003406F
	public void InstantStop()
	{
		this.targetSpeed = 0f;
		this.currentSpeed = 0f;
		base.enabled = false;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00035E90 File Offset: 0x00034090
	private void Update()
	{
		this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.targetSpeed, this.currentAccelRate * Time.deltaTime);
		base.transform.localRotation = base.transform.localRotation * Quaternion.AngleAxis(this.currentSpeed * Time.deltaTime, this.axis);
		if (this.currentSpeed == 0f && this.targetSpeed == 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04000BBC RID: 3004
	[SerializeField]
	private Vector3 axis;

	// Token: 0x04000BBD RID: 3005
	[SerializeField]
	private float spinUpDuration = 0.3f;

	// Token: 0x04000BBE RID: 3006
	[SerializeField]
	private float spinDownDuration = 0.3f;

	// Token: 0x04000BBF RID: 3007
	[SerializeField]
	private float maxSpeed = 360f;

	// Token: 0x04000BC0 RID: 3008
	private float currentSpeed;

	// Token: 0x04000BC1 RID: 3009
	private float targetSpeed;

	// Token: 0x04000BC2 RID: 3010
	private float currentAccelRate;

	// Token: 0x04000BC3 RID: 3011
	private float spinUpRate;

	// Token: 0x04000BC4 RID: 3012
	private float spinDownRate;
}
