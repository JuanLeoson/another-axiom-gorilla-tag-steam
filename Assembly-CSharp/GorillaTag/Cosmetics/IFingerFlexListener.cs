using System;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F47 RID: 3911
	public interface IFingerFlexListener
	{
		// Token: 0x060060E0 RID: 24800 RVA: 0x0001D558 File Offset: 0x0001B758
		bool FingerFlexValidation(bool isLeftHand)
		{
			return true;
		}

		// Token: 0x060060E1 RID: 24801
		void OnButtonPressed(bool isLeftHand, float value);

		// Token: 0x060060E2 RID: 24802
		void OnButtonReleased(bool isLeftHand, float value);

		// Token: 0x060060E3 RID: 24803
		void OnButtonPressStayed(bool isLeftHand, float value);

		// Token: 0x02000F48 RID: 3912
		public enum ComponentActivator
		{
			// Token: 0x04006CE0 RID: 27872
			FingerReleased,
			// Token: 0x04006CE1 RID: 27873
			FingerFlexed,
			// Token: 0x04006CE2 RID: 27874
			FingerStayed
		}
	}
}
