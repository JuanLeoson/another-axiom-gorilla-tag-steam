using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000CF2 RID: 3314
	public static class SceneViewUtils
	{
		// Token: 0x0600522B RID: 21035 RVA: 0x00198799 File Offset: 0x00196999
		private static bool RaycastWorldSafe(Vector2 screenPos, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x04005BAA RID: 23466
		public static readonly SceneViewUtils.FuncRaycastWorld RaycastWorld = new SceneViewUtils.FuncRaycastWorld(SceneViewUtils.RaycastWorldSafe);

		// Token: 0x02000CF3 RID: 3315
		// (Invoke) Token: 0x0600522E RID: 21038
		public delegate bool FuncRaycastWorld(Vector2 screenPos, out RaycastHit hit);

		// Token: 0x02000CF4 RID: 3316
		// (Invoke) Token: 0x06005232 RID: 21042
		public delegate GameObject FuncPickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
	}
}
