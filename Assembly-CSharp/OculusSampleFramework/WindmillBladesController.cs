using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D38 RID: 3384
	public class WindmillBladesController : MonoBehaviour
	{
		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060053C6 RID: 21446 RVA: 0x0019E49A File Offset: 0x0019C69A
		// (set) Token: 0x060053C7 RID: 21447 RVA: 0x0019E4A2 File Offset: 0x0019C6A2
		public bool IsMoving { get; private set; }

		// Token: 0x060053C8 RID: 21448 RVA: 0x0019E4AB File Offset: 0x0019C6AB
		private void Start()
		{
			this._originalRotation = base.transform.localRotation;
		}

		// Token: 0x060053C9 RID: 21449 RVA: 0x0019E4C0 File Offset: 0x0019C6C0
		private void Update()
		{
			this._rotAngle += this._currentSpeed * Time.deltaTime;
			if (this._rotAngle > 360f)
			{
				this._rotAngle = 0f;
			}
			base.transform.localRotation = this._originalRotation * Quaternion.AngleAxis(this._rotAngle, Vector3.forward);
		}

		// Token: 0x060053CA RID: 21450 RVA: 0x0019E524 File Offset: 0x0019C724
		public void SetMoveState(bool newMoveState, float goalSpeed)
		{
			this.IsMoving = newMoveState;
			if (this._lerpSpeedCoroutine != null)
			{
				base.StopCoroutine(this._lerpSpeedCoroutine);
			}
			this._lerpSpeedCoroutine = base.StartCoroutine(this.LerpToSpeed(goalSpeed));
		}

		// Token: 0x060053CB RID: 21451 RVA: 0x0019E554 File Offset: 0x0019C754
		private IEnumerator LerpToSpeed(float goalSpeed)
		{
			float totalTime = 0f;
			float startSpeed = this._currentSpeed;
			if (this._audioChangeCr != null)
			{
				base.StopCoroutine(this._audioChangeCr);
			}
			if (this.IsMoving)
			{
				this._audioChangeCr = base.StartCoroutine(this.PlaySoundDelayed(this._windMillStartSound, this._windMillRotationSound, this._windMillStartSound.length * 0.95f));
			}
			else
			{
				this.PlaySound(this._windMillStopSound, false);
			}
			for (float num = Mathf.Abs(this._currentSpeed - goalSpeed); num > Mathf.Epsilon; num = Mathf.Abs(this._currentSpeed - goalSpeed))
			{
				this._currentSpeed = Mathf.Lerp(startSpeed, goalSpeed, totalTime / 1f);
				totalTime += Time.deltaTime;
				yield return null;
			}
			this._lerpSpeedCoroutine = null;
			yield break;
		}

		// Token: 0x060053CC RID: 21452 RVA: 0x0019E56A File Offset: 0x0019C76A
		private IEnumerator PlaySoundDelayed(AudioClip initial, AudioClip clip, float timeDelayAfterInitial)
		{
			this.PlaySound(initial, false);
			yield return new WaitForSeconds(timeDelayAfterInitial);
			this.PlaySound(clip, true);
			yield break;
		}

		// Token: 0x060053CD RID: 21453 RVA: 0x0019E58E File Offset: 0x0019C78E
		private void PlaySound(AudioClip clip, bool loop = false)
		{
			this._audioSource.loop = loop;
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x04005D2B RID: 23851
		private const float MAX_TIME = 1f;

		// Token: 0x04005D2C RID: 23852
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04005D2D RID: 23853
		[SerializeField]
		private AudioClip _windMillRotationSound;

		// Token: 0x04005D2E RID: 23854
		[SerializeField]
		private AudioClip _windMillStartSound;

		// Token: 0x04005D2F RID: 23855
		[SerializeField]
		private AudioClip _windMillStopSound;

		// Token: 0x04005D31 RID: 23857
		private float _currentSpeed;

		// Token: 0x04005D32 RID: 23858
		private Coroutine _lerpSpeedCoroutine;

		// Token: 0x04005D33 RID: 23859
		private Coroutine _audioChangeCr;

		// Token: 0x04005D34 RID: 23860
		private Quaternion _originalRotation;

		// Token: 0x04005D35 RID: 23861
		private float _rotAngle;
	}
}
