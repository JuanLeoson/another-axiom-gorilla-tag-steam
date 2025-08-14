using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020006A8 RID: 1704
public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPoolVerify, IPunPrefabPool, ITickSystemPre
{
	// Token: 0x170003CC RID: 972
	// (get) Token: 0x060029D1 RID: 10705 RVA: 0x000E01CA File Offset: 0x000DE3CA
	// (set) Token: 0x060029D2 RID: 10706 RVA: 0x000E01D2 File Offset: 0x000DE3D2
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x060029D3 RID: 10707 RVA: 0x000E01DB File Offset: 0x000DE3DB
	private void Awake()
	{
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x000E01F8 File Offset: 0x000DE3F8
	private void Start()
	{
		PhotonNetwork.PrefabPool = this;
		for (int i = 0; i < this.networkPrefabsData.Length; i++)
		{
			ref PrefabType ptr = ref this.networkPrefabsData[i];
			if (ptr.prefab)
			{
				if (string.IsNullOrEmpty(ptr.prefabName))
				{
					ptr.prefabName = ptr.prefab.name;
				}
				int photonViewCount = ptr.prefab.GetComponentsInChildren<PhotonView>().Length;
				ptr.photonViewCount = photonViewCount;
				this.networkPrefabs.Add(ptr.prefabName, ptr);
			}
		}
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x000E0284 File Offset: 0x000DE484
	bool IPunPrefabPoolVerify.VerifyInstantiation(Player sender, string prefabName, Vector3 position, Quaternion rotation, int[] viewIDs, out GameObject prefab)
	{
		prefab = null;
		if (viewIDs != null)
		{
			float num = 10000f;
			PrefabType prefabType;
			if (position.IsValid(num) && rotation.IsValid() && this.networkPrefabs.TryGetValue(prefabName, out prefabType) && viewIDs.Length == prefabType.photonViewCount)
			{
				int num2 = (sender != null) ? sender.ActorNumber : 0;
				int num3 = viewIDs[0] / PhotonNetwork.MAX_VIEW_IDS;
				for (int i = 0; i < viewIDs.Length; i++)
				{
					int num4 = viewIDs[i];
					if (PhotonNetwork.ViewIDExists(num4))
					{
						return false;
					}
					for (int j = 0; j < viewIDs.Length; j++)
					{
						if (j != i && viewIDs[j] == num4)
						{
							return false;
						}
					}
					int num5 = num4 / PhotonNetwork.MAX_VIEW_IDS;
					if (num5 != num3)
					{
						return false;
					}
					if (num5 == 0)
					{
						if (!prefabType.roomObject)
						{
							return false;
						}
					}
					else if (num5 != num2)
					{
						return false;
					}
				}
				prefab = prefabType.prefab;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000E0364 File Offset: 0x000DE564
	GameObject IPunPrefabPoolVerify.Instantiate(GameObject prefabInstance, Vector3 position, Quaternion rotation)
	{
		bool activeSelf = prefabInstance.activeSelf;
		if (activeSelf)
		{
			prefabInstance.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(prefabInstance, position, rotation);
		this.netInstantiedObjects.Add(gameObject);
		if (activeSelf)
		{
			prefabInstance.SetActive(true);
		}
		return gameObject;
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x000E03A4 File Offset: 0x000DE5A4
	GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		PrefabType prefabType;
		if (!this.networkPrefabs.TryGetValue(prefabId, out prefabType))
		{
			return null;
		}
		return ((IPunPrefabPoolVerify)this).Instantiate(prefabType.prefab, position, rotation);
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x000E03D4 File Offset: 0x000DE5D4
	void IPunPrefabPool.Destroy(GameObject netObj)
	{
		if (netObj.IsNull())
		{
			return;
		}
		if (this.netInstantiedObjects.Remove(netObj))
		{
			PhotonViewCache photonViewCache;
			if (this.m_invalidCreatePool.Count < 200 && netObj.TryGetComponent<PhotonViewCache>(out photonViewCache) && !photonViewCache.Initialized)
			{
				if (this.m_m_invalidCreatePoolLookup.Add(netObj))
				{
					this.m_invalidCreatePool.Add(netObj);
				}
				return;
			}
			Object.Destroy(netObj);
			return;
		}
		else
		{
			PhotonView photonView;
			if (!netObj.TryGetComponent<PhotonView>(out photonView) || photonView.isRuntimeInstantiated)
			{
				Object.Destroy(netObj);
				return;
			}
			if (!this.objectsQueued.Contains(netObj))
			{
				this.objectsWaiting.Enqueue(netObj);
				this.objectsQueued.Add(netObj);
			}
			if (!this.waiting)
			{
				this.waiting = true;
				TickSystem<object>.AddPreTickCallback(this);
			}
			return;
		}
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x000E0494 File Offset: 0x000DE694
	void ITickSystemPre.PreTick()
	{
		if (this.waiting)
		{
			this.waiting = false;
			return;
		}
		Queue<GameObject> queue = this.queueBeingProcssed;
		Queue<GameObject> queue2 = this.objectsWaiting;
		this.objectsWaiting = queue;
		this.queueBeingProcssed = queue2;
		while (this.queueBeingProcssed.Count > 0)
		{
			GameObject gameObject = this.queueBeingProcssed.Dequeue();
			this.objectsQueued.Remove(gameObject);
			if (!gameObject.IsNull())
			{
				gameObject.SetActive(true);
				gameObject.GetComponents<PhotonView>(this.tempViews);
				for (int i = 0; i < this.tempViews.Count; i++)
				{
					PhotonNetwork.RegisterPhotonView(this.tempViews[i]);
				}
			}
		}
		if (this.objectsQueued.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			return;
		}
		this.waiting = true;
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x000E0558 File Offset: 0x000DE758
	private void OnLeftRoom()
	{
		foreach (GameObject gameObject in this.m_invalidCreatePool)
		{
			if (!gameObject.IsNull())
			{
				Object.Destroy(gameObject);
			}
		}
		this.m_invalidCreatePool.Clear();
		this.m_m_invalidCreatePoolLookup.Clear();
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x000E05C8 File Offset: 0x000DE7C8
	private void CheckVOIPSettings(RemoteVoiceLink voiceLink)
	{
		try
		{
			NetPlayer netPlayer = null;
			if (voiceLink.Info.UserData != null)
			{
				int num;
				if (int.TryParse(voiceLink.Info.UserData.ToString(), out num))
				{
					netPlayer = NetworkSystem.Instance.GetPlayer(num / PhotonNetwork.MAX_VIEW_IDS);
				}
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(voiceLink.PlayerId);
			}
			if (netPlayer != null)
			{
				RigContainer rigContainer;
				if ((voiceLink.Info.Bitrate > 20000 || voiceLink.Info.SamplingRate > 16000) && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
				{
					rigContainer.ForceMute = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	// Token: 0x040035AA RID: 13738
	[SerializeField]
	private PrefabType[] networkPrefabsData;

	// Token: 0x040035AB RID: 13739
	public Dictionary<string, PrefabType> networkPrefabs = new Dictionary<string, PrefabType>();

	// Token: 0x040035AC RID: 13740
	private Queue<GameObject> objectsWaiting = new Queue<GameObject>(20);

	// Token: 0x040035AD RID: 13741
	private Queue<GameObject> queueBeingProcssed = new Queue<GameObject>(20);

	// Token: 0x040035AE RID: 13742
	private HashSet<GameObject> objectsQueued = new HashSet<GameObject>();

	// Token: 0x040035AF RID: 13743
	private HashSet<GameObject> netInstantiedObjects = new HashSet<GameObject>();

	// Token: 0x040035B0 RID: 13744
	private List<PhotonView> tempViews = new List<PhotonView>(5);

	// Token: 0x040035B1 RID: 13745
	private List<GameObject> m_invalidCreatePool = new List<GameObject>(100);

	// Token: 0x040035B2 RID: 13746
	private HashSet<GameObject> m_m_invalidCreatePoolLookup = new HashSet<GameObject>(100);

	// Token: 0x040035B3 RID: 13747
	private bool waiting;
}
