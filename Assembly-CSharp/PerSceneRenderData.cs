using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class PerSceneRenderData : MonoBehaviour
{
	// Token: 0x06000C8A RID: 3210 RVA: 0x00043904 File Offset: 0x00041B04
	private void RefreshRenderer()
	{
		int sceneIndex = this.sceneIndex;
		new List<Renderer>();
		foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
		{
			if (renderer.gameObject.scene.buildIndex == sceneIndex)
			{
				this.representativeRenderer = renderer;
				return;
			}
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000C8B RID: 3211 RVA: 0x00043958 File Offset: 0x00041B58
	public string sceneName
	{
		get
		{
			return base.gameObject.scene.name;
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000C8C RID: 3212 RVA: 0x00043978 File Offset: 0x00041B78
	public int sceneIndex
	{
		get
		{
			return base.gameObject.scene.buildIndex;
		}
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00043998 File Offset: 0x00041B98
	private void Awake()
	{
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			this.mRenderers[i] = this.gO[i].GetComponent<MeshRenderer>();
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x000439CB File Offset: 0x00041BCB
	private void OnEnable()
	{
		BetterDayNightManager.Register(this);
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x000439D3 File Offset: 0x00041BD3
	private void OnDisable()
	{
		BetterDayNightManager.Unregister(this);
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x000439DC File Offset: 0x00041BDC
	public void AddMeshToList(GameObject _gO, MeshRenderer mR)
	{
		try
		{
			if (mR.lightmapIndex != -1)
			{
				this.gO[this.mRendererIndex] = _gO;
				this.mRendererIndex++;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00043A28 File Offset: 0x00041C28
	public bool CheckShouldRepopulate()
	{
		return this.representativeRenderer.lightmapIndex != this.lastLightmapIndex;
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000C92 RID: 3218 RVA: 0x00043A40 File Offset: 0x00041C40
	public bool IsLoadingLightmaps
	{
		get
		{
			return this.resourceRequests.Count != 0;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000C93 RID: 3219 RVA: 0x00043A50 File Offset: 0x00041C50
	public int LoadingLightmapsCount
	{
		get
		{
			return this.resourceRequests.Count;
		}
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00043A60 File Offset: 0x00041C60
	private Texture2D GetLightmap(string timeOfDay)
	{
		if (this.singleLightmap != null)
		{
			return this.singleLightmap;
		}
		Texture2D result;
		if (!this.lightmapsCache.TryGetValue(timeOfDay, out result))
		{
			ResourceRequest request;
			if (this.resourceRequests.TryGetValue(timeOfDay, out request))
			{
				return null;
			}
			request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, timeOfDay));
			this.resourceRequests.Add(timeOfDay, request);
			request.completed += delegate(AsyncOperation ao)
			{
				if (this == null)
				{
					return;
				}
				this.lightmapsCache.Add(timeOfDay, (Texture2D)request.asset);
				this.resourceRequests.Remove(timeOfDay);
				if (BetterDayNightManager.instance != null)
				{
					BetterDayNightManager.instance.RequestRepopulateLightmaps();
				}
			};
		}
		return result;
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x00043B14 File Offset: 0x00041D14
	public void PopulateLightmaps(string fromTimeOfDay, string toTimeOfDay, LightmapData[] lightmaps)
	{
		LightmapData lightmapData = new LightmapData();
		lightmapData.lightmapColor = this.GetLightmap(fromTimeOfDay);
		lightmapData.lightmapDir = this.GetLightmap(toTimeOfDay);
		if (lightmapData.lightmapColor != null && lightmapData.lightmapDir != null && this.representativeRenderer.lightmapIndex < lightmaps.Length)
		{
			lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
		}
		this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			if (i < this.mRenderers.Length && this.mRenderers[i] != null)
			{
				this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
			}
		}
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x00043BCC File Offset: 0x00041DCC
	public void ReleaseLightmap(string oldTimeOfDay)
	{
		Texture2D assetToUnload;
		if (this.lightmapsCache.Remove(oldTimeOfDay, out assetToUnload))
		{
			Resources.UnloadAsset(assetToUnload);
		}
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x00043BF0 File Offset: 0x00041DF0
	private void TryGetLightmapOrAsyncLoad(string momentName, Action<Texture2D> callback)
	{
		if (this.singleLightmap != null)
		{
			callback(this.singleLightmap);
		}
		Texture2D obj;
		if (this.lightmapsCache.TryGetValue(momentName, out obj))
		{
			callback(obj);
		}
		List<Action<Texture2D>> callbacks;
		if (!this._momentName_to_callbacks.TryGetValue(momentName, out callbacks))
		{
			callbacks = new List<Action<Texture2D>>(8);
			this._momentName_to_callbacks[momentName] = callbacks;
		}
		if (!callbacks.Contains(callback))
		{
			callbacks.Add(callback);
		}
		ResourceRequest request;
		if (this.resourceRequests.TryGetValue(momentName, out request))
		{
			return;
		}
		request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, momentName));
		this.resourceRequests.Add(momentName, request);
		request.completed += delegate(AsyncOperation ao)
		{
			if (this == null || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			Texture2D texture2D = (Texture2D)request.asset;
			this.lightmapsCache.Add(momentName, texture2D);
			this.resourceRequests.Remove(momentName);
			foreach (Action<Texture2D> action in callbacks)
			{
				if (action != null)
				{
					action(texture2D);
				}
			}
			callbacks.Clear();
		};
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00043D04 File Offset: 0x00041F04
	public bool IsLightmapWithNameLoaded(string lightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(lightmapName) && ((!string.IsNullOrEmpty(text) && text == lightmapName) || (!string.IsNullOrEmpty(text2) && text2 == lightmapName));
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x00043D5C File Offset: 0x00041F5C
	public bool IsLightmapsWithNamesLoaded(string fromLightmapName, string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(fromLightmapName) && !string.IsNullOrEmpty(toLightmapName) && !string.IsNullOrEmpty(text) && text == fromLightmapName && !string.IsNullOrEmpty(text2) && text2 == toLightmapName;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x00043DB8 File Offset: 0x00041FB8
	public void GetFromAndToLightmapNames(out string fromLightmapName, out string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		if (this.representativeRenderer.lightmapIndex < 0 || this.representativeRenderer.lightmapIndex >= lightmaps.Length)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		Texture2D lightmapColor = lightmaps[this.representativeRenderer.lightmapIndex].lightmapColor;
		Texture2D lightmapDir = lightmaps[this.representativeRenderer.lightmapIndex].lightmapDir;
		fromLightmapName = ((lightmapColor != null) ? lightmapColor.name : null);
		toLightmapName = ((lightmapDir != null) ? lightmapDir.name : null);
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x00043E54 File Offset: 0x00042054
	public static void g_StartAllScenesPopulateLightmaps(string fromLightmapName, string toLightmapName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		PerSceneRenderData[] array = Object.FindObjectsByType<PerSceneRenderData>(FindObjectsSortMode.None);
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.UnionWith(array);
		foreach (PerSceneRenderData perSceneRenderData in array)
		{
			perSceneRenderData.StartPopulateLightmaps(fromLightmapName, toLightmapName);
			perSceneRenderData.OnPopulateToAndFromLightmapsCompleted = (Action<PerSceneRenderData>)Delegate.Combine(perSceneRenderData.OnPopulateToAndFromLightmapsCompleted, new Action<PerSceneRenderData>(PerSceneRenderData._g_AllScenesPopulateLightmaps_OnOneCompleted));
		}
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x00043EBC File Offset: 0x000420BC
	private static void _g_AllScenesPopulateLightmaps_OnOneCompleted(PerSceneRenderData perSceneRenderData)
	{
		int count = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Remove(perSceneRenderData);
		int count2 = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		if (count2 == 0 && count2 != count)
		{
			Action action = PerSceneRenderData.g_OnAllScenesPopulateLightmapsCompleted;
			if (action == null)
			{
				return;
			}
			action();
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000C9D RID: 3229 RVA: 0x00043F01 File Offset: 0x00042101
	public static int g_AllScenesPopulatingLightmapsLoadCount
	{
		get
		{
			return PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		}
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x00043F10 File Offset: 0x00042110
	public void StartPopulateLightmaps(string fromMomentName, string toMomentName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		this._populateLightmaps_fromMomentLightmap = null;
		this._populateLightmaps_toMomentLightmap = null;
		this._populateLightmaps_fromMomentName = fromMomentName;
		this._populateLightmaps_toMomentName = toMomentName;
		this.TryGetLightmapOrAsyncLoad(fromMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
		this.TryGetLightmapOrAsyncLoad(toMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x00043F6C File Offset: 0x0004216C
	private void _PopulateLightmaps_OnLoadLightmap(Texture2D lightmapTex)
	{
		if (this == null || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this._populateLightmaps_fromMomentName != lightmapTex.name)
		{
			this._populateLightmaps_fromMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_toMomentName != lightmapTex.name)
		{
			this._populateLightmaps_toMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_fromMomentLightmap != null && this._populateLightmaps_toMomentLightmap != null)
		{
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			LightmapData lightmapData = new LightmapData
			{
				lightmapColor = this._populateLightmaps_fromMomentLightmap,
				lightmapDir = this._populateLightmaps_toMomentLightmap
			};
			if (this.representativeRenderer.lightmapIndex >= 0 && this.representativeRenderer.lightmapIndex < lightmaps.Length)
			{
				lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
			}
			LightmapSettings.lightmaps = lightmaps;
			this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
			for (int i = 0; i < this.mRendererIndex; i++)
			{
				if (i < this.mRenderers.Length && this.mRenderers[i] != null)
				{
					this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
				}
			}
			Action<PerSceneRenderData> onPopulateToAndFromLightmapsCompleted = this.OnPopulateToAndFromLightmapsCompleted;
			if (onPopulateToAndFromLightmapsCompleted == null)
			{
				return;
			}
			onPopulateToAndFromLightmapsCompleted(this);
		}
	}

	// Token: 0x04000F86 RID: 3974
	public Renderer representativeRenderer;

	// Token: 0x04000F87 RID: 3975
	public string lightmapsResourcePath;

	// Token: 0x04000F88 RID: 3976
	public Texture2D singleLightmap;

	// Token: 0x04000F89 RID: 3977
	private int lastLightmapIndex = -1;

	// Token: 0x04000F8A RID: 3978
	public GameObject[] gO = new GameObject[5000];

	// Token: 0x04000F8B RID: 3979
	public MeshRenderer[] mRenderers = new MeshRenderer[5000];

	// Token: 0x04000F8C RID: 3980
	public int mRendererIndex;

	// Token: 0x04000F8D RID: 3981
	private readonly Dictionary<string, ResourceRequest> resourceRequests = new Dictionary<string, ResourceRequest>(8);

	// Token: 0x04000F8E RID: 3982
	private readonly Dictionary<string, Texture2D> lightmapsCache = new Dictionary<string, Texture2D>(8);

	// Token: 0x04000F8F RID: 3983
	private Dictionary<string, List<Action<Texture2D>>> _momentName_to_callbacks = new Dictionary<string, List<Action<Texture2D>>>(8);

	// Token: 0x04000F90 RID: 3984
	private static readonly HashSet<PerSceneRenderData> _g_allScenesPopulateLightmaps_renderDatasHashSet = new HashSet<PerSceneRenderData>(32);

	// Token: 0x04000F91 RID: 3985
	public static Action g_OnAllScenesPopulateLightmapsCompleted;

	// Token: 0x04000F92 RID: 3986
	private string _populateLightmaps_fromMomentName;

	// Token: 0x04000F93 RID: 3987
	private string _populateLightmaps_toMomentName;

	// Token: 0x04000F94 RID: 3988
	private Texture2D _populateLightmaps_fromMomentLightmap;

	// Token: 0x04000F95 RID: 3989
	private Texture2D _populateLightmaps_toMomentLightmap;

	// Token: 0x04000F96 RID: 3990
	public Action<PerSceneRenderData> OnPopulateToAndFromLightmapsCompleted;
}
