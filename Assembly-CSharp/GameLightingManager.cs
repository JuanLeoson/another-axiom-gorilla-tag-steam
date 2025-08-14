using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x020005BD RID: 1469
public class GameLightingManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600240E RID: 9230 RVA: 0x000C1392 File Offset: 0x000BF592
	private void Awake()
	{
		this.InitData();
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000C139C File Offset: 0x000BF59C
	private void InitData()
	{
		GameLightingManager.instance = this;
		this.gameLights = new List<GameLight>(512);
		this.lightDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<GameLightingManager.LightData>());
		this.lightData = new GameLightingManager.LightData[50];
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000C1406 File Offset: 0x000BF606
	private void OnDestroy()
	{
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000C142C File Offset: 0x000BF62C
	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		this.customVertexLightingEnabled = enable;
		if (this.customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
			return;
		}
		Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000C1452 File Offset: 0x000BF652
	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor("_GT_GameLight_Ambient_Color", color);
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x000C145F File Offset: 0x000BF65F
	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor("_GT_DesaturateAndTint_TintColor", tint);
		Shader.SetGlobalFloat("_GT_DesaturateAndTint_TintAmount", enable ? 1f : 0f);
		this.desaturateAndTintEnabled = enable;
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000C148C File Offset: 0x000BF68C
	public void SliceUpdate()
	{
		if (this.mainCameraTransform == null)
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		if (this.skipNextSlice)
		{
			this.skipNextSlice = false;
			return;
		}
		this.immediateSort = false;
		this.SortLights();
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000C14C9 File Offset: 0x000BF6C9
	public void SortLights()
	{
		if (this.gameLights.Count <= 50)
		{
			return;
		}
		this.cameraPosForSort = this.mainCameraTransform.position;
		this.gameLights.Sort(new Comparison<GameLight>(this.CompareDistFromCamera));
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000C1503 File Offset: 0x000BF703
	private void Update()
	{
		this.RefreshLightData();
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000C150C File Offset: 0x000BF70C
	private void RefreshLightData()
	{
		if (this.lightData == null)
		{
			return;
		}
		if (this.customVertexLightingEnabled)
		{
			if (this.immediateSort)
			{
				this.immediateSort = false;
				this.skipNextSlice = true;
				this.SortLights();
			}
			for (int i = 0; i < 50; i++)
			{
				if (i < this.gameLights.Count)
				{
					this.GetFromLight(i, i);
				}
				else
				{
					this.ResetLight(i);
				}
			}
			this.lightDataBuffer.SetData(this.lightData);
			Shader.SetGlobalBuffer("_GT_GameLight_Lights", this.lightDataBuffer);
		}
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000C1594 File Offset: 0x000BF794
	public int AddGameLight(GameLight light, bool ignoreUnityLightDisable = false)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (this.gameLights.Contains(light))
		{
			return -1;
		}
		if (!ignoreUnityLightDisable)
		{
			light.light.enabled = false;
		}
		this.gameLights.Add(light);
		this.immediateSort = true;
		return this.gameLights.Count - 1;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000C1613 File Offset: 0x000BF813
	public void RemoveGameLight(GameLight light)
	{
		if (light != null && light.light != null)
		{
			light.light.enabled = true;
		}
		this.gameLights.Remove(light);
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000C1648 File Offset: 0x000BF848
	public void ClearGameLights()
	{
		if (this.gameLights != null)
		{
			this.gameLights.Clear();
		}
		if (this.lightData == null)
		{
			return;
		}
		for (int i = 0; i < this.lightData.Length; i++)
		{
			this.ResetLight(i);
		}
		this.lightDataBuffer.SetData(this.lightData);
		Shader.SetGlobalBuffer("_GT_GameLight_Lights", this.lightDataBuffer);
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000C16AC File Offset: 0x000BF8AC
	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		if (this.lightData == null)
		{
			return;
		}
		GameLight gameLight = null;
		if (gameLightIndex >= 0 && gameLightIndex < this.gameLights.Count)
		{
			gameLight = this.gameLights[gameLightIndex];
		}
		if (gameLight == null || gameLight.light == null)
		{
			return;
		}
		Vector4 lightPos = gameLight.light.transform.position;
		lightPos.w = 1f;
		Color c = gameLight.light.color * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f);
		Vector3 forward = gameLight.light.transform.forward;
		GameLightingManager.LightData lightData = new GameLightingManager.LightData
		{
			lightPos = lightPos,
			lightColor = c,
			lightDirection = forward
		};
		this.lightData[lightIndex] = lightData;
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000C17A0 File Offset: 0x000BF9A0
	private void ResetLight(int lightIndex)
	{
		GameLightingManager.LightData lightData = new GameLightingManager.LightData
		{
			lightPos = Vector4.zero,
			lightColor = Color.black,
			lightDirection = Vector4.zero
		};
		this.lightData[lightIndex] = lightData;
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000C17F0 File Offset: 0x000BF9F0
	private int CompareDistFromCamera(GameLight a, GameLight b)
	{
		if (a == null || a.light == null)
		{
			if (b == null || b.light == null)
			{
				return 0;
			}
			return -1;
		}
		else
		{
			if (b == null || b.light == null)
			{
				return 1;
			}
			float sqrMagnitude = (this.cameraPosForSort - a.light.transform.position).sqrMagnitude;
			float sqrMagnitude2 = (this.cameraPosForSort - b.light.transform.position).sqrMagnitude;
			return sqrMagnitude.CompareTo(sqrMagnitude2);
		}
	}

	// Token: 0x04002D7E RID: 11646
	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	// Token: 0x04002D7F RID: 11647
	public const int MAX_VERTEX_LIGHTS = 50;

	// Token: 0x04002D80 RID: 11648
	public Transform testLightsCenter;

	// Token: 0x04002D81 RID: 11649
	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	// Token: 0x04002D82 RID: 11650
	public float testLightBrightness = 10f;

	// Token: 0x04002D83 RID: 11651
	public float testLightRadius = 2f;

	// Token: 0x04002D84 RID: 11652
	public int maxUseTestLights = 1;

	// Token: 0x04002D85 RID: 11653
	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	// Token: 0x04002D86 RID: 11654
	private bool customVertexLightingEnabled;

	// Token: 0x04002D87 RID: 11655
	private bool desaturateAndTintEnabled;

	// Token: 0x04002D88 RID: 11656
	private Transform mainCameraTransform;

	// Token: 0x04002D89 RID: 11657
	private GameLightingManager.LightData[] lightData;

	// Token: 0x04002D8A RID: 11658
	private GraphicsBuffer lightDataBuffer;

	// Token: 0x04002D8B RID: 11659
	private Vector3 cameraPosForSort;

	// Token: 0x04002D8C RID: 11660
	private bool skipNextSlice;

	// Token: 0x04002D8D RID: 11661
	private bool immediateSort;

	// Token: 0x020005BE RID: 1470
	private struct LightData
	{
		// Token: 0x04002D8E RID: 11662
		public Vector4 lightPos;

		// Token: 0x04002D8F RID: 11663
		public Vector4 lightColor;

		// Token: 0x04002D90 RID: 11664
		public Vector4 lightDirection;
	}
}
