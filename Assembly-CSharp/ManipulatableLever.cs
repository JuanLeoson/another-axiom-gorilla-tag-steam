using System;
using UnityEngine;

// Token: 0x02000441 RID: 1089
public class ManipulatableLever : ManipulatableObject
{
	// Token: 0x06001AAF RID: 6831 RVA: 0x0008E847 File Offset: 0x0008CA47
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x0008E85C File Offset: 0x0008CA5C
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = this.leverGrip.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x0008E89C File Offset: 0x0008CA9C
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 position = hand.transform.position;
		Vector3 upwards = Vector3.Normalize(this.localSpace.MultiplyPoint3x4(position) - base.transform.localPosition);
		Vector3 eulerAngles = Quaternion.LookRotation(Vector3.forward, upwards).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, this.minAngle, this.maxAngle);
		base.transform.localEulerAngles = eulerAngles;
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x0008E954 File Offset: 0x0008CB54
	public void SetValue(float value)
	{
		float z = Mathf.Lerp(this.minAngle, this.maxAngle, value);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.z = z;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x0008E994 File Offset: 0x0008CB94
	public void SetNotch(int notchValue)
	{
		if (this.notches == null)
		{
			return;
		}
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (leverNotch.value == notchValue)
			{
				this.SetValue(Mathf.Lerp(leverNotch.minAngleValue, leverNotch.maxAngleValue, 0.5f));
				return;
			}
		}
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x0008E9EC File Offset: 0x0008CBEC
	public float GetValue()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.z > 180f)
		{
			localEulerAngles.z -= 360f;
		}
		else if (localEulerAngles.z < -180f)
		{
			localEulerAngles.z += 360f;
		}
		return Mathf.InverseLerp(this.minAngle, this.maxAngle, localEulerAngles.z);
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x0008EA58 File Offset: 0x0008CC58
	public int GetNotch()
	{
		if (this.notches == null)
		{
			return 0;
		}
		float value = this.GetValue();
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (value >= leverNotch.minAngleValue && value <= leverNotch.maxAngleValue)
			{
				return leverNotch.value;
			}
		}
		return 0;
	}

	// Token: 0x040022F6 RID: 8950
	[SerializeField]
	private float breakDistance = 0.2f;

	// Token: 0x040022F7 RID: 8951
	[SerializeField]
	private Transform leverGrip;

	// Token: 0x040022F8 RID: 8952
	[SerializeField]
	private float maxAngle = 22.5f;

	// Token: 0x040022F9 RID: 8953
	[SerializeField]
	private float minAngle = -22.5f;

	// Token: 0x040022FA RID: 8954
	[SerializeField]
	private ManipulatableLever.LeverNotch[] notches;

	// Token: 0x040022FB RID: 8955
	private Matrix4x4 localSpace;

	// Token: 0x02000442 RID: 1090
	[Serializable]
	public class LeverNotch
	{
		// Token: 0x040022FC RID: 8956
		public float minAngleValue;

		// Token: 0x040022FD RID: 8957
		public float maxAngleValue;

		// Token: 0x040022FE RID: 8958
		public int value;
	}
}
