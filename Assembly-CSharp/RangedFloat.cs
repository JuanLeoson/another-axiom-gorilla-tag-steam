using System;
using UnityEngine;

// Token: 0x020007A6 RID: 1958
public class RangedFloat : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x06003134 RID: 12596 RVA: 0x001008CB File Offset: 0x000FEACB
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x06003135 RID: 12597 RVA: 0x001008D3 File Offset: 0x000FEAD3
	public float Range
	{
		get
		{
			return this._max - this._min;
		}
	}

	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x06003136 RID: 12598 RVA: 0x001008E2 File Offset: 0x000FEAE2
	// (set) Token: 0x06003137 RID: 12599 RVA: 0x001008EA File Offset: 0x000FEAEA
	public float Min
	{
		get
		{
			return this._min;
		}
		set
		{
			this._min = value;
		}
	}

	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x06003138 RID: 12600 RVA: 0x001008F3 File Offset: 0x000FEAF3
	// (set) Token: 0x06003139 RID: 12601 RVA: 0x001008FB File Offset: 0x000FEAFB
	public float Max
	{
		get
		{
			return this._max;
		}
		set
		{
			this._max = value;
		}
	}

	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x0600313A RID: 12602 RVA: 0x00100904 File Offset: 0x000FEB04
	// (set) Token: 0x0600313B RID: 12603 RVA: 0x00100939 File Offset: 0x000FEB39
	public float normalized
	{
		get
		{
			if (!this.Range.Approx0(1E-06f))
			{
				return (this._value - this._min) / (this._max - this.Min);
			}
			return 0f;
		}
		set
		{
			this._value = this._min + Mathf.Clamp01(value) * (this._max - this._min);
		}
	}

	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x0600313C RID: 12604 RVA: 0x0010095C File Offset: 0x000FEB5C
	public float curved
	{
		get
		{
			return this._min + this._curve.Evaluate(this.normalized) * (this._max - this._min);
		}
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x00100984 File Offset: 0x000FEB84
	public float Get()
	{
		return this._value;
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x0010098C File Offset: 0x000FEB8C
	public void Set(float f)
	{
		this._value = Mathf.Clamp(f, this._min, this._max);
	}

	// Token: 0x04003CDB RID: 15579
	[SerializeField]
	private float _value = 0.5f;

	// Token: 0x04003CDC RID: 15580
	[SerializeField]
	private float _min;

	// Token: 0x04003CDD RID: 15581
	[SerializeField]
	private float _max = 1f;

	// Token: 0x04003CDE RID: 15582
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
