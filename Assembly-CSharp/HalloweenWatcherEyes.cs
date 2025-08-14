using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000859 RID: 2137
public class HalloweenWatcherEyes : MonoBehaviour
{
	// Token: 0x060035B3 RID: 13747 RVA: 0x001197D4 File Offset: 0x001179D4
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x00119833 File Offset: 0x00117A33
	private IEnumerator CheckIfNearPlayer(float initialSleep)
	{
		yield return new WaitForSeconds(initialSleep);
		for (;;)
		{
			base.enabled = ((base.transform.position - GTPlayer.Instance.transform.position).sqrMagnitude < this.watchRange * this.watchRange);
			if (!base.enabled)
			{
				this.LookNormal();
			}
			yield return new WaitForSeconds(this.timeBetweenUpdates);
		}
		yield break;
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x0011984C File Offset: 0x00117A4C
	private void Update()
	{
		Vector3 normalized = (GTPlayer.Instance.headCollider.transform.position - base.transform.position).normalized;
		if (Vector3.Dot(GTPlayer.Instance.headCollider.transform.forward, -normalized) > this.playersViewCenterCosAngle)
		{
			this.LookNormal();
			this.pretendingToBeNormalUntilTimestamp = Time.time + this.durationToBeNormalWhenPlayerLooks;
		}
		if (this.pretendingToBeNormalUntilTimestamp > Time.time)
		{
			return;
		}
		if (Vector3.Dot(base.transform.forward, normalized) < this.watchMinCosAngle)
		{
			this.LookNormal();
			return;
		}
		Quaternion b = Quaternion.LookRotation(normalized, base.transform.up);
		Quaternion rotation = Quaternion.Lerp(base.transform.rotation, b, this.lerpValue);
		this.leftEye.transform.rotation = rotation;
		this.rightEye.transform.rotation = rotation;
		if (this.lerpDuration > 0f)
		{
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, 1f, Time.deltaTime / this.lerpDuration);
			return;
		}
		this.lerpValue = 1f;
	}

	// Token: 0x060035B6 RID: 13750 RVA: 0x0011997A File Offset: 0x00117B7A
	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	// Token: 0x040042A2 RID: 17058
	public float timeBetweenUpdates = 5f;

	// Token: 0x040042A3 RID: 17059
	public float watchRange;

	// Token: 0x040042A4 RID: 17060
	public float watchMaxAngle;

	// Token: 0x040042A5 RID: 17061
	public float lerpDuration = 1f;

	// Token: 0x040042A6 RID: 17062
	public float playersViewCenterAngle = 30f;

	// Token: 0x040042A7 RID: 17063
	public float durationToBeNormalWhenPlayerLooks = 3f;

	// Token: 0x040042A8 RID: 17064
	public GameObject leftEye;

	// Token: 0x040042A9 RID: 17065
	public GameObject rightEye;

	// Token: 0x040042AA RID: 17066
	private float playersViewCenterCosAngle;

	// Token: 0x040042AB RID: 17067
	private float watchMinCosAngle;

	// Token: 0x040042AC RID: 17068
	private float pretendingToBeNormalUntilTimestamp;

	// Token: 0x040042AD RID: 17069
	private float lerpValue;
}
