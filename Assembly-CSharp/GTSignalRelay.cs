using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200073F RID: 1855
public class GTSignalRelay : MonoBehaviourStatic<GTSignalRelay>, IOnEventCallback
{
	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x06002E66 RID: 11878 RVA: 0x000F5696 File Offset: 0x000F3896
	public static IReadOnlyList<GTSignalListener> ActiveListeners
	{
		get
		{
			return GTSignalRelay.gActiveListeners;
		}
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x000F569D File Offset: 0x000F389D
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x000F56AC File Offset: 0x000F38AC
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000F56BC File Offset: 0x000F38BC
	public static void Register(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		int num = listener.signal;
		if (num == 0)
		{
			return;
		}
		if (!GTSignalRelay.gListenerSet.Add(listener))
		{
			return;
		}
		GTSignalRelay.gActiveListeners.Add(listener);
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, out list))
		{
			list = new List<GTSignalListener>(64);
			GTSignalRelay.gSignalIdToListeners.Add(num, list);
		}
		list.Add(listener);
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000F5728 File Offset: 0x000F3928
	public static void Unregister(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		GTSignalRelay.gListenerSet.Remove(listener);
		GTSignalRelay.gActiveListeners.Remove(listener);
		List<GTSignalListener> list;
		if (GTSignalRelay.gSignalIdToListeners.TryGetValue(listener.signal, out list))
		{
			list.Remove(listener);
		}
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000F5778 File Offset: 0x000F3978
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		Object.DontDestroyOnLoad(new GameObject("GTSignalRelay").AddComponent<GTSignalRelay>());
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000F5790 File Offset: 0x000F3990
	void IOnEventCallback.OnEvent(EventData eventData)
	{
		if (eventData.Code != 186)
		{
			return;
		}
		object[] array = (object[])eventData.CustomData;
		int key = (int)array[0];
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(key, out list))
		{
			return;
		}
		int sender = eventData.Sender;
		for (int i = 0; i < list.Count; i++)
		{
			try
			{
				GTSignalListener gtsignalListener = list[i];
				if (!gtsignalListener.deafen)
				{
					if (gtsignalListener.IsReady())
					{
						if (!gtsignalListener.ignoreSelf || sender != gtsignalListener.rigActorID)
						{
							if (!gtsignalListener.listenToSelfOnly || sender == gtsignalListener.rigActorID)
							{
								gtsignalListener.HandleSignalReceived(sender, array);
								if (gtsignalListener.callUnityEvent)
								{
									UnityEvent onSignalReceived = gtsignalListener.onSignalReceived;
									if (onSignalReceived != null)
									{
										onSignalReceived.Invoke();
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x04003A2F RID: 14895
	private static List<GTSignalListener> gActiveListeners = new List<GTSignalListener>(128);

	// Token: 0x04003A30 RID: 14896
	private static HashSet<GTSignalListener> gListenerSet = new HashSet<GTSignalListener>(128);

	// Token: 0x04003A31 RID: 14897
	private static Dictionary<int, List<GTSignalListener>> gSignalIdToListeners = new Dictionary<int, List<GTSignalListener>>(128);
}
