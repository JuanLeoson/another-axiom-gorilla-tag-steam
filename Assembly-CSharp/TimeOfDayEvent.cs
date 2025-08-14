using System;
using UnityEngine;

// Token: 0x02000B1D RID: 2845
public class TimeOfDayEvent : TimeEvent
{
	// Token: 0x17000677 RID: 1655
	// (get) Token: 0x06004484 RID: 17540 RVA: 0x00156731 File Offset: 0x00154931
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x17000678 RID: 1656
	// (get) Token: 0x06004485 RID: 17541 RVA: 0x00156739 File Offset: 0x00154939
	// (set) Token: 0x06004486 RID: 17542 RVA: 0x00156741 File Offset: 0x00154941
	public float timeStart
	{
		get
		{
			return this._timeStart;
		}
		set
		{
			this._timeStart = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000679 RID: 1657
	// (get) Token: 0x06004487 RID: 17543 RVA: 0x0015674F File Offset: 0x0015494F
	// (set) Token: 0x06004488 RID: 17544 RVA: 0x00156757 File Offset: 0x00154957
	public float timeEnd
	{
		get
		{
			return this._timeEnd;
		}
		set
		{
			this._timeEnd = Mathf.Clamp01(value);
		}
	}

	// Token: 0x1700067A RID: 1658
	// (get) Token: 0x06004489 RID: 17545 RVA: 0x00156765 File Offset: 0x00154965
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x0600448A RID: 17546 RVA: 0x00156770 File Offset: 0x00154970
	private void Start()
	{
		if (!this._dayNightManager)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (!this._dayNightManager)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x0600448B RID: 17547 RVA: 0x001567F2 File Offset: 0x001549F2
	private void Update()
	{
		this._elapsed += Time.deltaTime;
		if (this._elapsed < 1f)
		{
			return;
		}
		this._elapsed = 0f;
		this.UpdateTime();
	}

	// Token: 0x0600448C RID: 17548 RVA: 0x00156828 File Offset: 0x00154A28
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
		bool flag = this._currentTime >= 0f && this._currentTime >= this._timeStart && this._currentTime <= this._timeEnd;
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
	}

	// Token: 0x0600448D RID: 17549 RVA: 0x001568BF File Offset: 0x00154ABF
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x04004EAD RID: 20141
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x04004EAE RID: 20142
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x04004EAF RID: 20143
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x04004EB0 RID: 20144
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x04004EB1 RID: 20145
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x04004EB2 RID: 20146
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x04004EB3 RID: 20147
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
