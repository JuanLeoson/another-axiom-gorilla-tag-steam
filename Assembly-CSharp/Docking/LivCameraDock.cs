using System;
using Liv.Lck.GorillaTag;

namespace Docking
{
	// Token: 0x02000F89 RID: 3977
	public class LivCameraDock : Dock
	{
		// Token: 0x0600637F RID: 25471 RVA: 0x001F5A47 File Offset: 0x001F3C47
		private void Reset()
		{
			this.cameraSettings.fov = 80f;
		}

		// Token: 0x06006380 RID: 25472 RVA: 0x001F5A5C File Offset: 0x001F3C5C
		private void OnValidate()
		{
			if (this.cameraSettings.forceFov && (this.cameraSettings.fov < 30f || this.cameraSettings.fov > 110f))
			{
				this.cameraSettings.fov = 80f;
			}
		}

		// Token: 0x04006E79 RID: 28281
		public GtCameraDockSettings cameraSettings;
	}
}
