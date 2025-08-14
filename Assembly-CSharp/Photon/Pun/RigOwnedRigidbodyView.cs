using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000BE7 RID: 3047
	[RequireComponent(typeof(Rigidbody))]
	public class RigOwnedRigidbodyView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060049F1 RID: 18929 RVA: 0x00167444 File Offset: 0x00165644
		// (set) Token: 0x060049F2 RID: 18930 RVA: 0x0016744C File Offset: 0x0016564C
		public bool IsMine { get; private set; }

		// Token: 0x060049F3 RID: 18931 RVA: 0x00167455 File Offset: 0x00165655
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x060049F4 RID: 18932 RVA: 0x0016745E File Offset: 0x0016565E
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody>();
			this.m_NetworkPosition = default(Vector3);
			this.m_NetworkRotation = default(Quaternion);
		}

		// Token: 0x060049F5 RID: 18933 RVA: 0x00167484 File Offset: 0x00165684
		public void FixedUpdate()
		{
			if (!this.IsMine)
			{
				this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
			}
		}

		// Token: 0x060049F6 RID: 18934 RVA: 0x00167504 File Offset: 0x00165704
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.m_Body.position);
				stream.SendNext(this.m_Body.rotation);
				if (this.m_SynchronizeVelocity)
				{
					stream.SendNext(this.m_Body.velocity);
				}
				if (this.m_SynchronizeAngularVelocity)
				{
					stream.SendNext(this.m_Body.angularVelocity);
				}
				stream.SendNext(this.m_Body.IsSleeping());
				return;
			}
			Vector3 vector = (Vector3)stream.ReceiveNext();
			ref this.m_NetworkPosition.SetValueSafe(vector);
			Quaternion quaternion = (Quaternion)stream.ReceiveNext();
			ref this.m_NetworkRotation.SetValueSafe(quaternion);
			if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
			{
				this.m_Body.position = this.m_NetworkPosition;
			}
			if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
			{
				float d = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
				if (this.m_SynchronizeVelocity)
				{
					Vector3 velocity = (Vector3)stream.ReceiveNext();
					float num = 10000f;
					if (!velocity.IsValid(num))
					{
						velocity = Vector3.zero;
					}
					if (!this.m_Body.isKinematic)
					{
						this.m_Body.velocity = velocity;
					}
					this.m_NetworkPosition += this.m_Body.velocity * d;
					this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
				}
				if (this.m_SynchronizeAngularVelocity)
				{
					Vector3 angularVelocity = (Vector3)stream.ReceiveNext();
					float num = 10000f;
					if (!angularVelocity.IsValid(num))
					{
						angularVelocity = Vector3.zero;
					}
					this.m_Body.angularVelocity = angularVelocity;
					this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * d) * this.m_NetworkRotation;
					this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
				}
			}
			if ((bool)stream.ReceiveNext())
			{
				this.m_Body.Sleep();
			}
		}

		// Token: 0x040052B5 RID: 21173
		private float m_Distance;

		// Token: 0x040052B6 RID: 21174
		private float m_Angle;

		// Token: 0x040052B7 RID: 21175
		private Rigidbody m_Body;

		// Token: 0x040052B8 RID: 21176
		private Vector3 m_NetworkPosition;

		// Token: 0x040052B9 RID: 21177
		private Quaternion m_NetworkRotation;

		// Token: 0x040052BA RID: 21178
		public bool m_SynchronizeVelocity = true;

		// Token: 0x040052BB RID: 21179
		public bool m_SynchronizeAngularVelocity;

		// Token: 0x040052BC RID: 21180
		public bool m_TeleportEnabled;

		// Token: 0x040052BD RID: 21181
		public float m_TeleportIfDistanceGreaterThan = 3f;
	}
}
