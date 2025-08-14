using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007A4 RID: 1956
public class RadialBoundsTrigger : MonoBehaviour
{
	// Token: 0x0600312C RID: 12588 RVA: 0x001004A8 File Offset: 0x000FE6A8
	public void TestOverlap()
	{
		this.TestOverlap(this._raiseEvents);
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x001004B8 File Offset: 0x000FE6B8
	public void TestOverlap(bool raiseEvents)
	{
		if (!this.object1 || !this.object2)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			return;
		}
		float time = Time.time;
		float num = this.object1.radius + this.object2.radius;
		bool flag = (this.object2.center - this.object1.center).sqrMagnitude <= num * num;
		if (this._overlapping && flag)
		{
			this._overlapping = true;
			this._timeSpentInOverlap = time - this._timeOverlapStarted;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds, float> onOverlapStay = this.object1.onOverlapStay;
				if (onOverlapStay != null)
				{
					onOverlapStay.Invoke(this.object2, this._timeSpentInOverlap);
				}
				UnityEvent<RadialBounds, float> onOverlapStay2 = this.object2.onOverlapStay;
				if (onOverlapStay2 == null)
				{
					return;
				}
				onOverlapStay2.Invoke(this.object1, this._timeSpentInOverlap);
				return;
			}
		}
		else if (!this._overlapping && flag)
		{
			if (time - this._timeOverlapStopped < this.hysteresis)
			{
				return;
			}
			this._overlapping = true;
			this._timeOverlapStarted = time;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapEnter = this.object1.onOverlapEnter;
				if (onOverlapEnter != null)
				{
					onOverlapEnter.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapEnter2 = this.object2.onOverlapEnter;
				if (onOverlapEnter2 == null)
				{
					return;
				}
				onOverlapEnter2.Invoke(this.object1);
				return;
			}
		}
		else if (!flag && this._overlapping)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = time;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
				if (onOverlapExit != null)
				{
					onOverlapExit.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
				if (onOverlapExit2 == null)
				{
					return;
				}
				onOverlapExit2.Invoke(this.object1);
			}
		}
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x001006A4 File Offset: 0x000FE8A4
	private void FixedUpdate()
	{
		this.TestOverlap();
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x001006AC File Offset: 0x000FE8AC
	private void OnDisable()
	{
		if (this._raiseEvents && this.object1 && this.object2 && this._overlapping)
		{
			UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
			if (onOverlapExit != null)
			{
				onOverlapExit.Invoke(this.object2);
			}
			UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
			if (onOverlapExit2 != null)
			{
				onOverlapExit2.Invoke(this.object1);
			}
		}
		this._timeOverlapStarted = -1f;
		this._timeSpentInOverlap = 0f;
		this._overlapping = false;
	}

	// Token: 0x04003CCD RID: 15565
	[SerializeField]
	private Id32 _triggerID;

	// Token: 0x04003CCE RID: 15566
	[Space]
	public RadialBounds object1 = new RadialBounds();

	// Token: 0x04003CCF RID: 15567
	[Space]
	public RadialBounds object2 = new RadialBounds();

	// Token: 0x04003CD0 RID: 15568
	[Space]
	public float hysteresis = 0.5f;

	// Token: 0x04003CD1 RID: 15569
	[SerializeField]
	private bool _raiseEvents = true;

	// Token: 0x04003CD2 RID: 15570
	[Space]
	private bool _overlapping;

	// Token: 0x04003CD3 RID: 15571
	private float _timeSpentInOverlap;

	// Token: 0x04003CD4 RID: 15572
	[Space]
	private float _timeOverlapStarted;

	// Token: 0x04003CD5 RID: 15573
	private float _timeOverlapStopped;
}
