using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200057F RID: 1407
public class ElderGorilla : MonoBehaviour
{
	// Token: 0x0600225C RID: 8796 RVA: 0x000B98B0 File Offset: 0x000B7AB0
	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}
		if (GTPlayer.Instance.inOverlay || !GTPlayer.Instance.isUserPresent)
		{
			return;
		}
		this.tHMD = GTPlayer.Instance.headCollider.transform;
		this.tLeftHand = GTPlayer.Instance.leftControllerTransform;
		this.tRightHand = GTPlayer.Instance.rightControllerTransform;
		if (Time.time - this.timeLastValidArmDist > 1f)
		{
			this.CheckHandDistance(this.tLeftHand);
			this.CheckHandDistance(this.tRightHand);
		}
		this.CheckHeight();
		this.CheckMicVolume();
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x000B9950 File Offset: 0x000B7B50
	private void CheckHandDistance(Transform hand)
	{
		float num = Vector3.Distance(hand.localPosition, this.tHMD.localPosition);
		if (num >= 1f)
		{
			return;
		}
		if (num >= 0.75f)
		{
			this.countValidArmDists++;
			this.timeLastValidArmDist = Time.time;
		}
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000B99A0 File Offset: 0x000B7BA0
	private void CheckHeight()
	{
		float y = this.tHMD.localPosition.y;
		if (!this.trackingHeadHeight)
		{
			this.trackedHeadHeight = y - 0.05f;
			this.timerTrackedHeadHeight = 0f;
		}
		else if (this.trackedHeadHeight < y)
		{
			this.trackingHeadHeight = false;
		}
		if (this.trackingHeadHeight)
		{
			if (this.timerTrackedHeadHeight >= 1f)
			{
				this.savedHeadHeight = y;
				this.trackingHeadHeight = false;
				return;
			}
			this.timerTrackedHeadHeight += Time.deltaTime;
		}
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000B9A26 File Offset: 0x000B7C26
	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	// Token: 0x04002BC6 RID: 11206
	private const float MAX_HAND_DIST = 1f;

	// Token: 0x04002BC7 RID: 11207
	private const float COOLDOWN_HAND_DIST = 1f;

	// Token: 0x04002BC8 RID: 11208
	private const float VALID_HAND_DIST = 0.75f;

	// Token: 0x04002BC9 RID: 11209
	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	// Token: 0x04002BCA RID: 11210
	private Transform tHMD;

	// Token: 0x04002BCB RID: 11211
	private Transform tLeftHand;

	// Token: 0x04002BCC RID: 11212
	private Transform tRightHand;

	// Token: 0x04002BCD RID: 11213
	private int countValidArmDists;

	// Token: 0x04002BCE RID: 11214
	private float timeLastValidArmDist;

	// Token: 0x04002BCF RID: 11215
	private bool trackingHeadHeight;

	// Token: 0x04002BD0 RID: 11216
	private float trackedHeadHeight;

	// Token: 0x04002BD1 RID: 11217
	private float timerTrackedHeadHeight;

	// Token: 0x04002BD2 RID: 11218
	private float savedHeadHeight = 1.5f;
}
