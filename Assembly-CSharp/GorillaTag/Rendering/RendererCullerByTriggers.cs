using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000EF4 RID: 3828
	public class RendererCullerByTriggers : MonoBehaviour, IBuildValidation
	{
		// Token: 0x06005EF5 RID: 24309 RVA: 0x001DED98 File Offset: 0x001DCF98
		protected void OnEnable()
		{
			this.camWasTouching = false;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
		}

		// Token: 0x06005EF6 RID: 24310 RVA: 0x001DEDF4 File Offset: 0x001DCFF4
		protected void LateUpdate()
		{
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
			Vector3 position = this.mainCameraTransform.position;
			bool flag = false;
			foreach (Collider collider in this.colliders)
			{
				if (!(collider == null) && (collider.ClosestPoint(position) - position).sqrMagnitude < 0.010000001f)
				{
					flag = true;
					break;
				}
			}
			if (this.camWasTouching == flag)
			{
				return;
			}
			this.camWasTouching = flag;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = flag;
				}
			}
		}

		// Token: 0x06005EF7 RID: 24311 RVA: 0x001DEEB4 File Offset: 0x001DD0B4
		public bool BuildValidationCheck()
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				if (this.renderers[i] == null)
				{
					Debug.LogError("rendererculllerbytriggers has null renderer", base.gameObject);
					return false;
				}
			}
			for (int j = 0; j < this.colliders.Length; j++)
			{
				if (this.colliders[j] == null)
				{
					Debug.LogError("rendererculllerbytriggers has null collider", base.gameObject);
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400695C RID: 26972
		[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
		public Renderer[] renderers;

		// Token: 0x0400695D RID: 26973
		public Collider[] colliders;

		// Token: 0x0400695E RID: 26974
		private bool camWasTouching;

		// Token: 0x0400695F RID: 26975
		private const float cameraRadiusSq = 0.010000001f;

		// Token: 0x04006960 RID: 26976
		private Transform mainCameraTransform;
	}
}
