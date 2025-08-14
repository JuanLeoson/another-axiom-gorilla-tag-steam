using System;

namespace GorillaNetworking
{
	// Token: 0x02000D75 RID: 3445
	public class GorillaKeyboardButton : GorillaKeyButton<GorillaKeyboardBindings>
	{
		// Token: 0x060055E9 RID: 21993 RVA: 0x001AAEE2 File Offset: 0x001A90E2
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
