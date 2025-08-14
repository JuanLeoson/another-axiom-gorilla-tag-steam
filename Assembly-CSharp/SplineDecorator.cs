using System;
using UnityEngine;

// Token: 0x02000AEB RID: 2795
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x06004355 RID: 17237 RVA: 0x00153008 File Offset: 0x00151208
	private void Awake()
	{
		if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
		{
			return;
		}
		float num = (float)(this.frequency * this.items.Length);
		if (this.spline.Loop || num == 1f)
		{
			num = 1f / num;
		}
		else
		{
			num = 1f / (num - 1f);
		}
		int num2 = 0;
		for (int i = 0; i < this.frequency; i++)
		{
			int j = 0;
			while (j < this.items.Length)
			{
				Transform transform = Object.Instantiate<Transform>(this.items[j]);
				Vector3 point = this.spline.GetPoint((float)num2 * num);
				transform.transform.localPosition = point;
				if (this.lookForward)
				{
					transform.transform.LookAt(point + this.spline.GetDirection((float)num2 * num));
				}
				transform.transform.parent = base.transform;
				j++;
				num2++;
			}
		}
	}

	// Token: 0x04004E0B RID: 19979
	public BezierSpline spline;

	// Token: 0x04004E0C RID: 19980
	public int frequency;

	// Token: 0x04004E0D RID: 19981
	public bool lookForward;

	// Token: 0x04004E0E RID: 19982
	public Transform[] items;
}
