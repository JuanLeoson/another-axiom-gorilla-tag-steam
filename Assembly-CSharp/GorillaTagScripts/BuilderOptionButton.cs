using System;

namespace GorillaTagScripts
{
	// Token: 0x02000C04 RID: 3076
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x06004AE9 RID: 19177 RVA: 0x0016C0A7 File Offset: 0x0016A2A7
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06004AEA RID: 19178 RVA: 0x000023F5 File Offset: 0x000005F5
		private void OnDestroy()
		{
		}

		// Token: 0x06004AEB RID: 19179 RVA: 0x0016C0AF File Offset: 0x0016A2AF
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x06004AEC RID: 19180 RVA: 0x0016C0B8 File Offset: 0x0016A2B8
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action(this, isLeftHand);
		}

		// Token: 0x06004AED RID: 19181 RVA: 0x0016C0CC File Offset: 0x0016A2CC
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x040053D9 RID: 21465
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
