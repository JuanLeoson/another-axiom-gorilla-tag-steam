using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007A3 RID: 1955
public class RadialBounds : MonoBehaviour
{
	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x06003125 RID: 12581 RVA: 0x00100448 File Offset: 0x000FE648
	// (set) Token: 0x06003126 RID: 12582 RVA: 0x00100450 File Offset: 0x000FE650
	public Vector3 localCenter
	{
		get
		{
			return this._localCenter;
		}
		set
		{
			this._localCenter = value;
		}
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06003127 RID: 12583 RVA: 0x00100459 File Offset: 0x000FE659
	// (set) Token: 0x06003128 RID: 12584 RVA: 0x00100461 File Offset: 0x000FE661
	public float localRadius
	{
		get
		{
			return this._localRadius;
		}
		set
		{
			this._localRadius = value;
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x06003129 RID: 12585 RVA: 0x0010046A File Offset: 0x000FE66A
	public Vector3 center
	{
		get
		{
			return base.transform.TransformPoint(this._localCenter);
		}
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x0600312A RID: 12586 RVA: 0x0010047D File Offset: 0x000FE67D
	public float radius
	{
		get
		{
			return MathUtils.GetScaledRadius(this._localRadius, base.transform.lossyScale);
		}
	}

	// Token: 0x04003CC8 RID: 15560
	[SerializeField]
	private Vector3 _localCenter;

	// Token: 0x04003CC9 RID: 15561
	[SerializeField]
	private float _localRadius = 1f;

	// Token: 0x04003CCA RID: 15562
	[Space]
	public UnityEvent<RadialBounds> onOverlapEnter;

	// Token: 0x04003CCB RID: 15563
	public UnityEvent<RadialBounds> onOverlapExit;

	// Token: 0x04003CCC RID: 15564
	public UnityEvent<RadialBounds, float> onOverlapStay;
}
