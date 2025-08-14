using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000CA8 RID: 3240
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x0600506B RID: 20587 RVA: 0x00191771 File Offset: 0x0018F971
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
