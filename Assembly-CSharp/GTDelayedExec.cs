using System;
using UnityEngine;

// Token: 0x02000A5B RID: 2651
public class GTDelayedExec : ITickSystemTick
{
	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x060040A2 RID: 16546 RVA: 0x001472A2 File Offset: 0x001454A2
	// (set) Token: 0x060040A3 RID: 16547 RVA: 0x001472A9 File Offset: 0x001454A9
	public static GTDelayedExec instance { get; private set; }

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x060040A4 RID: 16548 RVA: 0x001472B1 File Offset: 0x001454B1
	// (set) Token: 0x060040A5 RID: 16549 RVA: 0x001472B9 File Offset: 0x001454B9
	public int listenerCount { get; private set; }

	// Token: 0x060040A6 RID: 16550 RVA: 0x001472C2 File Offset: 0x001454C2
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void InitializeAfterAssemblies()
	{
		GTDelayedExec.instance = new GTDelayedExec();
		TickSystem<object>.AddTickCallback(GTDelayedExec.instance);
	}

	// Token: 0x060040A7 RID: 16551 RVA: 0x001472D8 File Offset: 0x001454D8
	internal static void Add(IDelayedExecListener listener, float delay, int contextId)
	{
		if (GTDelayedExec.instance.listenerCount >= 1024)
		{
			Debug.LogError("Maximum number of delayed listeners reached.");
			return;
		}
		GTDelayedExec._listenerDelays[GTDelayedExec.instance.listenerCount] = Time.unscaledTime + delay;
		GTDelayedExec._listeners[GTDelayedExec.instance.listenerCount] = new GTDelayedExec.Listener(listener, contextId);
		GTDelayedExec instance = GTDelayedExec.instance;
		int listenerCount = instance.listenerCount;
		instance.listenerCount = listenerCount + 1;
	}

	// Token: 0x17000620 RID: 1568
	// (get) Token: 0x060040A8 RID: 16552 RVA: 0x00147347 File Offset: 0x00145547
	// (set) Token: 0x060040A9 RID: 16553 RVA: 0x0014734F File Offset: 0x0014554F
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060040AA RID: 16554 RVA: 0x00147358 File Offset: 0x00145558
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < this.listenerCount; i++)
		{
			if (Time.unscaledTime >= GTDelayedExec._listenerDelays[i])
			{
				try
				{
					GTDelayedExec._listeners[i].listener.OnDelayedAction(GTDelayedExec._listeners[i].contextId);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				int listenerCount = this.listenerCount;
				this.listenerCount = listenerCount - 1;
				GTDelayedExec._listenerDelays[i] = GTDelayedExec._listenerDelays[this.listenerCount];
				GTDelayedExec._listeners[i] = GTDelayedExec._listeners[this.listenerCount];
				i--;
			}
		}
	}

	// Token: 0x04004C48 RID: 19528
	public const int kMaxListeners = 1024;

	// Token: 0x04004C4A RID: 19530
	private static readonly float[] _listenerDelays = new float[1024];

	// Token: 0x04004C4B RID: 19531
	private static readonly GTDelayedExec.Listener[] _listeners = new GTDelayedExec.Listener[1024];

	// Token: 0x02000A5C RID: 2652
	private struct Listener
	{
		// Token: 0x060040AD RID: 16557 RVA: 0x0014742C File Offset: 0x0014562C
		public Listener(IDelayedExecListener listener, int contextId)
		{
			this.listener = listener;
			this.contextId = contextId;
		}

		// Token: 0x04004C4D RID: 19533
		public readonly IDelayedExecListener listener;

		// Token: 0x04004C4E RID: 19534
		public readonly int contextId;
	}
}
