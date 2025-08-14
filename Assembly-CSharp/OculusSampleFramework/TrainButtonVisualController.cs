using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000D2D RID: 3373
	public class TrainButtonVisualController : MonoBehaviour
	{
		// Token: 0x06005379 RID: 21369 RVA: 0x0019D248 File Offset: 0x0019B448
		private void Awake()
		{
			this._materialColorId = Shader.PropertyToID("_Color");
			this._buttonMaterial = this._meshRenderer.material;
			this._buttonDefaultColor = this._buttonMaterial.GetColor(this._materialColorId);
			this._oldPosition = base.transform.localPosition;
		}

		// Token: 0x0600537A RID: 21370 RVA: 0x0019D29E File Offset: 0x0019B49E
		private void OnDestroy()
		{
			if (this._buttonMaterial != null)
			{
				Object.Destroy(this._buttonMaterial);
			}
		}

		// Token: 0x0600537B RID: 21371 RVA: 0x0019D2BC File Offset: 0x0019B4BC
		private void OnEnable()
		{
			this._buttonController.InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
			this._buttonController.ContactZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonController.ActionZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonInContactOrActionStates = false;
		}

		// Token: 0x0600537C RID: 21372 RVA: 0x0019D31C File Offset: 0x0019B51C
		private void OnDisable()
		{
			if (this._buttonController != null)
			{
				this._buttonController.InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
				this._buttonController.ContactZoneEvent -= this.ActionOrInContactZoneStayEvent;
				this._buttonController.ActionZoneEvent -= this.ActionOrInContactZoneStayEvent;
			}
		}

		// Token: 0x0600537D RID: 21373 RVA: 0x0019D384 File Offset: 0x0019B584
		private void ActionOrInContactZoneStayEvent(ColliderZoneArgs collisionArgs)
		{
			if (!this._buttonInContactOrActionStates || collisionArgs.CollidingTool.IsFarFieldTool)
			{
				return;
			}
			Vector3 localScale = this._buttonContactTransform.localScale;
			Vector3 interactionPosition = collisionArgs.CollidingTool.InteractionPosition;
			float num = (this._buttonContactTransform.InverseTransformPoint(interactionPosition) - 0.5f * Vector3.one).y * localScale.y;
			if (num > -this._contactMaxDisplacementDistance && num <= 0f)
			{
				base.transform.localPosition = new Vector3(this._oldPosition.x, this._oldPosition.y + num, this._oldPosition.z);
			}
		}

		// Token: 0x0600537E RID: 21374 RVA: 0x0019D434 File Offset: 0x0019B634
		private void InteractableStateChanged(InteractableStateArgs obj)
		{
			this._buttonInContactOrActionStates = false;
			this._glowRenderer.gameObject.SetActive(obj.NewInteractableState > InteractableState.Default);
			switch (obj.NewInteractableState)
			{
			case InteractableState.ProximityState:
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonDefaultColor);
				this.LerpToOldPosition();
				return;
			case InteractableState.ContactState:
				this.StopResetLerping();
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonContactColor);
				this._buttonInContactOrActionStates = true;
				return;
			case InteractableState.ActionState:
				this.StopResetLerping();
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonActionColor);
				this.PlaySound(this._actionSoundEffect);
				this._buttonInContactOrActionStates = true;
				return;
			default:
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonDefaultColor);
				this.LerpToOldPosition();
				return;
			}
		}

		// Token: 0x0600537F RID: 21375 RVA: 0x0019D50F File Offset: 0x0019B70F
		private void PlaySound(AudioClip clip)
		{
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x06005380 RID: 21376 RVA: 0x0019D534 File Offset: 0x0019B734
		private void StopResetLerping()
		{
			if (this._lerpToOldPositionCr != null)
			{
				base.StopCoroutine(this._lerpToOldPositionCr);
			}
		}

		// Token: 0x06005381 RID: 21377 RVA: 0x0019D54C File Offset: 0x0019B74C
		private void LerpToOldPosition()
		{
			if ((base.transform.localPosition - this._oldPosition).sqrMagnitude < Mathf.Epsilon)
			{
				return;
			}
			this.StopResetLerping();
			this._lerpToOldPositionCr = base.StartCoroutine(this.ResetPosition());
		}

		// Token: 0x06005382 RID: 21378 RVA: 0x0019D597 File Offset: 0x0019B797
		private IEnumerator ResetPosition()
		{
			float startTime = Time.time;
			float endTime = Time.time + 1f;
			while (Time.time < endTime)
			{
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, this._oldPosition, (Time.time - startTime) / 1f);
				yield return null;
			}
			base.transform.localPosition = this._oldPosition;
			this._lerpToOldPositionCr = null;
			yield break;
		}

		// Token: 0x04005CBF RID: 23743
		private const float LERP_TO_OLD_POS_DURATION = 1f;

		// Token: 0x04005CC0 RID: 23744
		private const float LOCAL_SIZE_HALVED = 0.5f;

		// Token: 0x04005CC1 RID: 23745
		[SerializeField]
		private MeshRenderer _meshRenderer;

		// Token: 0x04005CC2 RID: 23746
		[SerializeField]
		private MeshRenderer _glowRenderer;

		// Token: 0x04005CC3 RID: 23747
		[SerializeField]
		private ButtonController _buttonController;

		// Token: 0x04005CC4 RID: 23748
		[SerializeField]
		private Color _buttonContactColor = new Color(0.51f, 0.78f, 0.92f, 1f);

		// Token: 0x04005CC5 RID: 23749
		[SerializeField]
		private Color _buttonActionColor = new Color(0.24f, 0.72f, 0.98f, 1f);

		// Token: 0x04005CC6 RID: 23750
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04005CC7 RID: 23751
		[SerializeField]
		private AudioClip _actionSoundEffect;

		// Token: 0x04005CC8 RID: 23752
		[SerializeField]
		private Transform _buttonContactTransform;

		// Token: 0x04005CC9 RID: 23753
		[SerializeField]
		private float _contactMaxDisplacementDistance = 0.0141f;

		// Token: 0x04005CCA RID: 23754
		private Material _buttonMaterial;

		// Token: 0x04005CCB RID: 23755
		private Color _buttonDefaultColor;

		// Token: 0x04005CCC RID: 23756
		private int _materialColorId;

		// Token: 0x04005CCD RID: 23757
		private bool _buttonInContactOrActionStates;

		// Token: 0x04005CCE RID: 23758
		private Coroutine _lerpToOldPositionCr;

		// Token: 0x04005CCF RID: 23759
		private Vector3 _oldPosition;
	}
}
