using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B3 RID: 1203
[RequireComponent(typeof(UseableObjectEvents))]
public class UseableObject : TransferrableObject
{
	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001DBF RID: 7615 RVA: 0x0009F7CF File Offset: 0x0009D9CF
	public bool isMidUse
	{
		get
		{
			return this._isMidUse;
		}
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001DC0 RID: 7616 RVA: 0x0009F7D7 File Offset: 0x0009D9D7
	public float useTimeElapsed
	{
		get
		{
			return this._useTimeElapsed;
		}
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001DC1 RID: 7617 RVA: 0x0009F7DF File Offset: 0x0009D9DF
	public bool justUsed
	{
		get
		{
			if (!this._justUsed)
			{
				return false;
			}
			this._justUsed = false;
			return true;
		}
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x0009F7F3 File Offset: 0x0009D9F3
	protected override void Awake()
	{
		base.Awake();
		this._events = base.gameObject.GetOrAddComponent<UseableObjectEvents>();
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x0009F80C File Offset: 0x0009DA0C
	internal override void OnEnable()
	{
		base.OnEnable();
		UseableObjectEvents events = this._events;
		VRRig myOnlineRig = base.myOnlineRig;
		NetPlayer player;
		if ((player = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
		{
			VRRig myRig = base.myRig;
			player = ((myRig != null) ? myRig.creator : null);
		}
		events.Init(player);
		this._events.Activate += this.OnObjectActivated;
		this._events.Deactivate += this.OnObjectDeactivated;
	}

	// Token: 0x06001DC4 RID: 7620 RVA: 0x0009F896 File Offset: 0x0009DA96
	internal override void OnDisable()
	{
		base.OnDisable();
		Object.Destroy(this._events);
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x0009F8A9 File Offset: 0x0009DAA9
	private void OnObjectActivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x0009F8A9 File Offset: 0x0009DAA9
	private void OnObjectDeactivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x0009F8AF File Offset: 0x0009DAAF
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isMidUse)
		{
			this._useTimeElapsed += Time.deltaTime;
		}
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x0009F8D4 File Offset: 0x0009DAD4
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onActivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._useTimeElapsed = 0f;
			this._isMidUse = true;
		}
		if (this._raiseActivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent activate = events.Activate;
			if (activate == null)
			{
				return;
			}
			activate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x06001DC9 RID: 7625 RVA: 0x0009F93C File Offset: 0x0009DB3C
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onDeactivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._isMidUse = false;
			this._justUsed = true;
		}
		if (this._raiseDeactivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent deactivate = events.Deactivate;
			if (deactivate == null)
			{
				return;
			}
			deactivate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x06001DCA RID: 7626 RVA: 0x0009F99D File Offset: 0x0009DB9D
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001DCB RID: 7627 RVA: 0x0009F9A8 File Offset: 0x0009DBA8
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002655 RID: 9813
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002656 RID: 9814
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04002657 RID: 9815
	[SerializeField]
	private UseableObjectEvents _events;

	// Token: 0x04002658 RID: 9816
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04002659 RID: 9817
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x0400265A RID: 9818
	[NonSerialized]
	private DateTime _lastActivate;

	// Token: 0x0400265B RID: 9819
	[NonSerialized]
	private DateTime _lastDeactivate;

	// Token: 0x0400265C RID: 9820
	[NonSerialized]
	private bool _isMidUse;

	// Token: 0x0400265D RID: 9821
	[NonSerialized]
	private float _useTimeElapsed;

	// Token: 0x0400265E RID: 9822
	[NonSerialized]
	private bool _justUsed;

	// Token: 0x0400265F RID: 9823
	[NonSerialized]
	private int tempHandPos;

	// Token: 0x04002660 RID: 9824
	public UnityEvent onActivateLocal;

	// Token: 0x04002661 RID: 9825
	public UnityEvent onDeactivateLocal;
}
