using System;
using UnityEngine;

// Token: 0x020006E5 RID: 1765
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x06002BE8 RID: 11240 RVA: 0x000E8D28 File Offset: 0x000E6F28
	public void Awake()
	{
		this.lights = new Color[this.lightTextures.Length][];
		this.dirs = new Color[this.dirTextures.Length][];
		for (int i = 0; i < this.dirTextures.Length; i++)
		{
			float value = Random.value;
			Debug.Log(value.ToString() + " before load " + Time.realtimeSinceStartup.ToString());
			this.dirs[i] = this.dirTextures[i].GetPixels();
			this.lights[i] = this.lightTextures[i].GetPixels();
			Debug.Log(value.ToString() + " after load " + Time.realtimeSinceStartup.ToString());
		}
	}

	// Token: 0x0400374F RID: 14159
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04003750 RID: 14160
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04003751 RID: 14161
	public Color[][] lights;

	// Token: 0x04003752 RID: 14162
	public Color[][] dirs;

	// Token: 0x04003753 RID: 14163
	public bool done;
}
