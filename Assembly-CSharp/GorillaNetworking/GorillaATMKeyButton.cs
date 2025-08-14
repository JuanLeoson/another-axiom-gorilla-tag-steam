using System;

namespace GorillaNetworking
{
	// Token: 0x02000D66 RID: 3430
	public class GorillaATMKeyButton : GorillaKeyButton<GorillaATMKeyBindings>
	{
		// Token: 0x0600551B RID: 21787 RVA: 0x001A6229 File Offset: 0x001A4429
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaATMKeyButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
