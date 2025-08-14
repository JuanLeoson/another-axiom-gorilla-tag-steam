using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D27 RID: 3367
	public class PanelHMDFollower : MonoBehaviour
	{
		// Token: 0x06005355 RID: 21333 RVA: 0x0019C776 File Offset: 0x0019A976
		private void Awake()
		{
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			this._panelInitialPosition = base.transform.position;
		}

		// Token: 0x06005356 RID: 21334 RVA: 0x0019C794 File Offset: 0x0019A994
		private void Update()
		{
			Vector3 position = this._cameraRig.centerEyeAnchor.position;
			Vector3 position2 = base.transform.position;
			float num = Vector3.Distance(position, this._lastMovedToPos);
			float num2 = (this._cameraRig.centerEyeAnchor.position - this._prevPos).magnitude / Time.deltaTime;
			Vector3 vector = base.transform.position - position;
			float magnitude = vector.magnitude;
			if ((num > this._maxDistance || this._minZDistance > vector.z || this._minDistance > magnitude) && num2 < 0.3f && this._coroutine == null && this._coroutine == null)
			{
				this._coroutine = base.StartCoroutine(this.LerpToHMD());
			}
			this._prevPos = this._cameraRig.centerEyeAnchor.position;
		}

		// Token: 0x06005357 RID: 21335 RVA: 0x0019C86E File Offset: 0x0019AA6E
		private Vector3 CalculateIdealAnchorPosition()
		{
			return this._cameraRig.centerEyeAnchor.position + this._panelInitialPosition;
		}

		// Token: 0x06005358 RID: 21336 RVA: 0x0019C88B File Offset: 0x0019AA8B
		private IEnumerator LerpToHMD()
		{
			Vector3 newPanelPosition = this.CalculateIdealAnchorPosition();
			this._lastMovedToPos = this._cameraRig.centerEyeAnchor.position;
			float startTime = Time.time;
			float endTime = Time.time + 3f;
			while (Time.time < endTime)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, newPanelPosition, (Time.time - startTime) / 3f);
				yield return null;
			}
			base.transform.position = newPanelPosition;
			this._coroutine = null;
			yield break;
		}

		// Token: 0x04005C93 RID: 23699
		private const float TOTAL_DURATION = 3f;

		// Token: 0x04005C94 RID: 23700
		private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

		// Token: 0x04005C95 RID: 23701
		[SerializeField]
		private float _maxDistance = 0.3f;

		// Token: 0x04005C96 RID: 23702
		[SerializeField]
		private float _minDistance = 0.05f;

		// Token: 0x04005C97 RID: 23703
		[SerializeField]
		private float _minZDistance = 0.05f;

		// Token: 0x04005C98 RID: 23704
		private OVRCameraRig _cameraRig;

		// Token: 0x04005C99 RID: 23705
		private Vector3 _panelInitialPosition = Vector3.zero;

		// Token: 0x04005C9A RID: 23706
		private Coroutine _coroutine;

		// Token: 0x04005C9B RID: 23707
		private Vector3 _prevPos = Vector3.zero;

		// Token: 0x04005C9C RID: 23708
		private Vector3 _lastMovedToPos = Vector3.zero;
	}
}
