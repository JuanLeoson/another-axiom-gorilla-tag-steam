using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000E72 RID: 3698
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x06005C72 RID: 23666 RVA: 0x001D14A0 File Offset: 0x001CF6A0
		private void Start()
		{
			if (!this.dayNightManager)
			{
				this.dayNightManager = BetterDayNightManager.instance;
			}
			this.rotationDegree = 0f;
			if (this.clockNeedle)
			{
				this.initialRotation = this.clockNeedle.localRotation;
			}
		}

		// Token: 0x06005C73 RID: 23667 RVA: 0x001D14F0 File Offset: 0x001CF6F0
		private void Update()
		{
			this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
			double currentTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).currentTimeInSeconds;
			double totalTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).totalTimeInSeconds;
			this.rotationDegree = (float)(360.0 * currentTimeInSeconds / totalTimeInSeconds);
			this.rotationDegree = Mathf.Floor(this.rotationDegree);
			if (this.clockNeedle)
			{
				this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
			}
		}

		// Token: 0x0400661E RID: 26142
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x0400661F RID: 26143
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x04006620 RID: 26144
		private BetterDayNightManager dayNightManager;

		// Token: 0x04006621 RID: 26145
		[DebugOption]
		private float rotationDegree;

		// Token: 0x04006622 RID: 26146
		private string currentTimeOfDay;

		// Token: 0x04006623 RID: 26147
		private Quaternion initialRotation;
	}
}
