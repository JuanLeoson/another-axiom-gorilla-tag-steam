using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
public class StopwatchCosmetic : TransferrableObject
{
	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001D76 RID: 7542 RVA: 0x0009DDD4 File Offset: 0x0009BFD4
	public bool isActivating
	{
		get
		{
			return this._isActivating;
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001D77 RID: 7543 RVA: 0x0009DDDC File Offset: 0x0009BFDC
	public float activeTimeElapsed
	{
		get
		{
			return this._activeTimeElapsed;
		}
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x0009DDE4 File Offset: 0x0009BFE4
	protected override void Awake()
	{
		base.Awake();
		if (StopwatchCosmetic.gWatchToggleRPC == null)
		{
			StopwatchCosmetic.gWatchToggleRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchToggle"));
		}
		if (StopwatchCosmetic.gWatchResetRPC == null)
		{
			StopwatchCosmetic.gWatchResetRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchReset"));
		}
		this._watchToggle = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchReset);
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x0009DE68 File Offset: 0x0009C068
	internal override void OnEnable()
	{
		base.OnEnable();
		int i;
		if (!this.FetchMyViewID(out i))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = i.GetStaticHash();
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x0009DEC3 File Offset: 0x0009C0C3
	internal override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x0009DEF8 File Offset: 0x0009C0F8
	private void OnWatchToggle(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchToggle");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int millis = (int)args[2];
		this._watchFace.SetMillisElapsed(millis, true);
		this._watchFace.WatchToggle();
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x0009DF78 File Offset: 0x0009C178
	private void OnWatchReset(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchReset");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		this._watchFace.WatchReset();
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x0009DFD8 File Offset: 0x0009C1D8
	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			return false;
		}
		if (rigContainer.Rig.netView == null)
		{
			return false;
		}
		viewID = rigContainer.Rig.netView.ViewID;
		return true;
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x0009E077 File Offset: 0x0009C277
	public bool PollActivated()
	{
		if (!this._activated)
		{
			return false;
		}
		this._activated = false;
		return true;
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x0009E08C File Offset: 0x0009C28C
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isActivating)
		{
			this._activeTimeElapsed += Time.deltaTime;
		}
		if (this._isActivating && this._activeTimeElapsed > 1f)
		{
			this._isActivating = false;
			this._watchFace.WatchReset(true);
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[]
			{
				this._photonID
			});
		}
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x0009E0FF File Offset: 0x0009C2FF
	public override void OnActivate()
	{
		if (!this.CanActivate())
		{
			return;
		}
		base.OnActivate();
		if (this.IsMyItem())
		{
			this._activeTimeElapsed = 0f;
			this._isActivating = true;
		}
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x0009E12C File Offset: 0x0009C32C
	public override void OnDeactivate()
	{
		if (!this.CanDeactivate())
		{
			return;
		}
		base.OnDeactivate();
		if (!this.IsMyItem())
		{
			return;
		}
		this._isActivating = false;
		this._activated = true;
		this._watchFace.WatchToggle();
		StopwatchCosmetic.gWatchToggleRPC.RaiseOthers(new object[]
		{
			this._photonID,
			this._watchFace.watchActive,
			this._watchFace.millisElapsed
		});
		this._activated = false;
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x0009E1B5 File Offset: 0x0009C3B5
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x0009E1C0 File Offset: 0x0009C3C0
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x040025FB RID: 9723
	[SerializeField]
	private StopwatchFace _watchFace;

	// Token: 0x040025FC RID: 9724
	[Space]
	[NonSerialized]
	private bool _isActivating;

	// Token: 0x040025FD RID: 9725
	[NonSerialized]
	private float _activeTimeElapsed;

	// Token: 0x040025FE RID: 9726
	[NonSerialized]
	private bool _activated;

	// Token: 0x040025FF RID: 9727
	[Space]
	[NonSerialized]
	private int _photonID = -1;

	// Token: 0x04002600 RID: 9728
	private static PhotonEvent gWatchToggleRPC;

	// Token: 0x04002601 RID: 9729
	private static PhotonEvent gWatchResetRPC;

	// Token: 0x04002602 RID: 9730
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchToggle;

	// Token: 0x04002603 RID: 9731
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchReset;

	// Token: 0x04002604 RID: 9732
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002605 RID: 9733
	[DebugOption]
	public bool disableDeactivation;
}
