using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020003C0 RID: 960
public class UiSceneMenu : MonoBehaviour
{
	// Token: 0x06001630 RID: 5680 RVA: 0x00078D20 File Offset: 0x00076F20
	private void Awake()
	{
		this.m_activeScene = SceneManager.GetActiveScene();
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			this.CreateLabel(i, scenePathByBuildIndex);
		}
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x00078D58 File Offset: 0x00076F58
	private void Update()
	{
		int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
		if (this.InputPrevScene())
		{
			this.ChangeScene((this.m_activeScene.buildIndex - 1 + sceneCountInBuildSettings) % sceneCountInBuildSettings);
		}
		else if (this.InputNextScene())
		{
			this.ChangeScene((this.m_activeScene.buildIndex + 1) % sceneCountInBuildSettings);
		}
		UiSceneMenu.s_lastThumbstickL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
		UiSceneMenu.s_lastThumbstickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x00078DC1 File Offset: 0x00076FC1
	private bool InputPrevScene()
	{
		return this.KeyboardPrevScene() || this.ThumbstickPrevScene(OVRInput.Controller.LTouch) || this.ThumbstickPrevScene(OVRInput.Controller.RTouch);
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00078DDD File Offset: 0x00076FDD
	private bool InputNextScene()
	{
		return this.KeyboardNextScene() || this.ThumbstickNextScene(OVRInput.Controller.LTouch) || this.ThumbstickNextScene(OVRInput.Controller.RTouch);
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00078DF9 File Offset: 0x00076FF9
	private bool KeyboardPrevScene()
	{
		return Input.GetKeyDown(KeyCode.UpArrow);
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00078E05 File Offset: 0x00077005
	private bool KeyboardNextScene()
	{
		return Input.GetKeyDown(KeyCode.DownArrow);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00078E11 File Offset: 0x00077011
	private bool ThumbstickPrevScene(OVRInput.Controller controller)
	{
		return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y >= 0.9f && this.GetLastThumbstickValue(controller).y < 0.9f;
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x00078E3B File Offset: 0x0007703B
	private bool ThumbstickNextScene(OVRInput.Controller controller)
	{
		return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y <= -0.9f && this.GetLastThumbstickValue(controller).y > -0.9f;
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x00078E65 File Offset: 0x00077065
	private Vector2 GetLastThumbstickValue(OVRInput.Controller controller)
	{
		if (controller != OVRInput.Controller.LTouch)
		{
			return UiSceneMenu.s_lastThumbstickR;
		}
		return UiSceneMenu.s_lastThumbstickL;
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00078E76 File Offset: 0x00077076
	private void ChangeScene(int nextScene)
	{
		SceneManager.LoadScene(nextScene);
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00078E80 File Offset: 0x00077080
	private void CreateLabel(int sceneIndex, string scenePath)
	{
		string text = Path.GetFileNameWithoutExtension(scenePath);
		text = Regex.Replace(text, "[A-Z]", " $0").Trim();
		if (this.m_activeScene.buildIndex == sceneIndex)
		{
			text = "Open: " + text;
		}
		TextMeshProUGUI textMeshProUGUI = Object.Instantiate<TextMeshProUGUI>(this.m_labelPf);
		textMeshProUGUI.SetText(string.Format("{0}. {1}", sceneIndex + 1, text), true);
		textMeshProUGUI.transform.SetParent(this.m_layoutGroup.transform, false);
	}

	// Token: 0x04001DFA RID: 7674
	[Header("Settings")]
	[SerializeField]
	private VerticalLayoutGroup m_layoutGroup;

	// Token: 0x04001DFB RID: 7675
	[SerializeField]
	private TextMeshProUGUI m_labelPf;

	// Token: 0x04001DFC RID: 7676
	private static Vector2 s_lastThumbstickL;

	// Token: 0x04001DFD RID: 7677
	private static Vector2 s_lastThumbstickR;

	// Token: 0x04001DFE RID: 7678
	private Scene m_activeScene;
}
