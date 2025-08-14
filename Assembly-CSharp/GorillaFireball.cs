using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007F6 RID: 2038
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06003300 RID: 13056 RVA: 0x00109526 File Offset: 0x00107726
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x06003301 RID: 13057 RVA: 0x00109540 File Offset: 0x00107740
	private void Update()
	{
		if (this.explosionStartTime != 0f)
		{
			float num = (Time.time - this.explosionStartTime) / this.totalExplosionTime * (this.maxExplosionScale - 0.25f) + 0.25f;
			base.gameObject.transform.localScale = new Vector3(num, num, num);
			if (base.photonView.IsMine && Time.time > this.explosionStartTime + this.totalExplosionTime)
			{
				PhotonNetwork.Destroy(PhotonView.Get(this));
			}
		}
	}

	// Token: 0x06003302 RID: 13058 RVA: 0x001095C8 File Offset: 0x001077C8
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x06003303 RID: 13059 RVA: 0x00109614 File Offset: 0x00107814
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06003304 RID: 13060 RVA: 0x00109623 File Offset: 0x00107823
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", RpcTarget.All, null);
		}
	}

	// Token: 0x06003305 RID: 13061 RVA: 0x0010964C File Offset: 0x0010784C
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x0010966C File Offset: 0x0010786C
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (base.photonView.IsMine)
		{
			if ((bool)base.photonView.InstantiationData[0])
			{
				base.transform.parent = GorillaPlaySpace.Instance.myVRRig.leftHandTransform;
				return;
			}
			base.transform.parent = GorillaPlaySpace.Instance.myVRRig.rightHandTransform;
		}
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x001096CF File Offset: 0x001078CF
	[PunRPC]
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04003FE4 RID: 16356
	public float maxExplosionScale;

	// Token: 0x04003FE5 RID: 16357
	public float totalExplosionTime;

	// Token: 0x04003FE6 RID: 16358
	public float gravityStrength;

	// Token: 0x04003FE7 RID: 16359
	private bool canExplode;

	// Token: 0x04003FE8 RID: 16360
	private float explosionStartTime;
}
