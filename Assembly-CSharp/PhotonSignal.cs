using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000A33 RID: 2611
[Serializable]
public class PhotonSignal
{
	// Token: 0x06003FA5 RID: 16293 RVA: 0x00143D88 File Offset: 0x00141F88
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, info);
		}
	}

	// Token: 0x06003FA6 RID: 16294 RVA: 0x00143DB8 File Offset: 0x00141FB8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, info);
		}
	}

	// Token: 0x06003FA7 RID: 16295 RVA: 0x00143DE4 File Offset: 0x00141FE4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, info);
		}
	}

	// Token: 0x06003FA8 RID: 16296 RVA: 0x00143E10 File Offset: 0x00142010
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, info);
		}
	}

	// Token: 0x06003FA9 RID: 16297 RVA: 0x00143E38 File Offset: 0x00142038
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, info);
		}
	}

	// Token: 0x06003FAA RID: 16298 RVA: 0x00143E60 File Offset: 0x00142060
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, info);
		}
	}

	// Token: 0x06003FAB RID: 16299 RVA: 0x00143E83 File Offset: 0x00142083
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6>(OnSignalReceived<T1, T2, T3, T4, T5, T6> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, info);
		}
	}

	// Token: 0x06003FAC RID: 16300 RVA: 0x00143E99 File Offset: 0x00142099
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4, T5>(OnSignalReceived<T1, T2, T3, T4, T5> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, info);
		}
	}

	// Token: 0x06003FAD RID: 16301 RVA: 0x00143EAD File Offset: 0x001420AD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3, T4>(OnSignalReceived<T1, T2, T3, T4> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, info);
		}
	}

	// Token: 0x06003FAE RID: 16302 RVA: 0x00143EBF File Offset: 0x001420BF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2, T3>(OnSignalReceived<T1, T2, T3> _event, T1 arg1, T2 arg2, T3 arg3, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, info);
		}
	}

	// Token: 0x06003FAF RID: 16303 RVA: 0x00143ECF File Offset: 0x001420CF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1, T2>(OnSignalReceived<T1, T2> _event, T1 arg1, T2 arg2, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, info);
		}
	}

	// Token: 0x06003FB0 RID: 16304 RVA: 0x00143EDD File Offset: 0x001420DD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke<T1>(OnSignalReceived<T1> _event, T1 arg1, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, info);
		}
	}

	// Token: 0x06003FB1 RID: 16305 RVA: 0x00143EEA File Offset: 0x001420EA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _Invoke(OnSignalReceived _event, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(info);
		}
	}

	// Token: 0x06003FB2 RID: 16306 RVA: 0x00143EF8 File Offset: 0x001420F8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB3 RID: 16307 RVA: 0x00143F58 File Offset: 0x00142158
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB4 RID: 16308 RVA: 0x00143FB8 File Offset: 0x001421B8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB5 RID: 16309 RVA: 0x00144014 File Offset: 0x00142214
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB6 RID: 16310 RVA: 0x00144070 File Offset: 0x00142270
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB7 RID: 16311 RVA: 0x001440C8 File Offset: 0x001422C8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB8 RID: 16312 RVA: 0x00144120 File Offset: 0x00142320
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6>(OnSignalReceived<T1, T2, T3, T4, T5, T6> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FB9 RID: 16313 RVA: 0x00144174 File Offset: 0x00142374
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5>(OnSignalReceived<T1, T2, T3, T4, T5> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FBA RID: 16314 RVA: 0x001441C8 File Offset: 0x001423C8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3, T4>(OnSignalReceived<T1, T2, T3, T4> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FBB RID: 16315 RVA: 0x00144218 File Offset: 0x00142418
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2, T3>(OnSignalReceived<T1, T2, T3> _event, T1 arg1, T2 arg2, T3 arg3, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FBC RID: 16316 RVA: 0x00144268 File Offset: 0x00142468
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1, T2>(OnSignalReceived<T1, T2> _event, T1 arg1, T2 arg2, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FBD RID: 16317 RVA: 0x001442B4 File Offset: 0x001424B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke<T1>(OnSignalReceived<T1> _event, T1 arg1, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06003FBE RID: 16318 RVA: 0x00144300 File Offset: 0x00142500
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void _SafeInvoke(OnSignalReceived _event, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x17000603 RID: 1539
	// (get) Token: 0x06003FBF RID: 16319 RVA: 0x0014434C File Offset: 0x0014254C
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
	}

	// Token: 0x17000604 RID: 1540
	// (get) Token: 0x06003FC0 RID: 16320 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int argCount
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x06003FC1 RID: 16321 RVA: 0x00144354 File Offset: 0x00142554
	// (remove) Token: 0x06003FC2 RID: 16322 RVA: 0x00144388 File Offset: 0x00142588
	public event OnSignalReceived OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003FC3 RID: 16323 RVA: 0x001443A5 File Offset: 0x001425A5
	protected PhotonSignal()
	{
		this._refID = PhotonSignal.RefID.Register(this);
	}

	// Token: 0x06003FC4 RID: 16324 RVA: 0x001443DD File Offset: 0x001425DD
	public PhotonSignal(string signalID) : this()
	{
		signalID = ((signalID != null) ? signalID.Trim() : null);
		if (string.IsNullOrWhiteSpace(signalID))
		{
			throw new ArgumentNullException("signalID");
		}
		this._signalID = XXHash32.Compute(signalID, 0U);
	}

	// Token: 0x06003FC5 RID: 16325 RVA: 0x00144413 File Offset: 0x00142613
	public PhotonSignal(int signalID) : this()
	{
		this._signalID = signalID;
	}

	// Token: 0x06003FC6 RID: 16326 RVA: 0x00144422 File Offset: 0x00142622
	public void Raise()
	{
		this.Raise(this._receivers);
	}

	// Token: 0x06003FC7 RID: 16327 RVA: 0x00144430 File Offset: 0x00142630
	public void Raise(ReceiverGroup receivers)
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
		object[] array = PhotonUtils.FetchScratchArray(2);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06003FC8 RID: 16328 RVA: 0x001444BD File Offset: 0x001426BD
	public void Enable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += this._EventHandle;
		this._enabled = true;
	}

	// Token: 0x06003FC9 RID: 16329 RVA: 0x001444DC File Offset: 0x001426DC
	public void Disable()
	{
		this._enabled = false;
		PhotonNetwork.NetworkingClient.EventReceived -= this._EventHandle;
	}

	// Token: 0x06003FCA RID: 16330 RVA: 0x001444FC File Offset: 0x001426FC
	private void _EventHandle(EventData eventData)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		if (eventData.Code != 177)
		{
			return;
		}
		int sender = eventData.Sender;
		object[] array = eventData.CustomData as object[];
		if (array == null)
		{
			return;
		}
		if (array.Length < 2 + this.argCount)
		{
			return;
		}
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num == 0 || num != this._signalID)
		{
			return;
		}
		obj = array[1];
		if (!(obj is int))
		{
			return;
		}
		int timestamp = (int)obj;
		if (!this._limiter.CheckCallTime(Time.time))
		{
			return;
		}
		NetPlayer netPlayer = PhotonUtils.GetNetPlayer(sender);
		PhotonSignalInfo info = new PhotonSignalInfo(netPlayer, timestamp);
		this._Relay(array, info);
	}

	// Token: 0x06003FCB RID: 16331 RVA: 0x001445BB File Offset: 0x001427BB
	protected virtual void _Relay(object[] args, PhotonSignalInfo info)
	{
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke(this._callbacks, info);
			return;
		}
		PhotonSignal._SafeInvoke(this._callbacks, info);
	}

	// Token: 0x06003FCC RID: 16332 RVA: 0x001445DE File Offset: 0x001427DE
	public virtual void ClearListeners()
	{
		this._callbacks = null;
	}

	// Token: 0x06003FCD RID: 16333 RVA: 0x001445E7 File Offset: 0x001427E7
	public virtual void Reset()
	{
		this.ClearListeners();
		this.Disable();
	}

	// Token: 0x06003FCE RID: 16334 RVA: 0x001445F5 File Offset: 0x001427F5
	public virtual void Dispose()
	{
		this._signalID = 0;
		this.Reset();
	}

	// Token: 0x06003FCF RID: 16335 RVA: 0x00144604 File Offset: 0x00142804
	~PhotonSignal()
	{
		this.Dispose();
	}

	// Token: 0x06003FD0 RID: 16336 RVA: 0x00144630 File Offset: 0x00142830
	public static implicit operator PhotonSignal(string s)
	{
		return new PhotonSignal(s);
	}

	// Token: 0x06003FD1 RID: 16337 RVA: 0x00144638 File Offset: 0x00142838
	public static explicit operator PhotonSignal(int i)
	{
		return new PhotonSignal(i);
	}

	// Token: 0x06003FD2 RID: 16338 RVA: 0x00144640 File Offset: 0x00142840
	static PhotonSignal()
	{
		Dictionary<ReceiverGroup, RaiseEventOptions> dictionary = new Dictionary<ReceiverGroup, RaiseEventOptions>();
		dictionary[ReceiverGroup.Others] = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others
		};
		dictionary[ReceiverGroup.All] = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		};
		dictionary[ReceiverGroup.MasterClient] = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.MasterClient
		};
		PhotonSignal.gGroupToOptions = dictionary;
		PhotonSignal.gSendReliable = SendOptions.SendReliable;
		PhotonSignal.gSendUnreliable = SendOptions.SendUnreliable;
		PhotonSignal.gSendReliable.Encrypt = true;
		PhotonSignal.gSendUnreliable.Encrypt = true;
	}

	// Token: 0x04004BBC RID: 19388
	protected int _signalID;

	// Token: 0x04004BBD RID: 19389
	protected bool _enabled;

	// Token: 0x04004BBE RID: 19390
	[SerializeField]
	protected ReceiverGroup _receivers = ReceiverGroup.All;

	// Token: 0x04004BBF RID: 19391
	[SerializeField]
	protected CallLimiter _limiter = new CallLimiter(1, 0.1f, 0.5f);

	// Token: 0x04004BC0 RID: 19392
	[FormerlySerializedAs("mute")]
	[SerializeField]
	protected bool _mute;

	// Token: 0x04004BC1 RID: 19393
	[SerializeField]
	protected bool _safeInvoke = true;

	// Token: 0x04004BC2 RID: 19394
	[SerializeField]
	protected bool _localOnly;

	// Token: 0x04004BC3 RID: 19395
	[NonSerialized]
	private int _refID;

	// Token: 0x04004BC4 RID: 19396
	private OnSignalReceived _callbacks;

	// Token: 0x04004BC5 RID: 19397
	protected static readonly Dictionary<ReceiverGroup, RaiseEventOptions> gGroupToOptions;

	// Token: 0x04004BC6 RID: 19398
	protected static readonly SendOptions gSendReliable;

	// Token: 0x04004BC7 RID: 19399
	protected static readonly SendOptions gSendUnreliable;

	// Token: 0x04004BC8 RID: 19400
	public const byte EVENT_CODE = 177;

	// Token: 0x04004BC9 RID: 19401
	public const int NULL_SIGNAL = 0;

	// Token: 0x04004BCA RID: 19402
	protected const int HEADER_SIZE = 2;

	// Token: 0x02000A34 RID: 2612
	private class RefID
	{
		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06003FD3 RID: 16339 RVA: 0x001446BA File Offset: 0x001428BA
		public static int Count
		{
			get
			{
				return PhotonSignal.RefID.gRefCount;
			}
		}

		// Token: 0x06003FD4 RID: 16340 RVA: 0x001446C1 File Offset: 0x001428C1
		public RefID()
		{
			this.intValue = StaticHash.ComputeTriple32(PhotonSignal.RefID.gNextID++);
			PhotonSignal.RefID.gRefCount++;
		}

		// Token: 0x06003FD5 RID: 16341 RVA: 0x001446F0 File Offset: 0x001428F0
		~RefID()
		{
			PhotonSignal.RefID.gRefCount--;
		}

		// Token: 0x06003FD6 RID: 16342 RVA: 0x00144724 File Offset: 0x00142924
		public static int Register(PhotonSignal ps)
		{
			if (ps == null)
			{
				return 0;
			}
			PhotonSignal.RefID refID = new PhotonSignal.RefID();
			PhotonSignal.RefID.gRefTable.Add(ps, refID);
			return refID.intValue;
		}

		// Token: 0x04004BCB RID: 19403
		public int intValue;

		// Token: 0x04004BCC RID: 19404
		private static int gNextID = 1;

		// Token: 0x04004BCD RID: 19405
		private static int gRefCount = 0;

		// Token: 0x04004BCE RID: 19406
		private static readonly ConditionalWeakTable<PhotonSignal, PhotonSignal.RefID> gRefTable = new ConditionalWeakTable<PhotonSignal, PhotonSignal.RefID>();
	}
}
