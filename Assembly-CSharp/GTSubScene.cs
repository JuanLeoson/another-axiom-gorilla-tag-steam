using System;
using UnityEngine;

// Token: 0x02000B10 RID: 2832
public class GTSubScene : ScriptableObject
{
	// Token: 0x06004430 RID: 17456 RVA: 0x00155B1C File Offset: 0x00153D1C
	public void SwitchToScene(int index)
	{
		this.scenes[index].LoadAsync();
	}

	// Token: 0x06004431 RID: 17457 RVA: 0x00155B2C File Offset: 0x00153D2C
	public void SwitchToScene(GTScene scene)
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			GTScene gtscene = this.scenes[i];
			if (!(scene == gtscene))
			{
				gtscene.UnloadAsync();
			}
		}
		scene.LoadAsync();
	}

	// Token: 0x06004432 RID: 17458 RVA: 0x00155B6C File Offset: 0x00153D6C
	public void LoadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].LoadAsync();
		}
	}

	// Token: 0x06004433 RID: 17459 RVA: 0x00155B9C File Offset: 0x00153D9C
	public void UnloadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].UnloadAsync();
		}
	}

	// Token: 0x04004E73 RID: 20083
	[DragDropScenes]
	public GTScene[] scenes = new GTScene[0];
}
