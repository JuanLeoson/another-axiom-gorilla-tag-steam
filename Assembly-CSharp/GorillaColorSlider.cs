using System;
using UnityEngine;

// Token: 0x020007D6 RID: 2006
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x0600323E RID: 12862 RVA: 0x00105FA1 File Offset: 0x001041A1
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x0600323F RID: 12863 RVA: 0x00105FBC File Offset: 0x001041BC
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x06003240 RID: 12864 RVA: 0x0010605C File Offset: 0x0010425C
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06003241 RID: 12865 RVA: 0x001060B8 File Offset: 0x001042B8
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
			else
			{
				base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
		}
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x04003EFA RID: 16122
	public bool setRandomly;

	// Token: 0x04003EFB RID: 16123
	public float zRange;

	// Token: 0x04003EFC RID: 16124
	public float maxValue;

	// Token: 0x04003EFD RID: 16125
	public float minValue;

	// Token: 0x04003EFE RID: 16126
	public Vector3 startingLocation;

	// Token: 0x04003EFF RID: 16127
	public int valueIndex;

	// Token: 0x04003F00 RID: 16128
	public float valueImReporting;

	// Token: 0x04003F01 RID: 16129
	public GorillaTriggerBox gorilla;

	// Token: 0x04003F02 RID: 16130
	private float startingZ;
}
