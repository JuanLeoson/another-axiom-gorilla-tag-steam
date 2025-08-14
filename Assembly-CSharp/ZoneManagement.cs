using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000267 RID: 615
public class ZoneManagement : MonoBehaviour
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000E34 RID: 3636 RVA: 0x000570FC File Offset: 0x000552FC
	// (remove) Token: 0x06000E35 RID: 3637 RVA: 0x00057130 File Offset: 0x00055330
	public static event ZoneManagement.ZoneChangeEvent OnZoneChange;

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000E36 RID: 3638 RVA: 0x00057163 File Offset: 0x00055363
	// (set) Token: 0x06000E37 RID: 3639 RVA: 0x0005716B File Offset: 0x0005536B
	public bool hasInstance { get; private set; }

	// Token: 0x06000E38 RID: 3640 RVA: 0x00057174 File Offset: 0x00055374
	private void Awake()
	{
		if (ZoneManagement.instance == null)
		{
			this.Initialize();
			return;
		}
		if (ZoneManagement.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x000571A2 File Offset: 0x000553A2
	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[]
		{
			zone
		});
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x000571B4 File Offset: 0x000553B4
	public static void SetActiveZones(GTZone[] zones)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		if (zones == null || zones.Length == 0)
		{
			return;
		}
		ZoneManagement.instance.SetZones(zones);
		Action action = ZoneManagement.instance.onZoneChanged;
		if (action != null)
		{
			action();
		}
		if (ZoneManagement.OnZoneChange != null)
		{
			ZoneManagement.OnZoneChange(ZoneManagement.instance.zones);
		}
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00057218 File Offset: 0x00055418
	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x0005724E File Offset: 0x0005544E
	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x0005725E File Offset: 0x0005545E
	public static void AddSceneToForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Add(sceneName);
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00057283 File Offset: 0x00055483
	public static void RemoveSceneFromForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Remove(sceneName);
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x000572A8 File Offset: 0x000554A8
	public static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindObjectOfType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x000572D4 File Offset: 0x000554D4
	public bool IsSceneLoaded(GTZone gtZone)
	{
		foreach (ZoneData zoneData in this.zones)
		{
			if (zoneData.zone == gtZone && this.scenesLoaded.Contains(zoneData.sceneName))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0005731C File Offset: 0x0005551C
	public bool IsZoneActive(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0005733C File Offset: 0x0005553C
	public HashSet<string> GetAllLoadedScenes()
	{
		return this.scenesLoaded;
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x00057344 File Offset: 0x00055544
	public bool IsSceneLoaded(string sceneName)
	{
		return this.scenesLoaded.Contains(sceneName);
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x00057354 File Offset: 0x00055554
	private void Initialize()
	{
		ZoneManagement.instance = this;
		this.hasInstance = true;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		List<GameObject> list = new List<GameObject>(8);
		for (int i = 0; i < this.zones.Length; i++)
		{
			list.Clear();
			ZoneData zoneData = this.zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
				for (int j = 0; j < zoneData.rootGameObjects.Length; j++)
				{
					GameObject gameObject = zoneData.rootGameObjects[j];
					if (!(gameObject == null))
					{
						list.Add(gameObject);
					}
				}
				hashSet.UnionWith(list);
			}
		}
		this.allObjects = hashSet.ToArray<GameObject>();
		this.objectActivationState = new bool[this.allObjects.Length];
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x00057410 File Offset: 0x00055610
	private void SetZones(GTZone[] newActiveZones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		this.activeZones.Clear();
		for (int j = 0; j < newActiveZones.Length; j++)
		{
			this.activeZones.Add(newActiveZones[j]);
		}
		this.scenesRequested.Clear();
		this.scenesRequested.Add("GorillaTag");
		float num = 0f;
		for (int k = 0; k < this.zones.Length; k++)
		{
			ZoneData zoneData = this.zones[k];
			if (zoneData == null || zoneData.rootGameObjects == null || !newActiveZones.Contains(zoneData.zone))
			{
				zoneData.active = false;
			}
			else
			{
				zoneData.active = true;
				num = Mathf.Max(num, zoneData.CameraFarClipPlane);
				if (!string.IsNullOrEmpty(zoneData.sceneName))
				{
					this.scenesRequested.Add(zoneData.sceneName);
				}
				foreach (GameObject x in zoneData.rootGameObjects)
				{
					if (!(x == null))
					{
						for (int m = 0; m < this.allObjects.Length; m++)
						{
							if (x == this.allObjects[m])
							{
								this.objectActivationState[m] = true;
								break;
							}
						}
					}
				}
			}
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		this.mainCamera.farClipPlane = num;
		int loadedSceneCount = SceneManager.loadedSceneCount;
		for (int n = 0; n < loadedSceneCount; n++)
		{
			this.scenesLoaded.Add(SceneManager.GetSceneAt(n).name);
		}
		foreach (string text in this.scenesRequested)
		{
			if (this.scenesLoaded.Add(text))
			{
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(text, LoadSceneMode.Additive);
				this._scenes_to_loadOps[text] = asyncOperation;
				asyncOperation.completed += this.HandleOnSceneLoadCompleted;
			}
		}
		this.scenesToUnload.Clear();
		foreach (string item in this.scenesLoaded)
		{
			if (!this.scenesRequested.Contains(item) && !this.sceneForceStayLoaded.Contains(item))
			{
				this.scenesToUnload.Add(item);
			}
		}
		foreach (string text2 in this.scenesToUnload)
		{
			this.scenesLoaded.Remove(text2);
			AsyncOperation value = SceneManager.UnloadSceneAsync(text2);
			this._scenes_to_unloadOps[text2] = value;
		}
		for (int num2 = 0; num2 < this.objectActivationState.Length; num2++)
		{
			if (!(this.allObjects[num2] == null))
			{
				this.allObjects[num2].SetActive(this.objectActivationState[num2]);
			}
		}
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00057754 File Offset: 0x00055954
	private void HandleOnSceneLoadCompleted(AsyncOperation thisLoadOp)
	{
		using (Dictionary<string, AsyncOperation>.ValueCollection.Enumerator enumerator = this._scenes_to_loadOps.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDone)
				{
					return;
				}
			}
		}
		using (Dictionary<string, AsyncOperation>.ValueCollection.Enumerator enumerator = this._scenes_to_unloadOps.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDone)
				{
					return;
				}
			}
		}
		Action onSceneLoadsCompleted = this.OnSceneLoadsCompleted;
		if (onSceneLoadsCompleted == null)
		{
			return;
		}
		onSceneLoadsCompleted();
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00057808 File Offset: 0x00055A08
	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].zone == zone)
			{
				return this.zones[i];
			}
		}
		return null;
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00057842 File Offset: 0x00055A42
	public static bool IsValidZoneInt(int zoneInt)
	{
		return zoneInt >= 11 && zoneInt <= 24;
	}

	// Token: 0x0400171A RID: 5914
	public static ZoneManagement instance;

	// Token: 0x0400171C RID: 5916
	[SerializeField]
	private ZoneData[] zones;

	// Token: 0x0400171D RID: 5917
	private GameObject[] allObjects;

	// Token: 0x0400171E RID: 5918
	private bool[] objectActivationState;

	// Token: 0x0400171F RID: 5919
	public Action onZoneChanged;

	// Token: 0x04001720 RID: 5920
	public Action OnSceneLoadsCompleted;

	// Token: 0x04001721 RID: 5921
	public List<GTZone> activeZones = new List<GTZone>(20);

	// Token: 0x04001722 RID: 5922
	private HashSet<string> scenesLoaded = new HashSet<string>();

	// Token: 0x04001723 RID: 5923
	private HashSet<string> scenesRequested = new HashSet<string>();

	// Token: 0x04001724 RID: 5924
	private HashSet<string> sceneForceStayLoaded = new HashSet<string>(8);

	// Token: 0x04001725 RID: 5925
	private List<string> scenesToUnload = new List<string>();

	// Token: 0x04001726 RID: 5926
	private Dictionary<string, AsyncOperation> _scenes_to_loadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x04001727 RID: 5927
	private Dictionary<string, AsyncOperation> _scenes_to_unloadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x04001728 RID: 5928
	private Camera mainCamera;

	// Token: 0x02000268 RID: 616
	// (Invoke) Token: 0x06000E4B RID: 3659
	public delegate void ZoneChangeEvent(ZoneData[] zones);
}
