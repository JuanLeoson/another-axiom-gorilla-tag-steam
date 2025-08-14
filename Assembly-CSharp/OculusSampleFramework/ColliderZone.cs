using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D09 RID: 3337
	public interface ColliderZone
	{
		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x0600528F RID: 21135
		Collider Collider { get; }

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x06005290 RID: 21136
		Interactable ParentInteractable { get; }

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06005291 RID: 21137
		InteractableCollisionDepth CollisionDepth { get; }
	}
}
