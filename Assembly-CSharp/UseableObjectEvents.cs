using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
public class UseableObjectEvents : MonoBehaviour
{
	// Token: 0x06001DCD RID: 7629 RVA: 0x0009F9CC File Offset: 0x0009DBCC
	public void Init(NetPlayer player)
	{
		bool isLocal = player.IsLocal;
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string str;
		if (isLocal && instance != null)
		{
			str = instance.GetPlayFabPlayerId();
		}
		else
		{
			str = player.NickName;
		}
		this.PlayerIdString = str + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.DisposeEvents();
		this.Activate = new PhotonEvent(this.PlayerId.ToString() + ".Activate");
		this.Deactivate = new PhotonEvent(this.PlayerId.ToString() + ".Deactivate");
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x06001DCE RID: 7630 RVA: 0x0009FA8D File Offset: 0x0009DC8D
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

	// Token: 0x06001DCF RID: 7631 RVA: 0x0009FAB0 File Offset: 0x0009DCB0
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

	// Token: 0x06001DD0 RID: 7632 RVA: 0x0009FAD3 File Offset: 0x0009DCD3
	private void OnDestroy()
	{
		this.DisposeEvents();
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x0009FADB File Offset: 0x0009DCDB
	private void DisposeEvents()
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

	// Token: 0x04002662 RID: 9826
	[NonSerialized]
	private string PlayerIdString;

	// Token: 0x04002663 RID: 9827
	[NonSerialized]
	private int PlayerId;

	// Token: 0x04002664 RID: 9828
	public PhotonEvent Activate;

	// Token: 0x04002665 RID: 9829
	public PhotonEvent Deactivate;
}
