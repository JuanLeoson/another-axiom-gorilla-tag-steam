using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000A38 RID: 2616
[Serializable]
public class PhotonSignal<T1, T2, T3, T4> : PhotonSignal
{
	// Token: 0x17000609 RID: 1545
	// (get) Token: 0x06003FFC RID: 16380 RVA: 0x00144C03 File Offset: 0x00142E03
	public override int argCount
	{
		get
		{
			return 4;
		}
	}

	// Token: 0x14000072 RID: 114
	// (add) Token: 0x06003FFD RID: 16381 RVA: 0x00144C06 File Offset: 0x00142E06
	// (remove) Token: 0x06003FFE RID: 16382 RVA: 0x00144C3A File Offset: 0x00142E3A
	public new event OnSignalReceived<T1, T2, T3, T4> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003FFF RID: 16383 RVA: 0x001447B7 File Offset: 0x001429B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06004000 RID: 16384 RVA: 0x001447C0 File Offset: 0x001429C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06004001 RID: 16385 RVA: 0x00144C57 File Offset: 0x00142E57
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06004002 RID: 16386 RVA: 0x00144C66 File Offset: 0x00142E66
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4);
	}

	// Token: 0x06004003 RID: 16387 RVA: 0x00144C7C File Offset: 0x00142E7C
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
		array[5] = arg4;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004004 RID: 16388 RVA: 0x00144D38 File Offset: 0x00142F38
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		T4 arg4;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3, out arg4))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4>(this._callbacks, arg, arg2, arg3, arg4, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4>(this._callbacks, arg, arg2, arg3, arg4, info);
	}

	// Token: 0x06004005 RID: 16389 RVA: 0x00144D80 File Offset: 0x00142F80
	public new static implicit operator PhotonSignal<T1, T2, T3, T4>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4>(s);
	}

	// Token: 0x06004006 RID: 16390 RVA: 0x00144D88 File Offset: 0x00142F88
	public new static explicit operator PhotonSignal<T1, T2, T3, T4>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4>(i);
	}

	// Token: 0x04004BD5 RID: 19413
	private OnSignalReceived<T1, T2, T3, T4> _callbacks;

	// Token: 0x04004BD6 RID: 19414
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4>).FullName.GetStaticHash();
}
