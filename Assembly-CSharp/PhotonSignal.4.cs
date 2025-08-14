using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000A37 RID: 2615
[Serializable]
public class PhotonSignal<T1, T2, T3> : PhotonSignal
{
	// Token: 0x17000608 RID: 1544
	// (get) Token: 0x06003FF0 RID: 16368 RVA: 0x000EA036 File Offset: 0x000E8236
	public override int argCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06003FF1 RID: 16369 RVA: 0x00144A73 File Offset: 0x00142C73
	// (remove) Token: 0x06003FF2 RID: 16370 RVA: 0x00144AA7 File Offset: 0x00142CA7
	public new event OnSignalReceived<T1, T2, T3> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003FF3 RID: 16371 RVA: 0x001447B7 File Offset: 0x001429B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06003FF4 RID: 16372 RVA: 0x001447C0 File Offset: 0x001429C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06003FF5 RID: 16373 RVA: 0x00144AC4 File Offset: 0x00142CC4
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06003FF6 RID: 16374 RVA: 0x00144AD3 File Offset: 0x00142CD3
	public void Raise(T1 arg1, T2 arg2, T3 arg3)
	{
		this.Raise(this._receivers, arg1, arg2, arg3);
	}

	// Token: 0x06003FF7 RID: 16375 RVA: 0x00144AE4 File Offset: 0x00142CE4
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3)
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
		array[4] = arg3;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06003FF8 RID: 16376 RVA: 0x00144B94 File Offset: 0x00142D94
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3>(this._callbacks, arg, arg2, arg3, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3>(this._callbacks, arg, arg2, arg3, info);
	}

	// Token: 0x06003FF9 RID: 16377 RVA: 0x00144BD8 File Offset: 0x00142DD8
	public new static implicit operator PhotonSignal<T1, T2, T3>(string s)
	{
		return new PhotonSignal<T1, T2, T3>(s);
	}

	// Token: 0x06003FFA RID: 16378 RVA: 0x00144BE0 File Offset: 0x00142DE0
	public new static explicit operator PhotonSignal<T1, T2, T3>(int i)
	{
		return new PhotonSignal<T1, T2, T3>(i);
	}

	// Token: 0x04004BD3 RID: 19411
	private OnSignalReceived<T1, T2, T3> _callbacks;

	// Token: 0x04004BD4 RID: 19412
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3>).FullName.GetStaticHash();
}
