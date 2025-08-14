using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200032E RID: 814
public abstract class TeleportInputHandler : TeleportSupport
{
	// Token: 0x0600138C RID: 5004 RVA: 0x000699CD File Offset: 0x00067BCD
	protected TeleportInputHandler()
	{
		this._startReadyAction = delegate()
		{
			base.StartCoroutine(this.TeleportReadyCoroutine());
		};
		this._startAimAction = delegate()
		{
			base.StartCoroutine(this.TeleportAimCoroutine());
		};
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x000699F9 File Offset: 0x00067BF9
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.InputHandler = this;
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateReady += this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x00069A30 File Offset: 0x00067C30
	protected override void RemoveEventHandlers()
	{
		if (base.LocomotionTeleport.InputHandler == this)
		{
			base.LocomotionTeleport.InputHandler = null;
		}
		base.LocomotionTeleport.EnterStateReady -= this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
		base.RemoveEventHandlers();
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x00069A84 File Offset: 0x00067C84
	private IEnumerator TeleportReadyCoroutine()
	{
		while (this.GetIntention() != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		base.LocomotionTeleport.CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
		yield break;
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x00069A93 File Offset: 0x00067C93
	private IEnumerator TeleportAimCoroutine()
	{
		LocomotionTeleport.TeleportIntentions intention = this.GetIntention();
		while (intention == LocomotionTeleport.TeleportIntentions.Aim || intention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			base.LocomotionTeleport.CurrentIntention = intention;
			yield return null;
			intention = this.GetIntention();
		}
		base.LocomotionTeleport.CurrentIntention = intention;
		yield break;
	}

	// Token: 0x06001391 RID: 5009
	public abstract LocomotionTeleport.TeleportIntentions GetIntention();

	// Token: 0x06001392 RID: 5010
	public abstract void GetAimData(out Ray aimRay);

	// Token: 0x04001B0E RID: 6926
	private readonly Action _startReadyAction;

	// Token: 0x04001B0F RID: 6927
	private readonly Action _startAimAction;
}
