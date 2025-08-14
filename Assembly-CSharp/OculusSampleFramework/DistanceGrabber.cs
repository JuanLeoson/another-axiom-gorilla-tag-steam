using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D00 RID: 3328
	[RequireComponent(typeof(Rigidbody))]
	public class DistanceGrabber : OVRGrabber
	{
		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x0600525E RID: 21086 RVA: 0x001993EA File Offset: 0x001975EA
		// (set) Token: 0x0600525F RID: 21087 RVA: 0x001993F2 File Offset: 0x001975F2
		public bool UseSpherecast
		{
			get
			{
				return this.m_useSpherecast;
			}
			set
			{
				this.m_useSpherecast = value;
				this.GrabVolumeEnable(!this.m_useSpherecast);
			}
		}

		// Token: 0x06005260 RID: 21088 RVA: 0x0019940C File Offset: 0x0019760C
		protected override void Start()
		{
			base.Start();
			Collider componentInChildren = this.m_player.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				this.m_maxGrabDistance = componentInChildren.bounds.size.z * 0.5f + 3f;
			}
			else
			{
				this.m_maxGrabDistance = 12f;
			}
			if (this.m_parentHeldObject)
			{
				Debug.LogError("m_parentHeldObject incompatible with DistanceGrabber. Setting to false.");
				this.m_parentHeldObject = false;
			}
			DistanceGrabber[] array = Object.FindObjectsOfType<DistanceGrabber>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != this)
				{
					this.m_otherHand = array[i];
				}
			}
		}

		// Token: 0x06005261 RID: 21089 RVA: 0x001994A8 File Offset: 0x001976A8
		public override void Update()
		{
			base.Update();
			Debug.DrawRay(base.transform.position, base.transform.forward, Color.red, 0.1f);
			DistanceGrabbable distanceGrabbable;
			Collider targetCollider;
			this.FindTarget(out distanceGrabbable, out targetCollider);
			if (distanceGrabbable != this.m_target)
			{
				if (this.m_target != null)
				{
					this.m_target.Targeted = (this.m_otherHand.m_target == this.m_target);
				}
				this.m_target = distanceGrabbable;
				this.m_targetCollider = targetCollider;
				if (this.m_target != null)
				{
					this.m_target.Targeted = true;
				}
			}
		}

		// Token: 0x06005262 RID: 21090 RVA: 0x00199550 File Offset: 0x00197750
		protected override void GrabBegin()
		{
			DistanceGrabbable target = this.m_target;
			Collider targetCollider = this.m_targetCollider;
			this.GrabVolumeEnable(false);
			if (target != null)
			{
				if (target.isGrabbed)
				{
					((DistanceGrabber)target.grabbedBy).OffhandGrabbed(target);
				}
				this.m_grabbedObj = target;
				this.m_grabbedObj.GrabBegin(this, targetCollider);
				base.SetPlayerIgnoreCollision(this.m_grabbedObj.gameObject, true);
				this.m_movingObjectToHand = true;
				this.m_lastPos = base.transform.position;
				this.m_lastRot = base.transform.rotation;
				Vector3 a = targetCollider.ClosestPointOnBounds(this.m_gripTransform.position);
				if (!this.m_grabbedObj.snapPosition && !this.m_grabbedObj.snapOrientation && this.m_noSnapThreshhold > 0f && (a - this.m_gripTransform.position).magnitude < this.m_noSnapThreshhold)
				{
					Vector3 vector = this.m_grabbedObj.transform.position - base.transform.position;
					this.m_movingObjectToHand = false;
					vector = Quaternion.Inverse(base.transform.rotation) * vector;
					this.m_grabbedObjectPosOff = vector;
					Quaternion grabbedObjectRotOff = Quaternion.Inverse(base.transform.rotation) * this.m_grabbedObj.transform.rotation;
					this.m_grabbedObjectRotOff = grabbedObjectRotOff;
					return;
				}
				this.m_grabbedObjectPosOff = this.m_gripTransform.localPosition;
				if (this.m_grabbedObj.snapOffset)
				{
					Vector3 position = this.m_grabbedObj.snapOffset.position;
					if (this.m_controller == OVRInput.Controller.LTouch)
					{
						position.x = -position.x;
					}
					this.m_grabbedObjectPosOff += position;
				}
				this.m_grabbedObjectRotOff = this.m_gripTransform.localRotation;
				if (this.m_grabbedObj.snapOffset)
				{
					this.m_grabbedObjectRotOff = this.m_grabbedObj.snapOffset.rotation * this.m_grabbedObjectRotOff;
				}
			}
		}

		// Token: 0x06005263 RID: 21091 RVA: 0x00199768 File Offset: 0x00197968
		protected override void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
		{
			if (this.m_grabbedObj == null)
			{
				return;
			}
			Rigidbody grabbedRigidbody = this.m_grabbedObj.grabbedRigidbody;
			Vector3 vector = pos + rot * this.m_grabbedObjectPosOff;
			Quaternion quaternion = rot * this.m_grabbedObjectRotOff;
			if (this.m_movingObjectToHand)
			{
				float num = this.m_objectPullVelocity * Time.deltaTime;
				Vector3 a = vector - this.m_grabbedObj.transform.position;
				if (num * num * 1.1f > a.sqrMagnitude)
				{
					this.m_movingObjectToHand = false;
				}
				else
				{
					a.Normalize();
					vector = this.m_grabbedObj.transform.position + a * num;
					quaternion = Quaternion.RotateTowards(this.m_grabbedObj.transform.rotation, quaternion, this.m_objectPullMaxRotationRate * Time.deltaTime);
				}
			}
			grabbedRigidbody.MovePosition(vector);
			grabbedRigidbody.MoveRotation(quaternion);
		}

		// Token: 0x06005264 RID: 21092 RVA: 0x00199850 File Offset: 0x00197A50
		private static DistanceGrabbable HitInfoToGrabbable(RaycastHit hitInfo)
		{
			if (hitInfo.collider != null)
			{
				GameObject gameObject = hitInfo.collider.gameObject;
				return gameObject.GetComponent<DistanceGrabbable>() ?? gameObject.GetComponentInParent<DistanceGrabbable>();
			}
			return null;
		}

		// Token: 0x06005265 RID: 21093 RVA: 0x0019988C File Offset: 0x00197A8C
		protected bool FindTarget(out DistanceGrabbable dgOut, out Collider collOut)
		{
			dgOut = null;
			collOut = null;
			float num = float.MaxValue;
			foreach (OVRGrabbable ovrgrabbable in this.m_grabCandidates.Keys)
			{
				DistanceGrabbable distanceGrabbable = ovrgrabbable as DistanceGrabbable;
				bool flag = distanceGrabbable != null && distanceGrabbable.InRange && (!distanceGrabbable.isGrabbed || distanceGrabbable.allowOffhandGrab);
				if (flag && this.m_grabObjectsInLayer >= 0)
				{
					flag = (distanceGrabbable.gameObject.layer == this.m_grabObjectsInLayer);
				}
				if (flag)
				{
					for (int i = 0; i < distanceGrabbable.grabPoints.Length; i++)
					{
						Collider collider = distanceGrabbable.grabPoints[i];
						Vector3 b = collider.ClosestPointOnBounds(this.m_gripTransform.position);
						float sqrMagnitude = (this.m_gripTransform.position - b).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							bool flag2 = true;
							if (this.m_preventGrabThroughWalls)
							{
								Ray ray = default(Ray);
								ray.direction = distanceGrabbable.transform.position - this.m_gripTransform.position;
								ray.origin = this.m_gripTransform.position;
								Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);
								RaycastHit raycastHit;
								if (Physics.Raycast(ray, out raycastHit, this.m_maxGrabDistance, 1 << this.m_obstructionLayer, QueryTriggerInteraction.Ignore) && (double)(collider.ClosestPointOnBounds(this.m_gripTransform.position) - this.m_gripTransform.position).magnitude > (double)raycastHit.distance * 1.1)
								{
									flag2 = false;
								}
							}
							if (flag2)
							{
								num = sqrMagnitude;
								dgOut = distanceGrabbable;
								collOut = collider;
							}
						}
					}
				}
			}
			if (dgOut == null && this.m_useSpherecast)
			{
				return this.FindTargetWithSpherecast(out dgOut, out collOut);
			}
			return dgOut != null;
		}

		// Token: 0x06005266 RID: 21094 RVA: 0x00199AA8 File Offset: 0x00197CA8
		protected bool FindTargetWithSpherecast(out DistanceGrabbable dgOut, out Collider collOut)
		{
			dgOut = null;
			collOut = null;
			Ray ray = new Ray(this.m_gripTransform.position, this.m_gripTransform.forward);
			int layerMask = (this.m_grabObjectsInLayer == -1) ? -1 : (1 << this.m_grabObjectsInLayer);
			RaycastHit raycastHit;
			if (Physics.SphereCast(ray, this.m_spherecastRadius, out raycastHit, this.m_maxGrabDistance, layerMask))
			{
				DistanceGrabbable distanceGrabbable = null;
				Collider collider = null;
				if (raycastHit.collider != null)
				{
					distanceGrabbable = raycastHit.collider.gameObject.GetComponentInParent<DistanceGrabbable>();
					collider = ((distanceGrabbable == null) ? null : raycastHit.collider);
					if (distanceGrabbable)
					{
						dgOut = distanceGrabbable;
						collOut = collider;
					}
				}
				if (distanceGrabbable != null && this.m_preventGrabThroughWalls)
				{
					ray.direction = raycastHit.point - this.m_gripTransform.position;
					dgOut = distanceGrabbable;
					collOut = collider;
					RaycastHit raycastHit2;
					if (Physics.Raycast(ray, out raycastHit2, this.m_maxGrabDistance, 1 << this.m_obstructionLayer, QueryTriggerInteraction.Ignore))
					{
						DistanceGrabbable x = null;
						if (raycastHit.collider != null)
						{
							x = raycastHit2.collider.gameObject.GetComponentInParent<DistanceGrabbable>();
						}
						if (x != distanceGrabbable && raycastHit2.distance < raycastHit.distance)
						{
							dgOut = null;
							collOut = null;
						}
					}
				}
			}
			return dgOut != null;
		}

		// Token: 0x06005267 RID: 21095 RVA: 0x00199BF9 File Offset: 0x00197DF9
		protected override void GrabVolumeEnable(bool enabled)
		{
			if (this.m_useSpherecast)
			{
				enabled = false;
			}
			base.GrabVolumeEnable(enabled);
		}

		// Token: 0x06005268 RID: 21096 RVA: 0x00199C0D File Offset: 0x00197E0D
		protected override void OffhandGrabbed(OVRGrabbable grabbable)
		{
			base.OffhandGrabbed(grabbable);
		}

		// Token: 0x04005BCA RID: 23498
		[SerializeField]
		private float m_spherecastRadius;

		// Token: 0x04005BCB RID: 23499
		[SerializeField]
		private float m_noSnapThreshhold = 0.05f;

		// Token: 0x04005BCC RID: 23500
		[SerializeField]
		private bool m_useSpherecast;

		// Token: 0x04005BCD RID: 23501
		[SerializeField]
		public bool m_preventGrabThroughWalls;

		// Token: 0x04005BCE RID: 23502
		[SerializeField]
		private float m_objectPullVelocity = 10f;

		// Token: 0x04005BCF RID: 23503
		private float m_objectPullMaxRotationRate = 360f;

		// Token: 0x04005BD0 RID: 23504
		private bool m_movingObjectToHand;

		// Token: 0x04005BD1 RID: 23505
		[SerializeField]
		private float m_maxGrabDistance;

		// Token: 0x04005BD2 RID: 23506
		[SerializeField]
		private int m_grabObjectsInLayer;

		// Token: 0x04005BD3 RID: 23507
		[SerializeField]
		private int m_obstructionLayer;

		// Token: 0x04005BD4 RID: 23508
		private DistanceGrabber m_otherHand;

		// Token: 0x04005BD5 RID: 23509
		protected DistanceGrabbable m_target;

		// Token: 0x04005BD6 RID: 23510
		protected Collider m_targetCollider;
	}
}
