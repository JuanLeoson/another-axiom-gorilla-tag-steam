using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200076E RID: 1902
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06002F95 RID: 12181 RVA: 0x000FB202 File Offset: 0x000F9402
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x000FB20A File Offset: 0x000F940A
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x000FB214 File Offset: 0x000F9414
	protected virtual void Tick()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this._instances.Count; i++)
		{
			T t = this._instances[i];
			if (t)
			{
				this.OnTick(deltaTime, t);
			}
		}
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x000FB25F File Offset: 0x000F945F
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x000FB268 File Offset: 0x000F9468
	private bool RegisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Register] Instance is null.", null);
			return false;
		}
		if (this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnRegister(instance);
		return true;
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x000FB2CC File Offset: 0x000F94CC
	private bool UnregisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Unregister] Instance is null.", null);
			return false;
		}
		if (!this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Remove(instance);
		this.OnUnregister(instance);
		return true;
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x000023F5 File Offset: 0x000005F5
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x000FB32E File Offset: 0x000F952E
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x000FB32E File Offset: 0x000F952E
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<!0>)this._instances).GetEnumerator();
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06002FA0 RID: 12192 RVA: 0x000FB33B File Offset: 0x000F953B
	int IReadOnlyCollection<!0>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x1700046B RID: 1131
	T IReadOnlyList<!0>.this[int index]
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06002FA2 RID: 12194 RVA: 0x000FB356 File Offset: 0x000F9556
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x000FB364 File Offset: 0x000F9564
	protected static void SetSingleton(GTSystem<T> system)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (GTSystem<T>.gSingleton != null && GTSystem<T>.gSingleton != system)
		{
			Object.Destroy(system);
			GTDev.LogWarning<string>("Singleton of type " + GTSystem<T>.gSingleton.GetType().Name + " already exists.", null);
			return;
		}
		GTSystem<T>.gSingleton = system;
		if (!GTSystem<T>.gInitializing)
		{
			return;
		}
		GTSystem<T>.gSingleton._instances.Clear();
		T[] collection = (from x in GTSystem<T>.gQueueRegister
		where x != null
		select x).ToArray<T>();
		GTSystem<T>.gSingleton._instances.AddRange(collection);
		GTSystem<T>.gQueueRegister.Clear();
		PhotonView component = GTSystem<T>.gSingleton.GetComponent<PhotonView>();
		if (component != null)
		{
			GTSystem<T>.gSingleton._photonView = component;
			GTSystem<T>.gSingleton._networked = true;
		}
		GTSystem<T>.gInitializing = false;
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x000FB454 File Offset: 0x000F9654
	public static void Register(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		GTSystem<T>.gSingleton.RegisterInstance(instance);
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x000FB4C0 File Offset: 0x000F96C0
	public static void Unregister(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		GTSystem<T>.gSingleton.UnregisterInstance(instance);
	}

	// Token: 0x04003B9D RID: 15261
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04003B9E RID: 15262
	[SerializeField]
	private bool _networked;

	// Token: 0x04003B9F RID: 15263
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04003BA0 RID: 15264
	private static GTSystem<T> gSingleton;

	// Token: 0x04003BA1 RID: 15265
	private static bool gInitializing = false;

	// Token: 0x04003BA2 RID: 15266
	private static bool gAppQuitting = false;

	// Token: 0x04003BA3 RID: 15267
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
