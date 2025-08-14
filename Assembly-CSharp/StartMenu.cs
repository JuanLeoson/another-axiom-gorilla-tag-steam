using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003B7 RID: 951
public class StartMenu : MonoBehaviour
{
	// Token: 0x06001610 RID: 5648 RVA: 0x0007834C File Offset: 0x0007654C
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Select Sample Scene", 0);
		int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
		for (int i = 0; i < sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			int sceneIndex = i;
			DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(scenePathByBuildIndex), delegate
			{
				this.LoadScene(sceneIndex);
			}, -1, 0, false);
		}
		DebugUIBuilder.instance.Show();
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x000783C1 File Offset: 0x000765C1
	private void LoadScene(int idx)
	{
		DebugUIBuilder.instance.Hide();
		Debug.Log("Load scene: " + idx.ToString());
		SceneManager.LoadScene(idx);
	}

	// Token: 0x04001DC8 RID: 7624
	public OVROverlay overlay;

	// Token: 0x04001DC9 RID: 7625
	public OVROverlay text;

	// Token: 0x04001DCA RID: 7626
	public OVRCameraRig vrRig;
}
