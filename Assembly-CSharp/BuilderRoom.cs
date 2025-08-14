using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054F RID: 1359
public class BuilderRoom : MonoBehaviour
{
	// Token: 0x04002A72 RID: 10866
	public List<GameObject> disableColliderRoots;

	// Token: 0x04002A73 RID: 10867
	public List<GameObject> disableRenderRoots;

	// Token: 0x04002A74 RID: 10868
	public List<GameObject> disableGameObjectsForScene;

	// Token: 0x04002A75 RID: 10869
	public List<GameObject> disableObjectsForPersistent;

	// Token: 0x04002A76 RID: 10870
	public List<MeshRenderer> disabledRenderersForPersistent;

	// Token: 0x04002A77 RID: 10871
	public List<Collider> disabledCollidersForScene;
}
