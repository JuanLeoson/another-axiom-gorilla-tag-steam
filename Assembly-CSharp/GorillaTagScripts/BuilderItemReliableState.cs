using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C03 RID: 3075
	public class BuilderItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06004AE7 RID: 19175 RVA: 0x0016BF54 File Offset: 0x0016A154
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.rightHandAttachPos);
				stream.SendNext(this.rightHandAttachRot);
				stream.SendNext(this.leftHandAttachPos);
				stream.SendNext(this.leftHandAttachRot);
				return;
			}
			this.rightHandAttachPos = (Vector3)stream.ReceiveNext();
			this.rightHandAttachRot = (Quaternion)stream.ReceiveNext();
			this.leftHandAttachPos = (Vector3)stream.ReceiveNext();
			this.leftHandAttachRot = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!this.rightHandAttachPos.IsValid(num))
			{
				this.rightHandAttachPos = Vector3.zero;
			}
			if (!this.rightHandAttachRot.IsValid())
			{
				this.rightHandAttachRot = quaternion.identity;
			}
			num = 10000f;
			if (!this.leftHandAttachPos.IsValid(num))
			{
				this.leftHandAttachPos = Vector3.zero;
			}
			if (!this.leftHandAttachRot.IsValid())
			{
				this.leftHandAttachRot = quaternion.identity;
			}
			this.dirty = true;
		}

		// Token: 0x040053D4 RID: 21460
		public Vector3 rightHandAttachPos = Vector3.zero;

		// Token: 0x040053D5 RID: 21461
		public Quaternion rightHandAttachRot = Quaternion.identity;

		// Token: 0x040053D6 RID: 21462
		public Vector3 leftHandAttachPos = Vector3.zero;

		// Token: 0x040053D7 RID: 21463
		public Quaternion leftHandAttachRot = Quaternion.identity;

		// Token: 0x040053D8 RID: 21464
		public bool dirty;
	}
}
