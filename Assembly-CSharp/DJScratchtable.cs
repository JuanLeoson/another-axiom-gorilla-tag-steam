using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class DJScratchtable : MonoBehaviour
{
	// Token: 0x06000A18 RID: 2584 RVA: 0x000371FB File Offset: 0x000353FB
	public void SetPlaying(bool playing)
	{
		this.isPlaying = playing;
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00037204 File Offset: 0x00035404
	private void OnTriggerStay(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = (base.transform.parent.InverseTransformPoint(collider.transform.position) - base.transform.localPosition).WithY(0f);
		float target = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		if (this.isTouching)
		{
			base.transform.localRotation = Quaternion.LookRotation(vector) * this.firstTouchRotation;
			if (this.isPlaying)
			{
				float num = Mathf.DeltaAngle(this.lastScratchSoundAngle, target);
				if (num > this.scratchMinAngle)
				{
					if (Time.time > this.cantForwardScratchUntilTimestamp)
					{
						this.scratchPlayer.Play(ScratchSoundType.Forward, this.isLeft);
						this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
						this.lastScratchSoundAngle = target;
						GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
					}
				}
				else if (num < -this.scratchMinAngle && Time.time > this.cantBackScratchUntilTimestamp)
				{
					this.scratchPlayer.Play(ScratchSoundType.Back, this.isLeft);
					this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
					this.lastScratchSoundAngle = target;
					GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
				}
			}
		}
		else
		{
			this.firstTouchRotation = Quaternion.Inverse(Quaternion.LookRotation(base.transform.InverseTransformPoint(collider.transform.position).WithY(0f)));
			if (this.isPlaying)
			{
				this.PauseTrack();
				this.scratchPlayer.Play(ScratchSoundType.Pause, this.isLeft);
				this.lastScratchSoundAngle = target;
				this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
				this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
			}
		}
		this.isTouching = true;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00037408 File Offset: 0x00035608
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		if (this.isPlaying)
		{
			this.ResumeTrack();
			this.scratchPlayer.Play(ScratchSoundType.Resume, this.isLeft);
		}
		this.isTouching = false;
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00037454 File Offset: 0x00035654
	public void SelectTrack(int track)
	{
		this.lastSelectedTrack = track;
		if (track == 0)
		{
			this.turntableVisual.Stop();
			this.isPlaying = false;
		}
		else
		{
			this.turntableVisual.Run();
			this.isPlaying = true;
		}
		int num = track - 1;
		for (int i = 0; i < this.tracks.Length; i++)
		{
			if (num == i)
			{
				float time = (float)(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) % this.trackDuration;
				this.tracks[i].Play();
				this.tracks[i].time = time;
			}
			else
			{
				this.tracks[i].Stop();
			}
		}
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x000374F4 File Offset: 0x000356F4
	public void PauseTrack()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].Stop();
		}
		this.pausedUntilTimestamp = Time.time + 1f;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00037532 File Offset: 0x00035732
	public void ResumeTrack()
	{
		this.SelectTrack(this.lastSelectedTrack);
		this.pausedUntilTimestamp = 0f;
	}

	// Token: 0x04000C2A RID: 3114
	[SerializeField]
	private bool isLeft;

	// Token: 0x04000C2B RID: 3115
	[SerializeField]
	private DJScratchSoundPlayer scratchPlayer;

	// Token: 0x04000C2C RID: 3116
	[SerializeField]
	private float scratchCooldown;

	// Token: 0x04000C2D RID: 3117
	[SerializeField]
	private float scratchMinAngle;

	// Token: 0x04000C2E RID: 3118
	[SerializeField]
	private AudioSource[] tracks;

	// Token: 0x04000C2F RID: 3119
	[SerializeField]
	private CosmeticFan turntableVisual;

	// Token: 0x04000C30 RID: 3120
	[SerializeField]
	private float trackDuration;

	// Token: 0x04000C31 RID: 3121
	[SerializeField]
	private float hapticStrength;

	// Token: 0x04000C32 RID: 3122
	[SerializeField]
	private float hapticDuration;

	// Token: 0x04000C33 RID: 3123
	private int lastSelectedTrack;

	// Token: 0x04000C34 RID: 3124
	private bool isPlaying;

	// Token: 0x04000C35 RID: 3125
	private bool isTouching;

	// Token: 0x04000C36 RID: 3126
	private Quaternion firstTouchRotation;

	// Token: 0x04000C37 RID: 3127
	private float lastScratchSoundAngle;

	// Token: 0x04000C38 RID: 3128
	private float cantForwardScratchUntilTimestamp;

	// Token: 0x04000C39 RID: 3129
	private float cantBackScratchUntilTimestamp;

	// Token: 0x04000C3A RID: 3130
	private float pausedUntilTimestamp;
}
