using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EE6 RID: 3814
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x06005EA8 RID: 24232 RVA: 0x001DD392 File Offset: 0x001DB592
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x06005EA9 RID: 24233 RVA: 0x001DD3B8 File Offset: 0x001DB5B8
		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 localPosition = transform.parent.InverseTransformPoint(this.cameraXform.position);
			localPosition.y = 0f;
			if (this.limitDistance && localPosition.sqrMagnitude > this.maxDistance * this.maxDistance)
			{
				localPosition = localPosition.normalized * this.maxDistance;
			}
			transform.localPosition = localPosition;
		}

		// Token: 0x04006917 RID: 26903
		private Transform cameraXform;

		// Token: 0x04006918 RID: 26904
		private bool hasCamera;

		// Token: 0x04006919 RID: 26905
		[SerializeField]
		private bool limitDistance;

		// Token: 0x0400691A RID: 26906
		[SerializeField]
		private float maxDistance = 1f;
	}
}
