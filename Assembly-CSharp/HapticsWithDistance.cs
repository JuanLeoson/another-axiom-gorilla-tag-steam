using System;
using UnityEngine;

// Token: 0x02000495 RID: 1173
[RequireComponent(typeof(SphereCollider))]
public class HapticsWithDistance : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06001D08 RID: 7432 RVA: 0x0009C1BF File Offset: 0x0009A3BF
	private bool OnWrongLayer()
	{
		return base.gameObject.layer != 18;
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x0009C1D3 File Offset: 0x0009A3D3
	public void SetVibrationMult(float mult)
	{
		this.vibrationMult = mult;
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x0009C1DC File Offset: 0x0009A3DC
	public void FingerFlexVibrationMult(bool dummy, float mult)
	{
		this.SetVibrationMult(mult);
	}

	// Token: 0x06001D0B RID: 7435 RVA: 0x0009C1E5 File Offset: 0x0009A3E5
	private void Awake()
	{
		this.inverseColliderRadius = 1f / base.GetComponent<SphereCollider>().radius;
	}

	// Token: 0x06001D0C RID: 7436 RVA: 0x0009C200 File Offset: 0x0009A400
	private void OnTriggerEnter(Collider other)
	{
		GorillaGrabber gorillaGrabber;
		if (other.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled)
		{
			if (gorillaGrabber.IsLeftHand)
			{
				this.leftOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
				return;
			}
			if (gorillaGrabber.IsRightHand)
			{
				this.rightOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
			}
		}
	}

	// Token: 0x06001D0D RID: 7437 RVA: 0x0009C254 File Offset: 0x0009A454
	private void OnTriggerExit(Collider other)
	{
		if (this.leftOfflineHand == other.transform)
		{
			this.leftOfflineHand = null;
			if (!this.rightOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
				return;
			}
		}
		else if (this.rightOfflineHand == other.transform)
		{
			this.rightOfflineHand = null;
			if (!this.leftOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x0009C2BC File Offset: 0x0009A4BC
	private void OnDisable()
	{
		this.leftOfflineHand = null;
		this.rightOfflineHand = null;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001D0F RID: 7439 RVA: 0x0009C2D2 File Offset: 0x0009A4D2
	// (set) Token: 0x06001D10 RID: 7440 RVA: 0x0009C2DA File Offset: 0x0009A4DA
	public bool TickRunning { get; set; }

	// Token: 0x06001D11 RID: 7441 RVA: 0x0009C2E4 File Offset: 0x0009A4E4
	public void Tick()
	{
		Vector3 position = base.transform.position;
		if (this.leftOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(true, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.leftOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
		if (this.rightOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(false, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.rightOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
	}

	// Token: 0x04002571 RID: 9585
	[SerializeField]
	[Tooltip("X is the normalized distance and should start at 0 and end at 1. Y is the vibration amplitude and can be anywhere from 0-1.")]
	private AnimationCurve vibrationIntensityByDistance;

	// Token: 0x04002572 RID: 9586
	private float inverseColliderRadius;

	// Token: 0x04002573 RID: 9587
	private float vibrationMult = 1f;

	// Token: 0x04002574 RID: 9588
	private Transform leftOfflineHand;

	// Token: 0x04002575 RID: 9589
	private Transform rightOfflineHand;
}
