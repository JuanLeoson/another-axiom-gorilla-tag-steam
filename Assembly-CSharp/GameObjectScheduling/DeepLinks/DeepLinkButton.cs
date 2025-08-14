using System;
using System.Collections;
using UnityEngine;

namespace GameObjectScheduling.DeepLinks
{
	// Token: 0x02000F9F RID: 3999
	public class DeepLinkButton : GorillaPressableButton
	{
		// Token: 0x060063E5 RID: 25573 RVA: 0x001F6C4E File Offset: 0x001F4E4E
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			this.sendingDeepLink = DeepLinkSender.SendDeepLink(this.deepLinkAppID, this.deepLinkPayload, new Action<string>(this.OnDeepLinkSent));
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x060063E6 RID: 25574 RVA: 0x001F6C86 File Offset: 0x001F4E86
		private void OnDeepLinkSent(string message)
		{
			this.sendingDeepLink = false;
			if (!this.isOn)
			{
				this.UpdateColor();
			}
		}

		// Token: 0x060063E7 RID: 25575 RVA: 0x001F6C9D File Offset: 0x001F4E9D
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.pressedTime);
			this.isOn = false;
			if (!this.sendingDeepLink)
			{
				this.UpdateColor();
			}
			yield break;
		}

		// Token: 0x04006EBF RID: 28351
		[SerializeField]
		private ulong deepLinkAppID;

		// Token: 0x04006EC0 RID: 28352
		[SerializeField]
		private string deepLinkPayload = "";

		// Token: 0x04006EC1 RID: 28353
		[SerializeField]
		private float pressedTime = 0.2f;

		// Token: 0x04006EC2 RID: 28354
		private bool sendingDeepLink;
	}
}
