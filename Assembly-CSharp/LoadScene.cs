using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200002A RID: 42
public class LoadScene : MonoBehaviour
{
	// Token: 0x06000094 RID: 148 RVA: 0x00004D86 File Offset: 0x00002F86
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this._sceneName, LoadSceneMode.Single);
		while (asyncOperation.progress < 0.99f)
		{
			yield return null;
		}
		asyncOperation.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x040000AF RID: 175
	[SerializeField]
	private float _delay;

	// Token: 0x040000B0 RID: 176
	[SerializeField]
	private string _sceneName;
}
