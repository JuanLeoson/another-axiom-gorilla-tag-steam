using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000A23 RID: 2595
[Serializable]
public class PhotonEvent : IOnEventCallback, IEquatable<PhotonEvent>
{
	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x06003F4E RID: 16206 RVA: 0x00143791 File Offset: 0x00141991
	// (set) Token: 0x06003F4F RID: 16207 RVA: 0x00143799 File Offset: 0x00141999
	public bool reliable
	{
		get
		{
			return this._reliable;
		}
		set
		{
			this._reliable = value;
		}
	}

	// Token: 0x17000601 RID: 1537
	// (get) Token: 0x06003F50 RID: 16208 RVA: 0x001437A2 File Offset: 0x001419A2
	// (set) Token: 0x06003F51 RID: 16209 RVA: 0x001437AA File Offset: 0x001419AA
	public bool failSilent
	{
		get
		{
			return this._failSilent;
		}
		set
		{
			this._failSilent = value;
		}
	}

	// Token: 0x06003F52 RID: 16210 RVA: 0x001437B3 File Offset: 0x001419B3
	private PhotonEvent()
	{
	}

	// Token: 0x06003F53 RID: 16211 RVA: 0x001437C2 File Offset: 0x001419C2
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x06003F54 RID: 16212 RVA: 0x001437FD File Offset: 0x001419FD
	public PhotonEvent(string eventId) : this(StaticHash.Compute(eventId))
	{
	}

	// Token: 0x06003F55 RID: 16213 RVA: 0x0014380B File Offset: 0x00141A0B
	public PhotonEvent(int eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06003F56 RID: 16214 RVA: 0x0014381B File Offset: 0x00141A1B
	public PhotonEvent(string eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06003F57 RID: 16215 RVA: 0x0014382C File Offset: 0x00141A2C
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x06003F58 RID: 16216 RVA: 0x00143858 File Offset: 0x00141A58
	public void AddCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (this._delegate != null)
		{
			foreach (Delegate @delegate in this._delegate.GetInvocationList())
			{
				if (@delegate != null && @delegate.Equals(callback))
				{
					return;
				}
			}
		}
		this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Combine(this._delegate, callback);
	}

	// Token: 0x06003F59 RID: 16217 RVA: 0x001438C6 File Offset: 0x00141AC6
	public void RemoveCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Remove(this._delegate, callback);
		}
	}

	// Token: 0x06003F5A RID: 16218 RVA: 0x001438EB File Offset: 0x00141AEB
	public void Enable()
	{
		if (this._disposed)
		{
			return;
		}
		if (this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
		this._enabled = true;
	}

	// Token: 0x06003F5B RID: 16219 RVA: 0x00143913 File Offset: 0x00141B13
	public void Disable()
	{
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
		this._enabled = false;
	}

	// Token: 0x06003F5C RID: 16220 RVA: 0x0014393B File Offset: 0x00141B3B
	public void Dispose()
	{
		this._delegate = null;
		if (this._enabled)
		{
			this._enabled = false;
			if (Application.isPlaying)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
		}
		this._eventId = -1;
		this._disposed = true;
	}

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06003F5D RID: 16221 RVA: 0x00143970 File Offset: 0x00141B70
	// (remove) Token: 0x06003F5E RID: 16222 RVA: 0x001439A4 File Offset: 0x00141BA4
	public static event Action<PhotonEvent, EventData, Exception> OnError;

	// Token: 0x06003F5F RID: 16223 RVA: 0x001439D8 File Offset: 0x00141BD8
	void IOnEventCallback.OnEvent(EventData ev)
	{
		if (ev.Code != 176)
		{
			return;
		}
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		try
		{
			object[] array = (object[])ev.CustomData;
			if (array.Length != 0)
			{
				object obj = array[0];
				if (obj is int)
				{
					int num = (int)obj;
					int eventId = this._eventId;
					if (num != -1)
					{
						if (eventId != -1)
						{
							object[] args = (array.Length == 1) ? Array.Empty<object>() : array.Skip(1).ToArray<object>();
							PhotonMessageInfoWrapped info = new PhotonMessageInfoWrapped(ev.Sender, PhotonNetwork.ServerTimestamp);
							this.InvokeDelegate(num, eventId, args, info);
						}
					}
				}
			}
		}
		catch (Exception arg)
		{
			Action<PhotonEvent, EventData, Exception> onError = PhotonEvent.OnError;
			if (onError != null)
			{
				onError(this, ev, arg);
			}
		}
	}

	// Token: 0x06003F60 RID: 16224 RVA: 0x00143AA8 File Offset: 0x00141CA8
	private void InvokeDelegate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		Action<int, int, object[], PhotonMessageInfoWrapped> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, target, args, info);
	}

	// Token: 0x06003F61 RID: 16225 RVA: 0x00143ABF File Offset: 0x00141CBF
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x06003F62 RID: 16226 RVA: 0x00143AC9 File Offset: 0x00141CC9
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x06003F63 RID: 16227 RVA: 0x00143AD3 File Offset: 0x00141CD3
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x06003F64 RID: 16228 RVA: 0x00143AE0 File Offset: 0x00141CE0
	private void Raise(PhotonEvent.RaiseMode mode, params object[] args)
	{
		if (this._disposed)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		SendOptions sendOptions = this._reliable ? PhotonEvent.gSendReliable : PhotonEvent.gSendUnreliable;
		switch (mode)
		{
		case PhotonEvent.RaiseMode.Local:
			this.InvokeDelegate(this._eventId, this._eventId, args, new PhotonMessageInfoWrapped(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.ServerTimestamp));
			return;
		case PhotonEvent.RaiseMode.RemoteOthers:
		{
			object[] eventContent = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, eventContent, PhotonEvent.gReceiversOthers, sendOptions);
			return;
		}
		case PhotonEvent.RaiseMode.RemoteAll:
		{
			object[] eventContent2 = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, eventContent2, PhotonEvent.gReceiversAll, sendOptions);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06003F65 RID: 16229 RVA: 0x00143BAC File Offset: 0x00141DAC
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x06003F66 RID: 16230 RVA: 0x00143C0C File Offset: 0x00141E0C
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x06003F67 RID: 16231 RVA: 0x00143C2C File Offset: 0x00141E2C
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int i = StaticHash.Compute(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Compute(staticHash, i);
	}

	// Token: 0x06003F68 RID: 16232 RVA: 0x00143C68 File Offset: 0x00141E68
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x06003F69 RID: 16233 RVA: 0x00143C86 File Offset: 0x00141E86
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x06003F6A RID: 16234 RVA: 0x00143CA4 File Offset: 0x00141EA4
	static PhotonEvent()
	{
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x06003F6B RID: 16235 RVA: 0x00143CFD File Offset: 0x00141EFD
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06003F6C RID: 16236 RVA: 0x00143D0B File Offset: 0x00141F0B
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x04004BA9 RID: 19369
	private const int INVALID_ID = -1;

	// Token: 0x04004BAA RID: 19370
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x04004BAB RID: 19371
	[SerializeField]
	private bool _enabled;

	// Token: 0x04004BAC RID: 19372
	[SerializeField]
	private bool _reliable;

	// Token: 0x04004BAD RID: 19373
	[SerializeField]
	private bool _failSilent;

	// Token: 0x04004BAE RID: 19374
	[NonSerialized]
	private bool _disposed;

	// Token: 0x04004BAF RID: 19375
	private Action<int, int, object[], PhotonMessageInfoWrapped> _delegate;

	// Token: 0x04004BB1 RID: 19377
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x04004BB2 RID: 19378
	private static readonly RaiseEventOptions gReceiversAll = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.All
	};

	// Token: 0x04004BB3 RID: 19379
	private static readonly RaiseEventOptions gReceiversOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	// Token: 0x04004BB4 RID: 19380
	private static readonly SendOptions gSendReliable;

	// Token: 0x04004BB5 RID: 19381
	private static readonly SendOptions gSendUnreliable = SendOptions.SendUnreliable;

	// Token: 0x02000A24 RID: 2596
	public enum RaiseMode
	{
		// Token: 0x04004BB7 RID: 19383
		Local,
		// Token: 0x04004BB8 RID: 19384
		RemoteOthers,
		// Token: 0x04004BB9 RID: 19385
		RemoteAll
	}
}
