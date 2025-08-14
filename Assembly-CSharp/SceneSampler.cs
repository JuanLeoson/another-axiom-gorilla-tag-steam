using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200037D RID: 893
public class SceneSampler : MonoBehaviour
{
	// Token: 0x06001518 RID: 5400 RVA: 0x00072F04 File Offset: 0x00071104
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x00072F14 File Offset: 0x00071114
	private void Update()
	{
		bool active = OVRInput.GetActiveController() == OVRInput.Controller.Touch || OVRInput.GetActiveController() == OVRInput.Controller.LTouch || OVRInput.GetActiveController() == OVRInput.Controller.RTouch;
		this.displayText.SetActive(active);
		if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			this.currentSceneIndex++;
			if (this.currentSceneIndex >= SceneManager.sceneCountInBuildSettings)
			{
				this.currentSceneIndex = 0;
			}
			SceneManager.LoadScene(this.currentSceneIndex);
		}
		Vector3 vector = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + Vector3.up * 0.09f;
		this.displayText.transform.position = vector;
		this.displayText.transform.rotation = Quaternion.LookRotation(vector - Camera.main.transform.position);
	}

	// Token: 0x04001CB3 RID: 7347
	private int currentSceneIndex;

	// Token: 0x04001CB4 RID: 7348
	public GameObject displayText;
}
