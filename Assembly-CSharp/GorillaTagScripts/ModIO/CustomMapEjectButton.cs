using System;
using System.Collections;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000C59 RID: 3161
	public class CustomMapEjectButton : GorillaPressableButton
	{
		// Token: 0x06004E39 RID: 20025 RVA: 0x00184F3B File Offset: 0x0018313B
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
			if (!this.processing)
			{
				this.HandleTeleport();
			}
		}

		// Token: 0x06004E3A RID: 20026 RVA: 0x00184F5E File Offset: 0x0018315E
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}

		// Token: 0x06004E3B RID: 20027 RVA: 0x00184F70 File Offset: 0x00183170
		private void HandleTeleport()
		{
			if (this.processing)
			{
				return;
			}
			this.processing = true;
			CustomMapEjectButton.EjectType ejectType = this.ejectType;
			if (ejectType != CustomMapEjectButton.EjectType.EjectFromVirtualStump)
			{
				if (ejectType == CustomMapEjectButton.EjectType.ReturnToVirtualStump)
				{
					CustomMapManager.ReturnToVirtualStump();
					this.processing = false;
					return;
				}
			}
			else
			{
				CustomMapManager.ExitVirtualStump(new Action<bool>(this.FinishTeleport));
			}
		}

		// Token: 0x06004E3C RID: 20028 RVA: 0x00184FB9 File Offset: 0x001831B9
		private void FinishTeleport(bool success = true)
		{
			if (!this.processing)
			{
				return;
			}
			this.processing = false;
		}

		// Token: 0x06004E3D RID: 20029 RVA: 0x00184FCB File Offset: 0x001831CB
		public void CopySettings(CustomMapEjectButtonSettings customMapEjectButtonSettings)
		{
			this.ejectType = (CustomMapEjectButton.EjectType)customMapEjectButtonSettings.ejectType;
		}

		// Token: 0x04005725 RID: 22309
		[SerializeField]
		private CustomMapEjectButton.EjectType ejectType;

		// Token: 0x04005726 RID: 22310
		private bool processing;

		// Token: 0x02000C5A RID: 3162
		public enum EjectType
		{
			// Token: 0x04005728 RID: 22312
			EjectFromVirtualStump,
			// Token: 0x04005729 RID: 22313
			ReturnToVirtualStump
		}
	}
}
