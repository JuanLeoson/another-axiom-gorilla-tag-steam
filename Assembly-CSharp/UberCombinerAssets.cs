using System;
using UnityEngine;

// Token: 0x02000B41 RID: 2881
public class UberCombinerAssets : ScriptableObject
{
	// Token: 0x17000688 RID: 1672
	// (get) Token: 0x06004557 RID: 17751 RVA: 0x00159C3F File Offset: 0x00157E3F
	public static UberCombinerAssets Instance
	{
		get
		{
			UberCombinerAssets.gInstance == null;
			return UberCombinerAssets.gInstance;
		}
	}

	// Token: 0x06004558 RID: 17752 RVA: 0x00159C52 File Offset: 0x00157E52
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06004559 RID: 17753 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Setup()
	{
	}

	// Token: 0x0600455A RID: 17754 RVA: 0x000023F5 File Offset: 0x000005F5
	public void ClearMaterialAssets()
	{
	}

	// Token: 0x0600455B RID: 17755 RVA: 0x000023F5 File Offset: 0x000005F5
	public void ClearPrefabAssets()
	{
	}

	// Token: 0x04004F98 RID: 20376
	[SerializeField]
	private Object _rootFolder;

	// Token: 0x04004F99 RID: 20377
	[SerializeField]
	private Object _resourcesFolder;

	// Token: 0x04004F9A RID: 20378
	[SerializeField]
	private Object _materialsFolder;

	// Token: 0x04004F9B RID: 20379
	[SerializeField]
	private Object _prefabsFolder;

	// Token: 0x04004F9C RID: 20380
	[Space]
	public Object MeshBakerDefaultCustomizer;

	// Token: 0x04004F9D RID: 20381
	public Material ReferenceUberMaterial;

	// Token: 0x04004F9E RID: 20382
	public Shader TextureArrayCapableShader;

	// Token: 0x04004F9F RID: 20383
	[Space]
	public string RootFolderPath;

	// Token: 0x04004FA0 RID: 20384
	public string ResourcesFolderPath;

	// Token: 0x04004FA1 RID: 20385
	public string MaterialsFolderPath;

	// Token: 0x04004FA2 RID: 20386
	public string PrefabsFolderPath;

	// Token: 0x04004FA3 RID: 20387
	private static UberCombinerAssets gInstance;
}
