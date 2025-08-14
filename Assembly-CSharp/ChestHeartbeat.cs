using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200056E RID: 1390
public class ChestHeartbeat : MonoBehaviour
{
	// Token: 0x060021F3 RID: 8691 RVA: 0x000B85E0 File Offset: 0x000B67E0
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000B86EE File Offset: 0x000B68EE
	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x04002B7D RID: 11133
	public int millisToWait;

	// Token: 0x04002B7E RID: 11134
	public int millisMin = 300;

	// Token: 0x04002B7F RID: 11135
	public int lastShot;

	// Token: 0x04002B80 RID: 11136
	public AudioSource audioSource;

	// Token: 0x04002B81 RID: 11137
	public Transform scaleTransform;

	// Token: 0x04002B82 RID: 11138
	private float deltaTime;

	// Token: 0x04002B83 RID: 11139
	private float heartMinSize = 0.9f;

	// Token: 0x04002B84 RID: 11140
	private float heartMaxSize = 1.2f;

	// Token: 0x04002B85 RID: 11141
	private float minTime = 0.05f;

	// Token: 0x04002B86 RID: 11142
	private float maxTime = 0.1f;

	// Token: 0x04002B87 RID: 11143
	private float endtime = 0.25f;

	// Token: 0x04002B88 RID: 11144
	private float currentTime;
}
