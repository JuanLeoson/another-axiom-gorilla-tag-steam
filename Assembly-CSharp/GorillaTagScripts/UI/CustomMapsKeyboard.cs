using System;

namespace GorillaTagScripts.UI
{
	// Token: 0x02000C70 RID: 3184
	public class CustomMapsKeyboard : GorillaKeyWrapper<CustomMapKeyboardBinding>
	{
		// Token: 0x06004ED0 RID: 20176 RVA: 0x001885BE File Offset: 0x001867BE
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			return CustomMapsKeyButton.BindingToString(binding);
		}
	}
}
