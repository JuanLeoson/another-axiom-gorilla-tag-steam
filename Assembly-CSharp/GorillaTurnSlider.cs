using System;
using UnityEngine;

// Token: 0x020007F2 RID: 2034
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x060032E5 RID: 13029 RVA: 0x00108EB9 File Offset: 0x001070B9
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x060032E6 RID: 13030 RVA: 0x000023F5 File Offset: 0x000005F5
	private void FixedUpdate()
	{
	}

	// Token: 0x060032E7 RID: 13031 RVA: 0x00108EE0 File Offset: 0x001070E0
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x060032E8 RID: 13032 RVA: 0x00108F64 File Offset: 0x00107164
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x060032E9 RID: 13033 RVA: 0x00108FC0 File Offset: 0x001071C0
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
				return;
			}
			base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
		}
	}

	// Token: 0x04003FD3 RID: 16339
	public float zRange;

	// Token: 0x04003FD4 RID: 16340
	public float maxValue;

	// Token: 0x04003FD5 RID: 16341
	public float minValue;

	// Token: 0x04003FD6 RID: 16342
	public GorillaTurning gorillaTurn;

	// Token: 0x04003FD7 RID: 16343
	private float startingZ;

	// Token: 0x04003FD8 RID: 16344
	public Vector3 startingLocation;
}
