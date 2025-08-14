using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000BE8 RID: 3048
	[HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
	public class RigOwnedTransformView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060049F8 RID: 18936 RVA: 0x0016776F File Offset: 0x0016596F
		// (set) Token: 0x060049F9 RID: 18937 RVA: 0x00167777 File Offset: 0x00165977
		public bool IsMine { get; private set; }

		// Token: 0x060049FA RID: 18938 RVA: 0x00167780 File Offset: 0x00165980
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x060049FB RID: 18939 RVA: 0x00167789 File Offset: 0x00165989
		public void Awake()
		{
			this.m_StoredPosition = base.transform.localPosition;
			this.m_NetworkPosition = Vector3.zero;
			this.m_networkScale = Vector3.one;
			this.m_NetworkRotation = Quaternion.identity;
		}

		// Token: 0x060049FC RID: 18940 RVA: 0x001677BD File Offset: 0x001659BD
		private void Reset()
		{
			this.m_UseLocal = true;
		}

		// Token: 0x060049FD RID: 18941 RVA: 0x001677C6 File Offset: 0x001659C6
		private void OnEnable()
		{
			this.m_firstTake = true;
		}

		// Token: 0x060049FE RID: 18942 RVA: 0x001677D0 File Offset: 0x001659D0
		public void Update()
		{
			Transform transform = base.transform;
			if (!this.IsMine && this.IsValid(this.m_NetworkPosition) && this.IsValid(this.m_NetworkRotation))
			{
				if (this.m_UseLocal)
				{
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					return;
				}
				transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
			}
		}

		// Token: 0x060049FF RID: 18943 RVA: 0x001678C4 File Offset: 0x00165AC4
		private bool IsValid(Vector3 v)
		{
			return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
		}

		// Token: 0x06004A00 RID: 18944 RVA: 0x00167924 File Offset: 0x00165B24
		private bool IsValid(Quaternion q)
		{
			return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) && !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
		}

		// Token: 0x06004A01 RID: 18945 RVA: 0x0016799C File Offset: 0x00165B9C
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			Transform transform = base.transform;
			if (stream.IsWriting)
			{
				if (this.m_SynchronizePosition)
				{
					if (this.m_UseLocal)
					{
						this.m_Direction = transform.localPosition - this.m_StoredPosition;
						this.m_StoredPosition = transform.localPosition;
						stream.SendNext(transform.localPosition);
						stream.SendNext(this.m_Direction);
					}
					else
					{
						this.m_Direction = transform.position - this.m_StoredPosition;
						this.m_StoredPosition = transform.position;
						stream.SendNext(transform.position);
						stream.SendNext(this.m_Direction);
					}
				}
				if (this.m_SynchronizeRotation)
				{
					if (this.m_UseLocal)
					{
						stream.SendNext(transform.localRotation);
					}
					else
					{
						stream.SendNext(transform.rotation);
					}
				}
				if (this.m_SynchronizeScale)
				{
					stream.SendNext(transform.localScale);
					return;
				}
			}
			else
			{
				if (this.m_SynchronizePosition)
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					ref this.m_NetworkPosition.SetValueSafe(vector);
					vector = (Vector3)stream.ReceiveNext();
					ref this.m_Direction.SetValueSafe(vector);
					if (this.m_firstTake)
					{
						if (this.m_UseLocal)
						{
							transform.localPosition = this.m_NetworkPosition;
						}
						else
						{
							transform.position = this.m_NetworkPosition;
						}
						this.m_Distance = 0f;
					}
					else
					{
						float d = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
						this.m_NetworkPosition += this.m_Direction * d;
						if (this.m_UseLocal)
						{
							this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
						}
						else
						{
							this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
						}
					}
				}
				if (this.m_SynchronizeRotation)
				{
					Quaternion quaternion = (Quaternion)stream.ReceiveNext();
					ref this.m_NetworkRotation.SetValueSafe(quaternion);
					if (this.m_firstTake)
					{
						this.m_Angle = 0f;
						if (this.m_UseLocal)
						{
							transform.localRotation = this.m_NetworkRotation;
						}
						else
						{
							transform.rotation = this.m_NetworkRotation;
						}
					}
					else if (this.m_UseLocal)
					{
						this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
					}
					else
					{
						this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
					}
				}
				if (this.m_SynchronizeScale)
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					ref this.m_networkScale.SetValueSafe(vector);
					transform.localScale = this.m_networkScale;
				}
				if (this.m_firstTake)
				{
					this.m_firstTake = false;
				}
			}
		}

		// Token: 0x06004A02 RID: 18946 RVA: 0x001677C6 File Offset: 0x001659C6
		public void GTAddition_DoTeleport()
		{
			this.m_firstTake = true;
		}

		// Token: 0x040052BF RID: 21183
		private float m_Distance;

		// Token: 0x040052C0 RID: 21184
		private float m_Angle;

		// Token: 0x040052C1 RID: 21185
		private Vector3 m_Direction;

		// Token: 0x040052C2 RID: 21186
		private Vector3 m_NetworkPosition;

		// Token: 0x040052C3 RID: 21187
		private Vector3 m_StoredPosition;

		// Token: 0x040052C4 RID: 21188
		private Vector3 m_networkScale;

		// Token: 0x040052C5 RID: 21189
		private Quaternion m_NetworkRotation;

		// Token: 0x040052C6 RID: 21190
		public bool m_SynchronizePosition = true;

		// Token: 0x040052C7 RID: 21191
		public bool m_SynchronizeRotation = true;

		// Token: 0x040052C8 RID: 21192
		public bool m_SynchronizeScale;

		// Token: 0x040052C9 RID: 21193
		[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
		public bool m_UseLocal;

		// Token: 0x040052CA RID: 21194
		private bool m_firstTake;
	}
}
