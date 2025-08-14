using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C3F RID: 3135
	public class GTSignalTest : GTSignalListener
	{
		// Token: 0x0400566C RID: 22124
		public MeshRenderer[] targets = new MeshRenderer[0];

		// Token: 0x0400566D RID: 22125
		[Space]
		public MeshRenderer target;

		// Token: 0x0400566E RID: 22126
		public List<GTSignalListener> listeners = new List<GTSignalListener>(12);
	}
}
