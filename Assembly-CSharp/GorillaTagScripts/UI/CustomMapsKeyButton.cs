using System;

namespace GorillaTagScripts.UI
{
	// Token: 0x02000C72 RID: 3186
	public class CustomMapsKeyButton : GorillaKeyButton<CustomMapKeyboardBinding>
	{
		// Token: 0x06004ED2 RID: 20178 RVA: 0x001885D0 File Offset: 0x001867D0
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			if (binding < CustomMapKeyboardBinding.up || (binding > CustomMapKeyboardBinding.option3 && binding < CustomMapKeyboardBinding.at))
			{
				if (binding >= CustomMapKeyboardBinding.up)
				{
					return binding.ToString();
				}
				int num = (int)binding;
				return num.ToString();
			}
			else
			{
				switch (binding)
				{
				case CustomMapKeyboardBinding.at:
					return "@";
				case CustomMapKeyboardBinding.dash:
					return "-";
				case CustomMapKeyboardBinding.period:
					return ".";
				case CustomMapKeyboardBinding.underscore:
					return "_";
				case CustomMapKeyboardBinding.plus:
					return "+";
				case CustomMapKeyboardBinding.space:
					return " ";
				default:
					return "";
				}
			}
		}

		// Token: 0x06004ED3 RID: 20179 RVA: 0x000023F5 File Offset: 0x000005F5
		protected override void OnButtonPressedEvent()
		{
		}
	}
}
