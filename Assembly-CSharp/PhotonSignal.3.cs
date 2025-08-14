using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000A36 RID: 2614
[Serializable]
public class PhotonSignal<T1, T2> : PhotonSignal
{
	// Token: 0x17000607 RID: 1543
	// (get) Token: 0x06003FE4 RID: 16356 RVA: 0x00012237 File Offset: 0x00010437
	public override int argCount
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x14000070 RID: 112
	// (add) Token: 0x06003FE5 RID: 16357 RVA: 0x001448EF File Offset: 0x00142AEF
	// (remove) Token: 0x06003FE6 RID: 16358 RVA: 0x00144923 File Offset: 0x00142B23
	public new event OnSignalReceived<T1, T2> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003FE7 RID: 16359 RVA: 0x001447B7 File Offset: 0x001429B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06003FE8 RID: 16360 RVA: 0x001447C0 File Offset: 0x001429C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06003FE9 RID: 16361 RVA: 0x00144940 File Offset: 0x00142B40
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06003FEA RID: 16362 RVA: 0x0014494F File Offset: 0x00142B4F
	public void Raise(T1 arg1, T2 arg2)
	{
		this.Raise(this._receivers, arg1, arg2);
	}

	// Token: 0x06003FEB RID: 16363 RVA: 0x00144960 File Offset: 0x00142B60
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2)
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
		array[3] = arg2;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06003FEC RID: 16364 RVA: 0x00144A08 File Offset: 0x00142C08
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		if (!args.TryParseArgs(2, out arg, out arg2))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2>(this._callbacks, arg, arg2, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2>(this._callbacks, arg, arg2, info);
	}

	// Token: 0x06003FED RID: 16365 RVA: 0x00144A48 File Offset: 0x00142C48
	public new static implicit operator PhotonSignal<T1, T2>(string s)
	{
		return new PhotonSignal<T1, T2>(s);
	}

	// Token: 0x06003FEE RID: 16366 RVA: 0x00144A50 File Offset: 0x00142C50
	public new static explicit operator PhotonSignal<T1, T2>(int i)
	{
		return new PhotonSignal<T1, T2>(i);
	}

	// Token: 0x04004BD1 RID: 19409
	private OnSignalReceived<T1, T2> _callbacks;

	// Token: 0x04004BD2 RID: 19410
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2>).FullName.GetStaticHash();
}
