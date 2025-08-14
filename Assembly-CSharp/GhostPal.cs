using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class GhostPal : MonoBehaviour
{
	// Token: 0x06000469 RID: 1129 RVA: 0x00019A7C File Offset: 0x00017C7C
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.trailingPosition = base.transform.position;
		this.triggerAudioClipIndex = this.triggerAudioClips.GetRandomIndex<AudioClip>();
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00019AB8 File Offset: 0x00017CB8
	private IEnumerator BounceOnTrigger()
	{
		float startTime = Time.time;
		while (Time.time - startTime < this.bounceOnTrigger[this.bounceOnTrigger.length - 1].time)
		{
			this.bounceHeight = this.bounceOnTrigger.Evaluate(Time.time - startTime);
			yield return null;
		}
		this.bounceHeight = 0f;
		yield break;
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00019AC8 File Offset: 0x00017CC8
	private void LateUpdate()
	{
		Vector3 position = this.rig.bodyTransform.position;
		Vector3 vector = base.transform.parent.position - position;
		float num = vector.y * 0.5f + this.orbitHeight;
		vector.y = 0f;
		float d = vector.magnitude + this.minDistanceFromPlayer;
		vector = vector.normalized * d;
		vector.y = num + this.bounceHeight;
		double num2 = (double)this.orbitSpeed * (PhotonNetwork.InRoom ? ((PhotonNetwork.Time - (double)this.rig.OwningNetPlayer.UserId.GetStaticHash()) * (double)((this.rig.OwningNetPlayer.ActorNumber % 2 == 0) ? 1 : -1)) : Time.timeAsDouble);
		Vector3 b = new Vector3(this.orbitRadius * (float)Math.Cos(num2), 0f, this.orbitRadius * (float)Math.Sin(num2));
		Vector3 vector2 = position + vector + b;
		Vector3 a = vector2 - this.rig.head.rigTarget.position;
		if (Vector3.Dot(this.rig.head.rigTarget.forward, a.normalized) >= this.lookAtDotProductMin)
		{
			this.lookAtTime = Mathf.Min(this.lookAtTime + Time.deltaTime, Mathf.Max(this.rotateTowardsPlayerFromLookTime[this.rotateTowardsPlayerFromLookTime.length - 1].time, this.minLookTimeToTrigger));
			if (this.lookAtTime >= this.minLookTimeToTrigger && !this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.friendlyAnimID);
				this.bounceCoroutine = base.StartCoroutine(this.BounceOnTrigger());
				this.triggerAudioSource.pitch = Random.Range(this.triggerAudioPitchMinMax.x, this.triggerAudioPitchMinMax.y);
				this.triggerAudioSource.clip = this.triggerAudioClips[this.triggerAudioClipIndex];
				this.triggerAudioSource.GTPlay();
				this.triggerAudioClipIndex = (this.triggerAudioClipIndex + Random.Range(0, this.triggerAudioClips.Length - 1)) % this.triggerAudioClips.Length;
				this.hasTriggered = true;
			}
		}
		else
		{
			this.lookAtTime = Mathf.Max(this.lookAtTime - Time.deltaTime, 0f);
			if (this.lookAtTime < this.minLookTimeToTrigger && this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.neutralAnimID);
				this.hasTriggered = false;
			}
		}
		if ((vector2 - this.trailingPosition).sqrMagnitude > 0.1f)
		{
			float t = 1f - Mathf.Exp(-this.faceMovementDirectionStrength * Time.deltaTime);
			this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector2, t);
		}
		Quaternion rotation = Quaternion.Slerp(Quaternion.LookRotation(vector2 - this.trailingPosition, Vector3.up), Quaternion.LookRotation(-a, Vector3.up), this.rotateTowardsPlayerFromLookTime.Evaluate(this.lookAtTime));
		base.transform.SetPositionAndRotation(vector2, rotation);
	}

	// Token: 0x04000520 RID: 1312
	[SerializeField]
	private float minDistanceFromPlayer = 1f;

	// Token: 0x04000521 RID: 1313
	[SerializeField]
	private float orbitRadius = 1f;

	// Token: 0x04000522 RID: 1314
	[SerializeField]
	private float orbitHeight = 1f;

	// Token: 0x04000523 RID: 1315
	[SerializeField]
	private float orbitSpeed = 0.1f;

	// Token: 0x04000524 RID: 1316
	[SerializeField]
	private float faceMovementDirectionStrength = 1f;

	// Token: 0x04000525 RID: 1317
	[Space]
	[SerializeField]
	private float lookAtDotProductMin = 0.95f;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	private AnimationCurve rotateTowardsPlayerFromLookTime;

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	private float minLookTimeToTrigger = 2f;

	// Token: 0x04000528 RID: 1320
	[SerializeField]
	private AnimationCurve bounceOnTrigger;

	// Token: 0x04000529 RID: 1321
	[SerializeField]
	private AudioSource triggerAudioSource;

	// Token: 0x0400052A RID: 1322
	[SerializeField]
	private Vector2 triggerAudioPitchMinMax = new Vector2(0.9f, 1.1f);

	// Token: 0x0400052B RID: 1323
	[SerializeField]
	private AudioClip[] triggerAudioClips;

	// Token: 0x0400052C RID: 1324
	private VRRig rig;

	// Token: 0x0400052D RID: 1325
	private Animator animator;

	// Token: 0x0400052E RID: 1326
	private float lookAtTime;

	// Token: 0x0400052F RID: 1327
	private bool hasTriggered;

	// Token: 0x04000530 RID: 1328
	private Coroutine bounceCoroutine;

	// Token: 0x04000531 RID: 1329
	private float bounceHeight;

	// Token: 0x04000532 RID: 1330
	private Vector3 trailingPosition;

	// Token: 0x04000533 RID: 1331
	private int triggerAudioClipIndex;

	// Token: 0x04000534 RID: 1332
	private int neutralAnimID = Animator.StringToHash("Neutral");

	// Token: 0x04000535 RID: 1333
	private int friendlyAnimID = Animator.StringToHash("Friendly");
}
