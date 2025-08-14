using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
[DisallowMultipleComponent]
public class GorillaHandSocket : MonoBehaviour
{
	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06002B74 RID: 11124 RVA: 0x000E5E88 File Offset: 0x000E4088
	public GorillaHandNode attachedHand
	{
		get
		{
			return this._attachedHand;
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x06002B75 RID: 11125 RVA: 0x000E5E90 File Offset: 0x000E4090
	public bool inUse
	{
		get
		{
			return this._inUse;
		}
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x000E5E98 File Offset: 0x000E4098
	public static bool FetchSocket(Collider collider, out GorillaHandSocket socket)
	{
		return GorillaHandSocket.gColliderToSocket.TryGetValue(collider, out socket);
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000E5EA6 File Offset: 0x000E40A6
	public bool CanAttach()
	{
		return !this._inUse && this._sinceSocketStateChange.HasElapsed(this.attachCooldown, true);
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x000E5EC4 File Offset: 0x000E40C4
	public void Attach(GorillaHandNode hand)
	{
		if (!this.CanAttach())
		{
			return;
		}
		if (hand == null)
		{
			return;
		}
		hand.attachedToSocket = this;
		this._attachedHand = hand;
		this._inUse = true;
		this.OnHandAttach();
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x000E5EF4 File Offset: 0x000E40F4
	public void Detach()
	{
		GorillaHandNode gorillaHandNode;
		this.Detach(out gorillaHandNode);
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x000E5F0C File Offset: 0x000E410C
	public void Detach(out GorillaHandNode hand)
	{
		if (this._inUse)
		{
			this._inUse = false;
		}
		if (this._attachedHand == null)
		{
			hand = null;
			return;
		}
		hand = this._attachedHand;
		hand.attachedToSocket = null;
		this._attachedHand = null;
		this.OnHandDetach();
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnHandAttach()
	{
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnHandDetach()
	{
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x000E5F62 File Offset: 0x000E4162
	protected virtual void OnUpdateAttached()
	{
		this._attachedHand.transform.position = base.transform.position;
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000E5F7F File Offset: 0x000E417F
	private void OnEnable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.TryAdd(this.collider, this);
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000E5FA2 File Offset: 0x000E41A2
	private void OnDisable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.Remove(this.collider);
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x000E5FC4 File Offset: 0x000E41C4
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x000E5FCC File Offset: 0x000E41CC
	private void FixedUpdate()
	{
		if (!this._inUse)
		{
			return;
		}
		if (!this._attachedHand)
		{
			return;
		}
		this.OnUpdateAttached();
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x000E5FEC File Offset: 0x000E41EC
	private void Setup()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(UnityTag.GorillaHandSocket);
		base.gameObject.SetLayer(UnityLayer.GorillaHandSocket);
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x040036C4 RID: 14020
	public Collider collider;

	// Token: 0x040036C5 RID: 14021
	public float attachCooldown = 0.5f;

	// Token: 0x040036C6 RID: 14022
	public HandSocketConstraint constraint;

	// Token: 0x040036C7 RID: 14023
	[NonSerialized]
	private GorillaHandNode _attachedHand;

	// Token: 0x040036C8 RID: 14024
	[NonSerialized]
	private bool _inUse;

	// Token: 0x040036C9 RID: 14025
	[NonSerialized]
	private TimeSince _sinceSocketStateChange;

	// Token: 0x040036CA RID: 14026
	private static readonly Dictionary<Collider, GorillaHandSocket> gColliderToSocket = new Dictionary<Collider, GorillaHandSocket>(64);
}
