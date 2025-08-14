using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000E8D RID: 3725
	[Serializable]
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x06005D53 RID: 23891 RVA: 0x001D7A22 File Offset: 0x001D5C22
		public TickSystemTimer()
		{
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x001D7BEB File Offset: 0x001D5DEB
		public TickSystemTimer(float cd) : base(cd)
		{
		}

		// Token: 0x06005D55 RID: 23893 RVA: 0x001D7BF4 File Offset: 0x001D5DF4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnTimedEvent()
		{
			Action action = this.callback;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x04006759 RID: 26457
		public Action callback;
	}
}
