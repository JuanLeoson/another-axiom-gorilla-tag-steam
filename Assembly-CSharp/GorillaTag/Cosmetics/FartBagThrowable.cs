using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F41 RID: 3905
	public class FartBagThrowable : MonoBehaviour, IProjectile
	{
		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x060060BD RID: 24765 RVA: 0x001ECDF5 File Offset: 0x001EAFF5
		// (set) Token: 0x060060BE RID: 24766 RVA: 0x001ECDFD File Offset: 0x001EAFFD
		public TransferrableObject ParentTransferable { get; set; }

		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x060060BF RID: 24767 RVA: 0x001ECE08 File Offset: 0x001EB008
		// (remove) Token: 0x060060C0 RID: 24768 RVA: 0x001ECE40 File Offset: 0x001EB040
		public event Action<IProjectile> OnDeflated;

		// Token: 0x060060C1 RID: 24769 RVA: 0x001ECE78 File Offset: 0x001EB078
		private void OnEnable()
		{
			this.placedOnFloor = false;
			this.deflated = false;
			this.handContactPoint = Vector3.negativeInfinity;
			this.handNormalVector = Vector3.zero;
			this.timeCreated = float.PositiveInfinity;
			this.placedOnFloorTime = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.ResetBlend();
			}
		}

		// Token: 0x060060C2 RID: 24770 RVA: 0x001ECED7 File Offset: 0x001EB0D7
		private void Update()
		{
			if (Time.time - this.timeCreated > this.forceDestroyAfterSec)
			{
				this.DeflateLocal();
			}
		}

		// Token: 0x060060C3 RID: 24771 RVA: 0x001ECEF4 File Offset: 0x001EB0F4
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rigidbody.velocity = velocity;
			this.timeCreated = Time.time;
			this.InitialPhotonEvent();
		}

		// Token: 0x060060C4 RID: 24772 RVA: 0x001ECF54 File Offset: 0x001EB154
		private void InitialPhotonEvent()
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			if (this.ParentTransferable)
			{
				NetPlayer netPlayer = (this.ParentTransferable.myOnlineRig != null) ? this.ParentTransferable.myOnlineRig.creator : ((this.ParentTransferable.myRig != null) ? (this.ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (this._events != null && netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.DeflateEvent;
			}
		}

		// Token: 0x060060C5 RID: 24773 RVA: 0x001ED028 File Offset: 0x001EB228
		private void OnTriggerEnter(Collider other)
		{
			if ((this.handLayerMask.value & 1 << other.gameObject.layer) != 0)
			{
				if (!this.placedOnFloor)
				{
					return;
				}
				this.handContactPoint = other.ClosestPoint(base.transform.position);
				this.handNormalVector = (this.handContactPoint - base.transform.position).normalized;
				if (Time.time - this.placedOnFloorTime > 0.3f)
				{
					this.Deflate();
				}
			}
		}

		// Token: 0x060060C6 RID: 24774 RVA: 0x001ED0B0 File Offset: 0x001EB2B0
		private void OnCollisionEnter(Collision other)
		{
			if ((this.floorLayerMask.value & 1 << other.gameObject.layer) != 0)
			{
				this.placedOnFloor = true;
				this.placedOnFloorTime = Time.time;
				Vector3 normal = other.contacts[0].normal;
				base.transform.position = other.contacts[0].point + normal * this.placementOffset;
				Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
				base.transform.rotation = rotation;
			}
		}

		// Token: 0x060060C7 RID: 24775 RVA: 0x001ED158 File Offset: 0x001EB358
		private void Deflate()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					this.handContactPoint,
					this.handNormalVector
				});
			}
			this.DeflateLocal();
		}

		// Token: 0x060060C8 RID: 24776 RVA: 0x001ED1C8 File Offset: 0x001EB3C8
		private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "DeflateEvent");
			if (this.callLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 position = (Vector3)obj;
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						float num = 10000f;
						if (!vector.IsValid(num))
						{
							return;
						}
						num = 10000f;
						if (!position.IsValid(num) || !this.ParentTransferable.targetRig.IsPositionInRange(position, 4f))
						{
							return;
						}
						this.handNormalVector = vector;
						this.handContactPoint = position;
						this.DeflateLocal();
						return;
					}
				}
			}
		}

		// Token: 0x060060C9 RID: 24777 RVA: 0x001ED278 File Offset: 0x001EB478
		private void DeflateLocal()
		{
			if (this.deflated)
			{
				return;
			}
			GameObject gameObject = ObjectPools.instance.Instantiate(this.deflationEffect, this.handContactPoint, true);
			gameObject.transform.up = this.handNormalVector;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
			if (componentInChildren.soundBank)
			{
				componentInChildren.Play();
			}
			this.placedOnFloor = false;
			this.timeCreated = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.FullyBlend();
			}
			this.deflated = true;
			base.Invoke("DisableObject", this.destroyWhenDeflateDelay);
		}

		// Token: 0x060060CA RID: 24778 RVA: 0x001ED327 File Offset: 0x001EB527
		private void DisableObject()
		{
			Action<IProjectile> onDeflated = this.OnDeflated;
			if (onDeflated != null)
			{
				onDeflated(this);
			}
			this.deflated = false;
		}

		// Token: 0x060060CB RID: 24779 RVA: 0x001ED344 File Offset: 0x001EB544
		private void OnDestroy()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.DeflateEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x04006CA8 RID: 27816
		[SerializeField]
		private GameObject deflationEffect;

		// Token: 0x04006CA9 RID: 27817
		[SerializeField]
		private float destroyWhenDeflateDelay = 3f;

		// Token: 0x04006CAA RID: 27818
		[SerializeField]
		private float forceDestroyAfterSec = 10f;

		// Token: 0x04006CAB RID: 27819
		[SerializeField]
		private float placementOffset = 0.2f;

		// Token: 0x04006CAC RID: 27820
		[SerializeField]
		private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;

		// Token: 0x04006CAD RID: 27821
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x04006CAE RID: 27822
		[SerializeField]
		private LayerMask handLayerMask;

		// Token: 0x04006CAF RID: 27823
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x04006CB0 RID: 27824
		private bool placedOnFloor;

		// Token: 0x04006CB1 RID: 27825
		private float placedOnFloorTime;

		// Token: 0x04006CB2 RID: 27826
		private float timeCreated;

		// Token: 0x04006CB3 RID: 27827
		private bool deflated;

		// Token: 0x04006CB4 RID: 27828
		private Vector3 handContactPoint;

		// Token: 0x04006CB5 RID: 27829
		private Vector3 handNormalVector;

		// Token: 0x04006CB6 RID: 27830
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04006CB9 RID: 27833
		private RubberDuckEvents _events;
	}
}
