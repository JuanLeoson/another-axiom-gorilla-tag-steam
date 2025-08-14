using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FF4 RID: 4084
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06006605 RID: 26117 RVA: 0x0020795C File Offset: 0x00205B5C
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x04007105 RID: 28933
		public BoingWork.Params Params;
	}
}
