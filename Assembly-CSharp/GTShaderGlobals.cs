using System;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class GTShaderGlobals : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x00040B20 File Offset: 0x0003ED20
	public static Vector3 WorldSpaceCameraPos
	{
		get
		{
			return GTShaderGlobals.gMainCameraWorldPos;
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x00040B27 File Offset: 0x0003ED27
	public static float Time
	{
		get
		{
			return GTShaderGlobals.gTime;
		}
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x00040B2E File Offset: 0x0003ED2E
	public static int Frame
	{
		get
		{
			return GTShaderGlobals.gIFrame;
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00040B35 File Offset: 0x0003ED35
	private void Awake()
	{
		GTShaderGlobals.gMainCamera = Camera.main;
		if (GTShaderGlobals.gMainCamera)
		{
			GTShaderGlobals.gMainCameraXform = GTShaderGlobals.gMainCamera.transform;
			GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		}
		this.SliceUpdate();
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00040B71 File Offset: 0x0003ED71
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		GTShaderGlobals.InitBlueNoiseTex();
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x00040B78 File Offset: 0x0003ED78
	public void SliceUpdate()
	{
		GTShaderGlobals.UpdateTime();
		GTShaderGlobals.UpdateFrame();
		GTShaderGlobals.UpdateCamera();
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00040B89 File Offset: 0x0003ED89
	private static void UpdateFrame()
	{
		GTShaderGlobals.gIFrame = UnityEngine.Time.frameCount;
		Shader.SetGlobalInteger(GTShaderGlobals._GT_iFrame, GTShaderGlobals.gIFrame);
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00040BA9 File Offset: 0x0003EDA9
	private static void UpdateCamera()
	{
		if (!GTShaderGlobals.gMainCameraXform)
		{
			return;
		}
		GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		Shader.SetGlobalVector(GTShaderGlobals._GT_WorldSpaceCameraPos, GTShaderGlobals.gMainCameraWorldPos);
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00040BE0 File Offset: 0x0003EDE0
	private static void UpdateTime()
	{
		GTShaderGlobals.gTime = (float)(DateTime.UtcNow - GTShaderGlobals.gStartTime).TotalSeconds;
		Shader.SetGlobalFloat(GTShaderGlobals._GT_Time, GTShaderGlobals.gTime);
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x00040C1E File Offset: 0x0003EE1E
	private static void UpdatePawns()
	{
		GTShaderGlobals.gActivePawns = GorillaPawn.ActiveCount;
		GorillaPawn.SyncPawnData();
		Shader.SetGlobalMatrixArray(GTShaderGlobals._GT_PawnData, GTShaderGlobals.gPawnData);
		Shader.SetGlobalInteger(GTShaderGlobals._GT_PawnActiveCount, GTShaderGlobals.gActivePawns);
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00040C58 File Offset: 0x0003EE58
	private static void InitBlueNoiseTex()
	{
		GTShaderGlobals.gBlueNoiseTex = Resources.Load<Texture2D>("Graphics/Textures/noise_blue_rgba_128");
		GTShaderGlobals.gBlueNoiseTexWH = GTShaderGlobals.gBlueNoiseTex.GetTexelSize();
		Shader.SetGlobalTexture(GTShaderGlobals._GT_BlueNoiseTex, GTShaderGlobals.gBlueNoiseTex);
		Shader.SetGlobalVector(GTShaderGlobals._GT_BlueNoiseTex_WH, GTShaderGlobals.gBlueNoiseTexWH);
	}

	// Token: 0x04000EC8 RID: 3784
	private static Camera gMainCamera;

	// Token: 0x04000EC9 RID: 3785
	private static Transform gMainCameraXform;

	// Token: 0x04000ECA RID: 3786
	private static Vector3 gMainCameraWorldPos;

	// Token: 0x04000ECB RID: 3787
	[Space]
	private static int gIFrame;

	// Token: 0x04000ECC RID: 3788
	private static float gTime;

	// Token: 0x04000ECD RID: 3789
	[Space]
	private static Texture2D gBlueNoiseTex;

	// Token: 0x04000ECE RID: 3790
	private static Vector4 gBlueNoiseTexWH;

	// Token: 0x04000ECF RID: 3791
	[Space]
	private static int gActivePawns;

	// Token: 0x04000ED0 RID: 3792
	[Space]
	private static DateTime gStartTime = DateTime.Today.AddDays(-1.0).ToUniversalTime();

	// Token: 0x04000ED1 RID: 3793
	private static Matrix4x4[] gPawnData = GorillaPawn.ShaderData;

	// Token: 0x04000ED2 RID: 3794
	private static ShaderHashId _GT_WorldSpaceCameraPos = "_GT_WorldSpaceCameraPos";

	// Token: 0x04000ED3 RID: 3795
	private static ShaderHashId _GT_BlueNoiseTex = "_GT_BlueNoiseTex";

	// Token: 0x04000ED4 RID: 3796
	private static ShaderHashId _GT_BlueNoiseTex_WH = "_GT_BlueNoiseTex_WH";

	// Token: 0x04000ED5 RID: 3797
	private static ShaderHashId _GT_iFrame = "_GT_iFrame";

	// Token: 0x04000ED6 RID: 3798
	private static ShaderHashId _GT_Time = "_GT_Time";

	// Token: 0x04000ED7 RID: 3799
	private static ShaderHashId _GT_PawnData = "_GT_PawnData";

	// Token: 0x04000ED8 RID: 3800
	private static ShaderHashId _GT_PawnActiveCount = "_GT_PawnActiveCount";
}
