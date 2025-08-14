using System;
using UnityEngine;

// Token: 0x020004DB RID: 1243
public class GorillaSceneCamera : MonoBehaviour
{
	// Token: 0x06001E67 RID: 7783 RVA: 0x000A1488 File Offset: 0x0009F688
	public void SetSceneCamera(int sceneIndex)
	{
		base.transform.position = this.sceneTransforms[sceneIndex].scenePosition;
		base.transform.eulerAngles = this.sceneTransforms[sceneIndex].sceneRotation;
	}

	// Token: 0x04002716 RID: 10006
	public GorillaSceneTransform[] sceneTransforms;
}
