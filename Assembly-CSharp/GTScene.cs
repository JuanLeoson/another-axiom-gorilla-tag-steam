using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000B0E RID: 2830
[Serializable]
public class GTScene : IEquatable<GTScene>
{
	// Token: 0x17000663 RID: 1635
	// (get) Token: 0x06004419 RID: 17433 RVA: 0x00155968 File Offset: 0x00153B68
	public string alias
	{
		get
		{
			return this._alias;
		}
	}

	// Token: 0x17000664 RID: 1636
	// (get) Token: 0x0600441A RID: 17434 RVA: 0x00155970 File Offset: 0x00153B70
	public string name
	{
		get
		{
			return this._name;
		}
	}

	// Token: 0x17000665 RID: 1637
	// (get) Token: 0x0600441B RID: 17435 RVA: 0x00155978 File Offset: 0x00153B78
	public string path
	{
		get
		{
			return this._path;
		}
	}

	// Token: 0x17000666 RID: 1638
	// (get) Token: 0x0600441C RID: 17436 RVA: 0x00155980 File Offset: 0x00153B80
	public string guid
	{
		get
		{
			return this._guid;
		}
	}

	// Token: 0x17000667 RID: 1639
	// (get) Token: 0x0600441D RID: 17437 RVA: 0x00155988 File Offset: 0x00153B88
	public int buildIndex
	{
		get
		{
			return this._buildIndex;
		}
	}

	// Token: 0x17000668 RID: 1640
	// (get) Token: 0x0600441E RID: 17438 RVA: 0x00155990 File Offset: 0x00153B90
	public bool includeInBuild
	{
		get
		{
			return this._includeInBuild;
		}
	}

	// Token: 0x17000669 RID: 1641
	// (get) Token: 0x0600441F RID: 17439 RVA: 0x00155998 File Offset: 0x00153B98
	public bool isLoaded
	{
		get
		{
			return SceneManager.GetSceneByBuildIndex(this._buildIndex).isLoaded;
		}
	}

	// Token: 0x1700066A RID: 1642
	// (get) Token: 0x06004420 RID: 17440 RVA: 0x001559B8 File Offset: 0x00153BB8
	public bool hasAlias
	{
		get
		{
			return !string.IsNullOrWhiteSpace(this._alias);
		}
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x001559C8 File Offset: 0x00153BC8
	public GTScene(string name, string path, string guid, int buildIndex, bool includeInBuild)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException("name");
		}
		if (string.IsNullOrWhiteSpace(path))
		{
			throw new ArgumentNullException("path");
		}
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this._name = name;
		this._path = path;
		this._guid = guid;
		this._buildIndex = buildIndex;
		this._includeInBuild = includeInBuild;
	}

	// Token: 0x06004422 RID: 17442 RVA: 0x00155A39 File Offset: 0x00153C39
	public override int GetHashCode()
	{
		return this._guid.GetHashCode();
	}

	// Token: 0x06004423 RID: 17443 RVA: 0x00155A46 File Offset: 0x00153C46
	public override string ToString()
	{
		return this.ToJson(false);
	}

	// Token: 0x06004424 RID: 17444 RVA: 0x00155A4F File Offset: 0x00153C4F
	public bool Equals(GTScene other)
	{
		return this._guid.Equals(other._guid) && this._name == other._name && this._path == other._path;
	}

	// Token: 0x06004425 RID: 17445 RVA: 0x00155A8C File Offset: 0x00153C8C
	public override bool Equals(object obj)
	{
		GTScene gtscene = obj as GTScene;
		return gtscene != null && this.Equals(gtscene);
	}

	// Token: 0x06004426 RID: 17446 RVA: 0x00155AAC File Offset: 0x00153CAC
	public static bool operator ==(GTScene x, GTScene y)
	{
		return x.Equals(y);
	}

	// Token: 0x06004427 RID: 17447 RVA: 0x00155AB5 File Offset: 0x00153CB5
	public static bool operator !=(GTScene x, GTScene y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06004428 RID: 17448 RVA: 0x00155AC1 File Offset: 0x00153CC1
	public void LoadAsync()
	{
		if (this.isLoaded)
		{
			return;
		}
		SceneManager.LoadSceneAsync(this._buildIndex, LoadSceneMode.Additive);
	}

	// Token: 0x06004429 RID: 17449 RVA: 0x00155AD9 File Offset: 0x00153CD9
	public void UnloadAsync()
	{
		if (!this.isLoaded)
		{
			return;
		}
		SceneManager.UnloadSceneAsync(this._buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
	}

	// Token: 0x0600442A RID: 17450 RVA: 0x00058615 File Offset: 0x00056815
	public static GTScene FromAsset(object sceneAsset)
	{
		return null;
	}

	// Token: 0x0600442B RID: 17451 RVA: 0x00058615 File Offset: 0x00056815
	public static GTScene From(object editorBuildSettingsScene)
	{
		return null;
	}

	// Token: 0x04004E6D RID: 20077
	[SerializeField]
	private string _alias;

	// Token: 0x04004E6E RID: 20078
	[SerializeField]
	private string _name;

	// Token: 0x04004E6F RID: 20079
	[SerializeField]
	private string _path;

	// Token: 0x04004E70 RID: 20080
	[SerializeField]
	private string _guid;

	// Token: 0x04004E71 RID: 20081
	[SerializeField]
	private int _buildIndex;

	// Token: 0x04004E72 RID: 20082
	[SerializeField]
	private bool _includeInBuild;
}
