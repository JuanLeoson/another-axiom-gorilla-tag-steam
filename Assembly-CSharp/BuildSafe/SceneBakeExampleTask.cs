using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x02000CEE RID: 3310
	public class SceneBakeExampleTask : SceneBakeTask
	{
		// Token: 0x0600521B RID: 21019 RVA: 0x001986A8 File Offset: 0x001968A8
		public override void OnSceneBake(Scene scene, SceneBakeMode mode)
		{
			for (int i = 0; i < 10; i++)
			{
				SceneBakeExampleTask.DuplicateAndRecolor(base.gameObject);
			}
			if (mode != SceneBakeMode.OnBuildPlayer)
			{
			}
		}

		// Token: 0x0600521C RID: 21020 RVA: 0x001986D8 File Offset: 0x001968D8
		private static void DuplicateAndRecolor(GameObject target)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(target);
			gameObject.transform.position = Random.insideUnitSphere * 4f;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material = new Material(component.sharedMaterial)
			{
				color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f)
			};
		}
	}
}
