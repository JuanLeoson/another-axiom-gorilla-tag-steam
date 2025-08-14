using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000A39 RID: 2617
[Serializable]
public class PhotonSignal<T1, T2, T3, T4, T5> : PhotonSignal
{
	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x06004008 RID: 16392 RVA: 0x00144DAB File Offset: 0x00142FAB
	public override int argCount
	{
		get
		{
			return 5;
		}
	}

	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06004009 RID: 16393 RVA: 0x00144DAE File Offset: 0x00142FAE
	// (remove) Token: 0x0600400A RID: 16394 RVA: 0x00144DE2 File Offset: 0x00142FE2
	public new event OnSignalReceived<T1, T2, T3, T4, T5> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x0600400B RID: 16395 RVA: 0x001447B7 File Offset: 0x001429B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x0600400C RID: 16396 RVA: 0x001447C0 File Offset: 0x001429C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x0600400D RID: 16397 RVA: 0x00144DFF File Offset: 0x00142FFF
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x0600400E RID: 16398 RVA: 0x00144E0E File Offset: 0x0014300E
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x0600400F RID: 16399 RVA: 0x00144E24 File Offset: 0x00143024
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
		array[6] = arg5;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004010 RID: 16400 RVA: 0x00144EE8 File Offset: 0x001430E8
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		T4 arg4;
		T5 arg5;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3, out arg4, out arg5))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4, T5>(this._callbacks, arg, arg2, arg3, arg4, arg5, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4, T5>(this._callbacks, arg, arg2, arg3, arg4, arg5, info);
	}

	// Token: 0x06004011 RID: 16401 RVA: 0x00144F36 File Offset: 0x00143136
	public new static implicit operator PhotonSignal<T1, T2, T3, T4, T5>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(s);
	}

	// Token: 0x06004012 RID: 16402 RVA: 0x00144F3E File Offset: 0x0014313E
	public new static explicit operator PhotonSignal<T1, T2, T3, T4, T5>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(i);
	}

	// Token: 0x04004BD7 RID: 19415
	private OnSignalReceived<T1, T2, T3, T4, T5> _callbacks;

	// Token: 0x04004BD8 RID: 19416
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4, T5>).FullName.GetStaticHash();
}
