using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000380 RID: 896
public class BouncingBallLogic : MonoBehaviour
{
	// Token: 0x06001521 RID: 5409 RVA: 0x00073186 File Offset: 0x00071386
	private void OnCollisionEnter()
	{
		this.audioSource.PlayOneShot(this.bounce);
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00073199 File Offset: 0x00071399
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.PlayOneShot(this.loadball);
		this.centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000731D0 File Offset: 0x000713D0
	private void Update()
	{
		if (!this.isReleased)
		{
			return;
		}
		this.UpdateVisibility();
		this.timer += Time.deltaTime;
		if (!this.isReadyForDestroy && this.timer >= this.TTL)
		{
			this.isReadyForDestroy = true;
			float length = this.pop.length;
			this.audioSource.PlayOneShot(this.pop);
			base.StartCoroutine(this.PlayPopCallback(length));
		}
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x00073248 File Offset: 0x00071448
	private void UpdateVisibility()
	{
		Vector3 direction = this.centerEyeCamera.position - base.transform.position;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position, direction), out raycastHit, direction.magnitude))
		{
			if (raycastHit.collider.gameObject != base.gameObject)
			{
				this.SetVisible(false);
				return;
			}
		}
		else
		{
			this.SetVisible(true);
		}
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x000732BC File Offset: 0x000714BC
	private void SetVisible(bool setVisible)
	{
		if (this.isVisible && !setVisible)
		{
			base.GetComponent<MeshRenderer>().material = this.hiddenMat;
			this.isVisible = false;
		}
		if (!this.isVisible && setVisible)
		{
			base.GetComponent<MeshRenderer>().material = this.visibleMat;
			this.isVisible = true;
		}
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x00073311 File Offset: 0x00071511
	public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
	{
		this.isReleased = true;
		base.transform.position = pos;
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().velocity = vel;
		base.GetComponent<Rigidbody>().angularVelocity = angVel;
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x0007334A File Offset: 0x0007154A
	private IEnumerator PlayPopCallback(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04001CBA RID: 7354
	[SerializeField]
	private float TTL = 5f;

	// Token: 0x04001CBB RID: 7355
	[SerializeField]
	private AudioClip pop;

	// Token: 0x04001CBC RID: 7356
	[SerializeField]
	private AudioClip bounce;

	// Token: 0x04001CBD RID: 7357
	[SerializeField]
	private AudioClip loadball;

	// Token: 0x04001CBE RID: 7358
	[SerializeField]
	private Material visibleMat;

	// Token: 0x04001CBF RID: 7359
	[SerializeField]
	private Material hiddenMat;

	// Token: 0x04001CC0 RID: 7360
	private AudioSource audioSource;

	// Token: 0x04001CC1 RID: 7361
	private Transform centerEyeCamera;

	// Token: 0x04001CC2 RID: 7362
	private bool isVisible = true;

	// Token: 0x04001CC3 RID: 7363
	private float timer;

	// Token: 0x04001CC4 RID: 7364
	private bool isReleased;

	// Token: 0x04001CC5 RID: 7365
	private bool isReadyForDestroy;
}
