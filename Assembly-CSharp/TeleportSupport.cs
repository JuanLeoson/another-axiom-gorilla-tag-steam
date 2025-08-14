using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200033C RID: 828
public abstract class TeleportSupport : MonoBehaviour
{
	// Token: 0x1700022E RID: 558
	// (get) Token: 0x060013C7 RID: 5063 RVA: 0x0006A556 File Offset: 0x00068756
	// (set) Token: 0x060013C8 RID: 5064 RVA: 0x0006A55E File Offset: 0x0006875E
	private protected LocomotionTeleport LocomotionTeleport { protected get; private set; }

	// Token: 0x060013C9 RID: 5065 RVA: 0x0006A567 File Offset: 0x00068767
	protected virtual void OnEnable()
	{
		this.LocomotionTeleport = base.GetComponent<LocomotionTeleport>();
		this.AddEventHandlers();
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0006A57B File Offset: 0x0006877B
	protected virtual void OnDisable()
	{
		this.RemoveEventHandlers();
		this.LocomotionTeleport = null;
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x0006A58A File Offset: 0x0006878A
	[Conditional("DEBUG_TELEPORT_EVENT_HANDLERS")]
	private void LogEventHandler(string msg)
	{
		Debug.Log("EventHandler: " + base.GetType().Name + ": " + msg);
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0006A5AC File Offset: 0x000687AC
	protected virtual void AddEventHandlers()
	{
		this._eventsActive = true;
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x0006A5B5 File Offset: 0x000687B5
	protected virtual void RemoveEventHandlers()
	{
		this._eventsActive = false;
	}

	// Token: 0x04001B4B RID: 6987
	private bool _eventsActive;
}
