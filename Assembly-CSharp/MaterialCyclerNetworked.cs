using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020002A2 RID: 674
[RequireComponent(typeof(PhotonView))]
public class MaterialCyclerNetworked : MonoBehaviour
{
	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000F8B RID: 3979 RVA: 0x0005B7DF File Offset: 0x000599DF
	public float SyncTimeOut
	{
		get
		{
			return this.syncTimeOut;
		}
	}

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06000F8C RID: 3980 RVA: 0x0005B7E8 File Offset: 0x000599E8
	// (remove) Token: 0x06000F8D RID: 3981 RVA: 0x0005B820 File Offset: 0x00059A20
	public event Action<int, int3> OnSynchronize;

	// Token: 0x06000F8E RID: 3982 RVA: 0x0005B855 File Offset: 0x00059A55
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0005B864 File Offset: 0x00059A64
	public void Synchronize(int materialIndex, Color c)
	{
		if (!this.masterClientOnly || PhotonNetwork.IsMasterClient)
		{
			int num = Mathf.CeilToInt(c.r * 9f);
			int num2 = Mathf.CeilToInt(c.g * 9f);
			int num3 = Mathf.CeilToInt(c.b * 9f);
			int num4 = num | num2 << 8 | num3 << 16;
			this.photonView.RPC("RPC_SynchronizePacked", RpcTarget.Others, new object[]
			{
				materialIndex,
				num4
			});
		}
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0005B8E8 File Offset: 0x00059AE8
	[PunRPC]
	public void RPC_SynchronizePacked(int index, int colourPacked, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPC_SynchronizePacked");
		RigContainer rigContainer;
		if (this.OnSynchronize == null || (this.masterClientOnly && !info.Sender.IsMasterClient) || !VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.IsPositionInRange(base.transform.position, 5f) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 21, info.SentServerTime))
		{
			return;
		}
		int num = colourPacked & 255;
		int num2 = colourPacked >> 8 & 255;
		int num3 = colourPacked >> 16 & 255;
		num = Mathf.Clamp(num, 0, 9);
		num2 = Mathf.Clamp(num2, 0, 9);
		num3 = Mathf.Clamp(num3, 0, 9);
		this.OnSynchronize(index, new int3(num, num2, num3));
	}

	// Token: 0x04001836 RID: 6198
	[SerializeField]
	private float syncTimeOut = 1f;

	// Token: 0x04001837 RID: 6199
	private PhotonView photonView;

	// Token: 0x04001838 RID: 6200
	[SerializeField]
	private bool masterClientOnly;
}
