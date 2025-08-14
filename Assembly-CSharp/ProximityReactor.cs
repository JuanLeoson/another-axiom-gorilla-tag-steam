using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200088B RID: 2187
public class ProximityReactor : MonoBehaviour
{
	// Token: 0x17000538 RID: 1336
	// (get) Token: 0x060036C7 RID: 14023 RVA: 0x0011E1DD File Offset: 0x0011C3DD
	public float proximityRange
	{
		get
		{
			return this.proximityMax - this.proximityMin;
		}
	}

	// Token: 0x17000539 RID: 1337
	// (get) Token: 0x060036C8 RID: 14024 RVA: 0x0011E1EC File Offset: 0x0011C3EC
	public float distance
	{
		get
		{
			return this._distance;
		}
	}

	// Token: 0x1700053A RID: 1338
	// (get) Token: 0x060036C9 RID: 14025 RVA: 0x0011E1F4 File Offset: 0x0011C3F4
	public float distanceLinear
	{
		get
		{
			return this._distanceLinear;
		}
	}

	// Token: 0x060036CA RID: 14026 RVA: 0x0011E1FC File Offset: 0x0011C3FC
	public void SetRigFrom()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.from = componentInParent.transform;
		}
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x0011E228 File Offset: 0x0011C428
	public void SetRigTo()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.to = componentInParent.transform;
		}
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x0011E252 File Offset: 0x0011C452
	public void SetTransformFrom(Transform t)
	{
		this.from = t;
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x0011E25B File Offset: 0x0011C45B
	public void SetTransformTo(Transform t)
	{
		this.to = t;
	}

	// Token: 0x060036CE RID: 14030 RVA: 0x0011E264 File Offset: 0x0011C464
	private void Setup()
	{
		this._distance = 0f;
		this._distanceLinear = 0f;
	}

	// Token: 0x060036CF RID: 14031 RVA: 0x0011E27C File Offset: 0x0011C47C
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x060036D0 RID: 14032 RVA: 0x0011E284 File Offset: 0x0011C484
	private void Update()
	{
		if (!this.from || !this.to)
		{
			this._distance = 0f;
			this._distanceLinear = 0f;
			return;
		}
		Vector3 position = this.from.position;
		float magnitude = (this.to.position - position).magnitude;
		if (!this._distance.Approx(magnitude, 1E-06f))
		{
			UnityEvent<float> unityEvent = this.onProximityChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(magnitude);
			}
		}
		this._distance = magnitude;
		float num = this.proximityRange.Approx0(1E-06f) ? 0f : MathUtils.LinearUnclamped(magnitude, this.proximityMin, this.proximityMax, 0f, 1f);
		if (!this._distanceLinear.Approx(num, 1E-06f))
		{
			UnityEvent<float> unityEvent2 = this.onProximityChangedLinear;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(num);
			}
		}
		this._distanceLinear = num;
		if (this._distanceLinear < 0f)
		{
			UnityEvent<float> unityEvent3 = this.onBelowMinProximity;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke(magnitude);
			}
		}
		if (this._distanceLinear > 1f)
		{
			UnityEvent<float> unityEvent4 = this.onAboveMaxProximity;
			if (unityEvent4 == null)
			{
				return;
			}
			unityEvent4.Invoke(magnitude);
		}
	}

	// Token: 0x040043B2 RID: 17330
	public Transform from;

	// Token: 0x040043B3 RID: 17331
	public Transform to;

	// Token: 0x040043B4 RID: 17332
	[Space]
	public float proximityMin;

	// Token: 0x040043B5 RID: 17333
	public float proximityMax = 1f;

	// Token: 0x040043B6 RID: 17334
	[Space]
	[NonSerialized]
	private float _distance;

	// Token: 0x040043B7 RID: 17335
	[NonSerialized]
	private float _distanceLinear;

	// Token: 0x040043B8 RID: 17336
	[Space]
	public UnityEvent<float> onProximityChanged;

	// Token: 0x040043B9 RID: 17337
	public UnityEvent<float> onProximityChangedLinear;

	// Token: 0x040043BA RID: 17338
	[Space]
	public UnityEvent<float> onBelowMinProximity;

	// Token: 0x040043BB RID: 17339
	public UnityEvent<float> onAboveMaxProximity;
}
