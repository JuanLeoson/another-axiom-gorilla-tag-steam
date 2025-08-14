using System;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E70 RID: 3696
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticTryOnNotifier : MonoBehaviour
	{
		// Token: 0x06005C6E RID: 23662 RVA: 0x001D13A0 File Offset: 0x001CF5A0
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this.m_vrrigCollection))
			{
				this.m_vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this.m_vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this.m_vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x06005C6F RID: 23663 RVA: 0x001D1418 File Offset: 0x001CF618
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(true, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			CosmeticsController.instance.TemporaryUnlock(playerRig.Rig, this.unlockList.Strings);
		}

		// Token: 0x06005C70 RID: 23664 RVA: 0x001D145C File Offset: 0x001CF65C
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(false, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			CosmeticsController.instance.ClearTemporaryUnlocks(playerRig.Rig, this.unlockList.Strings);
		}

		// Token: 0x04006617 RID: 26135
		private VRRigCollection m_vrrigCollection;

		// Token: 0x04006618 RID: 26136
		[SerializeField]
		private CosmeticTryOnNotifier.Mode mode;

		// Token: 0x04006619 RID: 26137
		[SerializeField]
		private StringList unlockList;

		// Token: 0x02000E71 RID: 3697
		private enum Mode
		{
			// Token: 0x0400661B RID: 26139
			TRY_ON,
			// Token: 0x0400661C RID: 26140
			ENABLE_LIST,
			// Token: 0x0400661D RID: 26141
			ENABLE_LIST_TITLEDATA
		}
	}
}
