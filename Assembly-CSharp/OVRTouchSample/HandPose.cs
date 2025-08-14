using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000D42 RID: 3394
	public class HandPose : MonoBehaviour
	{
		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x06005406 RID: 21510 RVA: 0x0019F5DE File Offset: 0x0019D7DE
		public bool AllowPointing
		{
			get
			{
				return this.m_allowPointing;
			}
		}

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x06005407 RID: 21511 RVA: 0x0019F5E6 File Offset: 0x0019D7E6
		public bool AllowThumbsUp
		{
			get
			{
				return this.m_allowThumbsUp;
			}
		}

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x06005408 RID: 21512 RVA: 0x0019F5EE File Offset: 0x0019D7EE
		public HandPoseId PoseId
		{
			get
			{
				return this.m_poseId;
			}
		}

		// Token: 0x04005D8A RID: 23946
		[SerializeField]
		private bool m_allowPointing;

		// Token: 0x04005D8B RID: 23947
		[SerializeField]
		private bool m_allowThumbsUp;

		// Token: 0x04005D8C RID: 23948
		[SerializeField]
		private HandPoseId m_poseId;
	}
}
