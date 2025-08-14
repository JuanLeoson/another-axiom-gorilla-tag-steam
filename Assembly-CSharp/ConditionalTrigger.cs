using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000566 RID: 1382
public class ConditionalTrigger : MonoBehaviour, IRigAware
{
	// Token: 0x1700035F RID: 863
	// (get) Token: 0x060021AF RID: 8623 RVA: 0x000B73F0 File Offset: 0x000B55F0
	private int intValue
	{
		get
		{
			return (int)this._tracking;
		}
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000B73F8 File Offset: 0x000B55F8
	public void SetProximityFromRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._from = this._rig.transform;
		}
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000B7436 File Offset: 0x000B5636
	public void SetProximityToRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._to = this._rig.transform;
		}
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000B7474 File Offset: 0x000B5674
	public void SetProximityFrom(Transform from)
	{
		this._from = from;
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000B747D File Offset: 0x000B567D
	public void SetProxmityTo(Transform to)
	{
		this._to = to;
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000B7486 File Offset: 0x000B5686
	public void TrackedSet(TriggerCondition conditions)
	{
		this._tracking = conditions;
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000B748F File Offset: 0x000B568F
	public void TrackedAdd(TriggerCondition conditions)
	{
		this._tracking |= conditions;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000B749F File Offset: 0x000B569F
	public void TrackedRemove(TriggerCondition conditions)
	{
		this._tracking &= ~conditions;
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000B7486 File Offset: 0x000B5686
	public void TrackedSet(int conditions)
	{
		this._tracking = (TriggerCondition)conditions;
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x000B748F File Offset: 0x000B568F
	public void TrackedAdd(int conditions)
	{
		this._tracking |= (TriggerCondition)conditions;
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x000B749F File Offset: 0x000B569F
	public void TrackedRemove(int conditions)
	{
		this._tracking &= (TriggerCondition)(~(TriggerCondition)conditions);
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x000B74B0 File Offset: 0x000B56B0
	public void TrackedClear()
	{
		this._tracking = TriggerCondition.None;
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000B74B9 File Offset: 0x000B56B9
	private void OnEnable()
	{
		this._timeSince = 0f;
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000B74CB File Offset: 0x000B56CB
	private void Update()
	{
		if (this.IsTracking(TriggerCondition.TimeElapsed))
		{
			this.TrackTimeElapsed();
		}
		if (this.IsTracking(TriggerCondition.Proximity))
		{
			this.TrackProximity();
			return;
		}
		this._distance = 0f;
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000B74F7 File Offset: 0x000B56F7
	private void TrackTimeElapsed()
	{
		if (this._timeSince.HasElapsed(this._interval, true))
		{
			UnityEvent unityEvent = this.onTimeElapsed;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000B7520 File Offset: 0x000B5720
	private void TrackProximity()
	{
		if (!this._from || !this._to)
		{
			this._distance = 0f;
			return;
		}
		this._distance = Vector3.Distance(this._to.position, this._from.position);
		if (this._distance >= this._maxDistance)
		{
			UnityEvent unityEvent = this.onMaxDistance;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x000B7592 File Offset: 0x000B5792
	private bool IsTracking(TriggerCondition condition)
	{
		return (this._tracking & condition) == condition;
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x000B759F File Offset: 0x000B579F
	private static void FindRig(out VRRig rig)
	{
		if (PhotonNetwork.InRoom)
		{
			rig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.Get(PhotonNetwork.LocalPlayer));
			return;
		}
		rig = VRRig.LocalRig;
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000B75C1 File Offset: 0x000B57C1
	public void SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x04002B1F RID: 11039
	[Space]
	[SerializeField]
	private TriggerCondition _tracking;

	// Token: 0x04002B20 RID: 11040
	[Space]
	[SerializeField]
	private Transform _from;

	// Token: 0x04002B21 RID: 11041
	[SerializeField]
	private Transform _to;

	// Token: 0x04002B22 RID: 11042
	[SerializeField]
	private float _maxDistance;

	// Token: 0x04002B23 RID: 11043
	[NonSerialized]
	private float _distance;

	// Token: 0x04002B24 RID: 11044
	[Space]
	public UnityEvent onMaxDistance;

	// Token: 0x04002B25 RID: 11045
	[SerializeField]
	private float _interval = 1f;

	// Token: 0x04002B26 RID: 11046
	[NonSerialized]
	private TimeSince _timeSince;

	// Token: 0x04002B27 RID: 11047
	[Space]
	public UnityEvent onTimeElapsed;

	// Token: 0x04002B28 RID: 11048
	[Space]
	private VRRig _rig;
}
