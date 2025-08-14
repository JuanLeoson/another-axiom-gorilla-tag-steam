using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000A35 RID: 2613
[Serializable]
public class PhotonSignal<T1> : PhotonSignal
{
	// Token: 0x17000606 RID: 1542
	// (get) Token: 0x06003FD8 RID: 16344 RVA: 0x0001D558 File Offset: 0x0001B758
	public override int argCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x1400006F RID: 111
	// (add) Token: 0x06003FD9 RID: 16345 RVA: 0x00144766 File Offset: 0x00142966
	// (remove) Token: 0x06003FDA RID: 16346 RVA: 0x0014479A File Offset: 0x0014299A
	public new event OnSignalReceived<T1> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003FDB RID: 16347 RVA: 0x001447B7 File Offset: 0x001429B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06003FDC RID: 16348 RVA: 0x001447C0 File Offset: 0x001429C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06003FDD RID: 16349 RVA: 0x001447C9 File Offset: 0x001429C9
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06003FDE RID: 16350 RVA: 0x001447D8 File Offset: 0x001429D8
	public void Raise(T1 arg1)
	{
		this.Raise(this._receivers, arg1);
	}

	// Token: 0x06003FDF RID: 16351 RVA: 0x001447E8 File Offset: 0x001429E8
	public void Raise(ReceiverGroup receivers, T1 arg1)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		RaiseEventOptions raiseEventOptions = PhotonSignal.gGroupToOptions[receivers];
		object[] array = PhotonUtils.FetchScratchArray(2 + this.argCount);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		array[2] = arg1;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06003FE0 RID: 16352 RVA: 0x00144888 File Offset: 0x00142A88
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		if (!args.TryParseArgs(2, out arg))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1>(this._callbacks, arg, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1>(this._callbacks, arg, info);
	}

	// Token: 0x06003FE1 RID: 16353 RVA: 0x001448C4 File Offset: 0x00142AC4
	public new static implicit operator PhotonSignal<T1>(string s)
	{
		return new PhotonSignal<T1>(s);
	}

	// Token: 0x06003FE2 RID: 16354 RVA: 0x001448CC File Offset: 0x00142ACC
	public new static explicit operator PhotonSignal<T1>(int i)
	{
		return new PhotonSignal<T1>(i);
	}

	// Token: 0x04004BCF RID: 19407
	private OnSignalReceived<T1> _callbacks;

	// Token: 0x04004BD0 RID: 19408
	private static readonly int kSignature = typeof(PhotonSignal<T1>).FullName.GetStaticHash();
}
