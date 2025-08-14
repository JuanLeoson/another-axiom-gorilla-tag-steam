using System;
using GorillaExtensions;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x020009D9 RID: 2521
public class ForceVolume : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003D52 RID: 15698 RVA: 0x00138B00 File Offset: 0x00136D00
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x06003D53 RID: 15699 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003D54 RID: 15700 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x00138B18 File Offset: 0x00136D18
	public void SliceUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x00138B68 File Offset: 0x00136D68
	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		return rb != null && xf != null;
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x00138BC8 File Offset: 0x00136DC8
	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.enterClip == null)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.enterClip, 1f);
			this.audioState = ForceVolume.AudioState.Enter;
		}
		this.enterPos = transform.position;
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x00138C38 File Offset: 0x00136E38
	public void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			this.audioState = ForceVolume.AudioState.None;
		}
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x00138C90 File Offset: 0x00136E90
	public void OnTriggerStay(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource && !this.audioSource.isPlaying)
		{
			ForceVolume.AudioState audioState = this.audioState;
			if (audioState != ForceVolume.AudioState.Enter)
			{
				if (audioState == ForceVolume.AudioState.Loop)
				{
					if (this.loopClip != null)
					{
						this.audioSource.enabled = true;
						this.audioSource.GTPlayOneShot(this.loopClip, 1f);
					}
					this.audioState = ForceVolume.AudioState.Loop;
				}
			}
			else
			{
				if (this.loopCresendoClip != null)
				{
					this.audioSource.enabled = true;
					this.audioSource.GTPlayOneShot(this.loopCresendoClip, 1f);
				}
				this.audioState = ForceVolume.AudioState.Crescendo;
			}
		}
		if (this.disableGrip)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		VRRig.LocalRig.BreakHandLinks();
		SizeManager sizeManager = null;
		if (this.scaleWithSize)
		{
			sizeManager = rigidbody.GetComponent<SizeManager>();
		}
		Vector3 vector = rigidbody.velocity;
		if (this.scaleWithSize && sizeManager)
		{
			vector /= sizeManager.currentScale;
		}
		Vector3 b = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 a = base.transform.position + b - transform.position;
		float num = a.magnitude + 0.0001f;
		Vector3 vector2 = a / num;
		float num2 = Vector3.Dot(vector, vector2);
		float d = this.accel;
		if (this.maxDepth > -1f)
		{
			float num3 = Vector3.Dot(transform.position - this.enterPos, vector2);
			float num4 = this.maxDepth - num3;
			float b2 = 0f;
			if (num4 > 0.0001f)
			{
				b2 = num2 * num2 / num4;
			}
			d = Mathf.Max(this.accel, b2);
		}
		float deltaTime = Time.deltaTime;
		Vector3 b3 = base.transform.up * d * deltaTime;
		vector += b3;
		Vector3 a2 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
		Vector3 a3 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
		Vector3 a4 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
		float d2 = 1f;
		float d3 = 1f;
		if (this.dampenLateralVelocity)
		{
			d2 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
			d3 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
		}
		vector = a2 + d2 * a3 + d3 * a4;
		if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
		{
			vector -= num2 * vector2;
			if (num > this.pullTOCenterMinDistance)
			{
				num2 += this.pullToCenterAccel * deltaTime;
				float b4 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, b4);
			}
			else
			{
				num2 = 0f;
			}
			vector += num2 * vector2;
			if (vector.magnitude > 0.0001f)
			{
				Vector3 vector3 = Vector3.Cross(base.transform.up, vector2);
				float magnitude = vector3.magnitude;
				if (magnitude > 0.0001f)
				{
					vector3 /= magnitude;
					num2 = Vector3.Dot(vector, vector3);
					vector -= num2 * vector3;
					num2 -= this.pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					vector += num2 * vector3;
				}
			}
		}
		if (this.scaleWithSize && sizeManager)
		{
			vector *= sizeManager.currentScale;
		}
		rigidbody.velocity = vector;
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x001390B4 File Offset: 0x001372B4
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x00139124 File Offset: 0x00137324
	public void SetPropertiesFromPlaceholder(ForceVolumeProperties properties, AudioSource volumeAudioSource, Collider colliderVolume)
	{
		this.accel = properties.accel;
		this.maxDepth = properties.maxDepth;
		this.maxSpeed = properties.maxSpeed;
		this.disableGrip = properties.disableGrip;
		this.dampenLateralVelocity = properties.dampenLateralVelocity;
		this.dampenXVelPerc = properties.dampenXVel;
		this.dampenZVelPerc = properties.dampenZVel;
		this.applyPullToCenterAcceleration = properties.applyPullToCenterAcceleration;
		this.pullToCenterAccel = properties.pullToCenterAccel;
		this.pullToCenterMaxSpeed = properties.pullToCenterMaxSpeed;
		this.pullTOCenterMinDistance = properties.pullToCenterMinDistance;
		this.enterClip = properties.enterClip;
		this.exitClip = properties.exitClip;
		this.loopClip = properties.loopClip;
		this.loopCresendoClip = properties.loopCrescendoClip;
		if (volumeAudioSource.IsNotNull())
		{
			this.audioSource = volumeAudioSource;
		}
		if (colliderVolume.IsNotNull())
		{
			this.volume = colliderVolume;
		}
	}

	// Token: 0x04004986 RID: 18822
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04004987 RID: 18823
	[SerializeField]
	private float accel;

	// Token: 0x04004988 RID: 18824
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x04004989 RID: 18825
	[SerializeField]
	private float maxSpeed;

	// Token: 0x0400498A RID: 18826
	[SerializeField]
	private bool disableGrip;

	// Token: 0x0400498B RID: 18827
	[SerializeField]
	private bool dampenLateralVelocity = true;

	// Token: 0x0400498C RID: 18828
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x0400498D RID: 18829
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x0400498E RID: 18830
	[SerializeField]
	private bool applyPullToCenterAcceleration = true;

	// Token: 0x0400498F RID: 18831
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x04004990 RID: 18832
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x04004991 RID: 18833
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x04004992 RID: 18834
	private Collider volume;

	// Token: 0x04004993 RID: 18835
	public AudioClip enterClip;

	// Token: 0x04004994 RID: 18836
	public AudioClip exitClip;

	// Token: 0x04004995 RID: 18837
	public AudioClip loopClip;

	// Token: 0x04004996 RID: 18838
	public AudioClip loopCresendoClip;

	// Token: 0x04004997 RID: 18839
	public AudioSource audioSource;

	// Token: 0x04004998 RID: 18840
	private Vector3 enterPos;

	// Token: 0x04004999 RID: 18841
	private ForceVolume.AudioState audioState;

	// Token: 0x020009DA RID: 2522
	private enum AudioState
	{
		// Token: 0x0400499B RID: 18843
		None,
		// Token: 0x0400499C RID: 18844
		Enter,
		// Token: 0x0400499D RID: 18845
		Crescendo,
		// Token: 0x0400499E RID: 18846
		Loop,
		// Token: 0x0400499F RID: 18847
		Exit
	}
}
