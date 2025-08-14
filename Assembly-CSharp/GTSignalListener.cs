using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200073E RID: 1854
public class GTSignalListener : MonoBehaviour
{
	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x06002E5A RID: 11866 RVA: 0x000F55DF File Offset: 0x000F37DF
	// (set) Token: 0x06002E5B RID: 11867 RVA: 0x000F55E7 File Offset: 0x000F37E7
	public int rigActorID { get; private set; } = -1;

	// Token: 0x06002E5C RID: 11868 RVA: 0x000F55F0 File Offset: 0x000F37F0
	private void Awake()
	{
		this.OnListenerAwake();
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x000F55F8 File Offset: 0x000F37F8
	private void OnEnable()
	{
		this.RefreshActorID();
		this.OnListenerEnable();
		GTSignalRelay.Register(this);
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x000F560C File Offset: 0x000F380C
	private void OnDisable()
	{
		GTSignalRelay.Unregister(this);
		this.OnListenerDisable();
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000F561A File Offset: 0x000F381A
	private void RefreshActorID()
	{
		this.rig = base.GetComponentInParent<VRRig>(true);
		int rigActorID;
		if (!(this.rig == null))
		{
			NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
			rigActorID = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : -1);
		}
		else
		{
			rigActorID = -1;
		}
		this.rigActorID = rigActorID;
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x000F5657 File Offset: 0x000F3857
	public virtual bool IsReady()
	{
		return this._callLimits.CheckCallTime(Time.time);
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnListenerAwake()
	{
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnListenerEnable()
	{
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnListenerDisable()
	{
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void HandleSignalReceived(int sender, object[] args)
	{
	}

	// Token: 0x04003A26 RID: 14886
	[Space]
	public GTSignalID signal;

	// Token: 0x04003A27 RID: 14887
	[Space]
	public VRRig rig;

	// Token: 0x04003A29 RID: 14889
	[Space]
	public bool deafen;

	// Token: 0x04003A2A RID: 14890
	[FormerlySerializedAs("listenToRigOnly")]
	public bool listenToSelfOnly;

	// Token: 0x04003A2B RID: 14891
	public bool ignoreSelf;

	// Token: 0x04003A2C RID: 14892
	[Space]
	public bool callUnityEvent = true;

	// Token: 0x04003A2D RID: 14893
	[Space]
	[SerializeField]
	private CallLimiter _callLimits = new CallLimiter(10, 0.25f, 0.5f);

	// Token: 0x04003A2E RID: 14894
	[Space]
	public UnityEvent onSignalReceived;
}
