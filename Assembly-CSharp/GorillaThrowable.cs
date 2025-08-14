using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020007F8 RID: 2040
public class GorillaThrowable : MonoBehaviourPun, IPunObservable, IPhotonViewCallback
{
	// Token: 0x0600330D RID: 13069 RVA: 0x001098E0 File Offset: 0x00107AE0
	public virtual void Start()
	{
		this.offset = Vector3.zero;
		this.headsetTransform = GTPlayer.Instance.headCollider.transform;
		this.velocityHistory = new Vector3[this.trackingHistorySize];
		this.positionHistory = new Vector3[this.trackingHistorySize];
		this.headsetPositionHistory = new Vector3[this.trackingHistorySize];
		this.rotationHistory = new Vector3[this.trackingHistorySize];
		this.rotationalVelocityHistory = new Vector3[this.trackingHistorySize];
		for (int i = 0; i < this.trackingHistorySize; i++)
		{
			this.velocityHistory[i] = Vector3.zero;
			this.positionHistory[i] = base.transform.position - this.headsetTransform.position;
			this.headsetPositionHistory[i] = this.headsetTransform.position;
			this.rotationHistory[i] = base.transform.eulerAngles;
			this.rotationalVelocityHistory[i] = Vector3.zero;
		}
		this.currentIndex = 0;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x00109A00 File Offset: 0x00107C00
	public virtual void LateUpdate()
	{
		if (this.isHeld && base.photonView.IsMine)
		{
			base.transform.rotation = this.transformToFollow.rotation * this.offsetRotation;
			if (!this.initialLerp && (base.transform.position - this.transformToFollow.position).magnitude > this.lerpDistanceLimit)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.transformToFollow.position + this.transformToFollow.rotation * this.offset, this.pickupLerp);
			}
			else
			{
				this.initialLerp = true;
				base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
			}
		}
		if (!base.photonView.IsMine)
		{
			this.rigidbody.isKinematic = true;
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.lerpValue);
		}
		this.StoreHistories();
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x000023F5 File Offset: 0x000005F5
	private void IsHandPushing(XRNode node)
	{
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x00109B6C File Offset: 0x00107D6C
	private void StoreHistories()
	{
		this.previousPosition = this.positionHistory[this.currentIndex];
		this.previousRotation = this.rotationHistory[this.currentIndex];
		this.previousHeadsetPosition = this.headsetPositionHistory[this.currentIndex];
		this.currentIndex = (this.currentIndex + 1) % this.trackingHistorySize;
		this.currentVelocity = (base.transform.position - this.headsetTransform.position - this.previousPosition) / Time.deltaTime;
		this.currentHeadsetVelocity = (this.headsetTransform.position - this.previousHeadsetPosition) / Time.deltaTime;
		this.currentRotationalVelocity = (base.transform.eulerAngles - this.previousRotation) / Time.deltaTime;
		this.denormalizedVelocityAverage = Vector3.zero;
		this.denormalizedRotationalVelocityAverage = Vector3.zero;
		this.loopIndex = 0;
		while (this.loopIndex < this.trackingHistorySize)
		{
			this.denormalizedVelocityAverage += this.velocityHistory[this.loopIndex];
			this.denormalizedRotationalVelocityAverage += this.rotationalVelocityHistory[this.loopIndex];
			this.loopIndex++;
		}
		this.denormalizedVelocityAverage /= (float)this.trackingHistorySize;
		this.denormalizedRotationalVelocityAverage /= (float)this.trackingHistorySize;
		this.velocityHistory[this.currentIndex] = this.currentVelocity;
		this.positionHistory[this.currentIndex] = base.transform.position - this.headsetTransform.position;
		this.headsetPositionHistory[this.currentIndex] = this.headsetTransform.position;
		this.rotationHistory[this.currentIndex] = base.transform.eulerAngles;
		this.rotationalVelocityHistory[this.currentIndex] = this.currentRotationalVelocity;
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x00109D98 File Offset: 0x00107F98
	public virtual void Grabbed(Transform grabTransform)
	{
		this.grabbingTransform = grabTransform;
		this.isHeld = true;
		this.transformToFollow = this.grabbingTransform;
		this.offsetRotation = base.transform.rotation * Quaternion.Inverse(this.transformToFollow.rotation);
		this.initialLerp = false;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		base.photonView.RequestOwnership();
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x00109E10 File Offset: 0x00108010
	public virtual void ThrowThisThingo()
	{
		this.transformToFollow = null;
		this.isHeld = false;
		this.synchThrow = true;
		this.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		this.rigidbody.isKinematic = false;
		this.rigidbody.useGravity = true;
		if (this.isLinear || this.denormalizedVelocityAverage.magnitude < this.linearMax)
		{
			if (this.denormalizedVelocityAverage.magnitude * this.throwMultiplier < this.throwMagnitudeLimit)
			{
				this.rigidbody.velocity = this.denormalizedVelocityAverage * this.throwMultiplier + this.currentHeadsetVelocity;
			}
			else
			{
				this.rigidbody.velocity = this.denormalizedVelocityAverage.normalized * this.throwMagnitudeLimit + this.currentHeadsetVelocity;
			}
		}
		else
		{
			this.rigidbody.velocity = this.denormalizedVelocityAverage.normalized * Mathf.Max(Mathf.Min(Mathf.Pow(this.throwMultiplier * this.denormalizedVelocityAverage.magnitude / this.linearMax, this.exponThrowMultMax), 0.1f) * this.denormalizedHeadsetVelocityAverage.magnitude, this.throwMagnitudeLimit) + this.currentHeadsetVelocity;
		}
		this.rigidbody.angularVelocity = this.denormalizedRotationalVelocityAverage * 3.1415927f / 180f;
		this.rigidbody.MovePosition(this.rigidbody.transform.position + this.rigidbody.velocity * Time.deltaTime);
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x00109FAC File Offset: 0x001081AC
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.rigidbody.velocity);
			return;
		}
		this.targetPosition = (Vector3)stream.ReceiveNext();
		this.targetRotation = (Quaternion)stream.ReceiveNext();
		this.rigidbody.velocity = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x0010A03C File Offset: 0x0010823C
	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<GorillaSurfaceOverride>() != null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("PlaySurfaceHit", RpcTarget.Others, new object[]
				{
					this.bounceAudioClip,
					this.InterpolateVolume()
				});
			}
			this.PlaySurfaceHit(collision.collider.GetComponent<GorillaSurfaceOverride>().overrideIndex, this.InterpolateVolume());
		}
	}

	// Token: 0x06003315 RID: 13077 RVA: 0x0010A0B8 File Offset: 0x001082B8
	[PunRPC]
	public void PlaySurfaceHit(int soundIndex, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			this.audioSource.volume = tapVolume;
			this.audioSource.clip = (GTPlayer.Instance.materialData[soundIndex].overrideAudio ? GTPlayer.Instance.materialData[soundIndex].audio : GTPlayer.Instance.materialData[0].audio);
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
		}
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x0010A154 File Offset: 0x00108354
	public float InterpolateVolume()
	{
		return (Mathf.Clamp(this.rigidbody.velocity.magnitude, this.minVelocity, this.maxVelocity) - this.minVelocity) / (this.maxVelocity - this.minVelocity) * (this.maxVolume - this.minVolume) + this.minVolume;
	}

	// Token: 0x04003FEF RID: 16367
	public int trackingHistorySize;

	// Token: 0x04003FF0 RID: 16368
	public float throwMultiplier;

	// Token: 0x04003FF1 RID: 16369
	public float throwMagnitudeLimit;

	// Token: 0x04003FF2 RID: 16370
	private Vector3[] velocityHistory;

	// Token: 0x04003FF3 RID: 16371
	private Vector3[] headsetVelocityHistory;

	// Token: 0x04003FF4 RID: 16372
	private Vector3[] positionHistory;

	// Token: 0x04003FF5 RID: 16373
	private Vector3[] headsetPositionHistory;

	// Token: 0x04003FF6 RID: 16374
	private Vector3[] rotationHistory;

	// Token: 0x04003FF7 RID: 16375
	private Vector3[] rotationalVelocityHistory;

	// Token: 0x04003FF8 RID: 16376
	private Vector3 previousPosition;

	// Token: 0x04003FF9 RID: 16377
	private Vector3 previousRotation;

	// Token: 0x04003FFA RID: 16378
	private Vector3 previousHeadsetPosition;

	// Token: 0x04003FFB RID: 16379
	private int currentIndex;

	// Token: 0x04003FFC RID: 16380
	private Vector3 currentVelocity;

	// Token: 0x04003FFD RID: 16381
	private Vector3 currentHeadsetVelocity;

	// Token: 0x04003FFE RID: 16382
	private Vector3 currentRotationalVelocity;

	// Token: 0x04003FFF RID: 16383
	public Vector3 denormalizedVelocityAverage;

	// Token: 0x04004000 RID: 16384
	private Vector3 denormalizedHeadsetVelocityAverage;

	// Token: 0x04004001 RID: 16385
	private Vector3 denormalizedRotationalVelocityAverage;

	// Token: 0x04004002 RID: 16386
	private Transform headsetTransform;

	// Token: 0x04004003 RID: 16387
	private Vector3 targetPosition;

	// Token: 0x04004004 RID: 16388
	private Quaternion targetRotation;

	// Token: 0x04004005 RID: 16389
	public bool initialLerp;

	// Token: 0x04004006 RID: 16390
	public float lerpValue = 0.4f;

	// Token: 0x04004007 RID: 16391
	public float lerpDistanceLimit = 0.01f;

	// Token: 0x04004008 RID: 16392
	public bool isHeld;

	// Token: 0x04004009 RID: 16393
	public Rigidbody rigidbody;

	// Token: 0x0400400A RID: 16394
	private int loopIndex;

	// Token: 0x0400400B RID: 16395
	private Transform transformToFollow;

	// Token: 0x0400400C RID: 16396
	private Vector3 offset;

	// Token: 0x0400400D RID: 16397
	private Quaternion offsetRotation;

	// Token: 0x0400400E RID: 16398
	public AudioSource audioSource;

	// Token: 0x0400400F RID: 16399
	public int timeLastReceived;

	// Token: 0x04004010 RID: 16400
	public bool synchThrow;

	// Token: 0x04004011 RID: 16401
	public float tempFloat;

	// Token: 0x04004012 RID: 16402
	public Transform grabbingTransform;

	// Token: 0x04004013 RID: 16403
	public float pickupLerp;

	// Token: 0x04004014 RID: 16404
	public float minVelocity;

	// Token: 0x04004015 RID: 16405
	public float maxVelocity;

	// Token: 0x04004016 RID: 16406
	public float minVolume;

	// Token: 0x04004017 RID: 16407
	public float maxVolume;

	// Token: 0x04004018 RID: 16408
	public bool isLinear;

	// Token: 0x04004019 RID: 16409
	public float linearMax;

	// Token: 0x0400401A RID: 16410
	public float exponThrowMultMax;

	// Token: 0x0400401B RID: 16411
	public int bounceAudioClip;
}
