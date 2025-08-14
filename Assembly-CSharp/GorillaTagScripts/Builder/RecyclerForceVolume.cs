using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA6 RID: 3238
	public class RecyclerForceVolume : MonoBehaviour
	{
		// Token: 0x06005065 RID: 20581 RVA: 0x001912E2 File Offset: 0x0018F4E2
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.hasWindFX = (this.windEffectRenderer != null);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x06005066 RID: 20582 RVA: 0x00191318 File Offset: 0x0018F518
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

		// Token: 0x06005067 RID: 20583 RVA: 0x00191378 File Offset: 0x0018F578
		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.enterPos = transform.position;
			ObjectPools.instance.Instantiate(this.windSFX, this.enterPos, true);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.transform.position = base.transform.position + Vector3.Dot(this.enterPos - base.transform.position, base.transform.right) * base.transform.right;
				this.windEffectRenderer.enabled = true;
			}
		}

		// Token: 0x06005068 RID: 20584 RVA: 0x00191428 File Offset: 0x0018F628
		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x0019145C File Offset: 0x0018F65C
		public void OnTriggerStay(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
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
			Vector3 a = Vector3.Dot(base.transform.position - transform.position, base.transform.up) * base.transform.up;
			float num = a.magnitude + 0.0001f;
			Vector3 vector2 = a / num;
			float num2 = Vector3.Dot(vector, vector2);
			float d = this.accel;
			if (this.maxDepth > -1f)
			{
				float num3 = Vector3.Dot(transform.position - this.enterPos, vector2);
				float num4 = this.maxDepth - num3;
				float b = 0f;
				if (num4 > 0.0001f)
				{
					b = num2 * num2 / num4;
				}
				d = Mathf.Max(this.accel, b);
			}
			float deltaTime = Time.deltaTime;
			Vector3 b2 = base.transform.forward * d * deltaTime;
			vector += b2;
			Vector3 a2 = Vector3.Dot(vector, base.transform.up) * base.transform.up;
			Vector3 a3 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 b3 = Mathf.Clamp(Vector3.Dot(vector, base.transform.forward), -1f * this.maxSpeed, this.maxSpeed) * base.transform.forward;
			float d2 = 1f;
			float d3 = 1f;
			if (this.dampenLateralVelocity)
			{
				d2 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				d3 = 1f - this.dampenYVelPerc * 0.01f * deltaTime;
			}
			vector = d3 * a2 + d2 * a3 + b3;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector2;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num5 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Clamp(num2, -1f * num5, num5);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector2;
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.velocity = vector;
		}

		// Token: 0x040059B8 RID: 22968
		[SerializeField]
		public bool scaleWithSize = true;

		// Token: 0x040059B9 RID: 22969
		[SerializeField]
		private float accel;

		// Token: 0x040059BA RID: 22970
		[SerializeField]
		private float maxDepth = -1f;

		// Token: 0x040059BB RID: 22971
		[SerializeField]
		private float maxSpeed;

		// Token: 0x040059BC RID: 22972
		[SerializeField]
		private bool disableGrip;

		// Token: 0x040059BD RID: 22973
		[SerializeField]
		private bool dampenLateralVelocity = true;

		// Token: 0x040059BE RID: 22974
		[SerializeField]
		private float dampenXVelPerc;

		// Token: 0x040059BF RID: 22975
		[FormerlySerializedAs("dampenZVelPerc")]
		[SerializeField]
		private float dampenYVelPerc;

		// Token: 0x040059C0 RID: 22976
		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		// Token: 0x040059C1 RID: 22977
		[SerializeField]
		private float pullToCenterAccel;

		// Token: 0x040059C2 RID: 22978
		[SerializeField]
		private float pullToCenterMaxSpeed;

		// Token: 0x040059C3 RID: 22979
		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		// Token: 0x040059C4 RID: 22980
		private Collider volume;

		// Token: 0x040059C5 RID: 22981
		public GameObject windSFX;

		// Token: 0x040059C6 RID: 22982
		[SerializeField]
		private MeshRenderer windEffectRenderer;

		// Token: 0x040059C7 RID: 22983
		private bool hasWindFX;

		// Token: 0x040059C8 RID: 22984
		private Vector3 enterPos;
	}
}
