using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

// Token: 0x02000A73 RID: 2675
internal abstract class TickSystem<T> : MonoBehaviour
{
	// Token: 0x06004138 RID: 16696 RVA: 0x0014A4EA File Offset: 0x001486EA
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06004139 RID: 16697 RVA: 0x0014A4FF File Offset: 0x001486FF
	private void Update()
	{
		TickSystem<T>.preTickCallbacks.TryRunCallbacks();
		TickSystem<T>.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x0600413A RID: 16698 RVA: 0x0014A515 File Offset: 0x00148715
	private void LateUpdate()
	{
		TickSystem<T>.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x0600413B RID: 16699 RVA: 0x0014A524 File Offset: 0x00148724
	static TickSystem()
	{
		TickSystem<T>.preTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPre>(100);
		TickSystem<T>.tickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperTick>(100);
		TickSystem<T>.postTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPost>(100);
	}

	// Token: 0x0600413C RID: 16700 RVA: 0x0014A597 File Offset: 0x00148797
	private static void OnEnterPlay()
	{
		TickSystem<T>.preTickCallbacks.Clear();
		TickSystem<T>.preTickWrapperTable.Clear();
		TickSystem<T>.tickCallbacks.Clear();
		TickSystem<T>.tickWrapperTable.Clear();
		TickSystem<T>.postTickCallbacks.Clear();
		TickSystem<T>.postTickWrapperTable.Clear();
	}

	// Token: 0x0600413D RID: 16701 RVA: 0x0014A5D8 File Offset: 0x001487D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		if (callback.PreTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre = TickSystem<T>.preTickWrapperPool.Take();
		tickCallbackWrapperPre.target = callback;
		TickSystem<T>.preTickWrapperTable[callback] = tickCallbackWrapperPre;
		TickSystem<T>.preTickCallbacks.Add(tickCallbackWrapperPre);
		callback.PreTickRunning = true;
	}

	// Token: 0x0600413E RID: 16702 RVA: 0x0014A620 File Offset: 0x00148820
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		if (callback.TickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick = TickSystem<T>.tickWrapperPool.Take();
		tickCallbackWrapperTick.target = callback;
		TickSystem<T>.tickWrapperTable[callback] = tickCallbackWrapperTick;
		TickSystem<T>.tickCallbacks.Add(tickCallbackWrapperTick);
		callback.TickRunning = true;
	}

	// Token: 0x0600413F RID: 16703 RVA: 0x0014A668 File Offset: 0x00148868
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		if (callback.PostTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost = TickSystem<T>.postTickWrapperPool.Take();
		tickCallbackWrapperPost.target = callback;
		TickSystem<T>.postTickWrapperTable[callback] = tickCallbackWrapperPost;
		TickSystem<T>.postTickCallbacks.Add(tickCallbackWrapperPost);
		callback.PostTickRunning = true;
	}

	// Token: 0x06004140 RID: 16704 RVA: 0x0014A6AF File Offset: 0x001488AF
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem<T>.AddPreTickCallback(callback);
		TickSystem<T>.AddTickCallback(callback);
		TickSystem<T>.AddPostTickCallback(callback);
	}

	// Token: 0x06004141 RID: 16705 RVA: 0x0014A6C4 File Offset: 0x001488C4
	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.AddPostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x06004142 RID: 16706 RVA: 0x0014A714 File Offset: 0x00148914
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem<T>.TickCallbackWrapperPre instance;
		if (!callback.PreTickRunning || !TickSystem<T>.preTickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.Remove(instance);
		callback.PreTickRunning = false;
		TickSystem<T>.preTickWrapperPool.Return(instance);
	}

	// Token: 0x06004143 RID: 16707 RVA: 0x0014A758 File Offset: 0x00148958
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem<T>.TickCallbackWrapperTick instance;
		if (!callback.TickRunning || !TickSystem<T>.tickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.tickCallbacks.Remove(instance);
		callback.TickRunning = false;
		TickSystem<T>.tickWrapperPool.Return(instance);
	}

	// Token: 0x06004144 RID: 16708 RVA: 0x0014A79C File Offset: 0x0014899C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem<T>.TickCallbackWrapperPost instance;
		if (!callback.PostTickRunning || !TickSystem<T>.postTickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.Remove(instance);
		callback.PostTickRunning = false;
		TickSystem<T>.postTickWrapperPool.Return(instance);
	}

	// Token: 0x06004145 RID: 16709 RVA: 0x0014A7DF File Offset: 0x001489DF
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem<T>.RemovePreTickCallback(callback);
		TickSystem<T>.RemoveTickCallback(callback);
		TickSystem<T>.RemovePostTickCallback(callback);
	}

	// Token: 0x06004146 RID: 16710 RVA: 0x0014A7F4 File Offset: 0x001489F4
	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.RemovePostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x04004CCC RID: 19660
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPre> preTickWrapperPool;

	// Token: 0x04004CCD RID: 19661
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPre> preTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPre>();

	// Token: 0x04004CCE RID: 19662
	private static readonly Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre> preTickWrapperTable = new Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x04004CCF RID: 19663
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperTick> tickWrapperPool;

	// Token: 0x04004CD0 RID: 19664
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperTick> tickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperTick>();

	// Token: 0x04004CD1 RID: 19665
	private static readonly Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick> tickWrapperTable = new Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x04004CD2 RID: 19666
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPost> postTickWrapperPool;

	// Token: 0x04004CD3 RID: 19667
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPost> postTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPost>();

	// Token: 0x04004CD4 RID: 19668
	private static readonly Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost> postTickWrapperTable = new Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x02000A74 RID: 2676
	private abstract class TickCallbackWrapper<U> : ObjectPoolEvents, ICallBack where U : class
	{
		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06004148 RID: 16712 RVA: 0x0014A842 File Offset: 0x00148A42
		// (set) Token: 0x06004149 RID: 16713 RVA: 0x0014A84A File Offset: 0x00148A4A
		public U target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
			}
		}

		// Token: 0x0600414A RID: 16714
		public abstract void CallBack();

		// Token: 0x0600414B RID: 16715 RVA: 0x000023F5 File Offset: 0x000005F5
		public void OnTaken()
		{
		}

		// Token: 0x0600414C RID: 16716 RVA: 0x0014A854 File Offset: 0x00148A54
		public void OnReturned()
		{
			this.target = default(U);
		}

		// Token: 0x04004CD5 RID: 19669
		protected U m_target;
	}

	// Token: 0x02000A75 RID: 2677
	private class TickCallbackWrapperPre : TickSystem<T>.TickCallbackWrapper<ITickSystemPre>
	{
		// Token: 0x0600414E RID: 16718 RVA: 0x0014A870 File Offset: 0x00148A70
		public override void CallBack()
		{
			this.m_target.PreTick();
		}
	}

	// Token: 0x02000A76 RID: 2678
	private class TickCallbackWrapperTick : TickSystem<T>.TickCallbackWrapper<ITickSystemTick>
	{
		// Token: 0x06004150 RID: 16720 RVA: 0x0014A885 File Offset: 0x00148A85
		public override void CallBack()
		{
			this.m_target.Tick();
		}
	}

	// Token: 0x02000A77 RID: 2679
	private class TickCallbackWrapperPost : TickSystem<T>.TickCallbackWrapper<ITickSystemPost>
	{
		// Token: 0x06004152 RID: 16722 RVA: 0x0014A89A File Offset: 0x00148A9A
		public override void CallBack()
		{
			this.m_target.PostTick();
		}
	}
}
