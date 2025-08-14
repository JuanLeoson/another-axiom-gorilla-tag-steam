using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004AF RID: 1199
public class RubberDuckEvents : MonoBehaviour
{
	// Token: 0x06001DA8 RID: 7592 RVA: 0x0009F2E0 File Offset: 0x0009D4E0
	public void Init(NetPlayer player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool isLocal = player.IsLocal;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (isLocal && instance != null)
			{
				text = instance.GetPlayFabPlayerId();
			}
			else
			{
				text = player.NickName;
			}
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.Dispose();
		this.Activate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Activate"));
		this.Deactivate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Deactivate"));
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x06001DA9 RID: 7593 RVA: 0x0009F3BA File Offset: 0x0009D5BA
	private void OnEnable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Enable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Enable();
	}

	// Token: 0x06001DAA RID: 7594 RVA: 0x0009F3DD File Offset: 0x0009D5DD
	private void OnDisable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Disable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Disable();
	}

	// Token: 0x06001DAB RID: 7595 RVA: 0x0009F400 File Offset: 0x0009D600
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x06001DAC RID: 7596 RVA: 0x0009F408 File Offset: 0x0009D608
	public void Dispose()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Dispose();
		}
		this.Activate = null;
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate != null)
		{
			deactivate.Dispose();
		}
		this.Deactivate = null;
	}

	// Token: 0x04002642 RID: 9794
	public int PlayerId;

	// Token: 0x04002643 RID: 9795
	public string PlayerIdString;

	// Token: 0x04002644 RID: 9796
	public PhotonEvent Activate;

	// Token: 0x04002645 RID: 9797
	public PhotonEvent Deactivate;
}
