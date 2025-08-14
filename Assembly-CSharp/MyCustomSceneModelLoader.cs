using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public class MyCustomSceneModelLoader : OVRSceneModelLoader
{
	// Token: 0x060015A5 RID: 5541 RVA: 0x000766F5 File Offset: 0x000748F5
	private IEnumerator DelayedLoad()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("[MyCustomSceneLoader] calling OVRSceneManager.LoadSceneModel() delayed by 1 second");
		base.SceneManager.LoadSceneModel();
		yield break;
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x00076704 File Offset: 0x00074904
	protected override void OnStart()
	{
		base.StartCoroutine(this.DelayedLoad());
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x00076713 File Offset: 0x00074913
	protected override void OnNoSceneModelToLoad()
	{
		Debug.Log("[MyCustomSceneLoader] There is no scene to load, but we don't want to trigger scene capture.");
	}
}
