using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	// Token: 0x02000CF6 RID: 3318
	internal class SceneSnapshot
	{
		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06005239 RID: 21049 RVA: 0x001987C2 File Offset: 0x001969C2
		public Dictionary<OVRAnchor, SceneSnapshot.Data> Anchors { get; } = new Dictionary<OVRAnchor, SceneSnapshot.Data>();

		// Token: 0x0600523A RID: 21050 RVA: 0x001987CA File Offset: 0x001969CA
		public bool Contains(OVRAnchor anchor)
		{
			return this.Anchors.ContainsKey(anchor);
		}

		// Token: 0x02000CF7 RID: 3319
		public class Data
		{
			// Token: 0x04005BAD RID: 23469
			public List<OVRAnchor> Children;

			// Token: 0x04005BAE RID: 23470
			public Rect? Rect;

			// Token: 0x04005BAF RID: 23471
			public Bounds? Bounds;
		}
	}
}
