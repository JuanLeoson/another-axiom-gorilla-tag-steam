using System;
using UnityEngine;

// Token: 0x02000B21 RID: 2849
public class GrowUntilCollision : MonoBehaviour
{
	// Token: 0x06004499 RID: 17561 RVA: 0x00156A90 File Offset: 0x00154C90
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.audioSource != null)
		{
			this.maxVolume = this.audioSource.volume;
			this.maxPitch = this.audioSource.pitch;
		}
		this.zero();
	}

	// Token: 0x0600449A RID: 17562 RVA: 0x00156AE0 File Offset: 0x00154CE0
	private void zero()
	{
		base.transform.localScale = Vector3.one * this.initialRadius;
		if (this.audioSource != null)
		{
			this.audioSource.volume = 0f;
			this.audioSource.pitch = 1f;
		}
		this.timeSinceTrigger = 0f;
	}

	// Token: 0x0600449B RID: 17563 RVA: 0x00156B41 File Offset: 0x00154D41
	private void OnTriggerEnter(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x0600449C RID: 17564 RVA: 0x00156B41 File Offset: 0x00154D41
	private void OnTriggerExit(Collider other)
	{
		this.tryToTrigger(base.transform.position, other.transform.position);
	}

	// Token: 0x0600449D RID: 17565 RVA: 0x00156B60 File Offset: 0x00154D60
	private void OnCollisionEnter(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x0600449E RID: 17566 RVA: 0x00156B90 File Offset: 0x00154D90
	private void OnCollisionExit(Collision collision)
	{
		this.tryToTrigger(base.transform.position, collision.GetContact(0).point);
	}

	// Token: 0x0600449F RID: 17567 RVA: 0x00156BBD File Offset: 0x00154DBD
	private void tryToTrigger(Vector3 p1, Vector3 p2)
	{
		if (this.timeSinceTrigger > this.minRetriggerTime)
		{
			if (this.colliderFound != null)
			{
				this.colliderFound.Invoke(p1, p2);
			}
			this.zero();
		}
	}

	// Token: 0x060044A0 RID: 17568 RVA: 0x00156BE8 File Offset: 0x00154DE8
	private void Update()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		if (base.transform.localScale.x < this.maxSize * num)
		{
			base.transform.localScale += Vector3.one * Time.deltaTime * num;
			if (this.audioSource != null)
			{
				this.audioSource.volume = this.maxVolume * (base.transform.localScale.x / this.maxSize);
				this.audioSource.pitch = 1f + this.maxPitch * (base.transform.localScale.x / this.maxSize);
			}
		}
		this.timeSinceTrigger += Time.deltaTime;
	}

	// Token: 0x04004EBD RID: 20157
	[SerializeField]
	private float maxSize = 10f;

	// Token: 0x04004EBE RID: 20158
	[SerializeField]
	private float initialRadius = 1f;

	// Token: 0x04004EBF RID: 20159
	[SerializeField]
	private float minRetriggerTime = 1f;

	// Token: 0x04004EC0 RID: 20160
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04004EC1 RID: 20161
	private AudioSource audioSource;

	// Token: 0x04004EC2 RID: 20162
	private float maxVolume;

	// Token: 0x04004EC3 RID: 20163
	private float maxPitch;

	// Token: 0x04004EC4 RID: 20164
	private float timeSinceTrigger;
}
