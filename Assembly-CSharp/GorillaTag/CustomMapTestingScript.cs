using System;
using System.Collections;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E89 RID: 3721
	public class CustomMapTestingScript : GorillaPressableButton
	{
		// Token: 0x06005D3A RID: 23866 RVA: 0x001D7A6C File Offset: 0x001D5C6C
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x001D7A81 File Offset: 0x001D5C81
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}
	}
}
