using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F0C RID: 3852
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x06005F61 RID: 24417 RVA: 0x001E1AAA File Offset: 0x001DFCAA
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x06005F62 RID: 24418 RVA: 0x001E1AC8 File Offset: 0x001DFCC8
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float angle = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, angle, Space.World);
		}

		// Token: 0x04006A15 RID: 27157
		private const float smoothTime = 0.005f;

		// Token: 0x04006A16 RID: 27158
		private float currentVelocity;
	}
}
