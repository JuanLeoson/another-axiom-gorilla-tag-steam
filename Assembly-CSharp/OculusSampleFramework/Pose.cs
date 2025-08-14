using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D37 RID: 3383
	public class Pose
	{
		// Token: 0x060053C4 RID: 21444 RVA: 0x0019E466 File Offset: 0x0019C666
		public Pose()
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x0019E484 File Offset: 0x0019C684
		public Pose(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		// Token: 0x04005D29 RID: 23849
		public Vector3 Position;

		// Token: 0x04005D2A RID: 23850
		public Quaternion Rotation;
	}
}
