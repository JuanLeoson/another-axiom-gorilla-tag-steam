using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000883 RID: 2179
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x06003693 RID: 13971 RVA: 0x0011D84D File Offset: 0x0011BA4D
	public Material AcquireMaterial(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		Material[] array = this.instanceMaterials;
		if (array != null && array.Length != 0)
		{
			return this.instanceMaterials[0];
		}
		return null;
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x0011D88B File Offset: 0x0011BA8B
	public Material[] AcquireMaterials(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		base.gameObject.GetComponent<Material>();
		return this.instanceMaterials;
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x0011D8BE File Offset: 0x0011BABE
	public void ReleaseMaterial(Object owner, bool autoDestroy = true)
	{
		this.materialOwners.Remove(owner);
		if (autoDestroy && this.materialOwners.Count == 0)
		{
			MaterialInstance.DestroySafe(this);
			if (!base.gameObject.activeInHierarchy)
			{
				this.RestoreRenderer();
			}
		}
	}

	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x06003696 RID: 13974 RVA: 0x0011D8F6 File Offset: 0x0011BAF6
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x06003697 RID: 13975 RVA: 0x0011D900 File Offset: 0x0011BB00
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x06003698 RID: 13976 RVA: 0x0011D90A File Offset: 0x0011BB0A
	// (set) Token: 0x06003699 RID: 13977 RVA: 0x0011D912 File Offset: 0x0011BB12
	public bool CacheSharedMaterialsFromRenderer
	{
		get
		{
			return this.cacheSharedMaterialsFromRenderer;
		}
		set
		{
			if (this.cacheSharedMaterialsFromRenderer != value)
			{
				if (value)
				{
					this.cachedSharedMaterials = this.CachedRenderer.sharedMaterials;
				}
				else
				{
					this.cachedSharedMaterials = null;
				}
				this.cacheSharedMaterialsFromRenderer = value;
			}
		}
	}

	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x0600369A RID: 13978 RVA: 0x0011D941 File Offset: 0x0011BB41
	private Renderer CachedRenderer
	{
		get
		{
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponent<Renderer>();
				if (this.CacheSharedMaterialsFromRenderer)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
			}
			return this.cachedRenderer;
		}
	}

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x0600369B RID: 13979 RVA: 0x0011D97C File Offset: 0x0011BB7C
	// (set) Token: 0x0600369C RID: 13980 RVA: 0x0011D9B1 File Offset: 0x0011BBB1
	private Material[] CachedRendererSharedMaterials
	{
		get
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				if (this.cachedSharedMaterials == null)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
				return this.cachedSharedMaterials;
			}
			return this.cachedRenderer.sharedMaterials;
		}
		set
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				this.cachedSharedMaterials = value;
			}
			this.cachedRenderer.sharedMaterials = value;
		}
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x0011D9CE File Offset: 0x0011BBCE
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x0011D9D6 File Offset: 0x0011BBD6
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x0600369F RID: 13983 RVA: 0x0011D9DE File Offset: 0x0011BBDE
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x060036A0 RID: 13984 RVA: 0x0011DA14 File Offset: 0x0011BC14
	private void Initialize()
	{
		if (!this.initialized && this.CachedRenderer != null)
		{
			if (!MaterialInstance.HasValidMaterial(this.defaultMaterials))
			{
				this.defaultMaterials = this.CachedRendererSharedMaterials;
			}
			else if (!this.materialsInstanced)
			{
				this.CachedRendererSharedMaterials = this.defaultMaterials;
			}
			this.initialized = true;
		}
	}

	// Token: 0x060036A1 RID: 13985 RVA: 0x0011DA6D File Offset: 0x0011BC6D
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x060036A2 RID: 13986 RVA: 0x0011DA98 File Offset: 0x0011BC98
	private void CreateInstances()
	{
		this.Initialize();
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = MaterialInstance.InstanceMaterials(this.defaultMaterials);
		if (this.CachedRenderer != null && this.instanceMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.instanceMaterials;
		}
		this.materialsInstanced = true;
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x0011DAF0 File Offset: 0x0011BCF0
	private static bool MaterialsMatch(Material[] a, Material[] b)
	{
		int? num = (a != null) ? new int?(a.Length) : null;
		int? num2 = (b != null) ? new int?(b.Length) : null;
		if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)))
		{
			return false;
		}
		int num3 = 0;
		for (;;)
		{
			int num4 = num3;
			num2 = ((a != null) ? new int?(a.Length) : null);
			if (!(num4 < num2.GetValueOrDefault() & num2 != null))
			{
				return true;
			}
			if (a[num3] != b[num3])
			{
				break;
			}
			num3++;
		}
		return false;
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x0011DB94 File Offset: 0x0011BD94
	private static Material[] InstanceMaterials(Material[] source)
	{
		if (source == null)
		{
			return null;
		}
		Material[] array = new Material[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != null)
			{
				if (MaterialInstance.IsInstanceMaterial(source[i]))
				{
					Debug.LogWarning("A material (" + source[i].name + ") which is already instanced was instanced multiple times.");
				}
				array[i] = new Material(source[i]);
				Material material = array[i];
				material.name += " (Instance)";
			}
		}
		return array;
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x0011DC14 File Offset: 0x0011BE14
	private static void DestroyMaterials(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				MaterialInstance.DestroySafe(materials[i]);
			}
		}
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x0011DC3A File Offset: 0x0011BE3A
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x0011DC58 File Offset: 0x0011BE58
	private static bool HasValidMaterial(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060036A8 RID: 13992 RVA: 0x0011DC86 File Offset: 0x0011BE86
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x04004397 RID: 17303
	private Renderer cachedRenderer;

	// Token: 0x04004398 RID: 17304
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x04004399 RID: 17305
	private Material[] instanceMaterials;

	// Token: 0x0400439A RID: 17306
	private Material[] cachedSharedMaterials;

	// Token: 0x0400439B RID: 17307
	private bool initialized;

	// Token: 0x0400439C RID: 17308
	private bool materialsInstanced;

	// Token: 0x0400439D RID: 17309
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x0400439E RID: 17310
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x0400439F RID: 17311
	private const string instancePostfix = " (Instance)";
}
