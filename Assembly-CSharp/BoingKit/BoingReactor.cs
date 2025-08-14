using System;

namespace BoingKit
{
	// Token: 0x02000FDD RID: 4061
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x0600657E RID: 25982 RVA: 0x00201FED File Offset: 0x002001ED
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x0600657F RID: 25983 RVA: 0x00201FF5 File Offset: 0x002001F5
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06006580 RID: 25984 RVA: 0x00201FFD File Offset: 0x002001FD
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
