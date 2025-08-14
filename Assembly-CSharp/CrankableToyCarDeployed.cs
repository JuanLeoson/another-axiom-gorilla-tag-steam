using System;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class CrankableToyCarDeployed : MonoBehaviour
{
	// Token: 0x060009DC RID: 2524 RVA: 0x00035FA0 File Offset: 0x000341A0
	public void Deploy(CrankableToyCarHoldable holdable, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		this.holdable = holdable;
		holdable.OnCarDeployed();
		base.transform.position = launchPos;
		base.transform.rotation = launchRot;
		base.transform.localScale = holdable.transform.lossyScale;
		this.rb.velocity = releaseVel;
		this.startedAtTimestamp = Time.time;
		this.expiresAtTimestamp = Time.time + lifetime;
		this.isRemote = isRemote;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00036018 File Offset: 0x00034218
	private void Update()
	{
		if (!this.isRemote && Time.time > this.expiresAtTimestamp)
		{
			if (this.holdable != null)
			{
				this.holdable.OnCarReturned();
			}
			return;
		}
		if (!this.wheelDriver.hasCollision)
		{
			this.expiresAtTimestamp -= Time.deltaTime;
			if (!this.offGroundDrivingAudio.isPlaying)
			{
				this.offGroundDrivingAudio.GTPlay();
				this.drivingAudio.Stop();
			}
		}
		else if (!this.drivingAudio.isPlaying)
		{
			this.drivingAudio.GTPlay();
			this.offGroundDrivingAudio.Stop();
		}
		float time = Mathf.InverseLerp(this.startedAtTimestamp, this.expiresAtTimestamp, Time.time);
		float d = this.thrustCurve.Evaluate(time);
		this.wheelDriver.SetThrust(this.maxThrust * d);
	}

	// Token: 0x04000BD2 RID: 3026
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04000BD3 RID: 3027
	[SerializeField]
	private FakeWheelDriver wheelDriver;

	// Token: 0x04000BD4 RID: 3028
	[SerializeField]
	private Vector3 maxThrust;

	// Token: 0x04000BD5 RID: 3029
	[SerializeField]
	private AnimationCurve thrustCurve;

	// Token: 0x04000BD6 RID: 3030
	private float startedAtTimestamp;

	// Token: 0x04000BD7 RID: 3031
	private float expiresAtTimestamp;

	// Token: 0x04000BD8 RID: 3032
	private CrankableToyCarHoldable holdable;

	// Token: 0x04000BD9 RID: 3033
	[SerializeField]
	private AudioSource drivingAudio;

	// Token: 0x04000BDA RID: 3034
	[SerializeField]
	private AudioSource offGroundDrivingAudio;

	// Token: 0x04000BDB RID: 3035
	private bool isRemote;
}
