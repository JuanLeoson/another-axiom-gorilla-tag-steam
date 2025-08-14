using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E6F RID: 3695
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticCameraDisableNotifier : MonoBehaviour
	{
		// Token: 0x06005C6A RID: 23658 RVA: 0x001D12F4 File Offset: 0x001CF4F4
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this._vrrigCollection))
			{
				this._vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this._vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this._vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x06005C6B RID: 23659 RVA: 0x001D1369 File Offset: 0x001CF569
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = false;
			}
		}

		// Token: 0x06005C6C RID: 23660 RVA: 0x001D1384 File Offset: 0x001CF584
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = true;
			}
		}

		// Token: 0x04006615 RID: 26133
		private VRRigCollection _vrrigCollection;

		// Token: 0x04006616 RID: 26134
		[SerializeField]
		private Camera _cosmeticCamera;
	}
}
