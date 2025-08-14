using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C28 RID: 3112
	public class DecorativeItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06004C88 RID: 19592 RVA: 0x0017B6D8 File Offset: 0x001798D8
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.isSnapped);
				stream.SendNext(this.snapPosition);
				stream.SendNext(this.respawnPosition);
				stream.SendNext(this.respawnRotation);
				return;
			}
			this.isSnapped = (bool)stream.ReceiveNext();
			this.snapPosition = (Vector3)stream.ReceiveNext();
			this.respawnPosition = (Vector3)stream.ReceiveNext();
			this.respawnRotation = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!this.snapPosition.IsValid(num))
			{
				this.snapPosition = Vector3.zero;
			}
			num = 10000f;
			if (!this.respawnPosition.IsValid(num))
			{
				this.respawnPosition = Vector3.zero;
			}
			if (!this.respawnRotation.IsValid())
			{
				this.respawnRotation = quaternion.identity;
			}
		}

		// Token: 0x040055A6 RID: 21926
		public bool isSnapped;

		// Token: 0x040055A7 RID: 21927
		public Vector3 snapPosition = Vector3.zero;

		// Token: 0x040055A8 RID: 21928
		public Vector3 respawnPosition = Vector3.zero;

		// Token: 0x040055A9 RID: 21929
		public Quaternion respawnRotation = Quaternion.identity;
	}
}
