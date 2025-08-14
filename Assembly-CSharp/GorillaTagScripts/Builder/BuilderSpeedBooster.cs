using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA2 RID: 3234
	public class BuilderSpeedBooster : MonoBehaviour
	{
		// Token: 0x0600504D RID: 20557 RVA: 0x001906DC File Offset: 0x0018E8DC
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.windRenderer.enabled = false;
			this.boosting = false;
		}

		// Token: 0x0600504E RID: 20558 RVA: 0x00190700 File Offset: 0x0018E900
		private void LateUpdate()
		{
			if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
			{
				this.audioSource.enabled = false;
			}
		}

		// Token: 0x0600504F RID: 20559 RVA: 0x00190750 File Offset: 0x0018E950
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

		// Token: 0x06005050 RID: 20560 RVA: 0x001907B0 File Offset: 0x0018E9B0
		private void CheckTableZone()
		{
			if (this.hasCheckedZone)
			{
				return;
			}
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
			{
				this.ignoreMonkeScale = !builderTable.isTableMutable;
			}
			this.hasCheckedZone = true;
		}

		// Token: 0x06005051 RID: 20561 RVA: 0x001907FC File Offset: 0x0018E9FC
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			this.positiveForce = (Vector3.Dot(base.transform.up, rigidbody.velocity) > 0f);
			if (this.positiveForce)
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
			else
			{
				this.windRenderer.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
			}
			this.windRenderer.enabled = true;
			this.enterPos = transform.position;
			if (!this.boosting)
			{
				this.boosting = true;
				this.enterTime = Time.timeAsDouble;
			}
		}

		// Token: 0x06005052 RID: 20562 RVA: 0x001908EC File Offset: 0x0018EAEC
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.windRenderer.enabled = false;
			this.CheckTableZone();
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.boosting && this.audioSource)
			{
				this.audioSource.enabled = true;
				this.audioSource.Stop();
				this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			}
			this.boosting = false;
		}

		// Token: 0x06005053 RID: 20563 RVA: 0x0019098C File Offset: 0x0018EB8C
		public void OnTriggerStay(Collider other)
		{
			if (!this.boosting)
			{
				return;
			}
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (!this.ignoreMonkeScale && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor > 0.99)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (Time.timeAsDouble > this.enterTime + (double)this.maxBoostDuration)
			{
				this.OnTriggerExit(other);
				return;
			}
			if (this.disableGrip)
			{
				GTPlayer.Instance.SetMaximumSlipThisFrame();
			}
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
			Vector3 vector3 = base.transform.up * d * deltaTime;
			if (!this.positiveForce)
			{
				vector3 *= -1f;
			}
			vector += vector3;
			if ((double)Vector3.Dot(vector3, Vector3.down) <= 0.1)
			{
				vector += Vector3.up * this.addedWorldUpVelocity * deltaTime;
			}
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
					float b3 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Min(num2, b3);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector2;
				if (vector.magnitude > 0.0001f)
				{
					Vector3 vector4 = Vector3.Cross(base.transform.up, vector2);
					float magnitude = vector4.magnitude;
					if (magnitude > 0.0001f)
					{
						vector4 /= magnitude;
						num2 = Vector3.Dot(vector, vector4);
						vector -= num2 * vector4;
						num2 -= this.pullToCenterAccel * deltaTime;
						num2 = Mathf.Max(0f, num2);
						vector += num2 * vector4;
					}
				}
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.velocity = vector;
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x00190DA0 File Offset: 0x0018EFA0
		public void OnDrawGizmosSelected()
		{
			base.GetComponents<Collider>();
			Gizmos.color = Color.magenta;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
		}

		// Token: 0x04005983 RID: 22915
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x04005984 RID: 22916
		[SerializeField]
		private float accel;

		// Token: 0x04005985 RID: 22917
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x04005986 RID: 22918
		[SerializeField]
		private float maxSpeed;

		// Token: 0x04005987 RID: 22919
		[SerializeField]
		private bool disableGrip;

		// Token: 0x04005988 RID: 22920
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x04005989 RID: 22921
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x0400598A RID: 22922
		[SerializeField]
		private float dampenZVelPerc;

		// Token: 0x0400598B RID: 22923
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x0400598C RID: 22924
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x0400598D RID: 22925
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x0400598E RID: 22926
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x0400598F RID: 22927
		[SerializeField]
		private float addedWorldUpVelocity = 10f;

		// Token: 0x04005990 RID: 22928
		[SerializeField]
		private float maxBoostDuration = 2f;

		// Token: 0x04005991 RID: 22929
		private bool boosting;

		// Token: 0x04005992 RID: 22930
		private double enterTime;

		// Token: 0x04005993 RID: 22931
		private Collider volume;

		// Token: 0x04005994 RID: 22932
		public AudioClip exitClip;

		// Token: 0x04005995 RID: 22933
		public AudioSource audioSource;

		// Token: 0x04005996 RID: 22934
		public MeshRenderer windRenderer;

		// Token: 0x04005997 RID: 22935
		private Vector3 enterPos;

		// Token: 0x04005998 RID: 22936
		private bool positiveForce = true;

		// Token: 0x04005999 RID: 22937
		private bool ignoreMonkeScale;

		// Token: 0x0400599A RID: 22938
		private bool hasCheckedZone;
	}
}
